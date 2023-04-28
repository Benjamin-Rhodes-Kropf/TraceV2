using System.IO;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class XCodePostProcessBuild
{
   
    [PostProcessBuild(0)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS) return;
        
        // ------------------ PLIST --------------------


        // ------------------ CAPABILITIES --------------------
        
        string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        ProjectCapabilityManager projCapability = new ProjectCapabilityManager(projPath, "Unity-iPhone/(entitlement file)", "Unity-iPhone");
    
        projCapability.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications | BackgroundModesOptions.BackgroundFetch);
        projCapability.AddPushNotifications(false);
        projCapability.WriteToFile();
        
        // ------------------ ADDING FRAMEWORK --------------------

        PBXProject project = new PBXProject();
        project.ReadFromFile(projPath);
        Debug.Log("PBXProject: " + project);
    }
}