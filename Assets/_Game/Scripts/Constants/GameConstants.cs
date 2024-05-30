using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class GameConstants
{
    public const string SAVEGAME = "SaveGame";
    public const string KEY_ALREADY_RECEIVE_DIAMOND_CONNECT_FACEBOOK = "KEY_CONNECT_FACEBOOK";
    public const string KEY_TUTORIAL = "KEY_TUTORIAL";
    public const string DEFAULT_USER_NAME = "Guest";
    public const string TITLE_ID = "44DFC";
    public const char CHAT_TROLL_SYNTAX = '?';
    public const int LEADERBOARD_MULTIPLIER_NUMBER = 1000000;
    //public const int MAX_MONEY_COLLECT = 2000;
    public const int MAX_TIME_COLLECT = 10800;

    // Battle Tag
    public const string ENEMY_TAG = "Enemy";
    public const string SUN_TAG = "Sun";
    public const string BOMBZONE_TAG = "BombZone";
    public const string PLANET_TAG = "Planet";
    public const string LIMIT_TAG = "Limit";
    public const string METEOR_TAG = "Meteor";
    public const string PLAYER_TAG = "Player";

    // Push data
    public const string DATA_KEY = "DataJSON";
    public const string ONL_TIME_KEY = "OnlineSeconds";
    public const string REWARD_RANK_KEY = "RewardRank";

    // Daily Mission
    public const int NORMAL_MISSION_MULTIPLIER = 2000;
    public const int FINAL_MISSION_MULTIPLIER = 2;
    public const int DAILY_QUEST_COUNT = 6;

    // File data
    public const string LOGIN_DATA_LOCAL_FILE_NAME = "alkdjflksdfjlsjdlfkahsdkasjdbasd";
    public const string LOGIN_DATA_HASH_KEY = "aisu3u41@3OWIE!AQqc$";
    public const string LOCAL_DATA_FILE_NAME = "alsxcv3mnx123hsdhv43asjdklans3k";
    public const string SERVER_DATA_FILE_NAME = "wqedassads3bsd6345g43xcvđ23342xcv";

    // Local backup data
    public const string LOCAL_BACKUP_DATASERVER = "QPQe7vPXc4Ti6UQM9NZ1khFkf0aBfffXaHk";
    public const string LOCAL_BACKUP_DATALOCAL = "APhfCcJao8et5Fq6oKSEEnHVd02gkStBO4";
    public const string LOCAL_BACKUP_DATARANK = "NhmxaK611mglAPUcr64nAjfYuDJraXloR3c4";
    public const string LOCAL_BACKUP_SKINLEVELS = "aD4dm50hlxCsY8Jb5RABz7vmkC4bblOF0slK7";
    public const string LOCAL_BACKUP_SKINPIECES = "GDk0sc9QrCXZCCUPyIOAX7ckZX3YnydA4NrRLv";
    public const string LOCAL_BACKUP_METADATA = "7cFOOVK1vGVJC1F9N4nIHwlWgWCTxiy828O0MPo";
}

// Scenes
public enum SceneName
{
    CustomLoading,
    Gameplay,
    Battle,
    Explore,
    Neightbor,
    MeteorBelt,
    BlackHole,
    BattlePass,
    BattlePassGameplay
}

public enum LeaderboardType
{
    CorrectStreak,
    Score
}

public enum LeaderboardResetType
{
    Weekly,
    Monthly,
    Annual
}
public enum TypeQuest
{
    Viet,
    English,
    Math,
    Dialy,
    Picture
}

public enum TypePlanet
{
    Antimatter,
    Gravity,
    Ice,
    Fire,
    Air,
    Default,
    Diamond,
    Destroy
} // Ko thay đổi thứ tự

public enum TypeMaterial
{
    Antimatter,
    Gravity,
    Ice,
    Fire,
    Air,
    Default,
} // Ko thay đổi thứ tự

public enum TypeSize
{
    Small,
    Normal,
    Big
}

public enum TypeShopItem
{
    Antimatter,
    Gravity,
    Ice,
    Fire,
    Air,
    Default,
    Diamond_1,
    Diamond_2,
    Diamond_3,
    Diamond_4,
    Diamond_5,
    Diamond_6,
    AutoRestoreIAP, // change to autorestore 2h
    X2,
    PlanetCore,
    AutoRestore,
    BattlePass,
    _1h = 17,
    _6h = 18,
    _12h = 19,
    _1d = 20,
    _2d = 21,
    _3d = 22,
    _7d = 23
}

public class RewardLogin
{
    public enum Type
    {
        Diamond,
        RockEffect,
        RockRandom,
        RecoverTime,
        Planet,
        AsteroidBelt,
    }

    public Type type;
    public float amount;
    public int day;

    RewardLogin(int day, float amount, Type type)
    {
        this.day = day;
        this.amount = amount;
        this.type = type;
    }

    public static readonly RewardLogin[] ListReards = {
        new RewardLogin(1, 100, Type.Diamond),
        new RewardLogin(2, 5, Type.RockEffect),
        new RewardLogin(3, 20, Type.RockRandom),
        new RewardLogin(4, 200, Type.Diamond),
        new RewardLogin(5, 30, Type.RecoverTime),
        new RewardLogin(6, 10, Type.RockEffect),
        new RewardLogin(7, 500, Type.Diamond),
        new RewardLogin(8, 100, Type.RockRandom),
        new RewardLogin(9, 60, Type.RecoverTime),
        new RewardLogin(10, 1000, Type.Diamond),
        new RewardLogin(11, 20, Type.RockEffect),
        new RewardLogin(12, 120, Type.RecoverTime),
        new RewardLogin(13, 1500, Type.Diamond),
        new RewardLogin(14, 1, Type.Planet),
    };
}



[Serializable]
public class DateLogin
{
    public int dayLogin;
    public int monthLogin;
    public int yearLogin;
    public long Timeoff;
}

[Serializable]
public class ItemX2
{
    public bool isActived;
    public int multiplyNumber;
    public DateTime startTime;

    public bool IsActived
    {
        get { return isActived; }
        set
        {
            isActived = value;
            if (MoneyManager.Instance && value)
            {
                startTime = DateTime.Now;
                multiplyNumber = 2;
                // Active x2 note in gameplay scene
                MoneyManager.Instance.ActiveItemX2(true);
            }
            else if(MoneyManager.Instance && !value)
            {
                startTime = DateTime.MinValue;
                multiplyNumber = 1;
                // Deactive x2 note in gameplay scene
                MoneyManager.Instance.ActiveItemX2(false);
            }
        }
    }

    public ItemX2()
    {
        isActived = false;
        multiplyNumber = 1;
        startTime = DateTime.MinValue;
    }
}