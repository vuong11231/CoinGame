using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace SteveRogers
{
    public class EditorDataMan : MonoBehaviour
    {
        [Serializable]
        private class Data
        {
            public string key = null;
            public string value = null;

            public bool BoolValue()
            {
                return value.Parse(0).ToBool();
            }
        }

        public static readonly string PATH = Directory.GetCurrentDirectory() + "\\EditorData\\EditorData.txt";

        [SerializeField]
        private Data[] list = null;

        private static Data[] dataFromFile = null;

        private static void Read()
        {
            if (dataFromFile != null) // already read
                return;

            if (File.Exists(PATH))
                dataFromFile = Utilities.SafeJsonDeserializeObject_FromFilepath<Data[]>(PATH, (e) => { });

            if (dataFromFile == null) // make sure it's not null after read
                dataFromFile = new Data[0];
        }

        public void Write()
        {
            if (list.IsNullOrEmpty())
                return;

            var c = JsonConvert.SerializeObject(list, Formatting.Indented);
            Utilities.WriteAllText(PATH, c, true);

            Utilities.WarningDone("Write Editor Data");
        }

        private static Data Get(string key)
        {
            Read();
            return Array.Find(dataFromFile, i => i.key.Equals(key));
        }

        public static bool IsTrue(string key)
        {
            var data = Get(key);
            return data != null && data.BoolValue();
        }
    }
}