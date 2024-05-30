using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Hellmade.Sound;
using SteveRogers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class PopupSearch : Popups
{
    public static PopupSearch instance;
    public static Action okButtonClick;
    public static int id;

    public InputField inputId;


    static void CheckInstance()
    {
        if (instance == null)
        {
            instance = Instantiate(
            Resources.Load<PopupSearch>("Prefabs/Pop-ups/Name/Popup Search"),
            Popups.CanvasPopup.transform,
            false);
        }
    }
    public static void Show()
    {
        CheckInstance();
        instance.Appear();
    }

    public static void Show(Action okFunction)
    {
        okButtonClick = okFunction;
        CheckInstance();
        instance.Appear();
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

                if (okButtonClick != null)
                {
                    okButtonClick.Invoke();
                    okButtonClick = null;
                }
            });
    }

    public override void NextStep(object value = null)
    {

    }

    public void OnClose()
    {
        if (GameStatics.IsAnimating)
            return;
        Disappear();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public static string WasSetName
    {
        get
        {
            return PlayerPrefs.GetString("WasSetName", null);
        }

        set
        {
            PlayerPrefs.SetString("WasSetName", value);
        }
    }

    public virtual void OnOk()
    {
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);

        if (GameStatics.IsAnimating)
            return;

        id = 0;
        int.TryParse(inputId.text, out id);

        Disappear();
    }
}
