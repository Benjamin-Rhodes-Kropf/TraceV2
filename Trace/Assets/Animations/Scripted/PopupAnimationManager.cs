using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PopupAnimationManager : MonoBehaviour
{
    [Header("Parents")] 
    [SerializeField] private RectTransform _popupParent;
    [SerializeField] private RectTransform _InactivePopupParent;
    
    [Header("Animation Keyframes")]
    [SerializeField] private RectTransform _middleScreenPosition;
    [SerializeField] private RectTransform _bottomScreenPosition;
    [SerializeField] private RectTransform _screenA;

    [Header("Animation Curves")] 
    [SerializeField] private float _slideDownDuration = 1;
    [SerializeField] private AnimationCurve slideDownCurve;
    
    
    public void slidePopupIn()
    {
        _screenA.transform.position = _bottomScreenPosition.transform.position;
        _screenA.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
        Debug.Log("PopupAnimationManager: popping screen up");
        StartCoroutine(LerpY(_screenA, _middleScreenPosition, _slideDownDuration, true));
    }
    
    public void slidePopupOut()
    {
        _screenA.transform.position = _middleScreenPosition.transform.position;

        Debug.Log("PopupAnimationManager: popping screen up");
        StartCoroutine(LerpY(_screenA, _bottomScreenPosition, _slideDownDuration, false));
    }


    YieldInstruction yieldInstruction = new WaitForEndOfFrame();
    IEnumerator LerpY(Transform screen, Transform target, float _dur, bool slideIn)
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

        if (slideIn)
        {
            var screenA = _screenA.GetChild(0);
            screenA.SetParent(_popupParent);
            screenA.transform.localPosition = Vector3.zero;
        }
        else
        {
            var screenA = _screenA.GetChild(0);
            screenA.SetParent(_InactivePopupParent);
            screenA.transform.localPosition = Vector3.zero;
        }
        
    }
}
