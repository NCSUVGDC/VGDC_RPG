namespace VGDC_RPG.Players.PlayerControllers
{
    public interface IPlayerController
    {
        Player Player { get; set; }

        void TurnStart();
        void OnGUI();
        void Update();
    }
}
