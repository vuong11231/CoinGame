using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace SteveRogers
{
    public class PointerDownHandler : MonoBehaviour, IPointerDownHandler
    {
        private readonly static float DISTANCE = 0.15f;

        private Dictionary<int, Action> onClicks = new Dictionary<int, Action>();
        private int count = 0;
        private float lastClickTick = 0;

        private void Update()
        {
            if (count > 0)
            {
                if (Time.realtimeSinceStartup - lastClickTick > DISTANCE)
                {
                    Action act = null;

                    if (onClicks.TryGetValue(count, out act))
                        act();

                    count = 0;
                }
            }
        }

        public void OnPointerDown(PointerEventData pointerEventData)
        {
            count++;
            lastClickTick = Time.realtimeSinceStartup;
            Action act = null;

            if (onClicks.TryGetValue(0, out act))
                act();
        }

        public void AddAction(int clickNumber_s, Action action_s)
        {
            if (clickNumber_s < 0 || action_s == null)
                return;

            if (onClicks.ContainsKey(clickNumber_s))
                onClicks[clickNumber_s] += action_s;
            else
                onClicks[clickNumber_s] = action_s;
        }

        public Action GetAction(int clickNumber_s)
        {
            if (onClicks == null)
                return null;

            return onClicks.TryGetFromDictionary(clickNumber_s);
        }
    }
}