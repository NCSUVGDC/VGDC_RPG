using UnityEngine;
using System.Collections;
using VGDC_RPG.Networking;
using VGDC_RPG;

public class ClientTest : MonoBehaviour
{
    public string HostIP;
    public int HostPort;

    // Use this for initialization
    void Start()
    {
        MatchClient.Init();
        new GameLogic();
        GameLogic.Instance.IsHost = false;
        
        MatchClient.Connect(HostIP, HostPort);
    }

    // Update is called once per frame
    void Update()
    {
        MatchClient.Update();
    }
}
