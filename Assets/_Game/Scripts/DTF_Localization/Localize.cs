using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class managing UI text localization. Language specific strings shall be saved following this
/// folder structure:
///
///     Resources/localization/Text/English.txt 
///     Resources/localization/Text/Italian.txt 
///     Resources/localization/Text/Japanese.txt
///
/// ... and so on, where the file name (and consequently the resource name) is the string version of
/// the SystemLanguage enumeration.
///
/// The file format is as follows:
///
///     key=value
///
/// A TAB character is also accepted as key/value separator. 
/// In the value you can use the standard /// notation for newline: \n
/// </summary>
public class Localize : MonoBehaviour
{
    public static string Not_enough_skill_point = "not_enough_skill_point";
    public static string Level_up = "level_up";
    public static string Required_skill_point = "required_skill_point";
    public static string Lv = "lv";
    public static string Unlocked_at_lv = "unlocked_at_lv";
    public static string Not_enough_gems = "not_enough_gems";
    public static string Skill_point = "skill_point";
    public static string Stage = "stage";
    public static string Level = "level";
    public static string Feature_is_coming_soon = "feature_is_coming_soon";
    #region Public Fields

    public string localizationKey;

    #endregion Public Fields

    #region Private Fields
    
    private static LanguageType currentLanguage = LanguageType.Unknown;
    public static Dictionary<string, string> CurrentLanguageStrings = new Dictionary<string, string>();


    #endregion Private Fields

    #region Public Properties
    /// <summary>
    /// The player language. If not set in PlayerPrefs then returns Application.systemLanguage
    /// </summary>
    public static LanguageType PlayerLanguage
    {
        get
        {
            return LanguageManager.Instance.selectedLanguage;
        }
    }

    #endregion Public Properties

    #region Private Properties

    /// <summary>
    /// This sets the current language. It expects a standard .Net CultureInfo.Name format
    /// </summary>
    private static LanguageType CurrentLanguage
    {
        get { return currentLanguage; }
        set
        {
            currentLanguage = value;
            CurrentLanguageStrings = LanguageManager.Instance.GetLanguage(currentLanguage);
            if (CurrentLanguageStrings == null)
            {
                Log.Error("Can't define your language");
            }
        }
    }

    #endregion Private Properties

    #region Public Methods

    /// <summary>
    /// Returns the localized string for a given key
    /// </summary>
    /// <param name="key">the key to lookup</param>
    /// <returns></returns>
    public static string GetLocalizedString(string key)
    {
        if (CurrentLanguageStrings.ContainsKey(key))
            return CurrentLanguageStrings[key].Replace(@"\n", "" + '\n');
        else
#if UNITY_EDITOR
            return string.Empty + "Empty! " + key;
#else
        return string.Empty;
#endif
    }

    /// <summary>
    /// This is to set the language by code. It also update all the Text components in the scene.
    /// </summary>
    /// <param name="language">The new language</param>
    public static void SetCurrentLanguage(LanguageType language)
    {
        CurrentLanguage = language;
        Localize[] allTexts = (Localize[])Resources.FindObjectsOfTypeAll(typeof(Localize));
        for (int i = 0; i < allTexts.Length; i++)
        {
            allTexts[i].OnLocalize();
        }
    }

    /// <summary>
    /// This is to set the language by code. It also update all the Text components in the scene.
    /// </summary>
    /// <param name="language"></param>
    /// <param name="allTexts"></param>
    public static void SetCurrentLanguage(LanguageType language, Localize[] allTexts)
    {
        if (allTexts == null)
            return;
        CurrentLanguage = language;
        //PopupManager.Instance.lbLanguage.OnLocalize();
        for (int i = 0; i < allTexts.Length; i++)
        {
            allTexts[i].OnLocalize();
        }
    }

    /// <summary>
    /// Update the value of the Text we are attached to.
    /// </summary>
    public void OnLocalize()
    {
#if CUSTOM_SCENE
        CurrentLanguage = LanguageType.Vietnamese;
#endif
        if (CurrentLanguage == LanguageType.Unknown)
            return;
        if (/*!text || */!enabled || gameObject == null || !gameObject.activeInHierarchy) return; // catching race condition
        //Log.Info(localizationKey);
        if (!System.String.IsNullOrEmpty(localizationKey) && CurrentLanguageStrings.ContainsKey(localizationKey))
        {
            //text.text = CurrentLanguageStrings[localizationKey].Replace(@"\n", "" + '\n');
        }
    }

    #endregion Public Methods

    #region Private Methods

    // Use this for initialization
    private void OnEnable()
    {
        //text = GetComponent<UILabel>();
        OnLocalize();
    }

    #endregion Private Methods
}
