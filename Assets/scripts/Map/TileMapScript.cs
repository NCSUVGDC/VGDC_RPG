using UnityEngine;
using System.Collections;
using Assets.scripts.Map;
using VGDC_RPG.Tiles;
using System.Collections.Generic;
using VGDC_RPG;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TileMapScript : MonoBehaviour
{
    public Texture2D tilesTex;

    public float FramesPerSecond = 2;

    private Material mat;
    private Texture2D texture;
    private TileData[,] map;

    public static TileMapScript Construct(ushort[,] m)
    {
        var tm = GameObject.Instantiate(Resources.Load("tilemap")) as GameObject;
        Debug.Log("here");
        Debug.Assert(tm != null, "tm null");
        var r = tm.GetComponent<TileMapScript>();
        r.map = new TileData[m.GetLength(0), m.GetLength(1)];
        for (int j = 0; j < m.GetLength(1); j++)
            for (int i = 0; i < m.GetLength(0); i++)
                r.map[i, j] = new TileData(Region.GetTile(m, i, j));
        return r;
    }

    public TileData this[int x, int y] { get { return map[x, y]; } }
    public TileData this[Int2 t] { get { return map[t.X, t.Y]; } }

    public int Width { get { return map.GetLength(0); } }
    public int Height { get { return map.GetLength(0); } }

    // Use this for initialization
    void Start()
    {
        mat = GetComponent<MeshRenderer>().sharedMaterials[0];
        GenerateMesh(map.GetLength(0), map.GetLength(1));
        GenerateTexture();
        mat.SetFloat("_TilesWidth", Width);
        mat.SetFloat("_TilesHeight", Height);
        mat.SetFloat("_AtlasSize", Constants.ATLAS_SIZE);
        mat.SetFloat("_AtlasResolution", mat.GetTexture(0).width);
    }

    public void SelectedTile(int x, int y)
    {
        var oc = texture.GetPixel(x, y);
        texture.SetPixel(x, y, new Color(oc.r, oc.g, oc.b, 0.5f));
    }

    public void DeselectedTile(int x, int y)
    {
        var oc = texture.GetPixel(x, y);
        texture.SetPixel(x, y, new Color(oc.r, oc.g, oc.b, 0.0f));
    }

    public void ApplySelection()
    {
        texture.Apply();
    }

    public void ClearHighlights()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                DeselectedTile(x, y);
        ApplySelection();
    }

    void GenerateTexture()
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

    private void GenerateMesh(int width, int height)
    {
        width--;
        height--;

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

        MeshFilter mf = GetComponent<MeshFilter>();

        mf.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetInt("_Frame", Mathf.FloorToInt(Time.realtimeSinceStartup * FramesPerSecond));
    }

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

    public void BlockTile(int x, int y)
    {
        map[x, y].ObjectOnTile = true;
    }

    public void UnblockTile(int x, int y)
    {
        map[x, y].ObjectOnTile = false;
    }
}