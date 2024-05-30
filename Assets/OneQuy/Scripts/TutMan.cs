using System;
using UnityEngine;
using SteveRogers;

public class TutMan : SingletonPersistentStatic<TutMan>
{
    public static float tutDisableTime = 0f;

    private static GameObject canvasGo = null;
    private static RectTransform focusRect = null;

    public const string TUT_KEY_01_HIDE_GO_WHEN_FIRST_ENTER_GAME = "TUT_KEY_01_HIDE_GO_WHEN_FIRST_ENTER_GAME";
    public const string TUT_KEY_02_UPGRADE_SUN = "TUT_KEY_02_UPGRADE_SUN";
    public const string TUT_KEY_03_FOCUS_GAMEPLAY_METEOR_PLANETS = "TUT_KEY_03_FOCUS_GAMEPLAY_METEOR_PLANETS";
    public const string TUT_KEY_04_PRESS_BUTTON_MANAGER = "TUT_KEY_04_PRESS_BUTTON_MANAGER";
    public const string TUT_KEY_05_PRESS_AUTO_UPGRADE = "TUT_KEY_05_PRESS_AUTO_UPGRADE";
    public const string TUT_KEY_06_PRESS_FIRE_EFFECT_BUTTON = "TUT_KEY_06_PRESS_FIRE_EFFECT_BUTTON";
    public const string TUT_KEY_07_COMPLETE_SUN_QUEST = "TUT_KEY_07_COMPLETE_SUN_QUEST";
    public const string TUT_KEY_07_1_FOCUS_QUICK_ATTACK_NEIGHBOR = "TUT_KEY_07_1_FOCUS_QUICK_ATTACK_NEIGHBOR";

    public const string TUT_KEY_08_BACK_TO_GAMEPLAY = "TUT_KEY_08_BACK_TO_GAMEPLAY";
    public const string TUT_KEY_09_PRESS_EXPLORE_BUTTON = "TUT_KEY_09_PRESS_EXPLORE_BUTTON";
    public const string TUT_KEY_09_FOCUS_NEIGHBOR = "TUT_KEY_09_FOCUS_NEIGHBOR";
    public const string TUT_KEY_10_FOCUS_ATTACK_BUTTON_NEIGHBOR = "TUT_KEY_10_FOCUS_ATTACK_BUTTON_NEIGHBOR";
    public const string TUT_KEY_11_FOCUS_RESTORE = "TUT_KEY_11_FOCUS_RESTORE";
    public const string TUT_KEY_12_FOCUS_METEOR_BUTTON = "TUT_KEY_12_FOCUS_METEOR_BUTTON";

    // (not step)
    public const string TUT_KEY_FOCUS_BLACK_HOLE_BUTTON = "TUT_KEY_FOCUS_BLACK_HOLE_BUTTON";
    public const string TUT_KEY_HAND_BLACK_HOLE = "TUT_KEY_HAND_BLACK_HOLE";

    // other.
    public const string CAN_PRESS_BUTTON_IN_GAMEPLAY = "CAN_PRESS_BUTTON_IN_GAMEPLAY";
    public const string CAN_PRESS_METEOR_IN_GAMEPLAY = "CAN_PRESS_METEOR_IN_GAMEPLAY";
    public const string CAN_PRESS_MAIN_BUTTON_IN_GAMEPLAY = "CAN_PRESS_MAIN_BUTTON_IN_GAMEPLAY";

    public float focusTime = 0.5f;

    public static string doingFlag = null;

    public static bool IsNeedToResetTut
    {
        get
        {
            return Utilities.GetPlayerPrefsBool("IsNeedToResetTut", false);
        }

        set
        {
            Utilities.SetPlayerPrefsBool("IsNeedToResetTut", value);
        }
    }

    private static void SetupMaxFocusAndActive()
    {
        CheckAndLoadPrefab();
        focusRect.position = Utilities.VECTOR_0;
        focusRect.sizeDelta = new Vector2(51473, 51473);
        canvasGo.SetActive(true);
    }

    private static void CheckAndLoadPrefab()
    {
        if (canvasGo)
            return;

        canvasGo = Instantiate(Resources.Load<GameObject>("TutFocusCanvas"));
        focusRect = canvasGo.transform.GetChild(0).GetComponent<RectTransform>();
    }
        
    private void Update()
    {
        if (Time.time > tutDisableTime && Input.anyKeyDown) // turn off when touch
        {
            if (canvasGo && canvasGo.activeSelf)
            {
                Hide();
            }
        }
    }

    private void OnDestroy() // Reset tut! 
    {
        if (!IsDone(TUT_KEY_11_FOCUS_RESTORE)) // Not completed all tuts
        {
            IsNeedToResetTut = true;

            string[] listToReset = new string[]
            {
                TUT_KEY_02_UPGRADE_SUN,
                TUT_KEY_03_FOCUS_GAMEPLAY_METEOR_PLANETS,
                TUT_KEY_04_PRESS_BUTTON_MANAGER,
                TUT_KEY_05_PRESS_AUTO_UPGRADE,
                TUT_KEY_06_PRESS_FIRE_EFFECT_BUTTON,
                TUT_KEY_07_COMPLETE_SUN_QUEST,
                TUT_KEY_07_1_FOCUS_QUICK_ATTACK_NEIGHBOR,
                TUT_KEY_08_BACK_TO_GAMEPLAY,
                TUT_KEY_09_PRESS_EXPLORE_BUTTON,
                TUT_KEY_09_FOCUS_NEIGHBOR,
                TUT_KEY_10_FOCUS_ATTACK_BUTTON_NEIGHBOR,
                TUT_KEY_11_FOCUS_RESTORE,

                CAN_PRESS_MAIN_BUTTON_IN_GAMEPLAY,
                CAN_PRESS_BUTTON_IN_GAMEPLAY,
                CAN_PRESS_METEOR_IN_GAMEPLAY,
            };

            foreach (var i in listToReset)
            {
                SetTutNotDone(i);
            }

            SetTutDone(TUT_KEY_01_HIDE_GO_WHEN_FIRST_ENTER_GAME);
        }
    }

    public static void Hide()
    {
        LeanTween.cancel(canvasGo);
        canvasGo.SetActive(false);
        OnDisableTut();
    }

    public static bool IsDone(string name)
    {
        if (MenuItem_CheatNotTut.IsTrue(false))
            return true;

        return Utilities.GetPlayerPrefsBool("WasTut-" + name, false);
    }

    public static void Focus(Vector2 localPos, int sizeDelta, bool setMax) // sub
    {
        Focus(localPos, new Vector2(sizeDelta, sizeDelta), setMax);
    }

    public static void SetTutDone (string key)
    {
        Utilities.SetPlayerPrefsBool("WasTut-" + key, true);
    }

    public static void SetTutNotDone(string key)
    {
        Utilities.SetPlayerPrefsBool("WasTut-" + key, false);
    }

    public static void Focus(Vector2 localPos, Vector2 sizeDelta, bool setMax) // main
    {
        var w = Screen.width * 16f / Screen.height;
        var h = 16f;

        localPos = new Vector2(localPos.x * w / 9, localPos.y * h / 18);

        if (setMax)
            SetupMaxFocusAndActive();
        else
            CheckAndLoadPrefab();

        LeanTween.cancel(canvasGo);
        var startpos = focusRect.localPosition;
        var startsize = focusRect.sizeDelta;
        canvasGo.SetActive(true);

        LeanTween.value(
            canvasGo,
            (float value) =>
                {
                    focusRect.localPosition = Vector3.Lerp(startpos, localPos, value);
                    focusRect.sizeDelta = Vector2.Lerp(startsize, sizeDelta, value);
                },
            0f,
            1f,
            Instance.focusTime);
    }

    private static void OnDisableTut()
    {
        if (doingFlag == TUT_KEY_03_FOCUS_GAMEPLAY_METEOR_PLANETS)
        {
            TutGameplayScene.TutMeteor_OnPressedFocus();
            return;
        }
        else if (doingFlag == TUT_KEY_04_PRESS_BUTTON_MANAGER)
        {
            TutGameplayScene.TutButtonManager_OnPressedFocusButtonManager();
            return;
        }
        else if (doingFlag == TUT_KEY_05_PRESS_AUTO_UPGRADE)
        {
            TutGameplayScene.TutAutoUpgrade_OnPressedFocus();
            return;
        }
        else if (doingFlag == TUT_KEY_06_PRESS_FIRE_EFFECT_BUTTON)
        {
            TutGameplayScene.TutFireEffectButton_OnPressedFocus();
            return;
        }
        else if (doingFlag == TUT_KEY_07_1_FOCUS_QUICK_ATTACK_NEIGHBOR)
        {
            TutGameplayScene.TutFocusQuickAttackNeighbor_OnPressedFocus();
            return;
        }
        else if (doingFlag == TUT_KEY_09_PRESS_EXPLORE_BUTTON)
        {
            TutGameplayScene.TutFocusExploreButton_OnPressedFocus();
            return;
        }
        else if (doingFlag == TUT_KEY_09_FOCUS_NEIGHBOR)
        {
            TutGameplayScene.TutFocusNeighbor_OnPressedFocus();
            return;
        }
        else if (doingFlag == TUT_KEY_10_FOCUS_ATTACK_BUTTON_NEIGHBOR)
        {
            TutGameplayScene.TutFocusAttackButtonAtChartScreen_OnPressedFocus();
            return;
        }
        else if (doingFlag == TUT_KEY_11_FOCUS_RESTORE)
        {
            TutGameplayScene.TutFocusRestore_OnPressedFocus();
            return;
        }
        else if (doingFlag == TUT_KEY_12_FOCUS_METEOR_BUTTON)
        {
            TutGameplayScene.FocusBigButtonMeteor_OnPressedFocus();
            return;
        }
        else if (doingFlag == TUT_KEY_FOCUS_BLACK_HOLE_BUTTON)
        {
            TutGameplayScene.FocusBigButtonBlackHole_OnPressedFocus();
            return;
        }  
        else if (doingFlag == TUT_KEY_07_COMPLETE_SUN_QUEST)
        {
            TutGameplayScene.TutCompleteSunQuest_OnPressedFocus();
            return;
        }
    }

    public static void Focus(string name, bool setMax) // sub
    {
        var step = TutManData.Instance.steps.Find(n => n.name.Equals(name));
        Focus(step.localPos, step.size, setMax);
        SetTutDone(name);
    }

    [Serializable]
    public struct Step
    {
        public string name;
        public Vector2 localPos;
        public int size;
    }
}