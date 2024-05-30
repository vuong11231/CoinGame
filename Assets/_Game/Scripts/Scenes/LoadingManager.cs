using Facebook.MiniJSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.SignInWithApple;
using UnityEngine.UI;

public class LoadingManager : Singleton<LoadingManager> {
    public static bool isLoading = false;
    public static string userStatus = "";

    public TextMeshProUGUI textStatus;
    public TextMeshProUGUI txtVersion;
    public GameObject startButton;
    public GameObject langGo = null;
    public GameObject facebookButton;
    //public GameObject appleButton;
    public GameObject appleSignInButton;
    public SignInWithApple signInWithApple;

    //bool loadRemoteConfig;
    bool loadUserData = false;
    bool loadRankData = false;
    bool loadListOfEnemy = false;
    bool loadAttackedData = false;
    bool loadFriendList = false;
    bool loadSkinData = false;
    bool loadMail = false;
    
    string userLoginType;

    protected override void Awake() {
        base.Awake();

        Sounds.Instance.PlayMusic();
        Screen.sleepTimeout = 0;
        DataGameSave.CheckInitLocal();
        DataHelper.SetBool(TextConstants.Login, false);
        textStatus.gameObject.SetActive(false);
        txtVersion.text = "Version " + Application.version;

#if UNITY_IOS
        facebookButton.gameObject.SetActive(false);
        //appleButton.gameObject.SetActive(true);
#endif

#if UNITY_ANDROID
        facebookButton.gameObject.SetActive(true);
        //appleButton.gameObject.SetActive(false);
#endif

        DataMaster.DownloadDataMaster(() => {
            if (langGo && !PlayerPrefs.HasKey("languageidx"))
                langGo.SetActive(true);
            else
                CheckVersionAndStartGame();
        });
    }

    void Start() {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_EDITOR || UNITY_IOS
        appleSignInButton.gameObject.SetActive(true);
#else
        appleSignInButton.gameObject.SetActive(false);
#endif

#if UNITY_EDITOR || UNITY_ANDROID
        facebookButton.gameObject.SetActive(true);
#else
        facebookButton.gameObject.SetActive(false);
#endif
    }

    public void OnSetLang(int idx) {
        langGo.SetActive(false);

        PlayerPrefs.SetInt("languageidx", idx);
        TextMan.LangIndex = idx;

        var arr = FindObjectsOfType<LocalizedText>();

        if (arr != null && arr.Length > 0)
            foreach (var l in arr)
                l.ReUpdate();

        userLoginType = "guest";
        CheckVersionAndStartGame();
    }

    void CheckVersionAndStartGame() {

        ServerSystem.Instance.SendRequest(ServerConstants.GET_REMOTE_CONFIG, new WWWForm(), () => {
            DataGameSave.remoteConfig = JsonConvert.DeserializeObject<Dictionary<string,string>>(ServerSystem.result);

            int currentVersion = int.Parse(DataGameSave.remoteConfig["version"]);
            float.TryParse(DataGameSave.remoteConfig[RemoteConfigKey.STEAL_MATERIAL_RATE_OFFLINE_COLLECT], out GameManager.STEAL_MATERIAL_RATE_OFFLINE_COLLECT);
            float.TryParse(DataGameSave.remoteConfig[RemoteConfigKey.STEAL_MATERIAL_RATE_TOTAL], out GameManager.STEAL_MATERIAL_RATE_TOTAL);
            bool.TryParse(DataGameSave.remoteConfig[RemoteConfigKey.SHOW_EVENT_BUTTON], out GameManager.SHOW_EVENT_BUTTON);

            Debug.Log(GameManager.STEAL_MATERIAL_RATE_OFFLINE_COLLECT);
            Debug.Log(GameManager.STEAL_MATERIAL_RATE_TOTAL);

            if (GameManager.VERSION < currentVersion) {
                ShowPopupRequireUpdate();
            } else {
                StartGameHere();
            }
        });
    }

    void ShowPopupRequireUpdate() {
        PopupConfirm.ShowOK(
                    TextMan.Get("New version"),
                    string.Format(TextMan.Get("You're using version {0} please update to newest version to access newest features"), GameManager.VERSION_NAME),
                    "OK",
                    () => {
                        Application.OpenURL(GameManager.LINK_OF_GAME_ON_STORE);
                        ShowPopupRequireUpdate();
                    });
    }

    private void StartGameHere(string userName = null) {
        DataGameSave.dataServer = new DataGameServer();        
        DataGameSave.dataServer.ListPlanet.Add(new DataPlanet());

        textStatus.gameObject.SetActive(true);
        textStatus.text = TextMan.Get("Logging in...");

        DataGameLogin loginData = DataGameLogin.LoadLoginDataFromLocal();
        DataGameSave.dataLogin = loginData;

        if (loginData == null) {
            StartCoroutine(RegisterThenLogin(userName));
        } else if (loginData.loginType == DataGameLogin.LoginType.Facebook) {
            DataGameSave.dataLogin = loginData;
            StartCoroutine(LoadDataByFacebookIdThenLogin());
        } else if (loginData.loginType == DataGameLogin.LoginType.Apple)
        {
            DataGameSave.dataLogin = loginData;
            StartCoroutine(LoadDataByAppleIdThenLogin());
        }
        else {
            DataGameSave.dataLogin = loginData;
            StartCoroutine(LoadDataThenLogin());
        }
    }

    IEnumerator RegisterThenLogin(string userName = null) {
        RegisterNewUser(userName);
        yield return new WaitUntil(() => loadUserData);

        StartCoroutine(LoadData());
    }

    IEnumerator LoadDataThenLogin() {
        LoadUserData();
        yield return new WaitUntil(() => loadUserData);

        StartCoroutine(LoadData());
    }

    IEnumerator LoadDataByFacebookIdThenLogin() {
        LoadDataByFacebookId();
        yield return new WaitUntil(() => loadUserData);

        StartCoroutine(LoadData());
    }

    IEnumerator LoadDataByAppleIdThenLogin()
    {
        LoadDataByAppleId();
        yield return new WaitUntil(() => loadUserData);

        StartCoroutine(LoadData());
    }

    IEnumerator LoadData() {
#if UNITY_IOS
        bool showFacebookButton = DataGameSave.dataServer.level >= 5;
        facebookButton.SetActive(showFacebookButton);
#endif

        LoadRankDataAndMetaData();
        yield return new WaitUntil(() => loadRankData);

        LoadAttackedData();
        yield return new WaitUntil(() => loadAttackedData);

        LoadMail();
        yield return new WaitUntil(() => loadMail);

        LoadSkinData();
        yield return new WaitUntil(() => loadSkinData);

        LoadListOfEnemy();
        yield return new WaitUntil(() => loadListOfEnemy);

        LoadFriendList();
        yield return new WaitUntil(() => loadFriendList);

        UpdateTutorialKey();
    }

    private void LoadMail() {
        ServerSystem.Instance.SendRequest(ServerConstants.GET_MAIL, new WWWForm(), () => {
            string json = ServerSystem.result;
            DataGameSave.mails = JsonConvert.DeserializeObject<List<DataMail>>(json);
            loadMail = true;
        });
    }

    public void LoadDataByFacebookId() {
        WWWForm form = new WWWForm();
        form.AddField("facebookid", DataGameSave.dataLogin.facebookid);
        ServerSystem.Instance.SendRequest(ServerConstants.GET_UNIVERSE_DATA_BY_FACEBOOK_ID, form, () => {
            try {
                UniverseModel model = JsonConvert.DeserializeObject<UniverseModel>(ServerSystem.Instance.ReadData());
                DataGameSave.ReadUniverseModel(model);
                loadUserData = true;
            } catch {
                DataGameSave.dataLogin.facebookid = "";
                DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Normal;
                DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);

                LoadUserData();
            }
        });
    }

    public void LoadDataByAppleId()
    {
        var form = new WWWForm();
        form.AddField("googleid", DataGameSave.dataLogin.googleid);
        ServerSystem.Instance.SendRequest(ServerConstants.GET_UNIVERSE_DATA_BY_APPLE_ID, form, () => {
            try
            {
                UniverseModel model = JsonConvert.DeserializeObject<UniverseModel>(ServerSystem.Instance.ReadData());
                DataGameSave.ReadUniverseModel(model);
                loadUserData = true;
            }
            catch
            {
                DataGameSave.dataLogin.googleid = "";
                DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Normal;
                DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);

                LoadUserData();
            }
        });
    }

    public void RegisterNewUser(string userName = null) {
        string url = ServerConstants.REGISTER_NEW_USER;
        string username = userName.IsNullOrEmpty() ? (Instance == null ? "User" : Instance.userLoginType) : userName;
        WWWForm form = new WWWForm();
        form.AddField("username", "temp_name");

        ServerSystem.Instance.SendRequest(url, form, () => {
            Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();
            result = JsonConvert.DeserializeObject<Dictionary<string, System.Object>>(ServerSystem.result);

            if (result["result"].ToString() == "success") {
                DataGameLogin loginData = new DataGameLogin();
                loginData.userid = Convert.ToInt32(result["user_id"].ToString());
                loginData.username = username;
                loginData.token = result["token"].ToString();

                DataGameLogin.SaveLoginDataToLocal(loginData);
                DataGameSave.dataLogin = loginData;

                if (Cheat.Get("auto login when register successful"))
                    LoadUserData(resetCollectTime: true);
                else
                    PopupConfirm.ShowOK(TextMan.Get("Register"), TextMan.Get("Register successfully!"), "OK", () => LoadUserData(resetCollectTime: true));
            } else {
                PopupConfirm.ShowOK(TextMan.Get("Register"), TextMan.Get("Register Fail!"), TextMan.Get("Try Again"), () => RegisterNewUser());
            }
        });
    }

    public void LoadUserData(bool resetCollectTime = false) {
        if (isLoading == false) {
            isLoading = true;
        } else {
            return;
        }

        string url = ServerConstants.GET_USER_DATA;

        WWWForm form = new WWWForm();
        form.AddField("userid", Convert.ToInt32(DataGameSave.dataLogin.userid.ToString()));
        form.AddField("token", DataGameSave.dataLogin.token.ToString());

        if (Instance) {
            Instance.textStatus.text = TextMan.Get("Loading player's data...");
        }

        ServerSystem.Instance.SendRequest(url, form, () => {
            if (!ServerSystem.Instance.IsResponseOK()) {
                isLoading = false;
                if (ServerSystem.Instance.ResultDetail() == "user id not exist in database!") {
                    PopupConfirm.ShowOK(TextMan.Get("Error"), TextMan.Get("Data corrupted, create a new one"), "OK", () => RegisterNewUser());
                } else {
                    PopupConfirm.ShowOK(TextMan.Get("Error"), TextMan.Get("Error when loading data"), TextMan.Get("Try Again"), () => LoadUserData());
                }
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

                    for (int i = 0; i < dataGameServer.ListPlanet.Count; i++) {
                        dataGameServer.ListPlanet[i].CollectTime = dataGameServer.ListPlanet[i].CollectTime.ToLocalTime();

                        if (dataGameServer.ListPlanet[i].CollectTime == DateTime.MinValue)
                            dataGameServer.ListPlanet[i].CollectTime = GameManager.Now;
                    }

                    if (resetCollectTime) {
                        for (int i = 0; i < dataGameServer.ListPlanet.Count; i++) {
                            dataGameServer.ListPlanet[i].CollectTime = GameManager.Now;
                        }
                    }

                    DataGameSave.dataServer = dataGameServer;

                    //WWWForm form1 = new WWWForm();
                    //form1.AddField("user_id", Convert.ToInt32(DataGameSave.dataLogin.userid.ToString()));
                    //ServerSystem.Instance.SendRequest(ServerConstants.GET_USER_DATA_JSON, form1, () => {
                        //if (ServerSystem.result != "") {
                        //    DataGameSave.dataServer = JsonConvert.DeserializeObject<DataGameServer>(ServerSystem.result);
                        //}

                        WWWForm form2 = new WWWForm();
                        form2.AddField("user_id", Convert.ToInt32(DataGameSave.dataLogin.userid.ToString()));

                        ServerSystem.Instance.SendRequest(ServerConstants.GET_UNIVERSE_DATA, form2, () => {
                            if (ServerSystem.Instance.IsResponseOK()) {
                                UniverseModel model = JsonConvert.DeserializeObject<UniverseModel>(ServerSystem.Instance.ReadData());
                                DataGameSave.ReadUniverseModel(model);
                            }

                            if (Cheat.Get("cheat level", out int lv)) 
                            {
                                DataGameSave.dataServer.level = 9;
                            }

                            loadUserData = true;
                        });
                    //});
                } catch {
                    RegisterNewUser();
                    isLoading = false;
                }
            }
        });
    }

    void LoadListOfEnemy() {
        WWWForm form = new WWWForm();
        form.AddField("userid", DataGameSave.dataLogin.userid);
        form.AddField("token", DataGameSave.dataLogin.token.ToString());
        form.AddField("level", DataGameSave.dataServer.level);

        if (Instance)
            Instance.textStatus.text = TextMan.Get("Loading battle's information...");

        ServerSystem.Instance.SendRequest(ServerConstants.FIND_EIGHT_ENEMY, form, () => {
            if (!ServerSystem.Instance.IsResponseOK()) {
                PopupConfirm.ShowOK(TextMan.Get("Error"), TextMan.Get("Error when loading data"), TextMan.Get("Try Again"), () => LoadUserData());
            } else {
                string data = ServerSystem.Instance.ReadData();
                List<DataGameServer> list = JsonConvert.DeserializeObject<List<DataGameServer>>(data);
                DataGameSave.listDataEnemies = list;

                loadListOfEnemy = true;
            }
        });
    }

    void LoadFriendList() {
        WWWForm form = new WWWForm();
        form.AddField("userid", DataGameSave.dataLogin.userid);
        form.AddField("token", DataGameSave.dataLogin.token.ToString());

        if (Instance)
            Instance.textStatus.text = TextMan.Get("Loading friend list...");

        ServerSystem.Instance.SendRequest(ServerConstants.GET_FRIEND_LIST, form, () => {
            if (!ServerSystem.Instance.IsResponseOK()) {
                PopupConfirm.ShowOK(TextMan.Get("Error"), TextMan.Get("Error when loading data"), TextMan.Get("Try Again"), () => LoadUserData());
            } else {
                string data = ServerSystem.Instance.ReadData();
                List<DataGameServer> list = JsonConvert.DeserializeObject<List<DataGameServer>>(data);
                DataGameSave.listDataFriends = list;

                if (Instance)
                    Instance.textStatus.text = TextConstants.LOADING_DONE;

                if (Instance && Instance.startButton)
                    Instance.startButton.SetActive(true);

                loadFriendList = true;
            }
        });
    }

    public void LoadRankDataAndMetaData() {
        DataGameSave.dataRank = new DataRank();

        WWWForm form = new WWWForm();
        form.AddField("user_id", DataGameSave.dataServer.userid);

        ServerSystem.Instance.SendRequest(ServerConstants.GET_RANK_DATA, form, () => {

            //Dictionary<string, string> metadataFromLocal = new Dictionary<string, string>();

            //try {
            //    metadataFromLocal = JsonConvert.DeserializeObject<Dictionary<string, string>>(DataGameSave.dataLocal.playFabToken);
            //} catch { }
            //Debug.Log("METADATA FROM LOCAL: " + metadataFromLocal.Count);
            //foreach (var item in metadataFromLocal) {
            //    Debug.Log(item);
            //}

            if (ServerSystem.Instance.IsResponseOK()) {
                DataGameSave.dataRank = new DataRank();
                string jsonData = ServerSystem.Instance.ReadData();
                var listRanks = new List<DataRank>();

                var jsonSettings = new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                };

                listRanks = JsonConvert.DeserializeObject<List<DataRank>>(jsonData, jsonSettings);
                if (listRanks.Count > 0) {
                    DataGameSave.dataRank = listRanks[0];
                }

                var dic = new Dictionary<string, string>();

                try {
                    dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(DataGameSave.dataRank.s1, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                } catch { }

                DataGameSave.metaData = dic != null ? dic : new Dictionary<string, string>();

                //backup data in DataGameLocal.playfabToken
                Dictionary<string, string> metadataFromJsonLocal = new Dictionary<string, string>();

                try {
                    metadataFromJsonLocal = JsonConvert.DeserializeObject<Dictionary<string, string>>(DataGameSave.dataLocal.playFabToken);
                } catch { }

                if (metadataFromJsonLocal.Count > 0) {
                    DataGameSave.metaData = metadataFromJsonLocal;
                }

                //load autoshot
                DataGameSave.autoShootCount = GameManager.MAX_AUTO_SHOOT_COUNT;
                int.TryParse(DataGameSave.GetMetaData(MetaDataKey.AUTO_SHOOT_COUNT), out DataGameSave.autoShootCount);

                int daycode = int.MaxValue;
                int.TryParse(DataGameSave.GetMetaData(MetaDataKey.AUTO_SHOOT_DAYCODE), out daycode);
                if (daycode < int.Parse(DataGameSave.GetDayCode(GameManager.Now)) && DataGameSave.autoShootCount < GameManager.MAX_AUTO_SHOOT_COUNT) {
                    DataGameSave.autoShootCount = GameManager.MAX_AUTO_SHOOT_COUNT;

                    DataGameSave.SaveMetaData(MetaDataKey.AUTO_SHOOT_DAYCODE, DataGameSave.GetDayCode(GameManager.Now));
                    DataGameSave.SaveMetaData(MetaDataKey.AUTO_SHOOT_COUNT, DataGameSave.autoShootCount.ToString());
                }

                //load autorestore
                DataGameSave.autoRestoreEndTime = long.MinValue;
                long.TryParse(DataGameSave.GetMetaData(MetaDataKey.AUTO_RESTORE_TIME), out DataGameSave.autoRestoreEndTime);

                DataGameSave.x2MaterialEndTime = long.MinValue;
                long.TryParse(DataGameSave.GetMetaData(MetaDataKey.X2_MATERIAL_END_TIME), out DataGameSave.x2MaterialEndTime);
            }

            loadRankData = true;
        });
    }

    public void LoadAttackedData() {
        WWWForm form = new WWWForm();
        form.AddField("user_id", DataGameSave.dataServer.userid);

        ServerSystem.Instance.SendRequest(ServerConstants.GET_IS_ATTACKED_CODE, form, () => {
            DataGameSave.dataServer.isAttackedCode = ServerSystem.result;
            loadAttackedData = true;
        });
    }

    public void LoadSkinData() {
        DataGameSave.skinPieces = new List<int>();
        DataGameSave.skinLevels = new List<int>();

        string jsonPiece = DataGameSave.GetMetaData(MetaDataKey.SKIN_PIECE);
        string jsonLevel = DataGameSave.GetMetaData(MetaDataKey.SKIN_LEVEL);

        if (jsonPiece != "") {
            DataGameSave.skinPieces = JsonConvert.DeserializeObject<List<int>>(jsonPiece);
        }

        if (jsonLevel != "") {
            DataGameSave.skinLevels = JsonConvert.DeserializeObject<List<int>>(jsonLevel);
        }

        loadSkinData = true;
    }

    public void UpdateTutorialKey() {
        if (DataGameSave.dataServer.level >= 3) {
            TutMan.SetTutDone(TutMan.TUT_KEY_01_HIDE_GO_WHEN_FIRST_ENTER_GAME);
            TutMan.SetTutDone(TutMan.TUT_KEY_02_UPGRADE_SUN);
            TutMan.SetTutDone(TutMan.TUT_KEY_03_FOCUS_GAMEPLAY_METEOR_PLANETS);
            TutMan.SetTutDone(TutMan.TUT_KEY_04_PRESS_BUTTON_MANAGER);
            TutMan.SetTutDone(TutMan.TUT_KEY_05_PRESS_AUTO_UPGRADE);
            TutMan.SetTutDone(TutMan.TUT_KEY_06_PRESS_FIRE_EFFECT_BUTTON);
            TutMan.SetTutDone(TutMan.TUT_KEY_07_COMPLETE_SUN_QUEST);
            TutMan.SetTutDone(TutMan.TUT_KEY_07_1_FOCUS_QUICK_ATTACK_NEIGHBOR);
            TutMan.SetTutDone(TutMan.TUT_KEY_08_BACK_TO_GAMEPLAY);
            TutMan.SetTutDone(TutMan.TUT_KEY_09_FOCUS_NEIGHBOR);
            TutMan.SetTutDone(TutMan.TUT_KEY_10_FOCUS_ATTACK_BUTTON_NEIGHBOR);
            TutMan.SetTutDone(TutMan.TUT_KEY_11_FOCUS_RESTORE);
        }

        if (DataGameSave.dataServer.level > 5) {
            TutMan.SetTutDone(TutMan.TUT_KEY_12_FOCUS_METEOR_BUTTON);
            TutMan.SetTutDone(TutMan.TUT_KEY_FOCUS_BLACK_HOLE_BUTTON);
            TutMan.SetTutDone(TutMan.TUT_KEY_HAND_BLACK_HOLE);
        }
    }

    public void StartGame() {
        Scenes.ChangeScene(SceneName.Gameplay);
    }

    public void OnFacebookButtonClick() {
        DataGameSave.LoginFacebookButtonClick();
    }

    public void OnAppleButtonClick()
    {
        DataGameSave.LoginAppleButtonClick(signInWithApple);
    }
}
