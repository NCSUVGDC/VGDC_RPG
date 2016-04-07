using UnityEngine;
using VGDC_RPG.Players;

namespace VGDC_RPG
{
    public static class Constants
    {
        public const int ATLAS_SIZE = 8;

        public static int GetDamage(Player o, Player p, Vector2 tpos, float splashRange)
        {
            splashRange++;
            if (GameLogic.Instance.Map.ProjectileRayCast(new Vector2(tpos.x, tpos.y), new Vector2(p.X + 0.5f, p.Y + 0.5f)))
            {
                var dist = Mathf.Sqrt(Vector2.SqrMagnitude(new Vector2(tpos.x - p.X - 0.5f, tpos.y - p.Y - 0.5f)));
                var dmg = Mathf.CeilToInt((1 / (dist + 1) - 1 / (splashRange + 1)) / (1 - 1 / (splashRange + 1)) * o.GetAttackDamage(p));
                return dmg;
            }
            return 0;
        }

        public static float GetPDamage(Int2 p, Vector2 tpos, float splashRange)
        {
            splashRange++;
            if (GameLogic.Instance.Map.ProjectileRayCast(new Vector2(tpos.x, tpos.y), new Vector2(p.X + 0.5f, p.Y + 0.5f)))
            {
                var dist = Mathf.Sqrt(Vector2.SqrMagnitude(new Vector2(tpos.x - p.X - 0.5f, tpos.y - p.Y - 0.5f)));
                var dmg = ((1 / (dist + 1) - 1 / (splashRange + 1)) / (1 - 1 / (splashRange + 1)));
                return dmg;
            }
            return 0;
        }
    }
}
