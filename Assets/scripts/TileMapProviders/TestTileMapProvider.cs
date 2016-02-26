using UnityEngine;
using VGDC_RPG.Map;

namespace VGDC_RPG.TileMapProviders
{
    public class TestTileMapProvider : TileMapProvider
    {
        private int w, h;

        public TestTileMapProvider(int width, int height)
        {
            w = width;
            h = height;
        }

        public ushort[,] GetTileMap()
        {
            var m = new ushort[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
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