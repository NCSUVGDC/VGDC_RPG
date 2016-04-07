using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGDC_RPG.Players
{
    public class PlayerEffect
    {
        public int HPRegen;
        public int HPAdd;
        public int TempHPAdd;

        public int Duration;

        public PlayerEffect(int duration, int hpRegen, int hpAdd, int tempHPAdd)
        {
            Duration = duration;
            HPRegen = hpRegen;
            HPAdd = hpAdd;
            TempHPAdd = tempHPAdd;
        }

        public void ApplyEffect(Player player)
        {
            if (HPAdd > 0)
                player.Heal(HPAdd);
            else if (HPAdd < 0 )
                player.Damage(-HPAdd);
            
            player.EffectHitPoints += TempHPAdd;
            if (TempHPAdd > 0)
                player.Heal(TempHPAdd);
            else if ( TempHPAdd < 0 )
                player.Damage(-TempHPAdd);
        }

        public void RemoveEffect(Player player)
        {
            player.EffectHitPoints -= TempHPAdd;
            if (TempHPAdd > 0)
                player.Damage(TempHPAdd);
            else if (TempHPAdd < 0)
                player.Heal(-TempHPAdd);
        }

        public void Turn(Player player)
        {
            if (HPRegen > 0)
                player.Heal(HPRegen);

            Duration--;
        }
    }
}
