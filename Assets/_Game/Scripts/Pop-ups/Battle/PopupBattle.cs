using Hellmade.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SteveRogers;

public class PopupBattle : Popups
{
    static PopupBattle _Instance;
    DataGameServer enemy_Data = null;
    public TextMeshProUGUI TxtMyName;
    public TextMeshProUGUI TxtEnemyName;
    public Text TxtLevel;
    public Text TxtLevelEnemy;
    public Image Avatar;
    public Image AvatarEnemy;
    public Sprite defaultAvatar;
    public Sprite vfactAvatar;

    //public GameObject Loading;

    string ava;
    Action _onClose;
    Coroutine coroutine;
    void SetData(DataGameServer enemyData, string IdPlayfab) {
        if (DataGameSave.dataServer.Name == "guest")
            TxtMyName.text = DataGameSave.GetGuestName(DataGameSave.dataServer.userid);
        else
            TxtMyName.text = DataGameSave.dataServer.Name;

        if (enemyData.Name == "guest")
            TxtEnemyName.text = DataGameSave.GetGuestName(enemyData.userid);
        else
            TxtEnemyName.text = enemyData.Name;

        TxtLevel.text = "Lv " + DataGameSave.dataServer.level;
        TxtLevelEnemy.text = "Lv " + enemyData.level;
        enemy_Data = enemyData;

        int avatarId = (enemyData.userid % 3) + 1;
        AvatarEnemy.sprite = Resources.Load<Sprite>("avatar_" + avatarId) as Sprite;

        UpdateAvatar();
    }

    public static void Show(DataGameServer enemyData,string IdPlayfab)
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(
            Resources.Load<PopupBattle>("Prefabs/Pop-ups/Battle/Popup Battle"),
            Popups.CanvasPopup.transform,
            false);
        }

        _Instance.SetData(enemyData, IdPlayfab);
        _Instance.Appear();

    }

    #region Overrive Methods
    public override void Appear()
    {

        base.Appear();
        UpdateAvatar();
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
        if (GameStatics.IsAnimating) 
            return;

        _onClose = () =>
        {
            if (!DataHelper.GetBool(TextConstants.IsTutorial, false))
            {
                DTutorial.instance.Show();

                Utilities.ActiveEventSystem = false;

                LeanTween.delayedCall(1f, () =>
                {
                    Utilities.ActiveEventSystem = true;
                    TutMan.Focus("focus sun - battle", true);
                });
            }
        };

        Disappear();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
    public void UpdateAvatar()
    {
        if (DataGameSave.dataLocal.indexAvatar != DataGameSave.dataServer.rankChartId)
        {
            Avatar.sprite = GameManager.Instance.listAvatar[DataGameSave.dataServer.rankChartId];
        }
        else
        {
            Avatar.sprite = GameManager.Instance.listAvatar[DataGameSave.dataLocal.indexAvatar];
        }
        AvatarEnemy.sprite = GameManager.Instance.listAvatar[enemy_Data.rankChartId];

        if (Scenes.Current == SceneName.BattlePassGameplay && Scenes.LastScene == SceneName.Gameplay && !GameManager.isFromRank) {
            AvatarEnemy.sprite = vfactAvatar;
        }
    }
}
