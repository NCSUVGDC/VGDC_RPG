﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VGDC_RPG.Networking;

namespace VGDC_RPG.Units.Items
{
    public class GrenadeWeapon : Weapon, INetClonable
    {
        public const ushort CLONE_OBJ_ID = 4;

        public int SplashRange = 3;

        public GrenadeWeapon() : base()
        {
            Name = "Grenade";
            Type = WeaponType.Ranged;
        }

        public GrenadeWeapon(DataReader r) : base(r)
        {
            Type = WeaponType.Ranged;
        }

        public override bool Attack(Unit attacker, Int2 tile)
        {
            bool canAttack = false;
            for (int y = -SplashRange; y <= SplashRange; y++)
                for (int x = -SplashRange; x <= SplashRange; x++)
                {
                    if (GameLogic.Map.ProjectileRayCast(new Vector2(tile.X, tile.Y), new Vector2(tile.X + x, tile.Y + y)))
                    {
                        var t = GameLogic.GetUnitOnTile(new Int2(x + tile.X, y + tile.Y));
                        var dmg = 0;
                        if (t != null)
                            dmg = Constants.GetDamage(attacker, t, new Vector2(tile.X, tile.Y), SplashRange);
                        else
                            dmg = Mathf.CeilToInt(Constants.GetPDamage(new Int2(Mathf.FloorToInt(tile.X), Mathf.FloorToInt(tile.Y)), new Vector2(tile.X + x, tile.Y + y), SplashRange));
                        if (t != null && dmg > 0 && attacker.TeamID != t.TeamID)
                        {
                            t.Damage(attacker.Stats.GetAttackDmg(dmg, t.Stats));
                            canAttack =  true;
                        }
                    }
                }

            return canAttack;
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
            List<Int2> attackTiles = new List<Int2>();
            int boost = Mathf.CeilToInt(unit.Stats.Range * Stones.Range[unit.Stats.Type, unit.Stats.SelectedStone - 1]);
            int range = unit.Stats.Range + boost;

            for (int y = Math.Max(unit.Y - range, 0); y <= Math.Min(unit.Y + range, GameLogic.Map.Height - 1); y++)
                for (int x = Math.Max(unit.X - range, 0); x <= Math.Min(unit.X + range, GameLogic.Map.Width - 1); x++)
                    if ((!GameLogic.Map.IsProjectileResistant(x, y) || GameLogic.Map.IsObjectOnTile(x, y)) &&
                        Map.Pathfinding.AStarSearch.Heuristic(new Int2(unit.X, unit.Y), new Int2(x, y)) <= range &&
                            GameLogic.Map.ProjectileRayCast(new Vector2(unit.X + 0.5f, unit.Y + 0.5f), new Vector2(x + 0.5f, y + 0.5f)))
                        attackTiles.Add(new Int2(x, y));
            return attackTiles;
        }
    }
}
