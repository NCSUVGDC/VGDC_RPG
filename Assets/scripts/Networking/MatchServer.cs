using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGDC_RPG.Networking
{
    public static class MatchServer
    {
        private static NetServer server;

        public static int ConnectionCount { get { return server.Connections.Count; } }

        public static void Init(int port)
        {
            server = new NetServer(8);
            server.Init(port);
            server.DataRecieved += Server_DataRecieved;
        }

        private static void Server_DataRecieved(NetConnection connection, NetCodes code, DataReader r)
        {
            switch (code)
            {
                default:
                    throw new Exception("S: Invalid net code: " + code.ToString());
            }
        }

        public static void Update()
        {
            server.Update();
        }

        public static void Send(DataWriter w)
        {
            server.SendReliableOrderedToAll(w);
        }

        public static void SendMap()
        {
            TileMapSender.SendToAll(GameLogic.Instance.mapConstructionData, server);
        }
    }
}
