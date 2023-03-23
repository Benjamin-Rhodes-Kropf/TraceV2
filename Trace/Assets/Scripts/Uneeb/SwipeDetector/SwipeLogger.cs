using UnityEngine;

public class SwipeLogger : MonoBehaviour
{

    private void Awake()
    {
        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
    }

    private void SwipeDetector_OnSwipe(SwipeData data)
    {
        if (ScreenManager.instance.activeParent.transform.childCount == 0 ||
            ScreenManager.instance.currentScreenName == "HomeScreen")
        {
            Debug.Log("Active Parent is Empty or map is active, cannot detect swipe");
        }
        else if (ScreenManager.instance.currentScreenName == "Contacts" && data.Direction == SwipeDirection.Left)
        {
            ScreenManager.instance.ChangeScreenForwardsSlideOff("HomeScreen");
        }
        else if (ScreenManager.instance.currentScreenName == "EditProfile" && data.Direction == SwipeDirection.Right)
        {
            ScreenManager.instance.ChangeScreenBackwardsSlideOff("HomeScreen");
        }
        Debug.Log("Swipe in Direction: " + data.Direction);

    }
}