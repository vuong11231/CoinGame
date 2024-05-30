using System.Collections;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

public enum AFTER_HANDLE_SYNC_TYPE
{
    SYNC_PUSH_CUZ_SAME_DEVICE,
    SERVER_FILE_NOT_AVAILABLE,
    USER_ACCEPTED_MERGE,
    USER_DENIED_MERGE,
    CHECK_AND_ASK_TO_MERGE_NULL
}

/// <summary>
/// GUIDE_TO_USE: 
/// + Define USE_SYNC in PlayerSetting
/// + Order SyncManager before some classes!
/// + Classes that use RegisterActionAfterHandleSync() must in Awake!
/// + If Home scene is loaded only once => Must call CheckAndAskToMerge() manual every time when other scene goes to (exclude Loading)!
/// </summary>
public class SyncManager : Singleton<SyncManager>
{
    private bool isInit = false;
    bool _isAskingMerge = false;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        UsedOnlyForAOTCodeGeneration();
#endif
        LeanTween.delayedCall(0.25f, () =>
        {
            Init();
            StartCoroutine(KeepPushingAfter2Mins());
        });
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    public void UsedOnlyForAOTCodeGeneration()
    {
        //Bug reported on github https://github.com/aws/aws-sdk-net/issues/477
        //IL2CPP restrictions: https://docs.unity3d.com/Manual/ScriptingRestrictions.html
        //Inspired workaround: https://docs.unity3d.com/ScriptReference/AndroidJavaObject.Get.html

        AndroidJavaObject jo = new AndroidJavaObject("android.os.Message");
        int valueString = jo.Get<int>("what");
    }
#endif

    IEnumerator KeepPushingAfter2Mins()
    {
        while (true)
        {
            yield return new WaitForSeconds(120);

            Push();
        }
    }

    // PUBLIC
    public void Init()
    {
        if (!isInit)
        {
            isInit = true;
            //
            CheckAndAskToMerge();
        }
    }

    private void OnCompleteForcedSync(bool isCompleted, object data)
    {

        DebugLog("Start download file!");

        if (data != null && !string.IsNullOrEmpty(data.ToString()))
        {
            DebugLog("Get file done!");
            serverUserDataModel = JsonConvert.DeserializeObject<DataGameLocal>(data.ToString());

#if UNITY_EDITOR
            if (Instance.forceMerge) // For debug
            {
                CheckAndForceMerge();
            }
            else
#endif
            if (serverUserDataModel.playFabToken != Token) // Two devices
            {
                CheckAndForceMerge();
            }
            else if (MonoHelper.IsFirstLaunch("sync")) // Same device but first launch (Ex. reinstall game)
            {
                MonoHelper.SetIsFirstLaunchDone("sync");
                CheckAndForceMerge();
            }
            else // Same device, so only push new data to server.
            {
                //CheckAndForceMerge();

                serverUserDataModel = null;
                Push();

                if (FacebookManager.IsLoggedIn())
                    CallAfterHandleSyncActionsList(AFTER_HANDLE_SYNC_TYPE.SYNC_PUSH_CUZ_SAME_DEVICE);

                // Daily Gift
                //PopupDailyGift.Show();
            }
        }
        else
        {
            // Daily Gift
            //PopupDailyGift.Show();
        }
    }

    private void OnCompleteSync(bool isCompleted, object data)
    {
        if (data != null && !string.IsNullOrEmpty(data.ToString()))
        {
            serverUserDataModel = JsonConvert.DeserializeObject<DataGameLocal>(data.ToString());
            if (Instance.forceMerge) // For debug
            {

                CheckAndAskToMerge();
            }
            else if (serverUserDataModel.playFabToken != Token) // Two devices
            {
                CheckAndAskToMerge();
            }
            else if (MonoHelper.IsFirstLaunch("sync")) // Same device but first launch (Ex. reinstall game)
            {
                MonoHelper.SetIsFirstLaunchDone("sync");
                CheckAndAskToMerge();
            }
            else // Same device, so only push new data to server.
            {
                CheckAndAskToMerge();
            }
        }
        else
        {
            _isAskingMerge = false;

            DebugLog("Get file fail! ");

            // The first push!
            Push();

            // Check to link FB
            CallAfterHandleSyncActionsList(AFTER_HANDLE_SYNC_TYPE.SERVER_FILE_NOT_AVAILABLE);
        }
    }

    public void Push()
    {
    }

    public static bool IsMergeing
    {
        get { return isMergeing; }
    }

    // PRIVATE

    [Header("Configs")]
    [SerializeField] private string sceneNameToCheckAndAskToMerge = "Home";
    //[SerializeField] private string s3FolderPath = "";
    //[SerializeField] private string cognito_identity_pool_id = "";
    //[SerializeField] private string s3_bucket_name = "";

    [Header("For Debug")]
    [SerializeField] private bool debugLog = false;
    [SerializeField] private bool forceMerge = false;
    [SerializeField] private bool notPush = false;
    [SerializeField] private bool useSimulateAccount = false;
    [SerializeField] private string simulateAccount = "";

    private static DataGameLocal serverUserDataModel = null;
    private static bool isMergeing = false;
    private bool isPushing = false;
    private Coroutine waitForNextSync = null;
    private List<Action<AFTER_HANDLE_SYNC_TYPE>> afterHandleSyncActionsList = null;

    protected override void Awake()
    {
        base.Awake();

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += delegate (
            UnityEngine.SceneManagement.Scene scene, 
            UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if(scene.name == sceneNameToCheckAndAskToMerge)
            {
                CheckAndAskToMerge();
            }
        };
    }

    public void CheckAndForceMerge()
    {
        if (Instance == null)
        {
            Debug.LogError("[SYNC] Can not check and ask to merge. Cuz you are starting from Home");
            return;
        }
        else
        {
            DebugLog("CheckAndAskToMerge");

            if (serverUserDataModel != null)
            {
                ShowMergeForceAnimation();
            }
            else
            {
                DebugLog("Not merge cuz no data to merge!");
            }
        }
    }

    public void CheckAndAskToMerge()
    {
        if (Instance == null)
        {
            Debug.LogError("[SYNC] Can not check and ask to merge. Cuz you are starting from Home");
            return;
        }
        else
        {
            //if (Scenes.Current == SceneName.Home)
            //{
                DebugLog("CheckAndAskToMerge");

                if (serverUserDataModel != null)
                {
                    ShowMergePopup();
                }
                else
                {
                    DebugLog("Not merge cuz no data to merge!");
                    CallAfterHandleSyncActionsList(AFTER_HANDLE_SYNC_TYPE.CHECK_AND_ASK_TO_MERGE_NULL);
                }
            //}
            //else
            //{
            //    DebugLog("Not merge cuz no in Home!");
            //}
        }
    }

    void OnMergeButtonPressed()
    {
        // Start
        isMergeing = true;
        DebugLog("Start merge!");

        // Merge!
        Merge(serverUserDataModel);

        // Turn off merging
        isMergeing = false;

        // Reset server variable
        serverUserDataModel = null;

        // Push to server newest info!
        Push();

        // Handle PlayFab account
        CallAfterHandleSyncActionsList(AFTER_HANDLE_SYNC_TYPE.USER_ACCEPTED_MERGE);

        // Done
        OnMergeDone();
    }

    void OnCancelMergeButtonPressed()
    {
        _isAskingMerge = false;

        // Reset server variable
        serverUserDataModel = null;

        // Not use gameplay data
        OnMergeCancel();

        // Push to server newest info!
        Push();

        // Handle PlayFab account
        CallAfterHandleSyncActionsList(AFTER_HANDLE_SYNC_TYPE.USER_DENIED_MERGE);
    }

    void DebugLog(string s)
    {
        if (debugLog)
        {
            string ss = "[SYNC MANAGER] " + s;
            Debug.LogError(ss);
        }
    }

    IEnumerator WaitForNextSync()
    {
        while (isPushing)
            yield return new WaitForSeconds(2);

        Push();
        waitForNextSync = null;
    }

    void CallAfterHandleSyncActionsList(AFTER_HANDLE_SYNC_TYPE type)
    {
        if (afterHandleSyncActionsList != null)
            foreach (var i in afterHandleSyncActionsList)
                i.Invoke(type);
    }

    // CONFIG FOR EACH PROJECT (PRIVATE)
    string Email
    {
        get
        {
            if (useSimulateAccount)
            {
                if (!simulateAccount.Contains("@"))
                {
                    simulateAccount += "@gmail.com";
                }

                Instance.DebugLog("Use simalation account! " + simulateAccount);
                return simulateAccount;
            }
            else
                return UserLogin.Profile.Email;
        }
    }

    string Token
    {
        get
        {
            return DataGameSave.dataLocal.playFabToken;
        }
    }

    string Json
    {
        get
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(DataGameSave.dataLocal);
        }
    }

    void ShowMergeForceAnimation()
    {
        // Start
        isMergeing = true;
        DebugLog("Start merge!");

        // Merge!
        Merge(serverUserDataModel);

        // Turn off merging
        isMergeing = false;

        // Reset server variable
        serverUserDataModel = null;

        // Push to server newest info!
        Push();

        // Handle PlayFab account
        CallAfterHandleSyncActionsList(AFTER_HANDLE_SYNC_TYPE.USER_ACCEPTED_MERGE);

        // Done
        OnMergeDone();
    }

    void ShowMergePopup()
    {
        _isAskingMerge = true;

        PopupConfirm.ShowYesNo("Sync Your Game", "Do you want to merge your gameplay data from the server?", 
            OnMergeButtonPressed, "Yes", OnCancelMergeButtonPressed);
    }

    void OnMergeDone()
    {
        //PopupWaiting.Show(() =>
        //{
        //    // Handle after merge
        //    Scenes.ChangeSceneForced(SceneName.Home);

        //    // Noti
        //    LeanTween.delayedCall(1, () =>
        //    {
        //        Toast.ShowShort("Merge done!");
        //    });
        //});
    }

    void OnMergeCancel()
    {
        
    }

    void Merge(DataGameLocal serverModel)  // Remember to check NULL for variable on server 
    {
        // user's info
        //DataGameSave.data.listLevel = serverModel.listLevel;
        //DataGameSave.data.Heart = serverModel.Heart;
        //DataGameSave.data.Coin = serverModel.Coin;
        //DataGameSave.data.Star = serverModel.Star;
        //DataGameSave.data.MaxLevelUnlock = serverModel.MaxLevelUnlock;
        //DataGameSave.data.numberBossDefeat = serverModel.numberBossDefeat;
        //// power up
        //DataGameSave.data.rainbowPower = serverModel.rainbowPower;
        //DataGameSave.data.columnBreakerPower = serverModel.columnBreakerPower;
        //DataGameSave.data.rowBreakerPower = serverModel.rowBreakerPower;
        //DataGameSave.data.bombBreakerPower = serverModel.bombBreakerPower;
        //DataGameSave.data.breakerPower = serverModel.breakerPower;
        //// booster
        //DataGameSave.data.verHorBooster = serverModel.verHorBooster;
        //DataGameSave.data.bombBooster = serverModel.bombBooster;
        //DataGameSave.data.rainbowBooster = serverModel.rainbowBooster;
        //// ads
        //if (!serverModel.isRemoveAds && !DataGameSave.data.isRemoveAds)
        //    DataGameSave.data.isRemoveAds = false;
        //else
        //    DataGameSave.data.isRemoveAds = true;
        //// facebook connect
        //DataGameSave.data.isFacebookReward = serverModel.isFacebookReward;
        //// game center
        //DataGameSave.data.countAchievementComplete = serverModel.countAchievementComplete;
        //DataGameSave.data.currentAchievementExp = serverModel.currentAchievementExp;
        //DataGameSave.data.maxAchievementExp = serverModel.maxAchievementExp;
        //DataGameSave.data.achievementLevel = serverModel.achievementLevel;
        //// star chest
        //DataGameSave.data.starInChest = serverModel.starInChest;
        //// inbox
        //DataGameSave.data.heartChest = serverModel.heartChest;
        //DataGameSave.data.heartSpin = serverModel.heartSpin;
        //DataGameSave.data.heartDaily = serverModel.heartDaily;
        //// spin
        //DataGameSave.data.isSpin = serverModel.isSpin;
        //// daily gift
        //DataGameSave.data.isDailyGift = serverModel.isDailyGift;
        //DataGameSave.data.visitDay = serverModel.visitDay;
        //// daily quest
        //DataGameSave.data.dailyQuests = serverModel.dailyQuests;
        //// achievement
        //DataGameSave.data.bloomKill = serverModel.bloomKill;
        //DataGameSave.data.capLobsterKill = serverModel.capLobsterKill;
        //DataGameSave.data.deadBeetleKill = serverModel.deadBeetleKill;
        //DataGameSave.data.fireGhostKill = serverModel.fireGhostKill;
        //DataGameSave.data.fireGolemKill = serverModel.fireGolemKill;
        //DataGameSave.data.helmitCrabKill = serverModel.helmitCrabKill;
        //DataGameSave.data.jrKrakenKill = serverModel.jrKrakenKill;
        //DataGameSave.data.kappaKill = serverModel.kappaKill;
        //DataGameSave.data.koboldKill = serverModel.koboldKill;
        //DataGameSave.data.koboldShieldKill = serverModel.koboldShieldKill;
        //DataGameSave.data.mummyKill = serverModel.mummyKill;
        //DataGameSave.data.rockKill = serverModel.rockKill;
        //DataGameSave.data.skeletonKill = serverModel.skeletonKill;
        //DataGameSave.data.warlockKill = serverModel.warlockKill;
        //DataGameSave.data.petRescue = serverModel.petRescue;
        //DataGameSave.data.firstSetObtain = serverModel.firstSetObtain;
        //DataGameSave.data.secondSetObtain = serverModel.secondSetObtain;
    }
}