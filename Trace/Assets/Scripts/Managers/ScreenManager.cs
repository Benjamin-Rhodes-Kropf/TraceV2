using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class ScreenManager : MonoBehaviour
{
    //Dont Destroy
    public static ScreenManager instance;
    
    //custom screen switch manager and animator
    [SerializeField] private ScreenSwitchAnimationManager _screenSwitchAnimationManager;
    [SerializeField] private PopupAnimationManager _popupAnimationManager;
    
    // containers for currently displayed screen and hidden screens
    [SerializeField] private Transform PopUpParent;
    [SerializeField] public Transform activeParent;
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

    public UIController uiController;
    public CameraManager camManager;
    public bool isComingFromCameraScene = false;
    //Reset Hierarchy
    void Awake()
    {
        //dont destroy
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // re-parent all screen transforms to hidden object
        foreach (var s in Screens)
        {
            s.ScreenObject.gameObject.SetActive(true);
            s.ScreenObject.transform.SetParent(inactiveParent, false);
        }
        foreach (var s in PopUpScreens)
        {
            s.ScreenObject.gameObject.SetActive(true);
            s.ScreenObject.transform.SetParent(inactiveParent, false);
        }

        LoadingScreen();
        activeParent.gameObject.SetActive(true);
        inactiveParent.gameObject.SetActive(false);
        inactivePopupParent.gameObject.SetActive(false);
    }
    void Start()
    {

        // **** To fix the microphone bug by stoping the use of microphone in start scene**** //
        //Microphone.End(null);
    }
    //This function is called when scene scene is loded
    private void OnLevelWasLoaded(int level)
    {
        ////checking if the scene is loaded from the camera scene and if the scene is main scene
        //if (isComingFromCameraScene && level == 0)
        //{
        //    //change the bool so that it runs one abd won't run until it is required
        //    isComingFromCameraScene = false;
        //    //this will turn on the main canvas from where we laave while coming to camera scene
        //    mainCanvas.SetActive(true);
        //}
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
            current.ScreenObject.GetComponent<FadeAnim>().FadeOut();
            history.Add(current); // add current screen to history
            current = newScreen; // assign new as current
            newScreen.ScreenObject.SetParent(endParent, false); // set new screen parent for animation
        }
    }
    public void LoadingScreen()
    {
        // clear history
        history = new List<UIScreen>();
        UIScreen screen = ScreenFromID("Loading");
        current = screen;
        current.ScreenObject.SetParent(activeParent, false); // set current screen parent for animation
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
    //load camera scene
    public void LoadArScene() {
        ChangeScreenNoAnim("Camera Screen");
        SceneManager.LoadScene("CemraWork");
    }
    //load main scene, used by the camera scene
    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("Main");

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
            newScreen.ScreenObject.SetParent(activeParent, false); // set new screen parent for animation
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
    public void ChangeScreenForwardsSlideOver(string ScreenID)
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
            _screenSwitchAnimationManager.slideScreenForwardSlideOver();
        }
    }
    public void ChangeScreenBackwardsSlideOver(string ScreenID)
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
            _screenSwitchAnimationManager.slideScreenBackwardSlideOver();
        }
    }
    public void ChangeScreenForwardsSlideOff(string ScreenID)
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
            _screenSwitchAnimationManager.slideScreenForwardSlideOff();
        }
    }
    public void ChangeScreenBackwardsSlideOff(string ScreenID)
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
            _screenSwitchAnimationManager.slideScreenBackwardSlideOff();
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