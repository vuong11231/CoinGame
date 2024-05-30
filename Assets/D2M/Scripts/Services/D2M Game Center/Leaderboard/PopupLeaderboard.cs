using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum RankType
{
    Daily,
    Weekly,
    Monthly,
    World
}

public class PopupLeaderboard : Popups
{
    static PopupLeaderboard _instance;
    
    public List<Image> listButtonImg;
    public LeaderboardController rankController;

    [Header("User UI")]
    public Text Rank;
    public Text Name;
    public Text Damage;
    public TextMeshProUGUI RankTier;
    public Image Avatar;

    [Header("Data")]
    public Sprite btnOn;
    public Sprite btnOff;

    public static RankType rankType;

    public static void Show()
    {
        if (_instance == null)
        {
            _instance = Instantiate(
                Resources.Load<PopupLeaderboard>("Prefabs/Pop-ups/Rank/Popup Rank"),
                Popups.CanvasPopup.transform,
                false);
        }

        _instance.Init();
        _instance.Appear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.Show_Popup);
    }

    void Init()
    {
        Name.text = DataGameSave.dataServer.Name;
        Avatar.sprite = UserLogin.Profile.Avatar;
    }

    public void UpdateUserInfo(LeaderboardDetailViewRowData data)
    {
        Rank.text = (data.currentPosition + 1).ToString();
        Damage.text = data.currentValue.ToString();
        string[] tmpString = data.type.Split('_');
        RankTier.text = tmpString[0];
    }

    #region Overrive Methods
    public override void Appear()
    {
        if (!GameStatics.IsAnimating)
        {
            base.Appear();
            OnDaily();
            //Animation
            AnimationHelper.AnimatePopupShowScaleHalf
            (
                this,
                Background.GetComponent<Image>(),
                Panel.gameObject,
                Panel,
                PopupConstants.TIME_MULTIPLY_APPEAR
            );

            // Hide banner
            //AdManager.Instance.HideBanner();
        }
    }

    public override void Disappear()
    {
        if (!GameStatics.IsAnimating)
        {
            //Sound
            //EazySoundManager.PlaySound(Sounds.Instance.Btn_Close);

            //Animtion
            AnimationHelper.AnimatePopupCloseScaleHalf
            (
                this,
                Background.GetComponent<Image>(),
                Panel.gameObject,
                Panel,
                PopupConstants.TIME_MULTIPLY_APPEAR,
                () =>
                {
                    base.Disappear();
                }
            );

            // Show banner
            //AdManager.Instance.ShowBanner();
        }
    }

    public override void Disable()
    {
        base.Disable();
        IsShowed = false;

        Background.SetActive(false);
        Panel.gameObject.SetActive(false);
    }

    public override void NextStep(object value = null)
    {
    }
    #endregion

    public void OnDaily()
    {
        rankType = RankType.Daily;
        listButtonImg[0].sprite = btnOn;
        listButtonImg[1].sprite = 
        listButtonImg[2].sprite = 
        listButtonImg[3].sprite = btnOff;
        rankController.SetData();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);

    }

    public void OnWeekly()
    {
        rankType = RankType.Weekly;
        listButtonImg[1].sprite = btnOn;
        listButtonImg[0].sprite =
        listButtonImg[2].sprite =
        listButtonImg[3].sprite = btnOff;
        rankController.SetData();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnMonthly()
    {
        rankType = RankType.Monthly;
        listButtonImg[2].sprite = btnOn;
        listButtonImg[1].sprite =
        listButtonImg[0].sprite =
        listButtonImg[3].sprite = btnOff;
        rankController.SetData();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnWorld()
    {
        rankType = RankType.World;
        listButtonImg[3].sprite = btnOn;
        listButtonImg[1].sprite =
        listButtonImg[2].sprite =
        listButtonImg[0].sprite = btnOff;
        rankController.SetData();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
    public void Onclose()
    {
        if (GameStatics.IsAnimating) return;
        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);

    }
}
