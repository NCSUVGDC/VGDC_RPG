using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace VGDC_RPG.Networking
{
    public class NetServer : NetPeer
    {
        private NetChannel reliableChannel;
        private NetChannel reliableOrderedChannel;
        private NetChannel unreliableChannel;
        private HostTopology topology;

        public Dictionary<int, NetConnection> Connections;

        public NetServer(int connections)
        {
            NetworkTransport.Init();
            Connections = new Dictionary<int, NetConnection>();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannel = new NetChannel(config.AddChannel(QosType.Reliable));
            reliableOrderedChannel = new NetChannel(config.AddChannel(QosType.ReliableSequenced));
            unreliableChannel = new NetChannel(config.AddChannel(QosType.Unreliable));

            topology = new HostTopology(config, connections);
        }

        public void Init(int port)
        {
            HostID = NetworkTransport.AddHost(topology, port);
        }

        public void SendReliable(DataWriter w, NetConnection c)
        {
            reliableChannel.SendMsg(w, c);
        }

        public void SendReliableOrdered(DataWriter w, NetConnection c)
        {
            reliableOrderedChannel.SendMsg(w, c);
        }

        public void SendUnreliable(DataWriter w, NetConnection c)
        {
            unreliableChannel.SendMsg(w, c);
        }

        public void SendReliableToAll(DataWriter w)
        {
            foreach (var c in Connections.Values)
                reliableChannel.SendMsg(w, c);
        }

        public void SendReliableOrderedToAll(DataWriter w)
        {
            foreach (var c in Connections.Values)
                reliableOrderedChannel.SendMsg(w, c);
        }

        public void SendUnreliableToAll(DataWriter w)
        {
            foreach (var c in Connections.Values)
                unreliableChannel.SendMsg(w, c);
        }

        public void SendReliableToGroup(DataWriter w, ConnectionGroup g)
        {
            foreach (var c in Connections.Values)
                if ((c.Groups & g) == g)
                    reliableChannel.SendMsg(w, c);
        }

        public void SendReliableOrderedToGroup(DataWriter w, ConnectionGroup g)
        {
            foreach (var c in Connections.Values)
                if ((c.Groups & g) == g)
                    reliableOrderedChannel.SendMsg(w, c);
        }

        public void SendUnreliableToGroup(DataWriter w, ConnectionGroup g)
        {
            foreach (var c in Connections.Values)
                if ((c.Groups & g) == g)
                    unreliableChannel.SendMsg(w, c);
        }

        public void SendReliableOrderedToGroupExclude(DataWriter w, ConnectionGroup g, NetConnection e)
        {
            foreach (var c in Connections.Values)
                if (c != e && (c.Groups & g) == g)
                    reliableOrderedChannel.SendMsg(w, c);
        }

        public override void Update()
        {
            int connectionId;
            int channelId;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            byte error;
            NetworkEventType recData = NetworkTransport.ReceiveFromHost(HostID, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
            if (error != 0)
                Debug.LogError("S: " + ((NetworkError)error).ToString());
            switch (recData)
            {
                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.ConnectEvent:
                    Debug.Log("S: Incomming connection: " + connectionId);
                    Connections.Add(connectionId, new NetConnection(this, connectionId));
                    OnPeerConnected(Connections[connectionId]);
                    break;
                case NetworkEventType.DataEvent:
                    var r = new DataReader(recBuffer, dataSize);
                    NetCodes nc = (NetCodes)r.ReadByte();
                    OnDataRecieved(Connections[connectionId], nc, r);
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("S: Disconnect: " + connectionId);
                    OnPeerDisonnected(Connections[connectionId]);
                    Connections.Remove(connectionId);
                    break;
            }
        }
    }
}
