////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native 2018 - New Generation
// @author Stan's Assets team 
// @support support@stansassets.com
// @website https://stansassets.com
//
//////////////////////////////////////////////////////////////////////////////// 
using System;
using System.Collections.Generic;
using SA.Foundation.Events;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.iOS.UIKit.Internal
{
	
    internal interface ISN_iUIAPI 
    {    
        SA_iEvent<ISN_UIAlertActionId> OnUIAlertActionPerformed { get; }
        void PresentUIAlertController(ISN_UIAlertController alert);
        void DismissUIAlertController(ISN_UIAlertController alert);

        void PreloaderLockScreen();
        void PreloaderUnlockScreen();

        void SaveTextureToCameraRoll(Texture2D texture, Action<SA_Result> callback);
        List<string> GetAvailableMediaTypes(ISN_UIImagePickerControllerSourceType type);
        bool IsSourceTypeAvailable(ISN_UIImagePickerControllerSourceType type);
        void PresentPickerController(ISN_UIPickerControllerRequest request, Action<ISN_UIPickerControllerResult> callback);

        void ShowUIWheelPicker(ISN_UIWheelPickerController controller, System.Action<ISN_UIWheelPickerResult> callback);
	}
}
