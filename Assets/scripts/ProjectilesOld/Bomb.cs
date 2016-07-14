//using UnityEngine;

//namespace VGDC_RPG.Projectiles
//{
//    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
//    public class Bomb : Arrow
//    {
//        public const int BOUNCES = 3;
//        public int SplashRange = 3;

//        public float LowScale = 0.5f;
//        public float HighScale = 1.0f;

//        public float RotPerSec = 0.25f;

//        public GameObject FragmentPrefab;
//        public GameObject WarpPrefab;

//        // Use this for initialization
//        void Start()
//        {
//            GetComponent<MeshRenderer>().material.mainTexture = Texture;
//        }

//        // Update is called once per frame
//        public override void Update()
//        {
//            if (!GameLogic.Instance.DoPlayerUpdates)
//                return;

//            if (lv >= 1)
//            {
//                Destroy(gameObject);
//                //if (Owner != null && Target != null)
//                //    Owner.Attack(Target);
//                /*for (int i = 0; i < GameLogic.Instance.TeamCount; i++)
//                {
//                    if (i == Owner.TeamID)
//                        continue;
//                    var nl = GameLogic.Instance.Players[i].ToArray();
//                    foreach (var p in nl)
//                        DoDamage(Owner, p);
//                }*/

//                Instantiate(WarpPrefab, TargetPosition, Quaternion.Euler(90, 0, 0));

//                for (int y = -SplashRange; y <= SplashRange; y++)
//                    for (int x = -SplashRange; x <= SplashRange; x++)
//                    {
//                        if (GameLogic.Instance.Map.ProjectileRayCast(new Vector2(TargetPosition.x, TargetPosition.z), new Vector2(TargetPosition.x + x, TargetPosition.z + y)))
//                        {
//                            var t = GameLogic.GetPlayerOnTile(x + Mathf.FloorToInt(TargetPosition.x), y + Mathf.FloorToInt(TargetPosition.z));
//                            var dmg = 0;
//                            if (t != null)
//                                dmg = Constants.GetDamage(Owner, t, new Vector2(TargetPosition.x, TargetPosition.z), SplashRange);
//                            else
//                                dmg = Mathf.CeilToInt(Constants.GetPDamage(new Int2(Mathf.FloorToInt(TargetPosition.x), Mathf.FloorToInt(TargetPosition.z)), new Vector2(TargetPosition.x + x, TargetPosition.z + y), SplashRange));
//                            if (dmg > 0)
//                            {
//                                var frag = Instantiate(FragmentPrefab).GetComponent<BombFragment>();
//                                frag.Damage = dmg;
//                                frag.StartPosition = TargetPosition;
//                                frag.TargetPosition = new Vector3(TargetPosition.x + x, TargetPosition.y, TargetPosition.z + y);
//                                frag.Owner = Owner;
//                                Owner.awaiting++;
//                            }
//                            //else
//                            //{
//                            //    var frag = GameObject.Instantiate(FragmentPrefab).GetComponent<BombFragment>();
//                            //    frag.Damage = 0;
//                            //    frag.StartPosition = TargetPosition;
//                            //    frag.TargetPosition = new Vector3(TargetPosition.x + x, TargetPosition.y, TargetPosition.z + y);
//                            //    frag.Owner = Owner;
//                            //    Owner.awaiting++;
//                            //}
//                        }
//                    }

//                GameLogic.Shake(0.5f);

//                Owner.TakingTurn = false;
//                Owner.awaiting--;
//                GameLogic.Instance.NextAction();
//            }
//            transform.rotation = Quaternion.Euler(90, Time.time * 360 * RotPerSec, 0);//Quaternion.Euler(90, Mathf.Rad2Deg * -Mathf.Atan2(TargetPosition.z - StartPosition.z, TargetPosition.x - StartPosition.x) + 90, 0);//Quaternion.FromToRotation(StartPosition, TargetPosition);// * Quaternion.Euler(90, 0, 0);
//            transform.position = Vector3.Lerp(StartPosition, TargetPosition, lv);
//            var sf = Mathf.Lerp(LowScale, HighScale, Mathf.Abs(Mathf.Sin(lv * Mathf.PI * BOUNCES)) * (1 - lv));
//            transform.localScale = new Vector3(sf, sf, sf);
//            lv += Speed / Vector3.Distance(StartPosition, TargetPosition) * Time.deltaTime;
//        }

//        /*private void DoDamage(Players.Player o, Players.Player p)
//        {
//            /*if (GameLogic.Instance.Map.ProjectileRayCast(new Vector2(TargetPosition.x, TargetPosition.z), new Vector2(p.X + 0.5f, p.Y + 0.5f)))
//            {
//                var dist = Vector2.SqrMagnitude(new Vector2(TargetPosition.x - p.X - 0.5f, TargetPosition.z - p.Y - 0.5f));
//                var dmg = Mathf.CeilToInt((1 / (dist + 1) - 1 / (SplashRange * SplashRange + 1)) / (1 - 1 / (SplashRange * SplashRange + 1)) * o.GetAttackDamage(p));
//                Debug.Log("Bomb Dmg: " + dmg + "@" + dist);
//                if (dmg > 0)
//                    o.Attack(p, dmg);
//            }
//            else
//                Debug.Log("NIS");*/



//            /*var dmg = Constants.GetDamage(o, p, new Vector2(TargetPosition.x, TargetPosition.z), SplashRange);
//            if (dmg > 0)
//                o.Attack(p, dmg);*/


//            /*var dmg = Constants.GetDamage(o, p, new Vector2(TargetPosition.x, TargetPosition.z), SplashRange);
//            if (dmg > 0)
//            {
//                var frag = GameObject.Instantiate(FragmentPrefab).GetComponent<BombFragment>();
//                frag.Damage = dmg;
//                frag.StartPosition = TargetPosition;
//                frag.TargetPosition = new Vector3(o.X + 0.5f, TargetPosition.y, o.Y + 0.5f);
//                frag.Owner = Owner;
//            }
//        }*/
//    }
//}
