using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace SteveRogers
{
    public class LogMan : SingletonPersistent<LogMan>, System.IDisposable
    {
        private const string FORMAT = "[{2}]{3}\n{0}\n{1}\n----------------------\n\n";

        [SerializeField]
        private string[] excludeConditions = new string[]
        {
            "atlasRequested wasn't listened",
        };

        private StringBuilder builder = null;
        private string logFilepath { set; get; } = null;
        private bool isActive = false;

        public delegate string StateContent();

        public StateContent StateContentCmd { private get; set; } = null;

        public bool IsActive
        {
            get { return isActive; }

            set
            {
                if (value == isActive)
                    return;
                
                isActive = value;

                if (isActive)
                    Application.logMessageReceived += Application_logMessageReceived;
                else
                    Application.logMessageReceived -= Application_logMessageReceived;
            }
        }

        public void Dispose()
        {
            if (builder != null)
            {
                builder.Clear();
                builder = null;
            }

            IsActive = false;
        }

        public void Clear()
        {
            if (builder != null)
                builder.Clear();
        }

        public string Text
        {
            get
            {
                if (builder == null)
                    return null;
                else
                    return builder.ToString();
            }
        }

        public string ReloadFromFile()
        {
            if (System.IO.File.Exists(logFilepath))
                return Utilities.ReadAllText(logFilepath);
            else
                return string.Empty;
        }

        private void Start()
        {
            if (MonoManager.Instance)
            {
                logFilepath = Utilities.GetRuntimeDataFolderPath(true) + "LogMan.txt";
                MonoManager.DoOnPause += OnAppPause;
            }
            else
            {
                Debug.LogError("MonoManager not instanced to setup LogMan!");
                Dispose();
                Destroy(this);
            }
        }

        private void OnAppPause(bool pause)
        {
            if (pause && IsActive)
            {
                var v = Text;

                if (v.IsValid())
                    Utilities.WriteAllText(logFilepath, v, true);
            }
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            foreach (var i in excludeConditions)
                if (condition.Contains(i))
                    return;

            if (builder == null)
                builder = new StringBuilder();

            builder.AppendFormat(FORMAT, condition, stackTrace, type, StateContentCmd == null ? string.Empty : StateContentCmd());
        }
    }
}