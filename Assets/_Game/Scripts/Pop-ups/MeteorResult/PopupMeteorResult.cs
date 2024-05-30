using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Hellmade.Sound;
using TMPro;
using SteveRogers;

public class PopupMeteorResult : Popups {

    public static float ROW_HEIGHT = 130;

    public static PopupMeteorResult instance;
    public static Action okButtonClick;
    public static DataReward reward;
    public static bool meteorWatchAdsX2 = false;

    public TextMeshProUGUI txtMaterial;
    public TextMeshProUGUI txtAir;
    public TextMeshProUGUI txtAntiMater;
    public TextMeshProUGUI txtFire;
    public TextMeshProUGUI txtGravity;
    public TextMeshProUGUI txtIce;
    public TextMeshProUGUI txtDiamond;
    public TextMeshProUGUI txtUnknown;

    public TextMeshProUGUI txtToy1;
    public TextMeshProUGUI txtToy2;
    public TextMeshProUGUI txtToy3;
    public TextMeshProUGUI txtToy4;
    public TextMeshProUGUI txtToy5;

    public GameObject material;
    public GameObject air;
    public GameObject antimater;
    public GameObject fire;
    public GameObject gravity;
    public GameObject ice;
    public GameObject diamond;

    public GameObject btnWatchAdsX2;

    public GameObject toy1;
    public GameObject toy2;
    public GameObject toy3;
    public GameObject toy4;
    public GameObject toy5;

    public DCamera cameraControl;
    public CameraManager cameraManage;

    public Text title;
    public Text okButtonTitle;
    public RectTransform contentRect;
    
    static void CheckInstance()
    {
        if (instance == null)
        {
            instance = Instantiate(
            Resources.Load<PopupMeteorResult>("Prefabs/Pop-ups/Meteor Result/Popup MeteorResult"),
            Popups.CanvasPopup.transform,
            false);
        }
    }

    public static void Show(string title = "You found", string okButtonTitle = "Great", DataReward reward = null, Action okFunction = null, bool showButtonWatchAds = true)
    {
        meteorWatchAdsX2 = false;
        okButtonClick = okFunction;
        PopupMeteorResult.reward = reward;
        CheckInstance();
        instance.title.text = title;
        instance.okButtonTitle.text = okButtonTitle;
        instance.btnWatchAdsX2.SetActive(reward.material > 50000 && showButtonWatchAds && Scenes.Current != SceneName.BattlePass);

        if (reward != null)
        {
            instance.txtMaterial.text = reward.material.ToString();
            instance.txtAir.text = reward.air.ToString();
            instance.txtAntiMater.text = reward.antimater.ToString();
            instance.txtFire.text = reward.fire.ToString();
            instance.txtGravity.text = reward.gravity.ToString();
            instance.txtIce.text = reward.ice.ToString();            
            instance.txtDiamond.text = reward.diamond.ToString();

            instance.txtToy1.text = reward.toy1.ToString();
            instance.txtToy2.text = reward.toy2.ToString();
            instance.txtToy3.text = reward.toy3.ToString();
            instance.txtToy4.text = reward.toy4.ToString();
            instance.txtToy5.text = reward.toy5.ToString();

            instance.material.SetActive(reward.material > 0);
            instance.air.SetActive(reward.air > 0);
            instance.antimater.SetActive(reward.antimater > 0);
            instance.fire.SetActive(reward.fire > 0);
            instance.ice.SetActive(reward.ice > 0);
            instance.gravity.SetActive(reward.gravity > 0);
            instance.diamond.SetActive(reward.diamond > 0);

            instance.toy1.SetActive(reward.toy1 > 0);
            instance.toy2.SetActive(reward.toy2 > 0);
            instance.toy3.SetActive(reward.toy3 > 0);
            instance.toy4.SetActive(reward.toy4 > 0);
            instance.toy5.SetActive(reward.toy5 > 0);
        }

        instance.Appear();
    }
    public void WatchAdsX2Reward()
    {
        // cho quang cao vao trong khu vuc nay 
        GoogleMobileAdsManager.Instance.ShowRewardedVideo(() =>
        {
            meteorWatchAdsX2 = true;
            okButtonClick.Invoke();
        });
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
