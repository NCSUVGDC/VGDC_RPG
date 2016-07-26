using UnityEngine;

namespace VGDC_RPG.TileMapProviders
{
    public class DrunkWalkCaveProvider : TileMapProvider
    {
        int width, height;

        public DrunkWalkCaveProvider(int w, int h)
        {
            width = w;
            height = h;
        }

        public ushort[][,] GetTileMap()
        {
            ushort[][,] r = new ushort[3][,];//[width, height];

            for (int n = 0; n < 3; n++)
                r[n] = new ushort[width, height];
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    r[0][i, j] = 2;

            int x = width / 2, y = height / 2;//x = 0, y = 0;
            int t = 0;
            int ti = 0;
            while (t < width * height * 3 / 4 && ti++ < 40000)//for (int i = 0; i < 4000; i++)// (x != width - 1 || y != height - 1)
            {
                int d = Random.Range(0, 4);
                switch (d)
                {
                    case 0:
                        if (x == 0)
                            continue;
                        x--;
                        break;
                    case 1:
                        if (y == 0)
                            continue;
                        y--;
                        break;
                    case 2:
                        if (x == width - 1)
                            continue;
                        x++;
                        break;
                    case 3:
                        if (y == height - 1)
                            continue;
                        y++;
                        break;
                }
                if (r[0][x, y] != 20)
                {
                    r[0][x, y] = 20;
                    t++;
                }
            }
            if (t < width * height * 3 / 4)
                Debug.LogError("DrunkMan failed. " + t / (float)(width * height * 3 / 4));

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    if (r[0][i, j] == 2 && Random.value <= 0.1)
                        r[0][i, j] = 3;


            return r;
        }

        public Color GetInitialSunColor()
        {
            return new Color(1, 1, 1, 0.25f);
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
