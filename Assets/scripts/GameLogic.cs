using UnityEngine;
using System.Collections;
using VGDC_RPG.Map;
using VGDC_RPG.TileMapProviders;
using System.Collections.Generic;

namespace VGDC_RPG
{
    public class GameLogic : MonoBehaviour
    {
        public GameObject[] Tiles;
        public TileMap Map { get; private set; }
        public GameObject PlayerPrefab;
        public List<Players.Player> Players { get; private set; }

        public static GameLogic Instance { get; private set; }

        // Use this for initialization
        void Start()
        {
            Instance = this;
            Map = TileMap.Construct(new TestTileMapProvider(64, 64).GetTileMap());
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
                    Map[x, y].ObjectOnTile = true;
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

            if (Time.frameCount % 60 == 0)
            {
                Map.ClearHighlights();

                var p = Players[i++ % 4];
                var tiles = PathFinder.FindHighlight(Map, Map[p.X, p.Y], 8);
                foreach (var t in tiles)
                    t.State = VGDC_RPG.Tiles.TileState.Highlighted;
                ///
                tiles = PathFinder.FindPath(Map, Map[p.X, p.Y], Map[Random.Range(0, Map.Width), Random.Range(0, Map.Height)]);
                if (tiles != null)
                    foreach (var t in tiles)
                        t.State = VGDC_RPG.Tiles.TileState.Selected;
                ///
            }
        }
    }
}