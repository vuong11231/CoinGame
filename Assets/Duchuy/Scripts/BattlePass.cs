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

public enum BattlePassRewardType {
    DIAMOND,
    BLACK_HOLE_STONES_RANDOM,
    PLANET_RANDOM,
    EFFECT_STONES_RANDOM,
    AUTO_RESTORE_PLANET,
    PLANET_ORANGE,
    PLANET_YELLOW,
    PLANET_PUPPLE,
    CHEST
}

public class BattlePass : MonoBehaviour {
    public static BattlePass Instance;

    public static float LEVEL_GROUP_WIDTH = 40f;

    public List<BattlePassCell> cells;
    public List<BattlePassCellData> datas;

    public List<int> prizeNormals;
    public List<int> prizeBattles;

    public Transform content;
    public GameObject cellPrefab;
    public GameObject vipBlackCover;
    public GameObject butonBuyVipBattlePass;
    public Image enemyImg;
    public TextMeshProUGUI enemyName;
    public TextMeshProUGUI enemyLevel;

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

    private void Start() {
        CheckPrizeReceived();
        CheckAutoRestore();
        SetupCells();
        StartCoroutine(UpdateEnemyDisplay());
    }

    void CheckPrizeReceived() {
        try {
            prizeNormals = JsonConvert.DeserializeObject<List<int>>(DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_PRIZE_RECEIVED_NORMAL));
        } catch { }
        if (prizeNormals == null || prizeNormals.Count == 0) {
            prizeNormals = new List<int>(new int[GameManager.BATTLE_PASS_PRIZE_AMOUNT]);
        }

        try {
            prizeBattles = JsonConvert.DeserializeObject<List<int>>(DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_PRIZE_RECEIVED_BATTLE_PASS));
        } catch { }
        if (prizeBattles == null || prizeBattles.Count == 0) {
            prizeBattles = new List<int>(new int[GameManager.BATTLE_PASS_PRIZE_AMOUNT]);
        }
    }

    IEnumerator UpdateEnemyDisplay() {
        if (DataGameSave.battlePassEnemy == null) {
            DataGameSave.GetBattlePassEnemy();
            yield return new WaitUntil(() => { return DataGameSave.battlePassEnemy != null; });
        }

        enemyName.text = DataGameSave.battlePassEnemy.Name;
        enemyLevel.text = "level " + DataGameSave.battlePassEnemy.level;
        enemyImg.sprite = GameManager.Instance.listAvatar.GetLastIfOverRange(DataGameSave.battlePassEnemy.rankChartId);

        //if (DataGameSave.battlePassEnemy == null) {
        //    DataGameSave.GetBattlePassEnemy();
        //} else {
        //    enemyName.text = DataGameSave.battlePassEnemy.Name;
        //    enemyLevel.text = "level " + DataGameSave.battlePassEnemy.level;
        //    enemyImg.sprite = GameManager.Instance.listAvatar.GetLastIfOverRange(DataGameSave.battlePassEnemy.rankChartId);
        //}
    }

    void CheckAutoRestore() {
        if (GameManager.IsAutoRestorePlanet) {
            for (int i = 0; i < DataGameSave.dataServer.ListPlanet.Count; i++)
            {
                DataGameSave.dataServer.ListPlanet[i].ShootTime = DateTime.MinValue;
                DataGameSave.dataServer.ListPlanet[i].Type = TypePlanet.Default;
            }
        } 
    }

    public void SetupCells() {
        cells = new List<BattlePassCell>();
        datas = new List<BattlePassCellData>();
        bool vip = DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_VIP_ENABLED) == "1";
        Debug.Log("BATTLE PASS VIP : " +  DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_VIP_ENABLED));
        vipBlackCover.CheckAndSetActive(!vip);
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_WIN_COUNT), out winCount);

        // get saved data
        List<int> normal = DataGameSave.GetMetaDataObject<List<int>>(MetaDataKey.BATTLE_PASS_PRIZE_RECEIVED_NORMAL);
        List<int> battle = DataGameSave.GetMetaDataObject<List<int>>(MetaDataKey.BATTLE_PASS_PRIZE_RECEIVED_BATTLE_PASS);

        if (normal == null || normal.Count == 0) {
            normal = new List<int>(new int[GameManager.BATTLE_PASS_PRIZE_AMOUNT]);
        }

        if (battle == null || battle.Count == 0) {
            battle = new List<int>(new int[GameManager.BATTLE_PASS_PRIZE_AMOUNT]);
        }

        datas.Add(new BattlePassCellData(1, boxBlue, BattlePassRewardType.DIAMOND, 10, 50, 1));
        datas.Add(new BattlePassCellData(2, boxBlue, BattlePassRewardType.DIAMOND, 30, 150, 1));
        datas.Add(new BattlePassCellData(3, boxYellow, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 10, 50, 2));
        datas.Add(new BattlePassCellData(4, boxBlue, BattlePassRewardType.DIAMOND, 30, 150, 3));
        datas.Add(new BattlePassCellData(5, boxYellow, BattlePassRewardType.PLANET_PUPPLE, 1, 3, 4));
        datas.Add(new BattlePassCellData(6, boxBlue, BattlePassRewardType.DIAMOND, 50, 250, 2));
        datas.Add(new BattlePassCellData(7, boxYellow, BattlePassRewardType.EFFECT_STONES_RANDOM, 1, 3, 3));
        datas.Add(new BattlePassCellData(8, boxPupple, BattlePassRewardType.AUTO_RESTORE_PLANET, 10, 30, 4));
        datas.Add(new BattlePassCellData(9, boxYellow, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 20, 60, 4));

        datas.Add(new BattlePassCellData(10, boxRed, BattlePassRewardType.PLANET_YELLOW, 1, 3, 5));
        datas.Add(new BattlePassCellData(11, boxBlue, BattlePassRewardType.DIAMOND, 150, 450, 2));
        datas.Add(new BattlePassCellData(12, boxYellow, BattlePassRewardType.EFFECT_STONES_RANDOM, 5, 15, 2));
        datas.Add(new BattlePassCellData(13, boxBlue, BattlePassRewardType.DIAMOND, 150, 450, 3));
        datas.Add(new BattlePassCellData(14, boxYellow, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 30, 90, 4));
        datas.Add(new BattlePassCellData(15, boxRed, BattlePassRewardType.PLANET_RANDOM, 10, 30, 5));
        datas.Add(new BattlePassCellData(16, boxBlue, BattlePassRewardType.DIAMOND, 200, 600, 2));
        datas.Add(new BattlePassCellData(17, boxBlue, BattlePassRewardType.AUTO_RESTORE_PLANET, 15, 45, 3));
        datas.Add(new BattlePassCellData(18, boxYellow, BattlePassRewardType.EFFECT_STONES_RANDOM, 10, 30, 5));
        datas.Add(new BattlePassCellData(19, boxBlue, BattlePassRewardType.DIAMOND, 500, 1500, 5));

        datas.Add(new BattlePassCellData(20, boxRed, BattlePassRewardType.PLANET_ORANGE, 1, 3, 5));
        datas.Add(new BattlePassCellData(21, boxRed, BattlePassRewardType.PLANET_RANDOM, 10, 30, 5));
        datas.Add(new BattlePassCellData(22, boxBlue, BattlePassRewardType.DIAMOND, 200, 600, 3));
        datas.Add(new BattlePassCellData(23, boxPupple, BattlePassRewardType.AUTO_RESTORE_PLANET, 15, 45, 4));
        datas.Add(new BattlePassCellData(24, boxYellow, BattlePassRewardType.EFFECT_STONES_RANDOM, 10, 30, 4));
        datas.Add(new BattlePassCellData(25, boxBlue, BattlePassRewardType.DIAMOND, 500, 1500, 5));
        datas.Add(new BattlePassCellData(26, boxRed, BattlePassRewardType.PLANET_ORANGE, 1, 3, 5));
        datas.Add(new BattlePassCellData(27, boxRed, BattlePassRewardType.PLANET_RANDOM, 10, 30, 5));
        datas.Add(new BattlePassCellData(28, boxBlue, BattlePassRewardType.DIAMOND, 200, 600, 3));
        datas.Add(new BattlePassCellData(29, boxPupple, BattlePassRewardType.AUTO_RESTORE_PLANET, 15, 45, 4));

        datas.Add(new BattlePassCellData(30, boxYellow, BattlePassRewardType.EFFECT_STONES_RANDOM, 10, 30, 4));
        datas.Add(new BattlePassCellData(31, boxBlue, BattlePassRewardType.DIAMOND, 500, 1500, 5));
        datas.Add(new BattlePassCellData(32, boxRed, BattlePassRewardType.PLANET_ORANGE, 1, 3, 5));
        datas.Add(new BattlePassCellData(33, boxRed, BattlePassRewardType.PLANET_RANDOM, 10, 30, 5));
        datas.Add(new BattlePassCellData(34, boxBlue, BattlePassRewardType.DIAMOND, 200, 600, 3));
        datas.Add(new BattlePassCellData(35, boxPupple, BattlePassRewardType.AUTO_RESTORE_PLANET, 15, 45, 4));
        datas.Add(new BattlePassCellData(36, boxYellow, BattlePassRewardType.EFFECT_STONES_RANDOM, 10, 30, 4));
        datas.Add(new BattlePassCellData(37, boxBlue, BattlePassRewardType.DIAMOND, 500, 1500, 5));
        datas.Add(new BattlePassCellData(38, boxRed, BattlePassRewardType.PLANET_ORANGE, 1, 3, 5));
        datas.Add(new BattlePassCellData(39, boxRed, BattlePassRewardType.PLANET_RANDOM, 10, 30, 5));

        datas.Add(new BattlePassCellData(40, boxBlue, BattlePassRewardType.DIAMOND, 200, 600, 3));
        datas.Add(new BattlePassCellData(41, boxPupple, BattlePassRewardType.AUTO_RESTORE_PLANET, 15, 45, 4));
        datas.Add(new BattlePassCellData(42, boxYellow, BattlePassRewardType.EFFECT_STONES_RANDOM, 10, 30, 4));
        datas.Add(new BattlePassCellData(43, boxBlue, BattlePassRewardType.DIAMOND, 500, 1500, 5));
        datas.Add(new BattlePassCellData(44, boxRed, BattlePassRewardType.PLANET_ORANGE, 1, 3, 5));
        datas.Add(new BattlePassCellData(45, boxRed, BattlePassRewardType.PLANET_RANDOM, 10, 30, 5));
        datas.Add(new BattlePassCellData(46, boxBlue, BattlePassRewardType.DIAMOND, 200, 600, 3));
        datas.Add(new BattlePassCellData(47, boxPupple, BattlePassRewardType.AUTO_RESTORE_PLANET, 15, 45, 4));
        datas.Add(new BattlePassCellData(48, boxYellow, BattlePassRewardType.EFFECT_STONES_RANDOM, 10, 30, 4));
        datas.Add(new BattlePassCellData(49, boxBlue, BattlePassRewardType.DIAMOND, 500, 1500, 5));
        datas.Add(new BattlePassCellData(50, boxRed, BattlePassRewardType.PLANET_ORANGE, 1, 3, 5));

        for (int i = 0; i < datas.Count; i++) {
            GameObject go = Instantiate(cellPrefab, content);
            cells.Add(go.GetComponent<BattlePassCell>());
            cells[i].LoadData(datas[i]);
            cells[i].SetCollectableProgress(winCount > i, normal[i], battle[i]);
            cells[i].index = i;
        }

        LEVEL_GROUP_WIDTH = cellPrefab.GetComponent<RectTransform>().sizeDelta.x;

        content.GetComponent<RectTransform>().sizeDelta = new Vector2(LEVEL_GROUP_WIDTH * 50, content.GetComponent<RectTransform>().sizeDelta.y);
        content.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(winCount - 1) * LEVEL_GROUP_WIDTH, 0);

        int index = winCount >= datas.Count ? datas.Count : winCount;
        if (index >= 1) {
            bool rewardEffect = normal[index - 1] == 0 || battle[index - 1] == 0 && (DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_VIP_ENABLED) == "1");

            if (rewardEffect) {
                PopupConfirm.ShowOK(TextMan.Get("Information"), TextMan.Get("New reward unlocked!"));
            }
        }
    }

    public void OpenBattlePassGamePlay() {
        if (DataGameSave.IsAllPlanetDestroyed()) {
            PopupConfirm.ShowOK(TextConstants.NO_PLANET, TextConstants.NO_PLANET_MESSAGE);
            return;
        }

        if (DataGameSave.battlePassEnemy == null) {
            DataGameSave.GetBattlePassEnemy();
            int index = UnityEngine.Random.Range(0, DataGameSave.listDataEnemies.Count);
            DataGameSave.dataEnemy = DataGameSave.listDataEnemies[index];
        } else {
            DataGameSave.dataEnemy = DataGameSave.battlePassEnemy;
        }

        DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.Battle].currentProgress++;

        if (BtnDailyMission.Instance) {
            BtnDailyMission.Instance.CheckDoneQuest();
        }

        int progress;
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_WIN_COUNT), out progress);
        if (progress == 0) {
            progress = 1;
        }
        progress = progress > datas.Count ? datas.Count : progress;
        BattlePassGameplay.data = datas[progress - 1];

        Scenes.LastScene = SceneName.BattlePass;
        Scenes.ChangeScene(SceneName.BattlePassGameplay);
    }

    public void BackToGameplay() {
        Scenes.ChangeScene(SceneName.Gameplay);
    }

    public void ReceivePrize(int index, bool isBattlePassPrize = false) {
        if (isBattlePassPrize && DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_VIP_ENABLED) != "1") {
            BuyVipBattlePass();
            return;
        }

        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_WIN_COUNT), out winCount);

        // get saved data
        List<int> normal = DataGameSave.GetMetaDataObject<List<int>>(MetaDataKey.BATTLE_PASS_PRIZE_RECEIVED_NORMAL);
        List<int> battle = DataGameSave.GetMetaDataObject<List<int>>(MetaDataKey.BATTLE_PASS_PRIZE_RECEIVED_BATTLE_PASS);

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

        BattlePassCellData data = datas[index];
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
        DataGameSave.SaveMetaData(MetaDataKey.BATTLE_PASS_PRIZE_RECEIVED_NORMAL, JsonConvert.SerializeObject(normal));
        DataGameSave.SaveMetaData(MetaDataKey.BATTLE_PASS_PRIZE_RECEIVED_BATTLE_PASS, JsonConvert.SerializeObject(battle));
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