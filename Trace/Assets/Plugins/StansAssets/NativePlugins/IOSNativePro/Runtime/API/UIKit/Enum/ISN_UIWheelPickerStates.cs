using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UIKit
{

    /// <summary>
    /// Constants indicating the type of alert to display.
    /// </summary>
    public enum ISN_UIWheelPickerStates
    {
        // User pick variant.
        IN_PROGRESS, 
        // User picker variant.
        DONE,
        // User canceled and closed ISN_UIWheelPicker
        CANCELED   
    }
}