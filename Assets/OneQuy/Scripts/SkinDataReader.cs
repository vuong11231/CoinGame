using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SteveRogers;
using Random = UnityEngine.Random;
using Newtonsoft.Json;

public static class SkinDataReader
{
    public static SkinData[] datas = null;
    public static Dictionary<int, List<PLanetName>> listRare;
    public static List<PLanetName> listRare_1 = new List<PLanetName>();
    public static List<PLanetName> listRare_2 = new List<PLanetName>();
    public static List<PLanetName> listRare_3 = new List<PLanetName>();
    public static List<PLanetName> listRare_4 = new List<PLanetName>();
    //tim vang cam

    public static void CheckAndLoad()
    {
        if (datas != null)
            return;

        var c = Utilities.ReadAllText_Resources("DataAvatar");
        var dic = Utilities.CreateDictionaryFromCSVContent(ref c);
        datas = new SkinData[(int)PLanetName._Count];

        for (PLanetName i = 0; i < PLanetName._Count; i++)
        {
            datas[(int)i] = new SkinData
            {
                sizePercent = Utilities.GetValueFromCSVDictionary(dic, i.ToString().ToUpper(), -1f, 0),
                damePercent = Utilities.GetValueFromCSVDictionary(dic, i.ToString().ToUpper(), -1f, 1),
                hpPercent = Utilities.GetValueFromCSVDictionary(dic, i.ToString().ToUpper(), -1f, 2),
                speedPercent = Utilities.GetValueFromCSVDictionary(dic, i.ToString().ToUpper(), -1f, 3),
                matPerSecondAddition = (int)Utilities.GetValueFromCSVDictionary(dic, i.ToString().ToUpper(), -1, 4),
                hasTrail = Utilities.GetValueFromCSVDictionary(dic, i.ToString().ToUpper(), "", 5).ToString() == "TRUE",
                hasEffect = Utilities.GetValueFromCSVDictionary(dic, i.ToString().ToUpper(), "", 6).ToString() == "TRUE",
                rareCode = (int)Utilities.GetValueFromCSVDictionary(dic, i.ToString().ToUpper(), 0f, 7),
            };

            var check = datas[(int)i];
            
            if (check.sizePercent < 0)
                Debug.LogError("size fail: " + i);
            
            if (check.damePercent < 0)
                Debug.LogError("dame fail: " + i);
            
            if (check.hpPercent < 0)
                Debug.LogError("hp fail: " + i);

            if (check.speedPercent < 0)
                Debug.LogError("speed fail: " + i);

            if (check.matPerSecondAddition < 0)
                Debug.LogError("mat fail: " + i);
        }
    }

    public static SkinData Get(PLanetName name)
    {
        CheckAndLoad();
        return datas[(int)name];
    }

    public static void InitRareList() {
        listRare_1 = new List<PLanetName>();
        listRare_2 = new List<PLanetName>();
        listRare_3 = new List<PLanetName>();
        listRare_4 = new List<PLanetName>();
        listRare = new Dictionary<int, List<PLanetName>>();

        for (PLanetName i = 0; i < PLanetName._Count; i++) {
            if (SkinDataReader.Get(i).rareCode == 1)
                listRare_1.Add(i);

            else if (SkinDataReader.Get(i).rareCode == 2)
                listRare_2.Add(i);

            else if (SkinDataReader.Get(i).rareCode == 3)
                listRare_3.Add(i);

            else if (SkinDataReader.Get(i).rareCode == 4)
                listRare_4.Add(i);
        }

        listRare.Add(1,listRare_1);
        listRare.Add(2,listRare_2);
        listRare.Add(3,listRare_3);
        listRare.Add(4,listRare_4);
    }

    public static Dictionary<PLanetName, int> GetRandomSkinPlanet(int amount, int rare = -1) {
        InitRareList();

        if (rare != -1) {
            if (rare <= 0 || rare > 4) {
                return null;
            }
            var list = listRare[rare];
            var dic = new Dictionary<PLanetName, int>();
            dic.Add(list[Random.Range(0, list.Count)], amount);
            return dic;
        }

        SkinDataReader.InitRareList();
        for (int i = 1; i <= 4; i++) {
            if (SkinDataReader.listRare[i].IsValid()) {
                SkinDataReader.listRare[i].ShuffleList();
            }
        }

        Dictionary<PLanetName, int> res = new Dictionary<PLanetName, int>();

        for (int i = 0; i < amount; i++) {
            var rand = UnityEngine.Random.Range(1, 101);
            PLanetName pick;

            if (rand < 75) {
                pick = SkinDataReader.listRare_1[UnityEngine.Random.Range(0, SkinDataReader.listRare_1.Count)];
            } else if (rand < 95) {
                pick = SkinDataReader.listRare_2[UnityEngine.Random.Range(0, SkinDataReader.listRare_2.Count)];
            } else if (rand < 100) {
                pick = SkinDataReader.listRare_3[UnityEngine.Random.Range(0, SkinDataReader.listRare_3.Count)];
            } else {
                pick = SkinDataReader.listRare_4[UnityEngine.Random.Range(0, SkinDataReader.listRare_4.Count)];
            }

            if (res.ContainsKey(pick))
                res[pick]++;
            else
                res.Add(pick, 1);
        }

        return res;
    }

    /// <summary>
    /// backup current skins and levels as old data, then send to server try to save data
    /// if success then minus diamond, else give the error and rollback data
    /// </summary>
    public static void TryBuyRandomSkinPlanet(int skinAmount, int diamondPrice, int rare = -1) {
        if (DataGameSave.dataLocal.Diamond < diamondPrice) {
            return;
        }

        InitRareList();
        Dictionary<PLanetName, int> res = SkinDataReader.GetRandomSkinPlanet(skinAmount, rare);

        List<int> oldLevels = DataGameSave.skinLevels;
        List<int> oldPieces = DataGameSave.skinPieces;
        Dictionary<string, string> oldMetaData = DataGameSave.metaData;

        foreach (var i in res) {
            var iname = (int)i.Key;
            DataGameSave.skinPieces[iname]++;
        }

        DataGameSave.SaveMetaData(MetaDataKey.SKIN_PIECE, JsonConvert.SerializeObject(DataGameSave.skinPieces));
        DataGameSave.SaveMetaData(MetaDataKey.SKIN_LEVEL, JsonConvert.SerializeObject(DataGameSave.skinLevels));
        DataGameSave.dataLocal.Diamond -= diamondPrice;

        DataGameSave.SaveToServer(() => {
            Debug.Log(ServerSystem.result);

            if (ServerSystem.Instance.IsResponseOK()) {
                PopupBattleResult2.Show(dic: res, okFunction: () => {
                    if (Scenes.Current != SceneName.BattlePass) {
                        foreach (var skinCell in UIMultiScreenCanvasMan.Instance.popupUpgrade.skinPlanetManager.listScriptPlanetCell)
                        {
                            skinCell.UpdateStatus();
                        }
                        foreach (var skinCellMini in UIMultiScreenCanvasMan.Instance.popupUpgrade.listScriptMiniSkinCell)
                        {
                            skinCellMini.CheckUnlock();
                        }
                    }
                });
            } else {
                //rollback data
                DataGameSave.skinLevels = oldLevels;
                DataGameSave.skinPieces = oldPieces;
                DataGameSave.metaData = oldMetaData;
                DataGameSave.dataLocal.Diamond += diamondPrice;

                PopupConfirm.ShowOK("Oops", TextMan.Get("Something wrong happened, please try again"));
            }
        }, () => {
            //rollback data
            DataGameSave.skinLevels = oldLevels;
            DataGameSave.skinPieces = oldPieces;
            DataGameSave.metaData = oldMetaData;
            DataGameSave.dataLocal.Diamond += diamondPrice;

            PopupConfirm.ShowOK("Oops", TextMan.Get("Something wrong happened, please try again"));
        });
    }
}

[Serializable]
public class SkinData
{
    public float sizePercent;
    public float damePercent;
    public float hpPercent;
    public float speedPercent;
    public int matPerSecondAddition;
    public bool hasTrail;
    public bool hasEffect;
    public int rareCode;
}