using System;
using UnityEngine;
using VGDC_RPG.Tiles;

namespace VGDC_RPG.Map
{
    public class TileLayer : MonoBehaviour//, INetEventHandler
    {
        internal TileMap Owner;
        internal TileData[,] map;
        private Material mat;
        private Texture2D texture;

        /// <summary>
        /// Gets the TileData at a given location.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The TileData at the given location.</returns>
        public TileData this[int x, int y] { get { return map[x, y]; } private set { map[x, y] = value; } }
        /// <summary>
        /// Gets the TileData at a given location.
        /// </summary>
        /// <param name="t">The position.</param>
        /// <returns>The TileData at the given location.</returns>
        /// <returns></returns>
        public TileData this[Int2 t] { get { return map[t.X, t.Y]; } }

        public int HandlerID { get; set; }

        void Start()
        {
            mat = GetComponent<MeshRenderer>().material;
            GetComponent<MeshFilter>().mesh = Owner.mesh;
            GenerateTexture();
            //System.IO.File.WriteAllBytes("C:\\Users\\Matthew\\Pictures\\tiletest.png", texture.EncodeToPNG());
            mat.SetFloat("_TilesWidth", Owner.Width);
            mat.SetFloat("_TilesHeight", Owner.Height);
            mat.SetFloat("_AtlasSize", Constants.ATLAS_SIZE);
            mat.SetFloat("_AtlasResolution", mat.GetTexture("_AtlasTex").width);
        }

        void Update()
        {
            mat.SetInt("_Frame", Mathf.FloorToInt(Time.realtimeSinceStartup * Owner.FramesPerSecond));
        }

        private void GenerateTexture()
        {
            int texWidth = map.GetLength(0);
            int texHeight = map.GetLength(1);
            texture = new Texture2D(texWidth, texHeight, TextureFormat.RGBAHalf, false);

            for (int y = 0; y < texHeight; y++)
                for (int x = 0; x < texWidth; x++)
                    texture.SetPixel(x, y, map[x, y].TileType.RenderData);

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();

            mat.mainTexture = texture;
        }
        
        /// <summary>
        /// Sets a tile on the tilemap and marks the lighting to be updated on the next update.
        /// </summary>
        /// <param name="x">X coordinate of the tile location.</param>
        /// <param name="y">Y coordinate of the tile location.</param>
        /// <param name="id">ID of the tile type to set.</param>
        public void SetTile(int x, int y, ushort id, bool netevent)
        {
            if (this[x, y].TileTypeID == id)
                return;

            var ntd = new TileData(id);

            texture.SetPixel(x, y, ntd.TileType.RenderData);
            texture.Apply();

            this[x, y] = new TileData(id);

            Owner.SetTileLight(x, y);
        }

        /// <summary>
        /// Recalculates region bounderies.
        /// </summary>
        public void UpdateRegions()
        {
            ushort[,] m = new ushort[Owner.Width, Owner.Height];
            for (int y = 0; y < Owner.Height; y++)
                for (int x = 0; x < Owner.Width; x++)
                    m[x, y] = Region.GetBase(this[x, y].TileTypeID);//SetTile(x, y, Region.GetBase(this[x, y].TileTypeID));
            for (int y = 0; y < Owner.Height; y++)
                for (int x = 0; x < Owner.Width; x++)
                    SetTile(x, y, Region.GetTile(m, x, y), true);//SetTile(x, y, Region.GetBase(this[x, y].TileTypeID));
        }

        /// <summary>
        /// Marks a tile as selected.
        /// Be sure once all tiles are selected/deselected to call ApplySelection to make changes visible.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void HighlightTile(int x, int y, int v)
        {
            if (texture == null)
                return;
            var oc = texture.GetPixel(x, y);
            texture.SetPixel(x, y, new Color(oc.r, oc.g, oc.b, v / 4.0f + 1 / 8.0f));
        }

        /// <summary>
        /// Marks a tile as unselected.
        /// Be sure once all tiles are selected/deselected to call ApplySelection to make changes visible.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void UnhighlightTile(int x, int y)
        {
            if (texture == null)
                return;
            var oc = texture.GetPixel(x, y);
            texture.SetPixel(x, y, new Color(oc.r, oc.g, oc.b, 0.0f));
        }

        /// <summary>
        /// Applies the changes in tile selection to the tile texture.
        /// </summary>
        public void ApplyHighlight()
        {
            if (texture == null)
                return;
            texture.Apply();
        }

        public void SetSelection(int x, int y)
        {
            if (mat != null)
            {
                mat.SetFloat("_SelX", x);
                mat.SetFloat("_SelY", y);
            }
        }
        private enum EventType : byte
        {
            ERROR = 0,
            SetTile
        }
    }
}
