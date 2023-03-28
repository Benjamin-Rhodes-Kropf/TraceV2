using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSelectManager : MonoBehaviour
{
    [SerializeField] private Animator timeSelectAnimator;
    [SerializeField] private bool isInAllTimeMode;
    public void OnEnable()
    {
        
    }

    public void SelectorPressed()
    {
        isInAllTimeMode = isInAllTimeMode ? false : true;
        if (!isInAllTimeMode)
        {
            timeSelectAnimator.Play("SelectRecentTime");
        }
        else
        {
            timeSelectAnimator.Play("SelectAllTime");
        }
    }

}
