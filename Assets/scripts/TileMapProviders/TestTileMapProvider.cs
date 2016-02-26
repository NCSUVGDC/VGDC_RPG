using UnityEngine;
using VGDC_RPG.Map;

namespace VGDC_RPG.TileMapProviders
{
    public class TestTileMapProvider : TileMapProvider
    {
        public TestTileMapProvider()
        {

        }

        public ushort[,] GetTileMap()
        {
            var m = new ushort[32, 32];
            for (int y = 0; y < 32; y++)
                for (int x = 0; x < 32; x++)
                    if (Random.value < 0.8)
                        m[x, y] = 1;
                    else if (Random.value < 0.1)
                        m[x, y] = 3;
                    else
                        m[x, y] = 2;
            return m;
        }
    }
}