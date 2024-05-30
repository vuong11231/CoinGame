using SteveRogers;
using System.Collections.Generic;

public static class TextConstants
{
    //// Quit Game
    //public static string QuitGameTitle = "Quit Game";
    //public static string QuitGameContent = "Are you sure to quit?";

    // Ads - No Video
    public static string AdNoVideoTitle
    {
        get
        {
            return TextMan.Get("NO VIDEOS");
        }
    }

    public static string AdNoVideoContent
    {
        get
        {
            return TextMan.Get("There are no videos right now, please try later :("); 
        }
    }

    //// Missing Internet - Rewarded video, Leaderboard
    //public static string ToastInternetMissing = "Internet Needed!";    
        //public static string MissingInternetContent_Leaderboard = "To view <color=#239619>Leaderboard</color>, Please make sure you have <color=#356FE3>Internet Connection</color> :(";
    //public static string NeedInternet = "Bat mang len de xu dung chuc nang";
    
    #region PlayerPrefs
    public static string Login = "Login";
    public static string IsTutorial = "IsTutorial";
    #endregion

    #region Icon Meteorite
    public static string M_Mater = "<sprite=\"meteorite\" index=5>  ";
    public static string M_Air = " <sprite=\"meteorite\" index=4>  ";
    public static string M_Fire = " <sprite=\"meteorite\" index=0>  ";
    public static string M_Ice = " <sprite=\"meteorite\" index=2>  ";
    public static string M_Gravity = " <sprite=\"meteorite\" index=1>  ";
    public static string M_Antimatter = " <sprite=\"meteorite\" index=3>  ";
    public static string M_Money = " <sprite=\"diamond\" index=0>";
    #endregion

    #region Content Planet
    public static string ContentAir = "Transform into Air Planet, increasing extra 10% size and decreasing incoming damage.";
    public static string ContentFire = "Transform into Fire Planet, increasing extra 150% damage. Strong against Air Planet.";
    public static string ContentIce = "Transform into Ice Planet, decreasing 55% enemy's speed.";
    public static string ContentGravity = "Transform into Gravity Planet, increasing gravitational force and radius as well as attracted material.";
    public static string ContentAntimatter = "Transform into Antimatter Planet, increasing explosion radius and destroy any enemy planet in range.";
    #endregion

    #region content Popup
    public static string CountDownPlanet = "Time to wait for the planet to recover:\n";
    #endregion

    public static string MaterialLose = "Total material lose: ";

    // Logging
    public static string LOGGING_IN = "Logging in...";
    public static string LOADING_PLAYER_DATA = "Loading player's data...";
    public static string LOADING_BATTLE_INFORMATION = "Loading battle's information...";
    public static string LOADING_FRIEND_LIST = "Loading friend list...";
    public static string LOADING_DONE = "Loading done...";

    //public static string TERM_SERVICE = "By clicking \"Start\" you consent to and will comply with the <link='https://www.google.com/'>TERM OF SERVICE</link> and <link='https://www.google.com/'>PRIVACY POLITY</link>";
    public static string NO_STONE = "You don't have any stone of this kind";
    public static string TERM_SERVICE = "By clicking \"Start\" you consent to and will comply with the <link='https://www.google.com/'>TERM OF SERVICE</link> and <link='https://www.google.com/'>PRIVACY POLITY</link>";


    // Attack
    public static string NO_PLANET
    {
        get
        {
            return TextMan.Get("No Planet");
        }
    }
     
    public static string NO_PLANET_MESSAGE
    {
        get
        {
            return TextMan.Get("All your planets are destroyed, please wait until they recover!");
        }
    }
     

    // Friend
    public static string FRIEND = TextMan.Get("Friend");
    public static string FRIEND_EXIST = " " + TextMan.Get("is already in your friend list");
    public static string FRIEND_ADDED = " " + TextMan.Get("is added to your friend list");
    public static string FRIEND_ERROR = TextMan.Get("Something wrong happened");


}
