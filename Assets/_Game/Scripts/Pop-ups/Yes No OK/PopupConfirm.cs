using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Hellmade.Sound;
public enum ConfirmType
{
    Ok,
    YesNo,
    YesNoQuit,
    Loading,
}

public class PopupConfirm : Popups
{
    public static PopupConfirm _Instance;

    [Header("UI")]
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtContent;
    public TextMeshProUGUI txtOk;
    public TextMeshProUGUI txtYes;
    public TextMeshProUGUI txtNo;

    public GameObject closeBtn;
    public GameObject OKBtn;
    public GameObject yesBtn;
    public GameObject x2Btn;
    public GameObject loading;

    public Image ImgBtnYes;
    public Sprite SprBtnYes;
    public Sprite SprBtnNotEnough;

    public GameObject tutMeteorPanel = null;
    public Text tutMeteorPanel_NumMatText = null;
    public Text tutMeteorPanel_NumFireText = null;

    public Text numOutsideTxt = null;

    Coroutine ReturnPlanet;
    ConfirmType _type;
    Action _onYes, _onNo, _onOK;
    public Action _onClose;

    #region Get - Set
    public ConfirmType GetConfirmType()
    {
        return _type;
    }
    #endregion

    static void CheckInstance()
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(
                Resources.Load<PopupConfirm>("Prefabs/Pop-ups/Yes No/Popup Confirm"),
                Popups.CanvasPopup.transform,
                false);
        }
    }

    public static void ShowYesNo(string title, string content, Action onYes, string yesBtnTxt = "Yes", Action onNo = null, string noBtnTxt = "No")
    {
        CheckInstance();

        _Instance._type = ConfirmType.YesNo;

        _Instance._onYes = onYes;
        _Instance._onNo = onNo;

        _Instance.yesBtn.SetActive(true);
        _Instance.closeBtn.SetActive(true);
        _Instance.OKBtn.SetActive(false);
        _Instance.x2Btn.SetActive(false);

        _Instance.Init(title, content, yesBtnTxt, noBtnTxt);
        _Instance.Appear();
    }

    public static void ShowTimeDown(long Time, string title, Action onYes, Action onNo = null)
    {
        CheckInstance();

        _Instance._type = ConfirmType.YesNo;

        _Instance._onYes = onYes;
        _Instance._onNo = onNo;

        _Instance.yesBtn.SetActive(true);
        _Instance.closeBtn.SetActive(true);
        _Instance.OKBtn.SetActive(false);
        _Instance.x2Btn.SetActive(false);

        _Instance.txtTitle.text = title;
        _Instance.txtNo.text = "Close";
        _Instance.StartCount(Time);
        _Instance.Appear();

    }
    void StartCount(long Time)
    {
        if (ReturnPlanet != null)
            StopCoroutine(ReturnPlanet);

        ReturnPlanet = StartCoroutine(CountDown(Time));
    }

    public Image diamondImg = null;

    IEnumerator CountDown(long Time)
    {
        long tempMoney = 0;

        while (Time > 0)
        {
            txtContent.text = TextConstants.CountDownPlanet + TimeHelper.ConverSecondtoDate4(Time);
            tempMoney = Time / 60 + 1;
            txtYes.text = tempMoney.ToString() + "   ";
            diamondImg.gameObject.SetActive(true);

            if (DataGameSave.dataLocal.Diamond >= (int)tempMoney)
            {
                ImgBtnYes.sprite = SprBtnYes;
            }
            else
            {
                ImgBtnYes.sprite = SprBtnNotEnough;
            }

            _onYes = () =>
                {
                    if (DataGameSave.dataLocal.Diamond >= (int)tempMoney)
                    {
                        DataGameSave.dataLocal.Diamond -= (int)tempMoney;
                        DataGameSave.dataServer.ListPlanet[SpaceManager.Instance.PlanetSelect.Manager.IndexSpace].ShootTime = DateTime.MinValue;
                        StopCoroutine(ReturnPlanet);
                        _onNo.Invoke();
                        SpaceManager.Instance.PlanetSelect.isActive = true;
                    }
                    else
                    {
                        StopCoroutine(ReturnPlanet);
                        _onNo.Invoke();
                        PopupShop.Show(true);
                    }
                };

            yield return new WaitForSecondsRealtime(1);

            Time--;
        }

        _onClose = () =>
        {
            StopCoroutine(ReturnPlanet);
            _onNo.Invoke();
            SpaceManager.Instance.PlanetSelect.isActive = true;
        };

        Disappear();
    }

    public Image matImg = null;

    public static void ShowMatImg(Sprite spr)
    {
        CheckInstance();
        _Instance.matImg.sprite = spr;
        _Instance.matImg.gameObject.SetActive(true);
    }

    public static void ShowOK(string title, string content, string okBtnTxt = "OK", Action onOK = null)
    {
        CheckInstance();

        _Instance._type = ConfirmType.Ok;

        _Instance._onOK = onOK;

        _Instance.OKBtn.SetActive(true);
        _Instance.yesBtn.SetActive(false);
        _Instance.closeBtn.SetActive(false);
        _Instance.x2Btn.SetActive(false);

        _Instance.Init(title, content, okBtnTxt);
        _Instance.Appear();
    }

    public static void ShowLoading(string title, string content)
    {
        CheckInstance();

        _Instance._type = ConfirmType.Loading;

        _Instance._onOK = null;

        _Instance.OKBtn.SetActive(false);
        _Instance.yesBtn.SetActive(false);
        _Instance.closeBtn.SetActive(false);
        _Instance.x2Btn.SetActive(false);
        _Instance.loading.SetActive(true);

        _Instance.Init(title, content, "");
        _Instance.Appear();
    }

    public static void HideLoading()
    {
        CheckInstance();
        _Instance.loading.SetActive(false);
        _Instance.Disappear();
    }

    public static void ShowReward(string title, string content, string x2BtnTxt = "  x2", Action onYes = null, Action onNo = null)
    {
        CheckInstance();

        _Instance._type = ConfirmType.YesNo;

        _Instance._onYes = onYes;
        _Instance._onNo = onNo;

        _Instance.x2Btn.SetActive(true);
        _Instance.closeBtn.SetActive(true);
        _Instance.OKBtn.SetActive(false);
        _Instance.yesBtn.SetActive(false);

        _Instance.Init(title, content, x2BtnTxt);
        _Instance.Appear();
    }

    void Init(string title, string content, string yesTxt = "Yes", string noTxt = "No") {
        ImgBtnYes.sprite = SprBtnYes;
        txtTitle.text = title;
        txtContent.text = content;
        txtNo.text = noTxt;
        if (_type == ConfirmType.YesNo || _type == ConfirmType.Ok) {
            txtYes.text = yesTxt;
        }
    }

    #region Overrive Methods
    public override void Appear()
    {
        base.Appear();

        if (_Instance._type == ConfirmType.Loading)
        {
            _Instance.loading.SetActive(true);
        } else
        {
            _Instance.loading.SetActive(false);
        }

        matImg.gameObject.SetActive(false);
        numOutsideTxt.gameObject.SetActive(false);
        diamondImg.gameObject.SetActive(false);
        tutMeteorPanel.SetActive(false);

        AnimationHelper.AnimatePopupShowScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_APPEAR);
        //Sound
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

    public void OnYes()
    {
        // Is Animation Activated
        if (GameStatics.IsAnimating)
            return;

        //EazySoundManager.PlayUISound(Sounds.Instance.BtnClicked);
        Sounds.IgnorePopupClose = true;

        //
        _onClose = () =>
        {
            if (_type == ConfirmType.Ok)
            {
                if (_onOK != null)
                    _onOK.Invoke();
            }
            else
            {
                if (_onYes != null)
                    _onYes.Invoke();
            }
        };

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnNo()
    {
        // Is Animation Activated
        if (GameStatics.IsAnimating)
            return;

        //EazySoundManager.PlayUISound(Sounds.Instance.BtnClicked);
        Sounds.IgnorePopupClose = true;

        //
        _onClose = () =>
        {
            if (_onNo != null)
                _onNo.Invoke();
        };

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
}
