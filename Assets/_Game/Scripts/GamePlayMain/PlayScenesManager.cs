using Lean.Pool;
using Newtonsoft.Json;
using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayScenesManager : Singleton<PlayScenesManager> {
    public static int maxLevelCondition = 1;
    public static int int_AmountButtonDiamond;

    public Transform[] processItems = null;

    public GameObject BackTut;
    public GameObject HandTut;
    public GameObject prefabCollect;

    public GameObject txtQuestBattle;
    public GameObject txtQuestMeteor;

    public GameObject[] redDotUpgrades = null;
    public GameObject redDotMail;
    public GameObject redDotMeteorFallEvent;

    public Text namePlayerTxt = null;

    public Text upgradeTxt = null;
    public Image upgradeBtnImg = null;

    public GameObject autoRestoreObj;
    public GameObject buttonClaim;
    public GameObject buttonRestore;
    public GameObject buttonMailDev;
    public GameObject redDot_buttonMailDev;
    public GameObject x2Material;

    public Text text_AmountButtonDiamond;
    public Text txtTotalMaterialCollect;
    public Text autoRestoreCountDown;

    public bool pausingSpins = false;

    private IEnumerator Start() {
        if (MenuItem_ClearCS.IsTrue(false))
            Utilities.ClearConsole();

        OnShowAttackInfo();
        UpdateNameText();
        CheckRestoreShopWatchAdsCount();

        redDotMail.SetActive(false);
        GameManager.planetSpeedRatio = 1f;
        GameManager.takeNoDame = false;
        GameStatics.IsAnimating = false;
        PopupCustom.showing = "";

        txtQuestBattle.CheckAndSetActive(false);
        txtQuestMeteor.CheckAndSetActive(false);
        LeanTween.alphaCanvas(txtQuestBattle.GetComponent<CanvasGroup>(), 0, 1f).setLoopPingPong();
        LeanTween.alphaCanvas(txtQuestMeteor.GetComponent<CanvasGroup>(), 0, 1f).setLoopPingPong();

        yield return new WaitForSeconds(1);

        ReadDataQuest();

        DataGameSave.GetRandomEnemy();
        DataGameSave.GetBossData();
        DataGameSave.GetTopRank();
        DataGameSave.GetRankChartData();
        DataGameSave.GetBattlePassEnemy();
        DataGameSave.GetListNeightborEnemy();

        Debug.Log(JsonConvert.SerializeObject(DataGameSave.dataServer));

        if (!PlayerPrefs.HasKey(PlayerPrefsKey.METEOR_FALL_EVENT_ENABLED))
        {
            PlayerPrefs.SetString(PlayerPrefsKey.METEOR_FALL_EVENT_ENABLED, "true");
        }

        while (Scenes.Current == SceneName.Gameplay) {
            yield return Utilities.WAIT_FOR_ONE_SECOND;

            CheckShowAutoRestore();
            CheckShowRewardAllSkin();
            UpdateTotalCollect();
            CheckRedDotMeteorFallEvent();

            var canUp = SpaceManager.Instance.CheckCanUpgrade();

            foreach (var i in redDotUpgrades)
                i.SetActive(canUp);

            redDotMail.SetActive(false);

            List<string> mailReads = DataGameSave.GetMailReads();

            foreach (var item in DataGameSave.mails) {
                bool canReceiveMail = true;
                if (item.targetid != null && item.targetid != "") {
                    string[] targets = item.targetid.Split(',');
                    canReceiveMail = new List<string>(targets).Contains(DataGameSave.dataServer.userid.ToString());
                }
                if (canReceiveMail && !mailReads.Contains(item.id)) {
                    redDotMail.SetActive(true);
                    break;
                }
            }
        }
    }

    public void CheckRedDotMeteorFallEvent() {
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_WIN_MAX_COUNT), out int maxWin);
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_WIN_1), out int win1);
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_WIN_2), out int win2);
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_WIN_3), out int win3);
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_WIN_4), out int win4);

        bool newReward = maxWin >= 1 && win1 != 1 || maxWin >= 2 && win1 != 1 || maxWin >= 3 && win1 != 1 || maxWin >= 4 && win1 != 1;
        redDotMeteorFallEvent.SetActive(newReward);
    }

    public void CheckShowRewardAllSkin() {
        if (DataGameSave.remoteConfig["full_skin"] == "true" && 
            DataGameSave.dataServer.level >= 10 && 
            DataGameSave.GetMetaData(MetaDataKey.FULL_SKIN_RECEIVED) != "true") {

            DataGameSave.SaveMetaData(MetaDataKey.FULL_SKIN_RECEIVED, "true");
            DataGameSave.SaveToServer(() => {
                for (int i = 0; i < DataGameSave.skinPieces.Count; i++) {
                    DataGameSave.skinPieces[i]++;
                    foreach (var skinCell in UIMultiScreenCanvasMan.Instance.popupUpgrade.skinPlanetManager.listScriptPlanetCell) {
                        skinCell.UpdateStatus();
                    }
                    foreach (var skinCellMini in UIMultiScreenCanvasMan.Instance.popupUpgrade.listScriptMiniSkinCell) {
                        skinCellMini.CheckUnlock();
                    }
                }

                PopupConfirm.ShowOK(TextMan.Get("ReceiveGift"), TextMan.Get("All skin unlocked"));
            }, ()=> {
                DataGameSave.SaveMetaData(MetaDataKey.FULL_SKIN_RECEIVED, "false");

                for (int i = 0; i < DataGameSave.skinPieces.Count; i++) {
                    DataGameSave.skinPieces[i]--;
                    foreach (var skinCell in UIMultiScreenCanvasMan.Instance.popupUpgrade.skinPlanetManager.listScriptPlanetCell) {
                        skinCell.UpdateStatus();
                    }
                    foreach (var skinCellMini in UIMultiScreenCanvasMan.Instance.popupUpgrade.listScriptMiniSkinCell) {
                        skinCellMini.CheckUnlock();
                    }
                }
            });
        }
    }

    public void CheckShowAutoRestore() {
        //if (!GameManager.IsAutoRestorePlanet) {
        //    autoRestoreObj.SetActive(false);
        //    return;
        //}

        //autoRestoreObj.SetActive(true);

        DateTime endTime = new DateTime(DataGameSave.autoRestoreEndTime);

        double second = (endTime - GameManager.Now).TotalSeconds;

        if (second <= 0) {
            autoRestoreCountDown.gameObject.SetActive(false);
            return;
        }else {
            autoRestoreCountDown.gameObject.SetActive(true);
        }

        TimeSpan t = TimeSpan.FromSeconds(second);

        autoRestoreCountDown.text = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                t.Hours,
                t.Minutes,
                t.Seconds);
    }

    int twinkleCount = -1;
    public Sprite[] btnTwinkleSprites = null;

    public void UpdateNameText() {
        namePlayerTxt.text = DataGameSave.dataServer.Name;
    }

    public void CheckRestoreShopWatchAdsCount() {
        string daycodeAutoRestore = DataGameSave.GetMetaData(MetaDataKey.SHOP_AUTO_RESTORE_DAYCODE);
        string daycodeDiamond = DataGameSave.GetMetaData(MetaDataKey.SHOP_GET_DIAMOND_DAYCODE);
        string daycodeTotalAttack = DataGameSave.GetMetaData(MetaDataKey.SHOP_TOTAL_ATTACK_DAYCODE);
        string nowDaycode = DataGameSave.GetDayCode(GameManager.Now);

        // there is one free diamond daily in shop
        if (daycodeDiamond != nowDaycode) {
            DataGameSave.SaveMetaData(MetaDataKey.SHOP_GET_DIAMOND_DAYCODE, nowDaycode);
            DataGameSave.SaveMetaData(MetaDataKey.SHOP_GET_DIAMOND_COUNT, (GameManager.MAX_WATCH_ADS_PER_DAY_LIMIT+1).ToString());
        }

        if (daycodeAutoRestore != nowDaycode) {
            DataGameSave.SaveMetaData(MetaDataKey.SHOP_AUTO_RESTORE_DAYCODE, nowDaycode);
            DataGameSave.SaveMetaData(MetaDataKey.SHOP_AUTO_RESTORE_COUNT, GameManager.MAX_WATCH_ADS_PER_DAY_LIMIT.ToString());
        }

        if (daycodeTotalAttack != nowDaycode) {
            DataGameSave.SaveMetaData(MetaDataKey.SHOP_TOTAL_ATTACK_DAYCODE, nowDaycode);
            DataGameSave.SaveMetaData(MetaDataKey.SHOP_TOTAL_ATTACK_COUNT, GameManager.MAX_WATCH_ADS_PER_DAY_LIMIT.ToString());
        }
    }

    public string GetUpgradeSunTitle(int sunLevel) {
        if (sunLevel == 1) {
            return TextMan.Get("Upgrade max levels of your Planet!");
        } else if (sunLevel == 2) {
            return TextMan.Get("Destroy 10 Planets and 5 Suns");
        } else if (sunLevel == 3) {
            return TextMan.Get("Destroy 100 Meteors");
        } else if (sunLevel == 4) {
            return TextMan.Get("Destroy 25 Special Meteors");
        } else if (sunLevel == 5) {
            return TextMan.Get("Destroy 50 Suns").Replace("50", "30");
        } else if (sunLevel == 6) {
            return TextMan.Get("Collect 10 Antimatters");
        } else if (sunLevel == 7) // ?
          {
            return TextMan.Get("Destroy 5000 Meteors").Replace("5000", "1000");
        } else if (sunLevel == 8) {
            return TextMan.Get("Hit multi-color Meteors 50 times");
        } else if (sunLevel == 9) {
            return TextMan.Get("Destroy 1000 Planets and 100 Suns");
        } else if (sunLevel == 10) {
            return TextMan.Get("You reached Max Level!");
        } else {
            return "...";
        }
    }

    public void OnPressed_UpgradeSun() {
    }

    public void OnPressed_LoginFB() {
    }

    private void Update() {
        if (twinkleCount > -1) {
            if (twinkleCount % 5 == 0) {
                if (upgradeBtnImg.sprite == btnTwinkleSprites[0])
                    upgradeBtnImg.sprite = btnTwinkleSprites[1];
                else
                    upgradeBtnImg.sprite = btnTwinkleSprites[0];
            }

            twinkleCount++;

            if (twinkleCount > 35)
                twinkleCount = -1;
        }

        int maxLevelCondition = CheckMaxQuestLevelCompleted();

        if (GameManager.LEVEL_AUTO_CORRECTION && 
            DataGameSave.dataServer.level > maxLevelCondition &&
            maxLevelCondition != 1) {

            DataGameSave.dataServer.level = maxLevelCondition;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        buttonClaim.SetActive(DataGameSave.dataServer.level < maxLevelCondition);
        if (buttonClaim.activeSelf && !buttonClaim.GetComponent<ParticleSystem>().isPlaying) {
            PlayScenesManager.Instance.buttonClaim.GetComponent<ParticleSystem>()?.Play();
            //buttonClaim.GetComponent<ParticleSystem>().Play();
        }

        UpdateQuestDisplay();

        if (DataGameSave.GetMetaData(MetaDataKey.GIFT_LETTER) == "true" && DataGameSave.dataServer.level >= 5) {
            redDot_buttonMailDev.SetActive(false);
        } else {
            redDot_buttonMailDev.SetActive(true);
        }
    }

    public void OnMailBoxButtonClick() {
        PopupMailbox.Show();
    }

    public void OnShowAttackInfo(bool skipWhenEmpty = true) {
        var listAttackedInfo = DataGameSave.dataServer?.GetAttackedInfo();

        if (skipWhenEmpty && !listAttackedInfo.IsValid()) {
            return;
        }

        if (listAttackedInfo == null) {
            listAttackedInfo = new List<AttackedInfoData>();
        }

        PopupAttackedInfo.Show(listAttackedInfo);

        if (!DataScriptableObject.Instance.notClearAttackedInfo) {
            DataGameSave.dataServer.isAttackedCode = null;
            DataGameSave.SaveToServer();
        }
    }

    public void OnRatingButtonClick() {
        PopupRating.Show();
    }

    public void OnEventButtonClick() {
        //PopupCustom.Show("Prefabs/Pop-ups/Custom/Popup Event");
        PopupCustom.Show("Prefabs/Pop-ups/Custom/Popup Meteor Fall Event");
    }

    public void OnFinishQuest() {
        if (DataGameSave.dataServer.level < maxLevelCondition) {
            PopupUpgradeSun.OnUpGradeMain(maxLevelCondition);
            DataGameSave.dataLocal.Diamond += int_AmountButtonDiamond;
            DataGameSave.SaveToServer();
        }
    }

    private static List<OneQuest> listQuest = new List<OneQuest>();

    public struct OneQuest {
        public int index;
        public int type;
        public int amount;
        public int rewardDiamond;
    }

    private void ReadDataQuest() {
        var content = Utilities.ReadAllText_Resources("DataQuest");
        var dic = Utilities.CreateDictionaryFromCSVContent(ref content);
        listQuest.Clear();
        foreach (var row in dic) {
            OneQuest oneQuest = new OneQuest();
            int.TryParse(row.Key, out oneQuest.index);
            int.TryParse(row.Value[1], out oneQuest.type);
            int.TryParse(row.Value[2], out oneQuest.amount);
            int.TryParse(row.Value[3], out oneQuest.rewardDiamond);
            listQuest.Add(oneQuest);
        }
    }

    /// <summary>
    /// return int: maxLevelCondition, indicate the max level that fit the condition
    /// </summary>
    /// <param name="processItems"></param>
    /// <param name="amountDiamond"></param>
    /// <returns></returns>
    public int CheckMaxQuestLevelCompleted() {
        if (SpaceManager.Instance == null || SpaceManager.Instance.ListSpace.IsNullOrEmpty() || listQuest.IsNullOrEmpty()) {
            return maxLevelCondition;
        }

        //index map 1 1 với level
        //var sunLevel = SpaceManager.Instance.Level;
        var Planet = SpaceManager.Instance.ListSpace[0].Planet;

        for (int i = 0; i < listQuest.Count; i++) {
            bool checkCodition = false;

            switch (listQuest[i].type) {
                case 0:
                    checkCodition = true;
                    break;
                case 1:    //Upgrade sun
                    if (!Planet) {
                        checkCodition = false;
                        break;
                    }
                    checkCodition = (Planet.LevelSize > 9 && Planet.LevelSpeed > 9 && Planet.LevelHeavier > 9) || DataGameSave.dataServer.level > 1;
                    break;

                case 2:    //Destroy solar
                    int destroyedSolar = DataGameSave.dataLocal.destroyedSolars;
                    checkCodition = destroyedSolar >= listQuest[i].amount;
                    break;

                case 3:   //Destroy meteor
                    int meteorPlanetHitCount = DataGameSave.dataLocal.meteorPlanetHitCount;
                    checkCodition = meteorPlanetHitCount >= listQuest[i].amount;
                    break;

                case 4:   //Destroy planet
                    int destroyedPlanets = DataGameSave.dataLocal.DestroyPlanet;
                    checkCodition = destroyedPlanets >= listQuest[i].amount;
                    break;
            }
            if (!checkCodition) {
                break;
            } 
            else {
                maxLevelCondition = i + 1;
            }
        }

        return maxLevelCondition;
    }

    public void UpdateQuestDisplay() {
        try {
            if (listQuest == null || listQuest.Count == 0) {
                return;
            }

            OneQuest quest;

            if (DataGameSave.dataServer.level < listQuest.Count) {
                quest = listQuest[DataGameSave.dataServer.level];
            } else {
                quest = listQuest[listQuest.Count - 1];
            }

            txtQuestBattle.CheckAndSetActive(quest.type == 2 || quest.type == 4);
            txtQuestMeteor.CheckAndSetActive(quest.type == 3);

            var sunLevel = SpaceManager.Instance.Level;
            var Planet = SpaceManager.Instance.ListSpace[0].Planet;

            processItems[0].gameObject.SetActive(false);
            processItems[1].gameObject.SetActive(false);
            processItems[2].gameObject.SetActive(false);

            switch (quest.type) {
                case 0:
                    break;
                case 1:    //Upgrade sun
                    if (!Planet) {
                        break;
                    }
                    processItems[0].gameObject.SetActive(true);
                    processItems[1].gameObject.SetActive(true);
                    processItems[2].gameObject.SetActive(true);
                    SetProcess(processItems, quest.type, 0, Planet.LevelSize, 10, TextMan.Get("Upgrade max Size"));
                    SetProcess(processItems, quest.type, 1, Planet.LevelSpeed, 10, TextMan.Get("Upgrade max Speed"));
                    SetProcess(processItems, quest.type, 2, Planet.LevelHeavier, 10, TextMan.Get("Upgrade max Weight"));
                    break;

                case 2:    //Destroy solar
                    int destroyedSolar = DataGameSave.dataLocal.destroyedSolars;
                    processItems[0].gameObject.SetActive(true);
                    processItems[1].gameObject.SetActive(false);
                    processItems[2].gameObject.SetActive(false);
                    SetProcess(processItems, quest.type, 0, destroyedSolar, quest.amount, TextMan.Get("Destroy") + " " + quest.amount + " " + TextMan.Get("Solars"));
                    break;

                case 3:   //Destroy meteor
                    int meteorPlanetHitCount = DataGameSave.dataLocal.meteorPlanetHitCount;
                    processItems[0].gameObject.SetActive(true);
                    processItems[1].gameObject.SetActive(false);
                    processItems[2].gameObject.SetActive(false);
                    SetProcess(processItems, quest.type, 0, meteorPlanetHitCount, quest.amount, TextMan.Get("Destroy") + " " + quest.amount + " " + TextMan.Get("Meteors"));
                    break;

                case 4:   //Destroy planet
                    int destroyedPlanets = DataGameSave.dataLocal.DestroyPlanet;
                    processItems[0].gameObject.SetActive(true);
                    processItems[1].gameObject.SetActive(false);
                    processItems[2].gameObject.SetActive(false);
                    SetProcess(processItems, quest.type, 0, destroyedPlanets, quest.amount, TextMan.Get("Destroy") + " " + quest.amount + " " + TextMan.Get("Planets"));
                    break;
            }

            text_AmountButtonDiamond.text = quest.rewardDiamond.ToString();
            int_AmountButtonDiamond = quest.rewardDiamond;
        } catch { }
    }

    public static void SetProcess(Transform[] processItems, int typeQuest, int itemIdx, int number, int total, string title) {
        var tran = processItems[itemIdx];
        tran.GetChild(0).GetComponent<Text>().text = title;
        tran.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = number * 1f / total;
        tran.GetChild(1).GetChild(1).GetComponent<Text>().text = number.ToString() + "/" + total.ToString();
        if (itemIdx == 0) {
            tran.GetChild(1).GetChild(2).gameObject.SetActive(false);
            tran.GetChild(1).GetChild(3).gameObject.SetActive(false);
            tran.GetChild(1).GetChild(4).gameObject.SetActive(false);
            switch (typeQuest) {
                case 1:
                    break;
                case 2:
                    tran.GetChild(1).GetChild(3).gameObject.SetActive(true);   //Destroy solar
                    break;
                case 3:
                    tran.GetChild(1).GetChild(4).gameObject.SetActive(true);   //Destroy meteor
                    break;
                case 4:
                    tran.GetChild(1).GetChild(2).gameObject.SetActive(true);   //Destroy planet
                    break;
                default:
                    break;
            }
        }
    }

    //public IEnumerator TextQuestBlinking() {
    //    LeanTween.alphaText(txtQuestBattle.GetComponent<RectTransform>(), 0, 4f).setLoopPingPong();
    //    LeanTween.alphaText(txtQuestMeteor.GetComponent<RectTransform>(), 0, 4f).setLoopPingPong();
    //    //txtQuestBattle.CheckAndSetActive(quest.type == 2 || quest.type == 4);
    //    //txtQuestMeteor.CheckAndSetActive(quest.type == 3);
    //}

    public int GetTotalMaterial() {
        int total = 0;

        try {
            foreach (SpaceController space in SpaceManager.Instance.ListSpace) {
                if (space.Planet == null) { return -1; }
                total += space.Planet.MoneyCollect;
            }

            return total;
        } catch {
            return -1;
        }
    }

    public void UpdateTotalCollect() {
        int total = GetTotalMaterial();
        if (total != -1) {
            txtTotalMaterialCollect.text = Utilities.MoneyShorter(total, 1).ToString();
        }
    }

    public void CollectMaterialTotal() {
        int total = GetTotalMaterial();
        if (total == -1) {
            return;
        }

        foreach (SpaceController space in SpaceManager.Instance.ListSpace) {
            space.Planet.ResetCollect(false);
        }

        if (GameManager.Now.Ticks < DataGameSave.x2MaterialEndTime)
            total *= 2;

        DataGameSave.dataLocal.M_Material += total;
        DataGameSave.SaveToServer();
        MoneyManager.Instance.UpdateMoneyDisplay();
        Sounds.Instance.PlayCollect();

        StartCoroutine(MonoHelper.DoSomeThing(.1f, () =>
        {
            GameObject temp = LeanPool.Spawn(prefabCollect, txtTotalMaterialCollect.gameObject.transform.position, Quaternion.identity);
            temp.GetComponent<CollectMaterialFx>().SetFx(total, TypePlanet.Default);
        }));
    }

    public void CollectMaterialTotalX2() {
        int total = GetTotalMaterial();
        if (total == 0) {
            return;
        }

        GoogleMobileAdsManager.Instance.ShowRewardedVideo(() => {
            foreach (SpaceController space in SpaceManager.Instance.ListSpace) {
                space.Planet.ResetCollect(false);
            }

            if (GameManager.Now.Ticks < DataGameSave.x2MaterialEndTime)
                total *= 2;

            DataGameSave.dataLocal.M_Material += total * 2;
            DataGameSave.SaveToServer();
            MoneyManager.Instance.UpdateMoneyDisplay();
            Sounds.Instance.PlayCollect();

            StartCoroutine(MonoHelper.DoSomeThing(.1f, () =>
            {
                GameObject temp = LeanPool.Spawn(prefabCollect, txtTotalMaterialCollect.gameObject.transform.position, Quaternion.identity);
                temp.GetComponent<CollectMaterialFx>().SetFx(total*2, TypePlanet.Default);
            }));
        });
    }
}   
