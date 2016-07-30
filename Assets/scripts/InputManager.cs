using UnityEngine;

namespace VGDC_RPG
{
    public static class InputManager
    {
        public static bool MouseDown { get; private set; }
        public static bool MouseUp { get; private set; }
        public static bool MousePressed { get; private set; }

        public static bool DragDown { get; private set; }
        public static bool DragUp { get; private set; }
        public static bool DragPressed { get; private set; }

        public static float MouseX { get; private set; }
        public static float MouseY { get; private set; }

        public static Vector2 MousePosition
        {
            get
            {
                return new Vector2(MouseX, MouseY);
            }
        }

        private static bool tps = false;
        public static void Update()
        {
            if (Input.touchSupported == true)
            {
                if (Input.touchCount != 0)
                {
                    MouseDown = Input.GetTouch(0).phase == TouchPhase.Ended && Input.GetTouch(0).tapCount == 1 && !DragPressed;
                    DragDown = Input.GetTouch(0).phase == TouchPhase.Moved && tps;
                    DragPressed = Input.GetTouch(0).phase == TouchPhase.Moved;
                    MouseX = Input.GetTouch(0).position.x;
                    MouseY = Input.GetTouch(0).position.y;
                    
                    tps = Input.GetTouch(0).phase == TouchPhase.Began;
                }
                else
                {
                    MouseDown = false;
                    DragPressed = false;
                    DragDown = false;

                    tps = false;
                }
            }
            else
            {
                MouseDown = (GameLogic.Map == null || !GameLogic.Map.EditMode) && Input.GetMouseButtonDown(0);
                MouseUp = (GameLogic.Map == null || !GameLogic.Map.EditMode) && Input.GetMouseButtonUp(0);
                MousePressed = (GameLogic.Map == null || !GameLogic.Map.EditMode) && Input.GetMouseButton(0);

                DragDown = Input.GetMouseButtonDown(1);
                DragUp = Input.GetMouseButtonUp(1);
                DragPressed = Input.GetMouseButton(1);

                MouseX = Input.mousePosition.x;
                MouseY = Input.mousePosition.y;
            }
        }
    }
}
