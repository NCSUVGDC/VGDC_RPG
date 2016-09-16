using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VGDC_RPG.Units.Items
{
    public class BowWeapon : Weapon
    {
        public override bool Attack(Unit attacker, Int2 tile)
        {
            var target = GameLogic.GetUnitOnTile(tile);
            if (target != null)
                if (target.TeamID != attacker.TeamID)
                {
                    target.Damage(6);
                    return true;
                }
            Debug.Log("No target.");
            return false;
        }

        public override List<Int2> GetAttackTiles(Unit unit)
        {
            List<Int2> attackTiles = new List<Int2>();
            for (int y = Math.Max(unit.Y - unit.Stats.Range, 0); y <= Math.Min(unit.Y + unit.Stats.Range, GameLogic.Map.Height - 1); y++)
                for (int x = Math.Max(unit.X - unit.Stats.Range, 0); x <= Math.Min(unit.X + unit.Stats.Range, GameLogic.Map.Width - 1); x++)
                    if ((!GameLogic.Map.IsProjectileResistant(x, y) || GameLogic.Map.IsObjectOnTile(x, y)) &&
                        Map.Pathfinding.AStarSearch.Heuristic(new Int2(unit.X, unit.Y), new Int2(x, y)) <= unit.Stats.Range &&
                            GameLogic.Map.ProjectileRayCast(new Vector2(unit.X + 0.5f, unit.Y + 0.5f), new Vector2(x + 0.5f, y + 0.5f)))
                        attackTiles.Add(new Int2(x, y));
            return attackTiles;
        }
    }
}
