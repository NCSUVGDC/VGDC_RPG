using UnityEngine;
using System.Collections;
using VGDC_RPG.Networking;
using VGDC_RPG;
using VGDC_RPG.Units;

public class ServerTest : MonoBehaviour
{
    private bool hasCloned = false;

    public Unit TestUnit;

    // Use this for initialization
    void Start()
    {
        MatchServer.Init(8080);

        new GameLogic();
        GameLogic.Instance.IsHost = true;

        TestUnit = new Unit();
        TestUnit.Name = "Billy Bob";
        TestUnit.SetPosition(2, 3);
        TestUnit.Sprite.SetSpriteSet("Grenadier");
    }

    // Update is called once per frame
    void Update()
    {
        MatchServer.Update();

        if (!hasCloned && MatchServer.ConnectionCount > 0)
        {
            var w = new DataWriter(512);
            TestUnit.Clone(w);
            MatchServer.Send(w);
            hasCloned = true;
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(200, 200, 60, 20), "Left"))
            TestUnit.SetPosition(TestUnit.X - 1, TestUnit.Y);
        if (GUI.Button(new Rect(260, 200, 60, 20), "Right"))
            TestUnit.SetPosition(TestUnit.X + 1, TestUnit.Y);
        if (GUI.Button(new Rect(230, 180, 60, 20), "Up"))
            TestUnit.SetPosition(TestUnit.X, TestUnit.Y + 1);
        if (GUI.Button(new Rect(230, 220, 60, 20), "Down"))
            TestUnit.SetPosition(TestUnit.X, TestUnit.Y - 1);
    }
}
