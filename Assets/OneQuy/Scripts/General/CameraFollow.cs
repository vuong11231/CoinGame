using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteveRogers
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform followObject = null;

        [SerializeField]
        private Vector3 followOffset;

        [SerializeField]
        private bool setOffsetAtStartup = true;

        [SerializeField]
        private float speed = 3f;

        [SerializeField]
        private bool followX = true;

        [SerializeField]
        private bool followY = true;

        [SerializeField]
        private bool followZ = true;

        private void Awake()
        {
            if (setOffsetAtStartup)
                SetOffset();
        }

        public void SetOffset()
        {
            if (followObject == null)
                return;
            
            followOffset = transform.position - followObject.transform.position;
        }

        public void SetOffset(Vector3 vector3)
        {
            followOffset = vector3;
        }

        private void FixedUpdate()
        {
            if (followObject == null)
                return;

            var xTarget = followObject.position.x + followOffset.x;
            var yTarget = followObject.position.y + followOffset.y;
            var zTarget = followObject.position.z + followOffset.z;

            float x = transform.position.x, y = transform.position.y, z = transform.position.z;

            if (followX && xTarget != transform.position.x)
            {
                x = Mathf.Lerp(transform.position.x, xTarget, speed * Time.deltaTime);
            }

            if (followY && yTarget != transform.position.y)
            {
                y = Mathf.Lerp(transform.position.y, yTarget, speed * Time.deltaTime);
            }

            if (followZ && zTarget != transform.position.z)
            {
                z = Mathf.Lerp(transform.position.z, zTarget, speed * Time.deltaTime);
            }

            transform.position = new Vector3(x, y, z);
        }

#if UNITY_EDITOR
        [ContextMenu("Update Offset")]
        private void UpdateOffset()
        {
            SetOffset();
        }
#endif
    }
}