using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation.Templates;

namespace SA.iOS.UIKit
{
    /// <summary>
    /// This type for saving data from ISN_UIWheelPicker callback.
    /// </summary>
    public class ISN_UIWheelPickerResult: SA_Result
    {
        [SerializeField] protected string m_Value;
        [SerializeField] protected string m_State;

        /// <summary>
        /// Get chosen value from ISN_UIWheelPicker callback.
        /// </summary>
        public string Value
        {
            get
            {
                return m_Value;
            }
        }

        /// <summary>
        /// Get current state of ISN_UIWheelPicker callback.
        /// </summary>
        public ISN_UIWheelPickerStates State
        {
            get
            {
                if(!String.IsNullOrEmpty(m_State) && m_State.Equals(ISN_UIWheelPickerStates.DONE.ToString()))
                {
                    return ISN_UIWheelPickerStates.DONE;
                }
                if(!String.IsNullOrEmpty(m_State) && m_State.Equals(ISN_UIWheelPickerStates.IN_PROGRESS.ToString()))
                {
                    return ISN_UIWheelPickerStates.IN_PROGRESS;
                }
                else
                {
                    return ISN_UIWheelPickerStates.CANCELED;
                }
            }
        }
    }
}