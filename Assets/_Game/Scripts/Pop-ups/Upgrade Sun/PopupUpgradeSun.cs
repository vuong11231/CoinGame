using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Hellmade.Sound;
using Lean.Pool;
using SteveRogers;

public class PopupUpgradeSun : Popups
{
    public static PopupUpgradeSun _Instance;
    Action _onClose;

    public Text TxtLevel;
    public Text TxtName;
    public TextMeshProUGUI TxtContent;
    public Button BtnUp;
    public TextMeshProUGUI TxtCollect;
    int MoneyCollect = 0;
    public GameObject PrefabCollect;

    public Transform effectFather = null;

    public Transform[] processItems = null;
    public Text upgradeTitle = null;

    public static bool Isshow = false;

    [SerializeField]
    Sprite activeImg;
    [SerializeField]
    Sprite deactiveImg;

    static void CheckInstance()
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(
            Resources.Load<PopupUpgradeSun>("Prefabs/Pop-ups/Upgrade Sun/Popup Upgrade Sun"),
            Popups.CanvasPopup.transform,
            false);
        }
    }

    public static void Show(int _Collect)
    {
        if (!TutMan.IsDone(TutMan.TUT_KEY_02_UPGRADE_SUN))
            return;

        CheckInstance();
        _Instance.MaterialNeed(SpaceManager.Instance.Data.MaterialNeed);
        _Instance.MoneyCollect = _Collect;
        _Instance.Appear();
    }

    void SetData()
    {
        //TxtCollect.text = MoneyCollect.ToString("000");
        TxtLevel.text = "Level : " + SpaceManager.Instance.Level;
        //if (SpaceManager.Instance.Level < ReadDataSun.Instance.ListDataSun.Count)
        //{
        //    TxtContent.text = "Up to level " + (SpaceManager.Instance.Level + 1).ToString() +
        //     "\n+ " + ReadDataSun.Instance.ListDataSun[SpaceManager.Instance.Level].MaterialPer5Sec + TextConstants.M_Mater + "/5s"
        //     + "\n+ Harder to be destroyed";

        //    if (SpaceManager.Instance.Data.AmountOrbit < ReadDataSun.Instance.ListDataSun[SpaceManager.Instance.Level].AmountOrbit)
        //        TxtContent.text += "\n+ Increasing one more orbit";
        //}
        //else
        //{
        //    TxtContent.text = "Your level is maximum";
        //}

        if (DataGameSave.dataServer.Name == "guest")
            TxtName.text = DataGameSave.GetGuestName(DataGameSave.dataServer.userid);
        else
            TxtName.text = DataGameSave.dataServer.Name;
        SetButtonUp(UpdateInfo(processItems, upgradeTitle));

        var sunLevel = SpaceManager.Instance.Level;

        // sun effect

        var sunvfxindex = 0;

        if (sunLevel == 1 || sunLevel == 4 || sunLevel == 7)
            sunvfxindex = 2;
        else if (sunLevel == 2 || sunLevel == 8)
            sunvfxindex = 3;
        else if (sunLevel == 3)
            sunvfxindex = 1;
        else
            sunvfxindex = 0;

        foreach (Transform t in effectFather)
            t.gameObject.SetActive(t.GetSiblingIndex() == sunvfxindex);
    }

    public override void Appear()
    {
        base.Appear();
        Isshow = true;
        SetData();
        AnimationHelper.AnimatePopupShowScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_APPEAR);
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.Show_Popup);
    }

    public override void Disappear()
    {

        AnimationHelper.AnimatePopupCloseScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_DISAPPEAR,
            () =>
            {
                base.Disappear();
                Isshow = false;

                if (_onClose != null)
                {
                    _onClose.Invoke();
                    _onClose = null;
                }

            });
    }

    public override void Disable()
    {
        base.Disable();
    }

    public override void NextStep(object value = null)
    {

    }

    private void SetButtonUp(bool active)
    {
        if (MenuItem_Debug_FullMoney.IsTrue(false) || active)
        {
            _Instance.BtnUp.interactable = true;
            _Instance.BtnUp.GetComponent<Image>().sprite = _Instance.activeImg;
        }
        else
        {
            _Instance.BtnUp.interactable = false;
            _Instance.BtnUp.GetComponent<Image>().sprite = _Instance.deactiveImg;
        }
    }

    public void MaterialNeed(int value)
    {
        //TxtMaterNeed.text = value.ToString();

        //if (SpaceManager.Instance.Level >= ReadDataSun.Instance.ListDataSun.Count)
        //{
        //    TxtMaterNeed.text = "Max";
        //}
    }

    public static void UpdateMoney(int _Collect)
    {
        _Instance.MoneyCollect = _Collect;
        _Instance.TxtCollect.text = _Instance.MoneyCollect.ToString("000");
    }

    public static bool UpdateInfo(Transform[] processItems, Text upgradeTitle)
    {
        return false;
        if (SpaceManager.Instance.ListSpace.IsNullOrEmpty())
            return false;
        var sunLevel = SpaceManager.Instance.Level;

        // conditions upgrade

        if (sunLevel == 1)
        {
            var Planet = SpaceManager.Instance.ListSpace[0].Planet;

            if (!Planet)
                return false;

            upgradeTitle.text = PlayScenesManager.Instance.GetUpgradeSunTitle(sunLevel);

            processItems[0].gameObject.SetActive(true);
            processItems[1].gameObject.SetActive(true);
            processItems[2].gameObject.SetActive(true);

            SetProcess(processItems, 0, Planet.LevelSize, 10, TextMan.Get("Upgrade max Size"));
            SetProcess(processItems, 1, Planet.LevelSpeed, 10, TextMan.Get("Upgrade max Speed"));
            SetProcess(processItems, 2, Planet.LevelHeavier, 10, TextMan.Get("Upgrade max Weight"));

            return Planet.LevelSize > 9 && Planet.LevelSpeed > 9 && Planet.LevelHeavier > 9;
        }

        else if (sunLevel == 2)
        {
            var destroyedPlanets = DataGameSave.dataLocal.DestroyPlanet;
            var destroyedSolar = DataGameSave.dataLocal.destroyedSolars;

            upgradeTitle.text = PlayScenesManager.Instance.GetUpgradeSunTitle(sunLevel);

            processItems[0].gameObject.SetActive(true);
            processItems[1].gameObject.SetActive(true);
            processItems[2].gameObject.SetActive(false);

            SetProcess(processItems, 0, destroyedPlanets, 10, TextMan.Get("Destroy 10 Planets"));
            SetProcess(processItems, 1, destroyedSolar, 5, TextMan.Get("Destroy 5 Suns"));

            return (destroyedPlanets >= 10 && destroyedSolar >= 5);
        }

        else if (sunLevel == 3)
        {
            upgradeTitle.text = PlayScenesManager.Instance.GetUpgradeSunTitle(sunLevel);

            processItems[0].gameObject.SetActive(true);
            processItems[1].gameObject.SetActive(false);
            processItems[2].gameObject.SetActive(false);

            SetProcess(processItems, 0, DataGameSave.dataLocal.meteorPlanetHitCount, 100, TextMan.Get("Destroy 100 Meteors"));

            return (DataGameSave.dataLocal.meteorPlanetHitCount >= 100);
        }

        else if (sunLevel == 4)
        {
            upgradeTitle.text = PlayScenesManager.Instance.GetUpgradeSunTitle(sunLevel);

            processItems[0].gameObject.SetActive(true);
            processItems[1].gameObject.SetActive(false);
            processItems[2].gameObject.SetActive(false);

            SetProcess(processItems, 0, DataGameSave.dataLocal.meteorSpecialPlanetHitCount, 25, TextMan.Get("Destroy 25 Specical Meteors"));
            return (DataGameSave.dataLocal.meteorSpecialPlanetHitCount >= 25);
        }

        else if (sunLevel == 5)
        {
            var destroyedSolar = DataGameSave.dataLocal.destroyedSolars;

            upgradeTitle.text = PlayScenesManager.Instance.GetUpgradeSunTitle(sunLevel);

            processItems[0].gameObject.SetActive(true);
            processItems[1].gameObject.SetActive(false);
            processItems[2].gameObject.SetActive(false);

            var value = 30;
            SetProcess(processItems, 0, destroyedSolar, value, TextMan.Get("Destroy 50 Suns").Replace("50", value.ToString()));
            return (destroyedSolar >= value);
        }

        else if (sunLevel == 6)
        {
            var number = DataGameSave.dataLocal.M_Antimatter;

            upgradeTitle.text = PlayScenesManager.Instance.GetUpgradeSunTitle(sunLevel);

            processItems[0].gameObject.SetActive(true);
            processItems[1].gameObject.SetActive(false);
            processItems[2].gameObject.SetActive(false);

            SetProcess(processItems, 0, number, 10, TextMan.Get("Collect 10 Antimatters"));
            return (number >= 10);
        }

        else if (sunLevel == 7) // ?
        {
            upgradeTitle.text = PlayScenesManager.Instance.GetUpgradeSunTitle(sunLevel);

            processItems[0].gameObject.SetActive(true);
            processItems[1].gameObject.SetActive(false);
            processItems[2].gameObject.SetActive(false);

            var val = 1000;
            SetProcess(processItems, 0, DataGameSave.dataLocal.meteorPlanetHitCount, val, TextMan.Get("Destroy 5000 Meteors").Replace("5000", val.ToString()));

            return (DataGameSave.dataLocal.meteorPlanetHitCount >= val);
        }

        else if (sunLevel == 8)
        {
            var number = DataGameSave.dataLocal.meteorMultiColorHitCount;

            upgradeTitle.text = PlayScenesManager.Instance.GetUpgradeSunTitle(sunLevel);

            processItems[0].gameObject.SetActive(true);
            processItems[1].gameObject.SetActive(false);
            processItems[2].gameObject.SetActive(false);

            SetProcess(processItems, 0, number, 50, TextMan.Get("Hit multi-color Meteors 50 times"));
            return (number >= 50);
        }

        else if (sunLevel == 9)
        {
            var destroyedPlanets = DataGameSave.dataLocal.DestroyPlanet;
            var destroyedSolar = DataGameSave.dataLocal.destroyedSolars;

            upgradeTitle.text = PlayScenesManager.Instance.GetUpgradeSunTitle(sunLevel);

            processItems[0].gameObject.SetActive(true);
            processItems[1].gameObject.SetActive(true);
            processItems[2].gameObject.SetActive(false);

            SetProcess(processItems, 0, destroyedPlanets, 1000, TextMan.Get("Destroy 1000 Planets"));
            SetProcess(processItems, 1, destroyedSolar, 100, TextMan.Get("Destroy 100 Suns"));

            return (destroyedPlanets >= 1000 && destroyedSolar >= 100);
        }

        else //if (sunLevel == 10)
        {
            upgradeTitle.text = PlayScenesManager.Instance.GetUpgradeSunTitle(sunLevel);

            processItems[0].gameObject.SetActive(false);
            processItems[1].gameObject.SetActive(false);
            processItems[2].gameObject.SetActive(false);

            return (false);
        }
    }

    public static void SetProcess(Transform[] processItems, int itemIdx, int number, int total, string title)
    {
        var tran = processItems[itemIdx];

        // title

        tran.GetChild(0).GetComponent<Text>().text = title;

        // process

        tran.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = number * 1f / total;
        tran.GetChild(1).GetChild(1).GetComponent<Text>().text = number.ToString() + "/" + total.ToString();
    }

    public void OnClose()
    {
        if (GameStatics.IsAnimating)
            return;

        Disappear();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnUpGrade() // sub 
    {
        OnUpGradeMain(DataGameSave.dataServer.level + 1);
    }

    public static void OnUpGradeMain(int level) // main 
    {
        DataGameSave.dataServer.level = level;
        SpaceManager.Instance.Level = DataGameSave.dataServer.level;
        Utilities.SetPlayerPrefsBool("just_level_up", true);

        MainPlanetController.Instance.PlaySunUpgradeEffect(SpaceManager.Instance.Level - 2);
        LeanTween.delayedCall(2, MainPlanetController.Instance.UpdateSpriteSun);

        DataGameSave.dataServer.sunHp = ReadDataSun.Instance.ListDataSun.GetLastIfOverRange(SpaceManager.Instance.Level - 1).Hp;

        if (SpaceManager.Instance.Level >= ReadDataSun.Instance.ListDataSun.Count)
        {
            if (_Instance)
                _Instance.TxtContent.text = TextMan.Get("You reached Max Level!");
        }

        DataGameSave.dataLocal.M_Material -= SpaceManager.Instance.Data.MaterialNeed;

        SpaceManager.Instance.UpgradeSun();

        if (_Instance)
        {
            _Instance.MaterialNeed(SpaceManager.Instance.Data.MaterialNeed);
            _Instance.SetData();
        }

        EazySoundManager.PlaySound(Sounds.Instance.Upgrade);

        DataGameSave.SaveToServer();
        DataGameSave.SaveToLocal();
        OnAfterLevelUp();
    }

    private static void OnAfterLevelUp()
    {
        if (DataGameSave.dataServer.level == 5)
        {
            if (UIMultiScreenCanvasMan.Instance)
                UIMultiScreenCanvasMan.Instance.CheckUnlockEventBtn();
        }
    }

    public void OnCollect()
    {
        if (GameStatics.IsAnimating)
            return;

        _onClose = () =>
        {
            if (MoneyCollect > 0)
            {
                SpaceManager.Instance.MainPlanet.ResetCollect();
                GameObject temp = LeanPool.Spawn(PrefabCollect, SpaceManager.Instance.MainPlanet.transform.position + Vector3.up * 2, Quaternion.identity);
                temp.GetComponent<CollectMaterialFx>().SetFx(MoneyCollect, TypePlanet.Default);
                DataGameSave.dataLocal.M_Material += MoneyCollect;
            }
        };
        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnCollectDouble()
    {
        _onClose = () =>
        {
            GoogleMobileAdsManager.Instance.ShowRewardedVideo(() =>
            {
                if (MoneyCollect > 0)
                {
                    //SpaceManager.Instance.PlanetSelect.ResetCollect();
                    SpaceManager.Instance.MainPlanet.ResetCollect();
                    GameObject temp = LeanPool.Spawn(PrefabCollect, SpaceManager.Instance.MainPlanet.transform.position + Vector3.up * 2, Quaternion.identity);
                    temp.GetComponent<CollectMaterialFx>().SetFx(2 * MoneyCollect, TypePlanet.Default);
                    DataGameSave.dataLocal.M_Material += 2 * MoneyCollect;
                }
            });
        };
        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
}
