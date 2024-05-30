using Hellmade.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupBattleResult : Popups
{
    static PopupBattleResult _Instance;

    public Text impaired;

    Action _onClose;
    int totalLose;

    public static void Show()
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(
            Resources.Load<PopupBattleResult>("Prefabs/Pop-ups/Battle Result/Popup Battle Result"),
            Popups.CanvasPopup.transform,
            false);
        }
        
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

    public void OnClose()
    {
        if (GameStatics.IsAnimating) return;
        _onClose = () =>
        {
            DataGameSave.dataLocal.battleHistory.Add(new OfflineHistory
            {
                listEnemyName = DataGameSave.dataServer.ListEnemy,
                totalMaterialLose = totalLose
            });
            DataGameSave.dataServer.ListEnemy = new List<string>();
            DataGameSave.SaveToServer();
        };

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
}
