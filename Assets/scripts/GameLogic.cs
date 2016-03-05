using UnityEngine;
using System.Collections;
using VGDC_RPG.Map;
using VGDC_RPG.TileMapProviders;
using System.Collections.Generic;

namespace VGDC_RPG
{
    public class GameLogic : MonoBehaviour
    {
        public enum GameState
        {
            ERROR = 0,
            SelectingStones,
            Main,
            GameOver
        }

        public TileMap Map { get; private set; }
        public GameObject PlayerPrefab;
        public GameObject GrenadierPrefab;
        public GameObject AIPrefab;
        public List<Players.Player>[] Players { get; private set; }
        public GameObject Camera;
        private CameraController CamScript;

        public static GameLogic Instance { get; private set; }

        public bool DoPlayerUpdates = true;

        public GameState CurrentGameState = GameState.SelectingStones;

        private int playerIndex = 0, teamIndex = 0;

        private bool npnu = false;

        public int TeamCount = 2;

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
        }

        private void SpawnPlayers()
        {
            //SpawnPlayer(GrenadierPrefab, 0);
            //for (int i = 0; i < 3; i++)
            //    SpawnPlayer(PlayerPrefab, 0);
            for (int i = 0; i < 8; i++)
                SpawnPlayer(AIPrefab, 0);

            for (int i = 0; i < 8; i++)
                SpawnPlayer(AIPrefab, 1);
        }

        private void SpawnPlayer(GameObject prefab, int team)
        {
            int attempts = 0;
            while (attempts++ < 1000)
            {
                int x = Random.Range(0, Map.Width);
                int y = Random.Range(0, Map.Height);

                if (Map[x, y].Walkable)
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

        private int i = 0;
        void Update()
        {
            DoPlayerUpdates = !Map.EditMode;

            if (Time.frameCount == 1)
                Players[0][0].Turn();

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
            if (teamIndex == 0 && playerIndex == 0 && CurrentGameState == GameState.SelectingStones)
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
                CurrentPlayer.Turn();
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
    }
}