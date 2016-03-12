using UnityEngine;
using System.Collections;
using UnityEditor;

public class BuildServerTest
{
    [MenuItem("Build/Build Server")]
    public static void BuildServer()
    {
        //var scenes = EditorBuildSettings.scenes;//.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        //Debug.Log(scenes[0].path);
        var error = BuildPipeline.BuildPlayer(new string[] { "Assets/scenes/NetworkTestServer.unity" }, "C:/Users/Matthew/Documents/VGDC_Builds/Server/VGDC_RPG.exe", BuildTarget.StandaloneWindows64, BuildOptions.Development);
    }
}
