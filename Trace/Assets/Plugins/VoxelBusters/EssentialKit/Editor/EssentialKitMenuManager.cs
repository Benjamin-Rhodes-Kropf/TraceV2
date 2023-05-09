using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using VoxelBusters.CoreLibrary.Editor.NativePlugins;

namespace VoxelBusters.EssentialKit.Editor
{
    public static class EssentialKitMenuManager
    {
        #region Constants

        private const string kMenuItemPath = "Window/Voxel Busters/Native Plugins/Essential Kit";

        #endregion

        #region Menu items

        [MenuItem(kMenuItemPath + "/Open Settings")]
        public static void OpenSettings()
        {
            Selection.activeObject  = null;
            SettingsService.OpenProjectSettings("Project/Voxel Busters/Essential Kit");
        }

        [MenuItem(kMenuItemPath + "/Import Settings")]
        public static void ImportSettings()
        {
            var     settings        = UpgradeUtility.ImportSettings();

            // save
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();

            // ping this object
            Selection.activeObject  = settings;
            EditorGUIUtility.PingObject(settings);
        }

        [MenuItem(kMenuItemPath + "/Uninstall")]
        public static void Uninstall()
        {
            UninstallPlugin.Uninstall();
        }

        #endregion
    }
}