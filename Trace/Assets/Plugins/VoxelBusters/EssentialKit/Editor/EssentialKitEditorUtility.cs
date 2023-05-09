using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.NativePlugins;
using VoxelBusters.CoreLibrary.Editor;

namespace VoxelBusters.EssentialKit.Editor
{
    public class EssentialKitEditorUtility
    {
        #region Constants

        private     const   string      kDefaultSettingsAssetOldPath    = "Assets/Plugins/VoxelBusters/EssentialKit/Resources/EssentialKitSettings.asset";

        #endregion

        #region Public methods

        public static void ImportEssentialResources()
        {
            var     targetPackages  = new string[]
            {
                UnityPackageUtility.GetPackagePath(CoreLibrarySettings.PackageName) + "/PackageResources/Essentials.unitypackage",
                UnityPackageUtility.GetPackagePath(NativePluginsSettings.PackageName) + "/PackageResources/Essentials.unitypackage",
                UnityPackageUtility.GetPackagePath(EssentialKitSettings.PackageName) + "/PackageResources/Essentials.unitypackage",
            };

            var     addOperation    = new AddPackageOperation(
                package: "com.voxelbusters.parser",
                callback: () =>
                {
                    RegisterForImportPackageCallbacks();
                    foreach (var package in targetPackages)
                    {
                        AssetDatabase.ImportPackage(package, false);
                    }
                    UnregisterFromImportPackageCallbacks();
                });
            addOperation.Start();
        }

        public static void ImportExtraResources()
        {
            var     targetPackages  = new string[]
            {
                UnityPackageUtility.GetPackagePath(EssentialKitSettings.PackageName) + "/PackageResources/Extras.unitypackage"
            };

            RegisterForImportPackageCallbacks();
            foreach (var package in targetPackages)
            {
                AssetDatabase.ImportPackage(package, false);
            }
            UnregisterFromImportPackageCallbacks();
        }

        #endregion

        #region Private methods

        private static void RegisterForImportPackageCallbacks()
        {
            AssetDatabase.importPackageStarted     += OnImportPackageStarted;
            AssetDatabase.importPackageCompleted   += OnImportPackageCompleted;
            AssetDatabase.importPackageCancelled   += OnImportPackageCancelled;
            AssetDatabase.importPackageFailed      += OnImportPackageFailed;
        }

        private static void UnregisterFromImportPackageCallbacks()
        {
            AssetDatabase.importPackageStarted     -= OnImportPackageStarted;
            AssetDatabase.importPackageCompleted   -= OnImportPackageCompleted;
            AssetDatabase.importPackageCancelled   -= OnImportPackageCancelled;
            AssetDatabase.importPackageFailed      -= OnImportPackageFailed;
        }

        private static bool IsFileStructureOutdated()
        {
            return IOServices.FileExists(kDefaultSettingsAssetOldPath);
        }

        private static void MigrateToNewFileStructure()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            AssetDatabase.MoveAsset(kDefaultSettingsAssetOldPath, EssentialKitSettings.DefaultSettingsAssetPath);
        }

        #endregion

        #region Callback methods

        [InitializeOnLoadMethod]
        private static void PostProcessPackage()
        {
            EditorApplication.delayCall += () =>
            {
                if (IsFileStructureOutdated())
                {
                    MigrateToNewFileStructure();
                }
            };
        }

        private static void OnImportPackageStarted(string packageName)
        {
            Debug.Log($"[UnityPackageUtility] Started importing package: {packageName}");
        }

        private static void OnImportPackageCompleted(string packageName)
        {
            Debug.Log($"[UnityPackageUtility] Imported package: {packageName}");
        }

        private static void OnImportPackageCancelled(string packageName)
        {
            Debug.Log($"[UnityPackageUtility] Cancelled the import of package: {packageName}");
        }

        private static void OnImportPackageFailed(string packageName, string errorMessage)
        {
            Debug.Log($"[UnityPackageUtility] Failed importing package: {packageName} with error: {errorMessage}");
        }

        #endregion
    }
}