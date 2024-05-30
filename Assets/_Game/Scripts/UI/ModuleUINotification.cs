using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using TMPro;

public class ModuleUINotification : AnimatorUGUIPanel {
    public DG.Tweening.DOTweenAnimation boardAnim;
    public Text titleText;
    public TextMeshProUGUI contentText;
    public GameObject yesNoBar, reverseYesNoBar;
    public TypeNotificationWindow typeNotice;
    public Button okBtn;
    public Action ClickYes;
    public Action ClickNo;

    protected override void OnCustomStart()
    {
        okBtn.OnClickAsObservable().Subscribe(_ =>
        {
            ClickBtnYes();
        });
    }

    public void Init()
    {
        gameObject.SetActive(true);
        //SoundManager.Instance.PlayAudioSE(TypeSoundSE.ShowPopUp);
        boardAnim.DORestart();
        switch (typeNotice)
        {
            case TypeNotificationWindow.Normal:
                okBtn.gameObject.SetActive(true);
                yesNoBar.SetActive(false);
                reverseYesNoBar.SetActive(false);
                break;
            case TypeNotificationWindow.YesNo:
                yesNoBar.SetActive(true);
                reverseYesNoBar.SetActive(false);
                okBtn.gameObject.SetActive(false);
                break;
        }
    }

    public void CloseNoti()
    {
        if (typeNotice == TypeNotificationWindow.YesNo)
        {
            ClickBtnNo();
        }
        else
        {
            ClickBtnYes();
        }
    }

    public void ClickBtnNo()
    {
        //SoundManager.Instance.PlayAudioSE(TypeSoundSE.ClickButton);
        StartCoroutine(CrHide());
        if (ClickNo != null)
        {
            ClickNo();
            ClickNo = null;
        }
    }

    public void ClickBtnYes()
    {
        //SoundManager.Instance.PlayAudioSE(TypeSoundSE.ClickButton);
        StartCoroutine(CrHide());
        if (ClickYes != null)
        {
            ClickYes();
        }
    }

    IEnumerator CrHide()
    {
        boardAnim.DOPlayBackwards();
        //SoundManager.Instance.PlayAudioSE(TypeSoundSE.HidePopUp);
        yield return new WaitForSecondsRealtime(boardAnim.duration);
        PopupManager.Instance.currentPopup = PopupType.None;
        gameObject.SetActive(false);
    }
}
