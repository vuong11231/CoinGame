using EnhancedUI.EnhancedScroller;
using Hellmade.Sound;
using Lean.Pool;


using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupLeaderboardDetail : Popups
{
    static PopupLeaderboardDetail _instance;

    [SerializeField]
    private Image leaderboardIcon;
    [SerializeField]
    private Text userName;
    [SerializeField]
    private Image userAvatar;
    [SerializeField]
    private Text userLevel;
    [SerializeField]
    private Image expCircle;
    [SerializeField]
    private Text leaderboardTitle;
    [SerializeField]
    private Image yourAvatar;
    [SerializeField]
    private TextMeshProUGUI yourName;
    [SerializeField]
    private TextMeshProUGUI yourValue;
    [SerializeField]
    private TextMeshProUGUI yourRank;
    [SerializeField]
    private LeaderboardDetailController leaderboardDetail;

    [Header("For IphoneX")]
    [SerializeField]
    private RectTransform Top;
    [SerializeField]
    private RectTransform Middle;

    [HideInInspector]
    public Leaderboards type;

    //private Dictionary<Leaderboards, GetLeaderboardResult> resultsDictionary_Global = new Dictionary<Leaderboards, GetLeaderboardResult>();
    //private Dictionary<Leaderboards, GetLeaderboardResult> resultsDictionary_Friend = new Dictionary<Leaderboards, GetLeaderboardResult>();
    float firstPosition = -2200;
    float secondPosition = 0;

    private void Start()
    {
        float aspect = (float)Screen.width / (float)Screen.height;

        // Iphone X and above

        if (SystemInfo.deviceModel == "iPhone10,3" || SystemInfo.deviceModel == "iPhone10,6" // iPhone X
            || SystemInfo.deviceModel == "iPhone11,8" || SystemInfo.deviceModel == "iPhone11,2" // iPhone XR , iPhone XS
            || SystemInfo.deviceModel == "iPhone11,4" || SystemInfo.deviceModel == "iPhone11,6")
        {
            Top.localPosition = new Vector3(Top.localPosition.x, Top.localPosition.y - 80);
            Middle.sizeDelta = new Vector2(Middle.sizeDelta.x, -638.5f);
            Middle.LeanSetLocalPosY(-69.70105f);
        }
    }

    public static void Show(Leaderboards type)
    {
        if (_instance == null)
        {
            _instance = Instantiate(
                Resources.Load<PopupLeaderboardDetail>("Prefabs/Pop-ups/Leaderboard/PopupLeaderboardDetail"),
                Popups.CanvasPopup2.transform,
                false);
        }

        _instance.type = type;
        _instance.Appear();
    }

    public override void Appear()
    {
        if (!GameStatics.IsAnimating)
        {
            base.Appear();
            //Animation
            AnimationHelper.AnimateGameCenterShow
            (
                this,
                Background.GetComponent<Image>(),
                Panel.gameObject,
                Panel,
                firstPosition,
                secondPosition,
                8f,
                () =>
                {
                    UpdatePopupUI();
                    Debug.Log("Thong bao from Duchuy: fix here");
                    //if (PlayFabClientAPI.IsClientLoggedIn())
                    //{
                    //    UpdatePopupUI();
                    //    //
                    //    //if (FacebookManager.IsLoggedIn())
                    //    //    leaderboardDetail.GetListData(false);
                    //    //else
                    //        //leaderboardDetail.GetListData(true);
                    //}
                }
            );
        }
    }

    public override void Disappear()
    {
        if (!GameStatics.IsAnimating)
        {
            //Sound
            //EazySoundManager.PlaySound(Sounds.Instance.Btn_Close);

            //Animtion
            AnimationHelper.AnimateGameCenterClose
            (
                this,
                Background.GetComponent<Image>(),
                Panel.gameObject,
                Panel,
                firstPosition,
                8f,
                () =>
                {
                    Popups.DisappearIgnoreNextAppear = true;
                    base.Disappear();
                }
            );
        }
    }

    public override void Disable()
    {
    }

    public override void NextStep(object value = null)
    {
    }

    private void UpdatePopupUI()
    {
        //leaderboardIcon.sprite = PlayfabManager.Instance.leaderboards[(int)type].Icon;
        //leaderboardTitle.text = PlayfabManager.Instance.leaderboards[(int)type].Title;
        //
        //userLevel.text = DataGameSave.data.achievementLevel.ToString();
        //expCircle.fillAmount = (float)DataGameSave.data.currentAchievementExp / DataGameSave.data.maxAchievementExp;
        yourValue.text = GetCurrentValue(type);
        GetYourLeaderboardsRank(type);

        // Top bar
        if (UserLogin.Profile.Avatar != null) userAvatar.sprite = UserLogin.Profile.Avatar;
        if (!string.IsNullOrEmpty(UserLogin.Profile.Username))
            userName.text = UserLogin.Profile.Username;

        // Bot bar
        if (UserLogin.Profile.Avatar != null) yourAvatar.sprite = UserLogin.Profile.Avatar;
        if (!string.IsNullOrEmpty(UserLogin.Profile.Username))
            yourName.text = UserLogin.Profile.Username;
        else
            yourName.text = GameConstants.DEFAULT_USER_NAME;
    }

    private string GetCurrentValue(Leaderboards type)
    {
        string result = "";

        //switch (type)
        //{
        //    case Leaderboards.Star_Tournament:
        //        result = DataGameSave.data.Star.ToString() + " Stars";
        //        break;
        //    case Leaderboards.Top_Player:
        //        result = "Unlock " + DataGameSave.data.MaxLevelUnlock.ToString() + " Levels";
        //        break;
        //    default:
        //        result = "";
        //        break;
        //}

        return result;
    }

    private void GetLeaderboardAndHandle(Leaderboards type, bool is_global)
    {
        Debug.Log("Thong bao from Duchuy: leaderboard get!");
        //if (is_global)
        //{
        //    PlayfabManager.GetLeaderboard(type.ToString(),
        //        (result) =>
        //        {
        //            resultsDictionary_Global[type] = result;
        //            SetRankOverallText(type, is_global);
        //        },
        //        (error) =>
        //        {
        //            Debug.LogError("GetLeaderboardAndHandle (Global) fail! " + error.GenerateErrorReport());

        //            resultsDictionary_Global[type] = null;
        //            SetRankOverallText(type, is_global);
        //        });
        //}
        //else
        //{
        //    PlayfabManager.GetFriendLeaderboard(type.ToString(),
        //       (result) =>
        //       {
        //           resultsDictionary_Friend[type] = result;
        //           SetRankOverallText(type, is_global);
        //       },
        //        (error) =>
        //        {
        //            Debug.LogError("GetLeaderboardAndHandle (Friend) fail! " + error.GenerateErrorReport());

        //            resultsDictionary_Friend[type] = null;
        //            SetRankOverallText(type, is_global);
        //        });
        //}
    }

    private void GetYourLeaderboardsRank(Leaderboards type)
    {
        if (FacebookManager.IsLoggedIn())
            GetLeaderboardAndHandle(type, false);
        else
            GetLeaderboardAndHandle(type, true);
    }

    private void SetRankOverallText(Leaderboards type, bool isGlobal)
    {
        Debug.Log("THong bao from Duchuy: set overral rank text!");
        //    int position = -1;
        //    GetLeaderboardResult result = null;

        //    // Calc the position

        //    if (isGlobal)
        //        result = resultsDictionary_Global.ContainsKey(type) ? resultsDictionary_Global[type] : null;
        //    else
        //        result = resultsDictionary_Friend.ContainsKey(type) ? resultsDictionary_Friend[type] : null;

        //    if (result == null || result.Leaderboard == null || result.Leaderboard.Count == 0)
        //        position = -1;
        //    else
        //    {
        //        var player = result.Leaderboard.Find(x => x.PlayFabId == PlayfabManager.PlayFabID);

        //        if (player != null)
        //            position = player.Position;
        //        else
        //            position = -2;
        //    }

        //    // Show text
        //    if (position == -1)
        //    {
        //        Debug.LogError("This leaderboard is empty!");
        //        yourRank.text = "";
        //    }
        //    else if (position == -2)
        //    {
        //        Debug.LogError("You have not been in this leaderboard!");
        //        yourRank.text = "";
        //    }
        //    else if (position == 0)
        //    {
        //        yourRank.text = "<sprite=\"crown_icon\" index=0 color=#ffba00>#" + (position + 1);
        //    }
        //    else if (position == 1)
        //    {
        //        yourRank.text = "<sprite=\"crown_icon\" index=0 color=#949494>#" + (position + 1);
        //    }
        //    else if (position == 2)
        //    {
        //        yourRank.text = "<sprite=\"crown_icon\" index=0 color=#ff6c00>#" + (position + 1);
        //    }
        //    else
        //    {
        //        yourRank.text = "#" + (position + 1);
        //    }
    }
}
