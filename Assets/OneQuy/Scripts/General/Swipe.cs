using System;
using System.Collections.Generic;
using UnityEngine;

namespace SteveRogers
{
    public class Swipe : MonoBehaviour
    {
        private Vector3 mouseDownPos;
        private bool tracking = true;
        public float limit = 20;
        
        public static Direction GetDirection(Vector3 screenStartPos, Vector3 screenEndPos, float limit)
        {
            var y = Math.Abs(screenEndPos.y - screenStartPos.y);
            var x = Math.Abs(screenEndPos.x - screenStartPos.x);
            
            if (x < y) // vertical
            {
                if (y < limit)
                    return Direction.None;

                if (screenEndPos.y > screenStartPos.y)
                {
                    return Direction.Up;
                }
                else if (screenEndPos.y < screenStartPos.y)
                {
                    return (Direction.Down);
                }
            }
            else // horizontal
            {
                if (x < limit)
                    return Direction.None;
                
                if (screenEndPos.x < screenStartPos.x)
                {
                    return (Direction.Left);
                }
                else if (screenEndPos.x > screenStartPos.x)
                {
                    return (Direction.Right);
                }
            }
            
            return Direction.None;
        }

        private void Update()
        {
            if (!IsActive)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPos = Input.mousePosition;
                tracking = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!tracking)
                    return;

                tracking = false;

                var dir = GetDirection(mouseDownPos, Input.mousePosition, limit);

                if (dir != Direction.None)
                    OnSwipe.SafeCall(dir);
            }
        }

        public Action<Direction> OnSwipe { get; set; }
        public bool _IsActive = true;

        public bool IsActive {
            private get {
                return _IsActive;
            }

            set {
                _IsActive = value;
                tracking = false;
            }
        } 
    }
}