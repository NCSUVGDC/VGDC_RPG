using UnityEngine;
using UnityEngine.Networking;

namespace VGDC_RPG.Networking
{
    public class NetClient : NetPeer
    {
        public enum ConnectionState
        {
            NotConnected,
            Disconnected,
            Connected,
        }

        public ConnectionState ConnectionStatus = ConnectionState.NotConnected;

        private NetChannel reliableChannel;
        private NetChannel reliableOrderedChannel;
        private NetChannel unreliableChannel;
        private HostTopology topology;

        public NetConnection ServerConnection;

        public NetClient()
        {
            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannel = new NetChannel(config.AddChannel(QosType.Reliable));
            reliableOrderedChannel = new NetChannel(config.AddChannel(QosType.ReliableSequenced));
            unreliableChannel = new NetChannel(config.AddChannel(QosType.Unreliable));

            topology = new HostTopology(config, 1);
        }

        public void Init(int port)
        {
#if SIM_LATENCY
            HostID = NetworkTransport.AddHostWithSimulator(topology, 50, 500, port);
#else
            HostID = NetworkTransport.AddHost(topology, port);
#endif
        }

        public void Init()
        {
            HostID = NetworkTransport.AddHost(topology);
        }

        public void Connect(string ip, int port)
        {
            byte error;
            var connectionId = NetworkTransport.Connect(HostID, ip, port, 0, out error);
            if (error != 0)
                Debug.LogError("NC: " + (NetworkError)error);
            else
            {
                ServerConnection = new NetConnection(this, connectionId);
            }
        }

        public void Disconnect()
        {
            byte error;
            if (!NetworkTransport.Disconnect(HostID, ServerConnection.ConnectionID, out error))
                Debug.LogError("NC: " + (NetworkError)error);
            else
            {
                ConnectionStatus = ConnectionState.Disconnected;
                ServerConnection = null;
            }
        }

        public void SendReliable(DataWriter w)
        {
            reliableChannel.SendMsg(w, ServerConnection);
        }

        public void SendReliableOrdered(DataWriter w)
        {
            reliableOrderedChannel.SendMsg(w, ServerConnection);
        }

        public void SendUnreliable(DataWriter w)
        {
            unreliableChannel.SendMsg(w, ServerConnection);
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
                    ConnectionStatus = ConnectionState.Connected;
                    OnPeerConnected(ServerConnection);
                    break;
                case NetworkEventType.DataEvent:
                    var r = new DataReader(recBuffer, dataSize);
                    NetCodes nc = (NetCodes)r.ReadByte();
                    OnDataRecieved(ServerConnection, nc, r);
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("S: Disconnect: " + connectionId);
                    ConnectionStatus = ConnectionState.Disconnected;
                    OnPeerDisonnected(ServerConnection);
                    ServerConnection = null;
                    break;
            }
        }
    }
}
