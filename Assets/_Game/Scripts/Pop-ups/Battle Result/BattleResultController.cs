using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResultController : MonoBehaviour, IEnhancedScrollerDelegate
{
    public BattleResultCellView linePrefab;
    public EnhancedScroller myScroller;

    void Start()
    {
        myScroller.Delegate = this;
        myScroller.ReloadData();
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        BattleResultCellView cellView = scroller.GetCellView(linePrefab) as BattleResultCellView;
        cellView.SetData(DataGameSave.dataServer.ListEnemy[dataIndex]);

        return cellView;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 89.8f;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return DataGameSave.dataServer.ListEnemy.Count;
    }
}
