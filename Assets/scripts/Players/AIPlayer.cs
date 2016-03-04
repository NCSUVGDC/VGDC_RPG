using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VGDC_RPG.Players
{
    public class AIPlayer : Player
    {
        private Player target;

        public override void Turn()
        {
            base.Turn();

            UpdateTarget();

            if (target != null && (Math.Abs(X - target.X) <= 1 && Math.Abs(Y - target.Y) <= 1))
                Attack(target);
            else
            {
                List<Int2> path = null;
                if (target != null)
                    path = Map.PathFinder.FindPathBeside(GameLogic.Instance.Map, new Int2(X, Y), new Int2(target.X, target.Y));
                if (path != null)
                {
                    Debug.Log(path.Count);
                    for (int i = path.Count - 1; i >= 0; i--)
                        if (possibleTiles.Contains(path[i]))
                        {
                            Debug.Log(i);
                            var nr = path.GetRange(0, i + 1);
                            Move(nr);
                            Debug.Log("Moving AI.");
                            return;
                        }
                }
            }

            TakingTurn = false;
            GameLogic.Instance.NextTurn();
        }

        private void Attack(Player target)
        {
            target.Kill();
        }

        private void UpdateTarget()
        {
            if (GameLogic.Instance.UserPlayers.Count == 0)
                target = null;

            float sd = float.MaxValue;
            Player nt = null;
            foreach (var p in GameLogic.Instance.UserPlayers)
            {
                var dx = X - p.X;
                var dy = Y - p.Y;
                var td = dx * dx + dy * dy;
                if (td < sd)
                {
                    sd = td;
                    nt = p;
                }
            }

            target = nt;
        }
    }
}
