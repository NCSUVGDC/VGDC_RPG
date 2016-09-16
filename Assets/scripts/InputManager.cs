using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VGDC_RPG
{
    public static class InputManager
    {
        public delegate void ToggleEditModeEH(bool value);

        public static event ToggleEditModeEH ToggleEditMode;

        public static bool MouseDown { get; private set; }
        public static bool MouseUp { get; private set; }
        public static bool MousePressed { get; private set; }

        public static bool EditMouseDown { get; private set; }
        public static bool EditMouseUp { get; private set; }
        public static bool EditMousePressed { get; private set; }

        public static bool DragDown { get; private set; }
        public static bool DragUp { get; private set; }
        public static bool DragPressed { get; private set; }

        public static float MouseX { get; private set; }
        public static float MouseY { get; private set; }

        public static bool InEditMode { get; private set; }

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
                MouseX = Input.mousePosition.x;
                MouseY = Input.mousePosition.y;

                var pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;

                List<RaycastResult> rcr = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, rcr);

                //if (rcr.Count == 0)
                {
                    MouseDown = (GameLogic.Map == null || !InEditMode) && Input.GetMouseButtonDown(0);
                    MouseUp = (GameLogic.Map == null || !InEditMode) && Input.GetMouseButtonUp(0);
                    MousePressed = (GameLogic.Map == null || !InEditMode) && Input.GetMouseButton(0);

                    EditMouseDown = (GameLogic.Map == null || InEditMode) && Input.GetMouseButtonDown(0);
                    EditMouseUp = (GameLogic.Map == null || InEditMode) && Input.GetMouseButtonUp(0);
                    EditMousePressed = (GameLogic.Map == null || InEditMode) && Input.GetMouseButton(0);

                    DragDown = Input.GetMouseButtonDown(1);
                    DragUp = Input.GetMouseButtonUp(1);
                    DragPressed = Input.GetMouseButton(1);
                }

                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    InEditMode = !InEditMode;
                    if (ToggleEditMode != null)
                        ToggleEditMode(InEditMode);
                }
            }
        }
    }
}
