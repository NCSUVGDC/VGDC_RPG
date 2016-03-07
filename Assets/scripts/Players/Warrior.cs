using UnityEngine;
using System.Collections;

namespace VGDC_RPG.Players
{
    public class Warrior : UserPlayer
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
                return "Warrior";
            }
        }
    }
}
