#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class OpenSceneEditor : Editor
{

    [MenuItem("Project/Play Custom Loading #a", priority = 5000)]
    public static void OpenLoading()
    {
        OpenScene("CustomLoading");
        EditorApplication.EnterPlaymode();
    }
    
    [MenuItem("Project/Open Custom Loading &a", priority = 5000)]
    public static void OpenLoadingg()
    {
        OpenScene("CustomLoading");
    }

    [MenuItem("Project/Open Scene - Game Play &g", false, 5000)]
    public static void OpenMain()
    {
        OpenScene("GamePlay");
    }

    [MenuItem("Project/Open Scene - Battle %&e", false, 5000)]
    public static void OpenGamePlaySCene()
    {
        OpenScene("Battle");
    }

    [MenuItem("Project/Open Scene - Neightbor", false, 5000)]
    public static void OpenNeightbor()
    {
        OpenScene("Neightbor");
    }
    
    [MenuItem("Project/Open Scene - BlackHole", false, 5000)]
    public static void OpenBlackHole()
    {
        OpenScene("BlackHole");
    }
    
    [MenuItem("Project/Open Scene - Explore", false, 5000)]
    public static void OpenExplore()
    {
        OpenScene("Explore");
    }
    
    [MenuItem("Project/Open Scene - MeteorBelt", false, 5000)]
    public static void OpenMeteorBelt()
    {
        OpenScene("MeteorBelt");
    }

    [MenuItem("Project/Open Scene - BattlePass", false, 5000)]
    public static void OpenBattlePass()
    {
        OpenScene("BattlePass");
    }

    [MenuItem("Project/Open Scene - BattlePassGameplay", false, 5000)]
    public static void OpenBattlePassGameplay()
    {
        OpenScene("BattlePassGameplay");
    }

    [MenuItem("Server/Run Data On Server", false, 5000)]
    public static void RunOnServer()
    {
        ServerConstants.BASE_URL = "https://universe-master-test2.herokuapp.com/";
    }

    [MenuItem("Server/Run Data On Client", false, 5000)]
    public static void RunOnClient()
    {
        ServerConstants.BASE_URL = "http://localhost:8080/";
    }

    private static void OpenScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/_Game/Scenes/" + sceneName + ".unity");
        }
    }

    private static void OpenNewScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/New/" + sceneName + ".unity");
        }
    }
}

#endif