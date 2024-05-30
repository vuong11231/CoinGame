using AppsFlyerSDK;
using Newtonsoft.Json;
using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class ServerSystem : MonoBehaviour {
    public static string result;
    public static ServerSystem Instance;

    private const string inappAndroidPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAyO4p433LMgm3kLyGuf01n15YiWOhXvPfGfRF0Gcx7R2CIv3LUiQ7b0YbyzdchgxdpI6WbHaZAuNOQgNbkDSmea0+FP4JQMp0bhF3kHO5hEFnk4KEbrnRm3Ka7W44uMqqkggnwqwkjcwOdiGGoiHls9RPDzSD/1iJw/an8fKsHNDwXks35Wf9QUZAd1L8F7dlkO7ABJ/6czwVoOd2ph1ToEX8fjSfcYAoML5deIuMZKM7N1LpNpizoNCptyFry2kZl2a7B3ie2AV5HZGlEp46QTFXo5HybJGv8zqH8k+k3lBiGAeNp9uqPKJ06tUsHbYqaINpXWgPnAveqeIgOb+uCwIDAQAB";
    //public bool showInfo = false;
    private bool showRequestTime = true;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {
        //StartCoroutine(CheckForMeteorFallEvent());
    }

    public void SendRequest(string url, WWWForm form, Action doneAction, Action failAction = null) {
        StartCoroutine(SendRequestCoroutine(url, form, doneAction, failAction, System.Environment.StackTrace));
    }

    IEnumerator SendRequestCoroutine(string url, WWWForm form, Action doneAction, Action failAction, string stackTrace) {
        UnityWebRequest www = UnityWebRequest.Post(url, form);

        float startRequest = Time.time;

        yield return www.SendWebRequest();

        result = www.downloadHandler.text;

        if (GameManager.DEBUG_MODE) {
            Debug.Log(url + " : " + result);
        }

        //if (showInfo) {
        //    Debug.Log(result);
        //    Debug.Log("dataGameServer: " + JsonConvert.SerializeObject(DataGameSave.dataServer));
        //    Debug.Log("dataGameLocal: " + JsonConvert.SerializeObject(DataGameSave.dataLocal));
        //}

        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(result);

            if (GameManager.DEBUG_MODE) {
                Debug.LogError("StackTrace detail " + stackTrace);
            }

            GameStatics.IsAnimating = false;

            if (SceneManager.GetActiveScene().name == SceneName.CustomLoading.ToString()) {
                PopupConfirm.ShowOK(TextMan.Get("Information"), TextMan.Get("Server is currently maintenance"), TextMan.Get("Try Again"), () => SendRequest(url, form, doneAction));
            } else if (Application.internetReachability == NetworkReachability.NotReachable) {
                PopupConfirm.ShowOK("Error", "No Internet!", "Try Again", () => SendRequest(url, form, doneAction));
            } else {
                //if (SceneManager.GetActiveScene().name == SceneName.CustomLoading.ToString()) {
                //    PopupConfirm.ShowOK("Error", "Can't connect to server!", "OK", null);
                //}
            }

            failAction.SafeCall();
        } else {
            doneAction.SafeCall();

            try {
                Hashtable hash = JsonConvert.DeserializeObject<Hashtable>(result);
                if (hash["result"].Equals("fail")) {
                    Debug.LogError(hash["detail"].ToString());

                    if (GameManager.DEBUG_MODE) {
                        Debug.LogError("StackTrace detail " + stackTrace);
                    }
                }
            } catch { }
        }
    }

    public void LogSql(string sql, string context = "") {
        StartCoroutine(LogSqlCoroutine(context, sql));
    }

    IEnumerator LogSqlCoroutine(string context, string sql) {
        WWWForm form = new WWWForm();
        form.AddField("sql", sql);

        UnityWebRequest www = UnityWebRequest.Post(ServerConstants.QUERY_FOR_DATA, form);

        yield return www.SendWebRequest();

        result = www.downloadHandler.text;

        //List<UniverseModel> list = JsonConvert.DeserializeObject<List<UniverseModel>>(result);

        Debug.Log(context + " - " + sql + " : " + result);
    }

    public bool IsResponseOK()
    {
        Hashtable hash = JsonConvert.DeserializeObject<Hashtable>(result);

        if (hash != null && hash["result"] != null && hash["result"].ToString() == "success")
            return true;
        else
            return false;
    }

    public void TrackBuy(string productName, Product product) {
        float gPrice = (float)product.metadata.localizedPrice * 0.7f;

        string gCurrency = product.metadata.localizedPriceString;


#if UNITY_ANDROID
        var googleReceipt = GooglePurchase.FromJson(product.receipt);
#endif
#if UNITY_IOS
        var appleReceipt = ApplePurchase.FromJson(product.receipt);
#endif
        Dictionary<string, string> addparam = new Dictionary<string, string>(){
                                { AFInAppEvents.CONTENT_ID, product.definition.id},
                                { AFInAppEvents.REVENUE, gPrice.ToString() },
                                { AFInAppEvents.CURRENCY, gCurrency}
                            };



#if UNITY_ANDROID
        AppsFlyerAndroid.validateAndSendInAppPurchase(
        inappAndroidPublicKey,
        googleReceipt.PayloadData.signature,
        googleReceipt.PayloadData.json,
        gPrice.ToString(),
        gCurrency,
        addparam,
        this);
#endif

#if UNITY_IOS && !UNITY_EDITOR
            AppsFlyeriOS.validateAndSendInAppPurchase(
            product.definition.id,
            gPrice.ToString(),
            product.metadata.isoCurrencyCode,
            product.transactionID,
            addparam,
            this);
#endif
    }

    public string ReadData()
    {
        Hashtable hash = JsonConvert.DeserializeObject<Hashtable>(result);

        return hash["data"].ToString();
    }

    public string ResultDetail()
    {
        Hashtable hash = JsonConvert.DeserializeObject<Hashtable>(result);

        return hash["detail"].ToString();
    }

    private static void GetChartData(string chartid, Action<string> result) {
        if (GameManager.DEBUG_MODE) {
            Debug.Log("get data for chart id " + chartid);
        }

        WWWForm form = new WWWForm();
        form.AddField("chartid", chartid);

        ServerSystem.Instance.SendRequest(ServerConstants.GET_CHART_DATA, form, () => {
            result.SafeCall(ServerSystem.result);
        });
    }

    public static void ShowCurrentRankChart(Action<string> result) {
        WWWForm form = new WWWForm();

        form.AddField("user_id", DataGameSave.dataServer.userid);

        ServerSystem.Instance.SendRequest(ServerConstants.GET_CURRENT_CHART_ID, form, () => {
            //Debug.Log("INSERTING TO NEW RANK CHART");
            if (ServerSystem.result != "") {
                GetChartData(ServerSystem.result, result);
            } else {
                form = new WWWForm();
                form.AddField("user_id", DataGameSave.dataServer.userid);
                form.AddField("day_code", DataGameSave.GetDayCode(GameManager.Now));
                form.AddField("level", DataGameSave.dataServer.level);

                ServerSystem.Instance.SendRequest(ServerConstants.INSERT_TO_NEW_RANK_CHART, form, () => {
                    GetChartData(ServerSystem.result, result);
                });
            }
        });
    }

    //public static void GetPrize(Action<string> result) {
    //    WWWForm form = new WWWForm();
    //    form.AddField("user_id", DataGameSave.dataServer.userid);

    //    //DataGameSave.dataLocal.destroyedSolars = 0;
    //    //DataGameSave.dataLocal.destroyPlanet = 0;
    //    //DataGameSave.dataLocal.meteorPlanetHitCount = 0;
    //    //DataGameSave.SaveToLocal();

    //    ServerSystem.Instance.SendRequest(ServerConstants.RESET_RANK_POINT, form, () => {
    //        form = new WWWForm();
    //        form.AddField("user_id", DataGameSave.dataServer.userid);
    //        form.AddField("day_code", DataGameSave.GetDayCode(GameManager.Now));
    //        form.AddField("level", DataGameSave.dataServer.level);

    //        ServerSystem.Instance.SendRequest(ServerConstants.INSERT_TO_NEW_RANK_CHART, form, () => {
    //            DataGameSave.SaveMetaData(MetaDataKey.RANK_LAST_RECEIVED_DAYCODE, DataGameSave.GetDayCode(GameManager.Now));
    //            GetChartData(ServerSystem.result, result);

    //            if (GameManager.DEBUG_MODE) {
    //                Debug.Log("New chart id: " + ServerSystem.result);
    //            }
    //        });
    //    });
    //}

    public static void RefreshListEnemy(Action doneAction = null)
    {
        if (GameStatics.IsAnimating)
            return;

        WWWForm form = new WWWForm();
        form.AddField("userid", DataGameSave.dataLogin.userid);
        form.AddField("token", DataGameSave.dataLogin.token.ToString());
        form.AddField("level", DataGameSave.dataServer.level);

        ServerSystem.Instance.SendRequest(ServerConstants.FIND_EIGHT_ENEMY, form, () =>
        {
            if (ServerSystem.Instance.IsResponseOK())
            {
                string data = ServerSystem.Instance.ReadData();
                List<DataGameServer> list = JsonConvert.DeserializeObject<List<DataGameServer>>(data);
                DataGameSave.listDataEnemies = list;

                if (doneAction != null)
                {
                    doneAction.Invoke();
                }
            }
        });
    }

    public IEnumerator CheckForMeteorFallEvent() {

        float countDown = UnityEngine.Random.Range(GameManager.METEOR_FALL_MIN_SECONDS, GameManager.METEOR_FALL_MIN_SECONDS);

        while (true) {
            if (UIMultiScreenCanvasMan.Instance) {
                countDown -= Time.deltaTime;
            }

            if (countDown < 0 && Scenes.Current == SceneName.Gameplay && 
                UIMultiScreenCanvasMan.modeExplore == UIMultiScreenCanvasMan.Mode.Gameplay &&
                PlayerPrefs.GetString(PlayerPrefsKey.METEOR_FALL_EVENT_ENABLED) == "true" &&
                DataGameSave.dataServer.level >= 3) {
                countDown = UnityEngine.Random.Range(GameManager.METEOR_FALL_MIN_SECONDS, GameManager.METEOR_FALL_MIN_SECONDS);

                if (UIMultiScreenCanvasMan.Instance) {
                    StartCoroutine(UIMultiScreenCanvasMan.Instance.DoMeteorFallEvent());
                }
                
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
