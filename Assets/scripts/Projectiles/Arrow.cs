using UnityEngine;
using System.Collections;

namespace VGDC_RPG.Projectiles
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class Arrow : MonoBehaviour
    {
        public Vector3 StartPosition;
        public Vector3 TargetPosition;
        public Players.Player Owner;
        public Players.Player Target;
        public float Speed = 2.0f;
        public Texture2D Texture;
        internal float lv = 0;

        // Use this for initialization
        void Start()
        {
            GetComponent<MeshRenderer>().material.mainTexture = Texture;
        }

        // Update is called once per frame
        public virtual void Update()
        {
            if (lv >= 1)
            {
                Destroy(gameObject);
                if (Owner != null && Target != null)
                    Owner.Attack(Target);
                Owner.TakingTurn = false;
                GameLogic.Instance.NextTurn();
            }
            transform.rotation = Quaternion.Euler(90, Mathf.Rad2Deg * -Mathf.Atan2(TargetPosition.z - StartPosition.z, TargetPosition.x - StartPosition.x) + 90, 0);//Quaternion.FromToRotation(StartPosition, TargetPosition);// * Quaternion.Euler(90, 0, 0);
            transform.position = Vector3.Lerp(StartPosition, TargetPosition, lv);
            lv += Speed / Vector3.Distance(StartPosition, TargetPosition) * Time.deltaTime;
        }
    }
}
