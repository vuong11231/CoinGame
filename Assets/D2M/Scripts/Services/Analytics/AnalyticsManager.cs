using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : Singleton<AnalyticsManager>
{
    const string EVENT_WATCH_ADS_START = "watch_ads_start";
    const string EVENT_WATCH_ADS_DONE = "watch_ads_done";
    const string EVENT_TRANSFORM_PLANET = "transform_planet";
    const string EVENT_UPGRADE_PLANET = "upgrade_planet";
    const string EVENT_CLICK_SPECIAL_METEOR = "click_special_meteor";

    Firebase.FirebaseApp app;

    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                Debug.Log("==>> init firebase done");
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    void OnApplicationPause(bool pauseStatus)
    {
        //FacebookManager.ResumeFromPause(pauseStatus);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                OSHelper.IsInternetAvailable = false;
            else
                OSHelper.IsInternetAvailable = true;
        }
    }

    public void TrackWatchVideoAdsStart()
    {
        //if (!OSHelper.IsInternetAvailable)
        //    return;

        // Facebook
        //FB.LogAppEvent(EVENT_WATCH_ADS);

        // Firebase
        Firebase.Analytics.FirebaseAnalytics.LogEvent(EVENT_WATCH_ADS_START);
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add(EVENT_WATCH_ADS_START, "1");
        AppsFlyerSDK.AppsFlyer.sendEvent(EVENT_WATCH_ADS_START, eventValues);
    }

    public void TrackWatchVideoAdsDone()
    {
        //if (!OSHelper.IsInternetAvailable)
        //    return;

        // Facebook
        //FB.LogAppEvent(EVENT_WATCH_ADS);

        // Firebase
        Firebase.Analytics.FirebaseAnalytics.LogEvent(EVENT_WATCH_ADS_DONE);
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add(EVENT_WATCH_ADS_DONE, "1");
        AppsFlyerSDK.AppsFlyer.sendEvent(EVENT_WATCH_ADS_DONE, eventValues);
    }

    public void TrackTransformPlanet(TypePlanet type)
    {
        //if (!OSHelper.IsInternetAvailable)
        //    return;

        //// Facebook
        //var paramFacebook = new Dictionary<string, object>();
        //paramFacebook["planet_type"] = type;

        //FB.LogAppEvent(
            //EVENT_TRANSFORM_PLANET,
            //null,
            //paramFacebook);

        // Firebase
        //Firebase.Analytics.FirebaseAnalytics.LogEvent(
            //EVENT_TRANSFORM_PLANET,
            //"planet_type",
            //type.ToString());
    }

    public void TrackUpgradePlanet(string attributeName)
    {
        //if (!OSHelper.IsInternetAvailable)
        //    return;

        //// Facebook
        //var paramFacebook = new Dictionary<string, object>();
        //paramFacebook["attribute_name"] = attributeName;

        //FB.LogAppEvent(
            //EVENT_TRANSFORM_PLANET,
            //null,
            //paramFacebook);

        // Firebase
        //Firebase.Analytics.FirebaseAnalytics.LogEvent(
            //EVENT_TRANSFORM_PLANET,
            //"attribute_name",
            //attributeName);
    }

    public void TrackSelectMeteor(TypeMaterial type)
    {
        //if (!OSHelper.IsInternetAvailable)
        //    return;

        //// Facebook
        //var paramFacebook = new Dictionary<string, object>();
        //paramFacebook["meteor_type"] = type;

        //FB.LogAppEvent(
            //EVENT_TRANSFORM_PLANET,
            //null,
            //paramFacebook);

        // Firebase
        //Firebase.Analytics.FirebaseAnalytics.LogEvent(
            //EVENT_TRANSFORM_PLANET,
            //"meteor_type",
            //type.ToString());
    }
}