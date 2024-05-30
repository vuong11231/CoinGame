//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AppsFlyerManager
//{
//    public void Init(string DevKey, string iOS_AppID, string Android_AppID)
//    {
//        /* Mandatory - set your AppsFlyer’s Developer key. */
//        AppsFlyer.setAppsFlyerKey(DevKey);

//        /* For detailed logging */
//        //AppsFlyer.setIsDebug (true);

//#if UNITY_IOS
//          /* Mandatory - set your apple app ID
//           NOTE: You should enter the number only and not the "ID" prefix */
//          AppsFlyer.setAppID (iOS_AppID);
//          AppsFlyer.trackAppLaunch ();
//#elif UNITY_ANDROID
//        /* Mandatory - set your Android package name */
//        AppsFlyer.setAppID(Android_AppID);
//        /* For getting the conversion data in Android, you need to add the "AppsFlyerTrackerCallbacks" listener.*/
//        AppsFlyer.init(DevKey, "AppsFlyerTrackerCallbacks");
//#endif
//    }
//}
