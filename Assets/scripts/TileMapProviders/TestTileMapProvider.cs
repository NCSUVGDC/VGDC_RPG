using UnityEngine;

namespace VGDC_RPG.TileMapProviders
{
    public class TestTileMapProvider : TileMapProvider
    {
        private int w, h;
        private SimplexNoise sn;

        public TestTileMapProvider(int width, int height)
        {
            w = width;
            h = height;
            sn = new SimplexNoise();
        }

        public TestTileMapProvider(int width, int height, int[] seed)
        {
            w = width;
            h = height;
            sn = new SimplexNoise(seed);
        }

        public ushort[][,] GetTileMap()
        {
            ushort[][,] m = new ushort[3][,];//[width, height];

            for (int n = 0; n < 3; n++)
                m[n] = new ushort[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    float hv = sn.Noise(x / 64f, y / 64f, 0) + sn.Noise(x / 8f, y / 8f, 100) + 0.05f;
                    m[0][x, y] = (byte)(sn.Noise(x / 16f, 30, y / 16f) > .5f ? 2 : 1);
                    if (hv < 0)
                        m[1][x, y] = 4;
                    else
                    {
                        if (sn.Noise(x / 4f, 80, y / 4f) > 0)
                            m[1][x, y] = 21;
                    }
                }
            return m;
        }
    }
}