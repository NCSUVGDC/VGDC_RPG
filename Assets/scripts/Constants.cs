//using VGDC_RPG.Players;

using UnityEngine;
using VGDC_RPG.Units;

namespace VGDC_RPG
{
    public static class Constants
    {
        public const int ATLAS_SIZE = 8;
        public const uint NET_VERSION = 1;

        public static int GetDamage(Unit o, Unit p, Vector2 tpos, float splashRange)
        {
            splashRange++;
            if (GameLogic.Map.ProjectileRayCast(new Vector2(tpos.x, tpos.y), new Vector2(p.X + 0.5f, p.Y + 0.5f)))
            {
                var dist = Mathf.Sqrt(Vector2.SqrMagnitude(new Vector2(tpos.x - p.X - 0.5f, tpos.y - p.Y - 0.5f)));
                var dmg = Mathf.CeilToInt((1 / (dist + 1) - 1 / (splashRange + 1)) / (1 - 1 / (splashRange + 1)) * 40);
                return dmg;
            }
            return 0;
        }

        public static float GetPDamage(Int2 p, Vector2 tpos, float splashRange)
        {
            splashRange++;
            if (GameLogic.Map.ProjectileRayCast(new Vector2(tpos.x, tpos.y), new Vector2(p.X + 0.5f, p.Y + 0.5f)))
            {
                var dist = Mathf.Sqrt(Vector2.SqrMagnitude(new Vector2(tpos.x - p.X - 0.5f, tpos.y - p.Y - 0.5f)));
                var dmg = ((1 / (dist + 1) - 1 / (splashRange + 1)) / (1 - 1 / (splashRange + 1)));
                Debug.Log("Splash damage: " + dmg);
                return dmg;
            }
            return 0;
        }
    }
}
