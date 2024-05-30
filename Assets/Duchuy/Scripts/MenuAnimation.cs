using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimation : MonoBehaviour
{
    public static MenuAnimation Instance { get; private set; }

    private bool isOpen = false;
    public List<GameObject> menuItems;
    public GameObject extraPopUp;

    private readonly int OPEN_POSITION = 0;
    private readonly int CLOSE_POSITION = 300;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        isOpen = true;
        OnMenu();
    }
    public void OnMenu()
    {
        if (isOpen && !GameStatics.IsAnimating)
        {
            CloseMenu();
        }
        else if (!isOpen && !GameStatics.IsAnimating)
        {
            OpenMenu();
        }
    }

    void OpenMenu()
    {

        //LeanTween.moveAnchorPositionX(menuItems[0], OPEN_POSITION, 0.5f).setEaseInOutBack();
        //LeanTween.delayedCall(0.15f, () => { LeanTween.moveAnchorPositionX(menuItems[1], OPEN_POSITION, 0.5f).setEaseInOutBack(); });
        //LeanTween.delayedCall(0.3f, () =>
        //{
        //    LeanTween.moveAnchorPositionX(menuItems[2], OPEN_POSITION, 0.5f).setEaseInOutBack();
        //    GameStatics.IsAnimating = false;
        //    isOpen = true;
        //});

        //Minh.ho: Update new Extrapopup
        GameStatics.IsAnimating = true;
        LeanTween.delayedCall(0, () =>
        {
            LeanTween.moveAnchorPositionX(extraPopUp, OPEN_POSITION, 0.5f).setEaseInOutBack();
            GameStatics.IsAnimating = false;
            isOpen = true;
        });
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void CloseMenu()
    {
        //GameStatics.IsAnimating = true;
        //LeanTween.moveAnchorPositionX(menuItems[0], CLOSE_POSITION, 0.5f).setEaseOutBack();
        //LeanTween.delayedCall(0.15f, () => { LeanTween.moveAnchorPositionX(menuItems[1], CLOSE_POSITION, 0.5f).setEaseOutBack(); });
        //LeanTween.delayedCall(0.3f, () =>
        //{
        //    LeanTween.moveAnchorPositionX(menuItems[2], CLOSE_POSITION, 0.5f).setEaseOutBack();
        //    GameStatics.IsAnimating = false;
        //    isOpen = false;
        //});

        //Minh.ho: Update new Extrapopup
        if(!isOpen)
        {
            return;
        }

        GameStatics.IsAnimating = true;
        LeanTween.delayedCall(0, () =>
        {
            LeanTween.moveAnchorPositionX(extraPopUp, CLOSE_POSITION, 0.5f).setEaseInOutBack();
            GameStatics.IsAnimating = false;
            isOpen = false;
        });
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnAchievement()
    {
        if (GameStatics.IsAnimating)
        {
            return;
        }

        CloseMenu();
        PopupDailyMission.Show("Achievement");
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnDailyMission()
    {
        if (GameStatics.IsAnimating)
        {
            return;
        }

        CloseMenu();
        PopupDailyMission.Show("DailyMission");
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnSetting()
    {
        if (!TutMan.IsDone(TutMan.CAN_PRESS_BUTTON_IN_GAMEPLAY))
            return;

        if (GameStatics.IsAnimating)
        {
            return;
        }

        CloseMenu();
        PopupSetting.Show();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
}
