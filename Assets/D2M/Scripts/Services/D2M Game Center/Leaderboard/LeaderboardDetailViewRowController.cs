using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardDetailViewRowController : EnhancedScrollerCellView
{
    [SerializeField]
    private Image Background;
    [SerializeField]
    private Image Avatar;
    [SerializeField]
    private Text Name;
    [SerializeField]
    private Text CurrentValue;
    [SerializeField]
    private Text CurrentRank;
    [SerializeField]
    private TextMeshProUGUI Reward;
    [SerializeField]
    private Text Level;
    [SerializeField]
    private GameObject BattleBtn;

    [Header("Sprite")]
    public Sprite defaultLine;
    public Sprite specialLine;
    public Sprite defaultAvatar;

    Coroutine coroutine;
    bool startedDownloadAvatar = false;
    string playfabId = "";

    public void SetData(LeaderboardDetailViewRowData _data, int index, int tierIndex)
    {
        playfabId = _data.playfabId;
        // rank
        SetRank(_data);
        //
        Name.text = _data.username;
        CurrentValue.text = _data.currentValue.ToString();
        // reward
        if(PopupLeaderboard.rankType != RankType.World)
        {
            var value = 0;
            switch (tierIndex)
            {
                case 0:
                    value = ReadDataRank.Instance.ListDataRank[(int)PopupLeaderboard.rankType][index].PlanetRank;
                    break;
                case 1:
                    value = ReadDataRank.Instance.ListDataRank[(int)PopupLeaderboard.rankType][index].SolarSystemRank;
                    break;
                case 2:
                    value = ReadDataRank.Instance.ListDataRank[(int)PopupLeaderboard.rankType][index].GalaxyRank;
                    break;
                case 3:
                    value = ReadDataRank.Instance.ListDataRank[(int)PopupLeaderboard.rankType][index].IntergalacticRank;
                    break;
                case 4:
                    value = ReadDataRank.Instance.ListDataRank[(int)PopupLeaderboard.rankType][index].Universe;
                    break;
            }
            Reward.text = TextConstants.M_Money + " " + value;
        }
        // avatar
        if (!_data.isYou)
        {
            Background.sprite = defaultLine;
            BattleBtn.SetActive(true);
            DownloadAvatar(_data);
        }
        else
        {
            Background.sprite = specialLine;
            BattleBtn.SetActive(false);
            if (UserLogin.Profile.Avatar != null)
                Avatar.sprite = UserLogin.Profile.Avatar;
            else
                Avatar.sprite = defaultAvatar;
        }
    }

    void SetRank(LeaderboardDetailViewRowData _data)
    {
        var position = _data.currentPosition;
        CurrentRank.text = (position + 1).ToString();
    }

    void DownloadAvatar(LeaderboardDetailViewRowData _data)
    {
        startedDownloadAvatar = false;

        if (!string.IsNullOrEmpty(_data.avatar))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            //if (gameObject.activeInHierarchy)
            //{
                startedDownloadAvatar = true;
                LeanTween.delayedCall(0.002f, () => 
                {
                    coroutine = StartCoroutine(MonoHelper.DownloadTexture(_data.avatar, (UnityEngine.Texture2D tex) =>
                    {
                          coroutine = null;
                          Avatar.color = Color.white;
                          Avatar.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                    }));
                }); 
            //}

        }
        else
            Avatar.sprite = defaultAvatar;
    }

    void StopDownloadAvatar()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void OnBattle()
    {
        if (GameStatics.IsAnimating)
            return;

        GameStatics.EnemyPlayfabId = playfabId;
        GameStatics.HasStartFromRank = true;
        DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.Battle].currentProgress++;
        if (BtnDailyMission.Instance)
        {
            BtnDailyMission.Instance.CheckDoneQuest();
        }
        Scenes.LastScene = SceneName.Gameplay;
        Scenes.ChangeScene(SceneName.Battle);
    }
}
