using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SteveRogers
{
    [DisallowMultipleComponent]
    public class InputController : MonoBehaviour
    {
        #region Variable

        private Dictionary<KeyCode, Action> onKeyUpActions = new Dictionary<KeyCode, Action>();

        #endregion

        #region Core (Private)

        private void OnGUI()
        {
            if (!Utilities.ActiveEventSystem)
                return;

            if (Event.current.type == EventType.KeyUp)
            {
                if (onKeyUpActions.TryGetValue(Event.current.keyCode, out Action act))
                    act();
                //else if (onKeyUpActions.TryGetValue(KeyCode.None, out act))
                //    act();
            }
        }

        #endregion

        #region Utils (Public)

        public void RegisterOnKeyUpAction(KeyCode key, Action act, bool is_register)
        {
            if (act == null)
                throw new Exception("cant register act null for key: " + key);

            if (onKeyUpActions.TryGetValue(key, out Action find)) // already has it
            {
                if (is_register)
                    onKeyUpActions[key] = (Action)Delegate.Combine(find, act);
                else
                    onKeyUpActions[key] = (Action)Delegate.Remove(find, act);
            }
            else // not has it yet
            {
                if (is_register)
                    onKeyUpActions[key] = act;
            }
        }

        #endregion
    }
}