using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace SteveRogers
{
    public class PlayerPrefBackup
    {
        [Serializable]
        private struct PlayerPrefPair
        {
            public enum TheType
            {
                None, String, Int, Float, _Count
            }

            public TheType Type
            {
                get;
                set;
            }

            public string Key
            {
                get;
                set;
            }

            public object Value
            {
                get;
                set;
            }

            public void SaveToPlayerPref()
            {
                //try
                //{
                    switch (Type)
                    {
                        case TheType.String:
                            PlayerPrefs.SetString(Key, Value.ToString());
                            break;

                        case TheType.Int:
                            PlayerPrefs.SetInt(Key, int.Parse(Value.ToString()));
                            break;

                        case TheType.Float:
                            PlayerPrefs.SetFloat(Key, float.Parse(Value.ToString()));
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                //}
                //catch (Exception e)
                //{
                //    Debug.LogError(Utilities.CreateLogContent("SaveToPlayerPref failed", Key, Value, Type, e.ToString()));
                //}
            }
        }

        public static readonly string PATH = Path.Combine(Directory.GetCurrentDirectory(), "EditorData", "PlayerPrefBackup.txt");

        private static PlayerPrefPair[] Get()
        {
            // From Unity docs: On Windows, PlayerPrefs are stored in the registry under HKCU\Software\[company name]\[product name] key, where company and product names are the names set up in Project Settings.
#if UNITY_5_5_OR_NEWER
            // From Unity 5.5 editor player prefs moved to a specific location
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Unity\\UnityEditor\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName);
#else
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName);
#endif

            // Parse the registry if the specified registryKey exists
            if (registryKey != null)
            {
                // Get an array of what keys (registry value names) are stored
                string[] valueNames = registryKey.GetValueNames();

                // Create the array of the right size to take the saved player prefs
                PlayerPrefPair[] tempPlayerPrefs = new PlayerPrefPair[valueNames.Length];

                // Parse and convert the registry saved player prefs into our array
                int i = 0;
                foreach (string valueName in valueNames)
                {
                    string key = valueName;

                    // Remove the _h193410979 style suffix used on player pref keys in Windows registry
                    int index = key.LastIndexOf("_");
                    key = key.Remove(index, key.Length - index);

                    // Get the value from the registry
                    object ambiguousValue = registryKey.GetValue(valueName);
                    PlayerPrefPair.TheType type = PlayerPrefPair.TheType.None;

                    // Unfortunately floats will come back as an int (at least on 64 bit) because the float is stored as
                    // 64 bit but marked as 32 bit - which confuses the GetValue() method greatly! 
                    if (ambiguousValue.GetType() == typeof(int))
                    {
                        // If the player pref is not actually an int then it must be a float, this will evaluate to true
                        // (impossible for it to be 0 and -1 at the same time)
                        if (PlayerPrefs.GetInt(key, -1) == -1 && PlayerPrefs.GetInt(key, 0) == 0)
                        {
                            // Fetch the float value from PlayerPrefs in memory
                            ambiguousValue = PlayerPrefs.GetFloat(key);
                            type = PlayerPrefPair.TheType.Float;
                        }
                        else
                            type = PlayerPrefPair.TheType.Int;
                    }
                    else if (ambiguousValue.GetType() == typeof(byte[]))
                    {
                        // On Unity 5 a string may be stored as binary, so convert it back to a string
                        ambiguousValue = System.Text.Encoding.Default.GetString((byte[])ambiguousValue);
                        type = PlayerPrefPair.TheType.String;
                    }
                    // Assign the key and value into the respective record in our output array
                    tempPlayerPrefs[i] = new PlayerPrefPair() { Type = type, Key = key, Value = ambiguousValue };
                    //Debug.Log(type + " - " + key + " - " + ambiguousValue);
                    i++;
                }
                // Return the results
                return tempPlayerPrefs;
            }
            else
            {
                // No existing player prefs saved (which is valid), so just return an empty array
                return null;
            }
        }
                       
        public static void Backup(bool force)
        {
            if (!force && File.Exists(PATH))
            {
                if (!EditorUtility.DisplayDialog("Confirm", "Already file backup is existed, overwrite?", "OK", "CANCEL"))
                    return;
            }

            var list = Get();
            var c = Newtonsoft.Json.JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented);
            Utilities.WriteAllText(PATH, c, true);
            Utilities.WarningDone("Backup PlayerPref");
        }        
        
        public static void Restore()
        {
            string er = null;
            var list = Utilities.SafeJsonDeserializeObject_FromFilepath<PlayerPrefPair[]>(PATH, (e) => er = e);

            if (list == null)
            {
                Debug.LogError("Cant restore: " + er);
                return;
            }

            foreach (var i in list)
                i.SaveToPlayerPref();

            Utilities.WarningDone("Restore PlayerPref");
        }
    }
}