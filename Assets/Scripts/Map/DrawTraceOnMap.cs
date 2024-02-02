using System;
using UnityEngine;
public class DrawTraceOnMap : MonoBehaviour
{
    /// <summary>
    /// Number of segments
    /// </summary>
    public int segments = 240;

    [SerializeField] private int scaleAmount;
    private void Start()
    {
        // Create new event OnDrawTooltip for all markers.
        OnlineMapsMarkerBase.OnMarkerDrawTooltip += OnMarkerDrawTooltip;
    }
    
    private void OnMarkerDrawTooltip(OnlineMapsMarkerBase marker)
    {
        // Here you draw the tooltip for the marker.
    }

    public void DrawCirlce(double lat, double lng, float radius, Color color, string markerID)
    {
        OnlineMapsMarkerManager.CreateItem(lng, lat, "Marker " + OnlineMapsMarkerManager.CountItems);
        OnlineMaps map = OnlineMaps.instance;
        double nlng, nlat;
        OnlineMapsUtils.GetCoordinateInDistance(lng, lat, radius, 90, out nlng, out nlat);
            
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
        double step = 360d / (segments-1);

        // Calculate each point of circle
        for (int i = 0; i < segments; i++)
        {
            double px = tx1 + Math.Cos(step * i * OnlineMapsUtils.Deg2Rad) * r;
            double py = ty1 + Math.Sin(step * i * OnlineMapsUtils.Deg2Rad) * r;
            map.projection.TileToCoordinates(px, py, 20, out lng, out lat);
            points[i] = new OnlineMapsVector2d(lng, lat);
        }

        // Create a new polygon to draw a circle
        OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingPoly(points, color, 3));

        // Add OnClick events to static markers
        ModifyTraceToBeClickable(OnlineMapsMarkerManager.instance[OnlineMapsMarkerManager.instance.Count-1], (int)radius, markerID);
    }
    
    public static Texture2D DrawCircleOnMap(Texture2D tex, Color color, int x, int y, int radius)
    {
        float rSquared = radius * radius;
        for (int u = x - radius; u < x + radius + 1; u++)
        for (int v = y - radius; v < y + radius + 1; v++)
            if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
            {
                
            }
            else
            {
                tex.SetPixel(u, v, Color.clear);
            }
        tex.Apply();
        return tex;
    }
    
    
    public void ModifyTraceToBeClickable(OnlineMapsMarker marker, int radius, string markerID)
    {
        Debug.Log("Modifying Marker");
        marker.OnClick += OnMarkerClick;
        
        var tex = new Texture2D(radius*2*scaleAmount,radius*2*scaleAmount);
        //var tex = new Texture2D(radius*2*scaleAmount,radius*2*scaleAmount, TextureFormat.ARGB32, false);
         Color fillColor = Color.clear;
         Color[] fillPixels = new Color[tex.width * tex.height];
         for (int i = 0; i < fillPixels.Length; i++)
         {
             fillPixels[i] = fillColor;
         }
         tex.SetPixels(fillPixels);
        
         tex.Apply();
        
        //marker.texture = DrawCircleOnMap(tex, new Color(100,250,100,100), radius*scaleAmount, radius*scaleAmount, radius*scaleAmount);
        //tex = DrawCircleOnMap(tex, Color.green, 300, 300, 100);

        marker.texture = tex;
        marker.label = markerID;
    }
    
    private void OnMarkerClick(OnlineMapsMarkerBase marker)
    {
        //todo: check if click was actually indside the circle
        Debug.Log(marker.label);
    }

    public void Clear()
    {
        for (int i = OnlineMapsDrawingElementManager.CountItems; i >=  0; i--)
        {
            OnlineMapsDrawingElementManager.RemoveItemAt(i);
        }
        for (int i = OnlineMapsMarkerManager.CountItems; i >  0; i--)
        {
            OnlineMapsMarkerManager.RemoveItemAt(i);
        }
    }
}