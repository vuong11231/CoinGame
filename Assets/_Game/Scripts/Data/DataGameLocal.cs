using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataGameLocal
{
    public string playFabLoginCustomID;
    public string playFabToken; //use this to save metadata
    public DateLogin lastLogin;
    public int musicVolume;
    public int soundVolume;
    public int diamond;
    public ItemX2 itemX2;
    public int destroySolarByOneHit;
    public int destroyedSolars;
    public int startBattle;
    public int upgradePlanet;
    public int watchAds;
    public int destroyPlanet;
    public int transformPlanet;
    public int collectElementMeterior;
    public int completeDailyQuest;
    public List<DailyQuestSave> dailyMissions;
    public int randomMissionReward;
    public List<OfflineHistory> battleHistory;
    public int indexAvatar = 0;   //Minh.Ho Add

    // steve
    public int meteorPlanetHitCount = 0;
    public int meteorSpecialPlanetHitCount = 0;
    public int meteorMultiColorHitCount = 0;
    //steve.

    public int M_AirStone = 0;
    public int M_FireStone = 0;
    public int M_IceStone = 0;
    public int M_GravityStone = 0;
    public int M_AntimatterStone = 0;
    public int M_ColorfulStone = 0;

    public int M_ToyStone1 = 0;
    public int M_ToyStone2 = 0;
    public int M_ToyStone3 = 0;
    public int M_ToyStone4 = 0;
    public int M_ToyStone5 = 0;

    public DataGameLocal()
    {
        playFabLoginCustomID =
        playFabToken = "";
        musicVolume =
        soundVolume = 1;
        int _day = DateTime.Now.Day;
        int _month = DateTime.Now.Month;
        int _year = DateTime.Now.Year;
        lastLogin = new DateLogin();
        lastLogin.dayLogin = _day;
        lastLogin.monthLogin = _month;
        lastLogin.yearLogin = _year;
        diamond =
        m_Material =
        m_Air =
        m_Fire =
        m_Ice =
        m_Gravity =
        m_Antimatter = 0;
        itemX2 = new ItemX2();
        destroySolarByOneHit =
        startBattle =
        upgradePlanet =
        watchAds =
        destroyPlanet =
        transformPlanet =
        collectElementMeterior =
        completeDailyQuest = 0;
        dailyMissions = new List<DailyQuestSave>();
        randomMissionReward = 0;
        battleHistory = new List<OfflineHistory>();
        indexAvatar = 0;
    }

    private int m_Material;
    private int m_Air;
    private int m_Fire;
    private int m_Ice;
    private int m_Gravity;
    private int m_Antimatter;

    public int M_Material
    {
        get { return m_Material; }
        set
        {
            if (value < 0)
                value = 0;

            m_Material = value;
            if (MoneyManager.Instance) {
                MoneyManager.Instance.OnUpMater(0);
            }
        }
    }

    public int M_Air
    {
        get { return m_Air; }
        set
        {
            if (value < 0)
                value = 0;

            m_Air = value;
            if (MoneyManager.Instance) {
                MoneyManager.Instance.OnUpAir(0);
            }
        }
    }

    public int M_Fire
    {
        get { return m_Fire; }
        set
        {
            if (value < 0)
                value = 0;

            m_Fire = value;
            if (MoneyManager.Instance) {
                MoneyManager.Instance.OnUpFire(0);
            }
        }
    }

    public int M_Ice
    {
        get { return m_Ice; }
        set
        {
            if (value < 0)
                value = 0;

            m_Ice = value;
            if (MoneyManager.Instance) {
                MoneyManager.Instance.OnUpIce(0);
            }
        }
    }

    public int M_Gravity
    {
        get { return m_Gravity; }
        set
        {
            if (value < 0)
                value = 0;

            m_Gravity = value;
            if (MoneyManager.Instance) {
                MoneyManager.Instance.OnUpGravity(0);
            }
        }
    }

    public int M_Antimatter
    {
        get { return m_Antimatter; }
        set
        {
            if (value < 0)
                value = 0;

            m_Antimatter = value;
            if (MoneyManager.Instance) {
                MoneyManager.Instance.OnUpAntimat(0);
            }
        }
    }

    public int Diamond
    {
        get
        {
            return diamond;
        }
        set
        {
            if (value < 0)
                value = 0;

            diamond = value;

            if (MoneyManager.Instance) {
                MoneyManager.Instance.UpdateMoneyDisplay();
            }
        }
    }

    public int DestroySolarByOneHit
    {
        get
        {
            return destroySolarByOneHit;
        }
        set
        {
            destroySolarByOneHit = value;
            if (DataGameSave.dataLocal != null) {
                GameCenters.UpdateAchievement(Achievements.destroy_all_with_1_shot, destroySolarByOneHit);
            }
        }
    }

    public int StartBattle
    {
        get
        {
            return startBattle;
        }
        set
        {
            startBattle = value;
            if (DataGameSave.dataLocal != null) {
                GameCenters.UpdateAchievement(Achievements.battle, startBattle);
            }
        }
    }

    public int UpgradePlanet
    {
        get
        {
            return upgradePlanet;
        }
        set
        {
            upgradePlanet = value;
            if (DataGameSave.dataLocal != null) {
                GameCenters.UpdateAchievement(Achievements.upgrade_planet, upgradePlanet);
            }
        }
    }

    public int WatchAds
    {
        get
        {
            return watchAds;
        }
        set
        {
            watchAds = value;
            if (DataGameSave.dataLocal != null) {
                GameCenters.UpdateAchievement(Achievements.watch_ads, watchAds);
            }
        }
    }

    public int DestroyPlanet
    {
        get
        {
            return destroyPlanet;
        }
        set
        {
            destroyPlanet = value;
            if (DataGameSave.dataLocal != null) {
                GameCenters.UpdateAchievement(Achievements.destroy_planet, destroyPlanet);
            }
        }
    }

    public int TransformPlanet
    {
        get
        {
            return transformPlanet;
        }
        set
        {
            transformPlanet = value;
            if (DataGameSave.dataLocal != null) {
                GameCenters.UpdateAchievement(Achievements.transform_planet, transformPlanet);
            }
        }
    }

    public int CollectElementMeterior
    {
        get
        {
            return collectElementMeterior;
        }
        set
        {
            collectElementMeterior = value;
            if (DataGameSave.dataLocal != null) {
                GameCenters.UpdateAchievement(Achievements.collect_meteor, collectElementMeterior);
            }
        }
    }

    public int CompleteDailyQuest
    {
        get
        {
            return completeDailyQuest;
        }
        set
        {
            completeDailyQuest = value;
            if (DataGameSave.dataLocal != null) {
                GameCenters.UpdateAchievement(Achievements.claim_daily_quest, completeDailyQuest);
            }
        }
    }

    public int GetNumberStone(TypePlanet type)
    {
        if (type == TypePlanet.Air)
            return M_Air;

        else if (type == TypePlanet.Fire)
            return M_Fire;

        else if (type == TypePlanet.Ice)
            return M_Ice;

        else if (type == TypePlanet.Gravity)
            return M_Gravity;

        else if (type == TypePlanet.Antimatter)
            return M_Antimatter;

        else if (type == TypePlanet.Default)
            return M_Material;

        else
            throw new NotImplementedException(type.ToString());
    }
}