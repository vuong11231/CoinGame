using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LeaderboardDetailViewRowData
{
    public bool isYou;
    public string playfabId;
    public string avatar;
    public string username;
    public int currentValue;
    public int currentPosition;
    public string type;
}
