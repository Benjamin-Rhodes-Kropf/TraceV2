using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class SceneItem : Editor
{

	[MenuItem ("Scenes/Trace/Main")]
	public static void LoadTraceMainScene ()
	{
		Open_Scene ("Assets/Scenes/Main");
	}
	
	[MenuItem ("Scenes/V3MapDemoScene")]
	public static void LoadMapsDemoScene ()
	{
		Open_Scene ("Assets/_ThirdParty/Infinity Code/Online maps/Examples/Scenes/Features/Demo");
	}


	static void Open_Scene (string scene_name)
	{
		if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ()) {
			EditorSceneManager.OpenScene ($"{scene_name}.unity");
		}
	}
}
