using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using System;
using System.Collections.Generic;

public class ScriptBatch : MonoBehaviour
{
    public static void MyBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        List<string> enabledScenePathNames = new List<string>();

        // Grab all of the scenes listed in the Editor Build Settings
        foreach (var buildSettingScene in EditorBuildSettings.scenes)
        {
           Scene scene = EditorSceneManager.OpenScene(buildSettingScene.path);
           GameObject[] sceneObjects = scene.GetRootGameObjects();
           
           // check the game objects in the scene for the build manager
           foreach (var gameObject in sceneObjects)
           {
               if (gameObject.tag == "BuildManager" && gameObject.GetComponent<BuildManagerScript>().isSceneBuildable) 
               {   
                   // Add a scene to the list of buildable scenes if it is checked with "isSceneBuildable" in its BuildManager GameObject
                   Debug.Log("Adding Buildable Scene to build list:");
                   Debug.Log(scene.name);
                   enabledScenePathNames.Add(buildSettingScene.path);
               }
           }  
        }
        buildPlayerOptions.scenes = enabledScenePathNames.ToArray();
        buildPlayerOptions.locationPathName = "Web Build from Command Line";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;


        // Run the build and save the results in the report
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
           // this could provide more information like "x number of scenes built out of y total"
           Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
           Debug.Log("Build failed");
        }
    }
}