using System;
using UnityEngine;
using System.Collections;

public class ConfineViewToCircleWithRadius : MonoBehaviour
{
    public float raduis = 1.5f; // km
    private OnlineMapsMarker playerMarker;
    private OnlineMapsMarkerManager markerManager;
    private float sqrRadius;
    private OnlineMaps map;
    private bool ignoreEvent;

    private void Start()
    {
        sqrRadius = raduis * raduis;
        map = OnlineMaps.instance;
        markerManager = OnlineMapsMarkerManager.instance;
        map.OnChangePosition += OnChangePosition;
        map.OnChangeZoom += OnChangePosition;
        playerMarker = new OnlineMapsMarker();
        playerMarker.position = map.position;
    }

    private void OnChangePosition()
    {
        if (ignoreEvent) return;

        double mx, my;
        playerMarker.GetPosition(out mx, out my);

        double px, py;
        map.GetPosition(out px, out py);

        double dx, dy;
        OnlineMapsUtils.DistanceBetweenPoints(px, py, mx, my, out dx, out dy);
        double sqrR = dx * dx + dy * dy;
        if (sqrR > sqrRadius)
        {
            double tx1, ty1, tx2, ty2;
            map.projection.CoordinatesToTile(px, py, map.zoom, out tx1, out ty1);
            map.projection.CoordinatesToTile(mx, my, map.zoom, out tx2, out ty2);

            double scale = raduis / Math.Sqrt(sqrR);
            double ntx = tx2 + (tx1 - tx2) * scale;
            double nty = ty2 + (ty1 - ty2) * scale;
            map.projection.TileToCoordinates(ntx, nty, map.zoom, out ntx, out nty);
            ignoreEvent = true;
            map.SetPosition(ntx, nty);
            ignoreEvent = false;

            OnlineMapsUtils.DistanceBetweenPoints(ntx, nty, mx, my, out dx, out dy);
            Debug.Log("New distance: " + Math.Sqrt(dx * dx + dy * dy));
        }
    }
}
