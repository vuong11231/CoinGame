using BayatGames.SaveGameFree.Encoders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class SkinCellMini : MonoBehaviour
{
    public int index_SkinCellMini;
    public string nameSkin;
    public Image skinImage;
    public GameObject tick;
    private Action<int,float> onSelect;


    public void SetData(int index, PopupSkinPlanet.OneSkinCell onePlanetCell, Action<int,float> callback)
    {
        index_SkinCellMini = index;
        nameSkin = onePlanetCell.namePlanet;
        if (index_SkinCellMini < GameManager.Instance.listSkin.Count)
        {
            skinImage.sprite = GameManager.Instance.listSkin[index_SkinCellMini];
        }
        tick.SetActive(false);
        onSelect = callback;
        CheckUnlock();
    }

    public void OnSelected()
    {
        onSelect.Invoke(index_SkinCellMini, (float)DataGameSave.dataServer.GetBonusPercent_At_LevelSkin(DataGameSave.skinLevels[index_SkinCellMini]));
    }

    public void CheckUnlock()
    {
        int piece = DataGameSave.skinPieces[index_SkinCellMini];
        int level = DataGameSave.skinLevels[index_SkinCellMini];
        bool isUnlock_by_level = DataGameSave.dataServer.level > index_SkinCellMini;
        if (piece > 0 || level > 1 || isUnlock_by_level)
        {
            this.GetComponent<Button>().interactable = true;
            skinImage.color *= Vector4.one;
        }
        else
        {
            this.GetComponent<Button>().interactable = false;
            skinImage.color = new Vector4(skinImage.color.r, skinImage.color.b, skinImage.color.g, 0.4f);
        }
    }
}
