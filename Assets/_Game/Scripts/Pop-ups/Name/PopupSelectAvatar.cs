using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Hellmade.Sound;
using SteveRogers;

public class PopupSelectAvatar : Popups
{
    public static PopupSelectAvatar instance;
    public Image mainAvatar;
    public GameObject holderAvatarCell;
    public GameObject oneAvatarCell;
    public static Action onSelectedAvatar = null;
    private List<AvatarCell> listScriptAvatarCell = new List<AvatarCell>();

    static void CheckInstance()
    {
        if (instance == null)
        {
            instance = Instantiate(
            Resources.Load<PopupSelectAvatar>("Prefabs/Pop-ups/Name/Popup Select Avatar"),
            Popups.CanvasPopup.transform,
            false);
        }
    }
    public static void Show(Action onSelected = null)
    {
        if(onSelected !=null)
        {
            onSelectedAvatar = onSelected;
        }
        CheckInstance();
        instance.Appear();
    }


    public override void Appear()
    {
        base.Appear();
        
        AnimationHelper.AnimatePopupShowScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_APPEAR);
        EazySoundManager.PlaySound(Sounds.Instance.Show_Popup);
        InitAvatarCell();
    }

    public override void Disappear()
    {
        AnimationHelper.AnimatePopupCloseScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            5,
            () =>
            {
                base.Disappear();
            });
    }

    public void InitAvatarCell()
    {
        if (GameManager.Instance.listAvatar == null)
        {
            return;
        }
        mainAvatar.sprite = GameManager.Instance.listAvatar[DataGameSave.dataServer.rankChartId];
        listScriptAvatarCell.Clear();
        for (int i = 0; i < GameManager.Instance.listAvatar.Count; i++)
        {
            GameObject oneCell = Instantiate(oneAvatarCell, holderAvatarCell.transform);
            oneCell.SetActive(true);
            var scriptOneCell = oneCell.GetComponent<AvatarCell>();
            scriptOneCell.SetData(i, GameManager.Instance.listAvatar[i],UpdateCurrentAvatar);
            listScriptAvatarCell.Add(scriptOneCell);
        }
    } 
    
    public void UpdateCurrentAvatar(int index)
    {
        foreach (AvatarCell item in listScriptAvatarCell)
        {
            if(item.indexThisAvatar == index)
            {
                item.tick.SetActive(true);
            }
            else
            {
                item.tick.SetActive(false);
            }
        }
        mainAvatar.sprite = GameManager.Instance.listAvatar[index];
    }
    
    public override void NextStep(object value = null)
    {

    }

    public void OnClose()
    {
        if (GameStatics.IsAnimating)
            return;
        Disappear();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public virtual void OnOk()
    {
        if (GameStatics.IsAnimating)
        {
            return;
        }

        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        onSelectedAvatar?.Invoke();

        if (!TutMan.IsDone(TutMan.TUT_KEY_01_HIDE_GO_WHEN_FIRST_ENTER_GAME))  //is tutorial
        {
            Utilities.ActiveEventSystem = false;
            TutGameplayScene.OnRenameAndSelectAvatarDone();
        }
        else
        {
            Utilities.ActiveEventSystem = true;
        }

        Disappear();
    }
}
