using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using System.Collections;
using System;
using SteveRogers;
using Newtonsoft.Json;

public class EventScroller : MonoBehaviour {
    public static EventScroller Instance;
    public static EventChartMode mode { get; private set; } = EventChartMode._Count;
    public static int playerRank = -1;

    public enum EventChartMode { HitPlanet, HitSun, HitMeteor, _Count }

    public EnhancedScroller mainScroller;
    public EnhancedScrollerCellView cellViewPrefab;

    public GameObject rankGroup, bannerGroup;
    public GameObject refreshedTxtGo = null;
    public GameObject loadingText;

    public Text remain24hTxt = null;
    public Text remain24hTxt_banner = null;
    public Text modeButtonTxt = null;
    public Text countColumeTitleTxt = null;
    public Text dailyAttackCount = null;

    public float cellSize = 200f;
    public int time24h = 24;
    public int refreshChartTime = 3;
    public bool needToRefresh = false;

    private Scroller<EventUserData> scroller = null;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        scroller = new Scroller<EventUserData>(mainScroller, cellViewPrefab, cellSize, 1);

        Refresh();
        MonoManager.RegisterUpdate(1, UpdateRemainTime24hText);
        MonoManager.RegisterUpdate(refreshChartTime, Refresh);
    }

    private void UpdateRemainTime24hText() {
        if (UIMultiScreenCanvasMan.modeExplore != UIMultiScreenCanvasMan.Mode.Event)
            return;

        if (!remain24hTxt)
            return;

        var now = GameManager.Now;
        var target = new DateTime(now.Year, now.Month, now.Day, time24h, 0, 0);
        var span = target - now;
        remain24hTxt.text = string.Format("End: {0}:{1}:{2}", (span.Hours + 1).ToString("00"), span.Minutes.ToString("00"), span.Seconds.ToString("00"));
        remain24hTxt_banner.text = string.Format("End: {0}:{1}:{2}", span.Hours.ToString("00"), span.Minutes.ToString("00"), span.Seconds.ToString("00"));
    }

    private bool isRefreshing = false;

    private void Update() {
        loadingText.SetActive(DataGameSave.eventDatas == null || DataGameSave.eventDatas.Count == 0);
    }

    public void Refresh() {
        //if (UIMultiScreenCanvasMan.modeExplore != UIMultiScreenCanvasMan.Mode.Event) {
        //    return;
        //}

        isRefreshing = true;

        //countColumeTitleTxt.text = TextMan.Get("Point");
        //modeButtonTxt.text = TextMan.Get("Battle Rank");

        //try {
        //    scroller.ClearActive();
        //} catch { }

        if (refreshedTxtGo)
        {
            LeanTween.cancel(refreshedTxtGo);
            refreshedTxtGo.SetActive(false);
        }

        if (!refreshedTxtGo || scroller == null)
            return;

        refreshedTxtGo.SetActive(true);
        LeanTween.delayedCall(refreshedTxtGo, 1, () => refreshedTxtGo.SetActive(false));

        isRefreshing = false;

        DataGameSave.GetRankChartData();

        if (DataGameSave.eventDatas.IsNullOrEmpty()) {
            //DataGameSave.GetRankChartData();
            return;
        }

        DataGameSave.eventDatas.Sort((a, b) => {
            return b.rankpoint.Parse(0).CompareTo(a.rankpoint.Parse(0));
        });

        for (int i = 0; i < DataGameSave.eventDatas.Count; i++) {
            if (DataGameSave.eventDatas[i].userid == DataGameSave.dataServer.userid.ToString()) {
                playerRank = i + 1;
            }
        }

        needToRefresh = false;
        scroller.Set(DataGameSave.eventDatas);
    }

    public void OnPressed_BattlePass() {
        Scenes.ChangeScene(SceneName.BattlePass);
    }

    public void OnSwitchedToEventScreen() 
    {
        bannerGroup.SetActive(true);
        var a  = rankGroup.GetComponent<CanvasGroup>();
        a.alpha = 0;
        a.interactable = false;

        Refresh();

        //dailyAttackCount.text = GameManager.DailyBattleAttackCount + "/" + GameManager.MAX_DAILY_ATTACK_COUNT;
        UpdateRemainTime24hText();
    }

    public void OnClickRank()
    {
        bannerGroup.SetActive(false);
        var a = rankGroup.GetComponent<CanvasGroup>();
        a.alpha = 1;
        a.interactable = true;

        if (GameManager.rankPointChange != 0) {
            PopupConfirm.ShowOK(TextMan.Get("Informantion"), string.Format(TextMan.Get("Last rank battle result: {0}"),GameManager.rankPointChange));
            GameManager.rankPointChange = 0;
        }
    }

    //public void ShowRank() {
    //    //bannerGroup.SetActive(true);
    //    var a = rankGroup.GetComponent<CanvasGroup>();
    //    a.alpha = 0;
    //    a.interactable = false;

    //    Refresh();

    //    dailyAttackCount.text = GameManager.DailyBattleAttackCount + "/" + GameManager.MAX_DAILY_ATTACK_COUNT;
    //    UpdateRemainTime24hText();

    //    bannerGroup.SetActive(false);
    //    a = rankGroup.GetComponent<CanvasGroup>();
    //    a.alpha = 1;
    //    a.interactable = true;

    //    if (GameManager.rankPointChange != 0) {
    //        PopupConfirm.ShowOK(TextMan.Get("Informantion"), TextMan.Get("Last rank battle result:") + GameManager.rankPointChange);
    //        GameManager.rankPointChange = 0;
    //    }
    //}

    /// <summary>
    /// click daily battle, find enemy which is higher than user 1 level
    /// attack win will get reward. max attack 5 times a day
    /// </summary>
    public void OnClickDailyBattle()
    {
        if (GameStatics.IsAnimating)
            return;

        for (int i = 0; i < DataGameSave.listDataEnemies.Count; i++)
        {
            if (DataGameSave.listDataEnemies[i].level == DataGameSave.dataServer.level + 1)
            {
                DataGameSave.dataEnemy = DataGameSave.listDataEnemies[i];
                PopupDailyBattle.Show(DataGameSave.listDataEnemies[i], "");

                return;
            }
        }

        // if not found then refresh enemy list and search again 
        ServerSystem.RefreshListEnemy(() => {
            OnClickDailyBattle();
        });
    }

    //public void SetTodayModeBaseOnDay() {
    //    //int n = GameManager.Now.Day % 3;
    //    //if (n == 0) {
    //    //    mode = EventChartMode.HitMeteor;
    //    //    if (countColumeTitleTxt && modeButtonTxt)
    //    //    {
    //    //        countColumeTitleTxt.text = TextMan.Get("Meteors");
    //    //        modeButtonTxt.text = TextMan.Get("Meteor_Chart");
    //    //    }
    //    //} else if (n == 1) {
    //    //    mode = EventChartMode.HitPlanet;
    //    //    if (countColumeTitleTxt && modeButtonTxt)
    //    //    {
    //    //        countColumeTitleTxt.text = TextMan.Get("Planets");
    //    //        modeButtonTxt.text = TextMan.Get("Planet_Chart");
    //    //    }
    //    //} else if (n == 2) {
    //    //    mode = EventChartMode.HitSun;
    //    //    if (countColumeTitleTxt && modeButtonTxt)
    //    //    {
    //    //        countColumeTitleTxt.text = TextMan.Get("Star");
    //    //        modeButtonTxt.text = TextMan.Get("Star_Chart");
    //    //    }
    //    //}


    //}
}

// model server
public class EventUserData {
    public string daycode;
    public string chartid;
    public string level;
    public string userid;
    public string name;
    public string rankpoint;
    public string rank_chart_id;
    //public string destroyed_solars;
    //public string destroy_planet;
    //public string meteor_planet_hit_count;
}