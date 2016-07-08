using UnityEngine;

public class SmokeTrailParticle : MonoBehaviour
{
    public Texture2D Texture;
    public float MaxLife = 3.0f;
    private float Life;

    // Use this for initialization
    void Start()
    {
        GetComponent<MeshRenderer>().material.mainTexture = Texture;
        Life = MaxLife;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(Life / MaxLife, Life / MaxLife);
        Life -= Time.deltaTime;
        if (Life <= 0)
            Destroy(gameObject);
    }
}
