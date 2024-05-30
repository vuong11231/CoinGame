using System.IO;

using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;

public static class D2mPListProcessor
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
#if UNITY_IPHONE
        //string plistPath = Path.Combine(path, "Info.plist");
        //PlistDocument plist = new PlistDocument();

        //plist.ReadFromFile(plistPath);
        //plist.root.get("LSApplicationQueriesSchemes", appId);
        //File.WriteAllText(plistPath, plist.WriteToString());
#endif
    }
}
#endif