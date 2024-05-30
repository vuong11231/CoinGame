using System;
using System.Collections;
using UnityEngine;

public class GoogleMobileAdsManager : MonoBehaviour {
    public UnityMainThreadDispatcher unityMainThreadDispatcher;
    public static GoogleMobileAdsManager Instance {
        get {
            if (instance == null) {
                instance = GameObject.FindObjectOfType<GoogleMobileAdsManager>();
                instance.Init();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }
    private static GoogleMobileAdsManager instance;

    //[NonSerialized] public RewardBasedVideoAd rewardBasedVideo;
    private bool isInited;

    public static event Action OnAdRewarded;

    private Action onAdRewarded;

    void Awake() {
        if (instance == null) {
            instance = this;
            Init();
            DontDestroyOnLoad(instance.gameObject);
        }
        else if (instance != this)
            Destroy(this.gameObject);

    }


    void Init() {
#if UNITY_ANDROID
        string appKey = "df8eabf9";
#elif UNITY_IPHONE
        string appKey = "df8eabf9"; // App id android/ios khac nhau 
#else
        string appKey = "unexpected_platform";
#endif

        Debug.Log("==>> unity-script: IronSource.Agent.setUserId" + AppsFlyerSDK.AppsFlyer.getAppsFlyerId());
        IronSource.Agent.setUserId(AppsFlyerSDK.AppsFlyer.getAppsFlyerId());
        IronSource.Agent.validateIntegration();

        Debug.Log("unity-script: unity version" + IronSource.unityVersion());

        // SDK init
        Debug.Log("unity-script: IronSource.Agent.init");
        IronSource.Agent.init(appKey);

        //Add Rewarded Video Events
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
    }

    public void LoadAdIfNeeded() {
        if (!isInited) return;
    }

    public bool IsRewardedVideoReady() {
        return false;
        //return this.rewardBasedVideo != null && this.rewardBasedVideo.IsLoaded();
    }

    public void RequestRewardedVideo() {
    }
   
    public void ShowRewardedVideo(Action onAdRewarded)
    {
#if UNITY_EDITOR
        onAdRewarded.Invoke();
#endif
        AnalyticsManager.Instance.TrackWatchVideoAdsStart();
        this.onAdRewarded = onAdRewarded;
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("unity-script: IronSource.Agent.isRewardedVideoAvailable - False");
        }
    }

    void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
    {
        Debug.Log("unity-script: I got RewardedVideoAvailabilityChangedEvent, value = " + canShowAd);
    }

    void RewardedVideoAdOpenedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent");
    }

    void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
    {
        this.onAdRewarded?.Invoke();
        Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
        AnalyticsManager.Instance.TrackWatchVideoAdsDone();

    }

    void RewardedVideoAdClosedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdClosedEvent");
    }

    void RewardedVideoAdStartedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdStartedEvent");
    }

    void RewardedVideoAdEndedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdEndedEvent");
    }

    void RewardedVideoAdShowFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
    }

    void RewardedVideoAdClickedEvent(IronSourcePlacement ssp)
    {
        Debug.Log("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName());
    }
}