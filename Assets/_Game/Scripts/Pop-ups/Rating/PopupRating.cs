using Hellmade.Sound;
using Newtonsoft.Json;
using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupRating : Popups {
    static PopupRating _Instance;

    public GameObject giftButton;

    Action _onClose;

    public static void Show() {
        if (_Instance == null) {
            _Instance = Instantiate(
            Resources.Load<PopupRating>("Prefabs/Pop-ups/Rating/Popup Rating"),
            Popups.CanvasPopup.transform,
            false);
        }

        _Instance.Appear();
    }

    public static void Hide(){
        _Instance.Disappear();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick); _Instance.Onclose();
    }

    #region Overrive Methods
    public override void Appear()
    {
        base.Appear();

        if (DataGameSave.GetMetaData(MetaDataKey.GIFT_LETTER) == "true") {
            giftButton.SetActive(false);
        }

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

    public void OnOpenFacebookGroup()
    {
        Application.OpenURL("https://www.facebook.com/groups/795777791029455");
    }

    public void OnOpenPlayStore()
    {
        Application.OpenURL(GameManager.LINK_OF_GAME_ON_STORE);
    }

    public void ReceiveGift() 
    {
        string gift = DataGameSave.GetMetaData(MetaDataKey.GIFT_LETTER);
        if (gift != null && gift == "true") {
            return;
        }

        int _air = 0;
        int _gravity = 0;
        int _fire = 0;
        int _anti = 0;
        int _ice = 0;

        for (int i = 0; i < 20; i++)
        {
            var t = (TypePlanet)UnityEngine.Random.Range(0, (int)TypePlanet.Default);

            if (t == TypePlanet.Air)
                _air++;
            else if (t == TypePlanet.Fire)
                _fire++;
            else if (t == TypePlanet.Ice)
                _ice++;
            else if (t == TypePlanet.Gravity)
                _gravity++;
            else if (t == TypePlanet.Antimatter)
                _anti++;
        }

        PopupBattleResult2.Show(
            reward: new DataReward
            {
                material = 0,
                air = _air,
                fire = _fire,
                ice = _ice,
                gravity = _gravity,
                antimater = _anti,
                diamond = 200
            },

            okFunction: () =>
            {
                DataGameSave.dataLocal.M_IceStone += _ice;
                DataGameSave.dataLocal.M_FireStone += _fire;
                DataGameSave.dataLocal.M_AirStone += _air;
                DataGameSave.dataLocal.M_GravityStone += _gravity;
                DataGameSave.dataLocal.M_AntimatterStone += _anti;
                DataGameSave.dataLocal.Diamond += 200;

                giftButton.SetActive(false);
                DataGameSave.SaveMetaData(MetaDataKey.GIFT_LETTER, "true");
                DataGameSave.SaveToLocal();
                DataGameSave.SaveToServer();
            }); ;
    }
}
