using System.Linq;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using SteveRogers;

public partial class AssetBundleEditor
{
    public readonly static string dir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + Utilities.ASSET_BUNDLE_FOLDER_NAME;

    public readonly static string dirWithSeparator = dir + '/';

    #region BUILD

    [MenuItem("Assets/Build AssetBundle - All", priority = 200)]
    private static void Build()
    {
        if (!EditorUtility.DisplayDialog("Confirm", "Build all?", "ok", "no"))
            return;

        Utilities.CheckAndCreateDirectory(dir);
        Debug.Log("Start build all bundle for: " + EditorUserBuildSettings.activeBuildTarget);
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        Utilities.WarningDone("Build All Asset Bundles");
    }
        
    [MenuItem("Assets/Build AssetBundle - Selections", priority = 200)]
    public static void Build_FromSelections()
    {
        BuildAssetBundlesFromSelections(EditorUserBuildSettings.activeBuildTarget);
    }

    #endregion

    #region UTILITIES

    //[MenuItem("Helper/Asset Bundle/Bundle Folder", priority = 0)]
    //private static void OpenAssetBundlesDirectory()
    //{
    //    HelpersEditor.OpenDirectory(dir);
    //}

    //[MenuItem("Helper/Asset Bundle/Delete Bundle Folder", priority = 1)]
    //private static void DeleteAssetBundlesDirectory()
    //{
    //    if (Directory.Exists(dir))
    //        Directory.Delete(dir, true);

    //    Debug.Log("Deleted!");
    //}
        
    [MenuItem("Assets/Set AssetBundles From File Names From Selected", priority = 200)]
    private static void SetAssetBundlesFromFileNames()
    {
        foreach (var i in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(i);
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            assetImporter.assetBundleName = i.name;
        }

        Debug.Log("Done!");
    }

    [MenuItem("Assets/Move Builded AssetBundle To Here", priority = 200)]
    private static void MoveBuildedAssetBundleToHere()
    {
        var i = Selection.activeObject;

        if (i == null)
            return;

        foreach (var file in Directory.EnumerateFiles(dir))
            if (Path.GetFileName(file).Equals(i.name))
            {
                Utilities.CutFile(file, Path.GetDirectoryName(AssetDatabase.GetAssetPath(i)), true, false);
                Utilities.WarningDone("Moved AB");
                return;
            }

        Debug.LogError("Not found bundle name: " + i.name);
    }

    //public static string GetExportDirPath()
    //{
    //    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
    //        return dir_iOS;
    //    else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX)
    //        return dir_Mac;
    //    else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
    //        return dir_Android;
    //    else
    //        return dir_Windows;
    //}

    #endregion

    #region PRIVATE CORE

    //private static bool ConfirmToBuild(BuildTarget build)
    //{
    //    if (EditorUserBuildSettings.activeBuildTarget != build)
    //        return EditorUtility.DisplayDialog("Confirm", "Do you want switch to " + build + "?", "Yesss", "No!!!");
    //    else
    //        return true;
    //}

    private static void BuildAssetBundlesFromSelections(BuildTarget target)
    {
        // Get all selected *assets*
        var assets = Selection.objects.Where(o => !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(o))).ToArray();


        if (assets.IsNullOrEmpty())
        {
            Debug.LogError($"You must select some assets to build {EditorUserBuildSettings.activeBuildTarget}!");
            return;
        }
        
        Debug.Log("Start build SELECTED bundles for: " + target);

        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        HashSet<string> processedBundles = new HashSet<string>();

        // Get asset bundle names from selection
        foreach (var o in assets)
        {
            var assetPath = AssetDatabase.GetAssetPath(o);
            var importer = AssetImporter.GetAtPath(assetPath);

            if (importer == null)
            {
                continue;
            }

            // Get asset bundle name & variant
            var assetBundleName = importer.assetBundleName;
            var assetBundleVariant = importer.assetBundleVariant;
            var assetBundleFullName = string.IsNullOrEmpty(assetBundleVariant) ? assetBundleName : assetBundleName + "." + assetBundleVariant;

            // Only process assetBundleFullName once. No need to add it again.
            if (processedBundles.Contains(assetBundleFullName))
            {
                continue;
            }

            processedBundles.Add(assetBundleFullName);

            AssetBundleBuild build = new AssetBundleBuild();

            build.assetBundleName = assetBundleName;
            build.assetBundleVariant = assetBundleVariant;
            build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleFullName);

            assetBundleBuilds.Add(build);
        }

        var builds = assetBundleBuilds.ToArray();

        Utilities.CheckAndCreateDirectory(dir);

        var options = BuildAssetBundleOptions.None;

        bool shouldCheckODR = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
#if UNITY_TVOS
             shouldCheckODR |= EditorUserBuildSettings.activeBuildTarget == BuildTarget.tvOS;
#endif
        if (shouldCheckODR)
        {
#if ENABLE_IOS_ON_DEMAND_RESOURCES
                 if (PlayerSettings.iOS.useOnDemandResources)
                     options |= BuildAssetBundleOptions.UncompressedAssetBundle;
#endif
#if ENABLE_IOS_APP_SLICING
                 options |= BuildAssetBundleOptions.UncompressedAssetBundle;
#endif
        }
                
        AssetBundleManifest res = null;

        if (builds == null || builds.Length == 0)
        {
            //@TODO: use append hash... (Make sure pipeline works correctly with it.)
            res = BuildPipeline.BuildAssetBundles(dir, options, target);
        }
        else
        {
            res = BuildPipeline.BuildAssetBundles(dir, builds, options, target);
        }

        if (res)
        {
            Utilities.SafeInvokeMethod<AssetBundleEditor>("OnAfterBuildBundle");
            Utilities.WarningDone("Build Bundle (From Selections)");
        }
    }

    #endregion
}
