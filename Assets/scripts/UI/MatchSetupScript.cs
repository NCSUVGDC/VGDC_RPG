using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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

        // Use this for initialization
        void Start()
        {
            //GameLogic.Instance.
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnGUI()
        {
            var buttonWidth = 100;
            var buttonHeight = 30;

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
                GameLogic.Instance.SetTeams(teams);

                for (int i = 0; i < teams; i++)
                {
                    for (int j = 0; j < amounts[i, 0]; j++)
                        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.WarriorPrefab, new Players.PlayerControllers.PlayerController(), i);
                    for (int j = 0; j < aiamounts[i, 0]; j++)
                        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.WarriorPrefab, new Players.PlayerControllers.DumbAIController(), i);

                    for (int j = 0; j < amounts[i, 1]; j++)
                        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.ClericPrefab, new Players.PlayerControllers.PlayerController(), i);
                    for (int j = 0; j < aiamounts[i, 1]; j++)
                        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.ClericPrefab, new Players.PlayerControllers.DumbAIController(), i);

                    for (int j = 0; j < amounts[i, 2]; j++)
                        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.GrenadierPrefab, new Players.PlayerControllers.PlayerController(), i);
                    for (int j = 0; j < aiamounts[i, 2]; j++)
                        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.GrenadierPrefab, new Players.PlayerControllers.DumbAIController(), i);

                    for (int j = 0; j < amounts[i, 3]; j++)
                        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.RangerPrefab, new Players.PlayerControllers.PlayerController(), i);
                    for (int j = 0; j < aiamounts[i, 3]; j++)
                        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.RangerPrefab, new Players.PlayerControllers.DumbAIController(), i);

                    for (int j = 0; j < amounts[i, 4]; j++)
                        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.AIPrefab, new Players.PlayerControllers.PlayerController(), i);
                    for (int j = 0; j < aiamounts[i, 4]; j++)
                        GameLogic.Instance.SpawnPlayer(GameLogic.Instance.AIPrefab, new Players.PlayerControllers.DumbAIController(), i);

                    GameLogic.Instance.enabled = true;
                    enabled = false;
                }
            }
        }
    }
}
