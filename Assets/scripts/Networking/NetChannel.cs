using UnityEngine;
using UnityEngine.Networking;

namespace VGDC_RPG.Networking
{
    public class NetChannel
    {
        public int ChannelID;

        public NetChannel(int channelID)
        {
            ChannelID = channelID;
        }

        public void SendMsg(DataWriter w, NetConnection nc)
        {
            byte error;
            if (!NetworkTransport.Send(nc.Peer.HostID, nc.ConnectionID, ChannelID, w.Buffer, w.Length, out error))
                Debug.LogError((NetworkError)error);
        }
    }
}
