namespace VGDC_RPG.Networking
{
    public interface INetEventHandler
    {
        int HandlerID { get; }

        void HandleEvent(DataReader r);
    }
}
