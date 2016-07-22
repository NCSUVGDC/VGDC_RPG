using System;
using System.Text;

namespace VGDC_RPG.Networking
{
    public static class MatchServer
    {
        public delegate void ChatReceivedEH(string msg);
        public delegate void PlayerJoinedEH(int cid);
        public delegate void PlayerLeftEH(int cid);

        public static event ChatReceivedEH ChatReceived;
        public static event PlayerJoinedEH PlayerJoined;
        public static event PlayerLeftEH PlayerLeft;

        private static NetServer server;

        public static int PeerConnectionCount { get { return server.Connections.Count; } }
        public static int MaxConnections = 16;
        public static int CurrentConnections = 0;
        public static string MatchName = "Test Match";
        private static byte[] MatchPassword;

        public static string Username;

        public static void Init(int port, string pw)
        {
            if (string.IsNullOrEmpty(pw))
                MatchPassword = new byte[0];
            else
                MatchPassword = System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(pw));

            server = new NetServer(MaxConnections * 2);
            server.Init(port);
            server.DataRecieved += Server_DataRecieved;
            server.PeerConnected += Server_PeerConnected;
            server.PeerDisconnected += Server_PeerDisconnected;
        }

        private static void Server_DataRecieved(NetConnection connection, NetCodes code, DataReader r)
        {
            switch (code)
            {
                case NetCodes.ConnectInfo:
                    uint netVersion = r.ReadUInt32();
                    if (netVersion != Constants.NET_VERSION)
                    {
                        Error("Host and client versions do not match!  Client: " + netVersion + ", Me: " + Constants.NET_VERSION, true, connection);
                    }
                    else
                    {
                        var rp = r.ReadByte() == 1 ? r.ReadBytes(32) : new byte[0];
                        if (MatchPassword.Length != 0 && rp.Length == 0)
                        {
                            Error("Password required.", true, connection);
                            return;
                        }
                        for (int i = 0; i < rp.Length; i++)
                            if (rp[i] != MatchPassword[i])
                            {
                                Error("Incorrect password.", true, connection);
                                return;
                            }

                        string username = r.ReadString();
                        connection.Tag = username;
                        connection.Groups = ConnectionGroup.One;

                        var w = new DataWriter(1);
                        w.Write((byte)NetCodes.ConnectionAccept);
                        server.SendReliableOrdered(w, connection);

                        if (PlayerJoined != null)
                            PlayerJoined(connection.ConnectionID);

                        SendChatRaw(username + " has joined.");
                    }
                    break;
                case NetCodes.Chat:
                    if ((connection.Groups & ConnectionGroup.One) != ConnectionGroup.One)
                        break;
                    SendChatRaw(connection.Tag as string + ": " + r.ReadString());
                    break;
                case NetCodes.Event:
                    NetEvents.HandleEvent(connection.ConnectionID, r);
                    SendExclude(new DataWriter(r), connection);
                    break;
                default:
                    throw new Exception("S: Invalid net code: " + code.ToString());
            }
        }

        public static void SendChat(string v)
        {
            SendChatRaw(Username + ": " + v);
        }

        private static void SendChatRaw(string v)
        {
            string ns = null;
            if (v.Length > 500)
            {
                ns = v.Substring(500);
                v = v.Substring(0, 500);
            }
            var w = new DataWriter(512);
            w.Write((byte)NetCodes.Chat);
            w.Write(v);
            server.SendReliableOrderedToAll(w);

            if (ChatReceived != null)
                ChatReceived(v);
            UnityEngine.Debug.Log("CR: " + v);

            if (ns != null)
                SendChatRaw(ns);
        }

        public static void Error(string v, bool disconnect, NetConnection connection)
        {
            UnityEngine.Debug.LogError(connection.IPAddress + ": " +  v);
            var w = new DataWriter(512);
            w.Write((byte)NetCodes.ERROR);
            w.Write((byte)(disconnect ? 1 : 0));
            w.Write(v);
            server.SendReliableOrdered(w, connection);
        }

        private static void Server_PeerConnected(NetConnection connection)
        {
            UnityEngine.Debug.Log("Peer connected: " + connection.IPAddress);
            var w = new DataWriter(512);
            w.Write((byte)NetCodes.ConnectInfo);
            w.Write(Constants.NET_VERSION);
            w.Write(MaxConnections);
            w.Write(CurrentConnections);
            w.Write(GameLogic.TeamCount);
            w.Write(MatchName);
            server.SendReliableOrdered(w, connection);
        }

        private static void Server_PeerDisconnected(NetConnection connection)
        {
            if (connection.Tag != null)
            {
                SendChatRaw((string)connection.Tag + " has left.");
                if (PlayerLeft != null)
                    PlayerLeft(connection.ConnectionID);
            }
        }

        public static void Update()
        {
            server.Update();
        }

        public static void Send(DataWriter w)
        {
            server.SendReliableOrderedToGroup(w, ConnectionGroup.One);
        }

        public static void SendExclude(DataWriter w, NetConnection c)
        {
            server.SendReliableOrderedToGroupExclude(w, ConnectionGroup.One, c);
        }

        public static void SendMap()
        {
            TileMapSender.SendToAll(GameLogic.mapConstructionData);
        }

        public static void SendTo(DataWriter w, NetConnection c)
        {
            server.SendReliableOrdered(w, c);
        }

        public static NetConnection GetConnection(int cid)
        {
            return server.Connections[cid];
        }
    }
}
