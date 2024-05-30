using System.IO;
using System.Collections.Generic;
using UnityEngine;
using SteveRogers;

// something!\0*

namespace SteveRogers
{
    public static class Cheat
    {
        private static string[] lines = null;
        private static bool loadedData = false;

        private static string Filepath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "cheat.txt");
            }
        }

        private static string FindLine(string key)
        {
            LoadData();

            if (lines.IsNullOrEmpty())
                return null;

            foreach (var l in lines)
                if (l.StartsWith(key))
                    return l;

            return null;
        }

        private static void LoadData()
        {
            if (loadedData)
                return;

            loadedData = true;

            if (File.Exists(Filepath))
                lines = File.ReadAllLines(Filepath);
        }

        private static bool IsInactiveLine(string line)
        {
            if (line.IsNullOrEmpty())
                return true;

            return line[line.Length - 1].Equals('*');
        }

        private static void CheckAndLogActiveLine(ref string line, ref string key)
        {
            if (line.IsNullOrEmpty())
                return;

            if (line.Contains("#"))
                Debug.Log("cheat: " + key);

            else if (line.Contains("@"))
                Debug.LogWarning("cheat: " + key);

            else if (line.Contains("!"))
                Debug.LogError("cheat: " + key);
        }

        // bool

        public static bool Get(string key)
        {
            var line = FindLine(key);
            var res = !IsInactiveLine(line);

            if (res)
                CheckAndLogActiveLine(ref line, ref key);

            return res;
        }

        // int

        public static bool Get(string key, out int result)
        {
            var line = FindLine(key);
            result = int.MinValue;

            if (IsInactiveLine(line))
                return false;

            var arr = line.Split(new char[] { '\\' });

            if (arr.Length < 2)
                return false;

            result = arr[1].Parse(int.MinValue);

            if (result == int.MinValue)
                return false;
            else
            {
                CheckAndLogActiveLine(ref line, ref key);
                return true;
            }
        }

        public static int Get(string key, int defaultValue = 0)
        {
            var line = FindLine(key);

            if (IsInactiveLine(line))
                return defaultValue;

            var arr = line.Split(new char[] { '\\' });

            if (arr.Length < 2)
                return defaultValue;

            CheckAndLogActiveLine(ref line, ref key);
            return arr[1].Parse(defaultValue);
        }

        // string

        public static bool Get(string key, out string result)
        {
            var line = FindLine(key);
            result = null;

            if (IsInactiveLine(line))
                return false;

            var arr = line.Split(new char[] { '\\' });

            if (arr.Length < 2)
                return false;

            result = arr[1];
            CheckAndLogActiveLine(ref line, ref key);
            return true;
        }

        public static string Get(string key, string defaultValue = null)
        {
            var line = FindLine(key);

            if (IsInactiveLine(line))
                return defaultValue;

            var arr = line.Split(new char[] { '\\' });

            if (arr.Length < 2)
                return defaultValue;

            CheckAndLogActiveLine(ref line, ref key);
            return arr[1];
        }

        // float

        public static bool Get(string key, out float result)
        {
            var line = FindLine(key);
            result = float.MinValue;

            if (IsInactiveLine(line))
                return false;

            var arr = line.Split(new char[] { '\\' });

            if (arr.Length < 2)
                return false;

            result = arr[1].Parse(float.MinValue);

            if (result == float.MinValue)
                return false;
            else
            {
                CheckAndLogActiveLine(ref line, ref key);
                return true;
            }
        }

        public static float Get(string key, float defaultValue = 0)
        {
            var line = FindLine(key);

            if (IsInactiveLine(line))
                return defaultValue;

            var arr = line.Split(new char[] { '\\' });

            if (arr.Length < 2)
                return defaultValue;

            CheckAndLogActiveLine(ref line, ref key);
            return arr[1].Parse(defaultValue);
        }
    }
}