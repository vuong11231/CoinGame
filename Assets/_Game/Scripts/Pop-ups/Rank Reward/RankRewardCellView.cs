using EnhancedUI.EnhancedScroller;

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankRewardCellView : EnhancedScrollerCellView
{
    public TextMeshProUGUI Content;

    public void SetData(int data)
    {
        Content.text = "Congratulation!!! Yout got " + TextConstants.M_Mater + " " + data;
    }

    public void OnClaim()
    {
        DataGameSave.dataLocal.Diamond += GameStatics.RewardRankValue;
        GameStatics.RewardRankValue = 0;        
        Destroy(gameObject);
    }
}
