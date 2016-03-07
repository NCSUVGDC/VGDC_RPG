using UnityEngine;
using System.Collections;
using VGDC_RPG.Tiles;
using System.Collections.Generic;
using VGDC_RPG;
using VGDC_RPG.Map;
using System;
using VGDC_RPG.TileMapProviders;

/// <summary>
/// Script for the TileMap game objects.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TileMap : MonoBehaviour
{
    /// <summary>
    /// The texture with tile information used by the shader to render the tile map.
    /// </summary>
    public Texture2D tilesTex;

    /// <summary>
    /// The framerate to animate tiles at.
    /// </summary>
    public float FramesPerSecond = 2;

    public ushort TileIDToSet = 1;

    private Material mat;
    private Texture2D texture;
    private Texture2D lightTexture;
    private TileData[,] map;
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
    public static TileMap Construct(ushort[,] m)
    {
        var tm = GameObject.Instantiate(Resources.Load("tilemap")) as GameObject;
        Debug.Assert(tm != null, "TileMap Resource GameObject null!");
        var r = tm.GetComponent<TileMap>();
        r.map = new TileData[m.GetLength(0), m.GetLength(1)];
        for (int j = 0; j < m.GetLength(1); j++)
            for (int i = 0; i < m.GetLength(0); i++)
                r.map[i, j] = new TileData(Region.GetTile(m, i, j));
        r.islands = new byte[r.Width, r.Height];
        for (int y = 0; y < r.Height; y++)
            for (int x = 0; x < r.Width; x++)
            {
                if (r.islands[x, y] == 0 && r[x, y].TileType.Walkable)
                {
                    r.FloodFillIsland(x, y, (byte)(r.islandP[0] + 1));
                    r.islandP[0]++;
                }
            }
        int lm = 0;
        for (int i = 1; i < r.islandP[0]; i++)
            if (r.islandP[i] > lm)
            {
                r.mi = i;
                lm = r.islandP[i];
                Debug.Log("MI: " + r.mi);
            }
        return r;
    }

    private void FloodFillIsland(int x, int y, byte ii)
    {
        islands[x, y] = ii;
        islandP[ii]++;
        var n = GetNeighbors(new Int2(x, y));
        foreach (var t in n)
        {
            if (this[t].TileType.Walkable && islands[t.X, t.Y] == 0)
                FloodFillIsland(t.X, t.Y, ii);  //Doesn't really need to be recursive.
        }
    }

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

    /// <summary>
    /// The number of tiles verically.
    /// </summary>
    public int Width { get { return map.GetLength(0); } }
    /// <summary>
    /// The number of tiles horizontally.
    /// </summary>
    public int Height { get { return map.GetLength(1); } }

    // Use this for initialization
    void Start()
    {
        mat = GetComponent<MeshRenderer>().sharedMaterials[0];
        var mesh = GenerateMesh(map.GetLength(0), map.GetLength(1));
        GetComponent<MeshFilter>().mesh = mesh;
        GenerateTexture();
        //System.IO.File.WriteAllBytes("C:\\Users\\Matthew\\Pictures\\tiletest.png", texture.EncodeToPNG());
        mat.SetFloat("_TilesWidth", Width);
        mat.SetFloat("_TilesHeight", Height);
        mat.SetFloat("_AtlasSize", Constants.ATLAS_SIZE);
        mat.SetFloat("_AtlasResolution", mat.GetTexture("_MainTex").width);

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

    public bool InSpawn(int x, int y)
    {
        return islands[x, y] == mi;
    }

    private void AddLights()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                if (this[x, y].TileType.EmissionR > 0)
                    lightingR.AddLight(x, y, this[x, y].TileType.EmissionR);
                if (this[x, y].TileType.EmissionG > 0)
                    lightingG.AddLight(x, y, this[x, y].TileType.EmissionG);
                if (this[x, y].TileType.EmissionB > 0)
                    lightingB.AddLight(x, y, this[x, y].TileType.EmissionB);
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
    public void SelectedTile(int x, int y)
    {
        if (texture == null)
            return;
        var oc = texture.GetPixel(x, y);
        texture.SetPixel(x, y, new Color(oc.r, oc.g, oc.b, 0.5f));
    }

    /// <summary>
    /// Marks a tile as unselected.
    /// Be sure once all tiles are selected/deselected to call ApplySelection to make changes visible.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void DeselectedTile(int x, int y)
    {
        if (texture == null)
            return;
        var oc = texture.GetPixel(x, y);
        texture.SetPixel(x, y, new Color(oc.r, oc.g, oc.b, 0.0f));
    }

    /// <summary>
    /// Applies the changes in tile selection to the tile texture.
    /// </summary>
    public void ApplySelection()
    {
        if (texture == null)
            return;
        texture.Apply();
    }

    /// <summary>
    /// Clears the tile selections.
    /// </summary>
    public void ClearSelection()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                DeselectedTile(x, y);
        ApplySelection();
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

    private Mesh GenerateMesh(int width, int height)
    {
        int triCount = width * height * 2;
        int vertCount = (width + 1) * (height + 1);

        Vector3[] vertices = new Vector3[vertCount];
        Vector3[] normals = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] indices = new int[triCount * 3];

        for (int y = 0; y < height + 1; y++)
            for (int x = 0; x < width + 1; x++)
            {
                vertices[y * (width + 1) + x] = new Vector3(x, 0, y);
                normals[y * (width + 1) + x] = Vector3.up;
                uvs[y * (width + 1) + x] = new Vector2(x / (float)(width + 1), y / (float)(height + 1));
            }

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                int i = (y * width + x) * 6;
                indices[i + 0] = y * (width + 1) + x;
                indices[i + 1] = y * (width + 1) + x + width + 1;
                indices[i + 2] = y * (width + 1) + x + width + 2;

                indices[i + 3] = y * (width + 1) + x;
                indices[i + 4] = y * (width + 1) + x + width + 2;
                indices[i + 5] = y * (width + 1) + x + 1;
            }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = indices;

        /*MeshFilter mf = GetComponent<MeshFilter>();

        mf.mesh = mesh;*/
        return mesh;
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetInt("_Frame", Mathf.FloorToInt(Time.realtimeSinceStartup * FramesPerSecond));

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
                    SetTile(t.X, t.Y, TileIDToSet);//(ushort)((this[t].TileTypeID % 20) + 1));
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
                UpdateRegions();
        }
    }

    /// <summary>
    /// Recalculates region bounderies.
    /// </summary>
    public void UpdateRegions()
    {
        ushort[,] m = new ushort[Width, Height];
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                m[x, y] = Region.GetBase(this[x, y].TileTypeID);//SetTile(x, y, Region.GetBase(this[x, y].TileTypeID));
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                SetTile(x, y, Region.GetTile(m, x, y));//SetTile(x, y, Region.GetBase(this[x, y].TileTypeID));
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
        map[x, y].ObjectOnTile = true;
    }

    /// <summary>
    /// Marks a tile as unobstructed so that players can walk onto it if it is walkable.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    public void UnblockTile(int x, int y)
    {
        map[x, y].ObjectOnTile = false;
    }

    /// <summary>
    /// Sets a tile on the tilemap and marks the lighting to be updated on the next update.
    /// </summary>
    /// <param name="x">X coordinate of the tile location.</param>
    /// <param name="y">Y coordinate of the tile location.</param>
    /// <param name="id">ID of the tile type to set.</param>
    public void SetTile(int x, int y, ushort id)
    {
        if (this[x, y].TileTypeID == id)
            return;

        var ntd = new TileData(id);

        texture.SetPixel(x, y, ntd.TileType.RenderData);
        texture.Apply();

        lightingR.RemoveLight(x, y);
        lightingG.RemoveLight(x, y);
        lightingB.RemoveLight(x, y);
        
        if (ntd.TileType.EmissionR > 0)
            lightingR.AddLight(x, y, ntd.TileType.EmissionR);
        if (ntd.TileType.EmissionG > 0)
            lightingG.AddLight(x, y, ntd.TileType.EmissionG);
        if (ntd.TileType.EmissionB > 0)
            lightingB.AddLight(x, y, ntd.TileType.EmissionB);

        lightingDirty = true;

        this[x, y] = new TileData(id);
    }
}