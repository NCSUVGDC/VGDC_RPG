using VGDC_RPG.Players.Weapons;

namespace VGDC_RPG.Players
{
    public class Warrior : Player
    {
        public override string GUIName
        {
            get
            {
                return "Warrior";
            }
        }

        public override string AssetName
        {
            get
            {
                return "Warrior";
            }
        }

        public Warrior()
        {
            var w = new MeleeWeapon();
            Inventory.Add(w);
            ActiveWeapon = w;
        }
    }
}
