using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerClickExample : MonoBehaviour
{
    private void Start()
    {
        OnlineMaps map = OnlineMaps.instance;

        // Add OnClick events to static markers
        foreach (OnlineMapsMarker marker in OnlineMapsMarkerManager.instance)
        {
            marker.OnClick += OnMarkerClick;
        }

        // Add OnClick events to dynamic markers
        OnlineMapsMarker dynamicMarker = OnlineMapsMarkerManager.CreateItem(Vector2.zero, null, "Dynamic marker");
        dynamicMarker.OnClick += OnMarkerClick;
    }

    private void OnMarkerClick(OnlineMapsMarkerBase marker)
    {
        // Show in console marker label.
        Debug.Log(marker.label);
    }
}
