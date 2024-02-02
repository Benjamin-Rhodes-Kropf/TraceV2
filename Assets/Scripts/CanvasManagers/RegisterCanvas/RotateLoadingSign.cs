using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLoadingSign : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f,0f,_speed *  Time.deltaTime, Space.Self);
    }
}
