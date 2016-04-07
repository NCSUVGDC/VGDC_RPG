using UnityEngine;

namespace VGDC_RPG.Players
{
    public class Grenadier : Player
    {
        public override string GUIName
        {
            get
            {
                return "Grenadier";
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
                return "Grenadier";
            }
        }
    }
}
