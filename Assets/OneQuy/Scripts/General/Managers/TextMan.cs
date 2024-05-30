using System;
using System.Collections;
using UnityEngine;

namespace SteveRogers
{
    /// <summary>
    /// Main
    /// </summary>
    public partial class TextMan : SingletonPersistentStatic<TextMan>
    {
        public static int LangIndex { set; get; } = 0;
        public static bool Inited { private set; get; } = false;

        [SerializeField]
        private Data data;

        [SerializeField]
        private bool warningIfNotFound = true;

        private void Start()
        {
            data.CheckAndParseSheetToArray();
            Inited = true;
        }

        public static IEnumerator Process(string targetLang, string sourceText)
        {
            // We use Auto by default to determine if google can figure it out.. sometimes it can't.
            string sourceLang = "auto";
            // Construct the url using our variables and googles api.
            string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
                + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + WWW.EscapeURL(sourceText);

            // Put together our unity bits for the web call.
            WWW www = new WWW(url);
            // Now we actually make the call and wait for it to finish.
            yield return www;

            // Check to see if it's done.
            if (www.isDone)
            {
                // Check to see if we don't have any errors.
                if (string.IsNullOrEmpty(www.error))
                {
                    print(www.text);

                    //// Parse the response using JSON.
                    //var N = JSONNode.Parse(www.text);
                    //// Dig through and take apart the text to get to the good stuff.
                    //translatedText = N[0][0][0];
                    //// This is purely for debugging in the Editor to see if it's the word you wanted.
                    //if (isDebug)
                    //	print(translatedText);
                }
            }
        }

        public static string Get(string id, string defaultText = null)
        {
            CheckInit();
            var content = Instance.data.Get(id);

            if (Instance.warningIfNotFound && content == null)
                Debug.LogWarning("TextMan not found id = " + id);

            return content ?? (defaultText ?? id);
        }
    }
    
    /// <summary>
    /// Models
    /// </summary>
    public partial class TextMan : SingletonPersistentStatic<TextMan>
    {
        [Serializable]
        public struct Text // main model
        {
            [SerializeField]
            private string id;

            [SerializeField]
            private string[] texts;

            private int hash;            

            public Text(string id, string[] texts)
            {
                if (id.IsNullOrEmpty() || texts.IsNullOrEmpty())
                    throw new Exception(Utilities.CreateLogContent("ID / texts is null!", id, texts.IsNullOrEmpty() ? "null" : texts[0]));

                this.id = id;
                hash = id.GetHashCode();
                this.texts = texts;
            }

            public string Get()
            {
                var res = texts.Get(TextMan.LangIndex);
                //return res ?? throw new Exception(Utilities.CreateLogContent("LangIndex is out of index: " + TextMan.LangIndex, id, texts.IsNullOrEmpty() ? "null" : texts[0]));
                return res ?? null;
            }

            public bool Equals(int hash)
            {
                if (this.hash == 0)
                {
                    this.hash = id.GetHashCode();

                    if (this.hash == 0)
                        throw new Exception("Hash == 0, id = " + id);
                }

                return this.hash == hash;
            }
        }

        [Serializable]
        public struct Data
        {
            [SerializeField]
            private string sheetResourcesPath;

            [SerializeField]
            private Text[] texts;

            public Data(Text[] texts)
            {
                this.texts = texts;
                sheetResourcesPath = null;
            }

            public void CheckAndParseSheetToArray()
            {
                if (texts.IsValid())
                    return;

                if (sheetResourcesPath.IsNullOrEmpty())
                    throw new Exception("ParseSheetToArray filepath is null");

                var lines = Utilities.ReadCSV_FromResources_WithSplit(sheetResourcesPath);

                if (lines.IsNullOrEmpty())
                    throw new Exception("ParseSheetToArray got null from input file / filepath");

                texts = new Text[lines.Count];

                for (int i = 0; i < texts.Length; i++)
                {
                    var id = lines[i][0];
                    lines[i].RemoveAt(0);
                    Text text = new Text(id, lines[i].ToArray());
                    texts[i] = text;
                }
            }

            public string Get(string id)
            {
                if (id == null)
                    throw new Exception("TextMan.Data.Get() param is null.");

                if (texts == null)
                    throw new Exception("TextMan.Data.texts is null, id = " + id);

                int hash = id.GetHashCode();

                foreach (var i in texts)
                {
                    if (i.Equals(hash))
                        return i.Get();
                }

                return null;
                //throw new Exception("TextMan.Data not found ID: " + id);
            }
        }
    }
}