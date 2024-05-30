using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;
using SteveRogers;

public class EventTopRankLoader : MonoBehaviour
{
    //public GameObject cell;
    public RectTransform content;
    public static float CELL_HEIGHT = 170f;
    
    public List<EventScrollerCellView> listCells;
    bool loaded = false;

    public TextMeshProUGUI txtCountDown;

    private void Start()
    {
        //StartCoroutine(CountDownTime());   

        //MonoManager.RegisterUpdate(1, CountDownTime);
    }

    private void Update()
    {
        if (DataGameSave.listTopRank == null) {
            for (int i = 0; i < listCells.Count; i++)
            {
                listCells[i].gameObject.SetActive(false);
            }
        }

        if (!loaded && DataGameSave.listTopRank != null)
        {
            for (int i = 0; i < listCells.Count; i++)
            {
                if (i >= DataGameSave.listTopRank.Count)
                {
                    listCells[i].gameObject.SetActive(false);
                }
                else {
                    listCells[i].gameObject.SetActive(true);
                    listCells[i].SetEventData(DataGameSave.listTopRank[i], GetPrize(i));
                }
            }

            content.sizeDelta = new Vector2(content.sizeDelta.x, DataGameSave.listTopRank.Count * CELL_HEIGHT);
        }
    }

    public void CountDownTime() {
        //var now = GameManager.Now;
        //var target = new DateTime(2021, GameManager.EVENT_END_MONTH, GameManager.EVENT_END_DAY, 24, 0, 0);
        //var span = target - now;
        //txtCountDown.text = string.Format("End: {0}:{1}:{2}", span.Hours.ToString("00"), span.Minutes.ToString("00"), span.Seconds.ToString("00"));

        //while (true) {
        //    //DateTime endTime = DateTime.ParseExact(GameManager.EVENT_END_TIME, "yyyyMMdd", CultureInfo.InvariantCulture);
        //    //DateTime remainTime = new DateTime((endTime - GameManager.Now).Ticks);
        //    //txtCountDown.text = remainTime.ToString("dd:hh:mm:ss");

        //    //if (UIMultiScreenCanvasMan.modeExplore != UIMultiScreenCanvasMan.Mode.Event)
        //    //    return;

        //    //if (!remain24hTxt)
        //    //    return;


        //    //remain24hTxt_banner.text = string.Format("End: {0}:{1}:{2}", span.Hours.ToString("00"), span.Minutes.ToString("00"), span.Seconds.ToString("00"));

        //    yield return new WaitForSecondsRealtime(1f); 
        //}
    }

    public int GetPrize(int rank) {
        if (rank == 0)
        {
            return 10000;
        }
        else if (rank == 1)
        {
            return 5000;
        }
        else if (rank == 2)
        {
            return 3000;
        }
        else if (rank == 3)
        {
            return 2000;
        }
        else if (rank == 4)
        {
            return 1000;
        }
        else {
            return 500;
        }
    }
}
