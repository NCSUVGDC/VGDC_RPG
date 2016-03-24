using System;
using UnityEngine;

namespace VGDC_RPG.Networking
{
    public class TileMapReciever : TileMapProviders.TileMapProvider
    {
        int width = -1;
        int height = -1;
        int layers = -1;
        ushort[][,] m;
        int l = -1;

        public bool Ready { get { return l == 0; } }

        public void HandleData(DataReader r)
        {
            var dataIndex = r.ReadInt32();
            Debug.Log("TMR: di: " + dataIndex);
            if (dataIndex == 0)
            {
                width = r.ReadInt32();
                height = r.ReadInt32();
                layers = r.ReadInt32();
                m = new ushort[layers][,];
                for (int i = 0; i < layers; i++)
                    m[i] = new ushort[width, height];
                l = height * layers;
            }
            else
            {
                int y = (dataIndex - 1) % height;
                int layer = (dataIndex - 1) / height;
                for (int x = 0; x < width; x++)
                    m[layer][x, y] = r.ReadUInt16();
                l--;
                if (l == 0)
                    Debug.Log("Tilemap complete.");
                else
                    Debug.Log("TMR: " + l);
            }
        }

        public ushort[][,] GetTileMap()
        {
            if (Ready)
                return m;
            else
                throw new Exception();
        }
    }
}
