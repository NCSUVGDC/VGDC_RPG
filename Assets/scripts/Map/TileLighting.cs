using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGDC_RPG.Map
{
    /// <summary>
    /// Lighting algorithm for TileMap.  Uses Breadth-First Search.
    /// </summary>
    public class TileLighting
    {
        private Queue<Int2> lightQueue;
        private Queue<Int2Distance> lightRemQueue;
        internal float[] lightData;
        private TileMapScript map;
        private static readonly float InvSqrt2 = (float)(Math.Sqrt(2));
        private bool lightDiagonal = true;

        public TileLighting(TileMapScript map)
        {
            this.map = map;
            lightData = new float[map.Width * map.Height];
            lightQueue = new Queue<Int2>();
            lightRemQueue = new Queue<Int2Distance>();
        }

        public void AddLight(int x, int y, float lightPower)
        {
            lightQueue.Enqueue(new Int2(x, y));
            SetLight(x, y, lightPower);
        }

        public void RemoveLight(int x, int y)
        {
            lightRemQueue.Enqueue(new Int2Distance(new Int2(x, y), GetLight(x, y)));
            SetLight(x, y, 0);
        }

        private void SetLight(int x, int y, float lightPower)
        {
            //UnityEngine.Debug.Log("SL: " + x + ", " + y + ":" + lightPower);
            var index = y * map.Width + x;
            lightData[index] = lightPower;
        }

        public float GetLight(int x, int y)
        {
            var index = y * map.Width + x;
            return lightData[index];
        }

        public void CalculateAdd()
        {
            while (lightQueue.Count > 0)
            {
                var n = lightQueue.Dequeue();
                var l = GetLight(n.X, n.Y);
                if (l > 0)
                {
                    if (n.X != 0 && GetLight(n.X - 1, n.Y) < l - map[n.X - 1, n.Y].TileType.Opacity)
                    {
                        //UnityEngine.Debug.Log("OP: " + map[n.X - 1, n.Y].TileType.Opacity);
                        SetLight(n.X - 1, n.Y, Math.Max(l - map[n.X - 1, n.Y].TileType.Opacity, 0));
                        lightQueue.Enqueue(new Int2(n.X - 1, n.Y));
                    }
                    if (n.X != map.Width - 1 && GetLight(n.X + 1, n.Y) < l - map[n.X + 1, n.Y].TileType.Opacity)
                    {
                        SetLight(n.X + 1, n.Y, Math.Max(l - map[n.X + 1, n.Y].TileType.Opacity, 0));
                        lightQueue.Enqueue(new Int2(n.X + 1, n.Y));
                    }
                    if (n.Y != 0 && GetLight(n.X, n.Y - 1) < l - map[n.X, n.Y - 1].TileType.Opacity)
                    {
                        SetLight(n.X, n.Y - 1, Math.Max(l - map[n.X, n.Y - 1].TileType.Opacity, 0));
                        lightQueue.Enqueue(new Int2(n.X, n.Y - 1));
                    }
                    if (n.Y != map.Height - 1 && GetLight(n.X, n.Y + 1) < l - map[n.X, n.Y + 1].TileType.Opacity)
                    {
                        SetLight(n.X, n.Y + 1, Math.Max(l - map[n.X, n.Y + 1].TileType.Opacity, 0));
                        lightQueue.Enqueue(new Int2(n.X, n.Y + 1));
                    }

                    if (lightDiagonal)
                    {
                        if (n.X != 0 && n.Y != 0 && GetLight(n.X - 1, n.Y - 1) < l - map[n.X - 1, n.Y - 1].TileType.Opacity)
                        {
                            SetLight(n.X - 1, n.Y - 1, Math.Max(l - map[n.X - 1, n.Y - 1].TileType.Opacity * InvSqrt2, 0));
                            lightQueue.Enqueue(new Int2(n.X - 1, n.Y - 1));
                        }

                        if (n.X != map.Width - 1 && n.Y != 0 && GetLight(n.X + 1, n.Y - 1) < l - map[n.X + 1, n.Y - 1].TileType.Opacity)
                        {
                            SetLight(n.X + 1, n.Y - 1, Math.Max(l - map[n.X + 1, n.Y - 1].TileType.Opacity * InvSqrt2, 0));
                            lightQueue.Enqueue(new Int2(n.X + 1, n.Y - 1));
                        }
                        if (n.X != 0 && n.Y != map.Height - 1 && GetLight(n.X - 1, n.Y + 1) < l - map[n.X - 1, n.Y + 1].TileType.Opacity)
                        {
                            SetLight(n.X - 1, n.Y + 1, Math.Max(l - map[n.X - 1, n.Y + 1].TileType.Opacity * InvSqrt2, 0));
                            lightQueue.Enqueue(new Int2(n.X - 1, n.Y + 1));
                        }

                        if (n.X != map.Width - 1 && n.Y != map.Height - 1 && GetLight(n.X + 1, n.Y + 1) < l - map[n.X + 1, n.Y + 1].TileType.Opacity)
                        {
                            SetLight(n.X + 1, n.Y + 1, Math.Max(l - map[n.X + 1, n.Y + 1].TileType.Opacity * InvSqrt2, 0));
                            lightQueue.Enqueue(new Int2(n.X + 1, n.Y + 1));
                        }
                    }
                }
            }
        }

        public void CalculateRemove()
        {
            while (lightRemQueue.Count > 0)
            {
                Int2Distance n = lightRemQueue.Dequeue();

                if (n.Value.X != 0 && GetLight(n.Value.X - 1, n.Value.Y) < n.Distance)
                {
                    lightRemQueue.Enqueue(new Int2Distance(new Int2(n.Value.X - 1, n.Value.Y), GetLight(n.Value.X - 1, n.Value.Y)));
                    SetLight(n.Value.X - 1, n.Value.Y, 0);
                }
                else if (n.Value.X != 0 && GetLight(n.Value.X - 1, n.Value.Y) > n.Distance)
                    lightQueue.Enqueue(new Int2(n.Value.X - 1, n.Value.Y));

                if (n.Value.X != map.Width - 1 && GetLight(n.Value.X + 1, n.Value.Y) < n.Distance)
                {
                    lightRemQueue.Enqueue(new Int2Distance(new Int2(n.Value.X + 1, n.Value.Y), GetLight(n.Value.X + 1, n.Value.Y)));
                    SetLight(n.Value.X + 1, n.Value.Y, 0);
                }
                else if (n.Value.X != map.Width - 1 && GetLight(n.Value.X + 1, n.Value.Y) > n.Distance)
                    lightQueue.Enqueue(new Int2(n.Value.X + 1, n.Value.Y));

                if (n.Value.Y != 0 && GetLight(n.Value.X, n.Value.Y - 1) < n.Distance)
                {
                    lightRemQueue.Enqueue(new Int2Distance(new Int2(n.Value.X, n.Value.Y - 1), GetLight(n.Value.X, n.Value.Y - 1)));
                    SetLight(n.Value.X, n.Value.Y - 1, 0);
                }
                else if (n.Value.Y != 0 && GetLight(n.Value.X, n.Value.Y - 1) > n.Distance)
                    lightQueue.Enqueue(new Int2(n.Value.X, n.Value.Y - 1));

                if (n.Value.Y != map.Height - 1 && GetLight(n.Value.X, n.Value.Y + 1) < n.Distance)
                {
                    lightRemQueue.Enqueue(new Int2Distance(new Int2(n.Value.X, n.Value.Y + 1), GetLight(n.Value.X, n.Value.Y + 1)));
                    SetLight(n.Value.X, n.Value.Y + 1, 0);
                }
                else if (n.Value.Y != map.Height - 1 && GetLight(n.Value.X, n.Value.Y + 1) > n.Distance)
                    lightQueue.Enqueue(new Int2(n.Value.X, n.Value.Y + 1));

                if (lightDiagonal)
                {
                    if (n.Value.X != 0 && n.Value.Y != 0 && GetLight(n.Value.X - 1, n.Value.Y - 1) < n.Distance)
                    {
                        lightRemQueue.Enqueue(new Int2Distance(new Int2(n.Value.X - 1, n.Value.Y - 1), GetLight(n.Value.X - 1, n.Value.Y - 1)));
                        SetLight(n.Value.X - 1, n.Value.Y - 1, 0);
                    }
                    else if (n.Value.X != 0 && n.Value.Y != 0 && GetLight(n.Value.X - 1, n.Value.Y - 1) > n.Distance)
                        lightQueue.Enqueue(new Int2(n.Value.X - 1, n.Value.Y - 1));

                    if (n.Value.X != map.Width - 1 && n.Value.Y != 0 && GetLight(n.Value.X + 1, n.Value.Y - 1) < n.Distance)
                    {
                        lightRemQueue.Enqueue(new Int2Distance(new Int2(n.Value.X + 1, n.Value.Y - 1), GetLight(n.Value.X + 1, n.Value.Y - 1)));
                        SetLight(n.Value.X + 1, n.Value.Y - 1, 0);
                    }
                    else if (n.Value.X != map.Width - 1 && n.Value.Y != 0 && GetLight(n.Value.X + 1, n.Value.Y - 1) > n.Distance)
                        lightQueue.Enqueue(new Int2(n.Value.X + 1, n.Value.Y - 1));

                    if (n.Value.X != 0 && n.Value.Y != map.Height - 1 && GetLight(n.Value.X - 1, n.Value.Y + 1) < n.Distance)
                    {
                        lightRemQueue.Enqueue(new Int2Distance(new Int2(n.Value.X - 1, n.Value.Y + 1), GetLight(n.Value.X - 1, n.Value.Y + 1)));
                        SetLight(n.Value.X - 1, n.Value.Y + 1, 0);
                    }
                    else if (n.Value.X != 0 && n.Value.Y != map.Height - 1 && GetLight(n.Value.X - 1, n.Value.Y + 1) > n.Distance)
                        lightQueue.Enqueue(new Int2(n.Value.X - 1, n.Value.Y + 1));

                    if (n.Value.X != map.Width - 1 && n.Value.Y != map.Height - 1 && GetLight(n.Value.X + 1, n.Value.Y + 1) < n.Distance)
                    {
                        lightRemQueue.Enqueue(new Int2Distance(new Int2(n.Value.X + 1, n.Value.Y + 1), GetLight(n.Value.X + 1, n.Value.Y + 1)));
                        SetLight(n.Value.X + 1, n.Value.Y + 1, 0);
                    }
                    else if(n.Value.X != map.Width - 1 && n.Value.Y != map.Height - 1 && GetLight(n.Value.X + 1, n.Value.Y + 1) > n.Distance)
                        lightQueue.Enqueue(new Int2(n.Value.X + 1, n.Value.Y + 1));
                }
            }
        }

        public void Calculate()
        {
            CalculateRemove();
            CalculateAdd();
        }
    }
}
