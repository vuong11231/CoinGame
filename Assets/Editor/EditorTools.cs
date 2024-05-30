#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;


public class EditorTools : Editor
{
    [MenuItem("Tools/Reset PlayerPrefs #&r", false)]
    public static void ResetPlayerPref()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("*** PlayerPrefs was reset! ***");
    }
}

#endif