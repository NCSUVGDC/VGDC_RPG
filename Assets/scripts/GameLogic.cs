using UnityEngine;
using System.Collections;
using VGDC_RPG.Map;
using VGDC_RPG.TileMapProviders;

namespace VGDC_RPG
{
    public class GameLogic : MonoBehaviour
    {
        public GameObject[] Tiles;
        public TileMap Map { get; private set; }

        public static GameLogic Instance { get; private set; }

        // Use this for initialization
        void Start()
        {
            Instance = this;
            Map = TileMap.Construct(new TestTileMapProvider().GetTileMap());
        }
    }
}