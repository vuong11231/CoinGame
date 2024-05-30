    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System.IO;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine.Networking;
    //using DG.Tweening;
    using System.Collections.Generic;

public static class D2mConstants
{
    #region Const, Static Variables
    public const string DEFINE_ADMOB = "DEFINE_ADMOB";
    public const string DEFINE_AD_NATIVE = "DEFINE_AD_NATIVE";
    public const string DEFINE_UNITY_ADS = "DEFINE_UNITY_ADS";
    public const string DEFINE_FACEBOOK_ADS = "DEFINE_FACEBOOK_ADS";

    public const string DEFINE_FIREBASE_ANALYTIC = "DEFINE_FIREBASE_ANALYTIC";
    public const string DEFINE_FIREBASE_CRASHLYTIC = "DEFINE_FIREBASE_CRASHLYTIC";
    public const string DEFINE_FIREBASE_MESSAGING = "DEFINE_FIREBASE_MESSAGING";
    public const string DEFINE_FIREBASE_AUTH = "DEFINE_FIREBASE_AUTH";

    public const string DEFINE_IAP = "DEFINE_IAP";

    public const string DEFINE_GAMESERVICES = "DEFINE_GAMESERVICES";
    public const string DEFINE_FACEBOOK_SDK = "DEFINE_FACEBOOK_SDK";

    public const string KeySoundGame = "KeySound";
    public const string KeyMusicGame = "KeyMusic";
    public const string KeyVibrate = "KeyVibrate";

    public const string KeyRemoveAds = "KeyRemoveAds";


    /// <summary>
    /// Create name of audio (sound or music) at here (this is demo)
    /// </summary>
    public const string MusicMenuGame = "Lounge Game1";
    public const string SoundButtonClick = "Button Click";


    public const string PathFolderData = "Data";

    public const string KeyCoreDataInfo = "KeyCoreDataInfo";

    // Database
    public const string DATABASE_USER = "userprofile";
    #endregion


    public static string GetPlatformName()
    {
        //return GetPlatformForAssetBundles(Application.platform);
#if UNITY_EDITOR
        return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#endif
        return GetPlatformForAssetBundles(Application.platform);
    }

#if UNITY_EDITOR
    private static string GetPlatformForAssetBundles(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "iOS";
            case BuildTarget.WebGL:
                return "WebGL";
#if !UNITY_5_3_OR_NEWER
                case BuildTarget.WebPlayer:
				return "WebPlayer";
#endif
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
#if !UNITY_2018_1_OR_NEWER
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
#endif
#if UNITY_2017_3_OR_NEWER
            case BuildTarget.StandaloneOSX:
                return "OSX";
#endif
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
            default:
                return null;
        }
    }
#endif

    private static string GetPlatformForAssetBundles(RuntimePlatform platform)
    {
        switch (platform)
        {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            case RuntimePlatform.WebGLPlayer:
                return "WebGL";
#if !UNITY_5_3_OR_NEWER
                case RuntimePlatform.OSXWebPlayer:
				case RuntimePlatform.WindowsWebPlayer:
				return "WebPlayer";
#endif
            case RuntimePlatform.WindowsPlayer:
                return "Windows";
            case RuntimePlatform.OSXPlayer:
                return "OSX";
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
            default:
                return null;
        }
    }


    public static string GetStreamingAssetsPath()
    {
#if UNITY_EDITOR
        //return Application.streamingAssetsPath; // Use the build output folder directly.
        return Path.Combine(Directory.GetCurrentDirectory(), "StreamingAssets");
        //return "file://" + Application.streamingAssetsPath;
#elif UNITY_WEBGL
			//return Path.GetDirectoryName(Application.absoluteURL).Replace("\\", "/")+ "/StreamingAssets";
            return Path.Combine(Path.GetDirectoryName(Application.absoluteURL).Replace("\\", "/"), "StreamingAssets");
#elif UNITY_STANDALONE
            //return Application.streamingAssetsPath;
            return Application.dataPath + "/StreamingAssets";
#elif UNITY_ANDROID
            return "jar:file://" + Application.dataPath + "!/assets";
#elif UNITY_IOS
            return Application.dataPath + "/Raw";
#else
            return "file://" + Application.streamingAssetsPath;
#endif
    }


    public static void OpenURL(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
#if UNITY_EDITOR
            Application.OpenURL(url);
#elif UNITY_WEBGL
				//Application.ExternalEval("window.open(\"" + url + "\",\"_blank\")");
				openWindow(url);
#else
				Application.OpenURL(url);
#endif
        }
    }

    public static bool IsInternetAvailable()
    {
        return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
    }

    #region Languages

    public static string GetLanguageName(string languageCode)
    {
        string text = languageCode.ToLower();
        switch (text)
        {
            case "bn":
                return "Bengali";
            case "de":
                return "Germany";
            case "es":
                return "Spanish";
            case "fr":
                return "French";
            case "id":
                return "Indonesia";
            case "in":
                return "Hindi";
            case "it":
                return "Italy";
            case "jp":
                return "Japanese";
            case "ko":
                return "Korean";
            case "mr":
                return "Marathi";
            case "pa":
                return "Punjabi";
            case "pt":
                return "Portguese";
            case "ru":
                return "Russian";
            case "ta":
                return "Tamil";
            case "te":
                return "Telugu";
            case "tr":
                return "Turkish";
            case "za":
                return "Swahili";
        }
        return "NA";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="FlagTag"></param>
    /// <returns></returns>
    public static string LanguageCodeFromCountryCode(string FlagTag)
    {
        string text = FlagTag.ToUpper();
        switch (text)
        {
            case "BN":
                return "bn";
            case "DE":
                return "de";
            case "ES":
                return "es";
            case "FR":
                return "fr";
            case "ID":
                return "in";
            case "IN":
                return "hi";
            case "IT":
                return "it";
            case "JP":
                return "ja";
            case "KO":
                return "ko";
            case "MR":
                return "mr";
            case "PA":
                return "pa";
            case "PT":
                return "pt";
            case "RU":
                return "ru";
            case "TA":
                return "ta";
            case "TE":
                return "te";
            case "TR":
                return "tr";
            case "ZA":
                return "sw";
        }
        return "NA";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="CountryCode"></param>
    /// <returns></returns>
    public static string CountryCodeFromLanguageCode(string CountryCode)
    {
        switch (CountryCode)
        {
            case "BN":
                return "BN";
            case "DE":
                return "DE";
            case "EN":
                return "EN";
            case "ES":
                return "ES";
            case "FR":
                return "FR";
            case "HI":
                return "IN";
            case "IN":
                return "ID";
            case "IT":
                return "it";
            case "JA":
                return "JP";
            case "KO":
                return "KO";
            case "MR":
                return "MR";
            case "PA":
                return "PA";
            case "PT":
                return "PT";
            case "RU":
                return "RU";
            case "TA":
                return "TA";
            case "TE":
                return "TE";
            case "TR":
                return "TR";
            case "SW":
                return "ZA";
        }
        return "NA";
    }

    #endregion


    #region Files and Folders
    public static void CreateDirectory(string path)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        if (!directoryInfo.Parent.Exists)
        {
            CreateDirectory(directoryInfo.Parent.FullName);
        }
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
    #endregion

    #region Get Content File From Server
    public static IEnumerator AsyncGetDataFromUrl(string url,
#if UNITY_2018_3_OR_NEWER
            Action<UnityWebRequest> callBack
#else
            Action<WWW> callBack
#endif
            )
    {
        if (!string.IsNullOrEmpty(url))
        {
#if UNITY_2018_3_OR_NEWER
            UnityWebRequest www = UnityWebRequest.Get(url);
            www.SendWebRequest();
#else
                WWW www = new WWW(url);
#endif
            if (callBack != null) callBack(www);
        }
        yield return null;
    }

    public static IEnumerator AsyncGetDataFromUrl(string url, Action<string> callBack)
    {
        string textData = "";
        if (!string.IsNullOrEmpty(url))
        {
#if UNITY_2018_3_OR_NEWER
            UnityWebRequest www = UnityWebRequest.Get(url);
            www.SendWebRequest();
#else
                WWW www = new WWW(url);
#endif

            while (!www.isDone)
            {
                yield return null;
            }
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                textData = "";
            }
            else
            {
#if UNITY_2018_3_OR_NEWER
                textData = www.downloadHandler.text;
#else
                    textData = www.text;
#endif
            }
        }
        if (callBack != null) callBack(textData);
        yield return null;
    }

    public static IEnumerator AsyncGetDataFromUrl(string url, TextAsset textAssetDefault, Action<string> callBack)
    {
        string textData = "";
        if (!string.IsNullOrEmpty(url))
        {
#if UNITY_2018_3_OR_NEWER
            UnityWebRequest www = UnityWebRequest.Get(url);
            www.SendWebRequest();
#else
                WWW www = new WWW(url);
#endif

            while (!www.isDone)
            {
                yield return null;
            }
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                textData = "";
            }
            else
            {
#if UNITY_2018_3_OR_NEWER
                textData = www.downloadHandler.text;
#else
                    textData = www.text;
#endif
            }
        }
        if (string.IsNullOrEmpty(textData) && textAssetDefault)
        {
            textData = textAssetDefault.text;
        }

        if (callBack != null) callBack(textData);
        yield return null;
    }
    #endregion

    public static void LoadScene(string sceneName, LoadSceneMode sceneMode)
    {
        SceneManager.LoadScene(sceneName, sceneMode);
    }

    public static IEnumerator AsyncLoadScene(string sceneName, LoadSceneMode sceneMode, System.Action callBack = null)
    {
        yield return null;
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, sceneMode);

        // Wait until the asynchronous scene fully loads
        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if (callBack != null) callBack();
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static string ToString(this int @this, bool isShortened)
    {
        if (isShortened)
        {
            if (@this >= System.Math.Pow(10, 9))
            {
                return string.Format("{0:0.##}B", @this / System.Math.Pow(10, 9));
            }
            else if (@this >= System.Math.Pow(10, 6))
            {
                return string.Format("{0:0.##}M", @this / System.Math.Pow(10, 6));
            }
            else if (@this >= System.Math.Pow(10, 3))
            {
                return string.Format("{0:0.##}K", @this / System.Math.Pow(10, 3));
            }
            else
            {
                return @this.ToString();
            }
        }
        else
        {
            return @this.ToString();
        }
    }

    //public static void ClearTween(this Tween target, bool complete = false)
    //{
    //    if (target != null)
    //    {
    //        target.Kill(complete);
    //        target = null;
    //    }
    //}

    //public static void CompleteTween(this Tween target, bool withCallback = false)
    //{
    //    if (target != null)
    //    {
    //        target.Complete(withCallback);
    //        target = null;
    //    }
    //}

    public static System.Object[] ShuffleArraySequence(System.Object[] array1, System.Object[] array2)
    {
        if (array1 == null) return array2;
        if (array2 == null) return array1;
        List<System.Object> array = new List<System.Object>();
        bool minIsArray1 = array1.Length < array2.Length;
        int min = Mathf.Min(array1.Length, array2.Length);
        int max = Mathf.Max(array1.Length, array2.Length);
        for (int i = 0; i < min; i++)
        {
            array.Add(array1[i]);
            array.Add(array2[i]);
        }
        for (int i = min; i < max; i++)
        {
            if (minIsArray1) array.Add(array2[i]);
            else array.Add(array1[i]);
        }
        return array.ToArray();
    }

    public static List<string> SubStringLimitLenght(this string target, int len)
    {
        List<string> list = new List<string>();
        while (target.Length > len)
        {
            list.Add(target.Substring(0, len));
            target = target.Substring(len, target.Length - len);
        }
        if (target.Length > 0) list.Add(target);
        return list;
    }

#if UNITY_EDITOR
    public static string GetDefines()
    {
        string defines = "";

#if UNITY_ANDROID
        defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
#elif UNITY_IOS
			defines = PlayerSettings.GetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS);
#elif UNITY_WEBGL
			defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL);
#elif UNITY_STANDALONE
			defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
#else
#endif
        return defines;
    }

    public static void SetDefines(string defines)
    {
#if UNITY_ANDROID
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines);
#elif UNITY_IOS
			PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS, defines);
#elif UNITY_WEBGL
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, defines);
#elif UNITY_STANDALONE
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines);
#else
#endif
    }
#endif
}