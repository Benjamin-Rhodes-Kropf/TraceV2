using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectRadiusCanvas : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(LoadMap());
    }

    IEnumerator LoadMap()
    {
        yield return new WaitForSeconds(0.3f);
        ScreenManager.instance.isComingFromCameraScene = true;
        SceneManager.UnloadSceneAsync(1);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
