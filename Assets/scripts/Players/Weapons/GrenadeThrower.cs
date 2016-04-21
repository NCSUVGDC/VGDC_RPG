using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VGDC_RPG.Players.Weapons
{
    public class GrenadeThrower : Bow
    {
        public override GameObject Arrow { get { return Resources.Load<GameObject>("Projectiles/BombProjectile"); } }

        public override string GUIName
        {
            get
            {
                return "Grenade";
            }
        }
    }
}
