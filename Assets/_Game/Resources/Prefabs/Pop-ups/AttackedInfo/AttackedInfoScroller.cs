using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using SteveRogers;

public class AttackedInfoScroller : MonoBehaviour
{
    public EnhancedScroller mainScroller;
    public EnhancedScrollerCellView cellViewPrefab;

    private Scroller<AttackedInfoData> scroller = null;
    public float cellSize = 300f;

    public void Set(List<AttackedInfoData> data)
    {
        if (scroller == null)
        {
            scroller = new Scroller<AttackedInfoData>(mainScroller, cellViewPrefab, cellSize, 1);
        }

        scroller.Set(data);
    }
}