using System;
using System.Text;

namespace VGDC_RPG.Networking
{
    public static class MatchClient
    {
        public delegate void ChatReceivedEH(string msg);

        public static event ChatReceivedEH ChatReceived;

        public static bool HasInitialized = false;

        private static NetClient client;
        private static TileMapReciever tmr;
        public static string Username;
        private static byte[] password;

        public static bool Joined;

        public static void Init(string username)
        {
            Username = username;
            client = new NetClient();
            client.Init();
            client.DataRecieved += Client_DataRecieved;
            Joined = false;

            HasInitialized = true;
        }

        public static void Connect(string ip, int port, string pw)
        {
            UnityEngine.Debug.Log("Connecting to " + ip + ":" + port + "...");
            client.Connect(ip, port);
            if (string.IsNullOrEmpty(pw))
                password = null;
            else
                password = System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(pw));
        }

        private static void Client_DataRecieved(NetConnection connection, NetCodes code, DataReader r)
        {
            switch (code)
            {
                case NetCodes.ConnectInfo:
                    uint hostVer = r.ReadUInt32();
                    GameLogic.TeamCount = r.ReadInt32();

                    if (hostVer != Constants.NET_VERSION)
                    {
                        Disconnect();
                        throw new Exception("Host and client versions do not match!  Host: " + hostVer + ", Me: " + Constants.NET_VERSION);
                    }
                    else
                    {
                        var w = new DataWriter(512);
                        w.Write((byte)NetCodes.ConnectInfo);
                        w.Write(Constants.NET_VERSION);
                        if (password != null)
                        {
                            w.Write((byte)1);
                            w.Write(password);
                        }
                        else
                            w.Write((byte)0);
                        w.Write(Username);
                        client.SendReliableOrdered(w);
                    }
                    break;
                case NetCodes.ConnectionAccept:
                    Joined = true;
                    break;
                case NetCodes.Clone:
                    NetCloner.HandleClone(r);
                    break;
                case NetCodes.Event:
                    NetEvents.HandleEvent(connection.ConnectionID, r);
                    break;
                case NetCodes.DownloadTileMap:
                    if (tmr == null)
                        tmr = new TileMapReciever();
                    tmr.HandleData(r);
                    if (tmr.Ready)
                    {
                        GameLogic.SetMapProvider(tmr);
                        tmr = null;
                    }
                    break;
                case NetCodes.Chat:
                    if (ChatReceived != null)
                        ChatReceived(r.ReadString());
                    break;
                case NetCodes.ERROR:
                    if (r.ReadByte() == 1)
                        Disconnect();
                    throw new Exception("Server gave error: " + r.ReadString());
                default:
                    throw new Exception("Invalid Net Code: " + code.ToString());
            }
        }

        public static void Disconnect()
        {
            client.Disconnect();
            client.DataRecieved -= Client_DataRecieved;
        }

        public static void Update(int recRate = NetPeer.REC_PER_UPDATE)
        {
            client.Update(recRate);
        }

        public static void SendChat(string v)
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
            client.SendReliableOrdered(w);

            if (ns != null)
                SendChat(ns);
        }

        public static void Send(DataWriter w)
        {
            client.SendReliableOrdered(w);
        }
    }
}
