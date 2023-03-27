using System.Collections.Generic;
using UnityEngine;
using SA.iOS.UIKit.Internal;
using System;
using UnityEngine.Assertions;

namespace SA.iOS.UIKit
{
    /// <summary>
    /// Object that create and control WheelPicker for iOS.
    /// In this controller we need to set data that UIPickerView should show
    /// and add listeners, that will be called when user will choose some option or
    /// Done/Cancel picking process.
    /// </summary>
    [Serializable]
    public class ISN_UIWheelPickerController
    {
        [SerializeField] private List<string> m_Values;

        /// <summary>
        /// Here we cate instance of UIWheelPicker controller.
        /// </summary>
        /// <param name="values">
        /// It's list of elements that should be shown in UIWheelPicker
        /// </param>
        public ISN_UIWheelPickerController(List<string> values)
        {
            this.m_Values = values;
        }

        /// <summary>
        /// Get values for ISN_UIWheelPicker.
        /// </summary>
        public List<string> Values
        {
            get
            {
                return m_Values;
            }
        }

        /// <summary>
        /// Show UIWheelPicker.
        /// </summary>
        /// <param name="callback">
        /// This is callback that will be called from UiWheelPicker
        /// when user will change it state or done/cancel option.
        /// It shouldn't be null.
        /// </param>
        public void Show(Action<ISN_UIWheelPickerResult> callback)
        {
            Assert.IsNotNull(callback);
            ISN_UILib.API.ShowUIWheelPicker(this, callback);
        }
    }
}
