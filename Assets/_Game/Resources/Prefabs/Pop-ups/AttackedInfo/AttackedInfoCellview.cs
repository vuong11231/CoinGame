using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using SteveRogers;
using System;

public class AttackedInfoCellview : EnhancedScrollerCellView
{
    public Text nameAttacker = null;
    public Text id = null;
    public Text matNum = null;
    public Button revengeBtn = null;

    public override void SetData<T>(ref T[] list, int startIndex)
    {
        AttackedInfoData[] l = list as AttackedInfoData[];
        var idd = l[startIndex].id;

        nameAttacker.text = l[startIndex].name;
        id.text = "ID: " + idd.ToString("0000");
        matNum.text = "-" + l[startIndex].mat.ToString("000000");

        revengeBtn.onClick.RemoveAllListeners();
        revengeBtn.onClick.AddListener(() => GameManager.Instance.Revenge(idd));
    }
}