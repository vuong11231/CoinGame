using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [Header("Enhanced scroller")]
    public EnhancedScroller myScroller;
    [SerializeField]
    private AchievementViewRowController achievementViewRowPrefab;
    [SerializeField]
    private Transform popup;

    List<AchievementViewRowData> _data;
    int _dataCount;

    private void Start()
    {
        myScroller.Delegate = this;
        _data = PlayfabManager.Instance.achievements;
        _dataCount = _data.Count;
        myScroller.ReloadData();
    }

    //[UnityEditor.MenuItem("Project/Somethings/Fill Achi Amount")]
    //private static void FillAmou()
    //{
    //    //GameObject go = UnityEditor.Selection.activeGameObject;

    //    //foreach (var i in go.GetComponent<PlayfabManager>().achievements)
    //    //    i.FillAmount();

    //    //SteveRogers.Utilities.MarkAllScenesDirty();



    //    GameObject go = UnityEditor.Selection.activeGameObject;
    //    List<string> l = new List<string>();

    //    foreach (var i in go.GetComponent<PlayfabManager>().achievements)
    //    {
    //        foreach (var a in i.achievementName)
    //        {
    //            if (l.Contains(a))
    //                continue;

    //            l.Add(a);
    //        }
    //    }

    //    SteveRogers.Utilities.Clipboard = string.Join("\n", l.ToArray());
    //    SteveRogers.Utilities.WarningDone();
    //}

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        AchievementViewRowController cellView = scroller.GetCellView(achievementViewRowPrefab) as AchievementViewRowController;

        cellView.SetData(_data[dataIndex], dataIndex, popup);

        return cellView;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 217.9f;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _dataCount;
    }
}
