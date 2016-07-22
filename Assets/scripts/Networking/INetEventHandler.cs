namespace VGDC_RPG.Networking
{
    public interface INetEventHandler
    {
        int HandlerID { get; }

        void HandleEvent(int cid, DataReader r);
    }
}
