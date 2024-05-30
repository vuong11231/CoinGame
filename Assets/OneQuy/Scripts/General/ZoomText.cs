using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

namespace SteveRogers
{
    public class ZoomText
    {
        private bool setuped = false;
        private Text text = null;
        private int start = 0;
        private int step = 0;
        private float timeStep = 0;
        private string prefix = null;
        private string subfix = null;
        private Coroutine coroutine = null;
        private int originFontSize;
        private float scaleFactor;
        
        public int To
        {
            private set; get;
        }

        public ZoomText(Text text, float scaleFactor)
        {
            if (text == null)
                throw new Exception("text is null");

            this.text = text;
            originFontSize = text.fontSize;
            this.scaleFactor = scaleFactor;
        }

        public void Setup(int start, int step, float timeStep, string prefix = "", string subfix = "")
        {
            if (IsRunning)
            {
                Debug.LogError("is running, cant setup now");
                return;
            }

            setuped = false;
          
            this.start = start;
            this.step = step;
            this.timeStep = timeStep;
            this.prefix = prefix;
            this.subfix = subfix;
            this.text.text = CurrentText;

            setuped = true;
        }

        public bool IsRunning
        {
            get { return coroutine != null; }
        }

        public void Zoom(int to)
        {
            if (!setuped)
            {
                Debug.LogError("not setup yet");
                return;
            }

            if (to == start)
                return;

            if ((to > start && step <= 0) ||
               (to < start && step >= 0))
            {
                Debug.LogError("param invalids");
                return;
            }

            To = to;

            // check if running
            
            Stop();

            // run !

            coroutine = MonoManager.RunCoroutine(Run_CRT());
        }

        public void Stop()
        {
            if (coroutine.StopCoroutineSafe())
            {
                coroutine = null;
                text.fontSize = originFontSize;
            }
        }

        private string CurrentText
        {
            get { return string.Format("{0}{1}{2}", prefix, start, subfix); }
        }

        private IEnumerator Run_CRT()
        {
            text.fontSize = (int)(originFontSize * scaleFactor);

            while (start != To)
            {
                start += step;
                text.text = CurrentText;

                if (timeStep > 0)
                    yield return new WaitForSeconds(timeStep);
                else
                    yield return null;
            }

            text.fontSize = originFontSize;
            coroutine = null;
            yield break;
        }
    }
}