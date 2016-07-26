using UnityEditor;
using System.IO;
using UnityEngine;
using System.Text;

public static class RebuildTexturePaths
{
    static int progressbarTotal = 5 * 2 * 4;
    static int progressbarCurrent;

    [MenuItem("Sprites/Rebuild Player GUIDs")]
    static void RebuildPlayerGUIDs()
    {
        progressbarCurrent = 0;
        Debug.Log("Starting Player GUID Rebuild...");

        if (SaveGUIDs("Idle/Cleric")
        && SaveGUIDs("Walking/Cleric")

        && SaveGUIDs("Idle/Warrior")
        && SaveGUIDs("Walking/Warrior")

        && SaveGUIDs("Idle/Ranger")
        && SaveGUIDs("Walking/Ranger")

        && SaveGUIDs("Idle/Grenadier")
        && SaveGUIDs("Walking/Grenadier")

        && SaveGUIDs("Idle/Robot")
        && SaveGUIDs("Walking/Robot"))
            Debug.Log("Player GUID Rebuild Complete.");
        else
            Debug.Log("Player GUID Rebuild Cancelled.");
        EditorUtility.ClearProgressBar();
    }

    private static bool SaveGUIDs(string path)
    {
        var subDirs = new string[] { "Front", "Back", "Left", "Right" };
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < subDirs.Length; i++)
        {
            EditorUtility.DisplayCancelableProgressBar("Rebuilding GUIDs...", path + ":" + subDirs[i], progressbarCurrent++ / (float)progressbarTotal);

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

                if (EditorUtility.DisplayCancelableProgressBar("Rebuilding GUIDs...", path + ":" + subDirs[i] + " - " + g, progressbarCurrent / (float)progressbarTotal))
                    return false;
            }
            if (guids.Length == 0)
                Debug.LogError("No sprites found for: " + path + "/" + subDirs[i]);
        }
        File.WriteAllText(Application.dataPath + "/resources/" + path.Replace('/', '_') + ".txt", sb.ToString());
        AssetDatabase.Refresh();
        return true;
    }
}
