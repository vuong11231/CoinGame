
using System.Collections.Generic;

public enum TypeEventScene
{
    None,
    OpenMap,
    OpenStage,
    OpenSurvival,
    OpenTrickMode,
    OpenPVP,
    OpenLadder,
    OpenRaid,
    OpenAccessories,
    OpenUpdateChar,
    OpenUpgradeSkill
}

public enum NameScene
{
    LoadingMenu,
    //MainMenu,
    //Endless,
    GamePlay,
    sceneLoading,
}

public enum TypeLogin
{
    GG,
    IOS,
    None
}

public enum TypeNotificationWindow
{
    Normal,
    YesNo
}

public enum TypeSoundSE
{
    StartGame,
    ClickButton,
    ShowPopUp,
    HidePopUp,
    Unlock,
    DeleteChar,
    UpdateGold,
    UpdateSoul,
    BuyPackage,
    Victory,
    Defeat,
    UpgradeHero,
}

public enum GameMode
{
    PUZZLE_MODE = 0,
    ENDLESS_MODE = 1
}

public enum EffectType
{
    All,
    //Hiệu ứng có hại
    Harmful,
    //Hiệu ứng có lợi
    Helpful,
    //Hiệu ứng hóa giải
    Dispel,
    //Hiệu ứng vật lý bị bay
    Physical
}

public enum QuestStageType
{
    Complete_Before_Time, // Hoàn thành sau ? giây
    Take_Damage_Above_Value, //ko nhan dame
    Kill_All_Enemies, //Giet tat ca cac quai
    Kill_Number_Enemies, //Giet bao nhieu quai
    Gain_Number_Combo,//Đạt ? combo
    Have_Character_InTeam,// có nhân vật ? trong team
    Use_Skill_1,// sử dụng skill 1 ? lần
    Use_Skill_2,//sử dụng skill 2 ? lần
    Use_Skill_3,//sử dụng skill 3 ? lần
    WinStage, // Thắng màn
}

public enum QuestStageDataChildType
{
    Number,
    Seconds,//thời gian
    Damage,
    Magic,
    Defend,
    HeroIndex,
}

public enum TypeMode
{
    PVEMode,
    SurvivalMode,
    TrickMode,
    PVPMode,
    TryingHero,
    Ladder,
    RaidMode,
    TeamBattle,
    None
}

public enum DailyRewardStatusType
{
    WAITING = 0,
    CAN_GET = 1,
    RECEIVED = 2
}

public enum MainGroupMoveType
{
    MOVE_TO_RIGHT,
    CENTER,
    MOVE_TO_LEFT
}

public enum RarityType
{
    None,
    Common,
    Epic,
    Legendary
}

public enum StatInfo
{
    Level,
    GloryName,
    EvolutionLevel,
    Damage,
    HP
}