using UnityEngine;
using System.Collections;

public class scrollingBackgroundTest : MonoBehaviour {

    public float scrollSpeed;
    private Vector2 savedOffset;

    void Start() {
        savedOffset = GetComponent<Renderer>().sharedMaterial.GetTextureOffset("_MainTex");
    }

    void Update() {
        float y = -Mathf.Repeat(Time.time * scrollSpeed, 1);
        Vector2 offset = new Vector2(y, savedOffset.y);
        GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
    }

    void OnDisable() {
        GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", savedOffset);
    }
}