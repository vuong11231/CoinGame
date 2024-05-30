using System;
using System.Collections;
using UnityEngine;

namespace SteveRogers
{
    public class ValueFire<T>
    {
        public delegate bool OnChanged(T value);

        private T _Value;
        private OnChanged onChanged = null;
        private Coroutine updateCRT = null;

        public ValueFire(OnChanged onChanged)
        {
            this.onChanged = onChanged;
        }

        public T Value
        {
            get
            {
                return _Value;
            }

            set
            {
                _Value = value;

                if (!onChanged(value) && updateCRT == null)
                {
                    updateCRT = MonoManager.RunCoroutine(Update_CRT());
                }
            }
        }

        private IEnumerator Update_CRT()
        {
            while (!onChanged(_Value))
            {
                yield return null;
            }
            
            updateCRT = null;
            yield break;
        }
    }
}