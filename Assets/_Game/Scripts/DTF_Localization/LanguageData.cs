using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class LanguageData : MonoBehaviour
{
    public LanguageType language;
    //public TextAsset data;
    public DictionaryOfStringAndString currentLanguageStrings = null;

    [ContextMenu("SetData")]
    public void SetData(int indexLanguage,int lengthLanguage,TextAsset data)
    {
        if (data == null)
            return;
        if (currentLanguageStrings == null)
            currentLanguageStrings = new DictionaryOfStringAndString();

        currentLanguageStrings.Clear();

        string dataText = data.text;
        string[] lines = dataText.Split(new string[] { "\r\n", "\n\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        currentLanguageStrings.Clear();
        string[] pairs;
        float progress;
        float totalProgress = (float)lines.Length * (float)lengthLanguage;
        for (int i = 0; i < lines.Length; i++)
        {
            //Log.Info("SetData:"+ lines[i]);
#if UNITY_EDITOR
            //progress = i + lines.Length * indexLanguage;
            //EditorUtility.DisplayProgressBar("Load Language", "Loading... " + "Localization/" + data.name+" ("+ progress + "/"+ totalProgress+")", progress / totalProgress);
#endif
            pairs = lines[i].Split(new char[] { '\t', '=' }, 2);
            if (pairs.Length == 2 && !pairs[0].Trim().Equals(string.Empty))
            {
                try
                {
                    Log.Info("Add LAnguage:" + pairs[0].Trim() + "=>" + pairs[1].Trim());
                    currentLanguageStrings.Add(pairs[0].Trim(), pairs[1].Trim());
                }
                catch (System.Exception)
                {
                    
                    print("Error Key: " + pairs[0].Trim());
                    throw;
                }

            }
        }
#if UNITY_EDITOR
        progress = indexLanguage;
        EditorUtility.DisplayProgressBar("Load Language", "Loading... " + "Localization/" + data.name+" ("+ progress + "/"+ lengthLanguage+")", progress / lengthLanguage);
#endif
    }

    public Dictionary<string, string> GetData()
    {
        //if (currentLanguageStrings == null)
        //    SetData();
        return currentLanguageStrings;
    }
}


[System.Serializable] public class DictionaryOfStringAndString : SerializableDictionary<string, string> { }
[System.Serializable] public class DictionaryOfNumberAndNumber : SerializableDictionary<int, int> { }

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    public List<TKey> keys = new List<TKey>();

    public List<TValue> values = new List<TValue>();

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
            throw new System.Exception(UtilityGame.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

        for (int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }


    public override string ToString()
    {
        var result = "";
        for (int i = 0; i < keys.Count; i++)
        {
            result += UtilityGame.Format("{0}:{1}", keys[i], values[i]);
            if (i < keys.Count - 1)
            {
                result += "&";
            }
        }
        if (result == "")
        {
            result = "Empty!";
        }
        return result;
    }
}