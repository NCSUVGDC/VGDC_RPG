using UnityEngine;

namespace VGDC_RPG.Projectiles
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class Bomb : Arrow
    {
        public const int BOUNCES = 3;
        public float SplashRange = 3;

        public float LowScale = 0.5f;
        public float HighScale = 1.0f;

        // Use this for initialization
        void Start()
        {
            GetComponent<MeshRenderer>().material.mainTexture = Texture;
        }

        // Update is called once per frame
        public override void Update()
        {
            if (lv >= 1)
            {
                Destroy(gameObject);
                //if (Owner != null && Target != null)
                //    Owner.Attack(Target);
                for (int i = 0; i < GameLogic.Instance.TeamCount; i++)
                {
                    if (i == Owner.TeamID)
                        continue;
                    var nl = GameLogic.Instance.Players[i].ToArray();
                    foreach (var p in nl)
                        DoDamage(Owner, p);
                }
                Owner.TakingTurn = false;
                GameLogic.Instance.NextTurn();
            }
            transform.rotation = Quaternion.Euler(90, Time.time * 90, 0);//Quaternion.Euler(90, Mathf.Rad2Deg * -Mathf.Atan2(TargetPosition.z - StartPosition.z, TargetPosition.x - StartPosition.x) + 90, 0);//Quaternion.FromToRotation(StartPosition, TargetPosition);// * Quaternion.Euler(90, 0, 0);
            transform.position = Vector3.Lerp(StartPosition, TargetPosition, lv);
            var sf = Mathf.Lerp(LowScale, HighScale, Mathf.Abs(Mathf.Sin(lv * Mathf.PI * BOUNCES)) * (1 - lv));
            transform.localScale = new Vector3(sf, sf, sf);
            lv += Speed / Vector3.Distance(StartPosition, TargetPosition) * Time.deltaTime;
        }

        private void DoDamage(Players.Player o, Players.Player p)
        {
            if (GameLogic.Instance.Map.ProjectileRayCast(new Vector2(TargetPosition.x, TargetPosition.z), new Vector2(p.X + 0.5f, p.Y + 0.5f)))
            {
                var dist = Vector2.SqrMagnitude(new Vector2(TargetPosition.x - p.X - 0.5f, TargetPosition.z - p.Y - 0.5f));
                var dmg = Mathf.CeilToInt((1 / (dist + 1) - 1 / (SplashRange * SplashRange + 1)) / (1 - 1 / (SplashRange * SplashRange + 1)) * o.AttackDamage);
                if (dmg > 0)
                    o.Attack(p, dmg);
            }
        }
    }
}
