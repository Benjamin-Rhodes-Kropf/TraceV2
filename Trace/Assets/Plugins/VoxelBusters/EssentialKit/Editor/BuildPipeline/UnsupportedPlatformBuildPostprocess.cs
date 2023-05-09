using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Editor.NativePlugins.Build;

namespace VoxelBusters.EssentialKit.Editor.Build
{
    [InitializeOnLoad]
    public static class UnsupportedPlatformBuildPostprocess
    {
        #region Constructors

        static UnsupportedPlatformBuildPostprocess()
        {
            // unregister from events
            BuildProcessReporter.OnPreprocessBuild -= OnPreprocessBuild;

            // register for events
            BuildProcessReporter.OnPreprocessBuild += OnPreprocessBuild;
        }

        #endregion

        #region Static methods

        public static void OnPreprocessBuild(BuildReport report)
        {
            // check whether plugin is configured
            if (!EssentialKitSettingsEditorUtility.SettingsExists || IsBuildTargetSupported(report.summary.platform))
            {
                return;
            }

            DebugLogger.Log("[EssentialKit] Initiating pre-build task execution.");

            // execute tasks
            EssentialKitBuildUtility.CreateStrippingFile(report.summary.platform);

            DebugLogger.Log("[EssentialKit] Successfully completed pre-build task execution.");
        }

        private static bool IsBuildTargetSupported(BuildTarget buildTarget)
        {
            return ((BuildTarget.iOS == buildTarget) || (BuildTarget.tvOS == buildTarget) || (BuildTarget.tvOS == buildTarget));
        }

        #endregion
    }
}