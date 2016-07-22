using UnityEngine;

namespace VGDC_RPG.TileObjects
{
    public class TileObject : MonoBehaviour
    {
        public bool Blocking = false;
        public int X, Y;

        // Use this for initialization
        void Start()
        {
            if (Blocking)
                GameLogic.Map.BlockTile(X, Y);
        }

        // Update is called once per frame
        public virtual void Update()
        {

        }

        void OnDestroy()
        {
            if (Blocking)
                GameLogic.Map.UnblockTile(X, Y);
        }

        public void SetPosition(int x, int y)
        {
            if (Blocking)
                GameLogic.Map.UnblockTile(X, Y);
            X = x;
            Y = y;
            if (Blocking)
                GameLogic.Map.BlockTile(X, Y);
        }
    }
}
