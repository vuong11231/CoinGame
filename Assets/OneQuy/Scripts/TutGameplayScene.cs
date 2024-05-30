using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SteveRogers;

public class TutGameplayScene : MonoBehaviour
{
    private static readonly Vector2 TutTextTopPosition = new Vector2(0, 550);
    private static readonly Vector2 TutTextMiddlePosition = new Vector2(0, 150);
    private static readonly Vector2 TutTextBottomPosition = new Vector2(0, -150);

    public GameObject zoomGo = null;
    public GameObject[] hideWhenFirstEnterGameGos = null;
    public ParticleSystem effectCreatSun;   //Minh.ho add
    private static TutGameplayScene Instance = null;
    private static GameObject meteorTutGoCloned = null;

    private void Start()
    {
        Instance = this;
        CheckGiveFreeFireStone();

        if (TutMan.IsNeedToResetTut)
        {
            TutMan.IsNeedToResetTut = false;

            if (PopupName.WasSetName.IsNullOrEmpty())
            {
                TutMan.SetTutNotDone(TutMan.TUT_KEY_01_HIDE_GO_WHEN_FIRST_ENTER_GAME);
                PopupName.Show();
            }
            else
                TutFocusUpgradeSun();
        }
        else
        {
            StartTut_StartTheGame();
        }
    }

    private void CheckGiveFreeFireStone()
    {
        if (!Utilities.GetPlayerPrefsBool("recieved_free_fire", false))
        {
            Utilities.SetPlayerPrefsBool("recieved_free_fire", true);
            DataGameSave.dataLocal.M_FireStone += 1;
            DataGameSave.SaveToLocal();
        }
    }

    public static bool StartTut_StartTheGame()
    {
        var tuted = TutMan.IsDone(TutMan.TUT_KEY_01_HIDE_GO_WHEN_FIRST_ENTER_GAME);

        if (tuted)
        {
            return false;
        }

        Utilities.ActiveEventSystem = false;

        foreach (var go in Instance.hideWhenFirstEnterGameGos)
        {
            go.SetActive(false);
        }

        SpaceManager.Instance.PlayEffectCreatSun();
        LeanTween.delayedCall(2f, () => TutTextCanvas.Show(TextMan.Get("tut_start_create_sun"), new Vector2(0, 296)));
        LeanTween.delayedCall(5f, () => SpaceManager.Instance.MainPlanet.sunParticleParent.gameObject.SetActive(true));
        LeanTween.delayedCall(8, () => TutTextCanvas.Hide());
        LeanTween.delayedCall(11f, () =>
        {
            Utilities.ActiveEventSystem = true;
            PopupName.Show();
        });

        return true;
    }

    public static void OnRenameAndSelectAvatarDone()
    {
        if (!SpaceManager.Instance)
            return;

        // save finished gameplay tut 

        TutMan.SetTutDone(TutMan.TUT_KEY_01_HIDE_GO_WHEN_FIRST_ENTER_GAME);

        if (SpaceManager.Instance.callInitedListSpace)
        {
            TutFocusUpgradeSun();
            return;
        }

        // spawn planets

        Instance.StartCoroutine(SpaceManager.Instance.InitListSpace());

        // text create planet

        LeanTween.delayedCall(1f,
             () =>
             {
                 TutTextCanvas.Show(TextMan.Get("tut_start_create_planet"), new Vector2(0, 296));
             });

        // hide text create planet

        LeanTween.delayedCall(4f, () =>
        {
            TutTextCanvas.Hide();
        });

        // zoom tut

        LeanTween.delayedCall(5f, () =>
        {
            TutFocusUpgradeSun();
        });
    }

    // TutFocusUpgradeSun()

    public static void TutFocusUpgradeSun()
    {
        // show ui

        foreach (var go in Instance.hideWhenFirstEnterGameGos)
            go.CheckAndSetActive(true);

        Utilities.ActiveEventSystem = true;



        if (TutMan.IsDone(TutMan.TUT_KEY_02_UPGRADE_SUN))
            return;

        Utilities.ActiveEventSystem = false;
        TutMan.Focus(new Vector2(-231.8f, 576.9f), 15000, true);

        LeanTween.delayedCall(1, () => TutTextCanvas.Show(TextMan.Get("tut_focus_upgrade_sun"), TutTextMiddlePosition));

        // set done

        LeanTween.delayedCall(5, () =>
        {
            TutMan.Hide();
            TutTextCanvas.Hide();
            TutMan.SetTutDone(TutMan.TUT_KEY_02_UPGRADE_SUN);
            TutMeteor();
        });
    }

    // TutMeteor()

    public static void TutMeteor()
    {
        Utilities.ActiveEventSystem = true;

        // check

        if (TutMan.IsDone(TutMan.TUT_KEY_03_FOCUS_GAMEPLAY_METEOR_PLANETS))
            return;

        // start

        TutMan.doingFlag = TutMan.TUT_KEY_03_FOCUS_GAMEPLAY_METEOR_PLANETS;
        Utilities.ActiveEventSystem = false;
        var z = GameManager.Instance.meteorTutPrefab.transform.position.z;
        var go = Instantiate(GameManager.Instance.meteorTutPrefab, new Vector3(8.91f, -6.73f, z), Quaternion.identity);
        meteorTutGoCloned = go;

        var timemove = 3;

        go.LeanMove(new Vector3(0, -10, z), timemove);

        LeanTween.delayedCall(3, () => TutTextCanvas.Show(TextMan.Get("Tap meteor to collect material"), TutTextMiddlePosition));

        // set done

        LeanTween.delayedCall(timemove, () =>
        {
            TutMan.Focus(new Vector2(0, -306.98f), 15000, true);

            LeanTween.delayedCall(2f, () =>
            {
                meteorTutGoCloned.GetComponent<ParticleSystem>()?.Play();
                TutTextCanvas.Hide();
            });
        });

    }

    public static void TutMeteor_OnPressedFocus()
    {
        Utilities.ActiveEventSystem = true;
        Destroy(meteorTutGoCloned);

        PopupConfirm.ShowOK(TextMan.Get("Congratulations!"), TextMan.Get("tut_meteor_popup_content"), onOK: () =>
        {
            // done

            DataGameSave.dataLocal.M_Material += GameManager.Instance.meteorTut_NumMatReward;
            DataGameSave.dataLocal.M_Fire += 1;
            TutMan.SetTutDone(TutMan.TUT_KEY_03_FOCUS_GAMEPLAY_METEOR_PLANETS);
            TutMan.SetTutDone(TutMan.CAN_PRESS_METEOR_IN_GAMEPLAY);
            TutMan.doingFlag = null;
            MeteoriteManager.Instance.StartSpawn();

            TutButtonManager();
        });

        PopupConfirm._Instance.tutMeteorPanel_NumFireText.text = "1";
        PopupConfirm._Instance.tutMeteorPanel_NumMatText.text = GameManager.Instance.meteorTut_NumMatReward.ToString();
        PopupConfirm._Instance.tutMeteorPanel.SetActive(true);
    }

    // TutButtonManager()

    public static void TutButtonManager()
    {
        Utilities.ActiveEventSystem = true;

        // check

        if (TutMan.IsDone(TutMan.TUT_KEY_04_PRESS_BUTTON_MANAGER))
            return;

        // start
        TutMan.tutDisableTime = Time.time + 5f;

        Utilities.ActiveEventSystem = false;
        LeanTween.delayedCall(0, () => TutMan.Focus(new Vector2(-154.7f, -736), 20000, true));

        LeanTween.delayedCall(3, () => TutTextCanvas.Show(TextMan.Get("Tap manager to custom your planets"), TutTextBottomPosition));

        LeanTween.delayedCall(5, () => {
            TutTextCanvas.Hide();
            UIMultiScreenCanvasMan.Instance.buttonUpgrade.GetComponent<ParticleSystem>()?.Play();

        });


        TutMan.doingFlag = TutMan.TUT_KEY_04_PRESS_BUTTON_MANAGER;
    }

    public static void TutButtonManager_OnPressedFocusButtonManager()
    {
        TutMan.SetTutDone(TutMan.TUT_KEY_04_PRESS_BUTTON_MANAGER);
        TutMan.doingFlag = null;
        UIMultiScreenCanvasMan.Instance.buttonUpgrade.GetComponent<ParticleSystem>()?.Destroy();
        TutAutoUpgrade();
    }

    // TutAutoUpgrade()

    public static void TutAutoUpgrade()
    {
        Utilities.ActiveEventSystem = true;

        // check

        if (TutMan.IsDone(TutMan.TUT_KEY_05_PRESS_AUTO_UPGRADE))
            return;

        // start

        Utilities.ActiveEventSystem = false;
        UIMultiScreenCanvasMan.Instance.OnPressed_Manager();

        TutMan.tutDisableTime = Time.time + 5f;

        LeanTween.delayedCall(0, () =>
        {
            TutMan.Focus(new Vector2(247, 279), 15000, true);

        });
        TutMan.doingFlag = TutMan.TUT_KEY_05_PRESS_AUTO_UPGRADE;

        LeanTween.delayedCall(3f, () =>
        {
            TutTextCanvas.Show(TextMan.Get("tut_focus_auto_upgrade"), TutTextBottomPosition);
        });

        LeanTween.delayedCall(6, () => {
            PopupUpgradePlanet._Instance.BtnAutoUp.GetComponent<ParticleSystem>()?.Play();
            TutTextCanvas.Hide();
        });
    }

    public static void TutAutoUpgrade_OnPressedFocus()
    {
        TutMan.SetTutDone(TutMan.TUT_KEY_05_PRESS_AUTO_UPGRADE);
        TutMan.doingFlag = null;
        PopupUpgradePlanet._Instance.BtnAutoUp.GetComponent<ParticleSystem>()?.Destroy();
        PopupUpgradePlanet._Instance.OnPressed_AutoUpgrade();
        TutFireEffectButton();
    }

    // TutFireEffectButton

    public static void TutFireEffectButton()
    {
        Utilities.ActiveEventSystem = true;

        // check

        if (TutMan.IsDone(TutMan.TUT_KEY_06_PRESS_FIRE_EFFECT_BUTTON))
            return;

        // start

        Utilities.ActiveEventSystem = false;

        TutMan.tutDisableTime = Time.time + 6f;
        TutMan.doingFlag = TutMan.TUT_KEY_06_PRESS_FIRE_EFFECT_BUTTON;

        LeanTween.delayedCall(0, () =>
        {
            TutMan.Focus(new Vector2(12, 152), 10000, false);

        });

        LeanTween.delayedCall(2f, () =>
        {
            TutTextCanvas.Show(TextMan.Get("tut_focus_fire_effect_button"), TutTextBottomPosition);
        });

        LeanTween.delayedCall(5, () => {
            PopupUpgradePlanet._Instance.BtnFire.GetComponent<ParticleSystem>()?.Play();
            TutTextCanvas.Hide();
        });
    }

    public static void TutFireEffectButton_OnPressedFocus()
    {
        TutMan.SetTutDone(TutMan.TUT_KEY_06_PRESS_FIRE_EFFECT_BUTTON);
        TutMan.doingFlag = null;

        if (PopupUpgradePlanet._Instance.Planet.Type >= TypePlanet.Default) // already had effect
        {
            PopupUpgradePlanet._Instance.OnPressPlanetEffects((int)TypePlanet.Fire);
        }

        PopupUpgradePlanet._Instance.BtnFire.GetComponent<ParticleSystem>()?.Destroy();
        Utilities.ActiveEventSystem = false;

        LeanTween.delayedCall(2, () =>
        {
            TutCompleteSunQuest();
        });
    }

    // TutCompleteSunQuest

    public static void TutCompleteSunQuest_OnPressedFocus()
    {
        TutMan.doingFlag = null;
        TutMan.SetTutDone(TutMan.TUT_KEY_07_COMPLETE_SUN_QUEST);
        PlayScenesManager.Instance.OnFinishQuest();
        //PlayScenesManager.Instance.buttonClaim.GetComponent<ParticleSystem>()?.Destroy();
        TutBackToGameplay();
    }

    public static void TutCompleteSunQuest()
    {
        Utilities.ActiveEventSystem = true;

        // check

        if (TutMan.IsDone(TutMan.TUT_KEY_07_COMPLETE_SUN_QUEST))
            return;

        // start

        Utilities.ActiveEventSystem = false;
        UIMultiScreenCanvasMan.Instance.OnPressed_Home();
        TutMan.tutDisableTime = Time.time + 6f;

        LeanTween.delayedCall(1, () =>
        {
            TutMan.Focus(new Vector2(-45, 507), 15000, false);

        });
        TutMan.doingFlag = TutMan.TUT_KEY_07_COMPLETE_SUN_QUEST;

        LeanTween.delayedCall(3f, () =>
        {
            TutTextCanvas.Show(TextMan.Get("Click here to claim your reward and level up"), TutTextBottomPosition);
        });

        LeanTween.delayedCall(6, () => {
            PlayScenesManager.Instance.buttonClaim.GetComponent<ParticleSystem>()?.Play();
            TutTextCanvas.Hide();
        });

    }

    // TutBackToGameplay()

    public static void TutBackToGameplay()
    {
        Utilities.ActiveEventSystem = true;

        // check

        if (TutMan.IsDone(TutMan.TUT_KEY_08_BACK_TO_GAMEPLAY))
            return;

        TutMan.tutDisableTime = Time.time + 5f;

        // start

        Utilities.ActiveEventSystem = false;
        LeanTween.delayedCall(1, () => TutTextCanvas.Show(TextMan.Get("tut_back_to_gameplay"), TutTextMiddlePosition));
        LeanTween.delayedCall(5, () => TutTextCanvas.Hide());

        LeanTween.delayedCall(5, () =>
        {
            TutFocusExploreButton();
        });
    }

    public static void TutFocusQuickAttackNeighbor() {
        Utilities.ActiveEventSystem = true;

        // check

        if (TutMan.IsDone(TutMan.TUT_KEY_07_1_FOCUS_QUICK_ATTACK_NEIGHBOR))
            return;

        // start

        Utilities.ActiveEventSystem = false;
        //UIMultiScreenCanvasMan.Instance.OnPressed_Home();
        //TutMan.tutDisableTime = Time.time + 6f;

        LeanTween.delayedCall(1, () =>
        {
            TutMan.Focus(new Vector2(320f, -270f), 15000, false);

        });
        TutMan.doingFlag = TutMan.TUT_KEY_07_1_FOCUS_QUICK_ATTACK_NEIGHBOR;

        LeanTween.delayedCall(3f, () =>
        {
            TutTextCanvas.Show(TextMan.Get("Click here to fight random enemy"), TutTextBottomPosition);
        });

        LeanTween.delayedCall(6, () => {
     
            TutTextCanvas.Hide();
        });
    }

    public static void TutFocusQuickAttackNeighbor_OnPressedFocus()
    {
        UIMultiScreenCanvasMan.Instance.OnHotKey_Attack();
        //UIMultiScreenCanvasMan.Instance.buttonExplore.GetComponent<ParticleSystem>()?.Destroy();
        TutMan.doingFlag = null;
        TutMan.SetTutDone(TutMan.TUT_KEY_07_1_FOCUS_QUICK_ATTACK_NEIGHBOR);
        TutMan.SetTutDone(TutMan.CAN_PRESS_BUTTON_IN_GAMEPLAY);

        //TutFocusNeighbor();
    }

    // TutFocusExploreButton()

    public static void TutFocusExploreButton_OnPressedFocus()
    {
        UIMultiScreenCanvasMan.Instance.OnPressed_Explore();
        UIMultiScreenCanvasMan.Instance.buttonExplore.GetComponent<ParticleSystem>()?.Destroy();
        TutMan.doingFlag = null;
        TutMan.SetTutDone(TutMan.TUT_KEY_09_PRESS_EXPLORE_BUTTON);
        TutMan.SetTutDone(TutMan.CAN_PRESS_BUTTON_IN_GAMEPLAY);

        TutFocusNeighbor();
    }

    public static void TutFocusExploreButton()
    {
        Utilities.ActiveEventSystem = true;

        // check

        if (TutMan.IsDone(TutMan.TUT_KEY_09_PRESS_EXPLORE_BUTTON))
            return;

        // start

        Utilities.ActiveEventSystem = false;
        LeanTween.delayedCall(1, () => TutMan.Focus(new Vector2(319, -735), 15000, false));
        TutMan.doingFlag = TutMan.TUT_KEY_09_PRESS_EXPLORE_BUTTON;

        LeanTween.delayedCall(3f, () =>
        {
            TutTextCanvas.Show(TextMan.Get("Click here to explore your neighbor solar systems"), TutTextBottomPosition);
        });

        LeanTween.delayedCall(6, () => {
            UIMultiScreenCanvasMan.Instance.buttonExplore.GetComponent<ParticleSystem>()?.Play();
            TutTextCanvas.Hide();
        });

    }

    // TutFocusNeighbor()

    public static void TutFocusNeighbor()
    {
        Utilities.ActiveEventSystem = true;

        // check

        if (TutMan.IsDone(TutMan.TUT_KEY_09_FOCUS_NEIGHBOR))
            return;

        // start

        Utilities.ActiveEventSystem = false;
        TutMan.tutDisableTime = Time.time + 4f;

        LeanTween.delayedCall(1, () => TutMan.Focus(new Vector2(-191, 238), 15000, false));
        TutMan.doingFlag = TutMan.TUT_KEY_09_FOCUS_NEIGHBOR;

        LeanTween.delayedCall(1, () => TutTextCanvas.Show(TextMan.Get("tut_focus_neighbor"), TutTextTopPosition));
        LeanTween.delayedCall(4, () => TutTextCanvas.Hide());
    }

    public static void TutFocusNeighbor_OnPressedFocus()
    {
        TutMan.doingFlag = null;
        TutMan.SetTutDone(TutMan.TUT_KEY_09_PRESS_EXPLORE_BUTTON);
        Scenes.ChangeScene(SceneName.Neightbor);
        TutFocusAttackButtonAtChartScreen();
    }

    // TutFocusAttackButtonAtChartScreen()

    public static void TutFocusAttackButtonAtChartScreen_OnPressedFocus()
    {
        TutMan.doingFlag = null;
        TutMan.SetTutDone(TutMan.TUT_KEY_10_FOCUS_ATTACK_BUTTON_NEIGHBOR);

        //TutTextCanvas.Show(TextMan.Get("tut_focus_neighbor"), TutTextTopPosition);
        //LeanTween.delayedCall(3, () => TutTextCanvas.Hide());

        LeanTween.delayedCall(3, () =>
        {
            var id = int.Parse(NeighborSystem.firstRowUserID);
            GameManager.Instance.Revenge(id);

            //LeanTween.delayedCall(1f, () => { TutTextCanvas.Show(TextMan.Get("Drag the planet and then release to attack the enemy"), TutTextMiddlePosition);  });
            
            //LeanTween.delayedCall(3f, () => TutTextCanvas.Hide());
        });
    }

    public static void TutFocusAttackButtonAtChartScreen()
    {
        Utilities.ActiveEventSystem = true;

        // check

        if (TutMan.IsDone(TutMan.TUT_KEY_10_FOCUS_ATTACK_BUTTON_NEIGHBOR))
            return;

        // start

        Utilities.ActiveEventSystem = false;
        LeanTween.delayedCall(1, () => TutMan.Focus(new Vector2(284, 514), 10000, false));
        TutMan.doingFlag = TutMan.TUT_KEY_10_FOCUS_ATTACK_BUTTON_NEIGHBOR;
    }

    // TutFocusRestore()

    public static void TutFocusRestore()
    {
        // check

        if (!TutMan.IsDone(TutMan.TUT_KEY_10_FOCUS_ATTACK_BUTTON_NEIGHBOR))
            return;

        if (TutMan.IsDone(TutMan.TUT_KEY_11_FOCUS_RESTORE))
            return;

        UIMultiScreenCanvasMan.Instance.OnPressed_Home();

        TutMan.SetTutDone(TutMan.TUT_KEY_11_FOCUS_RESTORE);
        Utilities.ActiveEventSystem = false;
        LeanTween.delayedCall(1, () => TutMan.Focus(new Vector2(-150, 527), 10000, false));
        TutMan.doingFlag = TutMan.TUT_KEY_11_FOCUS_RESTORE;

        LeanTween.delayedCall(1, () => TutTextCanvas.Show(TextMan.Get("tut_focus_restore"), TutTextMiddlePosition));
        LeanTween.delayedCall(4, () => TutTextCanvas.Hide());

        LeanTween.delayedCall(6f, () =>
        {
            TutFocusQuickAttackNeighbor();
        });
    }

    public static void TutFocusRestore_OnPressedFocus()
    {
        TutMan.doingFlag = null;

        if (PlanetController.skipButtonRestore)
            PlanetController.skipButtonRestore.onClick.Invoke();

        Utilities.ActiveEventSystem = true;
    }

    // Tut Focus Big Button - Meteor()

    public static void FocusBigButtonMeteor()
    {
        Utilities.ActiveEventSystem = true;

        // check

        if (TutMan.IsDone(TutMan.TUT_KEY_12_FOCUS_METEOR_BUTTON))
            return;

        if (DataGameSave.dataServer.level < 3)
            return;

        // start

        Utilities.ActiveEventSystem = false;
        LeanTween.delayedCall(1, () => TutMan.Focus(new Vector2(154, -343), 15000, false));
        TutMan.doingFlag = TutMan.TUT_KEY_12_FOCUS_METEOR_BUTTON;


        LeanTween.delayedCall(3f, () =>
        {
            TutTextCanvas.Show(TextMan.Get("Click here to explore the meteor belt"), TutTextBottomPosition);
        });

        LeanTween.delayedCall(6, () => {
            //UIMultiScreenCanvasMan.Instance.buttonExplore.GetComponent<ParticleSystem>()?.Play();
            TutTextCanvas.Hide();
        });
    }

    public static void FocusBigButtonMeteor_OnPressedFocus()
    {
        TutMan.doingFlag = null;
        TutMan.SetTutDone(TutMan.TUT_KEY_12_FOCUS_METEOR_BUTTON);

        if (SceneSystem.Instance)
            SceneSystem.Instance.SwitchToMeteorScene();
    }

    // Tut Focus Big Button - Black Hole()

    public static void FocusBigButtonBlackHole()
    {
        Utilities.ActiveEventSystem = true;

        // check

        if (TutMan.IsDone(TutMan.TUT_KEY_FOCUS_BLACK_HOLE_BUTTON))
            return;

        if (DataGameSave.dataServer.level < 4)
            return;

        // start

        Utilities.ActiveEventSystem = false;
        LeanTween.delayedCall(1, () => TutMan.Focus(new Vector2(-196f, -228), 15000, false));
        TutMan.doingFlag = TutMan.TUT_KEY_FOCUS_BLACK_HOLE_BUTTON;


        LeanTween.delayedCall(3f, () =>
        {
            TutTextCanvas.Show(TextMan.Get("Click here to explore the black hole"), TutTextBottomPosition);
        });

        LeanTween.delayedCall(6, () => {
            //UIMultiScreenCanvasMan.Instance.buttonExplore.GetComponent<ParticleSystem>()?.Play();
            TutTextCanvas.Hide();
        });
    }

    public static void FocusBigButtonBlackHole_OnPressedFocus()
    {
        TutMan.doingFlag = null;
        TutMan.SetTutDone(TutMan.TUT_KEY_FOCUS_BLACK_HOLE_BUTTON);

        if (SceneSystem.Instance)
            SceneSystem.Instance.OnPressed_Blackhole();
    }

    // 

    private void Update()
    {
        if (Utilities.IsPressed_Space)
        {
            "spaced".Log();

        }

        else if (Utilities.IsPressed_Escape)
        {
            "esc".Log();

        }
    }
}