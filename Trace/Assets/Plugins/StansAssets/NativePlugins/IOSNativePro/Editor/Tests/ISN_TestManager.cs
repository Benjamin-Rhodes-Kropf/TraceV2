using SA.iOS.GameKit;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using SA.iOS.XCode;
using SA.iOS.StoreKit;

namespace SA.iOS.Utilities 
{
    public static class ISN_TestManager
    {
        public const string SMALL_PACK = "buying_10000";
        public const string NC_PACK = "mm_subscription";
        
        public static void ApplyExampleConfig() 
        {
            Debug.Log("ISN_TestManager::ApplyExampleConfig");
            PlayerSettings.iOS.applicationDisplayName = "IOS Native";
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.appleDeveloperTeamID = "P42C7H5LKK";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.iosnative");
            
            var settings = ISN_Settings.Instance;

            //Contacts
            settings.Contacts = true;
            
            //In-Apps
            ISN_Settings.Instance.InAppProducts.Clear();

            var p = new ISN_SKProduct();
            p.LocalizedTitle = "iOS Test Product1";
            p.ProductIdentifier = "your.product.id1.here";

            var p2 = new ISN_SKProduct();
            p2.LocalizedTitle = "iOS Test Product1";
            p2.ProductIdentifier = "your.product.id2.here";

            ISN_Settings.Instance.InAppProducts.Add(p);
            ISN_Settings.Instance.InAppProducts.Add(p2);
            
            //GameKit
            ISD_API.Capability.GameCenter.Enabled = true;
            ISN_Settings.Instance.SavingAGame = true;
            
            ISN_Settings.Instance.Achievements.Clear();
            var achievement = new ISN_GKAchievement("my_first_achievement");
            achievement.Name = "IOS Native 2018 First Achievement";
            ISN_Settings.Instance.Achievements.Add(achievement);
            
            achievement = new ISN_GKAchievement("isn.test.achievement");
            achievement.Name = "Achievement #2";
            ISN_Settings.Instance.Achievements.Add(achievement);
            
            //Replay Kit
            settings.ReplayKit = true;
            
            //AV Kit
            settings.AVKit = true;

            //User Notifications
            settings.UserNotifications = true;
            ISD_API.Capability.PushNotifications.Enabled = true;


            //or Vending Test Environment
            ISD_API.Capability.InAppPurchase.Enabled = true;

            //social 
            settings.Social = true;
        }

        public static void OpenTestScene() 
        {
            EditorSceneManager.OpenScene(ISN_Settings.TEST_SCENE_PATH, OpenSceneMode.Single);
        }

        public static void MakeTestBuild() 
        {
            var playerOptions = new BuildPlayerOptions();
            playerOptions.target = BuildTarget.iOS;
            playerOptions.scenes = new[] { ISN_Settings.TEST_SCENE_PATH };
            playerOptions.locationPathName = "ios_native_test";
            
            playerOptions.options = BuildOptions.Development | BuildOptions.AutoRunPlayer;

            BuildPipeline.BuildPlayer(playerOptions);
        }
    }
}



