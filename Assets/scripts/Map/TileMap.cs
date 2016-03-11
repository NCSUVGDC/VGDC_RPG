using UnityEngine;
using System.Collections;
using VGDC_RPG.Tiles;
using System.Collections.Generic;
using VGDC_RPG;
using VGDC_RPG.Map;
using System;
using VGDC_RPG.TileMapProviders;

namespace VGDC_RPG.Map
{
    /// <summary>
    /// Script for the TileMap game objects.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class TileMap : MonoBehaviour
    {
        /// <summary>
        /// The framerate to animate tiles at.
        /// </summary>
        public float FramesPerSecond = 2;

        public int TileLayerToSet;
        public ushort TileIDToSet = 1;

        private Texture2D lightTexture;
        public TileLayer[] Layers = new TileLayer[3];
        private TileLighting lightingR;
        private TileLighting lightingG;
        private TileLighting lightingB;

        private GameObject lightLayer;

        private byte[,] islands;
        private int[] islandP = new int[256];
        private int mi;

        /// <summary>
        /// Material used by the light layer.
        /// </summary>
        public Material LightLayerMaterial;
        public Material TileMapMaterial;

        private bool lightingDirty = false;

        /// <summary>
        /// True if tilemap is in edit mode.  Should remain false during gameplay.
        /// </summary>
        public bool EditMode = false;

        /// <summary>
        /// Constructs and returns a new tilemap with the given tile ID array.
        /// </summary>
        /// <param name="m">The 2D array of tile IDs to use in creating the tilemap.</param>
        /// <returns>The TileMapScript attached to the new tilemap object.</returns>
        public static TileMap Construct(params ushort[][,] m)
        {
            var tm = GameObject.Instantiate(Resources.Load("tilemap")) as GameObject;
            Debug.Assert(tm != null, "TileMap Resource GameObject null!");
            var r = tm.GetComponent<TileMap>();
            r.Width = m[0].GetLength(0);
            r.Height = m[0].GetLength(1);

            r.Layers = new TileLayer[m.Length];

            for (int n = 0; n < m.Length; n++)
            {
                var go = new GameObject("TileLayer" + n, typeof(MeshRenderer), typeof(MeshFilter), typeof(TileLayer));
                go.transform.position = new Vector3(0, n - m.Length, 0);
                go.GetComponent<MeshRenderer>().material = r.TileMapMaterial;
                r.Layers[n] = go.GetComponent<TileLayer>();
                r.Layers[n].Owner = r;
                r.Layers[n].map = new TileData[m[n].GetLength(0), m[n].GetLength(1)];
                for (int j = 0; j < m[n].GetLength(1); j++)
                    for (int i = 0; i < m[n].GetLength(0); i++)
                        r.Layers[n].map[i, j] = new TileData(Region.GetTile(m[n], i, j));
            }
            r.islands = new byte[r.Width, r.Height];
            for (int y = 0; y < r.Height; y++)
                for (int x = 0; x < r.Width; x++)
                {
                    if (r.islands[x, y] == 0 && r.IsWalkable(x, y))
                    {
                        r.FloodFillIsland(x, y, (byte)(r.islandP[0] + 1));
                        r.islandP[0]++;
                    }
                }
            int lm = 0;
            for (int i = 1; i < r.islandP[0] + 1; i++)
                if (r.islandP[i] > lm)
                {
                    r.mi = i;
                    lm = r.islandP[i];
                    Debug.Log("MI: " + r.mi);
                }
            return r;
        }

        public bool IsWalkable(int x, int y)
        {
            for (int n = 0; n < Layers.Length; n++)
                if (!Layers[n][x, y].Walkable)
                    return false;
            return !IsObjectOnTile(x, y);
        }

        public int GetMovementCost(int x, int y)
        {
            int r = 0;
            for (int n = 0; n < Layers.Length; n++)
                r += Layers[n][x, y].TileType.MovementCost;
            return Mathf.Max(r, 0);
        }

        public float GetOpacity(int x, int y)
        {
            float r = 0;
            for (int n = 0; n < Layers.Length; n++)
                r += Layers[n][x, y].TileType.Opacity;
            return Mathf.Max(r, 0);
        }

        internal void SetTileLight(int x, int y)
        {
            lightingR.RemoveLight(x, y);
            lightingG.RemoveLight(x, y);
            lightingB.RemoveLight(x, y);

            float mr = GetEmissionRed(x, y);
            float mg = GetEmissionGreen(x, y);
            float mb = GetEmissionBlue(x, y);

            if (mr > 0)
                lightingR.AddLight(x, y, mr);
            if (mg > 0)
                lightingG.AddLight(x, y, mg);
            if (mb > 0)
                lightingB.AddLight(x, y, mb);

            lightingDirty = true;
        }

        public float GetEmissionRed(int x, int y)
        {
            float r = 0;
            for (int n = 0; n < Layers.Length; n++)
                r += Layers[n][x, y].TileType.EmissionR;
            return Mathf.Max(r, 0);
        }

        public float GetEmissionGreen(int x, int y)
        {
            float r = 0;
            for (int n = 0; n < Layers.Length; n++)
                r += Layers[n][x, y].TileType.EmissionG;
            return Mathf.Max(r, 0);
        }

        public float GetEmissionBlue(int x, int y)
        {
            float r = 0;
            for (int n = 0; n < Layers.Length; n++)
                r += Layers[n][x, y].TileType.EmissionB;
            return Mathf.Max(r, 0);
        }

        private void FloodFillIsland(int x, int y, byte ii)
        {
            islands[x, y] = ii;
            islandP[ii]++;
            var n = GetNeighbors(new Int2(x, y));
            foreach (var t in n)
            {
                if (IsWalkable(t.X, t.Y) && islands[t.X, t.Y] == 0)
                    FloodFillIsland(t.X, t.Y, ii);  //Doesn't really need to be recursive.
            }
        }

        

        /// <summary>
        /// The number of tiles verically.
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// The number of tiles horizontally.
        /// </summary>
        public int Height { get; private set; }

        internal Mesh mesh;

        private bool[,] oot;

        // Use this for initialization
        void Start()
        {
            mesh = GenerateMesh(Width, Height);
            oot = new bool[Width, Height];

            lightLayer = new GameObject("LightLayer", typeof(MeshFilter), typeof(MeshRenderer));
            lightLayer.GetComponent<MeshRenderer>().material = LightLayerMaterial;
            lightLayer.GetComponent<MeshFilter>().mesh = mesh;
            lightLayer.transform.position = new Vector3(0, 5, 0);
            lightTexture = new Texture2D(Width, Height, TextureFormat.RGBAHalf, false);
            /*for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    lightTexture.SetPixel(x, y, new Color(0, 0, 0, x / (float)Width));*/
            lightTexture.wrapMode = TextureWrapMode.Clamp;
            lightTexture.filterMode = FilterMode.Trilinear;
            lightTexture.Apply();
            LightLayerMaterial.SetTexture("_MainTex", lightTexture);
            LightLayerMaterial.SetFloat("_TilesWidth", Width);
            LightLayerMaterial.SetFloat("_TilesHeight", Height);

            lightingR = new TileLighting(this);
            lightingG = new TileLighting(this);
            lightingB = new TileLighting(this);
            AddLights();
        }

        //private Mesh GenerateMesh(int width, int height)
        //{
        //    int triCount = width * height * 2;
        //    int vertCount = (width + 1) * (height + 1);

        //    Vector3[] vertices = new Vector3[vertCount];
        //    Vector3[] normals = new Vector3[vertCount];
        //    Vector2[] uvs = new Vector2[vertCount];
        //    int[] indices = new int[triCount * 3];

        //    for (int y = 0; y < height + 1; y++)
        //        for (int x = 0; x < width + 1; x++)
        //        {
        //            vertices[y * (width + 1) + x] = new Vector3(x, 0, y);
        //            normals[y * (width + 1) + x] = Vector3.up;
        //            uvs[y * (width + 1) + x] = new Vector2(x / (float)(width + 1), y / (float)(height + 1));
        //        }

        //    for (int y = 0; y < height; y++)
        //        for (int x = 0; x < width; x++)
        //        {
        //            int i = (y * width + x) * 6;
        //            indices[i + 0] = y * (width + 1) + x;
        //            indices[i + 1] = y * (width + 1) + x + width + 1;
        //            indices[i + 2] = y * (width + 1) + x + width + 2;

        //            indices[i + 3] = y * (width + 1) + x;
        //            indices[i + 4] = y * (width + 1) + x + width + 2;
        //            indices[i + 5] = y * (width + 1) + x + 1;
        //        }

        //    Mesh mesh = new Mesh();
        //    mesh.vertices = vertices;
        //    mesh.normals = normals;
        //    mesh.uv = uvs;
        //    mesh.triangles = indices;

        //    /*MeshFilter mf = GetComponent<MeshFilter>();

        //    mf.mesh = mesh;*/
        //    return mesh;
        //}

        private Mesh GenerateMesh(int width, int height)
        {
            int triCount = 2;
            int vertCount = 4;

            Vector3[] vertices = new Vector3[vertCount];
            Vector3[] normals = new Vector3[vertCount];
            Vector2[] uvs = new Vector2[vertCount];
            int[] indices = new int[triCount * 3];

            for (int y = 0; y < 2; y++)
                for (int x = 0; x < 2; x++)
                {
                    vertices[y * 2 + x] = new Vector3(x * width, 0, y * height);
                    normals[y * 2 + x] = Vector3.up;
                    uvs[y * 2 + x] = new Vector2(x, y);
                }

            indices[0] = 0;
            indices[2] = 1;
            indices[1] = 2;

            indices[3] = 3;
            indices[4] = 1;
            indices[5] = 2;

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = indices;

            /*MeshFilter mf = GetComponent<MeshFilter>();

            mf.mesh = mesh;*/
            return mesh;
        }

        public bool InSpawn(int x, int y)
        {
            return islands[x, y] == mi;
        }

        private void AddLights()
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    var er = GetEmissionRed(x, y);
                    var eg = GetEmissionGreen(x, y);
                    var eb = GetEmissionBlue(x, y);
                    if (er > 0)
                        lightingR.AddLight(x, y, er);
                    if (eg > 0)
                        lightingG.AddLight(x, y, eg);
                    if (eb > 0)
                        lightingB.AddLight(x, y, eb);
                }
            UpdateLighting();
        }

        private void UpdateLighting()
        {
            var tc = Environment.TickCount;
            lightingR.Calculate();//CalculateAdd();
            Debug.Log("Red light time: " + (Environment.TickCount - tc));
            tc = Environment.TickCount;
            lightingG.Calculate();//.CalculateAdd();
            Debug.Log("Green light time: " + (Environment.TickCount - tc));
            tc = Environment.TickCount;
            lightingB.Calculate();//.CalculateAdd();
            Debug.Log("Blue light time: " + (Environment.TickCount - tc));
            //lightTexture.LoadRawTextureData(lighting.lightData);
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    lightTexture.SetPixel(x, y, new Color(lightingR.GetLight(x, y), lightingG.GetLight(x, y), lightingB.GetLight(x, y), 1));
            lightTexture.Apply();
        }

        /// <summary>
        /// Marks a tile as selected.
        /// Be sure once all tiles are selected/deselected to call ApplySelection to make changes visible.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SelectTile(int x, int y)
        {
            for (int n = 0; n < Layers.Length; n++)
                Layers[n].SelectTile(x, y);
        }

        /// <summary>
        /// Marks a tile as unselected.
        /// Be sure once all tiles are selected/deselected to call ApplySelection to make changes visible.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void DeselectTile(int x, int y)
        {
            for (int n = 0; n < Layers.Length; n++)
                Layers[n].DeselectTile(x, y);
        }

        /// <summary>
        /// Applies the changes in tile selection to the tile texture.
        /// </summary>
        public void ApplySelection()
        {
            for (int n = 0; n < Layers.Length; n++)
                Layers[n].ApplySelection();
        }

        /// <summary>
        /// Clears the tile selections.
        /// </summary>
        public void ClearSelection()
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    DeselectTile(x, y);
            ApplySelection();
        }

        // Update is called once per frame
        void Update()
        {
            if (lightingDirty)
            {
                lightingDirty = false;
                UpdateLighting();
            }

            if (EditMode)
                if (Input.GetMouseButton(0))//.GetMouseButtonDown(1))
                {
                    var t = GameLogic.Instance.GetScreenTile(Input.mousePosition.x, Input.mousePosition.y);
                    if (t.X >= 0 && t.Y >= 0 && t.X < Width && t.Y < Height)
                        Layers[TileLayerToSet].SetTile(t.X, t.Y, TileIDToSet);//(ushort)((this[t].TileTypeID % 20) + 1));
                }

            if (Input.GetMouseButtonDown(2))
            {
                var t = GameLogic.Instance.GetScreenTile(Input.mousePosition.x, Input.mousePosition.y);
                if (t.X >= 0 && t.Y >= 0 && t.X < Width && t.Y < Height)
                    Debug.Log("OT: " + IsWalkable(t.X, t.Y));
            }
        }

        private string mapSaveName = "";
        void OnGUI()
        {
            if (EditMode)
            {
                mapSaveName = GUI.TextField(new Rect(Screen.width - 100, Screen.height - 50, 100, 20), mapSaveName);
                if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 30, 100, 30), "Save"))
                    SavedTileMapProvider.SaveTileMap(mapSaveName, this);
                if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 80, 100, 30), "Regions"))
                    Layers[TileLayerToSet].UpdateRegions();
            }
        }

        /// <summary>
        /// Returns the locations of every tile neighboring a given tile location.
        /// </summary>
        /// <param name="t">The tile location.</param>
        /// <returns>A list of all neighboring locations.</returns>
        public List<Int2> GetNeighbors(Int2 t)
        {
            var r = new List<Int2>();
            if (t.X != 0)
                r.Add(new Int2(t.X - 1, t.Y));
            if (t.Y != 0)
                r.Add(new Int2(t.X, t.Y - 1));

            if (t.X != Width - 1)
                r.Add(new Int2(t.X + 1, t.Y));
            if (t.Y != Height - 1)
                r.Add(new Int2(t.X, t.Y + 1));

            return r;
        }

        /// <summary>
        /// Marks a tile as obstructed so that no players can walk onto it.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public void BlockTile(int x, int y)
        {
            oot[x, y] = true;
        }

        /// <summary>
        /// Marks a tile as unobstructed so that players can walk onto it if it is walkable.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public void UnblockTile(int x, int y)
        {
            oot[x, y] = false;
        }

        public bool ProjectileRayCast(Vector2 start, Vector2 goal)
        {
            //ClearSelection();
            Vector2 d = new Vector2(goal.x - start.x, goal.y - start.y).normalized;
            int gx = Mathf.FloorToInt(goal.x);
            int gy = Mathf.FloorToInt(goal.y);

            float x = start.x, y = start.y;
            float dx = 0, dy = 0;

            for (int i = 0; i < 200; i++)
            {
                if (d.x < 0)
                    dx = -(x - Mathf.FloorToInt(x - 0.0001f));
                else if (d.x > 0)
                    dx = 1 - (x - Mathf.FloorToInt(x - 0.0001f));

                if (d.y < 0)
                    dy = -(y - Mathf.FloorToInt(y - 0.0001f));
                else if (d.y > 0)
                    dy = 1 - (y - Mathf.FloorToInt(y - 0.0001f));

                float t = 0;
                if (dy == 0)
                    t = 1;
                else if (Mathf.Abs(dx / d.x) < Mathf.Abs(dy / d.y))
                    t = dx / d.x + 0.001f;
                else
                    t = dy / d.y + 0.001f;

                x += d.x * t;
                y += d.y * t;

                //Debug.Log("dx: " + dx + ", dy: " + dy);
                //Debug.Log(x + ", " + y);
                //SelectedTile(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
                //ApplySelection();
                if (Mathf.FloorToInt(x) == gx && Mathf.FloorToInt(y) == gy)
                    return true;
                if (IsProjectileResistant(Mathf.FloorToInt(x), Mathf.FloorToInt(y)) || IsObjectOnTile(Mathf.FloorToInt(x), Mathf.FloorToInt(y)))
                    return false;
            }

            Debug.LogError("Raycast fail.");
            return false;
        }

        private bool IsObjectOnTile(int x, int y)
        {
            if (oot == null)
                return false;
            return oot[x, y];
        }

        private bool IsProjectileResistant(int x, int y)
        {
            for (int n = 0; n < Layers.Length; n++)
                if (Layers[n][x, y].TileType.ProjectileResistant)
                    return true;
            return false;
        }
    }
}
