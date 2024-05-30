using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnDailyMission : Singleton<BtnDailyMission>
{
    public GameObject RedDot;
    public GameObject RedDot2;
    List<DailyQuestData> _data;
    private void Start()
    {
        CheckDoneQuest();
    }
    public void CheckDoneQuest()
    {
        _data = GameManager.Instance.dailyQuestData;
        for (int i = 0; i < GameConstants.DAILY_QUEST_COUNT; i++)
        {
            DailyQuestSave save;

            save = DataGameSave.dataLocal.dailyMissions[(int)_data[i].quest];
            // Is Available
            if (save.currentProgress >= _data[i].maxProgress)
            {
                // Already received
                if (save.isReceived)
                {
                    RedDot.SetActive(false);
                    RedDot2.SetActive(false);
                }
                // Not received
                else
                {
                    RedDot.SetActive(true);
                    RedDot2.SetActive(true);
                    break;
                }
            }
            // Not available
            else
            {
                    RedDot.SetActive(false);
                    RedDot2.SetActive(false);
            }
        }
    }
}
