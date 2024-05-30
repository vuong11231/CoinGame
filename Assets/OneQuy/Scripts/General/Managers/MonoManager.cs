using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SteveRogers
{
    public class MonoManager : SingletonPersistentStatic<MonoManager>
    {
        #region Variable

        private static Action onDestroy = null;
        private static Action<bool> onPause = null;

        private bool isPausing = false;

        #endregion

        #region Core (Private)

        private void OnDestroy()
        {
            onDestroy.SafeCall(); ;
        }

        private void OnApplicationFocus(bool notPause)
        {
            if (isPausing == !notPause)
                return;

            isPausing = !notPause;
            onPause.SafeCall(!notPause);
        }

        private void OnApplicationPause(bool pause)
        {
            if (isPausing == pause)
                return;

            isPausing = pause;
            onPause.SafeCall(pause);
        }

        #endregion

        #region Utils (Public)

        public static event Action DoOnDestroy
        {
            add
            {
                onDestroy = (Action)Delegate.Combine(onDestroy, value);
            }

            remove
            {
                onDestroy = (Action)Delegate.Remove(onDestroy, value);
            }
        }

        public static event Action<bool> DoOnPause
        {
            add
            {
                onPause = (Action<bool>)Delegate.Combine(onPause, value);
            }

            remove
            {
                onPause = (Action<bool>)Delegate.Remove(onPause, value);
            }
        }

        public static Coroutine RunCoroutine(IEnumerator enumerator)
        {
            CheckInit();
            return Instance.StartCoroutine(enumerator);
        }

        #endregion

        #region UpdateInterval

        public class UpdateInterval
        {
            private float current = 0;
            private float time;
            private Action action;

            public UpdateInterval(float _time, Action _action)
            {
                if (_action == null)
                    throw new Exception("UpdateInterval: param action is null");

                action = _action;
                time = _time;
            }

            public void OnUpdate(float dt)
            {
                current += dt;

                if (current >= time)
                {
                    current = 0;
                    action();
                }
            }
        }

        private static System.Collections.Generic.List<UpdateInterval> updateIntervals = null;

        public static void RemoveUpdate(UpdateInterval item)
        {
            updateIntervals.Remove(item);
        }

        public static UpdateInterval RegisterUpdate(float time, Action action)
        {
            if (updateIntervals == null)
                updateIntervals = new System.Collections.Generic.List<UpdateInterval>();

            var neww = new UpdateInterval(time, action);
            updateIntervals.Add(neww);
            return neww;
        }

        private void Update()
        {
            if (updateIntervals == null)
                return;

            var dt = Time.deltaTime;

            foreach (var u in updateIntervals)
                u.OnUpdate(dt);
        }

        #endregion

        #region Coroutines

        private static SafeList<CoroutineTag> registerCoroutines = null;

        private struct CoroutineTag
        {
            public Coroutine coroutine;
            public MonoBehaviour monoBehaviour;
            public string tag;
        }

        public static void StopCoroutineWithManager(string tag)
        {
            if (registerCoroutines == null)
                return;

            bool has_stop = false;

            foreach (var c in registerCoroutines)
            {
                if (c.tag.Equals(tag))
                {
                    has_stop = true;

                    if (c.monoBehaviour)
                    {
                        c.monoBehaviour.StopCoroutine(c.coroutine);
                    }
                }
            }

            if (has_stop)
                registerCoroutines.RemoveAll(c => c.tag.Equals(tag));
        }

        /// <summary>
        /// if this tag is running, it will be stopped and replay
        /// </summary>
        public static void StartCoroutineWithManager(IEnumerator enumerator, string tag, MonoBehaviour mono = null)
        {
            if (enumerator == null || tag == null || Instance == null)
                throw new ArgumentNullException();

            if (registerCoroutines != null)
                StopCoroutineWithManager(tag);
            else
                registerCoroutines = new SafeList<CoroutineTag>();

            MonoBehaviour runner = (mono == null ? Instance : mono);
            var c = runner.StartCoroutine(enumerator);

            registerCoroutines.Add(new CoroutineTag
            {
                tag = tag,
                coroutine = c,
                monoBehaviour = runner
            });
        }

        #endregion
    }
}