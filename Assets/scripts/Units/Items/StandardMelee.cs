using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VGDC_RPG.Networking;

namespace VGDC_RPG.Units.Items
{
    public class StandardMelee : Weapon, INetClonable
    {
        public const ushort CLONE_OBJ_ID = 2;

        public StandardMelee() : base()
        {
            Name = "Melee";
            Type = WeaponType.Melee;
        }

        public StandardMelee(DataReader r) : base(r)
        {
            Type = WeaponType.Melee;
        }

        public override bool Attack(Unit attacker, Int2 tile)
        {
            var target = GameLogic.GetUnitOnTile(tile);
            if (target != null)
                if (target.TeamID != attacker.TeamID)
                {
                    target.Damage(attacker.Stats.GetAttackDmg(10, target.Stats));
                    return true;
                }
            Debug.Log("No target.");
            return false;
        }

        public override void Clone(DataWriter w)
        {
            w.Write((byte)NetCodes.Clone);
            w.Write(CLONE_OBJ_ID);
            w.Write(HandlerID);
            w.Write(Name);
        }

        public override List<Int2> GetAttackTiles(Unit unit)
        {
            var r = new List<Int2>();
            if (unit.X > 0)
                r.Add(new Int2(unit.X - 1, unit.Y));
            if (unit.Y > 0)
                r.Add(new Int2(unit.X, unit.Y - 1));
            if (unit.X < GameLogic.Map.Width - 1)
                r.Add(new Int2(unit.X + 1, unit.Y));
            if (unit.Y < GameLogic.Map.Height - 1)
                r.Add(new Int2(unit.X, unit.Y + 1));

            return r;
        }
    }
}
