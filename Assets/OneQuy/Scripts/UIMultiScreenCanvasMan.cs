using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteveRogers;
using UnityEngine.SceneManagement;
using System;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class MoveUINumber
{
    public GameObject go;
    public float distance = 1800;
    public float distanceRight = 0;
    public float distanceRightEvent = 0;
    public bool isrect = true;
    public float startX;
}

public class UIMultiScreenCanvasMan : MonoBehaviour {
    public enum Mode { Upgrade, Gameplay, Explore, Event }
    public GameObject buttonUpgrade, buttonGameplay, buttonExplore, buttonEvent;
    public Vector3 sizeOnlick = Vector3.one;

    public GameObject panelFather = null;
    public PopupUpgradePlanet popupUpgrade = null;
    public EventScroller eventScroller = null;


    public GameObject meteorWarning;
    public GameObject meteorWarningInfo;
    public TextMeshProUGUI meteorWarningText;


    public float hidePosY = -110f;
    public float showPosY = 90f;
    public float defaultOrtho = 80;

    public MoveUINumber[] moveUINumbers = null;

    public static Mode modeExplore = Mode.Gameplay;
    public static UIMultiScreenCanvasMan Instance = null;

    public SceneSystem sceneSystem;
    public GameObject questStatusGroup, restorPlanetGroup;
    public GameObject hotKeyGroup;
    public GameObject hotKey_Attack, hotKey_MeoterBelt, hotKey_BlackHole;
    public GameObject buttonMail, buttonAttackInfo, buttonGiftLetter, buttonToyEvent, buttonAutoRestore, buttonIdleCollect, buttonMeteorFallEvent, buttonX2Material;
    public GameObject reddotShop;

    public TextMeshProUGUI txtBossCount;

    public Text eventButton_MainTxt = null;
    public Text eventButton_UnlockTxt = null;

    private static bool isShowing = true;

    private void Start() {
        Instance = this;

        foreach (var i in moveUINumbers) {
            if (i.go.GetComponent<RectTransform>()) {
                i.isrect = true;
                i.startX = i.go.GetComponent<RectTransform>().anchoredPosition.x;
            } else {
                i.startX = i.go.transform.position.x;
                i.isrect = false;
            }
        }

        SceneManager.sceneLoaded += (scene, mode) => {
            if (scene.name.Equals("GamePlay")) {
                modeExplore = Mode.Gameplay;
            }
        };

        CheckUnlockEventBtn();

        if (DataGameSave.dataServer.rankPoint > 0) {
            txtBossCount.text = DataGameSave.dataServer.rankPoint.ToString();
        }

        ShowLastMode();
    }

    private void Update() {
        CheckShowHotKey();
        CheckShowButton();
    }


    public void CheckUnlockEventBtn() {
        eventButton_MainTxt.gameObject.SetActive(DataGameSave.dataServer.level >= 5);
        eventButton_UnlockTxt.gameObject.SetActive(DataGameSave.dataServer.level < 5);
        //eventButton_MainTxt.gameObject.SetActive(true);
        //eventButton_UnlockTxt.gameObject.SetActive(false);
    }

    public void CheckShowButton() {
        if (modeExplore == Mode.Gameplay) {
            buttonMail.CheckAndSetActive(true);
            buttonAttackInfo.CheckAndSetActive(true);
            buttonToyEvent.CheckAndSetActive(GameManager.SHOW_EVENT_BUTTON);
            buttonAutoRestore.CheckAndSetActive(true);
            buttonX2Material.CheckAndSetActive(GameManager.Now.Ticks < DataGameSave.x2MaterialEndTime);
            buttonIdleCollect.CheckAndSetActive(true);
            questStatusGroup.CheckAndSetActive(true);
            buttonGiftLetter.CheckAndSetActive(DataGameSave.dataServer.level >= 5);
            buttonMeteorFallEvent.CheckAndSetActive(DataGameSave.dataServer.level >= 3 && GameManager.SHOW_METEOR_FALL_EVENT_BUTTON);
        } else {
            buttonMail.CheckAndSetActive(false);
            buttonAttackInfo.CheckAndSetActive(false);
            buttonToyEvent.CheckAndSetActive(false);
            buttonAutoRestore.CheckAndSetActive(false);
            buttonX2Material.CheckAndSetActive(false);
            buttonIdleCollect.CheckAndSetActive(false);
            questStatusGroup.CheckAndSetActive(false);
            buttonGiftLetter.CheckAndSetActive(false);
            buttonMeteorFallEvent.CheckAndSetActive(false);
            SpaceManager.Instance.restoreTimePanelGo.CheckAndSetActive(false);

            if (DataGameSave.dataServer.rankPoint > 0) {
                txtBossCount.text = DataGameSave.dataServer.rankPoint.ToString();
            }
        }

        string freeShop = (GameManager.MAX_WATCH_ADS_PER_DAY_LIMIT + 1).ToString();
        if (DataGameSave.GetMetaData(MetaDataKey.AUTO_SHOOT_COUNT) == freeShop ||
            DataGameSave.GetMetaData(MetaDataKey.SHOP_GET_DIAMOND_COUNT) == freeShop ||
            DataGameSave.GetMetaData(MetaDataKey.SHOP_TOTAL_ATTACK_COUNT) == freeShop) {
            reddotShop.SetActive(true);
        } else {
            reddotShop.SetActive(false);
        }
    }

    public void Hide() {
        if (!isShowing)
            return;

        isShowing = false;

        LeanTween.moveAnchorPositionY(panelFather, hidePosY, 0.3f)
            .setEase(LeanTweenType.easeSpring);
    }

    public void Show() {
        if (isShowing)
            return;

        isShowing = true;

        LeanTween.moveAnchorPositionY(panelFather, showPosY, 0.3f)
            .setEase(LeanTweenType.easeSpring);
    }

    public void CheckSizeButton() {
        buttonUpgrade.transform.GetChild(0).transform.localScale = Vector3.one;
        buttonEvent.transform.GetChild(0).transform.localScale = Vector3.one;
        buttonExplore.transform.GetChild(0).transform.localScale = Vector3.one;
        buttonGameplay.transform.GetChild(0).transform.localScale = Vector3.one;

        switch (modeExplore) {
            case Mode.Upgrade:
                buttonUpgrade.transform.GetChild(0).transform.localScale = sizeOnlick;
                break;
            case Mode.Gameplay:
                buttonGameplay.transform.GetChild(0).transform.localScale = sizeOnlick;
                break;
            case Mode.Explore:
                buttonExplore.transform.GetChild(0).transform.localScale = sizeOnlick;
                break;
            case Mode.Event:
                buttonEvent.transform.GetChild(0).transform.localScale = sizeOnlick;
                break;
            default:
                break;
        }
    }

    public void CheckShowHotKey() {
        if (DataGameSave.dataLocal == null || DataGameSave.dataServer == null)
            return;

        hotKeyGroup.SetActive(modeExplore == Mode.Gameplay);
        hotKey_MeoterBelt.SetActive(sceneSystem.IsUnlocked_Meteor);
        hotKey_Attack.SetActive(DataGameSave.dataServer?.ListPlanet?.Count > 0);

        bool checkQuantityStone = DataGameSave.dataLocal.M_AirStone > 0
                                   || DataGameSave.dataLocal.M_AntimatterStone > 0
                                   || DataGameSave.dataLocal.M_ColorfulStone > 0
                                   || DataGameSave.dataLocal.M_FireStone > 0
                                   || DataGameSave.dataLocal.M_GravityStone > 0
                                   || DataGameSave.dataLocal.M_IceStone > 0;
        hotKey_BlackHole.SetActive(sceneSystem.IsUnlocked_BlackHole && checkQuantityStone);
    }

    public void OnPressed_Manager() {
        if (TutMan.doingFlag != TutMan.TUT_KEY_04_PRESS_BUTTON_MANAGER && !TutMan.IsDone(TutMan.TUT_KEY_04_PRESS_BUTTON_MANAGER))
            return;

        if (Time.time < TutMan.tutDisableTime)
            return;

        if (modeExplore == Mode.Upgrade)
            return;

        if (SpaceManager.Instance.ListSpace.IsNullOrEmpty())
            return;

        if (MenuAnimation.Instance != null) {
            MenuAnimation.Instance.CloseMenu();
        }

        if (SpaceManager.Instance.PlanetSelect == null)
            SpaceManager.Instance.PlanetSelect = SpaceManager.Instance.ListSpace[0].Planet;

        popupUpgrade.CheckShowGroup();
        popupUpgrade.SetData();
        popupUpgrade.skinPlanetManager.ReadDataPlanetCell();
        popupUpgrade.InitSkinCellMiniList(popupUpgrade.skinPlanetManager.listSkinCells);
        CheckPosition(0);

        modeExplore = Mode.Upgrade;
        GameManager.lastGameplayMode = modeExplore;

        CheckSizeButton();
        Camera.main.orthographicSize = defaultOrtho;
    }

    public void OnPressed_Explore() {
        if (TutMan.doingFlag != TutMan.TUT_KEY_09_PRESS_EXPLORE_BUTTON && !TutMan.IsDone(TutMan.TUT_KEY_09_PRESS_EXPLORE_BUTTON))
            return;

        if (Time.time < TutMan.tutDisableTime)
            return;

        if (modeExplore == Mode.Explore)
            return;

        if (MenuAnimation.Instance != null) {
            MenuAnimation.Instance.CloseMenu();
        }

        CheckPosition(3);
        modeExplore = Mode.Explore;
        GameManager.lastGameplayMode = modeExplore;
        CheckSizeButton();
        Camera.main.orthographicSize = defaultOrtho;
        TutGameplayScene.FocusBigButtonMeteor();
        TutGameplayScene.FocusBigButtonBlackHole();
    }

    public void OnPressed_Home() {
        if (modeExplore == Mode.Gameplay)
            return;

        if (MenuAnimation.Instance != null) {
            MenuAnimation.Instance.CloseMenu();
        }

        CheckPosition(1);
        modeExplore = Mode.Gameplay;
        GameManager.lastGameplayMode = modeExplore;
        CameraManager.Instance.SetSize();
        CheckSizeButton();

        if (GameManager.needToSaveData) {
            DataGameSave.SaveToServer();
        }
    }

    public void OnPressed_Event() {
        if (!TutMan.IsDone(TutMan.CAN_PRESS_BUTTON_IN_GAMEPLAY))
            return;

        if (DataGameSave.dataServer.level < 5)
            return;

        if (modeExplore == Mode.Event)
            return;

        if (MenuAnimation.Instance != null) {
            MenuAnimation.Instance.CloseMenu();
        }

        CheckPosition(2);
        modeExplore = Mode.Event;
        GameManager.lastGameplayMode = modeExplore;
        Camera.main.orthographicSize = defaultOrtho;
        CheckSizeButton();
        eventScroller.OnSwitchedToEventScreen();
    }

    public void ShowLastMode() {
        if (GameManager.lastGameplayMode == Mode.Event) {
            OnPressed_Event();
        } else if (GameManager.lastGameplayMode == Mode.Explore) {
            OnPressed_Explore();
        } else if (GameManager.lastGameplayMode == Mode.Gameplay) {
            OnPressed_Home();
        } else if (GameManager.lastGameplayMode == Mode.Upgrade) {
            OnPressed_Manager();
        }
    }

    //public void ShowEventInfo() {
    //    PopupCustom.Show("Prefabs/Pop-ups/Custom/Popup Meteor Fall Event");
    //}
    //public void ShowRank() {
    //    if (!TutMan.IsDone(TutMan.CAN_PRESS_BUTTON_IN_GAMEPLAY))
    //        return;

    //    if (DataGameSave.dataServer.level < 5)
    //        return;

    //    if (modeExplore == Mode.Event)
    //        return;

    //    if (MenuAnimation.Instance != null) {
    //        MenuAnimation.Instance.CloseMenu();
    //    }

    //    CheckPosition(2);
    //    modeExplore = Mode.Event;
    //    Camera.main.orthographicSize = defaultOrtho;
    //    CheckSizeButton();
    //    eventScroller.OnSwitchedToEventScreen();

    //    EventScroller.Instance.OnClickRank();
    //}

    public List<GameObject> itemUI = new List<GameObject>();

    public void CheckPosition(int index)
    {
        for (int i = 0; i < itemUI.Count; i++)
        {
            if (itemUI[i].GetComponent<RectTransform>())
            {
                LeanTween.moveAnchorPositionX(itemUI[i].gameObject, 1800 * (i - index), 0.3f);
            }
            else
            {
                LeanTween.moveX(itemUI[i].gameObject, 180 * (i - index), 0.3f);
            }
        }
    }

    public void OnHotKey_Attack()
    {
        if (GameStatics.IsAnimating)
            return;

        //DataGameServer enemy = null;

        //random 10% meet boss
        if (DataGameSave.bossData != null && UnityEngine.Random.Range(0, 1f) <= GameManager.BOSS_RATIO)
        {
            DataGameSave.dataEnemy = DataGameSave.bossData;
            DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.Battle].currentProgress++;

            if (BtnDailyMission.Instance)
            {
                BtnDailyMission.Instance.CheckDoneQuest();
            }

            Scenes.LastScene = SceneName.Gameplay;
            Scenes.ChangeScene(SceneName.BattlePassGameplay);
            return;
        }

        if (DataGameSave.randomEnemy == null)
        {
            DataGameSave.GetRandomEnemy();
            int index = UnityEngine.Random.Range(0, DataGameSave.listDataEnemies.Count);
            DataGameSave.dataEnemy = DataGameSave.listDataEnemies[index];
            //enemy = DataGameSave.listDataEnemies[index];

        }
        else
        {
            DataGameSave.dataEnemy = DataGameSave.randomEnemy;
            //enemy = DataGameSave.randomEnemy;
        }

        //DataGameSave.dataEnemy = enemy;
        DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.Battle].currentProgress++;

        if (BtnDailyMission.Instance) {
            BtnDailyMission.Instance.CheckDoneQuest();
        }

        Scenes.LastScene = SceneName.Gameplay;
        Scenes.ChangeScene(SceneName.Battle);
    }

    public void OnHotKey_MeoterBelt()
    {
        //sceneSystem.SwitchToMeteorScene();
        PopupCustom.Show("Prefabs/Pop-ups/Custom/Popup Meteor");
    }

    public void OnHotKey_BlackHole()
    {
        sceneSystem.OnPressed_Blackhole();
    }

    public IEnumerator DoMeteorFallEvent() {
        meteorWarningText.text = "";

        for (int i = 0; i < 10; i++)
        {
            meteorWarning.SetActive(i % 2 == 0);
            yield return new WaitForSeconds(0.5f);
        }
        meteorWarning.SetActive(true);
        meteorWarningInfo.SetActive(true);
        meteorWarningText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        for (int i = 5; i >= 0; i--)
        {
            meteorWarningText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        MeteorBelt.isMeteorFall = true;
        Scenes.ChangeScene(SceneName.MeteorBelt);
    }
}