using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary.Editor;

namespace VoxelBusters.EssentialKit.Editor
{
    internal static class EssentialKitPackageLayout
    {
        public static string ExtrasPath { get { return "Assets/Plugins/VoxelBusters/EssentialKit"; } }

        public static string EditorExtrasPath { get { return ExtrasPath + "/Editor"; } }

        public static string IosPluginPath { get { return ExtrasPath + "/Plugins/iOS"; } }

        public static string AndroidPluginPath { get { return "Assets/Plugins/Android"; } }

        public static string EditorResourcesPath { get { return ExtrasPath + "/EditorResources"; } }

        // android
        public static string AndroidEditorSourcePath { get { return ExtrasPath + "/Editor" + "/Android"; } }
        public static string AndroidProjectFolderName { get { return "com.voxelbusters.essentialkit.androidlib"; } }
        public static string AndroidProjectPath { get { return AndroidPluginPath + "/" + AndroidProjectFolderName; } }
        public static string AndroidProjectAllLibsPath { get { return AndroidProjectPath + "/all_libs"; } }
        public static string AndroidProjectLibsPath { get { return AndroidProjectPath + "/libs"; } }
        public static string AndroidProjectResPath { get { return AndroidProjectPath + "/res"; } }
        public static string AndroidProjectResDrawablePath { get { return AndroidProjectResPath + "/drawable"; } }
        public static string AndroidProjectResValuesPath { get { return AndroidProjectResPath + "/values"; } }
        public static string AndroidProjectResValuesStringsPath { get { return AndroidProjectResValuesPath + "/essential_kit_strings.xml"; } }
    }
}