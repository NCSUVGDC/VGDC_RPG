using UnityEngine;

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
    }
}
