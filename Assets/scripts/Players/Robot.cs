using VGDC_RPG.Players.Weapons;

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

        public override int BaseMaxHitPoints
        {
            get
            {
                return 50;
            }
        }

        public override string AssetName
        {
            get
            {
                return "Robot";
            }
        }

        public Robot()
        {
            var w = new MeleeWeapon();
            Inventory.Add(w);
            ActiveWeapon = w;
        }
    }
}
