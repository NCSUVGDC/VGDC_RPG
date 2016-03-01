using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGDC_RPG.Tiles
{
    /// <summary>
    /// Holds a list of TileTypes.  Tile declarations go here.
    /// </summary>
    public static class TileLibrary
    {
        private static Dictionary<int, TileType> tiles = new Dictionary<int, TileType>();

        static TileLibrary()
        {
            int i = 1;
            tiles.Add(i++, new TileType(2, 0, 1));
            tiles.Add(i++, new TileType(1, 0, 1, false, 0.5f, 1, 0, 0));
            tiles.Add(i++, new TileType(3, 0, 4, false, 0, 0.5f, 0.5f, 1.0f));


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

        /// <summary>
        /// Returns the TileType with the given ID.
        /// </summary>
        /// <param name="id">The ID of the tile.</param>
        /// <returns>The TileType with the given ID.</returns>
        public static TileType Get(ushort id)
        {
            if (!tiles.ContainsKey(id))
                throw new ArgumentException("Given TileType ID was not found in the TileLibrary.", "id");
            return tiles[id];
        }
    }
}
