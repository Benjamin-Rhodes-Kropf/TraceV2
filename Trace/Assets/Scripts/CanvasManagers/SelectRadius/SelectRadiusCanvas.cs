using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SelectRadiusCanvas : MonoBehaviour
{
    [SerializeField]private UnityEngine.UI.Slider _radiusSlider;
    [SerializeField]private Button _isVisableToggleButton;
    [SerializeField]private bool _isTraceVisable;
    [SerializeField]private GameObject traceIsVisable;
    [SerializeField]private GameObject traceIsHidden;

    private void OnEnable()
    {
        StartCoroutine(LoadMap());
        
        if (PlayerPrefs.GetFloat("LeaveTraceSliderRadiusValue") != 0)
        {
            _radiusSlider.value = PlayerPrefs.GetFloat("LeaveTraceSliderRadiusValue");
        }
        else
        {
            PlayerPrefs.SetFloat("LeaveTraceSliderRadiusValue", 0.5f);
            _radiusSlider.value = 0.5f;
            SetRadius();
        }
        
        if (PlayerPrefs.GetInt("LeaveTraceIsVisable") != 0)
        {
            if (PlayerPrefs.GetInt("LeaveTraceIsVisable") == 1)
            {
                _isTraceVisable = true;
                traceIsVisable.SetActive(true);
                traceIsHidden.SetActive(false);
            }
            else
            {
                _isTraceVisable = false;
                traceIsVisable.SetActive(false);
                traceIsHidden.SetActive(true);
            }
        }
        else
        {
            PlayerPrefs.GetInt("LeaveTraceIsVisable", 1);
            _isTraceVisable = true;
            traceIsVisable.SetActive(true);
            traceIsHidden.SetActive(false);
        }
    }

    IEnumerator LoadMap()
    {
        yield return new WaitForSeconds(0.3f);
        ScreenManager.instance.isComingFromCameraScene = true;
        SceneManager.UnloadSceneAsync(1);
    }

    public void SendTraceButton()
    {
        PlayerPrefs.SetFloat("LeaveTraceSliderRadiusValue", _radiusSlider.value);
        if(_isTraceVisable)
            PlayerPrefs.SetInt("LeaveTraceIsVisable", 1);
        else
        {
            PlayerPrefs.SetInt("LeaveTraceIsVisable", -1);
        }
    }
    public void SetRadius()
    {
        SendTraceManager.instance.SetRadius(_radiusSlider.value);
    }

    public void ToggleTraceVisability()
    {
        _isTraceVisable = !_isTraceVisable;
        
        if (_isTraceVisable)
        {
            traceIsVisable.SetActive(true);
            traceIsHidden.SetActive(false);
        }
        else
        {
            traceIsVisable.SetActive(false);
            traceIsHidden.SetActive(true);
        }
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
