using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using VGDC_RPG.Networking;

namespace Assets.Editor
{
    public class HandlerListWindow : EditorWindow
    {
        private Vector2 scrollPos;

        [MenuItem("Window/Hander List")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            HandlerListWindow window = (HandlerListWindow)EditorWindow.GetWindow(typeof(HandlerListWindow));
            window.Show();
        }

        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            if (NetEvents.handlers != null)
                foreach (var kp in NetEvents.handlers)
                    EditorGUILayout.LabelField(kp.Key + ": " + kp.Value, EditorStyles.miniLabel);
            EditorGUILayout.EndScrollView();
        }
    }
}
