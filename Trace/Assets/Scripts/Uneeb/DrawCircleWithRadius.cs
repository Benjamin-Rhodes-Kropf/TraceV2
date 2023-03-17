using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircleWithRadius : MonoBehaviour
{
    public int renderQueueOffset = 100;

    private void Start()
    {
        foreach (OnlineMapsDrawingElement el in OnlineMapsDrawingElementManager.instance)
        {
            el.renderQueueOffset = renderQueueOffset;
        }
        OnlineMapsDrawingElementManager.OnAddItem += OnAddItem;
    }

    private void OnAddItem(OnlineMapsDrawingElement element)
    {
        element.renderQueueOffset = renderQueueOffset;
    }
}
