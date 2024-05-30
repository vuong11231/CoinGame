using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultCellView : EnhancedScrollerCellView
{
    public Text name;

    public void SetData(string enemyName)
    {
        name.text = "- " + enemyName;
    }
}
