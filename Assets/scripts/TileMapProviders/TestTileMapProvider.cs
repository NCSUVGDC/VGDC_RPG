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
                /*if (Random.value < 0.6)
                    m[x, y] = 4;
                else if (Random.value < 0.8)
                    m[x, y] = 1;
                else if (Random.value < 0.1)
                    m[x, y] = 3;
                else
                    m[x, y] = 2;*/
                {
                    var hv = SimplexNoise.Noise.Generate(x / 64.0f, y / 64.0f) * 4 + SimplexNoise.Noise.Generate(x / 8.0f, y / 8.0f);
                    if (Mathf.Abs(hv) > 2.5f)
                        m[x, y] = 4;
                    else
                    {
                        if (Random.value < 0.1)
                        {
                            m[x, y] = (ushort)(Random.value < 0.1 ? 3 : 2);
                        }
                        else
                            m[x, y] = 1;
                    }
                }
            return m;
        }
    }
}