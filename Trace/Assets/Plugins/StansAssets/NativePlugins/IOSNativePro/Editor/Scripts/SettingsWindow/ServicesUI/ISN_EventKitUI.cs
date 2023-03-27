using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;

namespace SA.iOS
{
    public class ISN_EventKitUI : ISN_ServiceSettingsUI
    {
        GUIContent NSCalendarsUsageDescription = new GUIContent("NSCalendars Usage Description[?]:", " The key lets you describe the reason your app accesses the user’s calendar app. When the system prompts the user to allow access, this string is displayed as part of the alert.");
        GUIContent NSRemindersUsageDescription = new GUIContent("NSReminders Usage Description[?]:", " The key lets you describe the reason your app accesses the reminder app. When the system prompts the user to allow access, this string is displayed as part of the alert.");


        public override void OnAwake() 
        {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://unionassets.com/ios-native-pro/getting-started-841");
            AddFeatureUrl("Accessing the Event Store", "https://unionassets.com/ios-native-pro/accessing-the-event-store-842");
            AddFeatureUrl("Events", "https://unionassets.com/ios-native-pro/events-843");
            AddFeatureUrl("Reminders", "https://unionassets.com/ios-native-pro/reminders-844");

        }

        public override string Title 
        {
            get 
            {
                return "Event Kit";
            }
        }

        public override string Description 
        {
            get 
            {
                return "Provides services for create, view, and edit calendar and reminder events.";
            }
        }

        protected override Texture2D Icon 
        {
            get 
            {
                return SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "CoreLocation_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver 
        {
            get 
            {
                return ISN_Preprocessor.GetResolver<ISN_EventKitResolver>();
            }
        }


        protected override void OnServiceUI() 
        {

            using (new SA_WindowBlockWithSpace(new GUIContent("Usage Description"))) 
            {
                EditorGUILayout.HelpBox("Once you link with iOS 10 you must declare access to any user private data types.", MessageType.Info);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(NSCalendarsUsageDescription);
                using (new SA_GuiIndentLevel(1)) 
                {
                    ISN_Settings.Instance.NSCalendarsUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.NSCalendarsUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(30));
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(NSRemindersUsageDescription);
                using (new SA_GuiIndentLevel(1)) 
                {
                    ISN_Settings.Instance.NSRemindersUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.NSRemindersUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(30));
                }

              
            }
        }
    }
}
