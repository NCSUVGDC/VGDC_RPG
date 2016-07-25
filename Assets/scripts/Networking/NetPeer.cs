namespace VGDC_RPG.Networking
{

    public delegate void PeerConnectedEH(NetConnection connection);
    public delegate void PeerDisconnectedEH(NetConnection connection);
    public delegate void DataRecievedEH(NetConnection connection, NetCodes code, DataReader r);

    public class NetPeer
    {
        public const int REC_PER_UPDATE = 1;  //TODO: packets are received multiple times if set to anything other that 1

        public int HostID;

        public event PeerConnectedEH PeerConnected;
        public event PeerDisconnectedEH PeerDisconnected;
        public event DataRecievedEH DataRecieved;

        protected void OnDataRecieved(NetConnection connection, NetCodes code, DataReader r)
        {
            if (DataRecieved != null)
                DataRecieved(connection, code, r);
        }

        protected void OnPeerConnected(NetConnection connection)
        {
            if (PeerConnected != null)
                PeerConnected(connection);
        }

        protected void OnPeerDisonnected(NetConnection connection)
        {
            if (PeerDisconnected != null)
                PeerDisconnected(connection);
        }

        public virtual void Update()
        {

        }
    }
}
