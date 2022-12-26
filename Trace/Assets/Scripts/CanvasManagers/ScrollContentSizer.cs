using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScrollContentSizer : MonoBehaviour
{
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private int heightOfRect;
    
    public int GetHeight()
    {
        return heightOfRect;
    }
    public void SetHeight(int height)
    {
        heightOfRect = height;
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x,heightOfRect);
    }

    private void OnEnable()
    {
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x,heightOfRect);
    }
}
