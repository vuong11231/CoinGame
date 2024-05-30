using Hellmade.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupRankReward : Popups
{
    static PopupRankReward _Instance;

    Action _onClose;

    public static void Show()
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(
            Resources.Load<PopupRankReward>("Prefabs/Pop-ups/Rank Reward/Popup Rank Reward"),
            Popups.CanvasPopup.transform,
            false);
        }

        _Instance.Appear();
    }

    #region Overrive Methods
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

    public override void NextStep(object value = null)
    {

    }
    #endregion

    public void OnClose()
    {
        if (GameStatics.IsAnimating)
            return;

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
}
