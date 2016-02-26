using UnityEngine;
using System.Collections;
using System;
using VGDC_RPG.Tiles;

namespace VGDC_RPG.Map
{
    public class TileMap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private Tiles.Tile[] tiles;

        private TileMap(int width, int height)
        {
            Width = width;
            Height = height;
            tiles = new Tiles.Tile[width * height];
        }

        public static TileMap Construct(ushort[,] m)
        {
            var tm = new TileMap(m.GetLength(0), m.GetLength(1));
            for (int y = 0; y < 32; y++)
                for (int x = 0; x < 32; x++)
                    tm[x,y] = ConstructTile(m[x,y],x,y);
            return tm;
        }

        private static Tiles.Tile ConstructTile(int v, int x, int y)
        {
            var go = GameObject.Instantiate(GameLogic.Instance.Tiles[v - 1], new Vector3(x + 0.5f, 0, y + 0.5f), Quaternion.Euler(90, 0, 0)) as GameObject;
            return go.GetComponent<Tiles.Tile>();
        }

        public Tiles.Tile this[int i, int j]
        {
            get
            {
                return tiles[j * Width + i];
            }
            private set
            {
                tiles[j * Width + i] = value;
            }
        }
    }
}