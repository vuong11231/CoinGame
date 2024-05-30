using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineHistoryController : MonoBehaviour, IEnhancedScrollerDelegate
{
    public OfflineHistoryCellView linePrefab;
    public EnhancedScroller myScroller;

    List<OfflineHistory> _data;
    int _dataCount;

    void Start()
    {
        myScroller.Delegate = this;
        _data = DataGameSave.dataLocal.battleHistory;
        _dataCount = _data.Count;
        myScroller.ReloadData();
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        OfflineHistoryCellView cellView = scroller.GetCellView(linePrefab) as OfflineHistoryCellView;
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
