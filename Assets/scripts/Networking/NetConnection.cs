using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

namespace VGDC_RPG.Networking
{
    public class NetConnection
    {
        public NetPeer Peer;
        public string Username;
        public int ConnectionID;
        
        public int SendRateBytes
        {
            get
            {
                byte error;
                int r = NetworkTransport.GetPacketSentRate(Peer.HostID, ConnectionID, out error);
                if (error != 0)
                    Debug.LogError((NetworkError)error);
                return r;
            }
        }

        public int RecieveRateBytes
        {
            get
            {
                byte error;
                int r = NetworkTransport.GetPacketReceivedRate(Peer.HostID, ConnectionID, out error);
                if (error != 0)
                    Debug.LogError((NetworkError)error);
                return r;
            }
        }

        public int Port { get; private set; }
        public string IPAddress { get; private set; }

        public NetConnection(NetPeer peer, int cid)
        {
            Peer = peer;
            ConnectionID = cid;

            string ip;
            int port;
            NetworkID network;
            NodeID dstNode;
            byte error;
            NetworkTransport.GetConnectionInfo(peer.HostID, ConnectionID, out ip, out port, out network, out dstNode, out error);
            if (error != 0)
                Debug.LogError((NetworkError)error);
            else
            {
                Port = port;
                IPAddress = ip;
            }
        }
    }
}
