using Hellmade.Sound;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SteveRogers;

public class HomeBottomUI : MonoBehaviour
{
    public static HomeBottomUI Instance;

    public static bool isAutoShooting = false;

    public GameObject ShootBtn;
    public GameObject ShootAutoBtn;
    public TextMeshProUGUI txtAutoShootCount;
    public Image autoShootWatchAds;

    public bool pausingSpins = false;

    float startTime = 0f;
    bool isStop = false;
    bool canAutoShooting = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        startTime = Time.time;
        if (txtAutoShootCount != null) {
            txtAutoShootCount.text = DataGameSave.autoShootCount.ToString();
        }
    }

    private void Update()
    {
        // drag and shoot (not use btn shoot)
        if ((Scenes.Current == SceneName.Battle || Scenes.Current == SceneName.BattlePassGameplay) && !Input.anyKey)
        {
            if (ShootPath.Instance.GetActivePath())
            {
                OnShoot();
            }
        }

        // btn auto shoot
        if (ShootAutoBtn == null)
        {
            return;
        }

        if (Time.time - startTime > 3f)
        {
            ShootAutoBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            canAutoShooting = true;
        }
        else {
            ShootAutoBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 120);
            canAutoShooting = false;
        }

        if (txtAutoShootCount) {
            txtAutoShootCount.gameObject.CheckAndSetActive(DataGameSave.autoShootCount > 0);
        }
        if (autoShootWatchAds) {
            autoShootWatchAds.gameObject.CheckAndSetActive(DataGameSave.autoShootCount <= 0);
        }
        txtAutoShootCount.text = DataGameSave.autoShootCount.ToString();
    }

    public void OnSetting()
    {
        if (GameStatics.IsAnimating)
            return;

        PopupSetting.Show();

        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnAchievement()
    {
        if (GameStatics.IsAnimating)
            return;

        PopupAchievements.Show();

        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnDailyMission()
    {
        if (GameStatics.IsAnimating)
            return;

        PopupDailyMission.Show();

        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnRank()
    {
        if (GameStatics.IsAnimating)
            return;

        PopupLeaderboard.Show();

        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnSwitchToGameplayScene()
    {
        DataGameSave.dataEnemy.beAttacked++;
        DataGameSave.SaveToServer();

        PopupExitBattle.ShowYesNo(
            () =>
            {
                if (GameStatics.hasShoot)
                {
                    DataGameSave.dataLocal.StartBattle++;
                    GameStatics.hasShoot = false;
                }
                GameStatics.HasStartFromRank = false;
                Scenes.ReturnToLastScene(Scenes.CurrentSceneId);

                //if (Scenes.LastScene == SceneName.Gameplay)
                //{
                //    Scenes.LastScene = SceneName.Battle;
                //    Scenes.ChangeScene(SceneName.Gameplay);
                //}
                //else
                //{
                //    Scenes.LastScene = SceneName.Battle;
                //    Scenes.ChangeScene(SceneName.Neightbor);
                //}
            });

        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnAutoShoot()
    {
        if (GameStatics.IsAnimating || isAutoShooting || !canAutoShooting)
            return;

        if (DataGameSave.autoShootCount <= 0) {
            DataGameSave.autoShootCount = 0;

            GoogleMobileAdsManager.Instance.ShowRewardedVideo(() =>
            {
                PopupConfirm.ShowOK(TextMan.Get("Congratulations"), string.Format(TextMan.Get("Add {0} Time Auto Shoot"),GameManager.MAX_AUTO_SHOOT_COUNT), "Great", () =>
                {
                    DataGameSave.autoShootCount += GameManager.MAX_AUTO_SHOOT_COUNT;
                    DataGameSave.SaveToServer();
                });
                DataGameSave.SaveToServer();
            });
            return;
        }

        isAutoShooting = true;
        var count = SpaceManager.Instance.ListSpace.Count;

        for (int i = 0; i < count; i++)
        {
            SpaceManager.Instance.ListSpace[i].Planet.StartDrag();
        }

        for (int i = 0; i < count; i++)
        {
            if (SpaceManager.Instance.ListSpace[i].Planet.IEShoot != null)
            {
                if (!SpaceManager.Instance.ListSpace[i].Planet.isFlying)
                    StartCoroutine(SpaceManager.Instance.ListSpace[i].Planet.IEShoot);

                PointPull.SetActive(null);

                if (ShootPath.Instance)
                {
                    ShootPath.Instance.SetActivePath(false);
                }
            }
        }

        DataGameSave.autoShootCount--;
        txtAutoShootCount.text = DataGameSave.autoShootCount.ToString();
        DataGameSave.SaveMetaData(MetaDataKey.AUTO_SHOOT_COUNT, DataGameSave.autoShootCount.ToString());
        DataGameSave.SaveMetaData(MetaDataKey.AUTO_SHOOT_DAYCODE, DataGameSave.GetDayCode(GameManager.Now));
        isAutoShooting = false;
    }

    public void OnShoot()
    {
        GameStatics.hasShoot = true;
        GameStatics.ShootTimes++;
        ShootBtn.GetComponent<Button>().interactable = false;
        var count = SpaceManager.Instance.ListSpace.Count;

        for (int i = 0; i < count; i++)
        {
            if (SpaceManager.Instance.ListSpace[i].Planet.IEShoot != null)
            {
                if (!SpaceManager.Instance.ListSpace[i].Planet.isFlying)
                    StartCoroutine(SpaceManager.Instance.ListSpace[i].Planet.IEShoot);
             
                PointPull.SetActive(null);

                if (ShootPath.Instance)
                {
                    ShootPath.Instance.SetActivePath(false);
                }
            }
        }

        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        Utilities.SetPlayerPrefsBool("firstEnterBattle", false);
    }

    public void OnPressed_Pause()
    {
        pausingSpins = !pausingSpins;

        if (!pausingSpins) {
            foreach (var i in SpaceManager.Instance.ListSpace) {
                if (i.Planet.IEShoot == null) {
                    i.Planet.SpinAround();
                }
            }
        }
    }
}
