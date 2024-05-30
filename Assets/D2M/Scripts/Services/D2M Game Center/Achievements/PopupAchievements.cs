using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hellmade.Sound;

public class PopupAchievements : Popups
{
    static PopupAchievements _instance;

    [Header("UI")]
    [SerializeField]
    private AchievementController scroller;

    public static void Show()
    {
        if (_instance == null)
        {
            _instance = Instantiate(
                Resources.Load<PopupAchievements>("Prefabs/Pop-ups/Achievement/Popup Achievements"),
                Popups.CanvasPopup.transform,
                false);
        }

        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.Show_Popup);
        _instance.Appear();
    }

    #region Override Methods
    public override void Appear()
    {
        if (!GameStatics.IsAnimating)
        {
            base.Appear();
            scroller.myScroller.JumpToDataIndex(0);
            scroller.myScroller.ReloadData();

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
                PopupConstants.TIME_MULTIPLY_DISAPPEAR,
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
        IsShowed = false;

        Background.SetActive(false);
        Panel.gameObject.SetActive(false);
    }

    public override void NextStep(object value = null)
    {
    }
    #endregion
}
