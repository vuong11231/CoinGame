using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Hellmade.Sound;
using SteveRogers;

public class PopupSkinPlanet : MonoBehaviour
{
    public Image mainSkin;
    public Text size_BonusPercent, damage_BonusPercent, hp_BonusPercent, speed_BonusPercent, materialFarm_BonusPercent;
    private int cacheIndexSelected = 0;
    public List<SkinCell> listScriptPlanetCell = new List<SkinCell>();
    public GameObject prefabsPlanetCell;
    public GameObject holderPlanetCell;

    private void Start()
    {
        //ReadDataPlanetCell();
        //InitPlanetCell();
    }

    #region ReadDataPlanetCell

    public List<OneSkinCell> listSkinCells = new List<OneSkinCell>();
    public struct OneSkinCell
    {
        public int indexThisPlanetCell;
        public string namePlanet;
        public int size_BonusPercent_with_SkinType, damage_BonusPercent_with_SkinType, hp_BonusPercent_with_SkinType, speed_BonusPercent_with_SkinType, materialFarm_BonusPercent_with_SkinType, rare;
        public bool trail, effect;
    }

    public void ReadDataPlanetCell()
    {
        DataGameSave.dataServer.CheckInitSkinPieceAndLevel();
        if (listScriptPlanetCell.Count>0)
        {
            foreach (var skinCell in listScriptPlanetCell)
            {
                skinCell.UpdateStatus();
            }
            return;
        }

        var content = Utilities.ReadAllText_Resources("DataAvatar");
        var dic = Utilities.CreateDictionaryFromCSVContent(ref content);
        listSkinCells.Clear();
        foreach (var row in dic)
        {
            if (row.Key.ToLower().Contains("planet"))
            {
                //Minh.ho: Delete first row
                continue;   
            }
        
            OneSkinCell oneSkinCell = new OneSkinCell();
            
            oneSkinCell.namePlanet = row.Key;
            int.TryParse(row.Value[0], out oneSkinCell.size_BonusPercent_with_SkinType);
            int.TryParse(row.Value[1], out oneSkinCell.damage_BonusPercent_with_SkinType);
            int.TryParse(row.Value[2], out oneSkinCell.hp_BonusPercent_with_SkinType);
            int.TryParse(row.Value[3], out oneSkinCell.speed_BonusPercent_with_SkinType);
            int.TryParse(row.Value[4], out oneSkinCell.materialFarm_BonusPercent_with_SkinType);
            oneSkinCell.trail = row.Value[5].Contains("TRUE");
            oneSkinCell.effect = row.Value[6].Contains("TRUE");
            int.TryParse(row.Value[7], out oneSkinCell.rare);
            listSkinCells.Add(oneSkinCell);

            //Debug.LogError("index: "+ oneQuest.index + "- type: " + oneQuest.type + "- amount: " + oneQuest.amount + "- diamond: "+ oneQuest.rewardDiamond +"\n");
        }
        InitSkinCell();
    }
    #endregion



    public void InitSkinCell()
    {
        listScriptPlanetCell.Clear();
        if (listSkinCells.Count > 0)
        {
            for (int i = 0; i < listSkinCells.Count; i++)
            {
                GameObject onePlanetCell = Instantiate(prefabsPlanetCell, holderPlanetCell.transform);
                onePlanetCell.SetActive(true);
                var scriptOnePlanetCell = onePlanetCell.GetComponent<SkinCell>();
                scriptOnePlanetCell.SetData(i,listSkinCells[i], UpdateStatitics);
                listScriptPlanetCell.Add(scriptOnePlanetCell);
            }
        }
        UpdateStatitics(cacheIndexSelected, DataGameSave.dataServer.GetBonusPercent_At_LevelSkin((DataGameSave.skinLevels[cacheIndexSelected])));
    }

    public void UpdateStatitics(int index,int bonusPercent)
    {
        cacheIndexSelected = index;
        mainSkin.sprite = GameManager.Instance.listSkin[index];

        int damageIndex = (int)(listSkinCells[index].damage_BonusPercent_with_SkinType + listSkinCells[index].damage_BonusPercent_with_SkinType * bonusPercent * 0.01f);
        damage_BonusPercent.text = "+" + damageIndex.ToString() + "%";

        int sizeIndex = (int)(listSkinCells[index].size_BonusPercent_with_SkinType + listSkinCells[index].size_BonusPercent_with_SkinType * bonusPercent * 0.01f);
        size_BonusPercent.text = "+" + sizeIndex.ToString() + "%";

        int hpIndex = (int)(listSkinCells[index].hp_BonusPercent_with_SkinType + listSkinCells[index].hp_BonusPercent_with_SkinType * bonusPercent * 0.01f);
        hp_BonusPercent.text = "+" + hpIndex.ToString() + "%";

        int speedIndex = (int)(listSkinCells[index].speed_BonusPercent_with_SkinType + listSkinCells[index].speed_BonusPercent_with_SkinType * bonusPercent * 0.01f);
        speed_BonusPercent.text = "+" + speedIndex.ToString() + "%";

        materialFarm_BonusPercent.text = "+" + listSkinCells[index].materialFarm_BonusPercent_with_SkinType.ToString() + "/s";
        foreach (SkinCell item in listScriptPlanetCell)
        {
            if (item.indexThisSkinCell == index)
            {
                item.tick.SetActive(true);
            }
            else
            {
                item.tick.SetActive(false);
            }
        }

        //.....//
    }

}

    
