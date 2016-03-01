using UnityEngine;
using System.Collections;
using VGDC_RPG.Map;
using VGDC_RPG.TileMapProviders;
using System.Collections.Generic;

namespace VGDC_RPG
{
    public class GameLogic : MonoBehaviour
    {
        public TileMapScript Map { get; private set; }
        public GameObject PlayerPrefab;
        public GameObject AIPrefab;
        public List<Players.UserPlayer> UserPlayers { get; private set; }
        public List<Players.AIPlayer> AIPlayers { get; private set; }
        public GameObject Camera;

        public static GameLogic Instance { get; private set; }

        private int playerIndex = 0;

        // Use this for initialization
        void Start()
        {
            Instance = this;
            Map = TileMapScript.Construct(new TestTileMapProvider(32, 32).GetTileMap());//new StaticTileMapProvider().GetTileMap());//new EmptyTileMapProvider(32, 32, 1).GetTileMap());//
            UserPlayers = new List<Players.UserPlayer>();
            AIPlayers = new List<Players.AIPlayer>();
            SpawnPlayers();
        }

        private void SpawnPlayers()
        {
            for (int i = 0; i < 4; i++)
                SpawnPlayer();
            for (int i = 0; i < 0; i++)
                SpawnAI();
        }

        private void SpawnPlayer()
        {
            int attempts = 0;
            while (attempts++ < 1000)
            {
                int x = Random.Range(0, Map.Width);
                int y = Random.Range(0, Map.Height);

                if (Map[x, y].Walkable)
                {
                    var player = GameObject.Instantiate(PlayerPrefab, new Vector3(x + 0.5f, 1, y + 0.5f), Quaternion.Euler(90, 0, 0)) as GameObject;
                    var s = player.GetComponent<Players.UserPlayer>();
                    s.X = x;
                    s.Y = y;
                    UserPlayers.Add(s);
                    Map.BlockTile(x, y);
                    return;
                }
            }
            Debug.LogError("Failed to spawn player after 1000 attempts.");
        }

        private void SpawnAI()
        {
            int attempts = 0;
            while (attempts++ < 1000)
            {
                int x = Random.Range(0, Map.Width);
                int y = Random.Range(0, Map.Height);

                if (Map[x, y].Walkable)
                {
                    var player = GameObject.Instantiate(AIPrefab, new Vector3(x + 0.5f, 1, y + 0.5f), Quaternion.Euler(90, 0, 0)) as GameObject;
                    var s = player.GetComponent<Players.AIPlayer>();
                    s.X = x;
                    s.Y = y;
                    AIPlayers.Add(s);
                    Map.BlockTile(x, y);
                    return;
                }
            }
            Debug.LogError("Failed to spawn player after 1000 attempts.");
        }

        private int i = 0;
        void Update()
        {
            if (Time.frameCount == 1)
                UserPlayers[0].Turn();

            /*foreach (var p in Players)
                if (p.IsMoving)
                    return;
            if (Time.frameCount % 240 == 0)
            {
                var ctc = System.Environment.TickCount;
                Map.ClearSelection();
                Debug.Log("Clear: " + (System.Environment.TickCount - ctc));

                var p = Players[i++ % Players.Count];
                Camera.transform.position = new Vector3(p.X + 0.5f, 10, p.Y + 0.5f);
                ctc = System.Environment.TickCount;
                var tiles = PathFinder.FindHighlight(Map, new Int2(p.X, p.Y), 8);
                Debug.Log("Find HL: " + (System.Environment.TickCount - ctc));
                ctc = System.Environment.TickCount;
                foreach (var t in tiles)
                    Map.SelectedTile(t.X, t.Y);
                Debug.Log("Sel Tile A: " + (System.Environment.TickCount - ctc));
                ///
                ctc = System.Environment.TickCount;
                tiles = PathFinder.FindPath(Map, new Int2(p.X, p.Y), tiles[Random.Range(0,tiles.Count)]);//new Int2(Random.Range(0, Map.Width), Random.Range(0, Map.Height)));
                Debug.Log("Find Path: " + (System.Environment.TickCount - ctc));
                ctc = System.Environment.TickCount;
                if (tiles != null)
                    foreach (var t in tiles)
                        Map.SelectedTile(t.X, t.Y);
                p.Move(tiles);
                Debug.Log("Sel Tile B: " + (System.Environment.TickCount - ctc));
                ///

                ctc = System.Environment.TickCount;
                Map.ApplySelection();
                Debug.Log("Apply: " + (System.Environment.TickCount - ctc));
            }*/
        }

        public void NextPlayer()
        {
            Map.ClearSelection();
            playerIndex = (playerIndex + 1) % (UserPlayers.Count + AIPlayers.Count);
            if (playerIndex < UserPlayers.Count)
                UserPlayers[playerIndex].Turn();
            else
                AIPlayers[playerIndex - UserPlayers.Count].Turn();
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
    }
}