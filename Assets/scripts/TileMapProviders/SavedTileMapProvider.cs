using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using VGDC_RPG.Map;

namespace VGDC_RPG.TileMapProviders
{
    public class SavedTileMapProvider : TileMapProvider
    {
        private string fn;

        public SavedTileMapProvider(string name)
        {
            fn = name;
        }

        public ushort[,] GetTileMap()
        {
            var fs = File.Open(Application.persistentDataPath + "/" + fn + ".dat", FileMode.Open);
            BinaryReader r = new BinaryReader(fs);
            var version = r.ReadInt32();
            if (version != 1)
                throw new Exception("Invalid tilemap file version.");
            var width = r.ReadInt32();
            var height = r.ReadInt32();
            ushort[,] m = new ushort[width, height];

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    m[i, j] = r.ReadUInt16();

            r.Close();
            fs.Close();
            return m;
        }

        public static void SaveTileMap(string name, TileMap m)
        {
            var fs = File.Create(Application.persistentDataPath + "/" + name + ".dat");
            BinaryWriter w = new BinaryWriter(fs);
            w.Write(1);
            w.Write(m.Width);
            w.Write(m.Height);
            for (int j = 0; j < m.Height; j++)
                for (int i = 0; i < m.Width; i++)
                    w.Write(Region.GetBase(m[i, j].TileTypeID));

            w.Close();
            fs.Close();
            Debug.Log("Map saved: " + name);
        }
    }
}
