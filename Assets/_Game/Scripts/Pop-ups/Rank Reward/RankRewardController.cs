using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankRewardController : MonoBehaviour, IEnhancedScrollerDelegate
{
    public RankRewardCellView linePrefab;
    public EnhancedScroller myScroller;

    List<int> _data;
    int _dataCount;

    void Start()
    {
        myScroller.Delegate = this;
        _data = new List<int>();
        if(GameStatics.RewardRankValue > 0)
            _data.Add(GameStatics.RewardRankValue);
        _dataCount = _data.Count;
        myScroller.ReloadData();
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        RankRewardCellView cellView = scroller.GetCellView(linePrefab) as RankRewardCellView;
        cellView.SetData(_data[dataIndex]);

        return cellView;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 150f;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _dataCount;
    }
}
