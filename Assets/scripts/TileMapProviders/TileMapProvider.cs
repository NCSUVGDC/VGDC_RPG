using UnityEngine;

namespace VGDC_RPG.TileMapProviders
{
    public interface TileMapProvider
    {
        ushort[][,] GetTileMap();
        Color GetInitialSunColor();
        Color GetInitialAmbientColor();
        float GetInitialBrightness();
    }
}