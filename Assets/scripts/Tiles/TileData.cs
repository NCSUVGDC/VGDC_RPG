using UnityEngine;
using System.Collections;

namespace VGDC_RPG.Tiles
{
    public struct TileData
    {
        public bool ObjectOnTile;
        public ushort TileTypeID;

        public TileData(ushort id)
        {
            TileTypeID = id;
            ObjectOnTile = false;
        }

        public TileType TileType
        {
            get
            {
                return TileLibrary.Get(TileTypeID);
            }
        }

        public bool Walkable
        {
            get
            {
                return !ObjectOnTile && TileType.Walkable;
            }
        }
    }
}