using Hellmade.Sound;
using Newtonsoft.Json;
using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMailbox : Popups {
    static PopupMailbox _Instance;

    public GameObject mailItem;
    public Transform content;
    public static List<string> mailReads;

    Action _onClose;

    public static void Show() {
        if (_Instance == null) {
            _Instance = Instantiate(
            Resources.Load<PopupMailbox>("Prefabs/Pop-ups/Mailbox/Popup Mail"),
            Popups.CanvasPopup.transform,
            false);
        }

        _Instance.Appear();
    }

    public static void Hide(){
        _Instance.Disappear();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick); _Instance.Onclose();
    }

    #region Overrive Methods
    public override void Appear()
    {
        base.Appear();

        foreach (Transform child in content) {
            Destroy(child.gameObject);
        }

        mailReads = DataGameSave.GetMailReads();

        foreach (var mail in DataGameSave.mails) {

            if (mail.targetid != null && mail.targetid != "") {
                string[] targets = mail.targetid.Split(',');
                bool canReceiveMail = new List<string>(targets).Contains(DataGameSave.dataServer.userid.ToString());
                if (!canReceiveMail) {
                    continue;
                }
            }

            GameObject go = Instantiate(mailItem, content);
            go.GetComponent<MailItem>().SetData(mail, mailReads.Contains(mail.id));
        }

        AnimationHelper.AnimatePopupShowScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_APPEAR);
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.Show_Popup);
    }



    public static void ReadMail(string id) {
        if (!mailReads.Contains(id)) {
            mailReads.Add(id);
        }
        DataGameSave.SaveMetaData(MetaDataKey.MAIL_READ, JsonConvert.SerializeObject(mailReads));
        Debug.Log(DataGameSave.metaData);
    }

    public override void Disappear()
    {
        AnimationHelper.AnimatePopupCloseScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_DISAPPEAR,
            () =>
            {
                base.Disappear();
                if (_onClose != null)
                {
                    _onClose.Invoke();
                    _onClose = null;
                }
            });
    }

    public override void Disable()
    {
        base.Disable();

    }

    public override void NextStep(object value = null)
    {

    }
    #endregion

    public void OnBattleHistory()
    {
        if (GameStatics.IsAnimating)
            return;

        _onClose = () =>
        {
            PopupOfflineHistory.Show();
        };

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnRankReward()
    {
        if (GameStatics.IsAnimating)
            return;

        _onClose = () =>
        {
            PopupRankReward.Show();
        };

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void Onclose()
    {
        if (GameStatics.IsAnimating)
            return;

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
}
