using Hellmade.Sound;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupMailDetail : Popups {
    static PopupMailDetail _Instance;
    public static DataMail _data;
    public static bool isRead;

    public TextMeshProUGUI title;
    public TextMeshProUGUI content;
    public Image sprite;
    public TextMeshProUGUI number;

    [Header("Sprite To Show In Mail")]
    public Sprite air;
    public Sprite antimater;
    public Sprite fire;
    public Sprite ice;
    public Sprite gravity;
    public Sprite diamond;
    public Sprite material;

    Action _onClose;
    
    public static void Show() {
        if (_Instance == null) {
            _Instance = Instantiate(
            Resources.Load<PopupMailDetail>("Prefabs/Pop-ups/Mailbox/Popup Mail Detail"),
            Popups.CanvasPopup.transform,
            false);
        }

        _Instance.Appear();
    }

    #region Overrive Methods
    public override void Appear() {

        base.Appear();

        title.text = _data.title;
        content.text = _data.content;

        if (_data.sprite == "diamond") {
            sprite.sprite = diamond;
        } else if (_data.sprite == "antimater") {
            sprite.sprite = antimater;
        } else if (_data.sprite == "air") {
            sprite.sprite = air;
        } else if (_data.sprite == "fire") {
            sprite.sprite = fire;
        } else if (_data.sprite == "ice") {
            sprite.sprite = ice;
        } else if (_data.sprite == "gravity") {
            sprite.sprite = gravity;
        } else {
            sprite.sprite = null;
        }

        if (_data.number != "0" && sprite.sprite != null) {
            number.text = _data.number.ToString();
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

    public override void Disappear() {
        AnimationHelper.AnimatePopupCloseScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_DISAPPEAR,
            () => {
                base.Disappear();
                if (_onClose != null) {
                    _onClose.Invoke();
                    _onClose = null;
                }
            });
    }

    public override void Disable() {
        base.Disable();

    }

    public override void NextStep(object value = null) {

    }
    #endregion

    public void OnBattleHistory() {
        if (GameStatics.IsAnimating)
            return;

        _onClose = () => {
            PopupOfflineHistory.Show();
        };

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnRankReward() {
        if (GameStatics.IsAnimating)
            return;

        _onClose = () => {
            PopupRankReward.Show();
        };

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void Onclose() {
        if (GameStatics.IsAnimating)
            return;

        Disappear();

        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void ReadMail() {
        if (GameStatics.IsAnimating)
            return;

        int amount = int.Parse(_data.number);

        DataReward reward = new DataReward();

        if (amount > 0 && !isRead)
        {
            if (_data.sprite == "material") {
                reward.material = amount;
                DataGameSave.dataLocal.M_Material += amount;
            }
            if (_data.sprite == "air")
            {
                reward.air = amount;
                DataGameSave.dataLocal.M_Air += amount;
            }
            if (_data.sprite == "antimater")
            {
                reward.antimater = amount;
                DataGameSave.dataLocal.M_Antimatter += amount;
            }
            if (_data.sprite == "fire")
            {
                reward.fire = amount;
                DataGameSave.dataLocal.M_Fire += amount;
            }
            if (_data.sprite == "gravity")
            {
                reward.gravity = amount;
                DataGameSave.dataLocal.M_Gravity += amount;
            }
            if (_data.sprite == "ice")
            {
                reward.ice = amount;
                DataGameSave.dataLocal.M_Ice += amount;
            }
            if (_data.sprite == "diamond")
            {
                reward.diamond = amount;
                DataGameSave.dataLocal.Diamond += amount;
            }

            isRead = true;
            PopupMailbox.ReadMail(_data.id);
            DataGameSave.SaveToServer();

            Onclose();
            PopupBlackHoleResult.Show("You received", "Great",reward);
            return;
        }

        Onclose();
        PopupMailbox.Show();
    }
}
