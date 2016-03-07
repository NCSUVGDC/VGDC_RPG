using UnityEngine;
using System.Collections;

namespace VGDC_RPG.Tiles
{
    /// <summary>
    /// Holds information about a specific tile type.
    /// </summary>
    public struct TileType
    {
        /// <summary>
        /// True if players can move onto this tile.
        /// </summary>
        public bool Walkable;
        /// <summary>
        /// True if this tile stops projectiles.
        /// </summary>
        public bool ProjectileResistant;
        /// <summary>
        /// The cost of walking onto this tile used by pathfinding.
        /// </summary>
        public int MovementCost;
        /// <summary>
        /// How much light this block absorbs.
        /// </summary>
        public float Opacity;
        public float EmissionR, EmissionG, EmissionB;
        /// <summary>
        /// The data used when rendering this tile.  The alpha component should always be 0 here.
        /// The R and G components are the u and v values in texels of the atlas tile position, B is the number of frames / the atlas size.
        /// </summary>
        public Color RenderData;

        /// <summary>
        /// Constructs a new TileType.
        /// </summary>
        /// <param name="u">The position of this tile in the atlas in the x direction.</param>
        /// <param name="v">The position of this tile in the atlas in the y direction.</param>
        /// <param name="frames">The number of frames the tile has.</param>
        /// <param name="walkable">True if this tile can be walked on by players.</param>
        public TileType(int u, int v, int frames, bool walkable, float opacity, float emissionR, float emissionG, float emissionB)
        {
            Walkable = walkable;
            ProjectileResistant = !walkable;
            MovementCost = 1;
            Opacity = opacity;
            EmissionR = emissionR;
            EmissionG = emissionG;
            EmissionB = emissionB;
            RenderData = new Color(u, v, frames, 0) / Constants.ATLAS_SIZE;
        }

        /// <summary>
        /// Constructs a new TileType that is walkable.
        /// </summary>
        /// <param name="u">The position of this tile in the atlas in the x direction.</param>
        /// <param name="v">The position of this tile in the atlas in the y direction.</param>
        /// <param name="frames">The number of frames the tile has.</param>
        public TileType(int u, int v, int frames, bool walkable) : this(u, v, frames, walkable, 0.1f, 0, 0, 0)
        {

        }

        /// <summary>
        /// Constructs a new TileType that is walkable.
        /// </summary>
        /// <param name="u">The position of this tile in the atlas in the x direction.</param>
        /// <param name="v">The position of this tile in the atlas in the y direction.</param>
        /// <param name="frames">The number of frames the tile has.</param>
        public TileType(int u, int v, int frames) : this(u, v, frames, true)
        {

        }
    }
}