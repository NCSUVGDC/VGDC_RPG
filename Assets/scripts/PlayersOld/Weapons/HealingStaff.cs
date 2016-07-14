//using System.Collections.Generic;
//using VGDC_RPG.Items;

//namespace VGDC_RPG.Players.Weapons
//{
//    public class HealingStaff : Item, Weapon
//    {
//        public override string Category
//        {
//            get
//            {
//                return "Weapon";
//            }
//        }

//        public override string GUIName
//        {
//            get
//            {
//                return "Staff";
//            }
//        }

//        public override PlayerEffect UseEffect
//        {
//            get
//            {
//                return new PlayerEffect(0, 0, 0, 0);
//            }
//        }

//        public bool DoesDamage { get { return false; } }

//        public override bool Consumable { get { return false; } }
//        public override bool RequiresAction { get { return false; } }

//        public bool Attack(Player me, Int2 tile)
//        {
//            var p = GameLogic.GetPlayerOnTile(tile.X, tile.Y);
//            if (p == null)
//                return false;
//            p.Heal(5);

//            me.TakingTurn = false;
//            GameLogic.Instance.NextAction();
//            return true;
//        }

//        public List<Int2> ComputeAttackTiles(Player me)
//        {
//            return GameLogic.Instance.Map.GetNeighbors(new Int2(me.X, me.Y));
//        }

//        public override void Use(Player useOn)
//        {
//            useOn.ActiveWeapon = this;
//        }
//    }
//}
