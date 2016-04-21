using System;
using System.Collections.Generic;
using UnityEngine;
using VGDC_RPG.Items;

namespace VGDC_RPG.Players.Weapons
{
    public class Bow : Item, Weapon
    {
        public override string Category
        {
            get
            {
                return "Weapon";
            }
        }

        public override string GUIName
        {
            get
            {
                return "Bow";
            }
        }

        public override PlayerEffect UseEffect
        {
            get
            {
                return new PlayerEffect(0, 0, 0, 0);
            }
        }

        public bool DoesDamage { get { return true; } }

        public override bool Consumable { get { return false; } }
        public override bool RequiresAction { get { return false; } }

        public virtual GameObject Arrow { get { return Resources.Load<GameObject>("Projectiles/ArrowProjectile"); } }

        public bool Attack(Player me, Int2 tile)
        {
            me.SpawnProjectile(Arrow, tile);

            me.TakingTurn = false;
            GameLogic.Instance.NextAction();
            return true;
        }

        public virtual List<Int2> ComputeAttackTiles(Player me)
        {
            //return GameLogic.Instance.Map.GetNeighbors(new Int2(me.X, me.Y));
            List<Int2> attackTiles = new List<Int2>();
            for (int y = Math.Max(me.Y - me.Range, 0); y <= Math.Min(me.Y + me.Range, GameLogic.Instance.Map.Height - 1); y++)
                for (int x = Math.Max(me.X - me.Range, 0); x <= Math.Min(me.X + me.Range, GameLogic.Instance.Map.Width - 1); x++)
                    if ((!GameLogic.Instance.Map.IsProjectileResistant(x, y) || GameLogic.Instance.Map.IsObjectOnTile(x, y)) &&
                        Map.Pathfinding.AStarSearch.Heuristic(new Int2(me.X, me.Y), new Int2(x, y)) <= me.Range &&
                            GameLogic.Instance.Map.ProjectileRayCast(new Vector2(me.X + 0.5f, me.Y + 0.5f), new Vector2(x + 0.5f, y + 0.5f)))
                        attackTiles.Add(new Int2(x, y));
            return attackTiles;
        }

        public override void Use(Player useOn)
        {
            useOn.ActiveWeapon = this;
        }
    }
}
