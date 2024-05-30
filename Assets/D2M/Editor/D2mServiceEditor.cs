    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

[CustomEditor(typeof(D2mServices))]
public class D2mServiceEditor : Editor
{
    private D2mServices _coreMobile = null;
    private void OnEnable()
    {
        _coreMobile = (D2mServices)target;

        string defines = D2mConstants.GetDefines();
        var array = defines.Split(';');

        _coreMobile.DefineAdMob = array.Contains(D2mConstants.DEFINE_ADMOB);
        _coreMobile.DefineAdNative = array.Contains(D2mConstants.DEFINE_AD_NATIVE);
        _coreMobile.DefineIAP = array.Contains(D2mConstants.DEFINE_IAP);
        _coreMobile.DefineFacebookSDK = array.Contains(D2mConstants.DEFINE_FACEBOOK_SDK);
        _coreMobile.DefineGameServices = array.Contains(D2mConstants.DEFINE_GAMESERVICES);
        _coreMobile.DefineFirebaseAnalytic = array.Contains(D2mConstants.DEFINE_FIREBASE_ANALYTIC);
        _coreMobile.DefineFirebaseCrashlytic = array.Contains(D2mConstants.DEFINE_FIREBASE_CRASHLYTIC);
        _coreMobile.DefineFirebaseMessaging = array.Contains(D2mConstants.DEFINE_FIREBASE_MESSAGING);
        _coreMobile.DefineFirebaseAuth = array.Contains(D2mConstants.DEFINE_FIREBASE_AUTH);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Set Defines"))
        {
            string defines = D2mConstants.GetDefines();
            string definesBefore = defines;
            var array = defines.Split(';');

            defines = CheckDefine(defines, array, _coreMobile.DefineAdMob, D2mConstants.DEFINE_ADMOB);
            defines = CheckDefine(defines, array, _coreMobile.DefineAdNative, D2mConstants.DEFINE_AD_NATIVE);
            defines = CheckDefine(defines, array, _coreMobile.DefineIAP, D2mConstants.DEFINE_IAP);
            defines = CheckDefine(defines, array, _coreMobile.DefineFacebookSDK, D2mConstants.DEFINE_FACEBOOK_SDK);
            defines = CheckDefine(defines, array, _coreMobile.DefineGameServices, D2mConstants.DEFINE_GAMESERVICES);
            defines = CheckDefine(defines, array, _coreMobile.DefineFirebaseAnalytic, D2mConstants.DEFINE_FIREBASE_ANALYTIC);
            defines = CheckDefine(defines, array, _coreMobile.DefineFirebaseCrashlytic, D2mConstants.DEFINE_FIREBASE_CRASHLYTIC);
            defines = CheckDefine(defines, array, _coreMobile.DefineFirebaseMessaging, D2mConstants.DEFINE_FIREBASE_MESSAGING);
            defines = CheckDefine(defines, array, _coreMobile.DefineFirebaseAuth, D2mConstants.DEFINE_FIREBASE_AUTH);

            if (!definesBefore.Equals(defines))
            {
                D2mConstants.SetDefines(defines);
            }
        }
    }

    private string CheckDefine(string defines, string[] arrayDefine, bool enableDefine, string defineString)
    {
        if (enableDefine && !arrayDefine.Contains(defineString))
        {
            defines += ";" + defineString;
        }
        else if (!enableDefine && arrayDefine.Contains(defineString))
        {
            defines = defines.Replace(";" + defineString, "");
            defines = defines.Replace(defineString + ";", "");
        }

        return defines;
    }
}