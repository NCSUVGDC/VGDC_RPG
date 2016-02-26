using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TileMapScript : MonoBehaviour
{

    public const int atlasWidth = 4;
    public Texture2D tilesTex;
    private Color[] tc = new Color[]
    {
        Color.black,
        new Color(2, 0, 1) / atlasWidth,
        new Color(1, 0, 1) / atlasWidth,
        new Color(3, 0, 4) / atlasWidth,
    };

    public float FramesPerSecond = 2;

    private Material mat;

    // Use this for initialization
    void Start()
    {
        mat = GetComponent<MeshRenderer>().sharedMaterials[0];
        GenerateMesh(64 * 2, 64 * 2);
        GenerateTexture(new VGDC_RPG.TileMapProviders.TestTileMapProvider(64, 64).GetTileMap());
    }

    void GenerateTexture(ushort[,] m)
    {
        int texWidth = m.GetLength(0);
        int texHeight = m.GetLength(1);
        Texture2D texture = new Texture2D(texWidth, texHeight);

        for (int y = 0; y < texHeight; y++)
            for (int x = 0; x < texWidth; x++)
                texture.SetPixel(x, y, tc[m[x, y]]);

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
}