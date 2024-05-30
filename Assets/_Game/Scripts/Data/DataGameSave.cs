using SteveRogers;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Hellmade.Sound;
using Newtonsoft.Json;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;
using static DataGameServer;
using System.Collections;
using Facebook.Unity;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Converters;
using UnityEngine.SignInWithApple;

public static class MetaDataKey {
    public static string SKIN_PIECE = "skin_piece";
    public static string SKIN_LEVEL = "skin_level";
    public static string MAIL_READ = "mailread";
    public static string FULL_SKIN_RECEIVED = "full_skin_received";
    public static string GIFT_LETTER = "gift_letter";
    public static string AUTO_SHOOT_COUNT = "autoshoot_count";
    public static string AUTO_SHOOT_DAYCODE = "autoshoot_daycode";
    public static string AUTO_RESTORE_TIME = "auto_restore_time";
    public static string METEOR_BELT_VISIT_COUNT_1 = "mb_1";
    public static string METEOR_BELT_VISIT_COUNT_2 = "mb_2";
    public static string METEOR_BELT_VISIT_COUNT_3 = "mb_3";
    public static string METEOR_BELT_DAYCODE = "mb_daycode";

    public static string SHOP_GET_DIAMOND_COUNT = "s_diamond";
    public static string SHOP_GET_DIAMOND_DAYCODE = "s_diamond_c";

    public static string SHOP_AUTO_RESTORE_COUNT = "s_restore";
    public static string SHOP_AUTO_RESTORE_DAYCODE = "s_restore_c";

    public static string SHOP_TOTAL_ATTACK_COUNT = "s_total";
    public static string SHOP_TOTAL_ATTACK_DAYCODE = "s_total_c";

    public static string BATTLE_PASS_WIN_COUNT = "bp_win";
    public static string BATTLE_PASS_PRIZE_RECEIVED_NORMAL = "bp_nor";
    public static string BATTLE_PASS_PRIZE_RECEIVED_BATTLE_PASS = "bp_bat";

    public static string BATTLE_PASS_VIP_ENABLED = "bp_vip";

    public static string BATTLE_PASS_LAST_ENEMY_ID = "bp_id";
    public static string BATTLE_PASS_LAST_WON_ID = "bp_won";

    public static string RANK_LAST_DAYCODE = "r_dc";
    public static string RANK_LAST_RECEIVED_DAYCODE = "r_rc";

    public static string METEOR_FALL_WIN_1 = "mf_1";
    public static string METEOR_FALL_WIN_2 = "mf_2";
    public static string METEOR_FALL_WIN_3 = "mf_3";
    public static string METEOR_FALL_WIN_4 = "mf_4";
    public static string METEOR_FALL_WIN_MAX_COUNT = "mf_m";

    public static string METEOR_FALL_PRIZE_RECEIVED_NORMAL = "mf_p";
    public static string METEOR_FALL_PRIZE_RECEIVED_BATTLE = "mf_b";
    public static string METEOR_FALL_RESET_PRIZE_DAY_CODE = "mf_d";

    public static string X2_MATERIAL_END_TIME = "x2";
}

public static class RemoteConfigKey {
    public static String FULL_SKIN = "full_skin";
    public static String FREE_DIAMOND = "free_diamond";
    public static String VERSION = "version";
    public static String STEAL_MATERIAL_RATE_OFFLINE_COLLECT = "soff"; // 100% số vật chất offline 
    public static String STEAL_MATERIAL_RATE_TOTAL = "sto"; // 10% tổng số vật chất
    public static String SHOW_EVENT_BUTTON = "event";
}


public static class PlayerPrefsKey {
    public static string LAST_RANK_CHART_DATA;
    public static string METEOR_FALL_EVENT_ENABLED = "meteor_fall_event";
}

public static class DataGameSave {
    public static DataGameLogin dataLogin;
    public static DataGameServer dataServer;
    public static DataGameServer dataBattle; //only use when battle
    public static DataGameLocal dataLocal;
    public static DataGameServer dataEnemy;
    public static DataRank dataRank;

    public static List<OneSkinLevelUnlockData> listSkinLevelUnlockData = new List<OneSkinLevelUnlockData>();
    public static List<int> skinPieces;
    public static List<int> skinLevels;
    public static List<DataMail> mails;
    public static Dictionary<string, string> remoteConfig;

    public static DataGameServer randomEnemy;
    public static DataGameServer bossData;
    public static DataGameServer battlePassEnemy;
    public static Dictionary<string, string> metaData;
    public static List<DataGameServer> listDataEnemies;
    public static List<DataGameServer> listDataFriends;
    public static List<EventUserData> eventDatas;
    public static List<DataGameServer> listTopRank;

    public static long autoRestoreEndTime;
    public static long x2MaterialEndTime;
    public static int autoShootCount = GameManager.MAX_AUTO_SHOOT_COUNT;

    public static void CheckInitLocal() {
        LoadLocalProfileOrCreateNewIfNotExist();
        EazySoundManager.GlobalMusicVolume = dataLocal.musicVolume;
        EazySoundManager.GlobalSoundsVolume = dataLocal.soundVolume;
        EazySoundManager.GlobalUISoundsVolume = dataLocal.soundVolume;
    }

    public static void SaveToLocal() {
        if (dataLocal == null || dataLocal.lastLogin == null)
            return;

        dataLocal.lastLogin.dayLogin = GameManager.Now.Day;
        dataLocal.lastLogin.monthLogin = GameManager.Now.Month;
        dataLocal.lastLogin.yearLogin = GameManager.Now.Year;

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(dataLocal);
        string content = EncryptionHelper.Encrypt(json, true);
        FileHelper.SaveFileWithPassword(content, GetLocalFilePath(), "", true);
    }

    /// <summary>
    /// only save rank data when
    /// rank chart code equal today code (DataGameSave.GetDayCode(GameManager.Now))
    /// </summary>
    public static void SaveRankData() {
        if (dataServer == null)
            return;

        WWWForm form = new WWWForm();
        form.AddField("user_id", DataGameSave.dataServer.userid);

        ServerSystem.Instance.SendRequest(ServerConstants.GET_CURRENT_CHART_ID, form, () => {
            //TODO: fix save rank data

            if (ServerSystem.result != "") {
                int daycodeOfPlayerChart = int.Parse(ServerSystem.result);
                if (daycodeOfPlayerChart.ToString() == DataGameSave.GetDayCode(GameManager.Now)) {
                    WWWForm form1 = new WWWForm();

                    form1.AddField("user_id", dataServer.userid);
                    form1.AddField("destroyed_solars", dataLocal.destroyedSolars);
                    form1.AddField("destroy_planet", dataLocal.destroyPlanet);
                    form1.AddField("meteor_planet_hit_count", dataLocal.meteorPlanetHitCount);

                    ServerSystem.Instance.SendRequest(ServerConstants.SAVE_RANK_DATA, form1, null);
                }
            }
        });
    }

    static void LoadLocalProfileOrCreateNewIfNotExist() {
        if (!System.IO.File.Exists(GetLocalFilePath())) {
            dataLocal = new DataGameLocal();

            for (int i = 0; i < GameConstants.DAILY_QUEST_COUNT; i++) {
                dataLocal.dailyMissions.Add(new DailyQuestSave() {
                    id = i,
                    currentProgress = 0,
                    isReceived = false
                });
            }
            dataLocal.dailyMissions[0].currentProgress = 1;
            if (BtnDailyMission.Instance) {
                BtnDailyMission.Instance.CheckDoneQuest();
            }
            dataLocal.playFabToken = GetToken();
            SaveToLocal();
        } else {
            LoadLocalUserData();
        }
    }

    public static void ClearData() {
        dataLocal = new DataGameLocal();
        for (int i = 0; i < GameConstants.DAILY_QUEST_COUNT; i++) {
            dataLocal.dailyMissions.Add(new DailyQuestSave() {
                id = i,
                currentProgress = 0,
                isReceived = false
            });
        }
        dataLocal.dailyMissions[0].currentProgress = 1;
        if (BtnDailyMission.Instance) {
            BtnDailyMission.Instance.CheckDoneQuest();
        }
        FileHelper.DeleteFile(GetLocalFilePath(), true);
        SaveToLocal();
    }

    static void LoadLocalUserData() {
        string json = FileHelper.LoadFileWithPassword(GetLocalFilePath(), "", true);

        if (string.IsNullOrEmpty(json)) {
            Debug.LogError("Error");
            dataLocal = new DataGameLocal();
            for (int i = 0; i < GameConstants.DAILY_QUEST_COUNT; i++) {
                dataLocal.dailyMissions.Add(new DailyQuestSave() {
                    id = i,
                    currentProgress = 0,
                    isReceived = false
                });
            }
            dataLocal.dailyMissions[0].currentProgress = 1;
            if (BtnDailyMission.Instance) {
                BtnDailyMission.Instance.CheckDoneQuest();
            }
            dataLocal.playFabToken = GetToken();
        } else {
            var decrypJson = EncryptionHelper.Decrypt(json, true);
            DataGameLocal local = Newtonsoft.Json.JsonConvert.DeserializeObject<DataGameLocal>(decrypJson);

            if (local == null) {
                Debug.LogError("Error");
                dataLocal = new DataGameLocal();
                for (int i = 0; i < GameConstants.DAILY_QUEST_COUNT; i++) {
                    dataLocal.dailyMissions.Add(new DailyQuestSave() {
                        id = i,
                        currentProgress = 0,
                        isReceived = false
                    });
                }
                dataLocal.dailyMissions[0].currentProgress = 1;
                if (BtnDailyMission.Instance) {
                    BtnDailyMission.Instance.CheckDoneQuest();
                }
                dataLocal.playFabToken = GetToken();
            } else {
                dataLocal = local;
            }
        }
    }

    public static void SetStatus(string status) {

        WWWForm form = new WWWForm();
        form.AddField("user_id", DataGameSave.dataServer.userid);
        form.AddField("status", status);
        string url = ServerConstants.SET_STATUS;

        ServerSystem.Instance.SendRequest(url, form, null);
    }

    public static string GetServerFilePath() {
        return FileHelper.GetWritablePath(GameConstants.SERVER_DATA_FILE_NAME);
    }

    public static string GetLocalFilePath() {
        return FileHelper.GetWritablePath(GameConstants.LOCAL_DATA_FILE_NAME);
    }

    public static string GetDayCode(DateTime date) {
        string year = date.Year.ToString();

        string month = date.Month.ToString();
        if (date.Month < 10)
            month = "0" + month;

        string day = date.Day.ToString();
        if (date.Day < 10)
            day = "0" + day;

        return year + month + day;
    }

    public static void SaveToServer(Action doneAction = null, Action failAction = null) {
        if (dataServer == null || dataLogin == null) {
            doneAction.SafeCall();
            return;
        }

        //backup metadata in DataGameSave.playfabToken
        DataGameSave.SaveMetaData(MetaDataKey.SKIN_PIECE, JsonConvert.SerializeObject(DataGameSave.skinPieces));
        DataGameSave.SaveMetaData(MetaDataKey.SKIN_LEVEL, JsonConvert.SerializeObject(DataGameSave.skinLevels));
        DataGameSave.SaveMetaData(MetaDataKey.AUTO_RESTORE_TIME, DataGameSave.autoRestoreEndTime.ToString());
        DataGameSave.SaveMetaData(MetaDataKey.X2_MATERIAL_END_TIME, DataGameSave.x2MaterialEndTime.ToString());

        DataGameSave.dataServer.lastOnline = GameManager.Now;
        DataGameSave.dataLocal.playFabToken = JsonConvert.SerializeObject(DataGameSave.metaData);

        string url = ServerConstants.SAVE_UNIVERSE_DATA;
        string data = JsonConvert.SerializeObject(GetUniverseModel());
        WWWForm form = new WWWForm();
        form.AddField("data", data);

        ServerSystem.Instance.SendRequest(url, form, () => {
            //save  meta data in var s1 of table local
            form = new WWWForm();
            form.AddField("user_id", DataGameSave.dataServer.userid);
            form.AddField("json", JsonConvert.SerializeObject(DataGameSave.metaData));

            ServerSystem.Instance.SendRequest(ServerConstants.SAVE_META_DATA, form, null);

            if (GameManager.DEBUG_MODE) {
                ServerSystem.Instance.LogSql("select playFabToken from userdata where userid = " + DataGameSave.dataServer.userid, "save to server");
            }

            doneAction.SafeCall();
        }, failAction);
    }

    public static string GetToken() {
        string token = "";
        if (string.IsNullOrEmpty(dataLocal.playFabToken)) {
            System.Random rand = new System.Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+";
            token = new string(Enumerable.Repeat(chars, 20)
                              .Select(c => c[rand.Next(c.Length)]).ToArray());
        }

        return token;
    }

    public static string GetRankTier(int level) {
        if (level <= 10) {
            return "Planet_";
        } else if (level > 10 && level <= 20) {
            return "SolarSystem_";
        } else if (level > 20 && level <= 30) {
            return "Galaxy_";
        } else if (level > 30 && level <= 40) {
            return "Intergalactic_";
        } else {
            return "Universe_";
        }
    }

    public static void GetRankChartData() {
        Debug.Log(System.Environment.StackTrace);
        ServerSystem.ShowCurrentRankChart((result) => {
            DataGameSave.eventDatas = SteveRogers.Utilities.SafeJsonDeserializeObject<List<EventUserData>>(result);
        });
    }

    public static bool IsAllPlanetDestroyed() {
        for (int i = 0; i < dataServer.ListPlanet.Count; i++) {
            if (dataServer.ListPlanet[i].type != TypePlanet.Destroy) {
                return false;
            }
        }

        return true;
    }

    public static string GetGuestName(int userId) {
        string[] names = new string[] { "Moon", "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };
        int id = userId % names.Length;

        return names[id];
    }

    public static void SaveMetaData(string key, string value) {
        if (metaData == null)
            return;

        if (metaData.ContainsKey(key)) {
            metaData[key] = value;
        } else {
            metaData.Add(key, value);
        }
    }

    public static string GetMetaData(string key) {
        if (metaData.ContainsKey(key)) {
            return metaData[key];
        }
        return "";
    }

    public static T GetMetaDataObject<T>(string key) {
        if (!metaData.ContainsKey(key)) {
            return default(T);
        }
        T result = default(T);
        try {
            result = JsonConvert.DeserializeObject<T>(metaData[key]);
        } catch { }
        return result;
    }

    public static List<string> GetMailReads() {
        List<string> mailReads = new List<string>();

        if (!DataGameSave.metaData.ContainsKey(MetaDataKey.MAIL_READ)) {
            DataGameSave.SaveMetaData(MetaDataKey.MAIL_READ, JsonConvert.SerializeObject(mailReads));
            return new List<string>();
        } else {
            try {
                mailReads = JsonConvert.DeserializeObject<List<string>>(DataGameSave.metaData[MetaDataKey.MAIL_READ]);
                return mailReads;
            } catch {
                return new List<string>();
            }
        }
    }

    public static void GetRandomEnemy() {
        WWWForm form = new WWWForm();
        form.AddField("userid", DataGameSave.dataServer.userid);
        form.AddField("token", DataGameSave.dataServer.token);
        form.AddField("level", DataGameSave.dataServer.level);

        ServerSystem.Instance.SendRequest(ServerConstants.FIND_ONE_ENEMY, form, () => {
            try {
                Hashtable result = new Hashtable();
                result = JsonConvert.DeserializeObject<Hashtable>(ServerSystem.result);

                if (result["result"].ToString() == "success") {
                    DataGameServer enemyData = JsonConvert.DeserializeObject<DataGameServer>(result["data"].ToString());
                    DataGameSave.randomEnemy = enemyData;
                } else {
                    throw new Exception();
                }
            } catch {
                DataGameServer enemyData = new DataGameServer();
                enemyData.Name = "Sun";
                int randomRange = new System.Random().Next(-3, 3);
                enemyData.level = (enemyData.level - randomRange > 0) ? enemyData.level - randomRange : 1;
                enemyData.ListPlanet = DataGameSave.dataServer.ListPlanet;
                DataGameSave.randomEnemy = enemyData;
            }
        });
    }

    public static void GetBattlePassEnemy() {
        // check last enemy
        string lastEnemey = DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_LAST_ENEMY_ID);
        string lastWon = DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_LAST_WON_ID);
        if (lastEnemey != null && lastEnemey != "" && lastWon != lastEnemey) {
            GetEnemyData(int.Parse(DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_LAST_ENEMY_ID)), (enemyData) => {
                DataGameSave.battlePassEnemy = enemyData;
            });
            return;
        }

        // find new enemy
        WWWForm form = new WWWForm();
        form.AddField("userid", DataGameSave.dataServer.userid);
        form.AddField("low", DataGameSave.dataServer.level);
        form.AddField("high", DataGameSave.dataServer.level + 2);

        ServerSystem.Instance.SendRequest(ServerConstants.FIND_ENEMY_IN_LEVEL_RANGE, form, () => {
            try {
                Hashtable result = new Hashtable();
                result = JsonConvert.DeserializeObject<Hashtable>(ServerSystem.result);

                if (result["result"].ToString() == "success") {
                    DataGameServer enemyData = JsonConvert.DeserializeObject<DataGameServer>(result["data"].ToString());
                    DataGameSave.battlePassEnemy = enemyData;
                } else {
                    throw new Exception();
                }
            } catch {
                DataGameServer enemyData = new DataGameServer();
                enemyData.Name = "Sun";
                int randomRange = new System.Random().Next(-3, 3);
                enemyData.level = (enemyData.level - randomRange > 0) ? enemyData.level - randomRange : 1;
                enemyData.ListPlanet = DataGameSave.dataServer.ListPlanet;
                DataGameSave.battlePassEnemy = enemyData;
            }

            DataGameSave.SaveMetaData(MetaDataKey.BATTLE_PASS_LAST_ENEMY_ID, DataGameSave.battlePassEnemy.userid.ToString());
        });
    }

    public static void GetBossData() {
        string url = ServerConstants.GET_REVENGE_ENEMY_DATA;

        WWWForm form = new WWWForm();
        form.AddField("userid", Convert.ToInt32(DataGameSave.dataLogin.userid.ToString()));
        form.AddField("token", DataGameSave.dataLogin.token.ToString());
        form.AddField("enemyid", GameManager.BOSS_USER_ID.ToString());

        ServerSystem.Instance.SendRequest(url, form, () => {
            if (!ServerSystem.Instance.IsResponseOK())
            {
                //PopupConfirm.ShowOK("Oops", TextMan.Get("Search not found"), "OK");
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

                    dataGameServer.sunHp = GameManager.BOSS_HP;
                    DataGameSave.bossData = dataGameServer;
                }
                catch
                {
                    //PopupConfirm.ShowOK("Oops", TextMan.Get("Search not found"), "OK");
                }
            }
        });
    }

    public static void GetEnemyData(int enmeyId, Action<DataGameServer> doneAction) {
        string url = ServerConstants.GET_REVENGE_ENEMY_DATA;

        WWWForm form = new WWWForm();
        form.AddField("userid", Convert.ToInt32(DataGameSave.dataLogin.userid.ToString()));
        form.AddField("token", DataGameSave.dataLogin.token.ToString());
        form.AddField("enemyid", enmeyId.ToString());

        ServerSystem.Instance.SendRequest(url, form, () => {
            if (!ServerSystem.Instance.IsResponseOK()) {
                PopupConfirm.ShowOK("Oops", "Something wrong with yout enemy", "OK");
            } else {
                try {
                    string jsonData = ServerSystem.Instance.ReadData();

                    DataGameServer dataGameServer = JsonConvert.DeserializeObject<DataGameServer>(jsonData,
                        new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

                    if (dataGameServer.ListEnemy == null) {
                        dataGameServer.ListEnemy = new List<string>();
                    }

                    if (dataGameServer.ListPlanet == null) {
                        dataGameServer.ListPlanet = new List<DataPlanet>();
                    }

                    if (dataGameServer.ListPlanet.Count == 0) {
                        dataGameServer.ListPlanet.Add(new DataPlanet());
                    }

                    doneAction.Invoke(dataGameServer);
                } catch { }
            }
        });
    }

    public static void GetListNeightborEnemy() {
        WWWForm form = new WWWForm();
        form.AddField("userid", DataGameSave.dataLogin.userid);
        form.AddField("token", DataGameSave.dataLogin.token.ToString());
        form.AddField("level", DataGameSave.dataServer.level);

        ServerSystem.Instance.SendRequest(ServerConstants.FIND_EIGHT_ENEMY, form, () => {
            if (ServerSystem.Instance.IsResponseOK()) {
                string data = ServerSystem.Instance.ReadData();
                List<DataGameServer> list = JsonConvert.DeserializeObject<List<DataGameServer>>(data);
                DataGameSave.listDataEnemies = list;
            }
        });
    }

    public static void GetTopRank() {
        WWWForm form = new WWWForm();
        form.AddField("userid", DataGameSave.dataLogin.userid);
        form.AddField("token", DataGameSave.dataLogin.token.ToString());
        form.AddField("level", DataGameSave.dataServer.level);

        ServerSystem.Instance.SendRequest(ServerConstants.GET_TOP_RANK, form, () => {
            if (ServerSystem.Instance.IsResponseOK())
            {
                string data = ServerSystem.Instance.ReadData();
                List<DataGameServer> list = JsonConvert.DeserializeObject<List<DataGameServer>>(data);
                DataGameSave.listTopRank = list;
            }
        });
    }

    public static void LoginFacebookButtonClick() {
        if (DataGameSave.dataLogin.facebookid != "" && DataGameSave.dataLogin.facebookid != null) {
            PopupConfirm.ShowOK("Oops!", TextMan.Get("Seem like you're already logged in!"));
            return;
        }
        Debug.Log("Check FB inited");

        if (FB.IsInitialized) {
            LogInFacebook();
        } else {
            FB.Init(() => {
                LogInFacebook();
            });
        }
    }

    public static void LoginAppleButtonClick(SignInWithApple signInWithApple)
    {
        if (DataGameSave.dataLogin.googleid != "" && DataGameSave.dataLogin.googleid != null)
        {
            PopupConfirm.ShowOK("Oops!", TextMan.Get("Seem like you're already logged in!"));
            return;
        }
        Debug.Log("Check FB inited");

        LogInApple(signInWithApple);
        // Code login here
    }

    public static void LogOutFacebook() {
        DataGameSave.dataServer.facebookid = "";
        DataGameSave.SetStatus("deleted");

        DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Normal;
        DataGameSave.dataLogin.facebookid = "";
        DataGameSave.dataLogin.userid = DataGameSave.dataServer.userid;
        DataGameSave.dataLogin.token = DataGameSave.dataServer.token;
        DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);

        PopupConfirm.ShowOK(TextMan.Get("Information"), "Log out ");
    }

    public static void LogOutApple() {
        DataGameSave.dataServer.googleid = "";
        DataGameSave.SetStatus("deleted");

        DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Normal;
        DataGameSave.dataLogin.googleid = "";
        DataGameSave.dataLogin.userid = DataGameSave.dataServer.userid;
        DataGameSave.dataLogin.token = DataGameSave.dataServer.token;
        DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);

        PopupConfirm.ShowOK(TextMan.Get("Information"), "Log out ");
    }

    public static void LogInFacebook() {
        Debug.Log("Login FB");
        Facebook.Unity.FB.LogInWithReadPermissions(new string[] { "public_profile", "email" }, (res) => {
            if (string.IsNullOrEmpty(res.Error)) {
                string facebookid = "";
                res.ResultDictionary.TryGetValue("user_id", out facebookid);

                Debug.Log("Login FB success: fbid = " + facebookid);

                if (facebookid == "") {
                    return;
                }

                WWWForm form = new WWWForm();
                form.AddField("facebookid", facebookid.ToString());

                Debug.Log("Sending FB id to server = " + facebookid);
                ServerSystem.Instance.SendRequest(ServerConstants.GET_UNIVERSE_DATA_BY_FACEBOOK_ID, form, () => {
                    Debug.Log("Send FB id to server success = " + facebookid);
                    if (!ServerSystem.Instance.IsResponseOK()) {
                        DataGameSave.dataServer.facebookid = facebookid.ToString();
                        DataGameSave.SaveToServer();

                        DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Facebook;
                        DataGameSave.dataLogin.facebookid = facebookid;
                        DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);

                        PopupConfirm.ShowOK(TextMan.Get("Success"), TextMan.Get("Data saved to your facebook account"));
                    } else {
                        UniverseModel model = JsonConvert.DeserializeObject<UniverseModel>(ServerSystem.Instance.ReadData());
                        DataGameServer server = JsonConvert.DeserializeObject<DataGameServer>(model.jsonserver);
                        DataGameLocal local = JsonConvert.DeserializeObject<DataGameLocal>(model.jsonlocal);

                        if (model.userid == DataGameSave.dataServer.userid) {
                            PopupConfirm.ShowOK("Oops!", TextMan.Get("Seem like you're already logged in!"));
                        } else {
                            string question = TextMan.Get("There is another data with this acount \nName: {0} \nLevel: {1} \nWhich one do you want to keep?");
                            PopupConfirm.ShowYesNo(
                                TextMan.Get(string.Format("{0} level {1}", server.Name, server.level)),
                                String.Format(question, server.Name, server.level),
                                () => {
                                    KeepThisData(facebookid, model, DataGameLogin.LoginType.Facebook);
                                }, TextMan.Get(string.Format("{0} level {1}", DataGameSave.dataServer.Name, DataGameSave.dataServer.level)),
                                () => {
                                    KeepOtherData(facebookid, model, DataGameLogin.LoginType.Facebook);
                                },
                                TextMan.Get(string.Format("{0} level {1}", server.Name, server.level)));
                        }
                    }
                });
            } else {
                Debug.Log("Login FB Error: " + res.Error);
            }
        });
    }

    public static void LogInApple(SignInWithApple signInWithApple)
    {
        Debug.Log("Login Apple");
        signInWithApple.Login(args =>
        {
            if (args.error != null)
            {
                Debug.Log("Sign in with apple errors occurred: " + args.error);
                return;
            }

            UserInfo userInfo = args.userInfo;

            var appleId = userInfo.userId;
            // Save the userId so we can use it later for other operations.

            // Print out information about the user who logged in.
            Debug.Log(
                string.Format("Display Name: {0}\nEmail: {1}\nUser ID: {2}\nID Token: {3}", userInfo.displayName ?? "",
                    userInfo.email ?? "", userInfo.userId ?? "", userInfo.idToken ?? ""));

            WWWForm form = new WWWForm();
            form.AddField("googleid", appleId.ToString());

            Debug.Log("Sending Apple id to server = " + appleId);
            ServerSystem.Instance.SendRequest(ServerConstants.GET_UNIVERSE_DATA_BY_APPLE_ID, form, () => {
                Debug.Log("Send FB id to server success = " + appleId);
                if (!ServerSystem.Instance.IsResponseOK())
                {
                    DataGameSave.dataServer.googleid = appleId;
                    DataGameSave.SaveToServer();

                    DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Facebook;
                    DataGameSave.dataLogin.googleid = appleId;
                    DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);

                    PopupConfirm.ShowOK(TextMan.Get("Success"), TextMan.Get("Data saved to your facebook account"));
                }
                else
                {
                    UniverseModel model = JsonConvert.DeserializeObject<UniverseModel>(ServerSystem.Instance.ReadData());
                    DataGameServer server = JsonConvert.DeserializeObject<DataGameServer>(model.jsonserver);
                    DataGameLocal local = JsonConvert.DeserializeObject<DataGameLocal>(model.jsonlocal);

                    if (model.userid == DataGameSave.dataServer.userid)
                    {
                        PopupConfirm.ShowOK("Oops!", TextMan.Get("Seem like you're already logged in!"));
                    }
                    else
                    {
                        string question = TextMan.Get("There is another data with this acount \nName: {0} \nLevel: {1} \nWhich one do you want to keep?");
                        PopupConfirm.ShowYesNo(
                            TextMan.Get(string.Format("{0} level {1}", server.Name, server.level)),
                            String.Format(question, server.Name, server.level),
                            () => {
                                KeepThisData(appleId, model, DataGameLogin.LoginType.Apple);
                            }, TextMan.Get(string.Format("{0} level {1}", DataGameSave.dataServer.Name, DataGameSave.dataServer.level)),
                            () => {
                                KeepOtherData(appleId, model, DataGameLogin.LoginType.Apple);
                            },
                            TextMan.Get(string.Format("{0} level {1}", server.Name, server.level)));
                    }
                }
            });
        });
    }

    static void KeepThisData(string externalId, UniverseModel other, DataGameLogin.LoginType loginType) {
        PopupConfirm.ShowYesNo(TextMan.Get("Information"),
            TextMan.Get(string.Format("Data {0} level {1} will be deleted, is it OK?", other.name, other.level)),
            () => {

                WWWForm form = new WWWForm();
                form.AddField("user_id", other.userid);
                form.AddField("status", "deleted");
                ServerSystem.Instance.SendRequest(ServerConstants.SET_STATUS, form, () => {
                    if (loginType == DataGameLogin.LoginType.Facebook)
                    {
                        DataGameSave.dataServer.facebookid = externalId;
                        DataGameSave.SaveToServer(() => {
                            DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Facebook;
                            DataGameSave.dataLogin.facebookid = externalId;
                            DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);
                            PopupConfirm.ShowOK(TextMan.Get("Overwrite"), TextMan.Get("Overwrited your data in facebook account by this data"), TextMan.Get("OK"), () => {
                                GameObject[] singletons = GameObject.FindGameObjectsWithTag("DontDestroyOnLoad");
                                foreach (GameObject singleton in singletons)
                                {
                                    UnityEngine.Object.Destroy(singleton);
                                }
                                SceneManager.LoadScene(SceneName.CustomLoading.ToString());
                            });
                        });
                    }
                    else if (loginType == DataGameLogin.LoginType.Apple)
                    {
                        DataGameSave.dataServer.googleid = externalId;
                        DataGameSave.SaveToServer(() => {
                            DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Apple;
                            DataGameSave.dataLogin.googleid = externalId;
                            DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);
                            PopupConfirm.ShowOK(TextMan.Get("Overwrite"), TextMan.Get("Overwrited your data in facebook account by this data"), TextMan.Get("OK"), () => {
                                GameObject[] singletons = GameObject.FindGameObjectsWithTag("DontDestroyOnLoad");
                                foreach (GameObject singleton in singletons)
                                {
                                    UnityEngine.Object.Destroy(singleton);
                                }
                                SceneManager.LoadScene(SceneName.CustomLoading.ToString());
                            });
                        });
                    }
                });

            });
    }

    static void KeepOtherData(string externalId, UniverseModel other, DataGameLogin.LoginType loginType) {

        PopupConfirm.ShowYesNo(TextMan.Get("Information"),
            TextMan.Get(string.Format("Data {0} level {1} will be deleted, is it OK?", DataGameSave.dataServer.Name, DataGameSave.dataServer.level)),
            () => {

                if (loginType == DataGameLogin.LoginType.Facebook)
                {
                    DataGameSave.dataServer.facebookid = externalId.ToString();
                    DataGameSave.SetStatus("deleted");

                    DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Facebook;
                    DataGameSave.dataLogin.facebookid = externalId;
                    DataGameSave.dataLogin.userid = other.userid;
                    DataGameSave.dataLogin.token = other.token;
                    DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);

                    PopupConfirm.ShowOK(TextMan.Get("Loaded data"), TextMan.Get("Loaded data from your facebook account"), TextMan.Get("OK"), () => {
                        GameObject[] singletons = GameObject.FindGameObjectsWithTag("DontDestroyOnLoad");
                        foreach (GameObject singleton in singletons)
                        {
                            UnityEngine.Object.Destroy(singleton);
                        }
                        SceneManager.LoadScene(SceneName.CustomLoading.ToString());
                    });
                }
                else
                {
                    DataGameSave.dataServer.googleid = externalId.ToString();
                    DataGameSave.SetStatus("deleted");

                    DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Apple;
                    DataGameSave.dataLogin.googleid = externalId;
                    DataGameSave.dataLogin.userid = other.userid;
                    DataGameSave.dataLogin.token = other.token;
                    DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);

                    PopupConfirm.ShowOK(TextMan.Get("Loaded data"), TextMan.Get("Loaded data from your facebook account"), TextMan.Get("OK"), () => {
                        GameObject[] singletons = GameObject.FindGameObjectsWithTag("DontDestroyOnLoad");
                        foreach (GameObject singleton in singletons)
                        {
                            UnityEngine.Object.Destroy(singleton);
                        }
                        SceneManager.LoadScene(SceneName.CustomLoading.ToString());
                    });
                }
                
            });
    }

    public static void LogoutFacebook() {
    }

    public static void SaveAttackedInfo(DataGameServer attacker, DataGameServer victim) {
        if (attacker == null)
            return;

        int materialLost = (int)(victim.GetAllMaterialCollect() * GameManager.STEAL_MATERIAL_RATE_OFFLINE_COLLECT);
        materialLost += (int) (victim.MaterialCollect * GameManager.STEAL_MATERIAL_RATE_TOTAL);
        //List<AttackedInfoData> currentarr = GetAttackedInfo();

        //if (currentarr == null)
        //    currentarr = new List<AttackedInfoData>();

        AttackedInfoData attackedData = new AttackedInfoData();
        attackedData.id = attacker.userid;
        attackedData.name = attacker.Name;
        attackedData.mat = materialLost;

        //currentarr.Add(new AttackedInfoData
        //{
        //    id = attacker.userid,
        //    name = attacker.Name,
        //    mat = (int)attacker.MaterialCollect
        //});

        //string isAttackedCode = Newtonsoft.Json.JsonConvert.SerializeObject(attackedData);

        WWWForm form = new WWWForm();
        form.AddField("user_id", victim.userid);
        form.AddField("id", attacker.userid);
        form.AddField("name", attacker.Name);
        form.AddField("mat", (int)attacker.MaterialCollect);

        ServerSystem.Instance.SendRequest(ServerConstants.UPDATE_IS_ATTACKED_CODE, form, () => {
            //Debug.Log(ServerSystem.result);
        });
    }

    public static void RewardFirstRank() {
        int rand = UnityEngine.Random.Range(0, 100);
        if (rand < 60)
        {
            // +60 % random 200 đá hố đen
            DataReward reward = PopupShop.GetRewardRandomEffectStones(200);
            GameManager.reward = reward;

            PopupMeteorResult.Show("Congratulation", "Return", reward, okFunction: () => {
                DataGameSave.dataLocal.M_Material += reward.material;
                DataGameSave.dataLocal.M_AirStone += reward.air;
                DataGameSave.dataLocal.M_AntimatterStone += reward.antimater;
                DataGameSave.dataLocal.M_FireStone += reward.fire;
                DataGameSave.dataLocal.M_GravityStone += reward.gravity;
                DataGameSave.dataLocal.M_IceStone += reward.ice;
                DataGameSave.dataLocal.Diamond += reward.diamond;
                DataGameSave.dataLocal.M_ToyStone1 += reward.toy1;
                DataGameSave.dataLocal.M_ToyStone2 += reward.toy2;
                DataGameSave.dataLocal.M_ToyStone3 += reward.toy3;
                DataGameSave.dataLocal.M_ToyStone4 += reward.toy4;
                DataGameSave.dataLocal.M_ToyStone5 += reward.toy5;

                DataGameSave.dataServer.MaterialCollect += reward.material;

                DataGameSave.SaveToLocal();
                DataGameSave.SaveToServer();
            });
        }
        else if (rand < 70)
        {
            // 1 skin hành tinh màu cam
            SkinDataReader.TryBuyRandomSkinPlanet(1, 0, 4);
        }
        else if (rand < 80)
        {
            // + 10 % 3 skin hành tinh màu cam
            SkinDataReader.TryBuyRandomSkinPlanet(3, 0, 4);
        }
        else if (rand < 85)
        {
            // + 5 % 10000 kc
            DataReward reward = new DataReward
            {
                diamond = 1000
            };

            GameManager.reward = reward;
            PopupMeteorResult.Show(reward: reward);

            DataGameSave.dataLocal.Diamond += 1000;
            DataGameSave.SaveToServer();
        }
        else if (rand < 90)
        {
            // + 5 % hồi hành tinh trong 24h
            string message = string.Format(TextMan.Get("You activated auto restore for {0} mins."), 24 * 60);
            PopupConfirm.ShowOK(TextMan.Get("Congratulations"), message, "Great", () => {
                if (!GameManager.IsAutoRestorePlanet)
                {
                    DataGameSave.autoRestoreEndTime = GameManager.Now.Ticks;
                }

                DateTime newEndTime = new DateTime(DataGameSave.autoRestoreEndTime).AddMinutes(24 * 60);
                DataGameSave.autoRestoreEndTime = newEndTime.Ticks;
                DataGameSave.SaveToServer();
            });
        }
        else if (rand < 95)
        {
            // + 5 % 200 lần bắn full attack
            PopupConfirm.ShowOK(TextMan.Get("Congratulations"), string.Format(TextMan.Get("Received {0} times Auto Shoot"), 200), "Great", () => {
                DataGameSave.autoShootCount += 200;
                DataGameSave.SaveToServer();
            });
            DataGameSave.SaveToServer();
        }
        else
        {
            // + 5 % 500 đá hố đen
            DataReward reward = PopupShop.GetRewardRandomEffectStones(500);
            GameManager.reward = reward;

            PopupMeteorResult.Show("Congratulation", "Return", reward, okFunction: () => {
                DataGameSave.dataLocal.M_Material += reward.material;
                DataGameSave.dataLocal.M_AirStone += reward.air;
                DataGameSave.dataLocal.M_AntimatterStone += reward.antimater;
                DataGameSave.dataLocal.M_FireStone += reward.fire;
                DataGameSave.dataLocal.M_GravityStone += reward.gravity;
                DataGameSave.dataLocal.M_IceStone += reward.ice;
                DataGameSave.dataLocal.Diamond += reward.diamond;
                DataGameSave.dataLocal.M_ToyStone1 += reward.toy1;
                DataGameSave.dataLocal.M_ToyStone2 += reward.toy2;
                DataGameSave.dataLocal.M_ToyStone3 += reward.toy3;
                DataGameSave.dataLocal.M_ToyStone4 += reward.toy4;
                DataGameSave.dataLocal.M_ToyStone5 += reward.toy5;

                DataGameSave.dataServer.MaterialCollect += reward.material;

                DataGameSave.SaveToLocal();
                DataGameSave.SaveToServer();
            });
        }

        DataGameSave.eventDatas = null;
        DataGameSave.SaveToServer();
        DataGameSave.GetRankChartData();
        EventScroller.Instance.Refresh();
    }

    public static UniverseModel GetUniverseModel() {
        UniverseModel model = new UniverseModel();

        //local
        model.userid = dataServer.userid;
        model.jsonserver = JsonConvert.SerializeObject(dataServer);
        model.jsonlocal = JsonConvert.SerializeObject(dataLocal);
        model.diamond = dataLocal.diamond;
        model.destroysolarbyonehit = dataLocal.destroySolarByOneHit;
        model.destroyedsolars = dataLocal.destroyedSolars;
        model.randommissionreward = dataLocal.randomMissionReward;

        model.meteorplanethitcount = dataLocal.meteorPlanetHitCount;
        model.meteorspecialplanethitcount = dataLocal.meteorSpecialPlanetHitCount;
        model.meteormulticolorhitcount = dataLocal.meteorMultiColorHitCount;

        model.m_airstone = dataLocal.M_AirStone;
        model.m_firestone = dataLocal.M_FireStone;
        model.m_icestone = dataLocal.M_IceStone;
        model.m_gravitystone = dataLocal.M_GravityStone;
        model.m_antimatterstone = dataLocal.M_Antimatter;
        model.m_colorfulstone = dataLocal.M_ColorfulStone;

        model.m_material = dataLocal.M_Material;
        model.m_air = dataLocal.M_Air;
        model.m_fire = dataLocal.M_Fire;
        model.m_ice = dataLocal.M_Ice;
        model.m_gravity = dataLocal.M_Gravity;
        model.m_antimatter = dataLocal.M_Antimatter;

        //server
        model.level = dataServer.level;
        model.materialcollect = dataServer.MaterialCollect;
        model.listenemy = JsonConvert.SerializeObject(dataServer.ListEnemy);
        model.name = dataServer.Name;
        model.token = dataServer.token;
        model.facebookid = dataServer.facebookid;
        model.googleid = dataServer.googleid;

        return model;
    }

    public static void ReadUniverseModel(UniverseModel model) {
        dataServer = JsonConvert.DeserializeObject<DataGameServer>(model.jsonserver);
        dataLocal = JsonConvert.DeserializeObject<DataGameLocal>(model.jsonlocal);

        dataLocal.Diamond = model.diamond;
        dataLocal.destroySolarByOneHit = model.destroysolarbyonehit;
        dataLocal.destroyedSolars = model.destroyedsolars;
        dataLocal.randomMissionReward = model.randommissionreward;

        dataLocal.meteorPlanetHitCount = model.meteorplanethitcount;
        dataLocal.meteorSpecialPlanetHitCount = model.meteorspecialplanethitcount;
        dataLocal.meteorMultiColorHitCount = model.meteormulticolorhitcount;

        dataLocal.M_AirStone = model.m_airstone;
        dataLocal.M_FireStone = model.m_firestone;
        dataLocal.M_IceStone = model.m_icestone;
        dataLocal.M_GravityStone = model.m_gravitystone;
        dataLocal.M_Antimatter = model.m_antimatterstone;
        dataLocal.M_ColorfulStone = model.m_colorfulstone;

        dataLocal.M_Material = model.m_material;
        dataLocal.M_Air = model.m_air;
        dataLocal.M_Fire = model.m_fire;
        dataLocal.M_Ice = model.m_ice;
        dataLocal.M_Gravity = model.m_gravity;
        dataLocal.M_Antimatter = model.m_antimatter;

        dataServer.level = model.level;
        dataServer.MaterialCollect = model.materialcollect;
        dataServer.ListEnemy = JsonConvert.DeserializeObject<List<string>>(model.listenemy);
        dataServer.Name = model.name;
        dataServer.token = model.token;
        dataServer.facebookid = model.facebookid;
        dataServer.googleid = model.googleid;

        if (model.status == "test") {
            dataServer.level = UnityEngine.Random.Range(9, 15);
            dataLocal.M_Material = UnityEngine.Random.Range(162744220, 262744220);
            dataLocal.M_Air = 99999;
            dataLocal.M_Fire = 99999;
            dataLocal.M_Ice = 99999;
            dataLocal.M_Gravity = 99999;
            dataLocal.M_Antimatter = 99999;
            dataLocal.Diamond = 99999;
            dataLocal.M_AirStone = 99999;
            dataLocal.M_FireStone = 99999;
            dataLocal.M_IceStone = 99999;
            dataLocal.M_GravityStone = 99999;
            dataLocal.M_Antimatter = 99999;
            dataLocal.M_ColorfulStone = 99999;
            dataLocal.M_ToyStone1 = 99999;
            dataLocal.M_ToyStone2 = 99999;
            dataLocal.M_ToyStone3 = 99999;
            dataLocal.M_ToyStone4 = 99999;
            dataLocal.M_ToyStone5 = 99999;        
        }

        LoadingManager.userStatus = model.status;
    }

    public static bool SaveFile<T>(string filename, T content) {
        try {
            string filePath = FileHelper.GetWritablePath(filename);

            string s = Newtonsoft.Json.JsonConvert.SerializeObject(content);

            FileHelper.SaveFileWithPassword(s, filePath, "", true);
            return true;
        } catch {
            return false;
        }
    }

    public static T LoadFile<T>(string filename) {
        string filePath = FileHelper.GetWritablePath(filename);
        if (!System.IO.File.Exists(filePath))
            return default(T);

        string s = FileHelper.LoadFileWithPassword(filePath, "", true);

        try {
            T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s);
            return result;
        } catch {
            return default(T);
        }
    }
}

