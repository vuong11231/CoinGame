//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class OneSignalManager : MonoBehaviour
//{
//    void Start()
//    {
//        // Enable line below to enable logging if you are having issues setting up OneSignal. (logLevel, visualLogLevel)
//        // OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.INFO, OneSignal.LOG_LEVEL.INFO);

//        OneSignal.StartInit("c622d402-55b5-4390-b29a-c97fc03b6c16")
//          .HandleNotificationOpened(HandleNotificationOpened)
//          .EndInit();

//        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
//    }

//    // Gets called when the player opens the notification.
//    private static void HandleNotificationOpened(OSNotificationOpenedResult result)
//    {
//        // Analaytics
//        LeanTween.delayedCall(1f, () =>
//        {
//            AnalyticsManager.Instance.TrackNotificationOpened(false);
//        });
//    }
//}
