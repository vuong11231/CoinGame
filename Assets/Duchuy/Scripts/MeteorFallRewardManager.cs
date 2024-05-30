using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using SteveRogers;
using Newtonsoft.Json;
using System;

public class MeteorFallRewardManager : MonoBehaviour {
    public static MeteorFallRewardManager Instance;

    public static float LEVEL_GROUP_WIDTH = 40f;

    public List<BattlePassCell> cells;
    //public List<BattlePassCellData> datas;

    public List<int> prizeNormals;
    public List<int> prizeBattles;

    public Transform content;
    public GameObject cellPrefab;
    public GameObject vipBlackCover;

    [Header("For Battle Pass Cell Data")]
    public Sprite boxRed;
    public Sprite boxYellow;
    public Sprite boxBlue;
    public Sprite boxPupple;

    int winCount = 0;

    private void Awake() {
        Instance = this;
        GameStatics.IsAnimating = false;
    }

    private void OnEnable() {
        CheckPrizeReceived();
        CheckAutoRestore();
        SetupCells();
        //StartCoroutine(UpdateEnemyDisplay());
    }

    void CheckPrizeReceived() {
        try {
            prizeNormals = JsonConvert.DeserializeObject<List<int>>(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_PRIZE_RECEIVED_NORMAL));
        } catch { }
        if (prizeNormals == null || prizeNormals.Count == 0) {
            prizeNormals = new List<int>(new int[GameManager.METEOR_FALL_PRIZE_AMOUNT]);
        }
    }

    void CheckAutoRestore() {
        if (GameManager.IsAutoRestorePlanet) {
            for (int i = 0; i < DataGameSave.dataServer.ListPlanet.Count; i++) {
                DataGameSave.dataServer.ListPlanet[i].ShootTime = DateTime.MinValue;
                DataGameSave.dataServer.ListPlanet[i].Type = TypePlanet.Default;
            }
        }
    }

    public void SetupCells() {
        cells = new List<BattlePassCell>();

        //datas = new List<BattlePassCellData>();
        ////bool vip = DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_VIP_ENABLED) == "1";

        ////datas = new List<BattlePassCellData>();
        //bool vip = DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_VIP_ENABLED) == "1";

        //vipBlackCover.CheckAndSetActive(!vip);
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_WIN_MAX_COUNT), out winCount);

        // get saved data
        List<int> normal = DataGameSave.GetMetaDataObject<List<int>>(MetaDataKey.BATTLE_PASS_PRIZE_RECEIVED_NORMAL);
        List<int> battle = DataGameSave.GetMetaDataObject<List<int>>(MetaDataKey.BATTLE_PASS_PRIZE_RECEIVED_BATTLE_PASS);

        if (normal == null || normal.Count == 0) {
            normal = new List<int>(new int[GameManager.BATTLE_PASS_PRIZE_AMOUNT]);
        }

        if (battle == null || battle.Count == 0) {
            battle = new List<int>(new int[GameManager.BATTLE_PASS_PRIZE_AMOUNT]);
        }

        string s = Utilities.ReadAllText_Resources("DataApocalyse");
        var dic = Utilities.CreateDictionaryFromCSVContent(ref s);

        //datas.Add(new BattlePassCellData(1, 50, 2, 2, 0, BattlePassRewardType.DIAMOND, 100));
        //datas.Add(new BattlePassCellData(2, 55, 2, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 10));
        //datas.Add(new BattlePassCellData(3, 60, 2, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 5));
        //datas.Add(new BattlePassCellData(4, 65, 2, 2, 0, BattlePassRewardType.PLANET_RANDOM, 10));
        //datas.Add(new BattlePassCellData(5, 70, 4, 2, 1, BattlePassRewardType.CHEST, 1));
        //datas.Add(new BattlePassCellData(6, 75, 2, 2, 0, BattlePassRewardType.DIAMOND, 150));
        //datas.Add(new BattlePassCellData(7, 80, 2, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 15));
        //datas.Add(new BattlePassCellData(8, 85, 2, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 6));
        //datas.Add(new BattlePassCellData(9, 90, 2, 2, 0, BattlePassRewardType.PLANET_RANDOM, 15));
        //datas.Add(new BattlePassCellData(10, 95, 4, 2, 2, BattlePassRewardType.CHEST, 2));

        //datas.Add(new BattlePassCellData(11, 10, 40, 10, 0, BattlePassRewardType.DIAMOND, 200));
        //datas.Add(new BattlePassCellData(12, 105, 4, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 20));
        //datas.Add(new BattlePassCellData(13, 110, 4, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 7));
        //datas.Add(new BattlePassCellData(14, 115, 4, 2, 0, BattlePassRewardType.PLANET_RANDOM, 20));
        //datas.Add(new BattlePassCellData(15, 120, 4, 2, 3, BattlePassRewardType.CHEST, 3));
        //datas.Add(new BattlePassCellData(16, 125, 4, 100, 0, BattlePassRewardType.DIAMOND, 250));
        //datas.Add(new BattlePassCellData(17, 130, 4, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 25));
        //datas.Add(new BattlePassCellData(18, 135, 4, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 8));
        //datas.Add(new BattlePassCellData(19, 140, 4, 2, 0, BattlePassRewardType.PLANET_RANDOM, 25));
        //datas.Add(new BattlePassCellData(20, 145, 4, 2, 4, BattlePassRewardType.CHEST, 4));

        //datas.Add(new BattlePassCellData(21, 145, 4, 2, 0, BattlePassRewardType.DIAMOND, 300));
        //datas.Add(new BattlePassCellData(22, 150, 4, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 30));
        //datas.Add(new BattlePassCellData(23, 0, 100, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 9));
        //datas.Add(new BattlePassCellData(24, 160, 10, 2, 0, BattlePassRewardType.PLANET_RANDOM, 30));
        //datas.Add(new BattlePassCellData(25, 165, 10, 2, 5, BattlePassRewardType.CHEST, 5));
        //datas.Add(new BattlePassCellData(26, 170, 10, 2, 0, BattlePassRewardType.DIAMOND, 350));
        //datas.Add(new BattlePassCellData(27, 175, 10, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 35));
        //datas.Add(new BattlePassCellData(28, 180, 10, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 10));
        //datas.Add(new BattlePassCellData(29, 185, 10, 2, 0, BattlePassRewardType.PLANET_RANDOM, 35));
        //datas.Add(new BattlePassCellData(30, 190, 10, 2, 0, BattlePassRewardType.CHEST, 6));

        //datas.Add(new BattlePassCellData(31, 195, 10, 2, 6, BattlePassRewardType.DIAMOND, 400));
        //datas.Add(new BattlePassCellData(32, 0, 50, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 40));
        //datas.Add(new BattlePassCellData(33, 205, 10, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 11));
        //datas.Add(new BattlePassCellData(34, 210, 10, 2, 0, BattlePassRewardType.PLANET_RANDOM, 40));
        //datas.Add(new BattlePassCellData(35, 215, 10, 2, 0, BattlePassRewardType.CHEST, 7));
        //datas.Add(new BattlePassCellData(36, 220, 10, 2, 7, BattlePassRewardType.DIAMOND, 450));
        //datas.Add(new BattlePassCellData(37, 225, 10, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 45));
        //datas.Add(new BattlePassCellData(38, 230, 10, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 12));
        //datas.Add(new BattlePassCellData(39, 235, 10, 2, 0, BattlePassRewardType.PLANET_RANDOM, 45));
        //datas.Add(new BattlePassCellData(40, 240, 10, 2, 8, BattlePassRewardType.CHEST, 8));

        //datas.Add(new BattlePassCellData(41, 245, 10, 2, 0, BattlePassRewardType.DIAMOND, 500));
        //datas.Add(new BattlePassCellData(42, 250, 10, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 50));
        //datas.Add(new BattlePassCellData(43, 255, 10, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 13));
        //datas.Add(new BattlePassCellData(44, 260, 10, 2, 0, BattlePassRewardType.PLANET_RANDOM, 50));
        //datas.Add(new BattlePassCellData(45, 0, 10, 120, 20, BattlePassRewardType.CHEST, 9));
        //datas.Add(new BattlePassCellData(46, 270, 10, 2, 0, BattlePassRewardType.DIAMOND, 550));
        //datas.Add(new BattlePassCellData(47, 275, 10, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 55));
        //datas.Add(new BattlePassCellData(48, 280, 10, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 14));
        //datas.Add(new BattlePassCellData(49, 285, 10, 2, 0, BattlePassRewardType.PLANET_RANDOM, 55));
        //datas.Add(new BattlePassCellData(50, 0, 0, 0, 40, BattlePassRewardType.CHEST, 10));

        foreach (Transform trans in content) {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < MeteorBelt.meteorFallDatas.Count; i++) {
            GameObject go = Instantiate(cellPrefab, content);
            cells.Add(go.GetComponent<BattlePassCell>());
            cells[i].LoadData(MeteorBelt.meteorFallDatas[i]);
            cells[i].SetCollectableProgress(winCount > i, normal[i], battle[i]);
            cells[i].index = i;
        }

        LEVEL_GROUP_WIDTH = cellPrefab.GetComponent<RectTransform>().sizeDelta.x;

        content.GetComponent<RectTransform>().sizeDelta = new Vector2(LEVEL_GROUP_WIDTH * 50, content.GetComponent<RectTransform>().sizeDelta.y);
        content.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(winCount - 1) * LEVEL_GROUP_WIDTH, 0);

        int index = winCount >= MeteorBelt.meteorFallDatas.Count ? MeteorBelt.meteorFallDatas.Count : winCount;
        if (index >= 1) {
            bool rewardEffect = normal[index - 1] == 0 || battle[index - 1] == 0 && (DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_VIP_ENABLED) == "1");

            if (rewardEffect) {
                PopupConfirm.ShowOK(TextMan.Get("Information"), TextMan.Get("New reward unlocked!"));
            }
        }
    }

    public void ReceivePrize(int index, bool isBattlePassPrize = false) {
        //if (isBattlePassPrize && DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_VIP_ENABLED) != "1") {
        //    BuyVipBattlePass();
        //    return;
        //}

        Debug.Log("THIS IS RECEIPRIZE");

        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_WIN_MAX_COUNT), out winCount);

        // get saved data
        List<int> normal = DataGameSave.GetMetaDataObject<List<int>>(MetaDataKey.METEOR_FALL_PRIZE_RECEIVED_NORMAL);
        List<int> battle = DataGameSave.GetMetaDataObject<List<int>>(MetaDataKey.METEOR_FALL_PRIZE_RECEIVED_BATTLE);

        if (normal == null || normal.Count == 0) {
            normal = new List<int>(new int[GameManager.BATTLE_PASS_PRIZE_AMOUNT]);
        }

        if (battle == null || battle.Count == 0) {
            battle = new List<int>(new int[GameManager.BATTLE_PASS_PRIZE_AMOUNT]);
        }

        if (isBattlePassPrize && battle[index] == 1 || !isBattlePassPrize && normal[index] == 1) {
            return;
        }
        //wincoun 1 --> index 0 true, index 1 false
        //wincount 2 --> index 1 true, index 2 false
        if (winCount <= index) {
            return;
        }

        BattlePassCellData data = MeteorBelt.meteorFallDatas[index];
        int amount;
        if (isBattlePassPrize) {
            amount = data.rewardBattlePass;
        } else {
            amount = data.rewardNormal;
        }

        if (data.rewardType == BattlePassRewardType.DIAMOND) {
            DataReward reward = new DataReward {
                diamond = amount
            };

            GameManager.reward = reward;
            PopupMeteorResult.Show(reward: reward);

            DataGameSave.dataLocal.Diamond += amount;
            DataGameSave.SaveToServer();
        } else if (data.rewardType == BattlePassRewardType.BLACK_HOLE_STONES_RANDOM) {
            DataReward reward = PopupShop.GetRewardRandomEffectStones(amount);
            GameManager.reward = reward;

            PopupMeteorResult.Show("Congratulation", "Return", reward, okFunction: () => {
                DataGameSave.dataLocal.M_Material += reward.material;
                DataGameSave.dataLocal.M_AirStone += reward.air;
                DataGameSave.dataLocal.M_AntimatterStone += reward.antimater;
                DataGameSave.dataLocal.M_FireStone += reward.fire;
                DataGameSave.dataLocal.M_GravityStone += reward.gravity;
                DataGameSave.dataLocal.M_IceStone += reward.ice;
                DataGameSave.dataLocal.Diamond += reward.diamond;
                DataGameSave.dataLocal.M_ToyStone1 += reward.toy1;
                DataGameSave.dataLocal.M_ToyStone2 += reward.toy2;
                DataGameSave.dataLocal.M_ToyStone3 += reward.toy3;
                DataGameSave.dataLocal.M_ToyStone4 += reward.toy4;
                DataGameSave.dataLocal.M_ToyStone5 += reward.toy5;

                DataGameSave.dataServer.MaterialCollect += reward.material;

                DataGameSave.SaveToLocal();
                DataGameSave.SaveToServer();
            });
        } else if (data.rewardType == BattlePassRewardType.PLANET_RANDOM) {
            SkinDataReader.TryBuyRandomSkinPlanet(amount, 0);
        } else if (data.rewardType == BattlePassRewardType.EFFECT_STONES_RANDOM) {
            DataReward reward = PopupShop.GetRewardRandomEffectStones(amount);

            PopupBattleResult2.Show(reward: reward,
                okFunction: () => {
                    DataGameSave.dataLocal.M_IceStone += reward.ice;
                    DataGameSave.dataLocal.M_FireStone += reward.fire;
                    DataGameSave.dataLocal.M_AirStone += reward.air;
                    DataGameSave.dataLocal.M_GravityStone += reward.gravity;
                    DataGameSave.dataLocal.M_AntimatterStone += reward.antimater;

                    DataGameSave.SaveToLocal();
                    DataGameSave.SaveToServer();
                });
        } else if (data.rewardType == BattlePassRewardType.AUTO_RESTORE_PLANET) {

            string message = string.Format(TextMan.Get("You activated auto restore for {0} mins."), amount);
            PopupConfirm.ShowOK(TextMan.Get("Congratulations"), message, "Great", () => {
                if (!GameManager.IsAutoRestorePlanet) {
                    DataGameSave.autoRestoreEndTime = GameManager.Now.Ticks;
                }

                DateTime newEndTime = new DateTime(DataGameSave.autoRestoreEndTime).AddMinutes(amount);
                DataGameSave.autoRestoreEndTime = newEndTime.Ticks;
                DataGameSave.SaveToServer();
            });
        } else if (data.rewardType == BattlePassRewardType.PLANET_ORANGE) {
            SkinDataReader.TryBuyRandomSkinPlanet(amount, 0, 4);
        } else if (data.rewardType == BattlePassRewardType.PLANET_YELLOW) {
            SkinDataReader.TryBuyRandomSkinPlanet(amount, 0, 3);
        } else if (data.rewardType == BattlePassRewardType.PLANET_PUPPLE) {
            SkinDataReader.TryBuyRandomSkinPlanet(amount, 0, 2);
        }

        if (isBattlePassPrize) {
            battle[index] = 1;
        } else {
            normal[index] = 1;
        }
        cells[index].SetCollectableProgress(true, normal[index], battle[index]);
        DataGameSave.SaveMetaData(MetaDataKey.METEOR_FALL_PRIZE_RECEIVED_NORMAL, JsonConvert.SerializeObject(normal));
        DataGameSave.SaveMetaData(MetaDataKey.METEOR_FALL_PRIZE_RECEIVED_BATTLE, JsonConvert.SerializeObject(battle));
    }

    public void OnClickButtonBuyVipBattlePass() {
        BuyVipBattlePass();
    }

    public static void BuyVipBattlePass() {
        IAPManager.Instance.Purchase(IAPItem.Diamond_4,
                 (product) => {
                     DataGameSave.SaveMetaData(MetaDataKey.BATTLE_PASS_VIP_ENABLED, "1");
                     DataGameSave.SaveToServer();
                     PopupShop.TrackBuy("battle_pass", product);
                     PopupConfirm.ShowOK(TextMan.Get("Congratulations"), TextMan.Get("Battle Pass vip activated"));
                     BattlePass.Instance.vipBlackCover.SetActive(false);
                     BattlePass.Instance.butonBuyVipBattlePass.SetActive(false);
                 },
                 () => {
                     PopupConfirm.ShowOK("Error!!!", "Purchase failed, please try again.");
                 });
    }
}
