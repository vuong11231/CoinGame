using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Hellmade.Sound;
using SteveRogers;

public class PopupName : Popups
{
    public static PopupName instance;
    public static Action okButtonClick;
    public InputField inputName;

    static void CheckInstance()
    {
        if (instance == null)
        {
            instance = Instantiate(
            Resources.Load<PopupName>("Prefabs/Pop-ups/Name/Popup Name"),
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

        if (string.IsNullOrEmpty(inputName.text))
            return;

        WasSetName = inputName.text;
        Utilities.ActiveEventSystem = true;
        DataGameSave.dataServer.Name = inputName.text;
        PlayScenesManager.Instance.UpdateNameText();     
        DataGameSave.SaveToServer();
        PopupSelectAvatar.Show();
        Disappear();
    }
}
