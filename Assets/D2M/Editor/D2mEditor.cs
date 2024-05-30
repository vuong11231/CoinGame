﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

class D2mEditor : EditorWindow
{
    // Scenes
    List<string> _scene;

    // Screenshot
    int startNumber;
    string screenshotDesPath = "/Data/Projects/D2M/";
    string encrypSource = "CSV";
    string decrypSource = "_TXT";
    string fileName = "";

    GUIStyle _guiStyle;

    [MenuItem("D2M/Scene Manager")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(D2mEditor));
    }

    #region Function
    static public void  StartGame()
    {
        if (!UnityEngine.Application.isPlaying)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_Game/Scenes/Loading.unity");
                EditorApplication.ExecuteMenuItem("Edit/Play");
            }
        }
        else EditorApplication.ExecuteMenuItem("Edit/Play");
    }

    static public void PLay()
    {
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }

    static public void PlayScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/_Game/Scenes/Gameplay.unity");
        }
    }

    static public void LoadingScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/_Game/Scenes/Loading.unity");
        }
    }

    static public void LoadScene(string scenename)
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_Game/Scenes/" + scenename + ".unity");
            }
        }
        else
        {
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
    }

    private static string[] ReadNames()
    {
        List<string> temp = new List<string>();
        //foreach (EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
        foreach (EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
        {
            if (S.enabled)
            {
                string name = S.path.Substring(S.path.LastIndexOf('/') + 1);
                name = name.Substring(0, name.Length - 6);
                temp.Add(name);
            }
        }
        return temp.ToArray();
    }

    private static string[] ReadScene()
    {
        List<string> temp = new List<string>();
        return temp.ToArray();
    }

    static public void ClearAllData()
    {
        //DataManager.ClearData();
        PlayerPrefs.DeleteAll();
        DataGameSave.ClearData();
        File.Delete(FacebookManager.LocalPathAvatar());
    }

    public void CaptureScreenshot(string _path)
    {
        int number = startNumber;
        string name = "" + number;

        while (System.IO.File.Exists(_path + "/" + name + ".png"))
        //while (System.IO.File.Exists("/Data/Projects/D2M/" + name + ".png"))
        {
            number++;
            name = "" + number;
        }
        startNumber = number + 1;
        //ScreenCapture.CaptureScreenshot("/Data/Projects/D2M/" + name + ".png");
        ScreenCapture.CaptureScreenshot(_path + "/" + name + ".png");
    }

    public void EncryptFile(string fileName)
    {
        string tempString = Resources.Load<TextAsset>(encrypSource + "/" + fileName).text;
        string path = Application.dataPath + "/_Game/Resources/" + decrypSource + "/" + fileName + ".txt";
        string content = EncryptionHelper.Encrypt(tempString, true);
        System.IO.File.AppendAllText(path, content);
    }

    public void DecryptFile(string fileName)
    {
        string tempString = Resources.Load<TextAsset>(decrypSource + "/" + fileName).text;
        string path = Application.dataPath + "/_Game/Resources/" + encrypSource + "/" + fileName + ".txt";
        string content = EncryptionHelper.Decrypt(tempString, true);
        System.IO.File.AppendAllText(path, content);
    }

    string txtAUto = "start Auto";
    Vector2 scrollPos;
    string gold;
    #endregion

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        //
        // Start Game
        if (GUILayout.Button("Start Game"))
        {
            StartGame();
        }

        //
        // Settings
        if (GUILayout.Button("Open Players Setting"))
        {
            EditorApplication.ExecuteMenuItem("File/Build Settings...");
        }

        //
        // Data
        GUILayout.Space(10);
        GUILayout.Label("------ DATA ------", style);
        GUILayout.Space(10);
        if (GUILayout.Button("Clear Data"))
        {
            ClearAllData();
        }

        //
        //  Scenes
        GUILayout.Space(10);
        GUILayout.Label("------ SCENES ------", style);
        GUILayout.Space(10);
        EditorGUILayout.BeginVertical();
        var allscene = ReadNames();
        for (int i = 0; i < allscene.Length; i++)
        {
            if (GUILayout.Button(allscene[i]))
            {

                LoadScene(allscene[i]);
            }
        }
        EditorGUILayout.EndVertical();

        //
        // Tools
        GUILayout.Space(10);
        GUILayout.Label("------ TOOLS ------", style);
        //EditorGUILayout.BeginHorizontal();
        //gold = GUILayout.TextField(gold, GUILayout.Width(70));
        //if (GUILayout.Button("add gold"))
        //{
        //    GDSManager.Instance.AddGold(Convert.ToInt32(gold));
        //}
        //EditorGUILayout.EndHorizontal();

        //
        // Screenshot
        screenshotDesPath = GUILayout.TextField(screenshotDesPath);
        if (GUILayout.Button("Capture Screenshot"))
        {
            CaptureScreenshot(screenshotDesPath);
        }

        GUILayout.Label("Encryp file txt");
        GUILayout.Label("   + File name: ");
        fileName = GUILayout.TextField(fileName);
        if (GUILayout.Button("Encrypt File"))
        {
            EncryptFile(fileName);
        }
        if (GUILayout.Button("Decrypt File"))
        {
            DecryptFile(fileName);
        }

        EditorGUILayout.EndScrollView();
    }
}
#endif
