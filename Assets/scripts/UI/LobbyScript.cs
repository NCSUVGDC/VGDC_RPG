using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using VGDC_RPG.Networking;
using VGDC_RPG;

public class LobbyScript : MonoBehaviour
{
    public InputField msgText;
    public InputField logText;

    void Start()
    {
        if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
            MatchServer.ChatReceived += MatchServer_ChatReceived;
        else
            MatchClient.ChatReceived += MatchServer_ChatReceived;
    }

    void Update()
    {
        if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
            MatchServer.Update();
        else
            MatchClient.Update();
    }

    private void MatchServer_ChatReceived(string msg)
    {
        logText.text += msg + '\n';
    }

    public void SendChat()
    {
        if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
            MatchServer.SendChat(msgText.text);
        else
            MatchClient.SendChat(msgText.text);

        msgText.text = "";
    }

    void OnDestroy()
    {
        if (GameLogic.Instance.IsHost && GameLogic.Instance.IsServer)
            MatchServer.ChatReceived -= MatchServer_ChatReceived;
        else
            MatchClient.ChatReceived -= MatchServer_ChatReceived;
    }
}
