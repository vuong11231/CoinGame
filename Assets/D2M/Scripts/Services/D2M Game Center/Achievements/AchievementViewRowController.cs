using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hellmade.Sound;
using SteveRogers;

public class AchievementViewRowController : EnhancedScrollerCellView
{
    [SerializeField]
    private Text AchievementName;
    [SerializeField]
    private Slider CounterSlider;
    [SerializeField]
    private Text RewardValue;

    [SerializeField]
    private GameObject matImageGo = null;

    [SerializeField]
    private GameObject matImageGo_Multi = null;

    [SerializeField]
    private Image ClaimButtonImg;
    [SerializeField]
    private Image RowImg;
    [SerializeField]
    private Image ProgressLineImg;
    [SerializeField]
    private GameObject ClearImg;
    [SerializeField]
    private Button ClaimBtn;
    [SerializeField]
    private Text ProgressTxt;

    [Header("Data")]
    public Sprite ButtonSprOff;
    public Sprite ButtonSprOn;
    public Sprite ButtonSprProgress;
    public Sprite RowSprOff;
    public Sprite RowSprOn;
    public Sprite RowSprProgress;
    public Sprite LineSprOn;
    public Sprite LineSprProgress;

    Transform parent;
    AchievementViewRowData data;
    int currentLevel;

    public void SetData(AchievementViewRowData _data, int index, Transform _parent)
    {
        parent = _parent;
        data = _data;
        //
        UpdateUI();
    }

    void UpdateUI()
    {
        var count = GameCenters.MaxProcess[(int)data.type].Length;
        for (int i = 0; i < count; i++)
        {
            var progress = GetProgess(data.type) / GameCenters.MaxProcess[(int)data.type][i];
            if (i < count - 1 && progress >= 1 && data.isReceived[i])
                continue;
            //
            CounterSlider.value = progress;
            // icon
            if (i <= count - 1 && progress >= 1 && !data.isReceived[i])
            {
                ClaimButtonImg.sprite = ButtonSprOn;
                RowImg.sprite = RowSprOn;
                ProgressLineImg.gameObject.SetActive(true);
                ProgressLineImg.sprite = LineSprOn;
                ClearImg.SetActive(false);
                ClaimBtn.interactable = true;
            }
            else if (i == count - 1 && progress >= 1 && data.isReceived[i])
            {
                ClaimButtonImg.sprite = ButtonSprOff;
                RowImg.sprite = RowSprOff;
                ProgressLineImg.gameObject.SetActive(false);
                ClearImg.SetActive(true);
                ClaimBtn.interactable = false;
            }
            else if (i <= count - 1 && progress < 1)
            {
                ClaimButtonImg.sprite = ButtonSprProgress;
                RowImg.sprite = RowSprProgress;
                ProgressLineImg.gameObject.SetActive(true);
                ProgressLineImg.sprite = LineSprProgress;
                ClearImg.SetActive(false);
                ClaimBtn.interactable = false;
            }
            // Disc
            var text = data.achievementName[i].Replace("<br>", "\n");
            //if (text.Length > 20 && !text.Contains("<sprite="))
            //{
            //    text = text.Substring(0, 20) + "...";
            //}
            AchievementName.text = TextMan.Get(text).Replace("#", data.amount[i].ToString());
            //
            ProgressTxt.text = GetProgess(data.type) + "/" + GameCenters.MaxProcess[(int)data.type][i];
            //
            matImageGo_Multi.SetActive(false);
            matImageGo.SetActive(false);
            switch (data.type)
            {
                case Achievements.level_up:
                case Achievements.watch_ads:
                case Achievements.claim_daily_quest:
                    matImageGo_Multi.SetActive(true);
                    RewardValue.text = data.meteriorReward[i].ToString();
                    break;
                default:
                    matImageGo.SetActive(true);
                    RewardValue.text = data.meteriorReward[i].ToString();
                    break;
            }
            currentLevel = i;
            break;
        }
    }

    public static float GetProgess(Achievements type)
    {
        float progress = 0;

        switch (type)
        {
            case Achievements.destroy_all_with_1_shot:
                progress = DataGameSave.dataLocal.DestroySolarByOneHit;
                break;
            case Achievements.level_up:
                progress = DataGameSave.dataServer.level;
                break;
            case Achievements.battle:
                progress = DataGameSave.dataLocal.StartBattle;
                break;
            case Achievements.upgrade_planet:
                progress = DataGameSave.dataLocal.UpgradePlanet;
                break;
            case Achievements.be_attack:
                progress = DataGameSave.dataServer.beAttacked;
                break;
            case Achievements.watch_ads:
                progress = DataGameSave.dataLocal.WatchAds;
                break;
            case Achievements.destroy_planet:
                progress = DataGameSave.dataLocal.DestroyPlanet;
                break;
            case Achievements.transform_planet:
                progress = DataGameSave.dataLocal.TransformPlanet;
                break;
            //case Achievements.invite_friend:
            //    progress = DataGameSave.dataLocal.InviteFriend;
            //    break;
            case Achievements.collect_meteor:
                progress = DataGameSave.dataLocal.CollectElementMeterior;
                break;
            case Achievements.claim_daily_quest:
                progress = DataGameSave.dataLocal.CompleteDailyQuest;
                break;
        }

        return progress;
    }

    public void OnClaim()
    {
        switch (data.type)
        {
            case Achievements.level_up:
            case Achievements.watch_ads:
            case Achievements.claim_daily_quest:
                DataGameSave.dataLocal.M_Air += data.meteriorReward[currentLevel];
                DataGameSave.dataLocal.M_Antimatter += data.meteriorReward[currentLevel];
                DataGameSave.dataLocal.M_Fire += data.meteriorReward[currentLevel];
                DataGameSave.dataLocal.M_Gravity += data.meteriorReward[currentLevel];
                DataGameSave.dataLocal.M_Ice += data.meteriorReward[currentLevel];
                break;
            default:
                DataGameSave.dataLocal.M_Material += data.meteriorReward[currentLevel];
                break;
        }
        DataHelper.SetBool("Achievement_" + data.type + "_" + currentLevel, true);
        data.isReceived[currentLevel] = true;
        UpdateUI(); 
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
}
