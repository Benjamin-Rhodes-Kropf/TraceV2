#if UNITY_EDITOR && UNITY_ANDROID
using UnityEditor;
using VoxelBusters.CoreLibrary.Editor.NativePlugins.Build;
using VoxelBusters.CoreLibrary;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Text;
using VoxelBusters.CoreLibrary.NativePlugins;
using UnityEditor.Build.Reporting;
using System.Linq;

namespace VoxelBusters.EssentialKit.Editor.Android
{
    [InitializeOnLoad]
    public class AndroidBuildProcess 
    {
#region Static fields

        private static EssentialKitSettings s_settings;

#endregion

#region Constructors

        static AndroidBuildProcess()
        {
            // unregister from events
            BuildProcessReporter.OnBuildTargetChange   -= OnBuildTargetChange;
            BuildProcessReporter.OnPreprocessBuild     -= OnPreprocessBuild;
            BuildProcessReporter.OnPostprocessBuild    -= OnPostprocessBuild;

            // register for events
            BuildProcessReporter.OnBuildTargetChange   += OnBuildTargetChange;
            BuildProcessReporter.OnPreprocessBuild     += OnPreprocessBuild;
            BuildProcessReporter.OnPostprocessBuild    += OnPostprocessBuild;
        }

#endregion

#region Static methods

        public static void OnBuildTargetChange(BuildTarget previousTarget, BuildTarget newTarget)
        {
            BuildForTarget(newTarget);
        }
    
        public static void OnPreprocessBuild(BuildReport report)
        {
            BuildForTarget(report.summary.platform);
        }

        public static void OnPostprocessBuild(BuildReport report)
        {
            // check whether target platform is android
            if (report.summary.platform != BuildTarget.Android)
            {
                return;
            }
        }

#endregion

#region Private methods

        private static void BuildForTarget(BuildTarget target)
        {
            // check whether plugin is configured
            if (!EssentialKitSettingsEditorUtility.SettingsExists)
            {
                EssentialKitSettingsEditorUtility.ShowSettingsNotFoundErrorDialog();
                return;
            }

            if (target != BuildTarget.Android)
            {
                return;
            }

            // update properties
            s_settings      = EssentialKitSettingsEditorUtility.DefaultSettings;

            // perform tasks
            PrepareForBuild();

            EssentialKitBuildUtility.CreateStrippingFile(target);

            // warn missing details and abort if not met
        }

        private static void PrepareForBuild()
        {
            // check if all required data is available for each feature
            CheckDataAvailabilityForFeatures();

            // enable required libraries
            EnabledRequiredLibraries();

            // generate config files
            //1. Copy required settings and configurable text to android xml files
            UpdateAndroidResourceFiles();
        }

        private static void CheckDataAvailabilityForFeatures()
        {
            StringBuilder builder = new StringBuilder();

            if (builder.Length != 0)
            {
                string error = string.Format("[VoxelBusters : {0}] \n{1}", EssentialKitSettings.DisplayName, builder.ToString());
                if (NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState())
                {
                    Debug.LogError(error, s_settings);
                }
                else
                {
                    Debug.LogWarning(error, s_settings);
                }
                
            }
        }

        private static void EnabledRequiredLibraries()
        {

            Dictionary<string, bool> config = new Dictionary<string, bool>();

            config.Add("feature.addressbook",           s_settings.AddressBookSettings.IsEnabled);
            config.Add("feature.networkservices",       s_settings.NetworkServicesSettings.IsEnabled);
            config.Add("feature.sharingservices",       s_settings.SharingServicesSettings.IsEnabled);
            config.Add("feature.uiviews",               true);
            config.Add("feature.extras",                true);
            config.Add("essentialkit.core",             true);


            foreach (var each in config.Keys)
            {
                bool    isEnabled       = config[each];
                string  fileName        = each + ".jar";

                if(isEnabled)
                {
                    IOServices.CopyFile(IOServices.CombinePath(EssentialKitPackageLayout.AndroidProjectAllLibsPath, fileName), IOServices.CombinePath(EssentialKitPackageLayout.AndroidProjectLibsPath, fileName));
                }
                else
                {
                    IOServices.DeleteFile(IOServices.CombinePath(EssentialKitPackageLayout.AndroidProjectLibsPath, fileName));
                }
            }
        }

        private static void UpdateAndroidResourceFiles()
        {

            // generate files
            GenerateRequiredFiles();

            // copy files
            CopyRequiredFiles();

            // update string resources
            UpdateStringsXml();
        }

        private static void GenerateRequiredFiles()
        {
            // generate manifest
            AndroidManifestGenerator.GenerateManifest(s_settings);

            // generate dependencies
            AndroidLibraryDependenciesGenerator.CreateLibraryDependencies();
        }

        private static void CopyRequiredFiles()
        {
        }

        private static void UpdateStringsXml()
        {
            Dictionary<string, string> config = new Dictionary<string, string>();

            if (s_settings.AddressBookSettings.IsEnabled)
            {
                config.Add("ADDRESS_BOOK_PERMISSON_REASON", s_settings.ApplicationSettings.UsagePermissionSettings.AddressBookUsagePermission.GetDescriptionForActivePlatform());
            }
            

            XmlDocument xml = new XmlDocument();
            xml.Load(EssentialKitPackageLayout.AndroidProjectResValuesStringsPath);
            XmlNodeList nodes = xml.SelectNodes("/resources/string");

            foreach (XmlNode node in nodes)
            {
                XmlAttribute xmlAttribute = node.Attributes["name"];
                string key = xmlAttribute.Value;
                
                if(config.ContainsKey(key))
                {
                    node.InnerText = config[key];
                }
            }

            xml.Save(EssentialKitPackageLayout.AndroidProjectResValuesStringsPath);
        }

        private static void CopyStreamingAssets()
        {
            // Copy audio files from streaming assets if any to Raw folder
            string path = UnityEngine.Application.streamingAssetsPath;
            if(IOServices.DirectoryExists(path))
            {
                string[] files = System.IO.Directory.GetFiles(path);
                string destinationFolder = "Assets/Plugins/Android/com.voxelbusters.essentialkit.androidlib/res/raw/";
                IOServices.CreateDirectory(destinationFolder);

                string[] formats = new string[]
                {
                    ".mp3",
                    ".wav",
                    ".ogg"
                };

                for(int i=0; i< files.Length; i++)
                {
                    string extension = System.IO.Path.GetExtension(files[i]);
                    if(formats.Contains(extension.ToLower()))
                    {
                        IOServices.CopyFile(files[i], IOServices.CombinePath(destinationFolder, IOServices.GetFileName(files[i]).ToLower()));
                    }
                }
            }
        }

#endregion
    }
}
#endif