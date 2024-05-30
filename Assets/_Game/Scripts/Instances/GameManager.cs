using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Hellmade.Sound;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SteveRogers;
using static UIMultiScreenCanvasMan;

public class GameManager : Singleton<GameManager> {
    // -------------------------------------- SETTING --------------------------------------

#if UNITY_ANDROID
    public static int VERSION = 643;
    public static string VERSION_NAME = "6.4.3";
    public static string LINK_OF_GAME_ON_STORE = "https://play.google.com/store/apps/details?id=com.mstudio.universemaster";
#else
    public static int VERSION = 643;
    public static string VERSION_NAME = "6.4.3";
    public static string LINK_OF_GAME_ON_STORE = "https://apps.apple.com/us/app/universe-master/id1542319485";
#endif

    public static bool IS_IOS = true;

    // using server time for anti cheat, local time for testing
    public static bool USING_SERVER_TIME = true;
    public static bool DEBUG_MODE = false;
    public static bool SHOW_EVENT_BUTTON = false;
    public static bool SHOW_METEOR_FALL_EVENT_BUTTON = false;

    // vfact event section
    public static float BOSS_SIZE = 2f;             // kích thước
    public static float BOSS_HP = 4000;             // hp
    public static float BOSS_USER_ID = 77141;       // user_id của boss
    public static float BOSS_RATIO = 0.02f;         // tỷ lệ ra boss
    public static float BOSS_DELAY_ATTACK = 7f;     // đợi 5s, sau đó boss bắt đầu attack
    public static float BOSS_ATTACK_RATE = 2f;      // attack 3s 1 lần
    public static float BOSS_INCREASE_HP_RATE = 0.2f;  // boss tăng 20% máu so với bình thường

    // battle pass
    public static float BATTLE_PASS_HP_INCREASE_1 = 1f;
    public static float BATTLE_PASS_HP_INCREASE_2 = 1.1f;
    public static float BATTLE_PASS_HP_INCREASE_3 = 1.3f;
    public static float BATTLE_PASS_HP_INCREASE_4 = 1.5f;
    public static float BATTLE_PASS_HP_INCREASE_5 = 1.7f;

    public static float BATTLE_PASS_ATTACK_RATE_1 = 4f;
    public static float BATTLE_PASS_ATTACK_RATE_2 = 3.5f;
    public static float BATTLE_PASS_ATTACK_RATE_3 = 3f;
    public static float BATTLE_PASS_ATTACK_RATE_4 = 2f;
    public static float BATTLE_PASS_ATTACK_RATE_5 = 1.6f;

    public static float BATTLE_PASS_ATTACK_MISS_RANGE_1 = 20f;
    public static float BATTLE_PASS_ATTACK_MISS_RANGE_2 = 15f;
    public static float BATTLE_PASS_ATTACK_MISS_RANGE_3 = 10f;
    public static float BATTLE_PASS_ATTACK_MISS_RANGE_4 = 5f;
    public static float BATTLE_PASS_ATTACK_MISS_RANGE_5 = 2f;

    public static float BATTLE_PASS_ATTACK_Y_RANGE = -150f;

    public static int BATTLE_PASS_PRIZE_AMOUNT = 50;
    public static float BATTLE_PASS_DAME_REDUCE = 0.3f;
    public static float BATTLE_PASS_PLAYER_HIT_RANGE = 10f;

    public static int METEOR_FALL_PRIZE_AMOUNT = 50;

    // daily limit
    public static int MAX_DAILY_ATTACK_COUNT = 5;
    public static int MAX_AUTO_SHOOT_COUNT = 10;
    public static int MAX_METEOR_VISIT_COUNT = 5;
    public static int MAX_WATCH_ADS_PER_DAY_LIMIT = 5;

    public static int AUTO_RESTORE_WATCH_ADS_MINUTE = 3;

    public static float CHECK_SERVER_TIME_RATE = 60f;
    public static float SAVE_DATA_FOR_ENEMY_RATE = 10f;

    // tự động hạ level nếu không đạt quest
    public static bool LEVEL_AUTO_CORRECTION = true;

    public static float STEAL_MATERIAL_RATE_OFFLINE_COLLECT = 0f; // 100% số vật chất offline 
    public static float STEAL_MATERIAL_RATE_TOTAL = 0f; // 10% tổng số vật chất

    //public static float METEOR_FALL_MIN_SECONDS = 60f;
    //public static float METEOR_FALL_MAX_SECONDS = 200f;
    public static float METEOR_FALL_MIN_SECONDS = 10000000000000000000000000000000000000f;
    public static float METEOR_FALL_MAX_SECONDS = 15000000000000000000000000000000000000f;
    public static float METEOR_FALL_HP = 2500f;
    public static float METEOR_FALL_WAVE_TIME_START = 10f;
    public static float METEOR_FALL_WAVE_TIME_INCREASE_PER_LEVEL = 2f;
    public static float METEOR_FALL_WAVE_TIME_INCREASE_PER_WAVE = 3f;

    // -------------------------------------- COMMON VARIABLE --------------------------------------
    public List<Sprite> listAvatar = new List<Sprite>();
    public List<Sprite> listSkin = new List<Sprite>();
    public static DataReward reward;

    public static float battlePassAttackMissRange;
    public static int meteorBeltLevel = 1;
    public static bool needToSaveData = false;
    public static float sunHp = 0;

    public static bool isFromDailyBattle = false;
    public static bool isClaimableDailyBattle = false;

    public static bool isFromRank = false;
    public static int rankPoint;
    public static int rankPointChange;

    public static bool executeFinish = false;

    public static Mode lastGameplayMode = Mode.Gameplay;
    public static float planetSpeedRatio = 1f;
    public static bool takeNoDame = false;
    // -------------------------------------- PUBLIC FIELD --------------------------------------
    public GameObject textTutCanvasPrefab = null;
    public Sprite diamondSprite = null;
    public Sprite matSprite = null;

    [Header("Configs Daily Mission")]
    public List<DailyQuestData> dailyQuestData;

    [Header("Other Configs")]
    public float devidePlanetDistance = 15f;

    [Header("Effects Outside")]
    public GameObject
        effectOutside_Air = null;

    public GameObject
        effectOutside_Fire = null,
        effectOutside_Ice = null,
        effectOutside_Anti = null,
        effectOutside_Default = null,
        effectOutside_Gravity = null;

    [Header("Attack Zones - Ice")]
    public float iceAttackZoneExistTime = 5;
    public GameObject attackEffect_IceZone = null;

    [Tooltip("Số bước để dừng lại hẳn")]
    public int planetAttacked_IceStopCountMax = 200;

    [Header("Attack Zones - Fire")]
    public GameObject attackEffect_FireZone = null;
    public GameObject attackEffect_FireGotDame = null;
    public float fireAttackZoneExistTime = 3;

    [Header("Tutorial")]
    public GameObject meteorTutPrefab = null;
    public int meteorTut_NumMatReward = 8700;
    public float maxCamOrthoTutZoom = 30;

    [Header("Attack Zones - AntiMatter")]
    public float antiMatterAttackZoneExistTime = 3;
    public GameObject attackEffect_AntiMatterZone = null;

    [Header("Others")]
    public DataScriptableObject DataFile = null;
    public DataPrefab dataPrefab = null;

    public static RuntimePlatform Platform {
        get {
            if (Application.isEditor) {
                return IS_IOS ? RuntimePlatform.IPhonePlayer : RuntimePlatform.Android;
            }
            return Application.platform;
        }
    }

    private static DateTime NowServer = DateTime.Now;
    public static DateTime Now {
        get {
            if (GameManager.USING_SERVER_TIME) {
                return NowServer;
            } else {
                return DateTime.Now;
            }
        }
    }

    public static DataPrefab Data => Instance.dataPrefab;

    public static bool IsAutoRestorePlanet {
        get {
            return GameManager.Now.Ticks < DataGameSave.autoRestoreEndTime;
        }
    }

    public static float BIG_METEOR_MAX_HP = 3000;

    public static float bigMeteorHp1 {
        set {
            PlayerPrefs.SetFloat("BigMeteorHp1", value);
        }

        get {
            return PlayerPrefs.GetFloat("BigMeteorHp1", BIG_METEOR_MAX_HP);
        }
    }

    public static float bigMeteorHp2 {
        set {
            PlayerPrefs.SetFloat("bigMeteorHp2", value);
        }

        get {
            return PlayerPrefs.GetFloat("bigMeteorHp2", BIG_METEOR_MAX_HP);
        }
    }

    public static int DailyBattleAttackCount {
        set {
            PlayerPrefs.SetInt("DailyBattleAttackCount", value);
        }

        get {
            int savedDay = PlayerPrefs.GetInt("DailyBattleSavedDay", -1);

            if (savedDay == -1 || savedDay != int.Parse(DataGameSave.GetDayCode(GameManager.Now))) {
                PlayerPrefs.SetInt("DailyBattleSavedDay", int.Parse(DataGameSave.GetDayCode(GameManager.Now)));
                PlayerPrefs.SetInt("DailyBattleAttackCount", MAX_DAILY_ATTACK_COUNT);
            }

            return PlayerPrefs.GetInt("DailyBattleAttackCount", 0);
        }
    }

    protected override void Awake() {
        base.Awake();
        TextMan.LangIndex = PlayerPrefs.GetInt("languageidx", 0);
    }

    private void Start() {
        CheckTime();
        //IsActiveFbSync = true;
        reward = new DataReward();

        if (Cheat.Get("clear console after start"))
            LeanTween.delayedCall(0.5f, Utilities.ClearConsole);

        Instantiate(textTutCanvasPrefab);

        PlayfabManager.GetServerTime(
               0,
               (result, index) => {
                   NowServer = result;
               });

        StartCoroutine(CheckServerTime());
        StartCoroutine(UpdateLocalTime());
        StartCoroutine(SaveDataForEnemy());
    }

    IEnumerator CheckServerTime() {
        while (true) {
            PlayfabManager.GetServerTime(
               0,
               (result, index) => {
                   NowServer = result;
               });

            yield return new WaitForSecondsRealtime(CHECK_SERVER_TIME_RATE);
        }
    }

    IEnumerator UpdateLocalTime() {
        while (true) {
            NowServer = NowServer.AddSeconds(1);
            yield return new WaitForSecondsRealtime(1);
        }
    }

    /// <summary>
    /// data use for enemy search
    /// </summary>
    /// <returns></returns>
    IEnumerator SaveDataForEnemy() {
        while (true) {
            if (DataGameSave.dataServer != null) {
                WWWForm form = new WWWForm();
                form.AddField("data", JsonConvert.SerializeObject(DataGameSave.dataServer));
                ServerSystem.Instance.SendRequest(ServerConstants.SAVE_USER_DATA, form, null);
            }
            
            yield return new WaitForSecondsRealtime(SAVE_DATA_FOR_ENEMY_RATE);
        }
        
    }

    public void OnApplicationQuit()
    {
        DataGameSave.SaveToLocal();
    }

    public void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            DataGameSave.SaveToLocal();
        }
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            DataGameSave.SaveToLocal();
        }
    }

    void CheckTime()
    {
        long timemax;
        long timeafk;
        timemax = TimeHelper.ConvertDatetimeToSecond(DateTime.Now) - TimeHelper.ConvertDatetimeToSecond(DateTime.MinValue);

        timeafk = timemax - DataGameSave.dataLocal.lastLogin.Timeoff;
        
        // 1 day passed
        if (timeafk > 0)
        {
            if (DataGameSave.dataLocal.lastLogin.dayLogin != DateTime.Now.Day)
            {
                PopupDailyMission.ResetData();
            }
        }
    }

    public void Revenge(int id)
    {
        string url = ServerConstants.GET_REVENGE_ENEMY_DATA;

        WWWForm form = new WWWForm();
        form.AddField("userid", Convert.ToInt32(DataGameSave.dataLogin.userid.ToString()));
        form.AddField("token", DataGameSave.dataLogin.token.ToString());
        form.AddField("enemyid", id.ToString());

        ServerSystem.Instance.SendRequest(url, form, () =>
        {
            if (!ServerSystem.Instance.IsResponseOK())
            {
                PopupConfirm.ShowOK("Oops", "Something wrong with yout enemy", "OK");
            }
            else
            {
                try
                {
                    string jsonData = ServerSystem.Instance.ReadData();

                    DataGameServer dataGameServer = JsonConvert.DeserializeObject<DataGameServer>(jsonData,
                        new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

                    if (dataGameServer.ListEnemy == null)
                    {
                        dataGameServer.ListEnemy = new List<string>();
                    }

                    if (dataGameServer.ListPlanet == null)
                    {
                        dataGameServer.ListPlanet = new List<DataPlanet>();
                    }

                    if (dataGameServer.ListPlanet.Count == 0)
                    {
                        dataGameServer.ListPlanet.Add(new DataPlanet());
                    }

                    AttackEnemy(dataGameServer);
                }
                catch { }
            }
        });
    }

    public void OnPressed_Privacy()
    {
        Application.OpenURL("https://sites.google.com/view/d2m-privacy-policy");
    }

    public void AttackEnemy(DataGameServer enemyData)
    {
        if (GameStatics.IsAnimating)
            return;

        if (DataGameSave.IsAllPlanetDestroyed()) {
            PopupConfirm.ShowOK(TextConstants.NO_PLANET, TextConstants.NO_PLANET_MESSAGE);
            return;
        }

        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);

        DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.Battle].currentProgress++;

        if (BtnDailyMission.Instance)
        {
            BtnDailyMission.Instance.CheckDoneQuest();
        }

        DataGameSave.dataEnemy = enemyData;
        DataGameSave.dataEnemy = enemyData;

        Scenes.LastScene = SceneName.Gameplay;
        Scenes.ChangeScene(SceneName.Battle);
    }

    private void Update()
    {
        //if (Utilities.IsPressed_Space)
        //{
        //    "spaced".Log();

        //}
    }
}