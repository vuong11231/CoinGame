using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hellmade.Sound;
using SteveRogers;

public class PopupAttackedInfo : Popups {
    public static PopupAttackedInfo _Instance;
    public AttackedInfoScroller scroller = null;

    public static List<AttackedInfoData> data = null;
    public Sprite matImg;

    void SetData() {
        scroller.Set(data);
    }

    private static void CheckInstance() {
        if (_Instance == null) {
            _Instance = Instantiate(
            Resources.Load<PopupAttackedInfo>("Prefabs/Pop-ups/AttackedInfo/AttackedInfo"),
            Popups.CanvasPopup.transform,
            false);
        }
    }

    public static void Show(List<AttackedInfoData> data) {
        PopupAttackedInfo.data = data;
        CheckInstance();
        _Instance.Appear();
    }

    public override void Appear() {
        base.Appear();
        SetData();
        AnimationHelper.AnimatePopupShowScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_APPEAR);
        EazySoundManager.PlaySound(Sounds.Instance.Show_Popup);
    }

    public override void Disappear() {
        AnimationHelper.AnimatePopupCloseScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_DISAPPEAR,
            () => {
                base.Disappear();
            });

        //if (data == null || data.Count == 0) {
        //    return;
        //}

        //int totalLost = 0;
        //foreach (AttackedInfoData attack in data) {
        //    totalLost += attack.mat;
        //}
        //PopupConfirm.ShowOK(TextMan.Get("Informantion"), TextMan.Get("Total lost") + "\n" + totalLost);
        //PopupConfirm.ShowMatImg(matImg);
        //DataGameSave.dataLocal.M_Material -= totalLost;
        //if (DataGameSave.dataLocal.M_Material < 0)
        //    DataGameSave.dataLocal.M_Material = 0;
        //DataGameSave.SaveToServer();
    }

    public void OnClearAttackInfo() {
        DataGameSave.dataServer.isAttackedCode = "";

        WWWForm form = new WWWForm();
        form.AddField("user_id", DataGameSave.dataServer.userid);
        form.AddField("id", -1);
        form.AddField("name", "");
        form.AddField("mat", -1);

        ServerSystem.Instance.SendRequest(ServerConstants.UPDATE_IS_ATTACKED_CODE, form, null);
        DataGameSave.SaveToServer();
        OnClose();
    }

    public void OnClose()
    {
        if (GameStatics.IsAnimating)
            return;
      
        // close

        Disappear();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public override void NextStep(object value = null)
    {
        _Instance.NextStep(value);
    }
}
