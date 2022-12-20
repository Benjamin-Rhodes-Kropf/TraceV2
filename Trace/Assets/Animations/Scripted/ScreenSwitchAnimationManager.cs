using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenSwitchAnimationManager : MonoBehaviour
{
    [Header("Parents")] 
    [SerializeField] private RectTransform _ActiveParent;
    [SerializeField] private RectTransform _InactiveParent;
    [SerializeField] private RectTransform _previousParent;
    
    [Header("Animation Keyframes")]
    [SerializeField] private RectTransform _leftScreenPosition;
    [SerializeField] private RectTransform _rightScreenPosition;
    [SerializeField] private RectTransform _middleScreenPosition;
    [SerializeField] private RectTransform _bottomScreenPosition;
    [SerializeField] private RectTransform _screenA;
    [SerializeField] private RectTransform _screenB;
    
    [Header("Animation Curves")] 
    [SerializeField] private float _horizontalSlideDuration = 1;
    [SerializeField] private float _slideDownDuration = 1;
    [SerializeField] private AnimationCurve slideCurve;
    [SerializeField] private AnimationCurve slideDownCurve;

    private void Start()
    {
        Application.targetFrameRate = 600;
    }

    public void slideScreensFoward()
    {
        _screenA.transform.position = _rightScreenPosition.transform.position;
        _screenB.transform.position = _middleScreenPosition.transform.position;
        
        Debug.Log("ScreenSwitchAnimationManager: sliding screens forward");
        StartCoroutine(LerpX(_screenA, _middleScreenPosition, _horizontalSlideDuration,true));
        StartCoroutine(LerpX(_screenB, _leftScreenPosition, _horizontalSlideDuration, false));
    }
    
    public void slideScreensBackward()
    {
        _screenA.transform.position = _leftScreenPosition.transform.position;
        _screenB.transform.position = _middleScreenPosition.transform.position;
        
        Debug.Log("ScreenSwitchAnimationManager: sliding screens backwards");
        StartCoroutine(LerpX(_screenA, _middleScreenPosition, _horizontalSlideDuration, true));
        StartCoroutine(LerpX(_screenB, _rightScreenPosition, _horizontalSlideDuration, false));
    }
    
    public void slideScreenDown()
    {
        var middlePos = _middleScreenPosition.transform.position;
        _screenA.transform.position = middlePos;
        _screenB.transform.position = middlePos;
        
        Debug.Log("ScreenSwitchAnimationManager: sliding screen down");
        StartCoroutine(LerpY(_screenA, _middleScreenPosition, _slideDownDuration, true));
        StartCoroutine(LerpY(_screenB, _bottomScreenPosition, _slideDownDuration, false));
    }
    
    YieldInstruction yieldInstruction = new WaitForEndOfFrame();
    IEnumerator LerpX(Transform screen, Transform target, float _dur, bool isScreenA)
    {
        float timeElapsed = 0;
        var startingPos = screen.transform.position;
        Vector3 initialStartValue = screen.transform.position;
        
        while (timeElapsed < _dur)
        {
            //Lerp Linear Version
            //screen.transform.position = new Vector3(Mathf.Lerp(initialStartValue.x, target.transform.position.x, timeElapsed / _dur), startingPos.y, startingPos.z);

            //With Custom Curve
            screen.transform.position = new Vector3(Mathf.Lerp(initialStartValue.x, target.transform.position.x, slideCurve.Evaluate(timeElapsed / _dur)), startingPos.y, startingPos.z);
            timeElapsed += Time.deltaTime;
            yield return yieldInstruction;
        }
        
        screen.transform.position = new Vector3(target.transform.position.x, startingPos.y, startingPos.z);

        if (isScreenA)
        {
            var screenA = _screenA.GetChild(0);
            screenA.SetParent(_ActiveParent);
            screenA.transform.localPosition = Vector3.zero;
        }
        else
        {
            var screenB = _screenB.GetChild(0);
            screenB.SetParent(_InactiveParent);
            screenB.transform.localPosition = Vector3.zero;
        }
    }
    
    IEnumerator LerpY(Transform screen, Transform target, float _dur, bool isScreenA)
    {
        float timeElapsed = 0;
        var startingPos = screen.transform.position;
        Vector3 initialStartValue = screen.transform.position;
        
        while (timeElapsed < _dur)
        {
            //Lerp Linear Version
            //screen.transform.position = new Vector3(startingPos.x,Mathf.Lerp(initialStartValue.y, target.transform.position.y, timeElapsed / _dur), startingPos.z);

            //With Custom Curve
            screen.transform.position = new Vector3(startingPos.x, Mathf.Lerp(screen.transform.position.y, target.transform.position.y, slideDownCurve.Evaluate(timeElapsed / _dur)), screen.transform.position.z);
            timeElapsed += Time.deltaTime;
            yield return yieldInstruction;
        }
        
        screen.transform.position = new Vector3(startingPos.x, target.transform.position.y, startingPos.z);

        if (isScreenA)
        {
            var screenA = _screenA.GetChild(0);
            screenA.SetParent(_ActiveParent);
            screenA.transform.localPosition = Vector3.zero;
        }
        else
        {
            var screenB = _screenB.GetChild(0);
            screenB.SetParent(_InactiveParent);
            screenB.transform.localPosition = Vector3.zero;
        }
    }
}
