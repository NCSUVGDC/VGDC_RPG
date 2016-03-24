namespace VGDC_RPG.Players
{
    public class Robot : Player
    {
        public override string GUIName
        {
            get
            {
                return "Robot";
            }
        }

        public override int MaxHitPoints
        {
            get
            {
                return 50;
            }
        }
    }
}
