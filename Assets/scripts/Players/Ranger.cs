using UnityEngine;
using VGDC_RPG.Players.Weapons;

namespace VGDC_RPG.Players
{
    public class Ranger : Player
    {
        public override string GUIName
        {
            get
            {
                return "Ranger";
            }
        }

        public override bool Ranged
        {
            get
            {
                return true;
            }
        }

        public override int Range
        {
            get
            {
                return 8;
            }
        }

        public override string AssetName
        {
            get
            {
                return "Ranger";
            }
        }

        public Ranger()
        {
            Inventory.Add(new Bow());
        }
    }
}
