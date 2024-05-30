using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

public class SceneSystem : MonoBehaviour
{
    public Text textLevel;
    public Text unlockMeteorTxt = null;
    public Text unlockBlackHoleTxt = null;

    public static SceneSystem Instance = null;

    private void Start()
    {
#if UNITY_EDITOR
        if (!GameManager.Instance && !SteveRogers.Utilities.ActiveScene.name.Equals("CustomLoading"))
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif

        Instance = this;

        if (textLevel != null)
            textLevel.text = "Lv." + DataGameSave.dataServer.level.ToString();
    }

    private void Update()
    {
        if (IsUnlocked_Meteor && unlockMeteorTxt)
        {
            if (unlockMeteorTxt.gameObject.activeSelf)
                unlockMeteorTxt.gameObject.SetActive(false);
        }
        
        if (IsUnlocked_BlackHole && unlockMeteorTxt)
        {
            if (unlockBlackHoleTxt.gameObject.activeSelf)
                unlockBlackHoleTxt.gameObject.SetActive(false);
        }
    }

    public int levelUnlockBlackHole = 3;
    public int levelUnlockMeteor = 2;

    public bool IsUnlocked_BlackHole
    {
        get
        {
            return DataGameSave.dataServer != null && DataGameSave.dataServer.level > levelUnlockBlackHole;
        }
    }

    public bool IsUnlocked_Meteor
    {
        get
        {
            return DataGameSave.dataServer != null && DataGameSave.dataServer.level > levelUnlockMeteor;
        }
    }

    public void OnPressed_Blackhole()
    {
        if (IsUnlocked_BlackHole)
            Scenes.ChangeScene(SceneName.BlackHole);
    }

    public void ChangeSceneByName(string name)
    {
        if (name == "blackhole") {
            Scenes.ChangeScene(SceneName.BlackHole);
        }
        if (name == "meteorbelt") {
            Scenes.ChangeScene(SceneName.MeteorBelt);
        }
        if (name == "neightbor") {
            Scenes.ChangeScene(SceneName.Neightbor);
        }
        if (name == "gameplay") {
            Scenes.ChangeScene(SceneName.Gameplay);
        }
        //string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(buildIndex));
        //int sceneIndex = buildIndex;
        //int.TryParse(sceneName)
        //Scenes.ChangeScene(SceneName.BlackHole);
    }

    public void SwitchToMeteorScene()
    {
        if (!IsUnlocked_Meteor)
            return;

        DataGameSave.dataEnemy = new DataGameServer();
        DataGameSave.dataEnemy = new DataGameServer();

        Scenes.LastScene = SceneName.Gameplay;
        Scenes.ChangeScene(SceneName.MeteorBelt);
    }

    public void ShowMeteorPopup() {
        PopupCustom.Show("Prefabs/Pop-ups/Custom/Popup Meteor");
    }

    public void SwitchToBlackHole()
    {
        Scenes.LastScene = SceneName.Gameplay;
        Scenes.ChangeScene(SceneName.BlackHole);
    }
}
