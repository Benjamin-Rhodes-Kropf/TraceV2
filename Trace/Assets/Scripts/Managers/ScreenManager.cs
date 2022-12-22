using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    //custom screen switch manager and animator
    [SerializeField] private ScreenSwitchAnimationManager _screenSwitchAnimationManager;
    [SerializeField] private PopupAnimationManager _popupAnimationManager;
    
    // containers for currently displayed screen and hidden screens
    [SerializeField] private Transform PopUpParent;
    [SerializeField] private Transform activeParent;
    [SerializeField] private Transform inactiveParent;
    [SerializeField] private Transform inactivePopupParent;
    
    // containers for animating screens
    [SerializeField] private Transform startParent;
    [SerializeField] private Transform endParent;
    
    // screens to be dislayed
    [SerializeField] private UIScreen[] Screens;
    [SerializeField] private UIScreen[] PopUpScreens;

    [SerializeField] private List<UIScreen> history;
    [SerializeField] private UIScreen current;
    [SerializeField] private UIScreen currentPopUp;
    
    //Reset Hierarchy
    void Awake()
    {
        // re-parent all screen transforms to hidden object
        foreach(var s in Screens)
        {
            s.ScreenObject.gameObject.SetActive(true);
            s.ScreenObject.transform.SetParent(inactiveParent, false);
        }
        foreach(var s in PopUpScreens)
        {
            s.ScreenObject.gameObject.SetActive(true);
            s.ScreenObject.transform.SetParent(inactiveParent, false);
        }
        
        LoadingScreen();
        activeParent.gameObject.SetActive(true);
        inactiveParent.gameObject.SetActive(false);
        inactivePopupParent.gameObject.SetActive(false);
    }
    
    
    //Call Custom Screen Display
    public void WelcomeScreen()
    {
        UIScreen newScreen = ScreenFromID("Welcome");
        if ( newScreen != null)
        {
            //startScreen leaves the view and endScreen slides into view
            history.Clear();
            current.ScreenObject.SetParent(startParent, false); // set current screen parent for animation
            history.Add(current); // add current screen to history
            current = newScreen; // assign new as current
            newScreen.ScreenObject.SetParent(endParent, false); // set new screen parent for animation
            _screenSwitchAnimationManager.slideScreenDown();
        }
    }
    public void LoadingScreen()
    {
        // clear history
        history = new List<UIScreen>();
        UIScreen screen = ScreenFromID("Loading");
        current = screen;
        current.ScreenObject.SetParent(startParent, false); // set current screen parent for animation
    }
    
    
    //Change Screen Displayed
    public void OpenPopup(string PopUpID)
    {
        currentPopUp = PopupFromID(PopUpID);
        currentPopUp.ScreenObject.SetParent(PopUpParent);
        // _popupAnimationManager.slidePopupIn();
    }
    
    public void ClosePopup()
    {
        _popupAnimationManager.slidePopupOut();
    }
    
    public void ChangeScreenNoAnim(string ScreenID)
    {
        UIScreen newScreen = ScreenFromID(ScreenID);
        if (newScreen != null)
        {
            //startScreen leaves the view and endScreen slides into view
            history.Clear();
            current.ScreenObject.SetParent(inactiveParent, false); // set current screen parent for animation
            history.Add(current); // add current screen to history
            current = newScreen; // assign new as current
            newScreen.ScreenObject.SetParent(endParent, false); // set new screen parent for animation
            //_screenSwitchAnimationManager.slideScreensFoward();
        }
    }
    
    public void ChangeScreenForwards(string ScreenID)
    {
        UIScreen newScreen = ScreenFromID(ScreenID);
        if ( newScreen != null)
        {
            //startScreen leaves the view and endScreen slides into view
            history.Clear();
            current.ScreenObject.SetParent(startParent, false); // set current screen parent for animation
            history.Add(current); // add current screen to history
            current = newScreen; // assign new as current
            newScreen.ScreenObject.SetParent(endParent, false); // set new screen parent for animation
            _screenSwitchAnimationManager.slideScreensFoward();
        }
    }
    public void ChangeScreenBackwards(string ScreenID)
    {
        UIScreen newScreen = ScreenFromID(ScreenID);
        if ( newScreen != null)
        {
            //startScreen leaves the view and endScreen slides into view
            history.Clear();
            current.ScreenObject.SetParent(startParent, false); // set current screen parent for animation
            history.Add(current); // add current screen to history
            current = newScreen; // assign new as current
            newScreen.ScreenObject.SetParent(endParent, false); // set new screen parent for animation
            _screenSwitchAnimationManager.slideScreensBackward();
        }
    }
    public void ChangeScreenDown(string ScreenID)
    {
        UIScreen newScreen = ScreenFromID(ScreenID);
        if ( newScreen != null)
        {
            //startScreen leaves the view and endScreen slides into view
            history.Clear();
            current.ScreenObject.SetParent(startParent, false); // set current screen parent for animation
            history.Add(current); // add current screen to history
            current = newScreen; // assign new as current
            newScreen.ScreenObject.SetParent(endParent, false); // set new screen parent for animation
            _screenSwitchAnimationManager.slideScreenDown();
        }
    }
    
    public void ChangeScreenFade(string ScreenID)
    {
        UIScreen newScreen = ScreenFromID(ScreenID);
        if ( newScreen != null)
        {
            //startScreen leaves the view and endScreen slides into view
            history.Clear();
            current.ScreenObject.GetComponent<FadeAnim>().FadeOut();
            history.Add(current); // add current screen to history
            current = newScreen; // assign new as current
            newScreen.ScreenObject.SetParent(endParent, false); // set new screen parent for animation
        }
    }
    
    public void GoBackScreen()
    {
        //Todo: Make work for more than one screen
        if (history.Count < 1) { 
            Debug.LogWarning("historyLessThanOne");
            return; // if first screen, ignore
        }
        UIScreen screen = history[history.Count - 1]; // get previous screen
        history.Remove(history[history.Count - 1]); // remove current screen from history
        _screenSwitchAnimationManager.slideScreensFoward();
        //ScreenAnimator.SetTrigger("Prev"); // trigger animation //Next
        current.ScreenObject.SetParent(endParent, false); // set current screen parent for animation
        current = screen; // assign new as current
        screen.ScreenObject.SetParent(startParent, false); // set new screen parent for animation
    }
    
    //get canvases
    UIScreen ScreenFromID(string ScreenID)
    {
        foreach (UIScreen screen in Screens)
        {
            if (screen.Name == ScreenID) return screen;
        }

        return null;
    }
    UIScreen PopupFromID(string ScreenID)
    {
        foreach (UIScreen screen in PopUpScreens)
        {
            if (screen.Name == ScreenID) return screen;
        }

        return null;
    }
    
    
    //functions called from animation
    public void SetActiveParent()
    {
        foreach (var s in Screens)
        {
            if (s != current) s.ScreenObject.SetParent(inactiveParent, false);
        }

        // show active screen
        current.ScreenObject.SetParent(activeParent, false);
    }
    public void ClearAllPopups()
    {
        foreach (var p in PopUpScreens)
        {
            p.ScreenObject.SetParent(inactiveParent, false);
        }
    }
}


[System.Serializable]
public class UIScreen
{
    public string Name;
    public Transform ScreenObject;
}