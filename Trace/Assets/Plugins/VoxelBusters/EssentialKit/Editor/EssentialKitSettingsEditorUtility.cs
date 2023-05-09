using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VoxelBusters.CoreLibrary.NativePlugins;
using VoxelBusters.CoreLibrary.Editor;
using VoxelBusters.CoreLibrary.Editor.NativePlugins.UnityUI;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.EssentialKit.Editor
{
    public static class EssentialKitSettingsEditorUtility
    {
        #region Static fields

        private     static      EssentialKitSettings        s_defaultSettings       = null;

        #endregion

        #region Static properties

        public static EssentialKitSettings DefaultSettings
        {
            get
            {
                if (s_defaultSettings == null)
                {
                    s_defaultSettings = LoadDefaultSettings(throwError: false);

                    if(s_defaultSettings == null)
                        s_defaultSettings = CreateDefaultSettings();
                }
                return s_defaultSettings;
            }
            set
            {
                Assert.IsPropertyNotNull(value, nameof(value));

                // set new value
                s_defaultSettings       = value;
            }
        }

        public static bool SettingsExists
        {
            get
            {
                if (s_defaultSettings == null)
                {
                    s_defaultSettings   = LoadDefaultSettings();
                }
                return (s_defaultSettings != null);
            }
        }

        #endregion

        #region Static methods

        public static void ShowSettingsNotFoundErrorDialog()
        {
            EditorUtility.DisplayDialog(
                title: "Error",
                message: "Native plugins is not configured. Please select plugin settings file from menu and configure it according to your preference.",
                ok: "Ok");
        }

        #endregion

        #region Private static methods

        private static EssentialKitSettings CreateDefaultSettings()
        {
            string  filePath    = EssentialKitSettings.DefaultSettingsAssetPath;
            var     settings    = ScriptableObject.CreateInstance<EssentialKitSettings>();
            SetDefaultProperties(settings);

            // create file
            AssetDatabaseUtility.CreateAssetAtPath(settings, filePath);
            AssetDatabase.Refresh();

            return settings;
        }

        private static EssentialKitSettings LoadDefaultSettings(bool throwError = true)
        {
            string  filePath    = EssentialKitSettings.DefaultSettingsAssetPath;
            var     settings    = AssetDatabase.LoadAssetAtPath<EssentialKitSettings>(filePath);
            if (settings)
            {
                SetDefaultProperties(settings);
                return settings;
            }

            if (throwError)
            {
                throw Diagnostics.PluginNotConfiguredException();
            }

            return null;
        }

        private static void SetDefaultProperties(EssentialKitSettings settings)
        {
            // set properties
            var     uiCollection        = settings.NativeUISettings.CustomUICollection;
            if (uiCollection.RendererPrefab == null)
            {
                uiCollection.RendererPrefab     = UnityUIEditorUtility.LoadRendererPrefab();
            }
            if (uiCollection.AlertDialogPrefab == null)
            {
                uiCollection.AlertDialogPrefab  = UnityUIEditorUtility.LoadAlertDialogPrefab();
            }
        }

        #endregion
    }
}