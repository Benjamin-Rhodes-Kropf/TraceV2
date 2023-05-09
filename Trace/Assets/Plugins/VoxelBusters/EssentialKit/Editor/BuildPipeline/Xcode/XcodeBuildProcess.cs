#if UNITY_IOS || UNITY_TVOS
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Editor;
using VoxelBusters.CoreLibrary.NativePlugins;
using VoxelBusters.CoreLibrary.Editor.NativePlugins;
using VoxelBusters.CoreLibrary.Editor.NativePlugins.Build;
using VoxelBusters.CoreLibrary.Editor.NativePlugins.Build.Xcode;
using VoxelBusters.EssentialKit;
using UnityEditor.Build.Reporting;

namespace VoxelBusters.EssentialKit.Editor.Build.Xcode
{
    [InitializeOnLoad]
    public static class XcodeBuildProcess
    {
#region Static fields

        private     static  EssentialKitSettings    s_settings;

        private     static  BuildReport             s_buildReport;

#endregion

#region Constructors

        static XcodeBuildProcess()
        {
            // unregister from events
            BuildProcessReporter.OnPreprocessBuild     -= OnPreprocessBuild;
            BuildProcessReporter.OnPostprocessBuild    -= OnPostprocessBuild;

            // register for events
            BuildProcessReporter.OnPreprocessBuild     += OnPreprocessBuild;
            BuildProcessReporter.OnPostprocessBuild    += OnPostprocessBuild;
        }

#endregion

#region Static methods

        public static void OnPreprocessBuild(BuildReport report)
        {
            // check whether plugin is configured
            if (!EssentialKitSettingsEditorUtility.SettingsExists)
            {
                EssentialKitSettingsEditorUtility.ShowSettingsNotFoundErrorDialog();
                return;
            }
            // check whether target is compatible for these tasks
            if (!IsBuildTargetSupported(report.summary.platform))
            {
                return;
            }

            DebugLogger.Log("[XcodeBuildProcess] Initiating pre-build task execution.");
            
            // update cached information
            s_settings      = EssentialKitSettingsEditorUtility.DefaultSettings;
            s_buildReport   = report;

            // execute tasks
            UpdateExporterSettings();
            EssentialKitBuildUtility.CreateStrippingFile(report.summary.platform);

            DebugLogger.Log("[XcodeBuildProcess] Successfully completed pre-build task execution.");
        }

        public static void OnPostprocessBuild(BuildReport report)
        {
            // check whether plugin is configured
            if (!EssentialKitSettingsEditorUtility.SettingsExists)
            {
                EssentialKitSettingsEditorUtility.ShowSettingsNotFoundErrorDialog();
                return;
            }
            if (!IsBuildTargetSupported(report.summary.platform))
            {
                return;
            }

            DebugLogger.Log("[XcodeBuildProcess] Initiating post-build task execution.");

            // update cached information
            s_settings      = EssentialKitSettingsEditorUtility.DefaultSettings;
            s_buildReport   = report;

            // execute tasks
            UpdateInfoPlist();
            UpdateUnityPreprocessor();

            DebugLogger.Log("[XcodeBuildProcess] Successfully completed post-build task execution.");
        }

#endregion

#region Private methods

        private static bool IsBuildTargetSupported(BuildTarget buildTarget)
        {
            return (BuildTarget.iOS == buildTarget || BuildTarget.tvOS == buildTarget);
        }

        private static void UpdateExporterSettings()
        {
            DebugLogger.Log("[XcodeBuildProcess] Updating native plugins exporter settings.");

            bool    enableBaseExporter  = false;
            var     baseExporter        = default(NativeFeatureExporterSettings);
            var     currentPlatform     = PlatformMappingServices.GetActivePlatform();
            foreach (var exporter in NativeFeatureExporterSettings.FindAllExporters(includeInactive: true))
            {
                switch (exporter.name)
                {
                    case Defaults.kBaseExporterName:
                        exporter.IsEnabled  = false;
                        baseExporter        = exporter;
                        break;

                    case NativeFeatureType.kAddressBook:
                    case NativeFeatureType.kNativeUI:
                    case NativeFeatureType.KSharingServices:
                    case NativeFeatureType.kCloudServices:
                    case NativeFeatureType.kGameServices:
                    case NativeFeatureType.kBillingServices:
                    case NativeFeatureType.kNetworkServices:
                    case NativeFeatureType.kWebView:
                    case NativeFeatureType.kMediaServices:
                        exporter.IsEnabled  = s_settings.IsFeatureUsed(exporter.name); 
                        enableBaseExporter |= exporter.IsEnabled;     
                        break;
                       
                    
                        
                    default:
                        break;
                }
                EditorUtility.SetDirty(exporter);
            }

            // update base exporter status
            if (baseExporter)
            {
                baseExporter.IsEnabled  = enableBaseExporter;
            }
        }

        private static void UpdateInfoPlist()
        {
            DebugLogger.Log("[XcodeBuildProcess] Updating plist configuration.");

            // open plist
            string  plistPath   = s_buildReport.summary.outputPath + "/Info.plist";
            var     plist       = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            var     rootDict    = plist.root;

            // add usage permissions
            var     permissions = GetUsagePermissions();
            foreach (string key in permissions.Keys)
            {
                rootDict.SetString(key, permissions[key]);
            }

            // add LSApplicationQueriesSchemes
            string[]    appQuerySchemes = GetApplicationQueriesSchemes();
            if (appQuerySchemes.Length > 0)
            {
                PlistElementArray   array;
                if (false == rootDict.TryGetElement(InfoPlistKey.kNSQuerySchemes, out array))
                {
                    array = rootDict.CreateArray(InfoPlistKey.kNSQuerySchemes);
                }

                // add required schemes
                for (int iter = 0; iter < appQuerySchemes.Length; iter++)
                {
                    if (false == array.Contains(appQuerySchemes[iter]))
                    {
                        array.AddString(appQuerySchemes[iter]);
                    }
                }
            }

            
            // save changes to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }

        private static Dictionary<string, string> GetUsagePermissions()
        {
            var     requiredPermissionsDict     = new Dictionary<string, string>(4);
            var     permissionSettings          = s_settings.ApplicationSettings.UsagePermissionSettings;

            // add address book permission
            var     abSettings                  = s_settings.AddressBookSettings;
            if (abSettings.IsEnabled || !NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState())
            {
                requiredPermissionsDict[InfoPlistKey.kNSContactsUsage] = permissionSettings.AddressBookUsagePermission.GetDescription(NativePlatform.iOS);
            }

            
            // add sharing related permissions
            var sharingSettings = s_settings.SharingServicesSettings;
            if (sharingSettings.IsEnabled || !NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState())
            {
                // added for supporting sharing/saving to gallery when share sheet is shown
                requiredPermissionsDict[InfoPlistKey.kNSPhotoLibraryAdd]   = permissionSettings.GalleryWritePermission.GetDescription(NativePlatform.iOS);
            }

            return requiredPermissionsDict;
        }

        private static string[] GetApplicationQueriesSchemes()
        {
            var     sharingSettings = s_settings.SharingServicesSettings;
            var     schemeList      = new List<string>();
            if (sharingSettings.IsEnabled)
            {
                schemeList.Add("fb");
                schemeList.Add("twitter");
                schemeList.Add("whatsapp");
            }

            return schemeList.ToArray();
        }

#endregion

#region Misc methods

        public static void UpdateUnityPreprocessor()
        {
            
        }

#endregion

#region Nested types

        private static class Defaults
        {
            internal const string   kBaseExporterName   = "Base";
        }

#endregion
    }
}
#endif