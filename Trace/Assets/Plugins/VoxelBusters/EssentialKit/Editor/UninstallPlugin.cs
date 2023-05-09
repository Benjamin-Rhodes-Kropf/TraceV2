using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using VoxelBusters.EssentialKit;

namespace VoxelBusters.EssentialKit.Editor
{
	public class UninstallPlugin
	{
		#region Constants
	
		private const	string		kUninstallAlertTitle		= "Uninstall - Cross Platform Native Plugins : Essential Kit";
		
        private const	string		kUninstallAlertMessage		= "Backup before doing this step to preserve changes done in this plugin. This deletes files only related to Cross Platform Native Plugins : Essential Kit plugin. Do you want to proceed?";

		private	static	readonly	string[]	kPluginFolders	= new string[]
		{
            EssentialKitPackageLayout.ExtrasPath
        };
		
		#endregion	
	
		#region Static methods
	
		public static void Uninstall()
		{
			bool	confirmationStatus		= EditorUtility.DisplayDialog(kUninstallAlertTitle, kUninstallAlertMessage, "Uninstall", "Cancel");
			if (confirmationStatus)
			{
				foreach (string folder in kPluginFolders)
				{
                    string	absolutePath	= Application.dataPath + "/../" + folder;
                    FileUtil.DeleteFileOrDirectory(absolutePath);
                    FileUtil.DeleteFileOrDirectory(absolutePath + ".meta");
				}
				
				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog("Cross Platform Native Plugins : Essential Kit",
				                            "Uninstall successful!", 
				                            "Ok");
			}
		}
		
		#endregion
	}
}