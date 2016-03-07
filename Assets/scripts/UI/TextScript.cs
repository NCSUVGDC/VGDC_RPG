using UnityEngine;
using System.Collections;

public class TextScript : MonoBehaviour
{
    public float Life = 2.0f;
    private float time = 0;
    public Vector3 Velocity = new Vector3(0, 0, 1);
    public Color Color = Color.white;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > Life)
            Destroy(gameObject);
        GetComponent<TextMesh>().color = Color.Lerp(Color, new Color(Color.r, Color.g, Color.b, 0), time / Life);
        transform.position += Velocity * Time.deltaTime;
    }
}
