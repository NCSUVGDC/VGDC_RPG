using UnityEngine;

public class StatsDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (FindObjectsOfType<StatsDisplay>().Length > 1)
            Destroy(gameObject);
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 10;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

        var ms = deltaTime * 1000.0f;
        var fps = 1.0f / deltaTime;
        GUI.Label(new Rect(0, 0, Screen.width, 16), string.Format("{0:0.0} ms ({1:0.} fps)", ms, fps), style);
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        GUI.Label(new Rect(1, 1, Screen.width, 16), string.Format("{0:0.0} ms ({1:0.} fps)", ms, fps), style);
    }
}
