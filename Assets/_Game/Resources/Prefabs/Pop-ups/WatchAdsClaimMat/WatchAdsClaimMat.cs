using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hellmade.Sound;
using SteveRogers;
using System.Runtime.InteropServices;
using System;
using TMPro;

public class WatchAdsClaimMat : Popups
{
    public static WatchAdsClaimMat _Instance;

    public Text numberTxt = null;
    public Image matImg = null;
    public Button yes = null;
    public Button no = null;

    public Sprite[] mats = null;

    public Sprite diamondSpr = null;
  
    private static void CheckInstance()
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(
            Resources.Load<WatchAdsClaimMat>("Prefabs/Pop-ups/WatchAdsClaimMat/WatchAdsClaimMat"),
            Popups.CanvasPopup.transform,
            false);
        }
    }

    public static void Show(int number, TypeMaterial type, Action onOk, Action onNo, bool isDiamond)
    {
        CheckInstance();

        if (isDiamond)
        {
            number = 20;
            _Instance.matImg.sprite = _Instance.diamondSpr;
        }
        else
            _Instance.matImg.sprite = _Instance.mats[(int)type];

        _Instance.numberTxt.text = "+" + number.ToString();
        
        _Instance.yes.onClick.RemoveAllListeners();
        _Instance.yes.onClick.AddListener(() => _Instance.OnClose());
        _Instance.yes.onClick.AddListener(() => onOk.SafeCall());

        _Instance.no.onClick.RemoveAllListeners();
        _Instance.no.onClick.AddListener(() => _Instance.OnClose());
        _Instance.no.onClick.AddListener(() => onNo.SafeCall());

        _Instance.Appear();
    }

    public override void Appear()
    {
        base.Appear();
        AnimationHelper.AnimatePopupShowScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_APPEAR);
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
            });
    }

    public void OnClose()
    {
        if (GameStatics.IsAnimating)
            return;
        
        // close

        Disappear();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public override void NextStep(object value = null)
    {
        _Instance.NextStep(value);
    }
}
