using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VGDC_RPG.Projectiles
{
    public class BombFragment : Arrow
    {
        public GameObject TrailPrefab;
        private float ti;
        public float TrailSpawnRate = 0.25f;

        public override void Update()
        {
            if (!GameLogic.Instance.DoPlayerUpdates)
                return;

            ti += Time.deltaTime;
            if (ti > TrailSpawnRate)
            {
                ti = 0;
                GameObject.Instantiate(TrailPrefab, transform.position, transform.rotation);
            }

            base.Update();
        }
    }
}
