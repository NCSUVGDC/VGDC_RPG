using UnityEngine;

namespace VGDC_RPG.TileMapProviders
{
    public class EmptyTileMapProvider : TileMapProvider
    {
        int w, h;
        ushort tid;

        public EmptyTileMapProvider(int width, int height, ushort tileID)
        {
            w = width;
            h = height;
            tid = tileID;
        }

        public ushort[][,] GetTileMap()
        {
            ushort[][,] m = new ushort[3][,];//[width, height];

            for (int n = 0; n < 3; n++)
                m[n] = new ushort[w, h];
            for (int j = 0; j < h; j++)
                for (int i = 0; i < w; i++)
                    m[0][i, j] = tid;
            return m;
        }
        
        public Color GetInitialSunColor()
        {
            return new Color(1, 1, 1, 0.5f);
        }

        public Color GetInitialAmbientColor()
        {
            return new Color(0.125f, 0.125f, 0.125f, 1);
        }

        public float GetInitialBrightness()
        {
            return 0.9f;
        }
    }
}
