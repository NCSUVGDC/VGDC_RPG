using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VGDC_RPG.Networking;

namespace VGDC_RPG.Units.Items
{
    public abstract class Weapon : Item
    {
        public enum WeaponType : byte
        {
            ERROR = 0,
            Melee,
            Ranged
        }

        public WeaponType Type;
        public bool DoesDamage = true;

        public Weapon() : base()
        {
            Name = "No Name Weapon";
            Type = WeaponType.ERROR;
        }

        public Weapon(DataReader r) : base(r)
        {

        }

        public abstract bool Attack(Unit attacker, Int2 tile);
        public abstract List<Int2> GetAttackTiles(Unit unit);
    }
}
