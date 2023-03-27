using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUpAccount : MonoBehaviour
{
    [SerializeField] private ScreenManager _screenManager;
    private void OnEnable()
    {
        StartCoroutine(fakeSetupTime());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator fakeSetupTime()
    {
        yield return new WaitForSeconds(2f);
        ScreenManager.instance.ChangeScreenFade("HomeScreen");
    }
}
