using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SteveRogers
{
    public class PointerHoverHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Action onPointerDown = null;
        private Action onPointerUp = null;
        private bool downing = false;
        private Vector3 downPos;
        
        public float limit = 20;

        public event Action DoOnPointerDown
        {
            add
            {
                onPointerDown = (Action)Delegate.Combine(onPointerDown, value);
            }

            remove
            {
                onPointerDown = (Action)Delegate.Remove(onPointerDown, value);
            }
        }
        public event Action DoOnPointerUp
        {
            add
            {
                onPointerUp = (Action)Delegate.Combine(onPointerUp, value);
            }

            remove
            {
                onPointerUp = (Action)Delegate.Remove(onPointerUp, value);
            }
        }
        
        public Action<Direction> OnSwipe { get; set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown.SafeCall();
            downing = true;
            downPos = eventData.position;
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp.SafeCall();

            if (downing)
            {
                downing = false;
                var dir = Swipe.GetDirection(downPos, eventData.position, limit);
                OnSwipe.SafeCall(dir);
            }
        }
    }
}
