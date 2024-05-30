using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MyDebug : MonoBehaviour
{
    public static MyDebug Instance;

    public TextMeshProUGUI txt;

    private void Awake()
    {
        Instance = this;
    }

    public static void Log(string s) {
        Instance.txt.text += "\n" + s;
    }
}
