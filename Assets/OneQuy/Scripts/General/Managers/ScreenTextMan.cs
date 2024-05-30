using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SteveRogers
{
    [DisallowMultipleComponent]
    public class ScreenTextMan : SingletonPersistentStatic<ScreenTextMan>
    {
        public static string Text
        {
            set; get;
        }

        public static string AppendNewLine
        {
            set
            {
                Text = string.Format("{0}\n{1}", Text, value);
            }
        }

        public static bool IsShowing
        {
            set; get;
        } = false;

        private const float percentHeightScrollView = 95f;
        private const int numButton = 3;

        private Vector2 scrollPosition;
        private GUILayoutOption guiLayoutOption_W = null;
        private GUILayoutOption guiLayoutOption_H = null;
        private GUIStyle style = null;
        private Rect rect_Copy;
        private Rect rect_Clear;
        private Rect rect_Close;

        [SerializeField]
        private bool registerLogMessages = false;

        protected override void Awake()
        {
            base.Awake();

            if (Application.isEditor || !registerLogMessages)
                return;

            Application.logMessageReceived += (string condition, string stackTrace, LogType type) =>
            {
                if (type == LogType.Log || type == LogType.Warning)
                    return;

                AppendNewLine = $"{condition}\n{stackTrace}\n\n";
                IsShowing = true;
            };
        }

        private void Start()
        {
            guiLayoutOption_W = GUILayout.Width(Screen.width);
            guiLayoutOption_H = GUILayout.Height(Screen.height - Screen.height * (100 - percentHeightScrollView) / 100);
            rect_Copy = new Rect(0 * (Screen.width / numButton), Screen.height - Screen.height * (100 - percentHeightScrollView) / 100, Screen.width / numButton, Screen.height * (100 - percentHeightScrollView) / 100);
            rect_Clear = new Rect(1 * (Screen.width / numButton), Screen.height - Screen.height * (100 - percentHeightScrollView) / 100, Screen.width / numButton, Screen.height * (100 - percentHeightScrollView) / 100);
            rect_Close = new Rect(2 * (Screen.width / numButton), Screen.height - Screen.height * (100 - percentHeightScrollView) / 100, Screen.width / numButton, Screen.height * (100 - percentHeightScrollView) / 100);
        }

        private void OnGUI()
        {
            if (!IsShowing)
                return;

            bool isportrait = Screen.width < Screen.height;

            if (isportrait) // portrait
            {
                GUI.skin.verticalScrollbar.fixedWidth = Screen.width * 0.05f;
                GUI.skin.verticalScrollbarThumb.fixedWidth = Screen.width * 0.05f;
            }
            else // landscape
            {
                GUI.skin.verticalScrollbar.fixedWidth = Screen.width * 0.02f;
                GUI.skin.verticalScrollbarThumb.fixedWidth = Screen.width * 0.02f;
            }

            if (isportrait) // portrait
            {
                GUI.skin.horizontalScrollbar.fixedHeight = Screen.height * 0.02f;
                GUI.skin.horizontalScrollbarThumb.fixedHeight = Screen.height * 0.02f;
            }
            else // landscape
            {
                GUI.skin.horizontalScrollbar.fixedHeight = Screen.height * 0.05f;
                GUI.skin.horizontalScrollbarThumb.fixedHeight = Screen.height * 0.05f;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, guiLayoutOption_W, guiLayoutOption_H);

            if (style == null)
            {
                style = new GUIStyle(GUI.skin.box);
                style.fontSize = (int)(15f * Screen.height / 598);
                style.alignment = TextAnchor.UpperLeft;
            }

            GUILayout.Label(Text, style);
            GUILayout.EndScrollView();

            // copy

            if (GUI.Button(rect_Copy, "COPY"))
            {
                Utilities.Clipboard = Text;
            }

            // clear

            if (GUI.Button(rect_Clear, "CLEAR"))
            {
                Text = string.Empty;
            }

            // close

            if (GUI.Button(rect_Close, "X"))
            {
                IsShowing = false;
            }
        }
    }
}
