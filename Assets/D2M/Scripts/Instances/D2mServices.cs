using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

[Serializable]
public class D2mConfigAppInfo
{
    [SerializeField]
    private string _idOrNameApp = "";
    public string IdOrNameApp
    {
        get { return _idOrNameApp; }
        set { _idOrNameApp = value; }
    }

    [SerializeField]
    private string _urlAssetBundle = "";
    public string UrlAssetBundle
    {
        get { return _urlAssetBundle; }
        set { _urlAssetBundle = value; }
    }

    [SerializeField]
    private string _urlConfigInAppPurchase = "";
    public string UrlConfigInAppPurchase
    {
        get { return _urlConfigInAppPurchase; }
        set { _urlConfigInAppPurchase = value; }
    }

    [SerializeField]
    private string _urlConfigAds = "";
    public string UrlConfigAds
    {
        get { return _urlConfigAds; }
        set { _urlConfigAds = value; }
    }

    [SerializeField]
    private string _urlConfigSocial;
    public string UrlConfigSocial
    {
        get { return _urlConfigSocial; }
        set { _urlConfigSocial = value; }
    }


    [SerializeField]
    private string _urlConfigSms = "";
    public string UrlConfigSms
    {
        get { return _urlConfigSms; }
        set { _urlConfigSms = value; }
    }

    [SerializeField]
    private string _urlConfigWebView = "";
    public string UrlConfigWebView
    {
        get { return _urlConfigWebView; }
        set { _urlConfigWebView = value; }
    }
}

[System.Serializable]
public class D2mCoreMobileInfo
{
    [SerializeField]
    private bool _isEnableLog = false;
    public bool IsEnableLog
    {
        get { return _isEnableLog; }
        set { _isEnableLog = value; }
    }

    [SerializeField]
    private D2mConfigAppInfo _configAppInfo;
    public D2mConfigAppInfo ConfigAppInfo
    {
        get { return _configAppInfo; }
        set { _configAppInfo = value; }
    }
}

public class D2mServices : Singleton<D2mServices>
{
    // info, bool success
    public static event Action<D2mCoreMobileInfo, bool> OnLoadConfigCompleted = delegate { };

    #region Vairables Define
    [SerializeField]
    private bool _defineAdMob;
    public bool DefineAdMob
    {
        get { return _defineAdMob; }
        set { _defineAdMob = value; }
    }

    [SerializeField]
    private bool _defineAdNative;
    public bool DefineAdNative
    {
        get { return _defineAdNative; }
        set { _defineAdNative = value; }
    }

    [SerializeField]
    private bool _defineIAP;
    public bool DefineIAP
    {
        get { return _defineIAP; }
        set { _defineIAP = value; }
    }

    [SerializeField]
    private bool _defineFacebookSDK;
    public bool DefineFacebookSDK
    {
        get { return _defineFacebookSDK; }
        set { _defineFacebookSDK = value; }
    }

    [SerializeField]
    private bool _defineGameServices;
    public bool DefineGameServices
    {
        get { return _defineGameServices; }
        set { _defineGameServices = value; }
    }

    [SerializeField]
    private bool _defineFirebaseAnalytic;
    public bool DefineFirebaseAnalytic
    {
        get { return _defineFirebaseAnalytic; }
        set { _defineFirebaseAnalytic = value; }
    }

    [SerializeField]
    private bool _defineFirebaseCrashlytic;
    public bool DefineFirebaseCrashlytic
    {
        get { return _defineFirebaseCrashlytic; }
        set { _defineFirebaseCrashlytic = value; }
    }

    [SerializeField]
    private bool _defineFirebaseMessaging;
    public bool DefineFirebaseMessaging
    {
        get { return _defineFirebaseMessaging; }
        set { _defineFirebaseMessaging = value; }
    }

    [SerializeField]
    private bool _defineFirebaseAuth;
    public bool DefineFirebaseAuth
    {
        get { return _defineFirebaseAuth; }
        set { _defineFirebaseAuth = value; }
    }

    #endregion

    #region Variables
    [Space]
    [SerializeField]
    private string _urlCoreConfig = "";
    public string UrlCoreConfig
    {
        get { return _urlCoreConfig; }
        set { _urlCoreConfig = value; }
    }

    [SerializeField]
    private TextAsset _textAssetCoreConfig;
    public TextAsset TextAssetCoreConfig
    {
        get { return _textAssetCoreConfig; }
        set { _textAssetCoreConfig = value; }
    }

    [SerializeField]
    private bool _isRunInBackground = true;

    [SerializeField]
    private D2mCoreMobileInfo _coreMobileInfo = new D2mCoreMobileInfo();
    public D2mCoreMobileInfo CoreMobileInfo
    {
        get { return _coreMobileInfo; }
        set { _coreMobileInfo = value; }
    }


    #endregion

    void Start()
    {
        //Init();
    }

    void Init()
    {
        try
        {
            Application.runInBackground = _isRunInBackground;
            StartCoroutine(AsyncLoadConfig());
        }
        catch (Exception ex)
        {
            Debug.LogError("D2mServices Init" + ex.Message);
            Setup();
            OnLoadConfigCompleted?.Invoke(CoreMobileInfo, false);
        }
    }

    IEnumerator AsyncLoadConfig()
    {
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(D2mConstants.AsyncGetDataFromUrl(UrlCoreConfig, _textAssetCoreConfig, (string data) =>
        {
            string textConfig = data;

            if (!string.IsNullOrEmpty(textConfig))
            {
                CoreMobileInfo = JsonConvert.DeserializeObject<D2mCoreMobileInfo>(textConfig);
                Setup();
                OnLoadConfigCompleted?.Invoke(CoreMobileInfo, true);
            }
            else
            {
                Setup();
                OnLoadConfigCompleted?.Invoke(CoreMobileInfo, false);
            }
        }));
        yield return null;
    }

    void Setup()
    {
#if UNITY_2018_1_OR_NEWER
        Debug.unityLogger.logEnabled = CoreMobileInfo.IsEnableLog;
#endif
    }


    #region Editor Methods
#if UNITY_EDITOR

    [ContextMenu("Save Info", false, 1000)]
    public void SaveInfo()
    {
        string str = JsonConvert.SerializeObject(_coreMobileInfo);

        string pathDirectory = Path.Combine(Application.dataPath, D2mConstants.PathFolderData);
        D2mConstants.CreateDirectory(pathDirectory);

        string path = Path.Combine(pathDirectory, @"D2mServices.txt");
        StreamWriter file = new System.IO.StreamWriter(path);
        file.WriteLine(str);
        file.Close();

        UnityEditor.AssetDatabase.Refresh();
    }

    [ContextMenu("Load Info", false, 1000)]
    public void LoadInfo()
    {
        string path = Path.Combine(Application.dataPath, D2mConstants.PathFolderData);
        path = Path.Combine(path, @"D2mServices.txt");
        if (File.Exists(path))
        {
            string text = System.IO.File.ReadAllText(path);
            CoreMobileInfo = JsonConvert.DeserializeObject<D2mCoreMobileInfo>(text);
        }

        UnityEditor.AssetDatabase.Refresh();
    }
#endif
    #endregion
}