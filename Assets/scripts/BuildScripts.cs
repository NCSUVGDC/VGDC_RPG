using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;

public static class BuildScript {

	public static void BuildWin64()
    {
        int i = 1;
        while (System.IO.Directory.Exists("build" + i))
            i++;
        string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        var error = BuildPipeline.BuildPlayer(scenes, "C:/Users/Matthew/Documents/vgdc_builds/build" + i, BuildTarget.StandaloneWindows64, BuildOptions.None);
        if (!string.IsNullOrEmpty(error))
            System.Console.WriteLine(error);
    }
}
