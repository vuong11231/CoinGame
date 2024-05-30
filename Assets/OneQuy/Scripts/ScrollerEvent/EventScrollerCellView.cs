using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using SteveRogers;
using System;
using Random = UnityEngine.Random;

public class EventScrollerCellView : EnhancedScrollerCellView
{
    public GameObject rankMeImageGo = null;
    public Image imagePlanet = null;
    public Text id = null;
    public Text nameTxt = null;
    public Text level = null;
    public Text planetNumber = null;
    public Text prize = null;
    public Image prizeImage = null;
    public Image chestImage = null;

    public Button button = null;
    public Image buttonImage = null;
    public Text buttonTxt = null;

    public Sprite attackSprite = null;
    public Sprite claimSprite = null;
    public Sprite notInteractionSprite = null;
    public Sprite diamondSprite = null;

    public string buttonType = "";

    private EventUserData data = null;

    public void SetEventData(DataGameServer data, int prize) {
        //id.text = data.userid.ToString();
        nameTxt.text = data.Name;
        planetNumber.text = data.rankPoint.ToString();
        this.prize.text = prize.ToString();
    }

    public override void SetData<T>(ref T[] list, int startIndex)
    {
        var l = list as EventUserData[];
        data = l.Get(startIndex);

        SetAvatar();
        id.text = (startIndex + 1).ToString();
        nameTxt.text = ((data == null || data.name.IsNullOrEmpty() || data.name.Equals("guest")) ? "User" + data.userid : data.name);
        level.text = TextMan.Get("Level") + " " + data?.level;
        prize.text = GetPrize(startIndex).ToString();

        prize.gameObject.SetActive(startIndex != 0);
        prizeImage.gameObject.SetActive(startIndex != 0);
        chestImage.gameObject.SetActive(startIndex == 0);

        // count
        planetNumber.text = data.rankpoint.IsNullOrEmpty() ? "0" : data.rankpoint.ToString();
        //if (!data.rankpoint.IsNullOrEmpty()) {
        //    planetNumber.text = data.rankpoint.ToString();
        //} else {
        //    planetNumber.text = "0";
        //}
        
        //if (EventScroller.mode == EventScroller.EventChartMode.HitMeteor)
        //    planetNumber.text = data?.meteor_planet_hit_count;
        //else if (EventScroller.mode == EventScroller.EventChartMode.HitPlanet)
        //    planetNumber.text = data?.destroy_planet;
        //else if (EventScroller.mode == EventScroller.EventChartMode.HitSun)
        //    planetNumber.text = data?.destroyed_solars;
        //else
        //    throw new NotImplementedException(EventScroller.mode.ToString());

        // rank me

        var isMe = IsMe;
        rankMeImageGo.SetActive(isMe);

        // button

        button.onClick.RemoveAllListeners();

        buttonType = GetButtonType(data);

        button.interactable = true;
        buttonImage.enabled = true;

        if (buttonType == "getprize")
        {

            buttonImage.sprite = claimSprite;
            buttonTxt.text = TextMan.Get("Claim");

            button.onClick.AddListener(() =>
            {
                if (EventScroller.Instance.needToRefresh) {
                    return;
                }

                string lastReceived = DataGameSave.GetMetaData(MetaDataKey.RANK_LAST_RECEIVED_DAYCODE);

                if (lastReceived != null && lastReceived == DataGameSave.GetDayCode(GameManager.Now)) {
                    return;
                }

                EventScroller.Instance.needToRefresh = true;

                WWWForm form = new WWWForm();
                form.AddField("user_id", DataGameSave.dataServer.userid);
                form.AddField("day_code", DataGameSave.GetDayCode(GameManager.Now));
                form.AddField("level", DataGameSave.dataServer.level);

                ServerSystem.Instance.SendRequest(ServerConstants.INSERT_TO_NEW_RANK_CHART, form, () => {
                    DataGameSave.SaveMetaData(MetaDataKey.RANK_LAST_RECEIVED_DAYCODE, DataGameSave.GetDayCode(GameManager.Now));

                    if (EventScroller.playerRank == 1) {
                        RewardFirstRank();
                    } else {
                        int amount = GetPrize(EventScroller.playerRank - 1);

                        DataGameSave.dataLocal.Diamond += amount;
                        DataGameSave.eventDatas = null;
                        DataGameSave.SaveToServer();
                        DataGameSave.GetRankChartData();

                        PopupConfirm.ShowOK("Congratulations", "Daily ranking reward\n" + amount, "Great", () => {
                            PopupConfirm.ShowOK("Rank Point Reset", "Your ranking point has been reset. Good luck on your new challenge", "Great");
                            EventScroller.Instance.Refresh();
                        });

                        PopupConfirm.ShowMatImg(PopupConfirm._Instance.diamondImg.sprite);
                    }
                });
            });
        }
        else if (buttonType == "attack")
        {
            buttonImage.sprite = attackSprite;
            buttonTxt.text = TextMan.Get("Attack");

            var id = int.Parse(data.userid);

            button.onClick.AddListener(() =>
            {
                GameManager.isFromRank = true;
                //GameManager.Instance.Revenge(id);
                DataGameSave.GetEnemyData(id, (enemyData) => {
                    if (GameStatics.IsAnimating)
                        return;

                    if (DataGameSave.IsAllPlanetDestroyed()) {
                        PopupConfirm.ShowOK(TextConstants.NO_PLANET, TextConstants.NO_PLANET_MESSAGE);
                        return;
                    }

                    //EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);

                    DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.Battle].currentProgress++;

                    if (BtnDailyMission.Instance) {
                        BtnDailyMission.Instance.CheckDoneQuest();
                    }

                    DataGameSave.dataEnemy = enemyData;

                    Scenes.LastScene = SceneName.Gameplay;
                    Scenes.ChangeScene(SceneName.BattlePassGameplay);
                });
            });
        }
        else
        {
            buttonImage.enabled = false;
            buttonTxt.text = "";
            button.interactable = false;
        }
    }

    private void RewardFirstRank() {
        DataGameSave.RewardFirstRank();
        //int rand = Random.Range(0, 100);
        //if (rand < 60) {
        //    // +60 % random 200 đá hố đen
        //    DataReward reward = PopupShop.GetRewardRandomEffectStones(200);
        //    GameManager.reward = reward;

        //    PopupMeteorResult.Show("Congratulation", "Return", reward, okFunction: () => {
        //        DataGameSave.dataLocal.M_Material += reward.material;
        //        DataGameSave.dataLocal.M_AirStone += reward.air;
        //        DataGameSave.dataLocal.M_AntimatterStone += reward.antimater;
        //        DataGameSave.dataLocal.M_FireStone += reward.fire;
        //        DataGameSave.dataLocal.M_GravityStone += reward.gravity;
        //        DataGameSave.dataLocal.M_IceStone += reward.ice;
        //        DataGameSave.dataLocal.Diamond += reward.diamond;
        //        DataGameSave.dataLocal.M_ToyStone1 += reward.toy1;
        //        DataGameSave.dataLocal.M_ToyStone2 += reward.toy2;
        //        DataGameSave.dataLocal.M_ToyStone3 += reward.toy3;
        //        DataGameSave.dataLocal.M_ToyStone4 += reward.toy4;
        //        DataGameSave.dataLocal.M_ToyStone5 += reward.toy5;

        //        DataGameSave.dataServer.MaterialCollect += reward.material;

        //        DataGameSave.SaveToLocal();
        //        DataGameSave.SaveToServer();
        //    });
        //} else if (rand < 70) {
        //    // 1 skin hành tinh màu cam
        //    SkinDataReader.TryBuyRandomSkinPlanet(1, 0, 4);
        //} else if (rand < 80) {
        //    // + 10 % 3 skin hành tinh màu cam
        //    SkinDataReader.TryBuyRandomSkinPlanet(3, 0, 4);
        //} else if (rand < 85) {
        //    // + 5 % 10000 kc
        //    DataReward reward = new DataReward {
        //        diamond = 1000
        //    };

        //    GameManager.reward = reward;
        //    PopupMeteorResult.Show(reward: reward);

        //    DataGameSave.dataLocal.Diamond += 1000;
        //    DataGameSave.SaveToServer();
        //} else if (rand < 90) {
        //    // + 5 % hồi hành tinh trong 24h
        //    string message = string.Format(TextMan.Get("You activated auto restore for {0} mins."), 24 * 60);
        //    PopupConfirm.ShowOK(TextMan.Get("Congratulations"), message, "Great", () => {
        //        if (!GameManager.IsAutoRestorePlanet) {
        //            DataGameSave.autoRestoreEndTime = GameManager.Now.Ticks;
        //        }

        //        DateTime newEndTime = new DateTime(DataGameSave.autoRestoreEndTime).AddMinutes(24 * 60);
        //        DataGameSave.autoRestoreEndTime = newEndTime.Ticks;
        //        DataGameSave.SaveToServer();
        //    });
        //} else if (rand < 95) {
        //    // + 5 % 200 lần bắn full attack
        //    PopupConfirm.ShowOK(TextMan.Get("Congratulations"), string.Format(TextMan.Get("Received {0} times Auto Shoot"), 200), "Great", () => {
        //        DataGameSave.autoShootCount += 200;
        //        DataGameSave.SaveToServer();
        //    });
        //    DataGameSave.SaveToServer();
        //} else {
        //    // + 5 % 500 đá hố đen
        //    DataReward reward = PopupShop.GetRewardRandomEffectStones(500);
        //    GameManager.reward = reward;

        //    PopupMeteorResult.Show("Congratulation", "Return", reward, okFunction: () => {
        //        DataGameSave.dataLocal.M_Material += reward.material;
        //        DataGameSave.dataLocal.M_AirStone += reward.air;
        //        DataGameSave.dataLocal.M_AntimatterStone += reward.antimater;
        //        DataGameSave.dataLocal.M_FireStone += reward.fire;
        //        DataGameSave.dataLocal.M_GravityStone += reward.gravity;
        //        DataGameSave.dataLocal.M_IceStone += reward.ice;
        //        DataGameSave.dataLocal.Diamond += reward.diamond;
        //        DataGameSave.dataLocal.M_ToyStone1 += reward.toy1;
        //        DataGameSave.dataLocal.M_ToyStone2 += reward.toy2;
        //        DataGameSave.dataLocal.M_ToyStone3 += reward.toy3;
        //        DataGameSave.dataLocal.M_ToyStone4 += reward.toy4;
        //        DataGameSave.dataLocal.M_ToyStone5 += reward.toy5;

        //        DataGameSave.dataServer.MaterialCollect += reward.material;

        //        DataGameSave.SaveToLocal();
        //        DataGameSave.SaveToServer();
        //    });
        //}

        //DataGameSave.eventDatas = null;
        //DataGameSave.SaveToServer();
        //DataGameSave.GetRankChartData();
        //EventScroller.Instance.Refresh();
    }

    private bool IsMe
    {
        get
        {
            return data != null && data.userid.Parse(-1) == DataGameSave.dataServer.userid;
        }
    }

    private string GetButtonType(EventUserData data)
    {
        if (data == null)
        {
            return "null";
        }

        int lastReceivedReward = int.MaxValue;
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.RANK_LAST_RECEIVED_DAYCODE), out lastReceivedReward);

        int nowDayCode = int.Parse(DataGameSave.GetDayCode(GameManager.Now));
        bool firstEnterRank = DataGameSave.GetMetaData(MetaDataKey.RANK_LAST_RECEIVED_DAYCODE) == null || DataGameSave.GetMetaData(MetaDataKey.RANK_LAST_RECEIVED_DAYCODE) == "";

        // event is over
        if (nowDayCode > lastReceivedReward && !firstEnterRank)
        {
            if (int.Parse(data.userid) == DataGameSave.dataServer.userid)
            {
                return "getprize";
            }
            else
            {
                return "enemydisabled";
            }
        }
        // event is active
        else
        {
            if (int.Parse(data.userid) == DataGameSave.dataServer.userid)
            {
                return "playerdisabled";
            }
            else
            {
                return "attack";
            }
        }
    }

    private int GetPrize(int indexRank)
    {
        if (indexRank == 0)
            return 1000;
        else if (indexRank == 1)
            return 500;
        else if (indexRank == 2)
            return 300;
        else if (indexRank < 10)
            return 100;
        else
            return 50;
    }

    private void SetAvatar()
    {
        //if (data == null)
        //{
        //    imagePlanet.sprite = null;
        //    return;
        //}

        int avatarId = data.rank_chart_id.Parse(0);
        //imagePlanet.sprite = Resources.Load<Sprite>("avatar_" + avatarId);
        imagePlanet.sprite = GameManager.Instance.listAvatar[avatarId];
    }

    public void OnClickChest() {
        PopupCustom.Show("Prefabs/Pop-ups/Custom/Popup Mystery");
    }
}