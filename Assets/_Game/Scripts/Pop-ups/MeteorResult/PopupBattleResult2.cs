using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Hellmade.Sound;
using TMPro;
using SteveRogers;

public class PopupBattleResult2 : Popups {

    public static float ROW_HEIGHT = 130;

    public static PopupBattleResult2 instance;
    public static Action okButtonClick;
    public static DataReward reward;

    public static bool watchAdsX2 = false;

    public TextMeshProUGUI txtMaterial;
    public TextMeshProUGUI txtAir;
    public TextMeshProUGUI txtAntiMater;
    public TextMeshProUGUI txtFire;
    public TextMeshProUGUI txtGravity;
    public TextMeshProUGUI txtIce;
    public TextMeshProUGUI txtDiamond;
    public TextMeshProUGUI txtUnknown;

    public GameObject btnWatchAdsX2;

    public GameObject material;
    public GameObject air;
    public GameObject antimater;
    public GameObject fire;
    public GameObject gravity;
    public GameObject ice;
    public GameObject diamond;

    public Transform[] skinTrans = null;
    public GridLayoutGroup gridLayoutGroup = null;

    public Image airImage;
    public Image antimaterImage;
    public Image fireImage;
    public Image gravityImage;
    public Image iceImage;

    public Text title;
    public Text okButtonTitle;
   
    public RectTransform contentRect;
    public ParticleSystem effectShowWonder;
    

    static void CheckInstance()
    {
        if (instance == null)
        {
            instance = Instantiate(
            Resources.Load<PopupBattleResult2>("Prefabs/Pop-ups/Meteor Result/Popup BattleResult"),
            Popups.CanvasPopup.transform,
            false);
        }
    }

    public static void Show(string title = "You found", string okButtonTitle = "Great",  Dictionary<PLanetName, int>  dic = null, Action okFunction = null)
    {
        Debug.Log("THIS IS POPUP BATTLE RESULT SHOW");
        CheckInstance();
        okButtonClick = okFunction;
        instance.title.text = title;
        instance.okButtonTitle.text = okButtonTitle;

        //if (reward != null) {
        //    instance.btnWatchAdsX2.SetActive(reward.material > 50000 && showWatchAdsX2Button);
        //}
        instance.btnWatchAdsX2.SetActive(false);
        watchAdsX2 = false;

        instance.gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        instance.gridLayoutGroup.constraintCount = 5;

        instance.fire.SetActive(false);
        instance.air.SetActive(false);
        instance.antimater.SetActive(false);
        instance.gravity.SetActive(false);
        instance.ice.SetActive(false);

        instance.material.SetActive(false);
        instance.diamond.SetActive(false);
        instance.PLayEffect();


        foreach (Transform i in instance.skinTrans)
            i.gameObject.SetActive(false);

        var index = 0;

        foreach (var i in dic)
        {
            instance.skinTrans[index].GetChild(0).GetComponent<TextMeshProUGUI>().text = i.Value.ToString();
            instance.skinTrans[index].GetChild(1).GetComponent<Image>().sprite = DataPrefab.Instance.skinSprites[(int)i.Key];
            instance.skinTrans[index].gameObject.SetActive(true);
            index++;
        }

        instance.Appear();
    }

    public void WatchAdsX2Reward()
    {
        // cho quang cao vao trong khu vuc nay 
        GoogleMobileAdsManager.Instance.ShowRewardedVideo(() =>
        {
            Debug.Log("WATCH ADS BATTLE x2 TRUE");
            watchAdsX2 = true;
            okButtonClick.Invoke();
        });
    }
    public void PLayEffect()
    {
        effectShowWonder.gameObject.SetActive(true);
    }

    public static void Show(string title = "You found", string okButtonTitle = "Great", DataReward reward = null, Action okFunction = null, bool showButtonWatchAds = true)
    {
        watchAdsX2 = false;
        CheckInstance();

        foreach (Transform i in instance.skinTrans)
            i.gameObject.SetActive(false);

        instance.gridLayoutGroup.constraint = GridLayoutGroup.Constraint.Flexible;

        okButtonClick = okFunction;
        PopupBattleResult2.reward = reward;
        instance.title.text = title;
        instance.okButtonTitle.text = okButtonTitle;
        instance.btnWatchAdsX2.SetActive(reward.material > 50000 && showButtonWatchAds);

        if (reward != null)
        {
            instance.txtMaterial.text = reward.material.ToString();
            instance.txtAir.text = reward.air.ToString();
            instance.txtAntiMater.text = reward.antimater.ToString();
            instance.txtFire.text = reward.fire.ToString();
            instance.txtGravity.text = reward.gravity.ToString();
            instance.txtIce.text = reward.ice.ToString();            
            instance.txtDiamond.text = reward.diamond.ToString();

            instance.fire.SetActive(reward.fire > 0);
            instance.air.SetActive(reward.air > 0);
            instance.antimater.SetActive(reward.antimater > 0);
            instance.gravity.SetActive(reward.gravity > 0);
            instance.ice.SetActive(reward.ice > 0);

            instance.material.SetActive(reward.material > 0);
            instance.diamond.SetActive(reward.diamond > 0);

            if (reward.isEffecStones)
            {
                instance.airImage.sprite = DataPrefab.GetSpriteStone(true, TypePlanet.Air);
                instance.antimaterImage.sprite = DataPrefab.GetSpriteStone(true, TypePlanet.Antimatter);
                instance.iceImage.sprite = DataPrefab.GetSpriteStone(true, TypePlanet.Ice);
                instance.fireImage.sprite = DataPrefab.GetSpriteStone(true, TypePlanet.Fire);
                instance.gravityImage.sprite = DataPrefab.GetSpriteStone(true, TypePlanet.Gravity);
            }
            else 
            {
                instance.airImage.sprite = DataPrefab.GetSpriteStone(false, TypePlanet.Air);
                instance.antimaterImage.sprite = DataPrefab.GetSpriteStone(false, TypePlanet.Antimatter);
                instance.iceImage.sprite = DataPrefab.GetSpriteStone(false, TypePlanet.Ice);
                instance.fireImage.sprite = DataPrefab.GetSpriteStone(false, TypePlanet.Fire);
                instance.gravityImage.sprite = DataPrefab.GetSpriteStone(false, TypePlanet.Gravity);
            }
        }

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
        effectShowWonder.gameObject.SetActive(false);
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

