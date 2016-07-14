//using UnityEngine;
//using VGDC_RPG.Networking;

//public class NetworkServerTestScript : MonoBehaviour
//{
//    NetServer ns;

//    // Use this for initialization
//    void Start()
//    {
//        Debug.developerConsoleVisible = true;

//        ns = new NetServer(10);
//        ns.DataRecieved += Ns_DataRecieved;
//        ns.PeerConnected += Ns_PeerConnected;
//        ns.Init(8888);
//        Debug.Log("S: hostID: " + ns.HostID);

//    }

//    private void Ns_PeerConnected(NetConnection connection)
//    {
//        GetConnectInfo(connection);
//    }

//    private void Ns_DataRecieved(NetConnection connection, NetCodes code, DataReader r)
//    {
//        Debug.Log("S: ID: " + code.ToString());
//        switch (code)
//        {
//            case NetCodes.ConnectInfo:
//                connection.Username = r.ReadString();
//                Debug.Log("S: Client player name: " + connection.Username);
//                break;
//            case NetCodes.Chat:
//                SendChat(connection.Username + "> " + r.ReadString());
//                break;
//            case NetCodes.DownloadTileMap:
//                TileMapSender.Send(new VGDC_RPG.TileMapProviders.TestTileMapProvider(32, 32).GetTileMap(), ns, connection);
//                break;
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        ns.Update();
//    }

//    void OnGUI()
//    {
//        if (GUI.Button(new Rect(0, 0, 100, 30), "Send TM"))
//        {
//            var w = new DataWriter(1024);
//            w.Write((byte)NetCodes.DownloadTileMap);
//            w.Write(0);
//            w.Write(32);
//            w.Write(32);
//            w.Write(3);

//            ns.SendReliableToAll(w);
//        }
//    }

//    private void GetConnectInfo(NetConnection connection)
//    {
//        byte[] buffer = new byte[1024];
//        var w = new DataWriter(buffer);
//        w.Write((byte)NetCodes.ConnectInfo);
//        w.Write("TSV0");
//        w.Write(ns.Connections.Count);

//        ns.SendReliable(w, connection);
//    }

//    private void SendChat(string v)
//    {
//        byte[] buffer = new byte[1024];
//        var w = new DataWriter(buffer);
//        w.Write((byte)NetCodes.Chat);
//        w.Write(v);

//        Debug.Log("S: Sending chat: " + v);
//        ns.SendReliableToAll(w);
//    }
//}
