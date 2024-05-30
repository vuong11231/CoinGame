using BayatGames.SaveGameFree.Encoders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class SkinCell : MonoBehaviour
{
    public int indexThisSkinCell;
    public Image skinImage;
    public Text namePlanet;
    private int size, damage, hp, speed, materialFarm;
    private bool trail, effect;
    public List<GameObject> backgroundRare = new List<GameObject>();

    public Text txt_pieceCollected;
    public Text txt_level;
    private int pieceCollected = 0;         //Minh.ho: can luu tren server vao du lieu nguoi choi
    private int level_of_Skin = 1;

    public bool isReadyForUpgrade = false;
    public Button buttonUpgrade;

    public Slider slider;
    public GameObject tick;
    private Action<int,int> onSelect;


    public void SetData(int index, PopupSkinPlanet.OneSkinCell onePlanetCell, Action<int,int> callback)
    {
        indexThisSkinCell = index;
        namePlanet.text = onePlanetCell.namePlanet.ToString();
        if(indexThisSkinCell< GameManager.Instance.listSkin.Count)
        {
            skinImage.sprite = GameManager.Instance.listSkin[indexThisSkinCell];
        }
        size = onePlanetCell.size_BonusPercent_with_SkinType;
        damage = onePlanetCell.damage_BonusPercent_with_SkinType;
        hp = onePlanetCell.hp_BonusPercent_with_SkinType;
        speed = onePlanetCell.speed_BonusPercent_with_SkinType;
        materialFarm = onePlanetCell.materialFarm_BonusPercent_with_SkinType;
        tick.SetActive(false);
        buttonUpgrade.gameObject.SetActive(isReadyForUpgrade);
        onSelect = callback;
        CheckRare(onePlanetCell.rare);
        UpdateStatus();
    }
    public void UpdateStatus()
    {
        CheckUnlock();
        CheckLevelAndPieces();
    }
    public void CheckRare(int rareIndex)
    {
        for (int i = 0; i < backgroundRare.Count; i++)
        {
            if(i+1 == rareIndex)
            {
                backgroundRare[i].SetActive(true);
            }
            else
            {
                backgroundRare[i].SetActive(false);
            }
        }
    }
    public void CheckLevelAndPieces()
    {
        pieceCollected = DataGameSave.skinPieces[indexThisSkinCell];
        level_of_Skin = DataGameSave.skinLevels[indexThisSkinCell];

        if(level_of_Skin >=10)
        {
            slider.gameObject.SetActive(false);
            txt_level.text = "Level " + level_of_Skin.ToString();
            return;
        }

        if(pieceCollected >= DataGameSave.dataServer.GetPieceNeedForNextLevel(level_of_Skin) 
           && !isReadyForUpgrade) 
        {
            isReadyForUpgrade = true;
            buttonUpgrade.gameObject.SetActive(true);
        }   

        //Minh.ho: Show text
        txt_pieceCollected.text = pieceCollected.ToString() + "/" + DataGameSave.dataServer.GetPieceNeedForNextLevel(level_of_Skin);
        slider.value = (float)pieceCollected / DataGameSave.dataServer.GetPieceNeedForNextLevel(level_of_Skin);
        txt_level.text = "Level " + level_of_Skin.ToString();
    }
    public void CheckUnlock()
    {
        int piece = DataGameSave.skinPieces[indexThisSkinCell];
        int level = DataGameSave.skinLevels[indexThisSkinCell];
        bool isUnlock_by_level = DataGameSave.dataServer.level > indexThisSkinCell;
        if (piece > 0 || level > 1 || isUnlock_by_level) 
        {
            this.GetComponent<Button>().interactable = true;
            this.GetComponent<CanvasGroup>().alpha = 1f;
        }
        else
        {
            this.GetComponent<Button>().interactable = false;
            this.GetComponent<CanvasGroup>().alpha = 0.4f;
        }
    }
    public void OnSelected()
    {
        onSelect?.Invoke(indexThisSkinCell, DataGameSave.dataServer.GetBonusPercent_At_LevelSkin(level_of_Skin));
    }
    public void UpdateLevelOfPlanetCell()
    {
        isReadyForUpgrade = false;
        buttonUpgrade.gameObject.SetActive(false);
        DataGameSave.skinPieces[indexThisSkinCell] -= DataGameSave.dataServer.GetPieceNeedForNextLevel(level_of_Skin);
        DataGameSave.skinLevels[indexThisSkinCell] += 1;
        DataGameSave.SaveToServer();
        CheckLevelAndPieces();
        onSelect?.Invoke(indexThisSkinCell, DataGameSave.dataServer.GetBonusPercent_At_LevelSkin(level_of_Skin));
    }
}
