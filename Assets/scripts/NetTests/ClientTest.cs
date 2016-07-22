using UnityEngine;
using VGDC_RPG.Networking;
using VGDC_RPG;

public class ClientTest : MonoBehaviour
{
    public string HostIP;
    public int HostPort;
    public string MatchPassword;

    public string Username;

    // Use this for initialization
    void Start()
    {
        MatchClient.Init(Username);
        GameLogic.Init();
        GameLogic.IsHost = false;
        
        MatchClient.Connect(HostIP, HostPort, MatchPassword);
    }

    // Update is called once per frame
    void Update()
    {
        MatchClient.Update();
    }
}
