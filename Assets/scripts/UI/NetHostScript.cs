//using UnityEngine;
//using UnityEngine.SceneManagement;
//using VGDC_RPG.TileMapProviders;
//using VGDC_RPG.Map;
//using VGDC_RPG.Networking;
//using System.Collections.Generic;
//using System;
//using VGDC_RPG.Players.PlayerControllers;

//namespace VGDC_RPG.UI
//{
//    public class NetHostScript : MonoBehaviour
//    {
//        private TeamSetupInfo hostTeam;
//        private List<TeamSetupInfo> clientTeamList = new List<TeamSetupInfo>();
//        private List<TeamSetupInfo> aiTeamList = new List<TeamSetupInfo>();

//        private Dictionary<int, ClientInfo> clientInfo = new Dictionary<int, ClientInfo>();

//        string[] names = new string[]
//        {
//            "Warrior",
//            "Cleric",
//            "Grenadier",
//            "Ranger",
//            "Enemy"
//        };

//        private int state;
//        private NetServer server;

//        public string Username = "Server";

//        // Use this for initialization
//        void Start()
//        {
//            server = new NetServer(7);
//            server.DataRecieved += Server_DataRecieved;
//            server.PeerConnected += Server_PeerConnected;
//            server.PeerDisconnected += Server_PeerDisconnected;
//            server.Init(8888);

//            hostTeam = new TeamSetupInfo(-1, 1, 1, 1, 1, 0);
//        }

//        private void Server_PeerDisconnected(NetConnection connection)
//        {
//            clientInfo.Remove(connection.ConnectionID);
//            int t = 1;
//            foreach (var c in clientInfo.Values)
//                c.Team = t++;
//        }

//        private void Server_PeerConnected(NetConnection connection)
//        {
//            clientInfo.Add(connection.ConnectionID, new ClientInfo(connection, clientInfo.Count));
//            GetConnectInfo(connection);
//        }

//        private void Server_DataRecieved(NetConnection connection, NetCodes code, DataReader r)
//        {
//            Debug.Log("S: ID: " + code.ToString());
//            switch (code)
//            {
//                case NetCodes.ConnectInfo:
//                    connection.Username = r.ReadString();
//                    Debug.Log("S: Client player name: " + connection.Username);
//                    break;
//                case NetCodes.Chat:
//                    SendChat(connection.Username + "> " + r.ReadString());
//                    break;
//                case NetCodes.DownloadTileMap:
//                    TileMapSender.Send(mapdata, server, connection);
//                    break;
//            }
//        }

//        private void GetConnectInfo(NetConnection connection)
//        {
//            byte[] buffer = new byte[1024];
//            var w = new DataWriter(buffer);
//            w.Write((byte)NetCodes.ConnectInfo);
//            w.Write("ASV1");
//            w.Write(server.Connections.Count);

//            server.SendReliable(w, connection);
//        }

//        private void SendChat(string v)
//        {
//            byte[] buffer = new byte[1024];
//            var w = new DataWriter(buffer);
//            w.Write((byte)NetCodes.Chat);
//            w.Write(v);

//            Debug.Log("S: Sending chat: " + v);
//            server.SendReliableToAll(w);
//        }

//        // Update is called once per frame
//        void Update()
//        {

//        }

//        string mw = "32", mh = "32";

//        private ushort[][,] mapdata;

//        void OnGUI()
//        {
//            var buttonWidth = 100;
//            var buttonHeight = 30;

//            if (state == 0)
//            {
//                mw = GUI.TextField(new Rect(0, 0, buttonWidth, buttonHeight), mw);
//                mh = GUI.TextField(new Rect(buttonWidth, 0, buttonWidth, buttonHeight), mh);
//                if (GUI.Button(new Rect(0, buttonHeight * 1, buttonWidth, buttonHeight), "Drunk-Walk Cave"))
//                {
//                    mapdata = new DrunkManCaveProvider(int.Parse(mw), int.Parse(mh)).GetTileMap();
//                    GameLogic.Instance.Map = TileMap.Construct(mapdata);
//                    state = 1;
//                }
//                if (GUI.Button(new Rect(0, buttonHeight * 2, buttonWidth, buttonHeight), "Perlin Landscape"))
//                {
//                    mapdata = new TestTileMapProvider(int.Parse(mw), int.Parse(mh)).GetTileMap();
//                    GameLogic.Instance.Map = TileMap.Construct(mapdata);
//                    state = 1;
//                }
//            }
//            else if (state == 1)
//            {
//                if (GUI.Button(new Rect(0, 0, buttonWidth / 4, buttonHeight), "-") && aiTeamList.Count > 0)
//                    aiTeamList.RemoveAt(aiTeamList.Count - 1);
//                else if (GUI.Button(new Rect(3 * buttonWidth / 4, 0, buttonWidth / 4, buttonHeight), "+"))
//                    aiTeamList.Add(new TeamSetupInfo(0, 0, 0, 0, 2));

//                for (int j = 0; j < 5; j++)
//                {
//                    GUI.Label(new Rect(j * buttonWidth + j * 10, buttonHeight, buttonWidth, buttonHeight), names[j]);
//                }
//                int i = 0;
//                RenderTeam(i++, hostTeam);
//                foreach (var t in clientTeamList)
//                    RenderTeam(i++, t);
//                foreach (var t in aiTeamList)
//                    RenderTeam(i++, t);

//                if (GUI.Button(new Rect((Screen.width - buttonWidth), 8 * buttonHeight, buttonWidth, buttonHeight), "Back"))
//                    SceneManager.LoadScene("scenes/mainMenu");

//                if (GUI.Button(new Rect((Screen.width - buttonWidth), 7 * buttonHeight, buttonWidth, buttonHeight), "Start"))
//                {
//                    SetTeams(1 + clientTeamList.Count + aiTeamList.Count);
//                    SpawnTeam(hostTeam);
//                    foreach (var t in clientTeamList)
//                        SpawnTeam(t);
//                    foreach (var t in aiTeamList)
//                        SpawnTeam(t);
//                }
//            }
//        }

//        private void SetTeams(int teams)
//        {
//            GameLogic.Instance.SetTeams(teams);

//            DataWriter w = new DataWriter(1024);
//            w.Write((byte)NetCodes.SetTeams);
//            w.Write(teams);
//            server.SendReliableOrderedToAll(w);
//        }

//        private void RenderTeam(int i, TeamSetupInfo ti)
//        {
//            var buttonWidth = 100;
//            var buttonHeight = 30;

//            for (int j = 0; j < 5; j++)
//            {
//                GUI.Label(new Rect(j * buttonWidth + buttonWidth / 4 + j * 10, i * buttonHeight * 3 + 2 * buttonHeight, buttonWidth / 4, buttonHeight), ti.UnitCount[j].ToString());
//                if (GUI.Button(new Rect(j * buttonWidth + j * 10, i * buttonHeight * 3 + 2 * buttonHeight, buttonWidth / 4, buttonHeight), "-") && ti.UnitCount[j] > 0)
//                    ti.UnitCount[j]--;
//                else if (GUI.Button(new Rect(j * buttonWidth + 3 * buttonWidth / 4 + j * 10, i * buttonHeight * 3 + 2 * buttonHeight, buttonWidth / 4, buttonHeight), "+"))
//                    ti.UnitCount[j]++;
//            }
//        }

//        private int tin = 0;
//        private void SpawnTeam(TeamSetupInfo ti)
//        {
//            IPlayerController pc;

//            for (int i = 0; i < ti.UnitCount[0]; i++)
//            {
//                pc = GetPC(ti);
//                GameLogic.Instance.SpawnPlayer(GameLogic.Instance.WarriorPrefab, pc, tin);
//            }

//            for (int i = 0; i < ti.UnitCount[1]; i++)
//            {
//                pc = GetPC(ti);
//                GameLogic.Instance.SpawnPlayer(GameLogic.Instance.ClericPrefab, pc, tin);
//            }

//            for (int i = 0; i < ti.UnitCount[2]; i++)
//            {
//                pc = GetPC(ti);
//                GameLogic.Instance.SpawnPlayer(GameLogic.Instance.GrenadierPrefab, pc, tin);
//            }

//            for (int i = 0; i < ti.UnitCount[3]; i++)
//            {
//                pc = GetPC(ti);
//                GameLogic.Instance.SpawnPlayer(GameLogic.Instance.RangerPrefab, pc, tin);
//            }

//            for (int i = 0; i < ti.UnitCount[4]; i++)
//            {
//                pc = GetPC(ti);
//                GameLogic.Instance.SpawnPlayer(GameLogic.Instance.AIPrefab, pc, tin);
//            }



//            tin++;
//        }

//        private static IPlayerController GetPC(TeamSetupInfo ti)
//        {
//            IPlayerController pc;
//            if (ti.AI)
//                pc = new DumbAIController();
//            else if (ti.CID == -1)
//                pc = new PlayerController();
//            else
//                pc = new RemoteController();
//            return pc;
//        }
//    }
//}
