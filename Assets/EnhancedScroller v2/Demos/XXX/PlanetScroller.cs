using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using SteveRogers;

public class PlanetScroller : MonoBehaviour
{
    public EnhancedScroller mainScroller;
    public EnhancedScrollerCellView cellViewPrefab;

    private Scroller<PlanetCellViewData> scroller = null;
    public float cellSize = 200f;

    public float scrollUnit = 1993 - 1778;
    public float startscrollPos = 1778;

    public static int currentPosIndex = 0;

    public void GoToPlanetPos(int idx)
    {
        if (idx < 0 || idx >= SpaceManager.Instance.ListSpace.Count)
            return;

        var nextpos = startscrollPos + (scrollUnit * idx);

        if (mainScroller.ScrollPosition == nextpos)
            return;

        currentPosIndex = idx;

        LeanTween.cancel(gameObject);

        LeanTween.value(
            gameObject,
            (float v) =>
                {
                    mainScroller.ScrollPosition = v;
                },
            mainScroller.ScrollPosition,
            nextpos,
            0.2f);
    }

    public void RefreshActives()
    {
        if (scroller == null)
        {
            scroller = new Scroller<PlanetCellViewData>(mainScroller, cellViewPrefab, cellSize, 1);
        }

        List<PlanetCellViewData> list = new List<PlanetCellViewData>(10);

        for (int i = 0; i < 10; i++)
        {
            var p = SpaceManager.Instance.ListSpace.Get(i);

            list.Add(new PlanetCellViewData
            {
                controller = p == null ? null : p.Planet
            });
        }

        scroller.Set(list);

        var curPosIdx = SpaceManager.Instance.ListSpace.FindIndex(i => i.Planet == SpaceManager.Instance.PlanetSelect);

        if (curPosIdx < 0)
            curPosIdx = 0;

        GoToPlanetPos(curPosIdx);
    }
}

public class PlanetCellViewData
{
    public PlanetController controller = null;
}