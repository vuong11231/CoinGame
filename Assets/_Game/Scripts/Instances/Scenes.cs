using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public static class Scenes {

    public static SceneName Current = SceneName.CustomLoading;
    public static SceneName LastScene = SceneName.CustomLoading;
    public static int CurrentSceneId = 1;

    public static void ChangeScene(SceneName name) {
        if (Current != name) {
            Popups.Reset();
            Scenes.LastScene = Current;
            Current = name;
            SceneManager.LoadScene(name.ToString());

            while (true) { 
                int randomId = Random.Range(0, 10000);
                if (CurrentSceneId != randomId) {
                    CurrentSceneId = randomId;
                    break;
                }
            }
        }
    }

    public static void ReturnToLastScene(int currentSceneId) {
        if (SceneManager.GetActiveScene().name == Scenes.LastScene.ToString()) {
            return;
        }

        if (currentSceneId != CurrentSceneId) {
            return;
        }

        ChangeScene(LastScene);
    }

    public static void ChangeSceneForced(SceneName name) {
        //GameStatics.IsPause = true;
        Popups.Reset();

        Current = name;
        SceneManager.LoadScene(name.ToString());
    }

    public static bool IsBattleScene() {
        return Current == SceneName.Battle || Current == SceneName.BattlePassGameplay;
    }
}
