using UnityEngine;

namespace VGDC_RPG.Players
{
    public class Cleric : Player
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
                return "Cleric";
            }
        }
    }
}
