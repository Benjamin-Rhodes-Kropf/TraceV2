using System;
using UnityEngine;
public class DrawTraceOnMap : MonoBehaviour
{
    /// <summary>
    /// Number of segments
    /// </summary>
    public int segments = 240;
    
    public void DrawCirlce(double lat, double lng, float radius, Color color)
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