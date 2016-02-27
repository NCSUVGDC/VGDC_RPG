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
        public List<Players.Player> Players { get; private set; }
        public GameObject Camera;

        public static GameLogic Instance { get; private set; }

        // Use this for initialization
        void Start()
        {
            Instance = this;
            Map = TileMapScript.Construct(new TestTileMapProvider(32, 32).GetTileMap());
            Players = new List<Players.Player>();
            SpawnPlayers();
        }

        private void SpawnPlayers()
        {
            for (int i = 0; i < 4; i++)
            {
                SpawnPlayer();
            }
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
                    var s = player.GetComponent<Players.Player>();
                    s.X = x;
                    s.Y = y;
                    Players.Add(s);
                    Map.BlockTile(x, y);
                    return;
                }
            }
            Debug.LogError("Failed to spawn player after 1000 attempts.");
        }

        private void ClearHighlights()
        {
            Map.ClearHighlights();
        }

        private int i = 0;
        void Update()
        {
            /*for (int y = 0; y < Map.Height; y++)
                for (int x = 0; x < Map.Width / 3; x++)
                    Map[x, y].State = VGDC_RPG.Tiles.TileState.Highlighted;
            for (int y = 0; y < Map.Height; y++)
                for (int x = Map.Width / 3; x < 2 * Map.Width / 3; x++)
                    Map[x, y].State = VGDC_RPG.Tiles.TileState.Selected;*/

            foreach (var p in Players)
                if (p.IsMoving)
                    return;
            if (Time.frameCount % 240 == 0)
            {
                var ctc = System.Environment.TickCount;
                Map.ClearHighlights();
                Debug.Log("Clear: " + (System.Environment.TickCount - ctc));

                var p = Players[i++ % 4];
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
            }
        }
    }
}