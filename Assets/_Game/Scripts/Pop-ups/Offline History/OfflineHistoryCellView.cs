using EnhancedUI.EnhancedScroller;
using SteveRogers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfflineHistoryCellView : EnhancedScrollerCellView
{
    public TextMeshProUGUI MaterialLose;

    string ListEnemy;

    public void SetData(OfflineHistory data)
    {
        // Name
        ListEnemy = "";
        var count = data.listEnemyName.Count;
        for (int i = 0; i < count; i++)
        {
            ListEnemy += data.listEnemyName[i];
            if (i != count - 1)
                ListEnemy += ", ";
        }

        // Material Lose
        MaterialLose.text = TextMan.Get("Total material lose: ") + TextConstants.M_Mater + " " + data.totalMaterialLose + " by " + ListEnemy;

        if(MaterialLose.text.Length > 150)
        {
            MaterialLose.text = MaterialLose.text.Substring(0, 150) + "...";
        }
    }
}
