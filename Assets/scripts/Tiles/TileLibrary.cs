using System;
using System.Collections.Generic;

namespace VGDC_RPG.Tiles
{
    /// <summary>
    /// Holds a list of TileTypes.  Tile declarations go here.
    /// </summary>
    public static class TileLibrary
    {
        private static Dictionary<ushort, TileType> tiles = new Dictionary<ushort, TileType>();
        private const bool SHORE_WALKABLE = false;

        static TileLibrary()
        {
            ushort i = 0;
            tiles.Add(i++, new TileType(0, 0, 1, true, 0.0f, 0.0f, 0.0f, 0.0f, 0));

            tiles.Add(i++, new TileType(2, 0, 1));                                                  //Grass
            tiles.Add(i++, new TileType(1, 0, 1, false, 0.5f, 0, 0, 0));                            //Stone
            tiles.Add(i++, new TileType(3, 0, 4, false, 0, 0.5f, 0.5f, 1.0f));                      //Lamp


            tiles.Add(i++, new TileType(5, 1, 1, false) { ProjectileResistant = false });           //Water
            tiles.Add(i++, new TileType(4, 0, 1, SHORE_WALKABLE) { ProjectileResistant = false });
            tiles.Add(i++, new TileType(5, 0, 1, SHORE_WALKABLE) { ProjectileResistant = false });
            tiles.Add(i++, new TileType(6, 0, 1, SHORE_WALKABLE) { ProjectileResistant = false });
            tiles.Add(i++, new TileType(4, 1, 1, SHORE_WALKABLE) { ProjectileResistant = false });
            tiles.Add(i++, new TileType(6, 1, 1, SHORE_WALKABLE) { ProjectileResistant = false });
            tiles.Add(i++, new TileType(4, 2, 1, SHORE_WALKABLE) { ProjectileResistant = false });
            tiles.Add(i++, new TileType(5, 2, 1, SHORE_WALKABLE) { ProjectileResistant = false });
            tiles.Add(i++, new TileType(6, 2, 1, SHORE_WALKABLE) { ProjectileResistant = false });

            tiles.Add(i++, new TileType(7, 0, 1, SHORE_WALKABLE) { ProjectileResistant = false });
            tiles.Add(i++, new TileType(7, 1, 1, SHORE_WALKABLE) { ProjectileResistant = false });
            tiles.Add(i++, new TileType(7, 2, 1, SHORE_WALKABLE) { ProjectileResistant = false });

            tiles.Add(i++, new TileType(4, 3, 1, SHORE_WALKABLE) { ProjectileResistant = false });
            tiles.Add(i++, new TileType(5, 3, 1, SHORE_WALKABLE) { ProjectileResistant = false });
            tiles.Add(i++, new TileType(6, 3, 1, SHORE_WALKABLE) { ProjectileResistant = false });
            tiles.Add(i++, new TileType(7, 3, 1, SHORE_WALKABLE) { ProjectileResistant = false });

            tiles.Add(i++, new TileType(1, 4, 1));                                                  //Wood

            tiles.Add(i++, new TileType(2, 4, 1, true, 0.0f, 0.0f, 0.0f, 0.0f, 0));                 //GrassDec
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
