using UnityEngine;
using System;

namespace SteveRogers
{
    [DisallowMultipleComponent]
    public class ScreenSizeManager : SingletonPersistent<ScreenSizeManager>
    {
        #region Core (Private)

        [SerializeField]
        private bool destroyAfterFirstChange = true;

        private int currentWeight = 0;
        private int currentHeight = 0;
        private static Action<double> onChanged = null;

        public void Dispose() 
        {
            onChanged = null;
        }

        private void Update()
        {
            if (Screen.width != currentWeight || Screen.height != currentHeight)
            {
                currentWeight = Screen.width;
                currentHeight = Screen.height;
                Ratio = Math.Round(Screen.width * 1.0f / Screen.height, 2);
                onChanged?.Invoke(Ratio);

                if (destroyAfterFirstChange)
                {
                    onChanged = null;
                    Destroy(this);
                }
            }
        }

        #endregion

        #region Utils (Public)

        public static event Action<double> OnChanged
        {
            add
            {
                onChanged += (Action<double>)Delegate.Combine(onChanged, value);
            }

            remove
            {
                onChanged += (Action<double>)Delegate.Remove(onChanged, value);
            }
        }

        public static double Ratio { get; private set; } = Math.Round(Screen.width * 1.0f / Screen.height, 2);

        #endregion
    }
}