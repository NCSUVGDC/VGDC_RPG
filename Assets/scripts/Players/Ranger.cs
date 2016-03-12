using UnityEngine;

namespace VGDC_RPG.Players
{
    public class Ranger : Player
    {
        public override int AttackDamage
        {
            get
            {
                return Mathf.FloorToInt(BaseDamage * 1.0f); //TODO: Stone modifications
            }
        }

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
