using Hellmade.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SteveRogers;

public class PopupDailyBattle : Popups
{
    public static PopupDailyBattle _Instance;
    DataGameServer enemy_Data = null;
    public TextMeshProUGUI TxtMyName;
    public TextMeshProUGUI TxtEnemyName;
    public Text TxtLevel;
    public Text TxtLevelEnemy;
    public Image Avatar;
    public Image AvatarEnemy;
    public Sprite defaultAvatar;
    public Text TxtAttackCount;

    public GameObject btnClaimable, btnNotClaim, btnOK, btnWatchAds;

    //public GameObject Loading;

    string ava;
    Action _onClose;
    Coroutine coroutine;
    public void SetData(DataGameServer enemyData, string IdPlayfab)
    {
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
            Resources.Load<PopupDailyBattle>("Prefabs/Pop-ups/Battle/Popup Daily Battle"),
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
        TxtAttackCount.text = GameManager.DailyBattleAttackCount + "/" + GameManager.MAX_DAILY_ATTACK_COUNT.ToString();

        if (GameManager.DailyBattleAttackCount <= 0)
        {
            btnOK.SetActive(false);
            btnWatchAds.SetActive(true);
        }
        else {
            btnOK.SetActive(true);
            btnWatchAds.SetActive(false);
        }

        AnimationHelper.AnimatePopupShowScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_APPEAR);
        CheckStatusAllButton();
        EazySoundManager.PlaySound(Sounds.Instance.Show_Popup);
    }

    public void CheckStatusAllButton()
    {
        btnClaimable.gameObject.SetActive(GameManager.isClaimableDailyBattle);
        btnNotClaim.gameObject.SetActive(!GameManager.isClaimableDailyBattle);
        btnOK.GetComponent<Button>().interactable = !GameManager.isClaimableDailyBattle;
    }
    public void OnClaimable()
    {
        GameManager.isClaimableDailyBattle = false;
        DataGameSave.dataLocal.Diamond += 20;

        //reset battle enemy
        if (GameStatics.IsAnimating)
            return;

        CheckStatusAllButton();
        TryFindAnotherEnemey();
    }

    public void TryFindAnotherEnemey() {
        ServerSystem.RefreshListEnemy(() =>
        {
            for (int i = 0; i < DataGameSave.listDataEnemies.Count; i++)
            {
                if (DataGameSave.listDataEnemies[i].level == DataGameSave.dataServer.level + 1)
                {
                    StartCoroutine(RandomNewEnemy(DataGameSave.listDataEnemies, i));
                    return;
                }
            }
            TryFindAnotherEnemey();
        });
    }

    IEnumerator RandomNewEnemy(List<DataGameServer> listEnemy, int indexTrueEnemy) {
        for (int j = 0; j < 4; j++) {
            for (int i = 0; i < listEnemy.Count; i++) {
                yield return new WaitForSeconds(0.125f);
                SetData(DataGameSave.listDataEnemies[i], "");
            }
        }
        DataGameSave.dataEnemy = DataGameSave.listDataEnemies[indexTrueEnemy];
        SetData(DataGameSave.dataEnemy, "");
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
            DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.Battle].currentProgress++;

            if (BtnDailyMission.Instance)
            {
                BtnDailyMission.Instance.CheckDoneQuest();
            }

            int attackCount = GameManager.DailyBattleAttackCount;

            if (attackCount > 0) 
            {
                //Minh.ho: GameManager.DailyBattleAttackCount--   in battle scene when we finish Destroy enemy
                GameManager.isFromDailyBattle = true;
                Scenes.LastScene = SceneName.Gameplay;
                Scenes.ChangeScene(SceneName.Battle);
            }
        };

        Disappear();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void WatchAdToBattle() {
        GoogleMobileAdsManager.Instance.ShowRewardedVideo(() =>
        {
            GameManager.DailyBattleAttackCount++;
            OnOk();
        });
    }

    public void UpdateAvatar()
    {
        if (DataGameSave.dataServer.rankChartId != null && DataGameSave.dataLocal.indexAvatar != DataGameSave.dataServer.rankChartId)
        {
            Avatar.sprite = GameManager.Instance.listAvatar[DataGameSave.dataServer.rankChartId];
        }
        else
        {
            Avatar.sprite = GameManager.Instance.listAvatar[DataGameSave.dataLocal.indexAvatar];
        }
        AvatarEnemy.sprite = GameManager.Instance.listAvatar[enemy_Data.rankChartId];
    }
}
