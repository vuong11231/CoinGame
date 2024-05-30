using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;
//using System;
//using System.Collections.Generic;

namespace SteveRogers
{
    public class Twinkle : MonoBehaviour
    {
        [SerializeField]
        private float interval = 0.1f;

        private float cur = 0;
        private Canvas canvas = null;

        private void Start()
        {
            canvas = GetComponent<Canvas>();

            if (canvas == null)
                throw new System.Exception("this Twinkle go can't found the canvas component: " + gameObject.name);
        }

        private void Update()
        {
            cur += Time.deltaTime;

            if (cur > interval)
            {
                cur = 0;
                canvas.enabled = !canvas.enabled;
            }
        }
    }
}