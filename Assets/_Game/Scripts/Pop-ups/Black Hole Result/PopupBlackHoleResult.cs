using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Hellmade.Sound;
using TMPro;

public class PopupBlackHoleResult : Popups {

    public static float ROW_HEIGHT = 130;

    public static PopupBlackHoleResult instance;
    public static Action okButtonClick;
    public static DataReward reward;

    public TextMeshProUGUI txtMaterial;
    public TextMeshProUGUI txtAir;
    public TextMeshProUGUI txtAntiMater;
    public TextMeshProUGUI txtFire;
    public TextMeshProUGUI txtGravity;
    public TextMeshProUGUI txtIce;
    public TextMeshProUGUI txtDiamond;
    public TextMeshProUGUI txtUnknown;

    public GameObject material;
    public GameObject air;
    public GameObject antimater;
    public GameObject fire;
    public GameObject gravity;
    public GameObject ice;
    public GameObject diamond;

    public DCamera cameraControl;
    public CameraManager cameraManage;

    public Text title;
    public Text okButtonTitle;
    public RectTransform contentRect;

    public static int rowCount;
    

    static void CheckInstance()
    {
        if (instance == null)
        {
            instance = Instantiate(
            Resources.Load<PopupBlackHoleResult>("Prefabs/Pop-ups/Black Hole Result/Popup BlackHoleResult"),
            Popups.CanvasPopup.transform,
            false);
        }
    }

    public static void Show(string title = "You found", string okButtonTitle = "Great", DataReward reward = null, Action okFunction = null)
    {
        rowCount = 7;
        okButtonClick = okFunction;
        PopupMeteorResult.reward = reward;
        CheckInstance();
        instance.title.text = title;
        instance.okButtonTitle.text = okButtonTitle;

        if (reward != null)
        {
            instance.txtMaterial.text = reward.material.ToString();
            instance.txtAir.text = reward.air.ToString();
            instance.txtAntiMater.text = reward.antimater.ToString();
            instance.txtFire.text = reward.fire.ToString();
            instance.txtGravity.text = reward.gravity.ToString();
            instance.txtIce.text = reward.ice.ToString();            
            instance.txtDiamond.text = reward.diamond.ToString();


            if (reward.material == 0)
            {
                rowCount--;
                instance.material.SetActive(false);
            }  
            if (reward.air == 0)
            {
                rowCount--;
                instance.air.SetActive(false);
            }
            if (reward.antimater == 0)
            {
                rowCount--;
                instance.antimater.SetActive(false);
            }
            if (reward.fire == 0)
            {
                rowCount--;
                instance.fire.SetActive(false);
            }
            if (reward.gravity == 0)
            {
                rowCount--;
                instance.gravity.SetActive(false);
            }
            if (reward.ice == 0)
            {
                rowCount--;
                instance.ice.SetActive(false);
            }
            if (reward.diamond == 0)
            {
                rowCount--;
                instance.diamond.SetActive(false);
            }
        }

        //instance.contentRect.sizeDelta = new Vector2(instance.contentRect.sizeDelta.x, rowCount * ROW_HEIGHT);
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

                if (okButtonClick != null)
                {
                    okButtonClick.Invoke();
                    okButtonClick = null;
                }
            });
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
            return;
        
        Disappear();

        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
}
