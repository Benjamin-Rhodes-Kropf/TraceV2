using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;

namespace SA.iOS
{
    internal class ISN_AdSupportUI : ISN_ServiceSettingsUI
    {

        public override void OnAwake() 
        {
            base.OnAwake();
            AddFeatureUrl("Get started", "https://unionassets.com/ios-native-pro/getting-started-837");
            AddFeatureUrl("Advertising Tracking", "https://unionassets.com/ios-native-pro/asidentifiermanager-api-835#advertising-tracking");
            AddFeatureUrl("Advertising Identifier", "https://unionassets.com/ios-native-pro/asidentifiermanager-api-835#advertising-identifier");
        }

        public override string Title 
        {
            get 
            {
                return "AdSupport";
            }
        }

        public override string Description 
        {
            get 
            {
                return "Access the advertising identifier and a flag that indicates whether the user has chosen to limit ad tracking.";
            }
        }

        protected override Texture2D Icon 
        {
            get 
            {
               return  SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "AdSupport_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver 
        {
            get 
            {
                return ISN_Preprocessor.GetResolver<ISN_AdSupportResolver>();
            }
        }


        protected override IEnumerable<string> SupportedPlatforms 
        {
            get 
            {
                return new List<string>() { "iOS" };
            }
        }

        protected override void OnServiceUI() 
        {
           
        }

    }
}