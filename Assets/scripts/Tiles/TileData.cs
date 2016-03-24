namespace VGDC_RPG.Tiles
{
    /// <summary>
    /// Holds information about a specific tile element in a tile map.
    /// </summary>
    public struct TileData
    {
        /// <summary>
        /// The id of the TileType in the TileLibrary.
        /// </summary>
        public ushort TileTypeID;

        /// <summary>
        /// Constructs a new TileData with the given ID.
        /// </summary>
        /// <param name="id"></param>
        public TileData(ushort id)
        {
            TileTypeID = id;
        }

        /// <summary>
        /// Returns the TileType of this tile in the TileLibrary.
        /// </summary>
        public TileType TileType
        {
            get
            {
                return TileLibrary.Get(TileTypeID);
            }
        }

        /// <summary>
        /// True if the tile can be walked on by players.
        /// </summary>
        public bool Walkable
        {
            get
            {
                return TileType.Walkable;
            }
        }
    }
}