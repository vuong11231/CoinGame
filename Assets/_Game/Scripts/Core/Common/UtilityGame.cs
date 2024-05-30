using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;

public class UtilityGame
{
    public readonly static string CharacterHairPath = "Assets/Prefabs/CharHair/Char_";
    public readonly static int Rows = 4;
    public readonly static int Columns = 4;
    public static string NONE_VALUE_CHAR = "--";
    public static readonly float AnimationDuration = 0.05f;
    public const int LIMIT_BUY_ARROW_VALUE = 2;
    public const int TOTAL_TIME_WAITING_VIDEO = 10;
    public const int MAX_MISSION_VALUE = 10;
    public const int UPGRADE_GLORY_HERO_PER = 5;
    public const int UPGRADE_GLORY_BOW_PER = 5;
    public const int UPGRADE_EVOLUTION_HERO_PER = 10;
    public const int MAX_MAP_VALUE = 7;
    public const int MAX_DAY_DAILY_GIFT = 7;
    public const int MAX_ACCURACY_RATE_ENEMY = 100;
    public const float TIME_PROTECT_DURATION = 10f;
    public const float RATIO_PERCENT = 100.0f;
    public const float ORTHO_SIZE_DEFAULT = 3f;
    public const int ITEM_TO_ANALYTIC = 128;
    public const int MAX_HERO_LEVEL = 99;
    public const int MAX_UPGRADE_BOW_LEVEL = 25;
    public const int MAX_ENEMY_LEVEL = 100;
    public const int MAX_EQUIPMENT_LEVEL = 149;
    public const int MAX_SKILL_LEVEL = 15;
    public const int MAX_HERO = 1;//tam thoi chi co 1 con
    public const int MAX_HERO_ID = 4;
    public const int TAKE_DAMAGE_NOT_BELOW_VALUE_PERCENT = 30;
    //private const int CO_EFFIECENT_STAGE_REWARD = 100;
    public const float SIZE_OF_CHARACTER = 0.14f;
    public const float FIRE_ARROW_DAMAGE_INCREASE = 1.2f;
    public const float PERCENT_USE_HEALING_BOOSTER = 0.2f;
    public const float MAX_FORCE_ARCHERY = 2.1f;
    public const float MIN_FORCE_ARCHERY = 0.35f;
    public const float MAX_ANGLE_ARCHERY = 145f;
    public const float MIN_ANGLE_ARCHERY = 40f;
    public static float SIZE_WIDTH = 1920F;
    public static float SIZE_HEIGHT = 1080F;
    public static float BUBBLE_RADIUS = 0.43f, BUBBLE_COLLIDE_RADIUS = 0.32f;
    public static float BUBBLE_SCALE = 1.0f;
    public const int GEM_TO_REVIVE = 5;
    public const int ISLAND_LENGTH_PER_ELEMENT = 5;
    public const float SHIP_ISLAND_SCALE = 0.25f;
    public const float TREASURE_ISLAND_SCALE = 0.4f;
    const float PERCENT_RANDOM_FISH_ITEM = 0.75f;
    public const int TIME_SHORTEN_BUILDING = 1800; //second
    public const int GOLD_VALUE_TO_SHORTEN = 20;
    public const int BOMB_FOR_SINGLE_ROOM = 1;
    public const int BOMB_FOR_FULL_ROOM = 2;

    public const int MAX_SCENE_LEVEL_ID = 23;
    public const int MAX_STAGE_ID = 9;
    public const int MAX_STAR_OF_STAGE = 3;
    public const int MAX_MAP_ID = 3;
    public const int MAX_COIN_IN_MAP = 75;
    public const int MAX_EXP_IN_MAP = 75;
    public const int MAX_STAGE_IN_MAP = 10;
    public const int TIME_BETWEEN_VIDEO_MAIN = 10;
    public const float MULTI_HEALTH_PVP = 10f;
    public const int MAX_SOUL_BET_WEEK = 3000;
    public const int MAX_SOUL_STREAK_WEEK = 5000;
    public const int TRUE = 1, FALSE = 0;
    public const float MAP_4_SPEED = 0.5f;
    public const int RATE_GOLD_TO_EVENT_COIN = 10;
    public const int SUMMONER_ID = 9999;
    public const string KeyIdUser = "KeyIdUser";
    public const string KeyTimeAutoBackup = "KeyTimeAutoBackup";
    public const float rangeFlip = 0.05f;
    public const int limitSlotInventory1 = 99;
    public const int limitSlotInventory2 = 999;
    public const int TURN_RAID_FREE = 2;
    public const float POWER_UP_VIDEO = 0.2f;
    public const int STAMINA_RAID = 10;
    public const int ROUNDSKIPTRICKMODE = 10;
    public const int MAX_VIDEO_MAIN = 7;
    public const int MAX_NUMBER_SLOT = 12;
    public const float NORMAL_REFUND_PERCENT = 0.2f;
    public const float GEM_REFUND_PERCENT = 1f;
    public static TimeSpan TimeFirstResetMarket = new TimeSpan(5, 0, 0);
    public static TimeSpan TimeSecondResetMarket = new TimeSpan(18, 0, 0);
    public static bool canUpdateDataToServer { get { return false; } }
    // character
    public const string Shader = "Spine/Shinigami/ChangeColorSpine";
    public static Quaternion flipCharacter = Quaternion.Euler(0, 180, 0);
    public static Color Transparent = new Color(1, 1, 1, 0);
    public static string KeyVersionDataBackup = "KeyVersionData";

    // key playpref
    public const string RatedGame = "RatedGame";
    public const string DataPlayer = "dataPlayer";
    public const string DataPlayerObscured = "dataPlayerObscured";
    public const string RemoveAds = "removeAds";
    public const string BackupData = "backupData";
    public const string DataCreatePlayer = "dataCreatePlayer";
    public const string DataCreatePlayerObscured = "dataCreatePlayerObscured";
    public const string DataAccountObscured = "dataAccountObscured";
    public const string SoundBgm = "soungBgm";
    public const string SoundSe = "soungSe";
    public const string NewVersion = "newVersion";
    public const string FirstBranchPoint = "FirstBrandPoint";
    public const string FirstBranchPointMastery = "FirstBranchPointMastery";
    public const string PlayerSkin = "PlayerSkin_";
    public const string LimitedOffer = "KeyLimitedOffer";
    public const string KeyLastDayLogin = "LastDayLoginGame";
    public const string BEST_SCORE_KEY = "BestScore";
    public const string SCORE_KEY = "Score";

    // Setting
    public const string IsEnableAutoButton = "IsEnableAutoButton";
    public const string IsEnableNotiFullStamina = "IsEnableNotiFullStamina";
    public const string IsEnableNotiHaveLimitedOffer = "isEnableNotiHaveLimitedOffer";
    public const string IsEnableNotiBeforeEndLimitedOffer = "IsEnableAutoButton";
    public const string KeyEnableRestore = "KeyEnableRestore";
    // item in game
    public const string Soul = "soul";
    public const string Gold = "gold";
    public const string Stamina = "stamina";
    public const string Skip = "skip";
    public const string EventCoin = "event_coin";

    //-new key
    public const string UnlockBetFeature = "unlockBet";
    public const string ExpiryDateLimitedExpRatio = "ExpiryDateLimitedExpRatio";
    public const string ExpiryDateLimitedGoldRatio = "ExpiryDateLimitedGoldRatio";

    //Quest
    public const string QuestSaveDay = "QuestSaveDay";
    public const string DailyQuestData = "DailyQuestData";
    public const string AchievementData = "AchievementData";
    public const string PurchaseCounter = "PurchaseCounter";
    public const string PurchaseMoneyCount = "PurchaseMoneyCout";

    public const string FirstTime = "FirstTime";
    public const string Session = "SESSION";
    public const string AccumulateSpendRewarded = "AccumulateSpendRewarded";

    //Starter package
    public const string KeyIsAutoShowStarterPackage = "IsAutoShowStarterPackage";
    public const string KeyIsBoughtStarterPackage = "IsBoughtStarterPackage";
    public const string KeyIsBoughtGrowUpPackage = "IsBoughtGrowUpPackage";
    public const string KeyIsSeenGrowUpPackage = "IsSeenGrowUpPackage";
    public const string KeyIsSeenGrowUpPackageSkip = "IsSeenGrowUpPackageSkip";

    //Analytics
    public const string ValueShopTypeSuggest = "SuggestShop";
    public const string ValueShopTypeNormal = "NormalShop";

    public const string KeyBlackMarketUserData = "KeyBlackMarketDataUser";
    public const string KeyRewardInstallTD = "RewardInstallTD";

    //Ladder
    public const string KeyGroupLadder = "KeyGroupLadder";
    public const string KeyTimeGetLeaderBoard = "TimesGetLadderLeaderBoard";
    public const string KeyDataLadderLeaderBoard = "dataLeaderBoardData";
    public const string KeyTimeGetGroupDataLadder = "TimesGetGroupLadder";
    public const string KeySaveBotLadderData = "KeySaveBotDataLadder";

    //Chest
    public const string KeyTimeToChestFree = "TimeToFreeChest_";

    //Building
    const string KeyTimeWaitingToSeeVideo = "TimeWaitingToSeeVideo";
    const string KeyTimeSubByShorten = "TimeSubByShorten_";

    public static void RequestStoreLink()
    {
        Application.OpenURL("market://details?id=" + Application.identifier);
    }

    public static bool CanSeeVideo
    {
        get
        {
            if (PlayerPrefs.GetInt("CanSeeVideo", 1) == 1)
                return true;
            else
                return false;
        }
        set
        {
            PlayerPrefs.SetInt("CanSeeVideo", value ? 1 : 0);
        }
    }

    public static int RateGameState
    {
        get { return PlayerPrefs.GetInt("click_rate", 0); }
        set { PlayerPrefs.SetInt("click_rate", value); }
    }

    public static string OldDateGiftData
    {
        get { return PlayerPrefs.GetString("DAY_GIFT_SAVE", ""); }
        set { PlayerPrefs.SetString("DAY_GIFT_SAVE", value); }
    }

    public static DailyRewardStatusType Get_Daily_Index_Can_Receive_reward(int day)
    {
        return (DailyRewardStatusType)PlayerPrefs.GetInt("Daily_Gift_Day_Data_" + day, (int)DailyRewardStatusType.WAITING);
    }

    public static void Set_Daily_Index_Can_Receive_reward(int day, DailyRewardStatusType dailyTypeStatus)
    {
        PlayerPrefs.SetInt("Daily_Gift_Day_Data_" + day, (int)dailyTypeStatus);
    }

    public const int FreeTurnRaid = 2;

    public static bool IS_DEBUG = true;
    public static bool IS_TEST = false;
    public static DateTime timeStartEventLadder = new DateTime(2018, 5, 14, 0, 0, 1);
    //public static DateTime timeStartEventWheel = new DateTime(2018, 5, 28, 0, 0, 1);
    /// <summary>
    /// Giờ Server tuy nhiên chuyển theo múi giờ GMT 0
    /// </summary>
    public static DateTime currentTimeServerTimeZoneZero = DateTime.MaxValue;
    public static float timeGetServerTime = 0;
    public static bool haveTimeServer = false;
    public static int lockIos = -1;
    // -1 la chua check, 1 la ios test, 0 la ios run like android, -2 la check roi nhung ko check dc
    public static string country_code = "";
    public static bool isUnityAds = true;
    //public static List<string> countries_unity_ads = new List<string> { "ID", "IN", "RU", "MY", "DE" };
    //public static bool isLuxuryRegion = true;
    //public static List<string> countries_luxury = new List<string> { "CA", "GB", "DE", "IT", "FR", "US" };
    public static string country_code_key = "KeyCountryUser";
    public static WaitForSeconds OneSecond = new WaitForSeconds(1f);
    public static Color32 positiveDispelColor = new Color32(65, 83, 195, 255);
    public static Color32 solveColor = new Color32(65, 83, 195, 255);
    public static Color32 slowColor = new Color32(65, 83, 195, 255);
    public static Color32 freezeColor = new Color32(65, 83, 195, 255);
    public static Color32 stunColor = new Color32(136, 136, 136, 255);
    public static Color32 poisonColor = new Color32(157, 60, 145, 255);
    public static Color32 bleedColor = new Color32(65, 255, 65, 255);
    public static Color32 burnColor = Color.red;

    public static DateTime GetServerCurrentTimeZoneZero()
    {
        return currentTimeServerTimeZoneZero.AddSeconds(Time.timeSinceLevelLoad - timeGetServerTime);
    }

    public static double GetScreenDimension()
    {
        return Math.Truncate(((float)Screen.width / Screen.height) * 100.0) / 100.0;
    }

    //Symbol

    public static List<int> dayCollectDailyRewardTracking = new List<int>() { 1, 3, 7, 14, 28 };

    public static List<int> levelUserTracking = new List<int>() { 5, 8, 10, 12, 15, 22, 30 };

    public static string FishSymbol { get { return Localize.GetLocalizedString("fish_symbol"); } }

    public static string GemSymbol { get { return Localize.GetLocalizedString("gem_symbol"); } }

    public static string GoldSymbol { get { return Localize.GetLocalizedString("gold_symbol"); } }

    public static string ExpLocalize { get { return Localize.GetLocalizedString("exp"); } }

    public static string StaminaSymbol { get { return Localize.GetLocalizedString("stamina"); } }

    public static string DisconnectServer { get { return Localize.GetLocalizedString("disconnect_server"); } }

    public static string LadderPointSymbol { get { return ":ladderpoint:"; } }

    public static string EventVideoSymbol { get { return ":video:"; } }

    public static string NotificationNotEnoughRes(params object[] args)
    {
        return Format(Localize.GetLocalizedString("not_enough_res_value"), args);
    }

    public static string NotificationComingSoon { get { return Localize.GetLocalizedString("coming_soon"); } }

    public static string NotificationNotEnoughFish { get { return Localize.GetLocalizedString("not_enought_fish"); } }

    public static string NotificationNotEnoughGold { get { return Format(Localize.GetLocalizedString("warning_not_enough_resource"), GoldSymbol); } }

    public static string NotificationNotEnoughGem { get { return Format(Localize.GetLocalizedString("warning_not_enough_resource"), GemSymbol); } }

    public static string NotificationRevival { get { return Localize.GetLocalizedString("notification_revival"); } }

    public static string ToCamelCase(string input)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
    }

    /// <summary>
    /// Set color by hex code for input string. Require the text component must be checked Rich text
    /// </summary>
    /// <param name="inputValue"></param>
    /// <param name="hexColorStr"></param>
    /// <returns></returns>
    public static string SetColorString(string inputValue, string hexColorStr)
    {
        return UtilityGame.Format("<color=#{1}>{0}</color>", inputValue, hexColorStr);
    }

    public static string[] ReadLineContent(string content)
    {
        return content.Split("\n"[0]);
    }

    public static string[] ReadValInContent(string content)
    {
        return content.Trim().Split(","[0]);
    }

    public static Vector2 ReadValRandom(string content)
    {
        string[] vals = content.Trim().Split("~"[0]);
        if (vals.Length == 2)
        {
            return new Vector2(float.Parse(vals[0]), float.Parse(vals[1]));
        }
        else
        {
            return new Vector2(float.Parse(vals[0]), float.Parse(vals[0]));
        }
    }



    /// <summary>
    /// Time play game and get stars. content (a~b). return vector2(x: minute, y: second)
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static Vector2 ReadValVector2(string content, char key = '~')
    {
        string[] vals = content.Trim().Split(key);
        if (vals.Length == 2)
        {
            return new Vector2(float.Parse(vals[0]), float.Parse(vals[1]));
        }
        else
        {
            return new Vector2(float.Parse(vals[0]), 0);
        }
    }

    public static Vector2Int ReadVector2Int(string content, char key = '~')
    {
        string[] vals = content.Trim().Split(key);
        try
        {
            Vector2Int value = Vector2Int.zero;
            if (vals.Length == 1)
                value = new Vector2Int(int.Parse(vals[0]), int.Parse(vals[0]));
            else
                value = new Vector2Int(int.Parse(vals[0]), int.Parse(vals[1]));
            return value;
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            return new Vector2Int();
        }
    }

    public static bool IsVibrate
    {
        get
        {
            return PlayerPrefs.GetInt("Vibrate", 1) == 0 ? false : true;
        }set
        {
            PlayerPrefs.SetInt("Vibrate", value ? 1 : 0);
        }
    }

    public static void SaveSoundBGM(float value)
    {
        PlayerPrefs.SetFloat(SoundBgm, value);
    }

    public static float GetSoundBGM(float defauteValue)
    {
        return PlayerPrefs.GetFloat(SoundBgm, defauteValue);
    }

    public static void SaveSoundSE(float value)
    {
        PlayerPrefs.SetFloat(SoundSe, value);
    }

    public static float GetSoundSE(float defauteValue)
    {
        return PlayerPrefs.GetFloat(SoundSe, defauteValue);
    }

    public static void PauseGame()
    {
        Time.timeScale = 0;
    }

    public static void UnPauseGame()
    {
        Time.timeScale = 1;
    }

    public static void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }

    public static string ConvertKMT(int num)
    {
        if (num < 1000000)
        {
            if (num >= 10000)
            {
                num = (num / 100) * 100;
                return (num / 1000f).ToString("0.#") + "K";
            }
            else
            {
                return num.ToString();
            }
        }
        else if (num < 1000000000)
        {
            num = (num / 100000) * 100000;
            return (num / 1000000f).ToString("0.#") + "M";
        }
        else
        {
            num = (num / 100000000) * 100000000;
            return (num / 1000000000f).ToString("0.#") + "B";
        }
    }

    public static string ConvertKMT(float num)
    {
        if (num < 1000000f)
        {
            if (num >= 10000f)
            {
                num = ((long)(num / 100)) * 100;
                return (num / 1000f).ToString("0.#") + "K";
            }
            else
            {
                return num.ToString();
            }
        }
        else if (num < 1000000000f)
        {
            num = ((long)(num / 100000)) * 100000;
            return (num / 1000000f).ToString("0.#") + "M";
        }
        else if (num < 1000000000000f)
        {
            num = ((long)(num / 100000000)) * 100000000;
            return (num / 1000000000f).ToString("0.#") + "B";
        }
        else
        {
            num = ((long)(num / 100000000000)) * 100000000000;
            return (num / 1000000000000f).ToString("0.#") + "T";
        }
    }

    public static string FormatNumber(long number)
    {
        return UtilityGame.Format("{0:n0}", number);
    }

    public static string GetVersion()
    {
        return Application.version;
    }

    public static int ConvertVersionToNumber(string version)
    {
        var arr = version.Split('.');
        int number = int.Parse(arr[0]);
        number = number * 100 + int.Parse(arr[1]);
        number = number * 100 + int.Parse(arr[2]);
        return number;
    }

    /// <summary>
    /// Convert number second to vector time 
    /// </summary>
    /// <param name="countTime"></param>
    /// <returns></returns>
    public static Vector2 ConvertTime(int countTime)
    {
        return new Vector2(countTime / 60, countTime % 60);
    }

    public static int ConvertSecond(Vector2 time)
    {
        return (int)(time.x * 60 + time.y);
    }

    public static bool IsLoadedSceneMainMenu()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("mainMenu");
    }

    public static bool IsLoadedScenePVP()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("gameplayPVPMode");
    }

    public static DateTime GetTimeFromString(string timeString)
    {
        try
        {
            var time = timeString.Split(' ');
            string timeParse = time[3] + "-" + time[2] + "-" + time[1];
            return DateTime.Parse(timeParse);
        }
        catch
        {
            return DateTime.Now;
        }

    }

    public static string GetTimeRemainCountDown(int timeRemain)
    {
        if (timeRemain >= 3600)
        {
            return UtilityGame.Format("{0:00}H {1:00}M {2:00}S", timeRemain / 3600, timeRemain % 3600 / 60, timeRemain % 60);
        }
        else if (timeRemain<3600 && timeRemain>=60)
        {
            return UtilityGame.Format("{0:00}M {1:00}S", timeRemain % 3600 / 60, timeRemain % 60);
        }
        else
        {
            return UtilityGame.Format("{0:00}S", timeRemain % 60);
        }
    }

    public static int GetGoldValueToCompleteImmediate(int timeRemain)
    {
        float percentDecrease = 10; //decrease value percent for immediately complete building
        int goldValueCalculate = 0;
        if (timeRemain / TIME_SHORTEN_BUILDING > 0)
        {
            goldValueCalculate = (int)(((float)timeRemain / TIME_SHORTEN_BUILDING) * GOLD_VALUE_TO_SHORTEN);
            goldValueCalculate -= (int)(goldValueCalculate * (percentDecrease / 100)); //sale off 
            //Debug.Log("Gold after: " + (goldValueCalculate * (percentDecrease / 100)));
        }
        else
        {
            goldValueCalculate = GOLD_VALUE_TO_SHORTEN - (int)(GOLD_VALUE_TO_SHORTEN * (percentDecrease / 100));
        }
        return goldValueCalculate;
    }

    public static bool IsNetworkAvailable()
    {
        return true;
    }

    //public static void SetFrameRate(int frameRate)
    //{
    //    Log.Info("FPS: " + frameRate);
    //    QualitySettings.vSyncCount = 0;
    //    Application.targetFrameRate = frameRate;
    //    GameManager.SpeedMultiflier = frameRate / (float)GameManager.BaseFrameRate;
    //    GameNewManager.SpeedMultiflier = (float)GameNewManager.BaseMinFrameRate / frameRate;
    //    GameNewManager.DoubleSpeedMultiflier = GameNewManager.SpeedMultiflier * 2;
    //    Time.fixedDeltaTime = 0.04f * GameNewManager.SpeedMultiflier;
    //}

    public static string RemoveSign4VietnameseString(string str)
    {
        for (int i = 1; i < VietnameseSigns.Length; i++)
        {
            for (int j = 0; j < VietnameseSigns[i].Length; j++)
                str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
        }
        return str;
    }
    

    public static DateTime ConvertStringTime(string time)
    {
        int year = int.Parse(time.Substring(0, 4));
        int month = int.Parse(time.Substring(4, 2));
        int day = int.Parse(time.Substring(6, 2));
        int hour = int.Parse(time.Substring(8, 2));
        int minute = int.Parse(time.Substring(10, 2));
        int second = int.Parse(time.Substring(12, 2));
        return new DateTime(year, month, day, hour, minute, second);
    }

    public static string ConvertTimeToString(DateTime time)
    {
        string year = time.Year.ToString();
        string month = time.Month.ToString("D2");
        string day = time.Day.ToString("D2");
        string hour = time.Hour.ToString("D2");
        string minute = time.Minute.ToString("D2");
        string second = time.Second.ToString("D2");
        return year + month + day + hour + minute + second;
    }

    public static DateTime ConvertIntTime(int time)
    {
        int year = 0;
        int month = 0;
        int day = 0;
        int hour = time % (3600 * 24) / 3600; ;
        int minute = time % (3600) / 60;
        int second = time % 60;
        return new DateTime(year, month, day, hour, minute, second);
    }

    public static string ConvertIntTimeToString(int time)
    {
        int hour = time % (3600 * 24) / 3600; ;
        int minute = time % (3600) / 60;
        int second = time % 60;
        return UtilityGame.Format("{0}:{1}:{2}", hour.ToString("D2"), minute.ToString("D2"), second.ToString("D2"));
    }

    public static string ConvertTimeSpawnToString(TimeSpan time)
    {
        return UtilityGame.Format("{0}:{1}:{2}", time.Hours.ToString("D2"), time.Minutes.ToString("D2"), time.Seconds.ToString("D2"));
    }


    private static readonly string[] VietnameseSigns = new string[]
    {
        "aAeEoOuUiIdDyYn",
        "áàạảãâấầậẩẫăắằặẳẵä",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữü",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ",
        "ñ"
    };

    public static string GetSkinName(int heroId, int skinId)
    {
        return "skin_" + heroId + "_" + skinId;
    }

    public static string GetSpriteHeroName(int heroId)
    {
        return "hero_" + heroId;
    }

    public static string GetPackageName(int heroId, int skinId)
    {
        return "package_" + heroId + "_" + skinId;
    }

    public static void ForEachEnum<T>(Action<T> action)
    {
        if (action != null)
        {
            var types = Enum.GetValues(typeof(T));
            foreach (var type in types)
            {
                action((T)type);
            }
        }
    }

    public static void QuitGame()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                //using (AndroidJavaClass javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                //{
                //    AndroidJavaObject unityActivity = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
                //    unityActivity.Call<bool>("moveTaskToBack", true);
                //}
                try
                {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                catch
                {
                    Application.Quit();
                }
                break;
            default:
                Log.Info("Quit Game");
                Application.Quit();
                break;
        }
    }

    public static string GetLocalizeGloryHero(int gloryLevel)
    {
        int gloryID = gloryLevel + 20;
        return Localize.GetLocalizedString("achievement_title_" + gloryID);
    }

    public static string GetLocalizeGloryBow(int gloryLevel)
    {
        return Localize.GetLocalizedString("achievement_title_7" + (gloryLevel - 1));
    }

    public static string GetStringColorRarity(RarityType rarity)
    {
        return GetStringColorRarity((int)rarity);
    }

    public static string GetStringColorRarity(int rarity)
    {
        switch (rarity)
        {
            case 0:
                return ("FFFFFFFF");
            case 1:
                return ("00FF37FF");
            case 2:
                return ("3176FFFF");
            case 3:
                return ("FCFF42FF");
            default:
                return "FFFFFFFF";
        }
    }

    public static string GetLocalizeRarity(RarityType rarity)
    {
        return GetLocalizeRarity((int)rarity);
    }

    public static string GetLocalizeRarity(int rarity)
    {
        switch (rarity)
        {
            case 0:
                return Localize.GetLocalizedString("common");
            case 1:
                return Localize.GetLocalizedString("rare");
            case 2:
                return Localize.GetLocalizedString("epic");
            case 3:
                return Localize.GetLocalizedString("legendary");
            default:
                return "";
        }
    }

    public static Sprite GetSpriteResourceIcon(int typeID)
    {
        string path = Format("Texture2D/ResourcesValue/" + typeID);
        return Resources.Load<Sprite>(path);
    }

    public static string GetStringRarityWithColor(string content, RarityType nameRarirty)
    {
        switch ((int)nameRarirty)
        {
            case 0:
                return "<color=#0000ff>" + content + "</color>";
            //return UtilityGame.Format("{0}% {1}", value, Localize.GetLocalizedString("common"));
            case 1:
                return "<color=#00ff00>" + content + "</color>";
            case 2:
                return "<color=#ffff00>" + content + "</color>";
            case 3:
                return "<color=#00ffff>" + content + "</color>";
            case 4:
                return "<color=#ff0000>" + content + "</color>";
            default:
                return "";
        }
    }

    public static string GetStringStatWithColor(string content, StatInfo stateInfoType)
    {
        switch (stateInfoType)
        {
            case StatInfo.Level:
                return Format("<color={1}>{0}</color>", content, "#00FF33");
            case StatInfo.GloryName:
                return Format("<color={1}>{0}</color>", content, "#FF1C00");
            case StatInfo.Damage:
                return Format("<color={1}>{0}</color>", content, "#FF8400");
            case StatInfo.HP:
                return Format("<color={1}>{0}</color>", content, "#00B5FF");
            case StatInfo.EvolutionLevel:
                return Format("<color={1}>{0}</color>", content, "#FFFF00");
            default:
                return "";
        }
    }

    public static Color GetColorRarity(RarityType rarity)
    {
        return HexToColor(GetStringColorRarity(rarity));
    }

    public static Color GetColorRarity(int rarity)
    {
        return HexToColor(GetStringColorRarity(rarity));
    }

    public static Color HexToColor(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }

  
    public static string GetSpriteNameResources(int id)
    {
        string result = string.Empty;
        switch (id)
        {
            case 0:
                result = "small_gold";
                break;
            case 1:
                result = "gem_3";
                break;
            case 3:
                result = "nomal_stamina";
                break;
            case 2:
                result = "normal_skip";
                break;
            case 5:
                result = "coin_normal";
                break;
            case 6:
                result = "coin_vip";
                break;
            //case TypeResources.PowerUp:
            //    result = "power_up";
            //    break;
            case 8:
                result = "bonusexp";
                break;
            case 4:
                result = "ladder_point";
                break;
        }
        return result;
    }

    public static string TimeSpanToString(TimeSpan timeSpan)
    {
        return UtilityGame.Format("{0:D2}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
    }

    public static String Format(String format, params object[] args)
    {
        try
        {
            return string.Format(format, args);
        }
        catch
        {
            return "Error Content!";
        }
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Sin(radian), Mathf.Cos(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    #region FORMAT MONEY
    public static string formatMoney(string t)
    {
        if (t.Length < 4) return t;
        var rs = "";

        var count = t.Length;
        for (var i = count - 1; i > 0; i--)
            if ((count - i) % 3 != 0)
                rs = t[i] + rs;
            else
                rs = "," + t[i] + rs;
        return t[0] + rs;
    }

    //public static string FormatMoney(object t, bool needSmallerThanTen = false)
    //{
    //    if (!needSmallerThanTen) return string.Format("{0:0,0}", t);
    //    return (int)t < 10 ? t.ToString() : string.Format("{0:0,0}", t);
    //}

    public static string formatNickName(string t, int characterNum)
    {
        var tmp = t;
        if (t.Length > characterNum) tmp = t.Substring(0, characterNum) + "...";
        return tmp;
    }

    public static long formatMoneyBack(string t)
    {
        var tmp = t.Split(',');
        var str = "";
        foreach (var mstr in tmp)
            str += mstr;
        return long.Parse(str);
    }

    public static string formatMoneyK(float t, bool isSpace = true)
    {
        var m = (int)t;
        if (t < 1000)
            return formatMoney(m.ToString());
        if (t < 1000000)
            return string.Format("{0:0}", t / 1000) + (isSpace ? " K" : "K");
        if (t < 1000000000)
            return string.Format("{0:0}", t / 1000000) + (isSpace ? " M" : "M");
        return "";
    }

    public static string formatMoneyAuto(float t, bool isSpace = true)
    {
        var m = (int)t;
        if (t < 1000)
            return formatMoney(m.ToString());
        if (t < 1000000)
            return string.Format("{0:0}", t / 1000) + (isSpace ? " K" : "K");
        if (t < 1000000000)
            return string.Format("{0:0}", t / 1000000) + (isSpace ? " M" : "M");
        if (t < 1000000000000)
            return string.Format("{0:0}", t / 1000000000) + (isSpace ? " B" : "B");
        return "";
    }

    public static string formatMoneyD(float money, bool isSpace = false)
    {
        var m = (int)money;
        if (money < 1000)
            return formatMoney(m.ToString());
        if (money < 1000000000)
            return string.Format("{0:0,0}", money / 1000) + (isSpace ? " K" : "K");
        //if (money < 1000000000)
        //{
        //    return String.Format("{0:0}", money / 1000000) + (isSpace ? " M" : "M");
        //}
        return "";
    }

    public static System.Collections.IEnumerator IEChangeNumber(UnityEngine.UI.Text txt, int fromNum, int toNum, float tweenTime = 3,
        float scaleNum = 1.5f, float delay = 0)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        var i = 0.0f;
        var rate = 2.0f / tweenTime;
        txt.transform.DOScale(scaleNum, tweenTime);
        while (i < tweenTime)
        {
            i += Time.deltaTime * rate;
            var a = Mathf.Lerp(fromNum, toNum, i);
            //txt.text = a > 0 ? formatMoney(a.ToString()) : "0";
            //txt.text = a > 0 ? string.Format("{0:0,0}", a) : "0";
            txt.text = a > 0 ? formatMoney(Mathf.RoundToInt(a).ToString()): "0";
            if (a == toNum) i = tweenTime;
            yield return null;
        }
        //txt.transform.localScale = Vector2.one;
        yield return new WaitForSeconds(.05f);
    }
    #endregion
}

public class ValueChecking
{

    public static int CheckedAdd(int origin, int add)
    {
        if (origin < 0)
            origin = 0;

        int z = 0;
        try
        {
            // The following line raises an exception because it is checked.
            z = checked(origin + add);
        }
        catch (Exception e)
        {
            z = int.MaxValue;
            //Log.Error(e);
        }
        // The value of z is still 0.
        return z;
    }

    // Using a checked expression.
    public static long CheckedAdd(long origin, int add)
    {
        if (origin < 0)
            origin = 0;

        long z = 0;
        try
        {
            // The following line raises an exception because it is checked.
            z = checked(origin + add);
        }
        catch (Exception e)
        {
            z = long.MaxValue;
            Log.Error(e);
        }
        // The value of z is still 0.
        return z;
    }

    public static long CheckedAdd(long origin, long add)
    {
        if (origin < 0)
            origin = 0;

        long z = 0;
        try
        {
            // The following line raises an exception because it is checked.
            z = checked(origin + add);
        }
        catch (Exception e)
        {
            z = long.MaxValue;
            Log.Error(e);
        }
        // The value of z is still 0.
        return z;
    }

    public static float CheckedAdd(float origin, float add)
    {
        if (origin < 0)
            origin = 0;

        float z = 0;
        try
        {
            // The following line raises an exception because it is checked.
            z = checked(origin + add);
        }
        catch (Exception e)
        {
            z = float.MaxValue;
            Log.Error(e);
        }
        // The value of z is still 0.
        return z;
    }

    public static long CheckedMulti(long origin, int multi)
    {
        if (origin < 0)
            origin = 0;

        long z = 0;
        try
        {
            // The following line raises an exception because it is checked.
            z = checked(origin * multi);
        }
        catch (Exception e)
        {
            z = long.MaxValue;
            Log.Error(e);
        }
        // The value of z is still 0.
        return z;
    }

    public static float CheckedMulti(float origin, float multi)
    {
        if (origin < 0)
            origin = 0;

        float z = 0;
        try
        {
            // The following line raises an exception because it is checked.
            z = checked(origin * multi);
        }
        catch (Exception e)
        {
            z = float.MaxValue;
            Log.Error(e);
        }
        // The value of z is still 0.
        return z;
    }
}

[System.Serializable]
public class ContainNumber
{
    public int index;
    public int belowNumber;
    public int aboveNumber;

    public ContainNumber(int index,int below, int above)
    {
        this.index = index;
        this.belowNumber = below + 1;
        this.aboveNumber = below + above;
    }

    public bool IsContainNumber(int value)
    {
        if (value >= belowNumber && value <= aboveNumber)
            return true;
        return false;
    }
}