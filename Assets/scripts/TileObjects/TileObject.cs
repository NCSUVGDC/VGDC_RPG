using UnityEngine;

namespace VGDC_RPG.TileOjects
{
    public class TileObject : MonoBehaviour
    {
        public bool Blocking = false;
        public int X, Y;

        // Use this for initialization
        void Start()
        {
            if (Blocking)
                GameLogic.Instance.Map.BlockTile(X, Y);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDestroy()
        {
            if (Blocking)
                GameLogic.Instance.Map.UnblockTile(X, Y);
        }
    }
}
