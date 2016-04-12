using UnityEngine;
using System.Collections;

public class WarpExplode : MonoBehaviour {

    public Texture2D Texture;
    public float MaxLife = 3.0f;
    private float Life;
    public float MaxScale = 16f;
    private Material mat;
    public float Effectiveness;

    // Use this for initialization
    void Start()
    {
        GetComponent<MeshRenderer>().material.mainTexture = Texture;
        mat = GetComponent<MeshRenderer>().material;
        Life = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(Life / MaxLife, Life / MaxLife) * MaxScale;
        Life += Time.deltaTime;
        mat.SetFloat("_Effectiveness", (1 - Life / MaxLife) * Effectiveness);
        if (Life > MaxLife)
            Destroy(gameObject);
    }
}
