using UnityEngine;

public class SwipeLogger : MonoBehaviour
{
    private void Awake()
    {
        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
    }

    private void SwipeDetector_OnSwipe(SwipeData data)
    {
        if (ScreenManager.instance.activeParent.transform.childCount == 0) {
            Debug.Log("Active Parent is Empty or map is active, cannot detect swipe");
        }
        else
            Debug.Log("Swipe in Direction: " + data.Direction);

    }
}