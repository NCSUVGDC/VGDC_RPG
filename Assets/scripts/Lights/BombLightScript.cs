using UnityEngine;

public class BombLightScript : MonoBehaviour
{
    public float Radius = 0.5f;
    public float DRadius = 0.1f;
    public float DSpeed = 10.0f;
    public Color Color = Color.white;

    // Use this for initialization
    void Start()
    {
        GetComponent<MeshRenderer>().material.color = Color;
    }

    // Update is called once per frame
    void Update()
    {
        var s = (Mathf.Sin(Time.time * DSpeed) * DRadius + Radius) * 2;
        transform.localScale = new Vector3(s, s, 0);
    }
}
