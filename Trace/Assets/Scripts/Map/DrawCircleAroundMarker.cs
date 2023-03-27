using System;
using Unity;
using UnityEngine;

public class DrawCircleAroundMarker : MonoBehaviour{
    /// <summary>
    /// Radius of the circle
    /// </summary>
    public ColorPaternForCircles[] colorPaterns;
    public float radiusKM = 0.1f;
    
    public int numOfCircles = 0;
    private void Awake()
    {
        numOfCircles = 0;
    }

    /// <summary>
    /// Number of segments
    /// </summary>
    public int segments = 64;
    public Vector2[] circlePoint;
    public bool drawCircle;

    int GetColorArrayIndex() {
        if (radiusKM <= 0.1f)
        {
            return 0;
        }
        else if (radiusKM > 0.1f && radiusKM <= 0.2f)
            return 1;
        else if (radiusKM > 0.2f && radiusKM <= 0.3f)
            return 2;
        else if (radiusKM > 0.3f && radiusKM <= 0.4f)
            return 3;
        else if (radiusKM > 0.4f && radiusKM <= 1f)
            return 4;
        else
            return 5;
    }
    /// <summary>
    /// This method is called when a user clicks on a map
    /// </summary>
    /// 
    private void DrawCircle()
    {
        if(Input.GetKeyDown("space") && numOfCircles  < 7)
        {
            numOfCircles++;
            // Get the coordinates under cursor
            double lng, lat;
            OnlineMapsControlBase.instance.GetCoords(out lng, out lat);
            Debug.Log("Lat is " + lat + "Long is " + lng);

            // Create a new marker under cursor
            OnlineMapsMarkerManager.CreateItem(lng, lat, new Texture2D(10, 10), "marker number" + OnlineMapsMarkerManager.CountItems);
            OnlineMaps map = OnlineMaps.instance;

            // Get the coordinate at the desired distance
            double nlng, nlat;
            OnlineMapsUtils.GetCoordinateInDistance(lng, lat, radiusKM, 90, out nlng, out nlat);

            double tx1, ty1, tx2, ty2;

            // Convert the coordinate under cursor to tile position
            map.projection.CoordinatesToTile(lng, lat, 20, out tx1, out ty1);

            // Convert remote coordinate to tile position
            map.projection.CoordinatesToTile(nlng, nlat, 20, out tx2, out ty2);

            // Calculate radius in tiles
            double r = tx2 - tx1;

            // Create a new array for points
            OnlineMapsVector2d[] points = new OnlineMapsVector2d[segments];

            // Calculate a step
            double step = 360d / segments;

            // Calculate each point of circle
            for (int i = 0; i < segments; i++)
            {
                double px = tx1 + Math.Cos(step * i * OnlineMapsUtils.Deg2Rad) * r;
                double py = ty1 + Math.Sin(step * i * OnlineMapsUtils.Deg2Rad) * r;
                map.projection.TileToCoordinates(px, py, 20, out lng, out lat);
                points[i] = new OnlineMapsVector2d(lng, lat);
            }

            // Create a new polygon to draw a circle
            OnlineMapsDrawingElement poly = OnlineMapsDrawingElementManager.AddItem
            (new OnlineMapsDrawingPoly(points, colorPaterns[GetColorArrayIndex()].borderColor,
            colorPaterns[GetColorArrayIndex()].borderSize, colorPaterns[GetColorArrayIndex()].bgColor));
            poly.OnClick += OnCircleClick;
        }
    }
    /// <summary>
    /// This method is called when the script starts
    /// </summary>
    private void Start()
    {
        // Subscribe to click on map event
    }

    private void Update()
    {
        if(drawCircle)
        DrawCircle();
    }

    private void OnCircleClick(OnlineMapsDrawingElement element)
    {
        OnlineMapsDrawingPoly poly = element as OnlineMapsDrawingPoly;

        circlePoint = poly.points as Vector2[];
        Debug.Log(poly.name);

        if(circlePoint == null || circlePoint.Length == 0)
        Debug.LogError("Cannot See The Circle Points!!!");
    }
}
[System.Serializable]
public class ColorPaternForCircles
{
    public string Name;
    public Color borderColor;
    public Color bgColor;
    public float borderSize;
}