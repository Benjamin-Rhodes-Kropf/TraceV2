using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideToPointAnim : MonoBehaviour
{
    [SerializeField] private Transform _start;
    [SerializeField] private Transform _finish;
    [SerializeField] private float _duration = 1;
    private void Awake()
    {
        transform.position = _start.position;
    }
}
