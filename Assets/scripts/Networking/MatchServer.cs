using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGDC_RPG.Networking
{
    public static class MatchServer
    {
        public delegate void ChatReceivedEH(string msg);

        public static event ChatReceivedEH ChatReceived;

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
                        if (MatchPassword != rp)
                        {
                            Error("Incorrect password.", true, connection);
                            return;
                        }
                        else
                        {
                            string username = r.ReadString();
                            connection.Tag = username;
                            connection.Groups = ConnectionGroup.One;
                        }
                    }
                    break;
                case NetCodes.Chat:
                    if ((connection.Groups & ConnectionGroup.One) != ConnectionGroup.One)
                        break;
                    SendChatRaw(connection.Tag as string + ": " + r.ReadString());
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
            var w = new DataWriter(512);
            w.Write((byte)NetCodes.Chat);
            w.Write(v);
            server.SendReliableOrderedToAll(w);

            if (ChatReceived != null)
                ChatReceived(v);
        }

        private static void Error(string v, bool disconnect, NetConnection connection)
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
            var w = new DataWriter(512);
            w.Write((byte)NetCodes.ConnectInfo);
            w.Write(Constants.NET_VERSION);
            w.Write(MaxConnections);
            w.Write(CurrentConnections);
            w.Write(MatchName);
            server.SendReliableOrdered(w, connection);
        }

        public static void Update()
        {
            server.Update();
        }

        public static void Send(DataWriter w)
        {
            server.SendReliableOrderedToGroup(w, ConnectionGroup.One);
        }

        public static void SendMap()
        {
            TileMapSender.SendToAll(GameLogic.Instance.mapConstructionData);
        }
    }
}
