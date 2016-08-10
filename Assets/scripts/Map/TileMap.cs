using UnityEngine;
using VGDC_RPG.Tiles;
using System.Collections.Generic;
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
        private List<int> islandP = new List<int>();
        private int mi;

        /// <summary>
        /// Material used by the light layer.
        /// </summary>
        public Material LightLayerMaterial;
        /// <summary>
        /// Material used by the tile layers.
        /// </summary>
        public Material TileMapMaterial;

        private bool lightingDirty = false;

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
                go.tag = "TileMap";
                go.transform.position = new Vector3(0, n - m.Length, 0);
                go.GetComponent<MeshRenderer>().material = r.TileMapMaterial;
                r.Layers[n] = go.GetComponent<TileLayer>();
                r.Layers[n].Owner = r;
                r.Layers[n].map = new TileData[m[n].GetLength(0), m[n].GetLength(1)];
                r.Layers[n].HandlerID = -2000 - n;
                for (int j = 0; j < m[n].GetLength(1); j++)
                    for (int i = 0; i < m[n].GetLength(0); i++)
                        r.Layers[n].map[i, j] = new TileData(Region.GetTile(m[n], i, j));
            }
            r.islands = new byte[r.Width, r.Height];
            r.islandP.Add(0);
            for (int y = 0; y < r.Height; y++)
            {
                for (int x = 0; x < r.Width; x++)
                {
                    if (r.islands[x, y] == 0 && r.IsWalkable(x, y))
                    {
                        r.islandP.Add(0);
                        r.FloodFillIsland(x, y, (byte)(r.islandP[0] + 1));
                        r.islandP[0]++;
                    }
                }
                Debug.Log("IGY: " + y);
            }
            int lm = 0;
            for (int i = 1; i < r.islandP[0] + 1; i++)
                if (r.islandP[i] > lm)
                {
                    r.mi = i;
                    lm = r.islandP[i];
                    Debug.Log("MI: " + r.mi);
                }
            r.PreStart();
            return r;
        }

        internal void Destroy()
        {
            foreach (var t in Layers)
                Destroy(t.gameObject);
            Destroy(lightLayer);
            Destroy(gameObject);
        }

        /// <summary>
        /// Determines if a tile is walkable by a player by checking that a tile is walkable across all layers.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>If this tile is walkable.</returns>
        public bool IsWalkable(int x, int y)
        {
            for (int n = 0; n < Layers.Length; n++)
                if (!Layers[n][x, y].Walkable)
                    return false;
            return !IsObjectOnTile(x, y);
        }

        /// <summary>
        /// Gets the cost of movement acrossed a tile by addtion of costs across layers.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>The cost of movement acrossed this tile.</returns>
        public int GetMovementCost(int x, int y)
        {
            int r = 0;
            for (int n = 0; n < Layers.Length; n++)
                r += Layers[n][x, y].TileType.MovementCost;
            return Mathf.Max(r, 0);
        }

        /// <summary>
        /// Determines the opacity of a tile used by the lighting system by addtion of opacity across layers.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>The amount of light this tile blocks.</returns>
        public float GetOpacity(int x, int y)
        {
            float r = 0;
            for (int n = 0; n < Layers.Length; n++)
                r = Math.Max(r, Layers[n][x, y].TileType.Opacity);
                //r += Layers[n][x, y].TileType.Opacity;
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

        /// <summary>
        /// Gets the amount of red light a tile emits.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>The emission amount of the red channel.</returns>
        public float GetEmissionRed(int x, int y)
        {
            float r = 0;
            for (int n = 0; n < Layers.Length; n++)
                r += Layers[n][x, y].TileType.EmissionR;
            return Mathf.Max(r, 0);
        }

        /// <summary>
        /// Gets the amount of green light a tile emits.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>The emission amount of the green channel.</returns>
        public float GetEmissionGreen(int x, int y)
        {
            float r = 0;
            for (int n = 0; n < Layers.Length; n++)
                r += Layers[n][x, y].TileType.EmissionG;
            return Mathf.Max(r, 0);
        }

        /// <summary>
        /// Gets the amount of blue light a tile emits.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <returns>The emission amount of the blue channel.</returns>
        public float GetEmissionBlue(int x, int y)
        {
            float r = 0;
            for (int n = 0; n < Layers.Length; n++)
                r += Layers[n][x, y].TileType.EmissionB;
            return Mathf.Max(r, 0);
        }

        private void FloodFillIsland(int x, int y, byte ii)
        {
            /*islands[x, y] = ii;
            islandP[ii]++;
            var n = GetNeighbors(new Int2(x, y));
            foreach (var t in n)
            {
                if (IsWalkable(t.X, t.Y) && islands[t.X, t.Y] == 0)
                    FloodFillIsland(t.X, t.Y, ii);  //Doesn't really need to be recursive.
            }*/
            Stack<Int2> fq = new Stack<Int2>();
            fq.Push(new Int2(x, y));

            while (fq.Count > 0)
            {
                var n = fq.Pop();
                islands[n.X, n.Y] = ii;
                islandP[ii]++;
                var nt = GetNeighbors(n);
                foreach (var t in nt)
                {
                    if (IsWalkable(t.X, t.Y) && islands[t.X, t.Y] == 0)
                        fq.Push(t);//FloodFillIsland(t.X, t.Y, ii);  //Doesn't really need to be recursive.
                }
            }
        }

            /*var n = GetNeighbors(new Int2(x, y));
            Queue<Int2> fq = new Queue<Int2>();
            fq.Enqueue(new Int2(x, y));
            foreach (var t in n)
                fq.Enqueue(t);
            while (fq.Count > 0)
            {
                var t = fq.Dequeue();
                if (IsWalkable(t.X, t.Y) && islands[t.X, t.Y] == 0)
                {
                    islands[t.X, t.Y] = ii;
                    islandP[ii]++;
                    //FloodFillIsland(t.X, t.Y, ii);  //Doesn't really need to be recursive.
                    foreach (var nt in GetNeighbors(new Int2(t.X, t.Y)))
                        fq.Enqueue(nt);
                }
            }
        }*/



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

        void PreStart()
        {
            oot = new bool[Width, Height];

            lightingR = new TileLighting(this);
            lightingG = new TileLighting(this);
            lightingB = new TileLighting(this);
        }

        // Use this for initialization
        void Start()
        {
            mesh = GenerateMesh(Width, Height);
            //oot = new bool[Width, Height];

            lightLayer = new GameObject("LightLayer", typeof(MeshFilter), typeof(MeshRenderer));
            lightLayer.layer = 10;
            lightLayer.tag = "TileMap";
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

            //lightingR = new TileLighting(this);
            //lightingG = new TileLighting(this);
            //lightingB = new TileLighting(this);
            AddLights();

            LightLayerMaterial.SetTexture("_LightTex", RTVs.LightsRTV);
            RTVs.BuffersResized += RTVs_BuffersResized;
        }

        private void RTVs_BuffersResized()
        {
            //LightLayerMaterial.SetTexture("_LightTex", RTVs.LightsRTV);
        }

        void OnDestroy()
        {
            RTVs.BuffersResized -= RTVs_BuffersResized;
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
            return islands[x, y] == mi && IsWalkable(x, y);
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
        /// Marks a tile as highlighted.
        /// Be sure once all tiles are selected/deselected to call ApplyHighlight to make changes visible.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void HighlightTile(int x, int y, int v)
        {
            for (int n = 0; n < Layers.Length; n++)
                Layers[n].HighlightTile(x, y, v);
        }

        /// <summary>
        /// Marks a tile as unhighlighted.
        /// Be sure once all tiles are selected/deselected to call ApplyHighlight to make changes visible.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void UnhighlightTile(int x, int y)
        {
            for (int n = 0; n < Layers.Length; n++)
                Layers[n].UnhighlightTile(x, y);
        }

        /// <summary>
        /// Applies the changes in tile highlighting to the tile texture.
        /// </summary>
        public void ApplyHightlight()
        {
            for (int n = 0; n < Layers.Length; n++)
                Layers[n].ApplyHighlight();
        }

        /// <summary>
        /// Clears the tile highlights.
        /// </summary>
        public void ClearHighlight()
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    UnhighlightTile(x, y);
            ApplyHightlight();
        }

        int lsx = -1;
        int lsy = -1;
        public void SetSelection(int x, int y)
        {
            if (lsx == x && lsy == y)
                return;
            lsx = x;
            lsy = y;
            for (int n = 0; n < Layers.Length; n++)
                Layers[n].SetSelection(x, y);
        }

        // Update is called once per frame
        void Update()
        {
            if (lightingDirty)
            {
                lightingDirty = false;
                UpdateLighting();
            }

            if (InputManager.InEditMode)
            {
                if (InputManager.EditMousePressed)//.GetMouseButtonDown(1))
                {
                    var t = GameLogic.GetScreenTile(InputManager.MouseX, InputManager.MouseY);
                    if (t.X >= 0 && t.Y >= 0 && t.X < Width && t.Y < Height)
                        Layers[TileLayerToSet].SetTile(t.X, t.Y, TileIDToSet, true);//(ushort)((this[t].TileTypeID % 20) + 1));
                }
                else if (InputManager.EditMouseUp)
                    Layers[TileLayerToSet].UpdateRegions();
            }

            if (Input.GetMouseButtonDown(2))
            {
                var t = GameLogic.GetScreenTile(InputManager.MouseX, InputManager.MouseY);
                if (t.X >= 0 && t.Y >= 0 && t.X < Width && t.Y < Height)
                    Debug.Log("OT: " + IsWalkable(t.X, t.Y));
            }
        }

        private string mapSaveName = "";
        void OnGUI()
        {
            if (InputManager.InEditMode)
            {
                mapSaveName = GUI.TextField(new Rect(Screen.width - 100, Screen.height - 50, 100, 20), mapSaveName);
                if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 30, 100, 30), "Save"))
                    SavedTileMapProvider.SaveTileMap(mapSaveName, this);
                //if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 80, 100, 30), "Regions"))
                //    Layers[TileLayerToSet].UpdateRegions();
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
            double rayPosX = start.x;
            double rayPosY = start.y;
            double rayDirX = goal.x - start.x;
            double rayDirY = goal.y - start.y;

            int mapX = (int)Math.Floor(rayPosX);
            int mapY = (int)Math.Floor(rayPosY);

            int goalMapX = (int)Math.Floor(goal.x);
            int goalMapY = (int)Math.Floor(goal.y);

            if (mapX == goalMapX && mapY == goalMapY)
                return true;

            double sideDistX;
            double sideDistY;

            double deltaDistX = Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
            double deltaDistY = Math.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));
            
            int stepX;
            int stepY;

            bool hit = false;

            if (rayDirX < 0)
            {
                stepX = -1;
                sideDistX = (rayPosX - mapX) * deltaDistX;
            }
            else
            {
                stepX = 1;
                sideDistX = (mapX + 1.0 - rayPosX) * deltaDistX;
            }
            if (rayDirY < 0)
            {
                stepY = -1;
                sideDistY = (rayPosY - mapY) * deltaDistY;
            }
            else
            {
                stepY = 1;
                sideDistY = (mapY + 1.0 - rayPosY) * deltaDistY;
            }

            for (int i = 0; i < 200 && !hit; i++)
            {
                //jump to next map square, OR in x-direction, OR in y-direction
                if (sideDistX < sideDistY)
                {
                    sideDistX += deltaDistX;
                    mapX += stepX;
                }
                else
                {
                    sideDistY += deltaDistY;
                    mapY += stepY;
                }

                if (mapX < 0 || mapY < 0 || mapX >= Width || mapY >= Height)
                    return false;
                else if (mapX == goalMapX && mapY == goalMapY)
                    return true;
                else if (IsProjectileResistant(mapX, mapY))
                    return false;
            }

            return false;
        }

        public bool IsObjectOnTile(int x, int y)
        {
            if (oot == null)
                return false;
            return oot[x, y];
        }

        public bool IsProjectileResistant(int x, int y)
        {
            for (int n = 0; n < Layers.Length; n++)
                if (Layers[n][x, y].TileType.ProjectileResistant || IsObjectOnTile(x, y))
                    return true;
            return false;
        }

        public int LargestIsland { get { return islandP[mi]; } }
    }
}
