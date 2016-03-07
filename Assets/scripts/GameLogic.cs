using UnityEngine;
using System.Collections;
using VGDC_RPG.Map;
using VGDC_RPG.TileMapProviders;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace VGDC_RPG
{
    public class GameLogic : MonoBehaviour
    {
        public enum GameState
        {
            ERROR = 0,
            SelectingStones,
            Main,
            GameOver,
        }

        public TileMap Map { get; private set; }
        public GameObject WarriorPrefab;
        public GameObject GrenadierPrefab;
        public GameObject ClericPrefab;
        public GameObject RangerPrefab;
        public GameObject AIPrefab;
        public List<Players.Player>[] Players { get; private set; }
        public GameObject Camera;
        private CameraController CamScript;

        public static GameLogic Instance { get; private set; }

        public bool DoPlayerUpdates = true;

        public GameState CurrentGameState = GameState.SelectingStones;

        private int playerIndex = -1, teamIndex = 0;

        private bool npnu = true;

        public int TeamCount = 2;

        private int tt;
        private int wt = -1;
        private bool menuOpen;

        // Use this for initialization
        void Start()
        {
            Instance = this;
            Map = TileMap.Construct(new TestTileMapProvider(64, 64).GetTileMap());//new SavedTileMapProvider("test1").GetTileMap());//new EmptyTileMapProvider(32, 32, 1).GetTileMap());//new StaticTileMapProvider().GetTileMap());//
            Players = new List<Players.Player>[TeamCount];
            for (int i = 0; i < TeamCount; i++)
                Players[i] = new List<Players.Player>();
            SpawnPlayers();
            CamScript = Camera.GetComponent<CameraController>();


            /*var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            PathFinder.FindPath(Map, new Int2(10, 10), new Int2(53, 53));
            sw.Stop();
            Debug.Log("SPF: " + sw.ElapsedMilliseconds);
            sw.Reset();
            sw.Start();
            System.GC.Collect();
            sw.Stop();
            Debug.Log("GC: " + sw.ElapsedMilliseconds);

            sw.Reset();
            sw.Start();
            VGDC_RPG.Map.Pathfinding.AStarSearch.FindPath(Map, new Int2(10, 10), new Int2(53, 53));
            sw.Stop();
            Debug.Log("ASPF: " + sw.ElapsedMilliseconds);
            sw.Reset();
            sw.Start();
            System.GC.Collect();
            sw.Stop();
            Debug.Log("GC: " + sw.ElapsedMilliseconds);

            sw.Reset();
            sw.Start();
            VGDC_RPG.Map.Pathfinding.AStarSearch.FindHighlight(Map, new Int2(10, 10), 8);
            sw.Stop();
            Debug.Log("SHF: " + sw.ElapsedMilliseconds);
            sw.Reset();
            sw.Start();
            System.GC.Collect();
            sw.Stop();
            Debug.Log("GC: " + sw.ElapsedMilliseconds);

            sw.Reset();
            sw.Start();
            VGDC_RPG.Map.Pathfinding.AStarSearch.FindHighlight(Map, new Int2(10, 10), 8);
            sw.Stop();
            Debug.Log("ASHF: " + sw.ElapsedMilliseconds);
            sw.Reset();
            sw.Start();
            System.GC.Collect();
            sw.Stop();
            Debug.Log("GC: " + sw.ElapsedMilliseconds);*/
        }

        private void SpawnPlayers()
        {
            SpawnPlayer(GrenadierPrefab, 1);
            //SpawnPlayer(ClericPrefab, 0);
            //SpawnPlayer(WarriorPrefab, 0);
            SpawnPlayer(RangerPrefab, 0);
            //for (int i = 0; i < 3; i++)
            //    SpawnPlayer(PlayerPrefab, 0);
            //for (int i = 0; i < 8; i++)
            //    SpawnPlayer(AIPrefab, 0);

            //for (int i = 0; i < 2; i++)
            //    SpawnPlayer(AIPrefab, 1);
        }

        private void SpawnPlayer(GameObject prefab, int team)
        {
            int attempts = 0;
            while (attempts++ < 1000)
            {
                int x = Random.Range(0, Map.Width);
                int y = Random.Range(0, Map.Height);

                if (Map.InSpawn(x, y))
                {
                    var player = GameObject.Instantiate(prefab, new Vector3(x + 0.5f, 1, y + 0.5f), Quaternion.Euler(90, 0, 0)) as GameObject;
                    var s = player.GetComponent<Players.Player>();
                    s.X = x;
                    s.Y = y;
                    s.TeamID = team;
                    Players[team].Add(s);
                    Map.BlockTile(x, y);
                    return;
                }
            }
            Debug.LogError("Failed to spawn player after 1000 attempts.");
        }
        
        void Update()
        {
            DoPlayerUpdates = !Map.EditMode && !menuOpen;

            if (Input.GetKeyDown(KeyCode.Escape))
                menuOpen = !menuOpen;

            if (npnu)
            {
                npnu = false;
                NextPlayer();
            }
        }

        public void NextPlayer()
        {
            Map.ClearSelection();
            if (CheckWin())
                return;
            playerIndex++;
            while (playerIndex >= Players[teamIndex].Count)
            {
                playerIndex = 0;
                teamIndex = (teamIndex + 1) % TeamCount;
            }
            if (teamIndex == 0 && playerIndex == 0 && CurrentGameState == GameState.SelectingStones && tt > 0)
                CurrentGameState = GameState.Main;
            turns = 0;
            NextTurn();
        }

        private bool CheckWin()
        {
            int teamsAlive = 0;
            int lta = -1;
            for (int i = 0; i < TeamCount; i++)
                if (Players[i].Count > 0)
                {
                    teamsAlive++;
                    lta = i;
                }
            if (teamsAlive <= 1)
            {
                CurrentGameState = GameState.GameOver;
                Debug.Log("Game Over: Team " + lta);
                wt = lta;
                return true;
            }
            return false;
        }

        public Players.Player CurrentPlayer
        {
            get
            {
                return Players[teamIndex][playerIndex];
            }
        }

        private int turns = 1;
        public void NextTurn()
        {
            Map.ClearSelection();
            turns++;
            if (turns > CurrentPlayer.ActionPoints)
                npnu = true;//NextPlayer();
            else
            {
                CurrentPlayer.Turn(turns);
                tt++;
            }
            CamScript.TargetPosition = new Vector3(CurrentPlayer.X + 0.5f, 10, CurrentPlayer.Y + 0.5f);
        }

        public Int2 GetScreenTile(float x, float y)
        {
            x -= GameLogic.Instance.Camera.GetComponent<Camera>().pixelWidth / 2.0f;
            y -= GameLogic.Instance.Camera.GetComponent<Camera>().pixelHeight / 2.0f;
            x /= 64.0f * CameraController.Zoom;
            y /= 64.0f * CameraController.Zoom;
            x += GameLogic.Instance.Camera.transform.position.x;
            y += GameLogic.Instance.Camera.transform.position.z;


            //Debug.Log("MP: " + x + ", " + y);
            //Debug.Log("SS: " + Screen.width + ", " + Screen.height);

            return new Int2(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        public void RemovePlayer(Players.Player player)
        {
            if (Players[player.TeamID].Contains(player))
                Players[player.TeamID].Remove(player);
            else
                throw new System.Exception("Player not found in list.");
        }

        void OnGUI()
        {
            var buttonWidth = 200;
            var buttonHeight = 30;
            if (CurrentGameState == GameState.GameOver)
            {
                GUI.Label(new Rect((Screen.width - 200) / 2, Screen.height / 2 - 30, 200, 20), "Game Over. Team " + wt + " wins!");
                if (GUI.Button(new Rect((Screen.width - buttonWidth) / 2, Screen.height / 2, buttonWidth, buttonHeight), "Main Menu"))
                    SceneManager.LoadScene("mainMenu");
            }
            else if (menuOpen)
            {
                if (GUI.Button(new Rect((Screen.width - buttonWidth) / 2, Screen.height / 2, buttonWidth, buttonHeight), "Main Menu"))
                    SceneManager.LoadScene("mainMenu");
            }
            else if (CurrentGameState == GameState.Main || CurrentGameState == GameState.SelectingStones)
            {
                GUI.Label(new Rect((Screen.width - 200) / 2 - 1, 9, 200, 20), "Team " + teamIndex + "  |  " + Players[teamIndex][playerIndex].GUIName + "  |  Turn " + turns, new GUIStyle() { alignment = TextAnchor.UpperCenter });
                GUI.Label(new Rect((Screen.width - 200) / 2, 10, 200, 20), "Team " + teamIndex + "  |  " + Players[teamIndex][playerIndex].GUIName + "  |  Turn " + turns, new GUIStyle() { alignment = TextAnchor.UpperCenter, normal = new GUIStyleState() { textColor = Color.white } });
            }
        }

        public static void SpawnText(string text, float x, float y, Color color)
        {
            var tgo = GameObject.Instantiate(Resources.Load("textobj")) as GameObject;
            tgo.transform.position += new Vector3(x, 0, y);
            tgo.GetComponent<TextMesh>().text = text;
            tgo.GetComponent<TextScript>().Color = color;
        }
    }
}