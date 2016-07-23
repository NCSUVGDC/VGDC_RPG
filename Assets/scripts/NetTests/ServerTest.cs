using UnityEngine;
using VGDC_RPG.Networking;
using VGDC_RPG;
using VGDC_RPG.Units;

public class ServerTest : MonoBehaviour
{
    private bool hasCloned = false, hasSentMap = false;

    public Unit TestUnit;
    public Unit TestUnit2;

    public string MatchPassword;

    // Use this for initialization
    void Start()
    {
        MatchServer.Init(8080, MatchPassword);

        GameLogic.Init();
        GameLogic.IsHost = true;
        GameLogic.GenerateTestMap(32, 32);

        TestUnit = new Unit();
        TestUnit.Name = "Billy Bob";
        TestUnit.SetPosition(2, 3);
        TestUnit.Sprite.SetSpriteSet("Grenadier");

        TestUnit2 = new Unit();
        TestUnit2.Name = "Bobby Bill";
        TestUnit2.SetPosition(2, 3);
        TestUnit2.Sprite.SetSpriteSet("Ranger");
    }

    // Update is called once per frame
    void Update()
    {
        MatchServer.Update();

        if (!hasSentMap && MatchServer.PeerConnectionCount > 0)
        {
            MatchServer.SendMap();
            hasSentMap = true;
        }
        else if (!hasCloned && MatchServer.PeerConnectionCount > 0)
        {
            var buffer = new byte[512];

            var w = new DataWriter(buffer);
            TestUnit.Clone(w);
            MatchServer.Send(w);

            w = new DataWriter(buffer);
            TestUnit2.Clone(w);
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

        if (GUI.Button(new Rect(200, 100, 60, 20), "Left"))
            TestUnit.GoTo(TestUnit.X - 3, TestUnit.Y);
        if (GUI.Button(new Rect(260, 100, 60, 20), "Right"))
            TestUnit.GoTo(TestUnit.X + 3, TestUnit.Y);
        if (GUI.Button(new Rect(230, 80, 60, 20), "Up"))
            TestUnit.GoTo(TestUnit.X, TestUnit.Y + 3);
        if (GUI.Button(new Rect(230, 120, 60, 20), "Down"))
            TestUnit.GoTo(TestUnit.X, TestUnit.Y - 3);
    }
}
