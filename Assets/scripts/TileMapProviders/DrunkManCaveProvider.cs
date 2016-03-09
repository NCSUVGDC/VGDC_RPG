using UnityEngine;

namespace VGDC_RPG.TileMapProviders
{
    public class DrunkManCaveProvider : TileMapProvider
    {
        int width, height;

        public DrunkManCaveProvider(int w, int h)
        {
            width = w;
            height = h;
        }

        public ushort[,] GetTileMap()
        {
            ushort[,] r = new ushort[width, height];

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    r[i, j] = 2;

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
                if (r[x, y] != 20)
                {
                    r[x, y] = 20;
                    t++;
                }
            }
            if (t < width * height * 3 / 4)
                Debug.LogError("DrunkMan failed. " + t / (float)(width * height * 3 / 4));

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    if (r[i, j] == 2 && Random.value <= 0.1)
                        r[i, j] = 3;


            return r;
        }
    }
}
