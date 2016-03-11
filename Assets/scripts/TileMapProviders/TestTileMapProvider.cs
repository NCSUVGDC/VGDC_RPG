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

        public ushort[][,] GetTileMap()
        {
            ushort[][,] m = new ushort[3][,];//[width, height];

            for (int n = 0; n < 3; n++)
                m[n] = new ushort[w, h];
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
                    m[0][x, y] = (ushort)(SimplexNoise.Noise.Generate(x / 4.0f, y / 4.0f, 200) + SimplexNoise.Noise.Generate(x / 32.0f, y / 32.0f, 200) < 0.8f ? 1 : 2);
                    if (Mathf.Abs(hv) > 1.5f)
                        m[1][x, y] = 4;
                    else
                    {
                        if (Random.value < 0.01)
                        {
                            m[1][x, y] = 3;//(ushort)(Random.value < 0.1 ? 3 : 2);
                        }
                        else if (Random.value < 0.3 && m[0][x, y] == 1)
                            m[1][x, y] = 21;
                        //else
                        //    m[0][x, y] = 1;
                    }
                }
            return m;
        }
    }
}