using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace SteveRogers
{
    public class NetMan : SingletonPersistentStatic<NetMan>
    {
        // inspector

        [SerializeField, Range(0, 5)]
        private float interval = 1;

        [SerializeField]
        private string host = @"https://time.com/";

        [SerializeField]
        private bool warningWhenOffline = true;

        // core

        private static bool _IsOnline = false;
        private static bool setDate = false;
        private static UnityWebRequest www = null;
        private static Stopwatch stopwatch = null;
        private static double totalResponseTime = 0;
        private static int totalRequest = 0;
        private static GUIStyle warningStyle = null;
        private static Rect warningRect;

        #region utils (public)

        public static bool IsReady { get; private set; } = false;

        public static bool IsOfflinee
        {
            get
            {
                return !IsOnlinee;
            }
        }

        public static bool IsOnlinee
        {
            get
            {
                CheckInit();

                if (!IsReady)
                    throw new Exception("need to check NetMan.IsReady before this!");

                return _IsOnline;
            }

            private set
            {
                CheckInit();
                _IsOnline = value;
            }
        }

        public static DateTime Datee { get; private set; } = DateTime.MinValue; // IsReady?

        public static double AvgRequestTimee
        {
            get
            {
                if (!IsReady)
                    throw new Exception("need to check NetMan.IsReady before this!");

                if (totalRequest == 0)
                    return double.MinValue;
                else
                    return totalResponseTime / totalRequest;
            }
        }

        #endregion

        #region core (private)

        private void Start()
        {
            stopwatch = new Stopwatch();
            StartCoroutine(Check_CRT());
        }

        private IEnumerator Check_CRT()
        {
            // new request

            if (DebugMan.Log_NetMan)
                Debug.Log("NetMan is checking...");

            stopwatch.Restart();

            // wait

            www = new UnityWebRequest(host);
            yield return www.SendWebRequest();

            // get result

            if (!setDate)
            {
                var headdate = www.GetResponseHeader("Date");
                Datee = headdate.Parse(DateTime.MinValue);

                if (Datee > DateTime.MinValue) // valid date
                {
                    setDate = true;
                }
            }

            stopwatch.Stop();
            totalRequest++;
            var responeTime = stopwatch.Elapsed.TotalSeconds;
            totalResponseTime += responeTime;

            if (www.isHttpError || www.isNetworkError)
                IsOnlinee = false;
            else
                IsOnlinee = true;

            IsReady = true;

            if (DebugMan.Log_NetMan)
            {
                Debug.Log(Utilities.CreateLogContent(
                    "NetMan result: " + (IsOnlinee ? 1 : 0),
                    $"respone time: {responeTime}s",
                    $"avg respone time: {AvgRequestTimee}s"));
            }

            // loop

            www.Dispose();
            var delay = interval - (float)responeTime;

            if (delay > 0)
            {
                if (DebugMan.Log_NetMan)
                    Debug.Log($"NetMan delays for next request: {delay}s");

                yield return new WaitForSeconds(delay);
            }

            StartCoroutine(Check_CRT());
        }

        private void OnGUI()
        {
            if (IsReady && Instance.warningWhenOffline && IsOfflinee)
            {
                if (warningStyle == null)
                {
                    int h = 598;
                    warningStyle = Utilities.CreateSimpleBackgroundBox(Utilities.SetAlpha(Color.red, 0.7f));
                    warningStyle.fontSize = (int)(15f * Screen.height / h);
                    warningRect = new Rect(0, (50f * Screen.height / h), Screen.width, (20f * Screen.height / h));
                }

                GUI.Box(warningRect, "Internet Not Available!", warningStyle);
            }
        }

        #endregion
    }
}