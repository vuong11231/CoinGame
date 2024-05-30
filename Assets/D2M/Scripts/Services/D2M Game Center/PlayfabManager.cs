using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class LeaderboardBaseData
{
    public string Title;
}

public enum GroupStatus
{
    NoExist,
    Available,
    Unavailable
}

public class PlayfabManager : Singleton<PlayfabManager>
{  
    public List<AchievementViewRowData> achievements;   

    void Start()
    {
        int count = 0;

        foreach (Achievements achieve in (Achievements[])Enum.GetValues(typeof(Achievements)))
        {
            var countA = achievements[count].isReceived.Length;
            for (int i = 0; i < countA; i++)
            {
                achievements[count].isReceived[i] = DataHelper.GetBool("Achievement_" + achieve + "_" + i, false);
            }
            count++;
        }
    }

    public static void GetServerTime(int index, Action<DateTime, int> _onsuccess)
    {
        _onsuccess.Invoke(GameManager.Now, index);
        //ServerSystem.Instance.SendRequest(ServerConstants.GET_SERVER_TIME, null, () =>
        //{
        //    string result = ServerSystem.result;
        //    DateTime now = JsonConvert.DeserializeObject<DateTime>(result);
        //    _onsuccess.Invoke(now, index);
        //});
    }   
}