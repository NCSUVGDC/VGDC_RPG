using System;
using System.IO;
using UnityEngine;
using VGDC_RPG.Map;

namespace VGDC_RPG.TileMapProviders
{
    public class SavedTileMapProvider : TileMapProvider
    {
        static SavedTileMapProvider()
        {
            if (!Directory.Exists(Application.persistentDataPath + "/tilemaps"))
                Directory.CreateDirectory(Application.persistentDataPath + "/tilemaps");
        }

        private string fn;

        public Color InitialSunColor { get; private set; }
        public Color InitialAmbientColor { get; private set; }
        public float InitialBrightness { get; private set; }

        public SavedTileMapProvider(string name)
        {
            fn = name;
        }

        public ushort[][,] GetTileMap()
        {
            var fs = File.Open(Application.persistentDataPath + "/tilemaps/" + fn + ".map", FileMode.Open);
            BinaryReader r = new BinaryReader(fs);
            var version = r.ReadInt32();
            if (version != 2)
                throw new Exception("Invalid tilemap file version.");
            InitialSunColor = new Color(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
            InitialAmbientColor = new Color(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
            InitialBrightness = r.ReadSingle();
            var layers = r.ReadInt32();
            var width = r.ReadInt32();
            var height = r.ReadInt32();
            ushort[][,] m = new ushort[layers][,];//[width, height];
            for (int n = 0; n < layers; n++)
            {
                m[n] = new ushort[width, height];
                for (int j = 0; j < height; j++)
                    for (int i = 0; i < width; i++)
                        m[n][i, j] = r.ReadUInt16();
            }

            r.Close();
            fs.Close();
            return m;
        }

        public static string[] GetSavedTileMaps()
        {
            var r = Directory.GetFiles(Application.persistentDataPath + "/tilemaps/", "*.map", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < r.Length; i++)
                r[i] = Path.GetFileNameWithoutExtension(r[i]);
            return r;
        }

        public static void SaveTileMap(string name, TileMap m)
        {
            var fs = File.Create(Application.persistentDataPath + "/tilemaps/" + name + ".map");
            BinaryWriter w = new BinaryWriter(fs);
            w.Write(2);
            var sc = GameLogic.GetSunColor();
            w.Write(sc.r);
            w.Write(sc.g);
            w.Write(sc.b);
            w.Write(sc.a);
            sc = GameLogic.GetAmbientColor();
            w.Write(sc.r);
            w.Write(sc.g);
            w.Write(sc.b);
            w.Write(sc.a);
            w.Write(GameLogic.GetBrightness());
            w.Write(m.Layers.Length);
            w.Write(m.Width);
            w.Write(m.Height);
            for (int n = 0; n < m.Layers.Length; n++)
                for (int j = 0; j < m.Height; j++)
                    for (int i = 0; i < m.Width; i++)
                        w.Write(Region.GetBase(m.Layers[n][i, j].TileTypeID));

            w.Close();
            fs.Close();
            Debug.Log("Map saved: " + name);
        }

        public Color GetInitialSunColor()
        {
            return InitialSunColor;
        }

        public Color GetInitialAmbientColor()
        {
            return InitialAmbientColor;
        }

        public float GetInitialBrightness()
        {
            return InitialBrightness;
        }
    }
}
