using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//still having problems with map markers culling when still in view
public class ScaleMarkers : MonoBehaviour
{
    /// <summary>
    /// Zoom, when the scale = 1.
    /// </summary>
    public int defaultZoom = 15;

    /// <summary>
    /// Instance of marker.
    /// </summary>
    [SerializeField]private OnlineMapsMarkerManager markerManager;

    /// <summary>
    /// Init.
    /// </summary>
    private void Start()
    {
        // Subscribe to change zoom.
        OnlineMaps.instance.OnChangeZoom += OnChangeZoom;

        // Initial rescale marker.
        OnChangeZoom();
    }

    /// <summary>
    /// On change zoom.
    /// </summary>
    private void OnChangeZoom()
    {
        float originalScale = 1 << defaultZoom;
        float currentScale = 1 << OnlineMaps.instance.zoom;

        foreach (var marker in markerManager.items)
        {
            marker.scale = currentScale / originalScale;
        }
    }
}
