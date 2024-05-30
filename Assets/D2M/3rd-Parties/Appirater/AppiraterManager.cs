//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AppiraterManager : Singleton<AppiraterManager>
//{
//    public string AppId = "1473689494";
//    public int DaysUntilPrompt = 1;
//    public int UsesUntilPrompt = 10;
//    public int SignificantEventsUntilPrompt = -1;
//    public int TimeBeforeReminding = 2;
//    public bool IsDebug = true;

//    static AppiraterManager _instance;

//    // Use this for initialization
//    void Start()
//    {
//        _instance = this;

//#if !UNITY_EDITOR && UNITY_IOS
//        InitAppirater();
//#elif UNITY_ANDROID
//#endif
//    }

//    void InitAppirater()
//    {
//#if UNITY_IOS
//        //Appirater.Appirater.Init(AppId, DaysUntilPrompt, UsesUntilPrompt,
//            //SignificantEventsUntilPrompt, TimeBeforeReminding, IsDebug);

//        //Appirater.Appirater.Init(AppId, 0, 0,
//            //1, 0, false);
//#endif
//    }

//    public static void Rate()
//    {
//        _instance.RateFunction();
//    }


//    public void RateFunction()
//    {
//        // Block Interstitial Ad 1 time
//        AdManager.PASS_SHOW_INTERSTITIAL_ONE_TIME = true;

//#if !UNITY_EDITOR && UNITY_IOS
//        Appirater.Appirater.DidSignificantEvent();

//#elif UNITY_ANDROID
//        showAlertDialog(new string[] { "Rate " + StoreConstants.GAME_NAME, "If you like our game please rate it 5 stars on Google Play!!", "Later", "Rate Now" }, (int obj) => {
//            Debug.Log("Local Handler called: " + obj);
//        });
//#endif
//    }

//#if UNITY_ANDROID
//    const string pluginName = "com.example.ratingplugin2.ratingPlugin";

//    class AlertViewCallback : AndroidJavaProxy
//    {
//        private System.Action<int> alertHandler;

//        public AlertViewCallback(System.Action<int> alertHandlerIn) : base(pluginName + "$AlertViewCallback")
//        {
//            alertHandler = alertHandlerIn;
//        }
//        public void onButtonTapped(int index)
//        {
//            Debug.Log("Button tapped: " + index);
//            if (alertHandler != null)
//                alertHandler(index);
//        }
//    }

//    static AndroidJavaClass _pluginClass;
//    static AndroidJavaObject _pluginInstance;

//    public static AndroidJavaClass PluginClass
//    {
//        get
//        {
//            if (_pluginClass == null)
//            {
//                _pluginClass = new AndroidJavaClass(pluginName);
//                AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//                AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
//                _pluginClass.SetStatic<AndroidJavaObject>("mainActivity", activity);
//            }
//            return _pluginClass;
//        }
//    }

//    public static AndroidJavaObject PluginInstance
//    {
//        get
//        {
//            if (_pluginInstance == null)
//            {
//                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance");
//            }
//            return _pluginInstance;
//        }
//    }

//    double getElapsedTime()
//    {
//        if (Application.platform == RuntimePlatform.Android)
//            return PluginInstance.Call<double>("getElapsedTime");
//        Debug.LogWarning("Wrong platform");
//        return 0;
//    }

//    void showAlertDialog(string[] strings, System.Action<int> handler = null)
//    {
//        if (strings.Length < 3)
//        {
//            Debug.LogError("AlertView requires at least 3 strings");
//            return;
//        }

//        if (Application.platform == RuntimePlatform.Android)
//            PluginInstance.Call("showAlertView", new object[] { strings, new AlertViewCallback(handler) });
//        else
//            Debug.LogWarning("AlertView not supported on this platform");
//    }
//#endif
//}
