using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Hellmade.Sound;
using TMPro;
using Lean.Pool;
public class PopupSelectPlanet : Popups
{
    static PopupSelectPlanet _Instance;
    Action _onClose;
    public Image ImgMain;
    public Text TxtTitle;
    public TextMeshProUGUI TxtCollect;
    int MoneyCollect = 0;
    public GameObject PrefabCollect;

    public GameObject RedDot;

    static void CheckInstance()
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(
            Resources.Load<PopupSelectPlanet>("Prefabs/Pop-ups/Select Planet/Popup Select Planet"),
            Popups.CanvasPopup.transform,
            false);
        }
    }

    public static void Show(Sprite Spr,TypePlanet _Type,int _Collect)
    {
        CheckInstance();
        _Instance.ImgMain.sprite = Spr;

        _Instance.TxtTitle.text = _Type.ToString();
        _Instance.MoneyCollect = _Collect;
        _Instance.Appear();
    }

    public static void UpdateMoney(int _Collect)
    {
        if (_Instance == null) return;
        _Instance.MoneyCollect = _Collect;
        _Instance.SetData();
    }

    void SetData()
    {
        TxtCollect.text = MoneyCollect + TextConstants.M_Mater;
        RedDot.SetActive(SpaceManager.Instance.PlanetSelect.CanUpgrade);
    }

    #region Overrive Methods
    public override void Appear()
    {
        base.Appear();
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
    #endregion

    public void OnClose()
    {
        if (GameStatics.IsAnimating)
            return;
        _onClose = () =>
        {
            SpaceManager.Instance.FxBack.SetActive(false);
            SpaceManager.Instance.PlanetSelect = null;
        };
        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnTransform()
    {
        _onClose = () =>
        {
            PopupTransformPlanet.Show();
        };
        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnUpgrade()
    {
        _onClose = () =>
        {
            //PopupUpgradePlanet.Show(SpaceManager.Instance.PlanetSelect.Type == TypePlanet.Default);
        };
        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnCollect()
    {
        if (GameStatics.IsAnimating)
            return;

        _onClose = () =>
        {
            if (MoneyCollect > 0)
            {
                GameObject temp = LeanPool.Spawn(PrefabCollect, SpaceManager.Instance.PlanetSelect.transform.position, Quaternion.identity);
                temp.GetComponent<CollectMaterialFx>().SetFx(MoneyCollect, TypePlanet.Default);
                DataGameSave.dataLocal.M_Material += MoneyCollect;
                RedDot.SetActive(SpaceManager.Instance.PlanetSelect.CanUpgrade);

                SpaceManager.Instance.PlanetSelect.ResetCollect();
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
                    GameObject temp = LeanPool.Spawn(PrefabCollect, SpaceManager.Instance.PlanetSelect.transform.position, Quaternion.identity);
                    temp.GetComponent<CollectMaterialFx>().SetFx(2*MoneyCollect, TypePlanet.Default);
                    DataGameSave.dataLocal.M_Material += 2*MoneyCollect;
                    RedDot.SetActive(SpaceManager.Instance.PlanetSelect.CanUpgrade);
                    SpaceManager.Instance.PlanetSelect.ResetCollect();
                }
            });
        };
        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
}
