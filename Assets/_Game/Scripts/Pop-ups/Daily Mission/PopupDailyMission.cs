using Hellmade.Sound;
using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DailyQuests
{
    Login,
    Battle,
    UpgradePlanet,
    TransformPlanet,
    DestroyPlanet,
    FinishAll,
}
[Serializable]
public struct DailyQuestData
{
    public DailyQuests quest;
    public int maxProgress;
}

[Serializable]
public class DailyQuestSave
{
    public int id;
    public int currentProgress;
    public bool isReceived;
}

public class PopupDailyMission : Popups
{
    static PopupDailyMission _Instance;

    public Text CountdownTxt;
    public List<Text> RewardTxts;
    public List<Image> MissionBackgrounds;
    public List<Image> RewardButtonImg;
    public List<Text> ProgessTxt;
    public List<Slider> Sliders;
    public List<GameObject> Clears;

    [Header("Sprite")]
    public Sprite LineOn;
    public Sprite LineProgress;
    public Sprite LineOff;
    public Sprite ButtonOn;
    public Sprite ButtonProgress;
    public Sprite ButtonOff;
    public Sprite ProgressOn;
    public Sprite ProgressOff;
    public Sprite[] mats = null;

    [Header("Content")]
    public GameObject DailyMission;
    public GameObject Achievement;
    public GameObject LoginReward;

    [Header("UI")]
    [SerializeField]
    private AchievementController scroller;
    public Text TabTitle;
    public Toggle toggleAchievement;
    public Toggle toggleDailyMission;
    public Toggle toggleLoginReward;

    List<DailyQuestData> _data;
    Action _onClose;
    Coroutine _CountDown;
    int _time;
    TypeMaterial randMeteorType;
    DailyQuestSave save;

    public static void Show(String selectedTabName = "DailyMission")
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(
            Resources.Load<PopupDailyMission>("Prefabs/Pop-ups/Daily Mission/Popup Daily Mission"),
            Popups.CanvasPopup.transform,
            false);
        }

        _Instance.Init();
        _Instance.Appear();
        _Instance.SetToggle("Achievement", false);
        _Instance.SetToggle("LoginReward", false);
        _Instance.SetToggle("DailyMission", false);
        _Instance.SetToggle(selectedTabName, true);
    }

    public void Init()
    {
        _data = GameManager.Instance.dailyQuestData;
        randMeteorType = (TypeMaterial)DataGameSave.dataLocal.randomMissionReward;

        for (int i = 0; i < GameConstants.DAILY_QUEST_COUNT; i++)
        {
            if(i < GameConstants.DAILY_QUEST_COUNT - 1)
            {
                RewardTxts[i].text = (DataGameSave.dataServer.level * GameConstants.NORMAL_MISSION_MULTIPLIER).ToString();
                RewardTxts[i].transform.GetChild(0).GetComponent<Image>().sprite = mats[0];
            }
            else
            {
                var icon = 0;
                switch (randMeteorType)
                {
                    case TypeMaterial.Antimatter:
                        //icon = TextConstants.M_Antimatter;
                        icon = 1;
                        break;
                    case TypeMaterial.Gravity:
                        //icon = TextConstants.M_Gravity;
                        icon = 2;
                        break;
                    case TypeMaterial.Ice:
                        //icon = TextConstants.M_Ice;
                        icon = 3;
                        break;
                    case TypeMaterial.Fire:
                        //icon = TextConstants.M_Fire;
                        icon = 4;
                        break;
                    case TypeMaterial.Air:
                        //icon = TextConstants.M_Air;
                        icon = 5;
                        break;
                }
                RewardTxts[i].text = GameConstants.FINAL_MISSION_MULTIPLIER.ToString();
                RewardTxts[i].transform.GetChild(0).GetComponent<Image>().sprite = mats[icon];
            }
            //
            save = DataGameSave.dataLocal.dailyMissions[(int)_data[i].quest];
            ProgessTxt[i].text = save.currentProgress + "/" + _data[i].maxProgress;
            Sliders[i].value = ((float)save.currentProgress) / _data[i].maxProgress;
            // Is Available
            if (save.currentProgress >= _data[i].maxProgress)
            {
                // Already received
                if (save.isReceived)
                {
                    // Huy note
                    MissionBackgrounds[i].sprite = LineOff;
                    RewardButtonImg[i].sprite = ButtonOff;
                    Clears[i].SetActive(true);
                    Sliders[i].fillRect.GetComponent<Image>().sprite = ProgressOn;
                }
                // Not received
                else
                {
                    //Huy note
                    MissionBackgrounds[i].sprite = LineOn;
                    RewardButtonImg[i].sprite = ButtonOn;
                    Clears[i].SetActive(false);
                    Sliders[i].fillRect.GetComponent<Image>().sprite = ProgressOn;
                }
            }
            // Not available
            else
            {
                //Huy note
                MissionBackgrounds[i].sprite = LineProgress;
                RewardButtonImg[i].sprite = ButtonProgress;
                Clears[i].SetActive(false);
                Sliders[i].fillRect.GetComponent<Image>().sprite = ProgressOff;
            }
        }
    }
    
    void CountDownTime()
    {
        _time = (23 - DateTime.Now.Hour) * 3600 + (59 - DateTime.Now.Minute) * 60 + 59 - DateTime.Now.Second;
        CountdownTxt.text = TextMan.Get("End in") + " " + TimeHelper.ConverSecondtoDateSpace(_time);
        if (_CountDown == null)
        {
            _CountDown = StartCoroutine(CountDown());
        }
    }

    IEnumerator CountDown()
    {
        while (true)
        {
            CountdownTxt.text = TextMan.Get("End in") + " "  + TimeHelper.ConverSecondtoDateSpace(_time);
            yield return new WaitForSecondsRealtime(1);
            _time--;
            if (_time < 0)
            {
                _onClose = () =>
                {
                    ResetData();
                    Show();
                };
                Disappear();
            }
        }
    }

    public static void ResetData()
    {
        DailyQuestSave save = null;
        DataGameSave.dataLocal.randomMissionReward = UnityEngine.Random.Range(0, 5);
        for (int i = 0; i < GameConstants.DAILY_QUEST_COUNT; i++)
        {
            save = DataGameSave.dataLocal.dailyMissions[i];
            save.currentProgress = 0;
            save.isReceived = false;
        }
    }

    public void ReceiveReward(DailyQuests quest)
    {
        // Update data
        DataGameSave.dataLocal.dailyMissions[(int)quest].isReceived = true;
        // Daily mission
        DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.FinishAll].currentProgress++;
        if (BtnDailyMission.Instance)
        {
            BtnDailyMission.Instance.CheckDoneQuest();
        }
        // Reload UI
        Init();
        //
        var index = (int)quest;
        if (index < GameConstants.DAILY_QUEST_COUNT - 1)
        {
            DataGameSave.dataLocal.M_Material += (DataGameSave.dataServer.level * GameConstants.NORMAL_MISSION_MULTIPLIER);
        }
        else
        {
            switch (randMeteorType)
            {
                case TypeMaterial.Antimatter:
                    DataGameSave.dataLocal.M_Antimatter += GameConstants.FINAL_MISSION_MULTIPLIER;
                    break;
                case TypeMaterial.Gravity:
                    DataGameSave.dataLocal.M_Gravity += GameConstants.FINAL_MISSION_MULTIPLIER;
                    break;
                case TypeMaterial.Ice:
                    DataGameSave.dataLocal.M_Ice += GameConstants.FINAL_MISSION_MULTIPLIER;
                    break;
                case TypeMaterial.Fire:
                    DataGameSave.dataLocal.M_Fire += GameConstants.FINAL_MISSION_MULTIPLIER;
                    break;
                case TypeMaterial.Air:
                    DataGameSave.dataLocal.M_Air += GameConstants.FINAL_MISSION_MULTIPLIER;
                    break;
            }
        }

        // Analytics
        //AnalyticsManager.Instance.TrackClaimDailyQuest(quest.ToString(), rewardData.rewardType.ToString(), rewardData.rewardValue);

        //Disappear();
    }

    #region Overrive Methods
    public override void Appear()
    {
       
        base.Appear();
        CountDownTime();
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

    public void OnClaim(int index)
    {
        // Is Animation Activated
        if (GameStatics.IsAnimating)
            return;
        //Sound
        //EazySoundManager.PlayUISound(Sounds.Instance.Btn_Clicked);
        Sounds.IgnorePopupClose = true;

        //
        // Is Available
        save = DataGameSave.dataLocal.dailyMissions[(int)_data[index].quest];
        if (save.currentProgress >= _data[index].maxProgress)
        {
            // Not received
            if (!save.isReceived)
            {
                ReceiveReward(_data[index].quest);
                //Sound
                EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
                if (BtnDailyMission.Instance)
                {
                    BtnDailyMission.Instance.CheckDoneQuest();
                }
                //HomeDailyQuestButton.UpdateNotification();
            }
            // Already received
            else
            {
                // Sound
                //EazySoundManager.PlaySound(Sounds.Instance.Btn_Lock);
            }
        }
        // not available
        else
        {
            // Sound
            //EazySoundManager.PlaySound(Sounds.Instance.Btn_Lock);
        }
    
    }

    public void OnClose()
    {
        // Is Animation Activated
        if (GameStatics.IsAnimating)
            return;
        //EazySoundManager.PlaySound(Sounds.Instance.Btn_Close);
        Sounds.IgnorePopupClose = true;
        //

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnToggleChanged(Toggle toggle)
    {
        UpdateToggleContent(toggle.name, toggle.isOn);
    }

    public void SetToggle(String name, bool isOn)
    {
        if (name == "DailyMission")
        {
            this.toggleDailyMission.isOn = true;
        }
        else if (name == "Achievement")
        {
            this.toggleAchievement.isOn = true;
        }
        else if (name == "LoginReward")
        {
            this.toggleLoginReward.isOn = true;
        }
    }

    public void UpdateToggleContent(String name, bool isToggleOn)
    {
        if (name == "DailyMission")
        {
            DailyMission.SetActive(isToggleOn);
            TabTitle.text = "Daily Mission";
        }
        else if (name == "Achievement")
        {
            Achievement.SetActive(isToggleOn);
            TabTitle.text = "Achievement";
            if (isToggleOn)
            {
                scroller.myScroller.JumpToDataIndex(0);
                scroller.myScroller.ReloadData();
            }
        }
        else if (name == "LoginReward")
        {
            TabTitle.text = "Login Reward";
            LoginReward.SetActive(isToggleOn);
        }
    }
}
