using System;
using UnityEngine;

namespace SteveRogers
{
    public class CallInterval
    {
        private Action action = null;
        private float last = 0;
        private float interval = 1;
        private LeanTweenTag tag = LeanTweenTag._Count;

        public CallInterval(float interval, LeanTweenTag tag, Action action)
        {
            this.action = action ?? throw new Exception("The action is null!");
            Interval = interval;
            this.tag = tag;
        }

        public float Interval
        {
            private get
            {
                return interval;
            }

            set
            {
                if (value <= 0)
                    Debug.LogError("The interval wrong: " + value);
                else
                    interval = value;
            }
        }
        
        public void Call()
        {
            var dis = Time.realtimeSinceStartup - last;

            if (dis < Interval)
            {
                LeanTween.delayedCall(Interval - dis, Call)
                         .setTag(tag);
            }
            else
            {
                last = Time.realtimeSinceStartup;
                action();
            }
        }

        public void Reset()
        {
            last = 0;
        }
    }
}