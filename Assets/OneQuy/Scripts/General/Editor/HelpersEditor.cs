using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;
using UnityEngine;
using SteveRogers;

/// <summary>
/// # - Shift
/// & - Alt
/// % - Ctrl
/// </summary>
public partial class HelpersEditor
{
    #region Varibles and Core

    public readonly static string dir = Directory.GetCurrentDirectory();

    #endregion

    #region Directory (Whole menu)

    [MenuItem("Directory/Open Persistent Folder")]
    public static void OpenPersistentDataPath()
    {
        Utilities.OpenDirectory(Application.persistentDataPath);
    }

    [MenuItem("Directory/Open RuntimeData Folder")]
    public static void OpenRuntimeDataFolder()
    {
        Utilities.OpenDirectory(Utilities.GetRuntimeDataFolderPath(false));
    }

    [MenuItem("Directory/Open Files Folder")]
    public static void OpenFilesFolder()
    {
        Utilities.OpenDirectory(Directory.GetCurrentDirectory() + "/Files");
    }

    [MenuItem("Directory/Open Resources Folder")]
    public static void OpenResFolder()
    {
        Utilities.OpenDirectory(Utilities.GetResourcesPath(false));
    }

    //[MenuItem("Directory/Remove Empty Folders From Clipboard", priority = 100)]
    //public static void RemoveEmptyFoldersFromClipboard()
    //{
    //    if (Utilities.Clipboard.IsNullOrEmpty())
    //    {
    //        Debug.LogError("clipboard null");
    //        return;
    //    }

    //    var lines = Utilities.Clipboard.Split(Utilities.SPLIT_CHAR_NEW_LINE);

    //    for (int i = 0; i < lines.Length; i++)
    //    {
    //        RemoveEmptyFolder(lines[i]);
    //    }

    //    AssetDatabase.Refresh();
    //    Utilities.WarningDone();
    //}

    //private static void RemoveEmptyFolder(string assetMetaFilepath)
    //{
    //    assetMetaFilepath = assetMetaFilepath?.Trim();

    //    if (assetMetaFilepath.IsNullOrEmpty())
    //        return;

    //    var p = Path.Combine(Directory.GetCurrentDirectory(), assetMetaFilepath);
    //    var folderFullpath = Path.Combine(Path.GetDirectoryName(p), Path.GetFileNameWithoutExtension(p));
    //    Directory.Delete(folderFullpath, true);
    //}

    #endregion

    #region PlayerPref - (-400)

    [MenuItem("Helper/PlayerPref/Delete All", priority = -400)]
    public static void DeletePref()
    {
        //if (EditorUtility.DisplayDialog("Confirm", "Are you sure?", "Sure", "No"))
        {
            PlayerPrefs.DeleteAll();
            Utilities.WarningDone();
        }
    }

    [MenuItem("Helper/PlayerPref/Backup", priority = -400)]
    public static void BackupPref()
    {
        PlayerPrefBackup.Backup(false);
    }

    [MenuItem("Helper/PlayerPref/Backup Force/Confirm", priority = -400)]
    public static void BackupPrefForce()
    {
        PlayerPrefBackup.Backup(true);
    }

    [MenuItem("Helper/PlayerPref/Restore", priority = -400)]
    public static void RestorePref()
    {
        PlayerPrefBackup.Restore();
    }

    [MenuItem("Helper/PlayerPref/Restore", true)]
    public static bool RestorePref_Val()
    {
        return File.Exists(PlayerPrefBackup.PATH);

    }

    #endregion

    #region Cheat - 200
    //#if SteveCheat
    [MenuItem("Helper/Cheat/Build Cheat File PC #q", priority = 200)]
    public static void BuildCheatFilePC()
    {
        CheatManager[] man = Resources.FindObjectsOfTypeAll<CheatManager>();

        if (man.IsValid())
        {
            string s = Newtonsoft.Json.JsonConvert.SerializeObject(man[0].buildData);
            Utilities.WriteAllText(CheatManager.Filepath, s, true);
            Utilities.WarningDone("Build Cheat");
        }
        else
            Debug.LogError("Not found Cheat go!");
    }

    [MenuItem("Helper/Cheat/Delete Cheat Folder PC #&q", priority = 200)]
    public static void DeleteCheatFolderPC()
    {
        if (Directory.Exists(CheatManager.GetFolderPath(true)))
            Directory.Delete(CheatManager.GetFolderPath(true), true);

        Utilities.WarningDone("Delete Cheat");
    }
    //#endif
    #endregion

    #region Utilities - 3000

    [MenuItem("Helper/Utilities/FPS Shower", priority = 3000)]
    public static void ShowFPS()
    {
        Utilities.CreateFPSShower();
    }

    [MenuItem("Helper/Utilities/Stats Shower", priority = 3000)]
    public static void ShowStats()
    {
        Utilities.CreateStatsShower();
    }

    [MenuItem("Helper/Utilities/Clear Console #`", priority = 3000)]
    public static void ClearConsole()
    {
        Utilities.ClearConsole();
    }

    [MenuItem("Helper/Utilities/Show Current Define Symbols", priority = 3000)]
    public static void DefineSymbols_Show()
    {
        Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup));
    }

    [MenuItem("Helper/Utilities/Copy Gameobject Tag", priority = 3000)]
    public static void CopyGameobjectTag()
    {
        Utilities.Clipboard = Selection.activeGameObject.tag;
        Utilities.WarningDone("copy tag: " + Utilities.Clipboard);
    }

    [MenuItem("Helper/Utilities/Save Selection #.", priority = 3000)]
    public static void SaveSelection()
    {
        if (!Selection.activeGameObject)
            return;

        var s = Utilities.RootNameGo(Selection.activeGameObject, false);
        EditorPrefs.SetString("steve-save-seletion", s);
        Utilities.WarningDone("Save!");
    }

    [MenuItem("Helper/Utilities/Restore Selection #/", priority = 3000)]
    public static void RestoreSelection()
    {
        var s = EditorPrefs.GetString("steve-save-seletion", null);

        if (s.IsNullOrEmpty())
            return;

        Scene scene = SceneManager.GetActiveScene();
        var roots = scene.GetRootGameObjects();
        var found = false;

        foreach (var i in roots)
        {
            Utilities.TraverseDFS(i.transform, (t) =>
            {
                var path = Utilities.RootNameGo(t.gameObject, false);

                if (s.Equals(path))
                {
                    Selection.activeGameObject = t.gameObject;
                    found = true;
                    return false;
                }
                else
                    return true;
            });

            if (found)
                break;
        }

        if (!found)
            Debug.LogError("Not found: " + s);
    }

    #endregion

    #region Others

    [MenuItem("Helper/Reset", priority = 100)]
    public static void Reset()
    {
        Directory.Delete(Application.persistentDataPath, true);
        PlayerPrefs.DeleteAll();
        Utilities.SafeInvokeMethod<HelpersEditor>("OnReset");
        Utilities.WarningDone("Reset");
    }

    [MenuItem("Helper/Open File From Asset Path", priority = 100)]
    public static void OpenFileFromClipboardAssetPath()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), Utilities.Clipboard);

        if (File.Exists(path))
            Utilities.OpenFileWithDefaultApp(path);
        else
            Debug.LogError("not found file: " + path);
    }

    [MenuItem("Helper/Make Prefabs", priority = 100)]
    private static void CreatePrefabs()
    {
        var objs = Selection.gameObjects;

        string pathBase = EditorUtility.SaveFolderPanel("Choose save folder", "Assets", "");

        if (!string.IsNullOrEmpty(pathBase))
        {

            pathBase = pathBase.Remove(0, pathBase.IndexOf("Assets")) + Path.DirectorySeparatorChar;

            foreach (var go in objs)
            {
                string localPath = pathBase + go.name + ".prefab";

                if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
                {
                    if (EditorUtility.DisplayDialog("Are you sure?",
                            "The prefab already exists. Do you want to overwrite it?",
                            "Yes",
                            "No"))
                        PrefabUtility.SaveAsPrefabAssetAndConnect(go, localPath, InteractionMode.UserAction);
                }
                else
                    PrefabUtility.SaveAsPrefabAssetAndConnect(go, localPath, InteractionMode.UserAction);
            }
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Helper/Utilities/Select TempVar #t", priority = 3000)]
    public static void SelectTempVar()
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/TempVar.asset");
    }

    #endregion

}
