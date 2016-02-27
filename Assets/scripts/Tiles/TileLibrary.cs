using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGDC_RPG.Tiles
{
    public static class TileLibrary
    {
        private static Dictionary<int, TileType> tiles = new Dictionary<int, TileType>();

        static TileLibrary()
        {
            int i = 1;
            tiles.Add(i++, new TileType(2, 0, 1));
            tiles.Add(i++, new TileType(1, 0, 1, false));
            tiles.Add(i++, new TileType(3, 0, 4, false));


            tiles.Add(i++, new TileType(5, 1, 1, false));
            tiles.Add(i++, new TileType(4, 0, 1));
            tiles.Add(i++, new TileType(5, 0, 1));
            tiles.Add(i++, new TileType(6, 0, 1));
            tiles.Add(i++, new TileType(4, 1, 1));
            tiles.Add(i++, new TileType(6, 1, 1));
            tiles.Add(i++, new TileType(4, 2, 1));
            tiles.Add(i++, new TileType(5, 2, 1));
            tiles.Add(i++, new TileType(6, 2, 1));

            tiles.Add(i++, new TileType(7, 0, 1));
            tiles.Add(i++, new TileType(7, 1, 1));
            tiles.Add(i++, new TileType(7, 2, 1));

            tiles.Add(i++, new TileType(4, 3, 1));
            tiles.Add(i++, new TileType(5, 3, 1));
            tiles.Add(i++, new TileType(6, 3, 1));
            tiles.Add(i++, new TileType(7, 3, 1));
        }

        public static TileType Get(ushort id)
        {
            return tiles[id];
        }
    }
}
