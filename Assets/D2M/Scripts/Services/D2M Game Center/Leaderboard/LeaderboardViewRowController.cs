using EnhancedUI.EnhancedScroller;
using Hellmade.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardViewRowController : MonoBehaviour
{
    [HideInInspector]
    public Leaderboards type;

    public int index;
    public LeaderboardDetailController detailController;

    public void SetData()
    {
        switch (PopupLeaderboard.rankType)
        {
            case RankType.Daily:
                switch (index)
                {
                    case 0:
                        type = Leaderboards.Planet_Day;
                        break;
                    case 1:
                        type = Leaderboards.SolarSystem_Day;
                        break;
                    case 2:
                        type = Leaderboards.Galaxy_Day;
                        break;
                    case 3:
                        type = Leaderboards.Intergalactic_Day;
                        break;
                    case 4:
                        type = Leaderboards.Universe_Day;
                        break;
                }
                break;
            case RankType.Weekly:
                switch (index)
                {
                    case 0:
                        type = Leaderboards.Planet_Week;
                        break;
                    case 1:
                        type = Leaderboards.SolarSystem_Week;
                        break;
                    case 2:
                        type = Leaderboards.Galaxy_Week;
                        break;
                    case 3:
                        type = Leaderboards.Intergalactic_Week;
                        break;
                    case 4:
                        type = Leaderboards.Universe_Week;
                        break;
                }
                break;
            case RankType.Monthly:
                switch (index)
                {
                    case 0:
                        type = Leaderboards.Planet_Month;
                        break;
                    case 1:
                        type = Leaderboards.SolarSystem_Month;
                        break;
                    case 2:
                        type = Leaderboards.Galaxy_Month;
                        break;
                    case 3:
                        type = Leaderboards.Intergalactic_Month;
                        break;
                    case 4:
                        type = Leaderboards.Universe_Month;
                        break;
                }
                break;
            case RankType.World:
                switch (index)
                {
                    case 0:
                        type = Leaderboards.Planet_World;
                        break;
                    case 1:
                        type = Leaderboards.SolarSystem_World;
                        break;
                    case 2:
                        type = Leaderboards.Galaxy_World;
                        break;
                    case 3:
                        type = Leaderboards.Intergalactic_World;
                        break;
                    case 4:
                        type = Leaderboards.Universe_World;
                        break;
                }
                break;
        }

        detailController.GetListData();
    }
}
