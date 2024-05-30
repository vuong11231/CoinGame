using BayatGames.SaveGameFree.Encoders;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarCell : MonoBehaviour
{
    public int indexThisAvatar;
    public Image avatarIcon;
    public GameObject tick;
    private Action<int> onSelect;


    public void SetData(int index, Sprite avatar, Action<int> onSelectThisAvatar)
    {
        indexThisAvatar = index;
        tick.SetActive(false);
        avatarIcon.sprite = avatar;
        onSelect = onSelectThisAvatar;
    }
    public void OnSelected()
    {
        DataGameSave.dataLocal.indexAvatar = indexThisAvatar;
        DataGameSave.dataServer.rankChartId = indexThisAvatar;
        DataGameSave.SaveToServer();
        DataGameSave.SaveToLocal();
        onSelect.Invoke(indexThisAvatar);
    }
}
