using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Achievements
{
    destroy_all_with_1_shot, // 1
    level_up, // 2
    battle, // 3
    upgrade_planet, // 4
    be_attack, // 5
    watch_ads, // 6
    destroy_planet, // 7
    transform_planet, // 8
    //invite_friend, // 9
    collect_meteor, // 10
    claim_daily_quest, // 11
}

public enum Leaderboards
{
    SUN_LEVEL,
    Galaxy_Day,
    Galaxy_Month,
    Galaxy_Week,
    Galaxy_World,
    Intergalactic_Day,
    Intergalactic_Month,
    Intergalactic_Week,
    Intergalactic_World,
    Planet_Day,
    Planet_Month,
    Planet_Week,
    Planet_World,
    SolarSystem_Day,
    SolarSystem_Month,
    SolarSystem_Week,
    SolarSystem_World,
    Universe_Day,
    Universe_Month,
    Universe_Week,
    Universe_World,

    COUNT
}

public class GameCenters
{
    public static int[][] MaxProcess =
    {
        new int[] { 1, 5, 10, 50, 100, 500, 1000 }, // 1
        new int[] { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50 }, // 2
        new int[] { 10, 50, 100, 500, 1000, 5000, 10000 }, // 3
        new int[] { 10, 50, 100, 500, 1000, 5000, 10000 }, // 4
        new int[] { 1, 5, 10, 50, 100, 500, 1000 }, // 5
        new int[] { 10, 50, 100, 500, 1000, 5000, 10000 }, // 6
        new int[] { 10, 50, 100, 500, 1000, 5000, 10000 }, // 7
        new int[] { 5, 25, 50, 100, 500, 1000, 5000 }, // 8
        new int[] { 10, 50, 100, 500, 1000, 5000, 10000 }, // 10
        new int[] { 7, 30, 60, 90, 120, 150, 180, 210, 240, 270 }, // 11
    };

    static string _Name;
    static int _MaxProgress;
    static double _Progress;

    #region Achievement

    public static void UpdateAchievement(Achievements type, int value)
    {
        var count = MaxProcess[(int)type].Length;
        var index = -1;

        for (int i = 0; i < count; i++)
        {
            if (i == count - 1 && DataHelper.GetBool("Toast_Achievement_" + type + "_" + i, false) == true)
            {
                return;
            }
            else if(DataHelper.GetBool("Toast_Achievement_" + type + "_" + i, false) == false)
            {
                index = i;
                break;
            }
        }

        count = MaxProcess[(int)type].Length;
        _MaxProgress = MaxProcess[(int)type][index];
        _Progress = ((double)value / _MaxProgress) * 100d;

        // Maximum

        if (_Progress >= 100d && !PlayfabManager.Instance.achievements[(int)type].isReceived[index])
        {
            DataHelper.SetBool("Toast_Achievement_" + type + "_" + index, true);

            AchievementToast.ShowShort(
                PlayfabManager.Instance.achievements[(int)type].achievementName[index],
                Popups.CanvasPopup.transform
            );
        }
    }

    #endregion
}