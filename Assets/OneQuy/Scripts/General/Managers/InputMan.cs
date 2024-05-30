using System.Collections;
using UnityEngine;
using System;

namespace SteveRogers
{
    public class InputMan : SingletonPersistentStatic<InputMan>
    {
        #region Variable

        private static Action onKeyDown_Escape = null;
        private static Action onAnyKeyDown = null;

        #endregion

        #region Core (Private)

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                onAnyKeyDown.SafeCall();
            else if (Input.anyKeyDown)
                onAnyKeyDown.SafeCall();

        }

        #endregion

        #region Utils (Public)
        
        public static event Action DoOnKeyDown_Escape
        {
            add
            {
                onKeyDown_Escape = (Action)Delegate.Combine(onKeyDown_Escape, value);
            }

            remove
            {
                onKeyDown_Escape = (Action)Delegate.Remove(onKeyDown_Escape, value);
            }
        }
        
        public static event Action DoOnAnyKeyDown
        {
            add
            {
                onAnyKeyDown = (Action)Delegate.Combine(onAnyKeyDown, value);
            }

            remove
            {
                onAnyKeyDown = (Action)Delegate.Remove(onAnyKeyDown, value);
            }
        }

        #endregion
    }
}