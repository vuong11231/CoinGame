using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_2018_3_OR_NEWER
using UnityEngine.Networking;
#endif

public enum ETypeAssetBundle
{
    None = 0,
    Object,
    Scene,
    //Audio
    Data,
}

public enum ESubTypeAssetBundle
{
    None = 0,
    Prefab,
    Music,
    Sound
}

public enum EDownloadAssetBundleMethod
{
    LoadFromMemoryAsync,
    LoadFromFile,
    LoadFromCacheOrDownload,
    UnityWebRequest
}

[Serializable]
public class D2mAssetBundleConfigInfo
{
    [SerializeField]
    private List<DMCPlatformBundleInfo> _listPlatformBundleInfo;
    public List<DMCPlatformBundleInfo> ListPlatformBundleInfo
    {
        get { return _listPlatformBundleInfo; }
        set { _listPlatformBundleInfo = value; }
    }

    [SerializeField]
    private float _timeWaitConnect = 10;
    public float TimeWaitConnect
    {
        get { return _timeWaitConnect; }
        set { _timeWaitConnect = value; }
    }

    [SerializeField]
    private bool _enableLogProgress;
    public bool EnableLogProgress
    {
        get { return _enableLogProgress; }
    }
}

[Serializable]
public class DMCPlatformBundleInfo
{
    [JsonProperty("platform")]
    public string PlatformName = "";
    [JsonProperty]
    public bool IsUsingHashCodeInManifest = false;
    [JsonProperty("url_server")]
    public string UrlServer = "";
    [JsonProperty("enable")]
    public bool Enable = true;
    //[JsonProperty("version")]
    //public string Version = "1.0.0";
    //[JsonProperty("version_code")]
    //public int VersionCode = 1;
    [JsonProperty("assets")]
    public D2mBundleInfo[] BundleInfos;
}

[Serializable]
public class D2mBundleInfo
{
    [JsonProperty("asset_bundle_name")]
    public string Name;
    [JsonProperty("url_download")]
    public string UrlDownload;
    [JsonProperty("url_manifest")]
    public string UrlManifest;
    [JsonProperty("type")]
    public ETypeAssetBundle Type = ETypeAssetBundle.Object;
    [JsonProperty("sub_type")]
    public ESubTypeAssetBundle SubType = ESubTypeAssetBundle.None;
    [JsonProperty("version")]
    public int Version = 0;
    //[JsonProperty("objects")]
    //public DMCObjectBundleInfo[] ObjectInBundles;

    [SerializeField]
    private bool _isDownloadWhenStart = true;
    public bool IsDownloadWhenStart
    {
        get { return _isDownloadWhenStart; }
        set { _isDownloadWhenStart = value; }
    }
}

[Serializable]
public class D2mObjectBundleInfo
{
    [SerializeField]
    private string _name = "";
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    [SerializeField]
    private string _url;
    public string Url
    {
        get { return _url; }
        set { _url = value; }
    }
}

[Serializable]
public class D2mAssetBundleData
{
    public D2mBundleInfo bundleInfo;
    public AssetBundle assetBundle;

    public D2mAssetBundleData()
    {

    }

    public D2mAssetBundleData(D2mBundleInfo bundleInfo, AssetBundle assetBundle)
    {
        this.bundleInfo = bundleInfo;
        this.assetBundle = assetBundle;
    }
}

public class AssetsBundles : Singleton<AssetsBundles>
{

    #region Constants
    private const string KeyABConfig = "VSGAB.Config";
    #endregion

    #region Events
    public static event Action OnDownloadConfigCompleted = delegate { };

    public static event Action OnDownloadAllAssetStart = delegate { };
    public static event Action<bool> OnDownloadAllAssetCompleted = delegate { };

    // string: asset name
    public static event Action<string> OnDownloadAssetStart = delegate { };
    public static event Action<string, bool> OnDownloadAssetCompleted = delegate { };

    public static event Action<string, float> OnDownloadAssetProgress = delegate { };
    #endregion


    #region Variables and Properties
    [Header("Setup Config")]
    [SerializeField]
    private D2mAssetBundleConfigInfo _assetBundleConfigInfo;
    //[SerializeField]
    //private ELoadFileMethod _loadFileConfigBundleMethod = ELoadFileMethod.FileFromLocal;
    [SerializeField]
    protected string _urlFileConfigBundle = "URL Config Asset Bundle";
    public string UrlFileConfigBundle
    {
        get { return _urlFileConfigBundle; }
        set { _urlFileConfigBundle = value; }
    }

    [SerializeField]
    protected TextAsset _textConfigLocal = null;

    [SerializeField]
    protected EDownloadAssetBundleMethod _downloadAssetBundleMethod = EDownloadAssetBundleMethod.LoadFromCacheOrDownload;
    //[SerializeField]
    protected DMCPlatformBundleInfo _infoPlatformBundle;
    public DMCPlatformBundleInfo InfoPlatformBundle
    {
        get { return _infoPlatformBundle; }
    }

    //[SerializeField]
    protected string _platformName = "";
    //[SerializeField]
    private bool _isDownloadingAsset = false;

    /// <summary>
    /// Dictionary contain all AssetBundle of program
    /// </summary>
    private Dictionary<string, D2mAssetBundleData> _dicAssetBundleData = new Dictionary<string, D2mAssetBundleData>();

    protected long TotalRequiredFileSize = 100;
    #endregion

    #region Unity Methods
    void Start()
    {
        _platformName = D2mConstants.GetPlatformName();
        D2mServices.OnLoadConfigCompleted += CoreMobileManager_OnLoadConfigCompleted;
    }

    private void OnDestroy()
    {
        D2mServices.OnLoadConfigCompleted -= CoreMobileManager_OnLoadConfigCompleted;

    }
    #endregion

    private void CoreMobileManager_OnLoadConfigCompleted(D2mCoreMobileInfo info, bool success)
    {
        if (!string.IsNullOrEmpty(info.ConfigAppInfo.UrlAssetBundle))
        {
            _urlFileConfigBundle = info.ConfigAppInfo.UrlAssetBundle;
        }

        // Load Config File Bundle
        StartCoroutine(D2mConstants.AsyncGetDataFromUrl(_urlFileConfigBundle, _textConfigLocal, (string data) =>
        {
            if (!string.IsNullOrEmpty(data))
            {
                _assetBundleConfigInfo = JsonConvert.DeserializeObject<D2mAssetBundleConfigInfo>(data);
            }
#if UNITY_EDITOR
            var bundleInfo = _assetBundleConfigInfo.ListPlatformBundleInfo.Where(p => p.PlatformName.Equals(_platformName + "_Editor")).FirstOrDefault();
#else
                var bundleInfo = _assetBundleConfigInfo.ListPlatformBundleInfo.Where(p => p.PlatformName.Equals(_platformName)).FirstOrDefault();
#endif
            if (bundleInfo != null) _infoPlatformBundle = bundleInfo;

            OnDownloadConfigCompleted();

            DownloadAllAssetBundle();
        }));
    }

    #region Methods
    /// <summary>
    /// Initialization
    /// </summary>
    void Init()
    {
        _dicAssetBundleData.Clear();
    }

    public void DownloadAllAssetBundle()
    {
        if (_infoPlatformBundle != null)
        {
            StartCoroutine(AsyncDownloadAllAsset());
        }
        else
        {
            OnDownloadAllAssetCompleted(false);
        }
    }

    private bool _isLoadAllAssetSuccess = false;
    protected IEnumerator AsyncDownloadAllAsset()
    {
        Debug.Log(string.Format("<color=blue>AsyncDownloadAssetBundle platformName: {0}</color>", _platformName));
        if (_isDownloadingAsset) yield return new WaitWhile(() => _isDownloadingAsset);

        //if (!DMCMobileUtils.IsInternetAvailable())
        //{
        //    yield return new WaitForSeconds(_assetBundleConfigInfo.TimeWaitConnect);
        //}

        _isDownloadingAsset = true;
        _isLoadAllAssetSuccess = false;

        // Load Asset Bundles
        if (_infoPlatformBundle != null && _infoPlatformBundle.Enable)
        {
            _isLoadAllAssetSuccess = true;
            OnDownloadAllAssetStart();
            switch (_downloadAssetBundleMethod)
            {
                case EDownloadAssetBundleMethod.LoadFromCacheOrDownload:
                    foreach (D2mBundleInfo info in _infoPlatformBundle.BundleInfos)
                    {
                        yield return AsyncLoadFromCacheOrDownload(info, true);
                        yield return null;
                    }
                    break;
                default:
                    yield return null;
                    break;
            }
        }
        yield return null;

        if (_isLoadAllAssetSuccess) Debug.Log("<color=green>AsyncDownloadAllAsset OnLoadAllAssetCompleted</color>");
        else Debug.Log("<color=red>AsyncDownloadAllAsset OnLoadAllAssetFailed</color>");

        OnDownloadAllAssetCompleted(_isLoadAllAssetSuccess);
        _isDownloadingAsset = false;
        yield return null;

        yield return AsyncDownloadAllAssetInBackground();
    }

    protected IEnumerator AsyncDownloadAllAssetInBackground()
    {
        Debug.Log(string.Format("<color=blue>AsyncDownloadAllAssetInBackground platformName: {0}</color>", _platformName));

        if (_isDownloadingAsset) yield return new WaitWhile(() => _isDownloadingAsset);

        //int timeWaitConnect = 0;
        //while (!OSHelper.IsInternetAvailable && timeWaitConnect <= _assetBundleConfigInfo.TimeWaitConnect)
        //{
        //    timeWaitConnect += 1;
        //    yield return new WaitForSeconds(1);
        //}

        _isDownloadingAsset = true;
        _isLoadAllAssetSuccess = false;

        // Load Asset Bundles
        if (_infoPlatformBundle != null && _infoPlatformBundle.Enable)
        {
            _isLoadAllAssetSuccess = true;
            OnDownloadAllAssetStart();
            switch (_downloadAssetBundleMethod)
            {
                case EDownloadAssetBundleMethod.LoadFromCacheOrDownload:
                    foreach (D2mBundleInfo info in _infoPlatformBundle.BundleInfos)
                    {
                        yield return AsyncLoadFromCacheOrDownload(info, false);
                        yield return new WaitForSeconds(0.25f);
                    }
                    break;
                default:
                    yield return null;
                    break;
            }
        }
        yield return null;

        if (_isLoadAllAssetSuccess) Debug.Log("<color=green>AsyncDownloadAllAssetInBackground OnLoadAllAssetCompleted</color>");
        else Debug.Log("<color=red>AsyncDownloadAllAssetInBackground OnLoadAllAssetFailed</color>");
        _isDownloadingAsset = false;
    }

    #region LoadFromCacheOrDownload

    protected IEnumerator AsyncLoadFromCacheOrDownload(D2mBundleInfo info, bool isWhenStart)
    {
        if (!_dicAssetBundleData.ContainsKey(info.Name)
            && ((isWhenStart && info.IsDownloadWhenStart) || (!isWhenStart && !info.IsDownloadWhenStart)))
        {
            bool isDownloadSuccess = false;
            string urlAssetBundle = "";
            if (string.IsNullOrEmpty(info.UrlDownload) ||
                (!info.UrlDownload.StartsWith("http") && !info.UrlDownload.StartsWith("file:\\")))
            {
                urlAssetBundle = Path.Combine(_infoPlatformBundle.UrlServer, _platformName);
                urlAssetBundle = Path.Combine(urlAssetBundle, info.Name);
            }
            else
            {
                urlAssetBundle = info.UrlDownload;
            }

            if (urlAssetBundle.StartsWith(@"file://StreamingAssets"))
            {
#if UNITY_EDITOR
                var parentFolderAsset = Directory.GetParent(Application.dataPath);
                //urlAssetBundle = urlAssetBundle.Replace("StreamingAssets", Path.Combine(parentFolderAsset.FullName, "StreamingAssets"));
                urlAssetBundle = urlAssetBundle.Replace("StreamingAssets", Path.Combine(parentFolderAsset.FullName, "AssetBundles"));
                if (!Directory.Exists(urlAssetBundle))
                    urlAssetBundle = urlAssetBundle.Replace("StreamingAssets", Path.Combine(parentFolderAsset.FullName, "StreamingAssets")); ;
#elif UNITY_ANDROID
                    urlAssetBundle = "jar:" + urlAssetBundle;
                    urlAssetBundle = urlAssetBundle.Replace("StreamingAssets", Application.dataPath + "!/assets");
#elif UNITY_IOS
                    urlAssetBundle = urlAssetBundle.Replace("StreamingAssets", Path.Combine(Application.dataPath, "Raw"));
#endif
            }

            if (!string.IsNullOrEmpty(urlAssetBundle))
            {
                while (!Caching.ready)
                {
                    yield return null;
                }

                Debug.Log("<color=yellow> Start download asset NAME:" + info.Name + "</color>");
                Debug.Log("<color=blue>at url: " + urlAssetBundle + "</color>");
                //OnDownloadAssetStart?.Invoke(info.Name);
                OnDownloadAssetStart(info.Name);
                string nameAsset = info.Name.Replace('/', '_').Replace(@"\", "_");
                string keyHash = nameAsset + "_hash";
                string strHash = info.Version.ToString();

                Hash128 hash = default(Hash128);
                hash = Hash128.Parse(strHash);
                Debug.Log("<color=blue> strHash:" + strHash + "</color>");
                Debug.Log("<color=blue> hash:" + hash.ToString() + "</color>");
                CachedAssetBundle cachedAssetBundle = new CachedAssetBundle(nameAsset, hash);

                if (!Caching.IsVersionCached(cachedAssetBundle))
                {
                    Debug.Log("<color=red>hash:" + hash.ToString() + " isn't cached </color>");
                }
                else
                {
                    Debug.Log("<color=red>hash:" + hash.ToString() + " is cached </color>");
                }
#if UNITY_2018_3_OR_NEWER
                UnityWebRequest www = new UnityWebRequest();
                www = UnityWebRequestAssetBundle.GetAssetBundle(urlAssetBundle, cachedAssetBundle, 0u);

#if UNITY_EDITOR
                Coroutine watchDownloadingProgress = StartCoroutine(WatchDownloadingProgress(info.Name, www));
#endif

                yield return www.SendWebRequest();

#if UNITY_EDITOR
                StopCoroutine(watchDownloadingProgress);
#endif

                AssetBundle assetBundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
                if (www.isDone && string.IsNullOrEmpty(www.error) && assetBundle)
                {
                    D2mAssetBundleData bundleData = new D2mAssetBundleData(info, assetBundle);
                    _dicAssetBundleData[info.Name] = bundleData;
                    _isLoadAllAssetSuccess &= true;
                    isDownloadSuccess = true;
                }
                else
                {
                    Debug.Log(www.error);
                    _isLoadAllAssetSuccess &= false;
                }
                www.Dispose();
                www = null;
#else
                    //WWW www = WWW.LoadFromCacheOrDownload(info.LinkUrl, info.Version);
                    WWW www = WWW.LoadFromCacheOrDownload(urlAssetBundle, cachedAssetBundle, 0u);
                    //WWW www = WWW.LoadFromCacheOrDownload(urlAssetBundle, hash);
                    StartCoroutine(WatchDownloadingProgress(info.Name, www));
                    yield return www;
                    if (www.isDone && string.IsNullOrEmpty(www.error))
                    {
                        DMCAssetBundleData bundleData = new DMCAssetBundleData(info, www.assetBundle);
                        _dicAssetBundleData[info.Name] = bundleData;
                        _isLoadAllAssetSuccess &= true;
                        isDownloadAssetSuccess = true;
                    }
                    else
                    {
                        _isLoadAllAssetSuccess &= false;
                    }
                    www.Dispose();
                    www = null;
#endif
                //Resources.UnloadUnusedAssets();
            }

            if (isDownloadSuccess) Debug.Log("<color=green>Load Asset " + info.Name + " Success!</color>");
            else Debug.Log("<color=red>Load Asset " + info.Name + " Failed!</color>");
            //OnDownloadAssetCompleted?.Invoke(info.Name, isDownloadSuccess);
            OnDownloadAssetCompleted(info.Name, isDownloadSuccess);
        }

        yield return null;
    }

#if UNITY_2018_3_OR_NEWER
    protected IEnumerator WatchDownloadingProgress(string bundleName, UnityWebRequest www)
    {
        while (www != null && !www.isDone)
        {
            OnDownloadAssetProgress(bundleName, www.downloadProgress);
            if (_assetBundleConfigInfo.EnableLogProgress)
            {
                Debug.Log("<color=green>WatchDownloadingProgress " + bundleName + ": " + www.downloadProgress + "</color>");
            }
            yield return null;
        }

        OnDownloadAssetProgress(bundleName, 1);
        Debug.Log("<color=green>WatchDownloadingProgress " + bundleName + " is complete</color>");
    }
#else
        protected IEnumerator WatchDownloadingProgress(string bundleName, WWW www)
        {
            while (!www.isDone)
            {
                if (OnDownloadAssetProgress != null) OnDownloadAssetProgress(bundleName, www.progress);
                if (_assetBundleConfigInfo.EnableLogProgress)
                {
                    Debug.Log("WatchDownloadingProgress " + bundleName + ": " + www.progress);
                }
                yield return null;
            }

            if (OnDownloadAssetProgress != null) OnDownloadAssetProgress(bundleName, 1);
            //Debug.Log("<color=green>WatchDownloadingProgress " + bundleName + " is complete</color>");
        }
#endif

    public IEnumerator WatchRequestingProgress(string bundleName, AssetBundleRequest request)
    {
        while (!request.isDone)
        {
            if (_assetBundleConfigInfo.EnableLogProgress)
            {
                Debug.Log("WatchRequestingProgress " + bundleName + ": " + request.progress);
            }
            yield return null;
        }

        Debug.Log("<color=green>WatchRequestingProgress " + bundleName + " is complete</color>");
    }

    #endregion

    #region OLD
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="type">type objects in pack (prefab, scene, image, ...)</param>
    ///// <param name="subType">sub type, option of developer(clothes, image_ui, ...)</param>
    ///// <returns></returns>
    //public Dictionary<string, DMCAssetBundleData> GetAssetBundleData(string type, string subType)
    //{
    //    var assets = _dicAssetBundleData.Where(info => info.Value.bundleInfo.Type.ToLower().Equals(type.ToLower())
    //        && info.Value.bundleInfo.SubType.ToLower().Equals(subType.ToLower()));
    //    Dictionary<string, DMCAssetBundleData> dic = new Dictionary<string, DMCAssetBundleData>();
    //    foreach (var asset in assets)
    //    {
    //        dic[asset.Key] = asset.Value;
    //    }
    //    return dic;
    //}

    //public IEnumerator LoadAllAssetsAsync(string assetBundleName, Type type, Action<object> onComplete)
    //{
    //    if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
    //    {
    //        yield return null;
    //    }
    //    else
    //    {
    //        if (_dicAssetBundleData[assetBundleName].bundleInfo.Type.ToLower().Equals("prefab"))
    //        {
    //            AssetBundle bundle = _dicAssetBundleData[assetBundleName].assetBundle;
    //            AssetBundleRequest request = bundle.LoadAllAssetsAsync(type);
    //            StartCoroutine(WatchRequestingProgress(assetBundleName, request));
    //            yield return request;
    //            if (onComplete != null)
    //            {
    //                onComplete(request.allAssets);
    //            }
    //        }
    //        else
    //            yield return null;
    //    }
    //}

    //public IEnumerator LoadAssetAsync(string assetBundleName, string assetName, Type type, Action<object> onComplete)
    //{
    //    if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
    //    {
    //        Debug.Log("<color=red>LoadAssetAsync Can't exist " + assetBundleName + " bundle</color>");
    //        yield return null;
    //    }
    //    else
    //    {
    //        AssetBundle bundle = _dicAssetBundleData[assetBundleName].assetBundle;
    //        AssetBundleRequest request = bundle.LoadAssetAsync(assetName, type);
    //        StartCoroutine(WatchRequestingProgress(assetBundleName, request));
    //        yield return request;
    //        if (onComplete != null)
    //        {
    //            onComplete(request.asset);
    //        }
    //    }
    //}

    //public T[] LoadAllAssetAsync<T>(string assetBundleName) where T : UnityEngine.Object
    //{
    //    return null;
    //}

    //public T LoadAssetAsync<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
    //{
    //    //if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
    //    //{
    //    //    return null; //return new AssetBundle();
    //    //}
    //    //else
    //    //{
    //    //    if (_dicAssetBundleData[assetBundleName].bundleInfo.Type.ToLower().Equals("scene"))
    //    //    {
    //    //        AssetBundleRequest request = bundle.LoadAssetAsync(assetName, type);
    //    //        StartCoroutine(WatchRequestingProgress(assetBundleName, request));
    //    //        yield return request;
    //    //    }
    //    //    else
    //    //    {
    //    //        return null;
    //    //    }
    //    //}

    //    return null;
    //}

    //public AssetBundle GetAssetBundle(string assetBundleName)
    //{
    //    if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
    //    {
    //        return null; //return new AssetBundle();
    //    }
    //    else
    //    {
    //        //if (_dicAssetBundleData[assetBundleName].bundleInfo.Type.ToLower().Equals("scene"))
    //        //{
    //        //    return _dicAssetBundleData[assetBundleName].assetBundle;
    //        //}
    //        //else
    //        //{
    //        //    return null;
    //        //}

    //        return _dicAssetBundleData[assetBundleName].assetBundle;
    //    }
    //}
    #endregion


    public AssetBundleRequest GetAssetBundleRequest(string assetBundleName, string assetName, Type type)
    {
        if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
        {
            return null; //return new AssetBundle();
        }
        else
        {
            AssetBundle bundle = _dicAssetBundleData[assetBundleName].assetBundle;
            AssetBundleRequest request = bundle.LoadAssetAsync(assetName, type);
            return request;
        }
    }

    public IEnumerator AsyncLoadAllAssets(string assetBundleName, Type type, Action<UnityEngine.Object[]> onComplete)
    {
        if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
        {
            yield return null;
        }
        else
        {
            AssetBundle bundle = _dicAssetBundleData[assetBundleName].assetBundle;
            AssetBundleRequest request = bundle.LoadAllAssetsAsync(type);

#if UNITY_EDITOR
            StartCoroutine(WatchRequestingProgress(assetBundleName, request));
#endif

            yield return request;
            onComplete?.Invoke(request.allAssets);
        }
    }

    public IEnumerator AsyncLoadAllAssets(string assetBundleName, Action<UnityEngine.Object[]> onComplete)
    {
        if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
        {
            yield return null;
        }
        else
        {
            AssetBundle bundle = _dicAssetBundleData[assetBundleName].assetBundle;
            AssetBundleRequest request = bundle.LoadAllAssetsAsync<UnityEngine.Object>();

#if UNITY_EDITOR
            StartCoroutine(WatchRequestingProgress(assetBundleName, request));
#endif

            yield return request;
            onComplete?.Invoke(request.allAssets);
        }
    }


    public IEnumerator AsyncLoadAllAssetsByName(string assetBundleName, Action<UnityEngine.Object[]> onComplete)
    {
        if (!_dicAssetBundleData.ContainsKey(assetBundleName) || _dicAssetBundleData[assetBundleName] == null)
        {
            yield return null;
        }
        else
        {
            AssetBundle bundle = _dicAssetBundleData[assetBundleName].assetBundle;
            AssetBundleRequest request = bundle.LoadAllAssetsAsync<UnityEngine.Object>();

#if UNITY_EDITOR
            StartCoroutine(WatchRequestingProgress(assetBundleName, request));
#endif

            yield return request;
            onComplete?.Invoke(request.allAssets);
        }
    }

    public IEnumerator AsyncLoadAllAssets(ETypeAssetBundle typeAsset, Action<UnityEngine.Object[]> onComplete)
    {
        var pairs = _dicAssetBundleData.Where(ob => ob.Value.bundleInfo.Type == typeAsset).ToArray();

        List<UnityEngine.Object> allAssets = new List<UnityEngine.Object>();
        foreach (var pair in pairs)
        {
            AssetBundleRequest request = pair.Value.assetBundle.LoadAllAssetsAsync<UnityEngine.Object>();

#if UNITY_EDITOR
            StartCoroutine(WatchRequestingProgress(pair.Key, request));
#endif

            yield return request;
            allAssets.AddRange(request.allAssets);
        }

        onComplete?.Invoke(allAssets.ToArray());
    }

    public IEnumerator AsyncLoadAllAssets(ETypeAssetBundle typeAsset, ESubTypeAssetBundle subTypeAsset, Action<UnityEngine.Object[]> onComplete)
    {
        var pairs = _dicAssetBundleData.Where(ob => ob.Value.bundleInfo.Type == typeAsset && ob.Value.bundleInfo.SubType == subTypeAsset).ToArray();

        List<UnityEngine.Object> allAssets = new List<UnityEngine.Object>();
        foreach (var pair in pairs)
        {
            AssetBundleRequest request = pair.Value.assetBundle.LoadAllAssetsAsync<UnityEngine.Object>();

#if UNITY_EDITOR
            StartCoroutine(WatchRequestingProgress(pair.Key, request));
#endif

            yield return request;
            allAssets.AddRange(request.allAssets);
        }

        onComplete?.Invoke(allAssets.ToArray());
    }


    //public string GetVersion()
    //{
    //    if (_infoPlatformBundle != null)
    //    {
    //        return _infoPlatformBundle.Version;
    //    }
    //    else
    //    {
    //        return "Version zero";
    //    }
    //}
    #endregion


#if UNITY_EDITOR
    [ContextMenu("Save Info")]
    public void SaveInfo()
    {
        string str = JsonConvert.SerializeObject(_assetBundleConfigInfo);

        string pathDirectory = Path.Combine(Application.dataPath, D2mConstants.PathFolderData);
        D2mConstants.CreateDirectory(pathDirectory);
        string path = Path.Combine(pathDirectory, @"AssetBundle.txt");
        StreamWriter file = new System.IO.StreamWriter(path);
        file.WriteLine(str);
        file.Close();
        UnityEditor.AssetDatabase.Refresh();
    }

    [ContextMenu("Load Info")]
    public void LoadInfo()
    {
        string path = Path.Combine(Application.dataPath, D2mConstants.PathFolderData);
        path = Path.Combine(path, @"AssetBundle.txt");
        if (File.Exists(path))
        {
            string text = System.IO.File.ReadAllText(path);
            _assetBundleConfigInfo = JsonConvert.DeserializeObject<D2mAssetBundleConfigInfo>(text);
        }
    }
#endif
}