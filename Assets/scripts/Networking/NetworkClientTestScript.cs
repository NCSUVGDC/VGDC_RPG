//using UnityEngine;
//using VGDC_RPG.Networking;

//public class NetworkClientTestScript : MonoBehaviour
//{
//    NetClient nc;

//    // Use this for initialization
//    void Start()
//    {
//        nc = new NetClient();
//        nc.DataRecieved += Nc_DataRecieved;
//        nc.Init();
//        //nc.Connect("127.0.0.1", 8888);
//        Debug.Log("C: hostID: " + nc.HostID);
//    }

//    private void Nc_DataRecieved(NetConnection connection, NetCodes code, DataReader r)
//    {
//        Debug.Log("C: ID: " + nc.ToString());
//        switch (code)
//        {
//            case NetCodes.ConnectInfo:
//                var serverVer = r.ReadString();
//                var connectedPlayers = r.ReadInt32();
//                Debug.Log("C: Server version reported: " + serverVer);
//                Debug.Log("C: Connected players: " + connectedPlayers);
//                SendConnectionInfo();
//                break;
//            case NetCodes.Chat:
//                Debug.Log("C: Chat: " + r.ReadString());
//                break;
//            case NetCodes.DownloadTileMap:
//                if (tmr == null)
//                {
//                    tmr = new TileMapReciever();
//                    tmr.HandleData(r);
//                    AcceptTileMap();
//                }
//                else
//                    tmr.HandleData(r);

//                if (tmr.Ready)
//                {
//                    VGDC_RPG.Map.TileMap.Construct(tmr.GetTileMap());
//                    tmr = null;
//                }
//                break;
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        nc.Update();
//    }

//    string un = "";
//    string msg = "";
//    void OnGUI()
//    {
//        un = GUI.TextField(new Rect(Screen.width / 2, 0, 300, 30), un);
//        if (GUI.Button(new Rect(Screen.width / 2 + 300, 0, 100, 30), "Connect"))
//        {
//            nc.Connect("127.0.0.1", 8888);
//        }

//        msg = GUI.TextField(new Rect(Screen.width / 2, 100, 300, 30), msg);
//        if (GUI.Button(new Rect(Screen.width / 2 + 300, 100, 100, 30), "Send"))
//        {
//            SendChat(msg);
//            msg = "";
//        }

//        if (GUI.Button(new Rect(Screen.width / 2 + 300, 30, 100, 30), "Disconnect"))
//        {
//            nc.Disconnect();
//        }
//    }

//    private void SendChat(string msg)
//    {
//        byte[] buffer = new byte[1024];
//        var w = new DataWriter(buffer);

//        w.Write((byte)NetCodes.Chat);
//        w.Write(msg);

//        Debug.Log("C: Sending chat: " + msg);
//        nc.SendReliable(w);
//    }

//    TileMapReciever tmr;

//    private void AcceptTileMap()
//    {
//        byte[] buffer = new byte[1024];
//        var w = new DataWriter(buffer);

//        w.Write((byte)NetCodes.DownloadTileMap);

//        nc.SendReliable(w);
//    }

//    private void SendConnectionInfo()
//    {
//        byte[] buffer = new byte[1024];
//        var w = new DataWriter(buffer);

//        w.Write((byte)NetCodes.ConnectInfo);
//        w.Write(un);

//        nc.SendReliable(w);
//    }
//}
