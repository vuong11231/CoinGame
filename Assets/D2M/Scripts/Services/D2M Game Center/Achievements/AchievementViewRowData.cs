using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AchievementViewRowData
{
    public Achievements type;
    public string[] achievementName;
    public int[] amount;
    public int[] meteriorReward;
    public bool[] isReceived;

    public void FillAmount()
    {
        amount = new int[achievementName.Length];

        for (int i = 0; i < achievementName.Length; i++)
        { 
            var t = achievementName[i];
            var digits = "";

            for (int j = 0; j < t.Length; j++)
            {
                var c = t[j];

                if (char.IsDigit(c))
                {
                    digits += c;
                }
            }

            if (digits.IsValid())
            {
                achievementName[i] = achievementName[i].Replace(digits, "#");
                amount[i] = digits.Parse(-1);
            }
            else
                amount[i] = -1;
        }
    }
}
