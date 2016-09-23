using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VGDC_RPG.Units
{
    public class AIController
    {
        public byte PlayerID { get; private set; }
        public Unit Target { get; private set; }

        public AIController(byte id)
        {
            PlayerID = id;
            Debug.Log("Created AIController for ID: " + PlayerID);
        }

        public void Update()
        {
            if (GameLogic.CurrentPlayer != PlayerID
                || GameLogic.Units[PlayerID][GameLogic.CurrentUnitID].Sprite.IsMoving)
                return;

            var unit = GameLogic.Units[PlayerID][GameLogic.CurrentUnitID];

            if (Target == null || !Target.Stats.Alive)
                UpdateTarget();
            if (Target == null)
                return;

            if (!unit.HasAttacked && unit.Inventory.SelectedWeapon.GetAttackTiles(unit).Contains(new Int2(Target.X, Target.Y)))
            {
                unit.Inventory.SelectedWeapon.Attack(unit, new Int2(Target.X, Target.Y));
                unit.HasAttacked = true;
            }
            else if (!unit.HasMoved && !unit.HasAttacked)
            {
                List<Int2> path = null;
                if (Target != null)
                    path = Map.Pathfinding.AStarSearch.FindPathBeside(GameLogic.Map, new Int2(unit.X, unit.Y), new Int2(Target.X, Target.Y));
                if (path != null)
                    for (int i = path.Count - 1; i >= 0; i--)
                        if (unit.PossibleMovementTiles.Contains(path[i]))
                        {
                            unit.GoTo(path[i].X, path[i].Y);
                            break;
                        }
                unit.HasMoved = true;
            }
            else
                GameLogic.EndTurn();
        }

        public void StartTurn()
        {
            GameLogic.Units[PlayerID][GameLogic.CurrentUnitID].ComputePossibleMovementTiles();
        }

        private void UpdateTarget()
        {
            var unit = GameLogic.Units[PlayerID][GameLogic.CurrentUnitID];
            float sd = float.MaxValue;
            Unit nt = null;
            for (int i = 0; i < GameLogic.Units.Length; i++)
            {
                if (GameLogic.MatchInfo.PlayerInfos[i].Team == GameLogic.MatchInfo.PlayerInfos[PlayerID].Team)
                    continue;
                foreach (var p in GameLogic.Units[i])
                {
                    if (!p.Stats.Alive)
                        continue;
                    var dx = unit.X - p.X;
                    var dy = unit.Y - p.Y;
                    var td = Math.Abs(dx) + Math.Abs(dy);
                    if (td < sd)
                    {
                        sd = td;
                        nt = p;
                    }
                }
            }

            Target = nt;
        }
    }
}
