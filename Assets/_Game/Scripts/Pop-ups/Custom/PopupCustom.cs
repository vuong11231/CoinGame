using Hellmade.Sound;
using Newtonsoft.Json;
using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupCustom : Popups {
    static Popups _Instance;
    static Dictionary<String, Popups> dicPopups;

    Action _onClose;
    public static string showing = "";

    public static void Show(string prefabPath) {
        if (showing == prefabPath) {
            return;
        }
        showing = prefabPath;

        if (dicPopups == null) {
            dicPopups = new Dictionary<string, Popups>();
        }

        if (!dicPopups.ContainsKey(prefabPath)) {
            dicPopups.Add(prefabPath, null);
        }

        if (dicPopups[prefabPath] != null)
        {
            dicPopups[prefabPath].Appear();
            _Instance = dicPopups[prefabPath];
        }
        else {
            _Instance = Instantiate(Resources.Load<Popups>(prefabPath), Popups.CanvasPopup.transform, false);
            _Instance.Appear();
            dicPopups[prefabPath] = _Instance;
        }
    }

    public static void Hide(){
        _Instance.Disappear();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);

        if (GameStatics.IsAnimating)
            return;

        _Instance.Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
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

        showing = "";
    }

    public override void Disable()
    {
        base.Disable();

    }

    public override void NextStep(object value = null)
    {

    }
    #endregion

    public void OnBattleHistory()
    {
        if (GameStatics.IsAnimating)
            return;

        _onClose = () =>
        {
            PopupOfflineHistory.Show();
        };

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnRankReward()
    {
        if (GameStatics.IsAnimating)
            return;

        _onClose = () =>
        {
            PopupRankReward.Show();
        };

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void Onclose()
    {
        if (GameStatics.IsAnimating)
            return;

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
}
