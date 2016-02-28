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
        internal byte[] lightData;
        private TileMapScript map;

        public TileLighting(TileMapScript map)
        {
            this.map = map;
            lightData = new byte[map.Width * map.Height];
            lightQueue = new Queue<Int2>();
        }

        public void AddLight(int x, int y, byte lightPower)
        {
            lightQueue.Enqueue(new Int2(x, y));
            SetLight(x, y, lightPower);
        }

        private void SetLight(int x, int y, byte lightPower)
        {
            var index = y * map.Width + x;
            lightData[index] = lightPower;
        }

        public byte GetLight(int x, int y)
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
                if (l > 1)
                {
                    if (n.X != 0 && GetLight(n.X - 1, n.Y) + 2 <= l)
                    {
                        //UnityEngine.Debug.Log("OP: " + map[n.X - 1, n.Y].TileType.Opacity);
                        SetLight(n.X - 1, n.Y, (byte)Math.Max(l - map[n.X - 1, n.Y].TileType.Opacity, 0));
                        lightQueue.Enqueue(new Int2(n.X - 1, n.Y));
                    }
                    if (n.X != map.Width - 1 && GetLight(n.X + 1, n.Y) + 2 <= l)
                    {
                        SetLight(n.X + 1, n.Y, (byte)Math.Max(l - map[n.X + 1, n.Y].TileType.Opacity, 0));
                        lightQueue.Enqueue(new Int2(n.X + 1, n.Y));
                    }
                    if (n.Y != 0 && GetLight(n.X, n.Y - 1) + 2 <= l)
                    {
                        SetLight(n.X, n.Y - 1, (byte)Math.Max(l - map[n.X, n.Y - 1].TileType.Opacity, 0));
                        lightQueue.Enqueue(new Int2(n.X, n.Y - 1));
                    }
                    if (n.Y != map.Height - 1 && GetLight(n.X, n.Y + 1) + 2 <= l)
                    {
                        SetLight(n.X, n.Y + 1, (byte)Math.Max(l - map[n.X, n.Y + 1].TileType.Opacity, 0));
                        lightQueue.Enqueue(new Int2(n.X, n.Y + 1));
                    }
                }
            }
        }
    }
}
