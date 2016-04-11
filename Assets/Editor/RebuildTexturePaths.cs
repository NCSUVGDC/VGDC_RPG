using UnityEditor;
using System;
using System.IO;
using UnityEngine;
using VGDC_RPG;
using System.Text;

public static class RebuildTexturePaths
{
    [MenuItem("Sprites/Rebuild Player GUIDs")]
    static void RebuildPlayerGUIDs()
    {
        SaveGUIDs("Idle/Cleric");
        SaveGUIDs("Walking/Cleric");

        SaveGUIDs("Idle/Warrior");
        SaveGUIDs("Walking/Warrior");

        SaveGUIDs("Idle/Ranger");
        SaveGUIDs("Walking/Ranger");

        SaveGUIDs("Idle/Grenadier");
        SaveGUIDs("Walking/Grenadier");

        SaveGUIDs("Idle/Robot");
        SaveGUIDs("Walking/Robot");
    }

    private static void SaveGUIDs(string path)
    {
        var subDirs = new String[] { "Front", "Back", "Left", "Right" };
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < subDirs.Length; i++)
        {
            var guids = AssetDatabase.FindAssets("t:texture2D", new string[] { "Assets/resources/" + path + "/" + subDirs[i] });
            
            sb.Append(guids.Length + "\n");
            var sl = "Assets/resources/".Length;
            foreach (var g in guids)
            {
                var s = AssetDatabase.GUIDToAssetPath(g);
                sb.Append(s.Substring(sl, s.Length - sl - 4) + "\n");

                var importer = AssetImporter.GetAtPath(s) as TextureImporter;
                importer.textureType = TextureImporterType.Advanced;
                importer.filterMode = FilterMode.Point;
                importer.mipmapEnabled = true;
                importer.mipmapFilter = TextureImporterMipFilter.KaiserFilter;
                importer.SaveAndReimport();
            }
            if (guids.Length == 0)
                Debug.LogError("No sprites found for: " + path + "/" + subDirs[i]);
            
        }
        File.WriteAllText(Application.dataPath + "/resources/" + path.Replace('/', '_') + ".txt", sb.ToString());
        AssetDatabase.Refresh();
    }
}
