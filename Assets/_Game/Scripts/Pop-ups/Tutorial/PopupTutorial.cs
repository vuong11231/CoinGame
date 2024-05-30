using Hellmade.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupTutorial : Popups
{
    static PopupTutorial _Instance;

    Action _onClose;

    public GameObject hand;

    public static void Show()
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(
            Resources.Load<PopupTutorial>("Prefabs/Pop-ups/Tutorial/Popup Tutorial"),
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

        GameObject planet = GameObject.Find("planetdefault");
        GameObject go = Instantiate(hand, planet.transform);
        go.transform.localPosition = new Vector3(1, 1, 1);
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
    public void OnOk()
    {
        if (GameStatics.IsAnimating) return;
        DataHelper.SetBool(TextConstants.IsTutorial, true);
        Disappear();
    }
}
