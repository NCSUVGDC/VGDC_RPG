using VGDC_RPG.Networking;

namespace VGDC_RPG
{
    public class TeamSetupInfo
    {
        public int[] UnitCount;
        public int CID;
        public bool AI;

        public TeamSetupInfo(int cid, params int[] uc)
        {
            UnitCount = uc;
            CID = cid;
        }

        public TeamSetupInfo(params int[] uc)
        {
            UnitCount = uc;
            AI = true;
        }
    }

    public class ClientInfo
    {
        public bool Ready;
        public NetConnection Connection;
        public int Team;

        public ClientInfo(NetConnection connection, int team)
        {
            Connection = connection;
            Team = team;
        }
    }
}
