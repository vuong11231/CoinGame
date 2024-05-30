using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    public List<LeaderboardViewRowController> listRankTier;

    List<LeaderboardViewRowData> _data;
    int _dataCount;
    
    public void SetData()
    {
        var count = listRankTier.Count;
        for(int i = 0; i < count; i++)
        {
            listRankTier[i].SetData();
        }
    }
}
