using UnityEngine;
using System.Collections;

public class scrollingBackground : MonoBehaviour {

    public float scrollSpeed;

    void Update() {
        float diag = Mathf.Repeat(Time.time * scrollSpeed, 1);
        Vector2 offset = new Vector2(diag, diag);
        GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}