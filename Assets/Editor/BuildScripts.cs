using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.IO;

public static class BuildScript
{

    public static void BuildWin64()
    {
        var buildPath = File.ReadAllText("buildinfo.txt");
        //int i = 1;
        //while (System.IO.Directory.Exists("C:/Users/Matthew/Documents/vgdc_builds/build" + i))
        //    i++;
        string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        var error = BuildPipeline.BuildPlayer(scenes, buildPath + "/VGDC_RPG.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
        if (!string.IsNullOrEmpty(error))
            File.AppendAllText("C:/Users/Matthew/Documents/vgdc_builds/repo/buildlog.txt", "[ERROR]\n" + error);
        else
            File.AppendAllText("C:/Users/Matthew/Documents/vgdc_builds/repo/buildlog.txt", "Build successful.");
    }

    public static void BuildLinU()
    {
        var buildPath = File.ReadAllText("buildinfo.txt");
        //int i = 1;
        //while (System.IO.Directory.Exists("C:/Users/Matthew/Documents/vgdc_builds/build" + i))
        //    i++;
        string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        var error = BuildPipeline.BuildPlayer(scenes, buildPath + "/VGDC_RPG", BuildTarget.StandaloneLinuxUniversal, BuildOptions.None);
        if (!string.IsNullOrEmpty(error))
            File.AppendAllText("C:/Users/Matthew/Documents/vgdc_builds/repo/buildlog.txt", "[ERROR]\n" + error);
        else
            File.AppendAllText("C:/Users/Matthew/Documents/vgdc_builds/repo/buildlog.txt", "Build successful.");
    }
}
