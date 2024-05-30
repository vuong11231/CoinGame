using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Hellmade.Sound;


public class PopupExitBattle : Popups
{
    public static PopupExitBattle _Instance;
    Action _onYes, _onNo;
    public Action _onClose;
    static void CheckInstance()
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(
                Resources.Load<PopupExitBattle>("Prefabs/Pop-ups/Battle/Popup Exit Battle"),
                Popups.CanvasPopup.transform,
                false);
        }
    }

    public static void ShowYesNo(Action onYes , Action onNo = null)
    {
        CheckInstance();

        _Instance._onYes = onYes;
        _Instance._onNo = onNo;

        _Instance.Appear();
    }

    public void OnYes()
    {
        // Is Animation Activated
        if (GameStatics.IsAnimating)
            return;

        //EazySoundManager.PlayUISound(Sounds.Instance.BtnClicked);
        Sounds.IgnorePopupClose = true;
        

        _onClose = () =>
        {
            if (_onYes != null)
            {
                _onYes.Invoke();
            }
        };

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnNo()
    {
        // Is Animation Activated
        if (GameStatics.IsAnimating)
            return;

        //EazySoundManager.PlayUISound(Sounds.Instance.BtnClicked);
        Sounds.IgnorePopupClose = true;

        //
        _onClose = () =>
        {
            if (_onNo != null)
                _onNo.Invoke();
        };

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public override void NextStep(object value = null)
    {

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
}
