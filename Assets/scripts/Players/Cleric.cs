using UnityEngine;
using VGDC_RPG.Players.Weapons;

namespace VGDC_RPG.Players
{
    public class Cleric : Player
    {
        public override string GUIName
        {
            get
            {
                return "Cleric";
            }
        }

        public override string AssetName
        {
            get
            {
                return "Cleric";
            }
        }

        public Cleric()
        {
            Inventory.Add(new HealingStaff());
        }
    }
}
