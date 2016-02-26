using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//namespace VGDC_RPG
//{
    public class Map
    {
        private Tile[,] tiles;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Map(int width, int height)
        {
            tiles = new Tile[width, height];
        }

        public Tile this[int i, int j]
        {
            get
            {
                return tiles[i, j];
            }
            set
            {
                tiles[i, j] = value;
            }
        }
    }
//}
