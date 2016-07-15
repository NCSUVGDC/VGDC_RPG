using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VGDC_RPG.Map;
using VGDC_RPG.TileMapProviders;

namespace VGDC_RPG
{
    public class GameLogic
    {
        public static GameLogic Instance;

        public TileMap Map;
        public GameObject Camera;

        public ushort[][,] mapConstructionData;

        public bool IsHost, IsServer;

        public GameLogic()
        {
            Instance = this;
            Camera = GameObject.Find("CameraObject");
        }

        public Int2 GetScreenTile(float x, float y)
        {
            x -= Camera.GetComponent<Camera>().pixelWidth / 2.0f;
            y -= Camera.GetComponent<Camera>().pixelHeight / 2.0f;
            x /= 64.0f * CameraController.Zoom;
            y /= 64.0f * CameraController.Zoom;
            x += Camera.transform.position.x;
            y += Camera.transform.position.z;

            return new Int2(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        }

        public void GenerateTestMap(int width, int height)
        {
            for (int i = 0; i < 20; i++)
            {
                var tc = Environment.TickCount;
                mapConstructionData = new TestTileMapProvider(width, height).GetTileMap();
                var map = TileMap.Construct(mapConstructionData);
                Debug.Log("TMCT: " + (Environment.TickCount - tc));
                if (map.LargestIsland * 4 >= (width * height))
                {
                    Map = map;
                    break;
                }
                else
                {
                    map.Destroy();
                }
            }
            if (Map == null)
                Debug.LogError("Failed to generate suitable map after 20 attemps.");
        }

        public void SetMap(ushort[][,] data)
        {
            mapConstructionData = data;
            Map = TileMap.Construct(mapConstructionData);
        }
    }
}
