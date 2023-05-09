using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VoxelBusters.CoreLibrary.Editor
{
    public static class UnityPackageUtility
    {
        #region Static methods

        private static string GetPackageFullPathInternal(string package)
        {
            string  assetPath   = "Assets/" + package;
            if (IOServices.DirectoryExists(assetPath))
            {
                return assetPath;
            }
            return "Packages/" + package;
        }

        #endregion

        #region Public methods

        public static string GetPackagePath(string package)
        {
            return GetPackageFullPathInternal(package);
        }

        public static string GetEditorResourcesPath(string package)
        {
            return GetPackagePath(package) + "/EditorResources";
        }

        public static string GetResourcesPath(string package)
        {
            return GetPackagePath(package) + "/Resources";
        }

        public static string GetEditorScriptsPath(string package)
        {
            return GetPackagePath(package) + "/Editor";
        }

        public static string GetRuntimeScriptsPath(string package)
        {
            return GetPackagePath(package) + "/Runtime";
        }

        #endregion
    }
}