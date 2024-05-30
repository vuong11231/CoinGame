using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SteveRogers
{
    public class TimescaleSetter : MonoBehaviour
    {
#if UNITY_EDITOR
        void Update()
        {
            if (!MenuItem_Debug_Timescale.IsTrue(false))
                return;

            var numpressed = Utilities.IsPressed_Num();

            if (numpressed > -1)
            {
                if (Input.GetKey(KeyCode.RightControl))
                    Time.timeScale = numpressed / 10f;
                else
                    Time.timeScale = numpressed;

                Utilities.WarningDone("Timescale: " + Time.timeScale);
            }
        }
#endif
    }


    #region debug

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class MenuItem_Debug_Timescale // CHANGE THIS (1/3)
    {
        public static bool IsTrue(bool valueIfNotEditor)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetBool(MENU_NAME, false);
#else
        return valueIfNotEditor;
#endif
        }

#if UNITY_EDITOR

        private static bool enabled_;

        private const string MENU_NAME = "Helper/Timescale Setter"; // CHANGE THIS (2/3)

        /// Called on load thanks to the InitializeOnLoad attribute
        static MenuItem_Debug_Timescale() // CHANGE THIS (3/3)
        {
            enabled_ = EditorPrefs.GetBool(MENU_NAME, false);

            /// Delaying until first editor tick so that the menu
            /// will be populated before setting check state, and
            /// re-apply correct action
            EditorApplication.delayCall += () =>
            {
                PerformAction(enabled_);
            };
        }

        [MenuItem(MENU_NAME)]
        private static void ToggleAction()
        {

            /// Toggling action
            PerformAction(!enabled_);
        }

        public static void PerformAction(bool enabled)
        {

            /// Set checkmark on menu item
            Menu.SetChecked(MENU_NAME, enabled);

            /// Saving editor state
            EditorPrefs.SetBool(MENU_NAME, enabled);
            enabled_ = enabled;
        }

#endif
    }

    #endregion
}