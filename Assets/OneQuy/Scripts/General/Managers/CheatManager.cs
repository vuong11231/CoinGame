using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

namespace SteveRogers
{
    public enum CheatTag { Fps, Stats, _Count }

    public class CheatManager : SingletonPersistent<CheatManager>
    {
        [Serializable]
        public class Cheat
        {
            public CheatTag tag;
            public CheatValue value;
        }

        [Serializable]
        public class CheatValue
        {
            public bool active = true;
            public float floatValue = 0f;
            public string stringValue = string.Empty;
        }

        #region VARIABLES (PRIVATE)

#if UNITY_EDITOR
        [SerializeField, Header("Runtime")]
        private bool runtimeDisable = false;
#endif
        [Header("For Set Default")]

        public Cheat[] defaultValue;

        [Header("Build Data")]

        public Cheat[] buildData;

        private static System.Collections.Generic.Dictionary<CheatTag, CheatValue> data = new System.Collections.Generic.Dictionary<CheatTag, CheatValue>();
        private static bool activated = false;

        private readonly Rect POS = new Rect(Screen.width - 35, Screen.height - 30, 100, 100);
        private const string CHEAT_TEXT = "dev";
        private const string CHEAT_FILE_NAME = "cheatdata";

        #endregion

        #region CORE (PRIVATE)

        protected override void Awake()
        {
            base.Awake();
            Load();

            if (activated)
            {
                if (data[CheatTag.Fps].active)
                    Utilities.CreateFPSShower();

                if (data[CheatTag.Stats].active)
                    Utilities.CreateStatsShower();
            }
        }

        public static string GetFolderPath(bool withSeparator)
        {
#if UNITY_EDITOR
            if (withSeparator)
                return Path.Combine(Application.persistentDataPath, "Cheat") + Path.DirectorySeparatorChar;
            else
                return Path.Combine(Application.persistentDataPath, "Cheat");
#else
            if (withSeparator)
                return Application.persistentDataPath + Path.DirectorySeparatorChar;
            else
                return Application.persistentDataPath;
#endif
        }

        public static string Filepath
        {
            get
            {
                return GetFolderPath(true) + CHEAT_FILE_NAME;
            }
        }


        private static string ReadCheatFile()
        {
#if UNITY_EDITOR
            return Utilities.ReadAllTextSafe(Filepath);
#else
            if (Utilities.IsDebugDevice)
                return Utilities.ReadAllTextSafe(Filepath);
            else
                return null;
#endif
        }

        private static void Load()
        {
            string text = ReadCheatFile();

            if (!string.IsNullOrEmpty(text))
            {
                Cheat[] list = Utilities.SafeJsonDeserializeObject<Cheat[]>(text);

                if (list != null)
                {
                    foreach (Cheat i in list)
                        data[i.tag] = i.value;
                }
            }

            if (data.Count > 0)
                activated = true;

            // Fill missing tags equal to FALSE

            for (CheatTag tag = 0; tag <= CheatTag._Count; tag++)
                if (!data.ContainsKey(tag))
                    data[tag] = new CheatValue { active = false };
        }

        private void OnGUI()
        {
            if (activated)
            {
                GUI.Label(POS, CHEAT_TEXT);
            }
        }

        #endregion

        #region GET VALUE (PUBLIC)

        public static bool IsActive(CheatTag tag)
        {
#if UNITY_EDITOR
            if (Instance.runtimeDisable || !data[tag].active)
#else
            if (!data[tag].active)
#endif
                return false;

            return true;
        }

        public static float GetValue(CheatTag tag, float defaultValue)
        {
            if (IsActive(tag))
                return data[tag].floatValue;
            else
                return defaultValue;
        }

        public static int GetValue(CheatTag tag, int defaultValue)
        {
            if (IsActive(tag))
                return (int)data[tag].floatValue;
            else
                return defaultValue;
        }

        public static string GetValue(CheatTag tag, string defaultValue)
        {
            if (IsActive(tag))
                return data[tag].stringValue;
            else
                return defaultValue;
        }

        public static void SetValue(CheatTag tag, bool active)
        {
            if (active)
                activated = true;

            data[tag].active = active;
        }

        public static void ActiveAndSetValue(CheatTag tag, int value)
        {
            SetValue(tag, true);
            data[tag].floatValue = value;
        }

        public static void SetValue(CheatTag tag, ref int defaultRef, int value)
        {
            if (IsActive(tag))
                data[tag].floatValue = value;
            else
                defaultRef = value;
        }

        public static void SetValueAppend(CheatTag tag, ref int defaultRef, int value)
        {
            if (IsActive(tag))
                data[tag].floatValue += value;
            else
                defaultRef += value;
        }

        public static void SetValueAppend(CheatTag tag, int value, Action onNotActive)
        {
            if (IsActive(tag))
                data[tag].floatValue += value;
            else
                onNotActive();
        }

        public static void SetValue(CheatTag tag, int value, Action onNotActive)
        {
            if (IsActive(tag))
                data[tag].floatValue = value;
            else
                onNotActive();
        }

        #endregion
    }
}