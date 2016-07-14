using UnityEngine;
using UnityEngine.SceneManagement;
using VGDC_RPG.TileMapProviders;
using VGDC_RPG.Map;

namespace VGDC_RPG.UI
{
    public class MatchSetupScript : MonoBehaviour
    {
        int teams = 2;
        int[,] amounts = new int[8, 5];
        int[,] aiamounts = new int[8, 5];
        string[] names = new string[]
        {
            "Warrior",
            "Cleric",
            "Grenadier",
            "Ranger",
            "Enemy"
        };

        bool mapGenerated = false;

        // Use this for initialization
        void Start()
        {
            //GameLogic.Instance.
        }

        // Update is called once per frame
        void Update()
        {

        }

        string mw = "32", mh = "32";

        void OnGUI()
        {
            var buttonWidth = 100;
            var buttonHeight = 30;

            if (!mapGenerated)
            {
                mw = GUI.TextField(new Rect(0, 0, buttonWidth, buttonHeight), mw);
                mh = GUI.TextField(new Rect(buttonWidth, 0, buttonWidth, buttonHeight), mh);
                if (GUI.Button(new Rect(0, buttonHeight * 1, buttonWidth, buttonHeight), "Drunk-Walk Cave"))
                {
                    GameLogic.Instance.Map = TileMap.Construct(new DrunkManCaveProvider(int.Parse(mw), int.Parse(mh)).GetTileMap());
                    mapGenerated = true;
                }
                if (GUI.Button(new Rect(0, buttonHeight * 2, buttonWidth, buttonHeight), "Perlin Landscape"))
                {
                    for (int i = 0; i < 20; i++)
                    {
                        var tc = System.Environment.TickCount;
                        GameLogic.Instance.Map = TileMap.Construct(new TestTileMapProvider(int.Parse(mw), int.Parse(mh)).GetTileMap());
                        Debug.Log("TMCT: " + (System.Environment.TickCount - tc));
                        if (GameLogic.Instance.Map.LargestIsland * 4 >= (int.Parse(mw) * int.Parse(mh)))
                        {
                            mapGenerated = true;
                            break;
                        }
                        else
                        {
                            GameLogic.Instance.Map.Destroy();
                        }
                    }
                    if (!mapGenerated)
                        Debug.LogError("Failed to generate suitable map.");
                }
            }
            else
            {
                GUI.Label(new Rect(buttonWidth / 4, 0, buttonWidth / 4, buttonHeight), teams.ToString());
                if (GUI.Button(new Rect(0, 0, buttonWidth / 4, buttonHeight), "-") && teams > 1)
                    teams--;
                else if (GUI.Button(new Rect(3 * buttonWidth / 4, 0, buttonWidth / 4, buttonHeight), "+") && teams < 8)
                    teams++;

                for (int j = 0; j < 5; j++)
                {
                    GUI.Label(new Rect(j * buttonWidth + j * 10, buttonHeight, buttonWidth, buttonHeight), names[j]);
                }
                for (int i = 0; i < teams; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        GUI.Label(new Rect(j * buttonWidth + buttonWidth / 4 + j * 10, i * buttonHeight * 3 + 2 * buttonHeight, buttonWidth / 4, buttonHeight), amounts[i, j].ToString());
                        if (GUI.Button(new Rect(j * buttonWidth + j * 10, i * buttonHeight * 3 + 2 * buttonHeight, buttonWidth / 4, buttonHeight), "-") && amounts[i, j] > 0)
                            amounts[i, j]--;
                        else if (GUI.Button(new Rect(j * buttonWidth + 3 * buttonWidth / 4 + j * 10, i * buttonHeight * 3 + 2 * buttonHeight, buttonWidth / 4, buttonHeight), "+"))
                            amounts[i, j]++;

                        GUI.Label(new Rect(j * buttonWidth + buttonWidth / 4 + j * 10, i * buttonHeight * 3 + 3 * buttonHeight, buttonWidth / 4, buttonHeight), aiamounts[i, j].ToString());
                        if (GUI.Button(new Rect(j * buttonWidth + j * 10, i * buttonHeight * 3 + 3 * buttonHeight, buttonWidth / 4, buttonHeight), "-") && aiamounts[i, j] > 0)
                            aiamounts[i, j]--;
                        else if (GUI.Button(new Rect(j * buttonWidth + 3 * buttonWidth / 4 + j * 10, i * buttonHeight * 3 + 3 * buttonHeight, buttonWidth / 4, buttonHeight), "+"))
                            aiamounts[i, j]++;
                    }
                }

                if (GUI.Button(new Rect((Screen.width - buttonWidth), 8 * buttonHeight, buttonWidth, buttonHeight), "Back"))
                    SceneManager.LoadScene("scenes/mainMenu");

                if (GUI.Button(new Rect((Screen.width - buttonWidth), 7 * buttonHeight, buttonWidth, buttonHeight), "Start"))
                {
                    //GameLogic.Instance.SetTeams(teams);

                    //for (int i = 0; i < teams; i++)
                    //{
                    //    for (int j = 0; j < amounts[i, 0]; j++)
                    //        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.WarriorPrefab, new Players.PlayerControllers.PlayerController(), i);
                    //    for (int j = 0; j < aiamounts[i, 0]; j++)
                    //        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.WarriorPrefab, new Players.PlayerControllers.DumbAIController(), i);

                    //    for (int j = 0; j < amounts[i, 1]; j++)
                    //        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.ClericPrefab, new Players.PlayerControllers.PlayerController(), i);
                    //    for (int j = 0; j < aiamounts[i, 1]; j++)
                    //        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.ClericPrefab, new Players.PlayerControllers.DumbAIController(), i);

                    //    for (int j = 0; j < amounts[i, 2]; j++)
                    //        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.GrenadierPrefab, new Players.PlayerControllers.PlayerController(), i);
                    //    for (int j = 0; j < aiamounts[i, 2]; j++)
                    //        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.GrenadierPrefab, new Players.PlayerControllers.DumbAIController(), i);

                    //    for (int j = 0; j < amounts[i, 3]; j++)
                    //        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.RangerPrefab, new Players.PlayerControllers.PlayerController(), i);
                    //    for (int j = 0; j < aiamounts[i, 3]; j++)
                    //        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.RangerPrefab, new Players.PlayerControllers.DumbAIController(), i);

                    //    for (int j = 0; j < amounts[i, 4]; j++)
                    //        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.AIPrefab, new Players.PlayerControllers.PlayerController(), i);
                    //    for (int j = 0; j < aiamounts[i, 4]; j++)
                    //        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.AIPrefab, new Players.PlayerControllers.DumbAIController(), i);

                    //    GameLogic.Instance.enabled = true;
                    //    enabled = false;
                    //}
                }
            }
        }
    }
}
