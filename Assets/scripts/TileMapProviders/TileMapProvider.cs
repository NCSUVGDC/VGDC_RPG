using UnityEngine;
using System.Collections;
using VGDC_RPG.Map;

namespace VGDC_RPG.TileMapProviders
{
    public interface TileMapProvider
    {
        ushort[,] GetTileMap();
    }
}