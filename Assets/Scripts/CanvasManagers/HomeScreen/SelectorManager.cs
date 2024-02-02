using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorManager : MonoBehaviour
{
    [SerializeField] private Animator selectorAnimator;
    public bool selectorInUpPosition;
    public void OnEnable()
    {
        
    }

    public void SelectorPressed()
    {
        selectorInUpPosition = selectorInUpPosition ? false : true;
        if (!selectorInUpPosition)
        {
            selectorAnimator.Play("MoveDown");
        }
        else
        {
            selectorAnimator.Play("MoveUp");
        }
    }
}
