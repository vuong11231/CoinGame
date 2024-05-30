using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum TypeFont
{
    None,
    Unity,
    NGUI,
}

public class LanguageManager : SingletonMonoDontDestroy<LanguageManager>
{

    private const string KeySelectedLanguage = "SelectedLanguage";
    private const string KeyIsSelectedLanguage = "IsSelectedLanguage";

    [System.NonSerialized]
    public TypeFont typeFont = TypeFont.None;

    //[System.NonSerialized]
    public LanguageType selectedLanguage = LanguageType.English;

    public LanguageType[] listLanguageName;
    [System.NonSerialized]
    public LanguageType[] LanguageSupported = new LanguageType[]{
        LanguageType.Vietnamese,
        LanguageType.English,
        //LanguageType.French,
        //LanguageType.German,
        //LanguageType.Italian,
        //LanguageType.Indonesian,
        //LanguageType.Malaysia,
        //LanguageType.Portuguese,
        //LanguageType.Polish,
        //LanguageType.Korean,
        //LanguageType.Russian,
        //LanguageType.ChineseSimplified,
        //LanguageType.Thai,
        //LanguageType.Korean,
        //LanguageType.Japanese,
        //LanguageType.Spanish
    };
    [System.NonSerialized]
    public LanguageType[] LanguageUnityFont = new LanguageType[] {
        LanguageType.Korean,
        LanguageType.ChineseSimplified,
        LanguageType.Thai,
        LanguageType.Korean,
        LanguageType.Japanese
    };

    //[HideInInspector]
    public LanguageData[] languageData;

    /// <summary>
    /// Tất cả các Label active trong scene Loading.
    /// </summary>
    private Localize[] allTexts;
    public List<string> keyDebugs;

    private void Awake()
    {
        className = "LanguageManager";
    }

    public void Init()
    {

    }

    public void InitLanguageAtSceneLoadData()
    {
        InitLanguage();
    }

    [ContextMenu("Debug Languge")]
    public void LogLanguge()
    {
        List<int> langugeBugKeyLocal = new List<int>();
        Dictionary<string, List<string>> LogBuglanguge = new Dictionary<string, List<string>>();
        for (int keyIndex = 0; keyIndex < languageData[0].currentLanguageStrings.keys.Count; keyIndex++)
        {
            for (int langugeIndex = 1; langugeIndex < languageData.Length; langugeIndex++)
            {
                foreach (var item in keyDebugs)
                {
                    if (keyIndex >= languageData[langugeIndex].currentLanguageStrings.values.Count)
                    {
                        if (!langugeBugKeyLocal.Contains(langugeIndex))
                        {
                            langugeBugKeyLocal.Add(langugeIndex);
                        }
                    }
                    else
                    {
                        bool b1 = languageData[0].currentLanguageStrings.values[keyIndex].Contains(item);
                        bool b2 = b1;
                        if (languageData[0].currentLanguageStrings.keys[keyIndex] == languageData[langugeIndex].currentLanguageStrings.keys[keyIndex])
                        {
                            b2 = languageData[langugeIndex].currentLanguageStrings.values[keyIndex].Contains(item);
                        }
                        else
                        {
                            for (int i = 0; i < languageData[langugeIndex].currentLanguageStrings.keys.Count; i++)
                            {
                                if (languageData[0].currentLanguageStrings.keys[keyIndex] == languageData[langugeIndex].currentLanguageStrings.keys[i])
                                {
                                    b2 = languageData[langugeIndex].currentLanguageStrings.values[i].Contains(item);
                                }
                            }
                        }
                        if (b1 != b2)
                        {
                            string keyLocal = languageData[0].currentLanguageStrings.keys[keyIndex];
                            string keyLanguge = languageData[langugeIndex].name;
                            if (!LogBuglanguge.ContainsKey(keyLocal))
                            {
                                LogBuglanguge.Add(keyLocal, new List<string>() { keyLanguge });
                            }
                            else if (!LogBuglanguge[keyLocal].Contains(keyLanguge))
                            {
                                LogBuglanguge[keyLocal].Add(keyLanguge);
                            }
                        }
                    }
                }
            }
        }

        foreach (var langugeIndex in langugeBugKeyLocal)
        {
            foreach (var key in languageData[0].currentLanguageStrings.keys)
            {
                if (!languageData[langugeIndex].currentLanguageStrings.keys.Contains(key))
                {
                    Debug.Log(string.Format("Missing key ({0}) in languge ({1})", key, languageData[langugeIndex].name));
                }
            }
        }

        string log = "";
        foreach (var keyLocal in LogBuglanguge)
        {
            log += string.Format("Key: ({0}) \n", keyLocal.Key);
            foreach (var keyLanguge in keyLocal.Value)
            {
                log += string.Format("  ({0}) \n", keyLanguge);
            }
        }
        if (!string.IsNullOrEmpty(log))
        {
            Debug.LogError(log);
        }
    }

    //public void InitLanguage(Localize[] allTexts)
    //{
    //    //this.allTexts = allTexts;

    //    InitLanguage();

    //    this.StartDelayMethod(0.5f, OpenAskSelectLanguage);
    //}

    #region Public Methods

    public void LoadAllLanguages(List<TextAsset> datas)
    {

        languageData = null;

        languageData = new LanguageData[listLanguageName.Length];

        for (var i = 0; i < languageData.Length; i++)
        {

            languageData[i] = Resources.Load<GameObject>("Localization/" + listLanguageName[i].ToString()).GetComponent<LanguageData>();
            languageData[i].SetData(i,listLanguageName.Length,datas[i]);
        }

#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();
#endif
    }

    public void OpenAskSelectLanguage()
    {
        if (!IsSelectedLanguage())
        {
            PopupManager.Instance.ShowPopup(PopupType.SelectLanguage);
            SetLanguageSelected();
        }
    }

    public bool IsSelectedLanguage()
    {
        return PlayerPrefs.GetInt(KeyIsSelectedLanguage, UtilityGame.FALSE) == UtilityGame.TRUE;
    }

    public void SetLanguageSelected()
    {
        PlayerPrefs.SetInt(KeyIsSelectedLanguage, UtilityGame.TRUE);
    }

    public Dictionary<string, string> GetLanguage(LanguageType language)
    {
        foreach (LanguageData dt in languageData)
        {
            if (dt.language.Equals(language))
                return dt.GetData();
        }
        return null;
    }

    public void LoadSelectedLanguage()
    {
        selectedLanguage = (LanguageType)(PlayerPrefs.GetInt(KeySelectedLanguage));
        if (CheckSupport(selectedLanguage))
        {

        }
        else
        {
            selectedLanguage = LanguageType.English;
        }
    }

    public void SetLanguage(LanguageType language)
    {
        UpdateTypeFont(language);
        //  Vì scene Loading chỉ có ít label nên cho hết vào allTexts để set trực tiếp thay vì phải dùng hàm Find.
        //if (!UtilityGame.IsLoadedSceneMainMenu())
        //{

        //    Localize.SetCurrentLanguage(language, allTexts);
        //}
        //else
        //{
            Localize.SetCurrentLanguage(language);
        //}
        //Save
        selectedLanguage = language;
        PlayerPrefs.SetInt(KeySelectedLanguage, (int)selectedLanguage);
    }

    private void UpdateTypeFont(LanguageType language)
    {
        // set type font for language
        //if (Array.FindIndex(LanguageUnityFont, x => x.Equals(language)) == -1)
        //{
        //    typeFont = TypeFont.NGUI;
        //}
        //else
        //{
            typeFont = TypeFont.Unity;
        //}
    }

    private bool CheckSupport(LanguageType language)
    {
        bool isSupport = false;
        for (int i = 0; i < LanguageSupported.Length; i++)
        {
            if (LanguageSupported[i] == language)
            {
                isSupport = true;
                break;
            }
        }
        return isSupport;
    }

    #endregion Public Methods

    #region Private Methods
    public void InitLanguage()
    {
        if (!IsSelectedLanguage())
        {
#if UNITY_EDITOR
            selectedLanguage = LanguageType.Vietnamese;
#else
            selectedLanguage = (LanguageType)((int)Application.systemLanguage);
            //selectedLanguage = LanguageType.English;
#endif
            if (CheckSupport(selectedLanguage))
            {
            }
            else
            {
                selectedLanguage = LanguageType.English;
            }
        }
        else
        {
            LoadSelectedLanguage();
        }
        SetLanguage(selectedLanguage);
    }

    #endregion Private Methods

}

public enum LanguageType
{
    Afrikaans = 0,
    Arabic = 1,
    Basque = 2,
    Belarusian = 3,
    Bulgarian = 4,
    Catalan = 5,
    Chinese = 6,
    Czech = 7,
    Danish = 8,
    Dutch = 9,
    English = 10,
    Estonian = 11,
    Faroese = 12,
    Finnish = 13,
    French = 14,
    German = 15,
    Greek = 16,
    Hebrew = 17,
    Hungarian = 18,
    Icelandic = 19,
    Indonesian = 20,
    Italian = 21,
    Japanese = 22,
    Korean = 23,
    Latvian = 24,
    Lithuanian = 25,
    Norwegian = 26,
    Polish = 27,
    Portuguese = 28,
    Romanian = 29,
    Russian = 30,
    SerboCroatian = 31,
    Slovak = 32,
    Slovenian = 33,
    Spanish = 34,
    Swedish = 35,
    Thai = 36,
    Turkish = 37,
    Ukrainian = 38,
    Vietnamese = 39,
    ChineseSimplified = 40,
    ChineseTraditional = 41,
    Unknown = 42,
    Malaysia = 43,
}