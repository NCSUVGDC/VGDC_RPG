using UnityEngine;
using System.Collections;

namespace VGDC_RPG.Tiles
{
    public struct TileType
    {
        public bool Walkable;
        public int MovementCost;
        public int Opacity;
        public Color RenderData;

        public TileType(int u, int v, int frames, bool walkable)
        {
            Walkable = walkable;
            MovementCost = 1;
            Opacity = 0;
            RenderData = new Color(u, v, frames, 0) / Constants.ATLAS_SIZE;
        }

        public TileType(int u, int v, int frames) : this(u, v, frames, true)
        {

        }
    }
}