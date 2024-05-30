using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class D2mPlugin : Singleton<D2mPlugin>
{
#if UNITY_ANDROID
    static AndroidJavaObject Systems;
#elif UNITY_IOS
    [DllImport("__Internal")]
    private static extern bool IsSchemeAvailable(string urlScheme);
#endif

    private void Start()
    {
#if UNITY_ANDROID
        Systems = new AndroidJavaObject("com.d2m.unityplugin.SystemPlugin");
#endif
    }

    public bool IsAppInstalled(string packageName)
    {
#if UNITY_EDITOR
        return true;
#elif UNITY_ANDROID
        return Systems.Call<bool>("IsAppInstalled", packageName);
#elif UNITY_IOS
        return IsSchemeAvailable(packageName);
#endif
    }
}
