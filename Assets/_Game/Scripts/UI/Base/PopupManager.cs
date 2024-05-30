using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using uGUI.PopUps;

public enum PopupType
{
    None,
    Notification,
    RateGame,
    BigInfo,
    BuyItem,
    SelectLanguage,
    Setting,
    UpdatePopup,
    PowerUpVideo,
    BonusLevelUp,
    QualitySettings,
    ChildNotification,
    SpinPopup,
    ShowItem,
}

public class PopupManager : SingletonMonoDontDestroy<PopupManager>
{
    public PopupType currentPopup = PopupType.None;
    [SerializeField]
    private GameObject toastPanel;
    [SerializeField]
    private ModuleUINotification moduleUINoti;
    [SerializeField]
    private ModuleUINotification moduleUIChildNoti;
    //[SerializeField]
    //private ModuleUIBonusLevelUp moduleUILevelUp;
    [SerializeField]
    private GameObject[] _popUpGameObjects;
    private Dictionary<PopupType, UguiPopUpBase> _popUpDictionary;

    DOTweenAnimation toastAnim;

    public void Init()
    {
        className = "PopupManager";
    }

    private void Awake()
    {
        toastAnim = toastPanel.transform.GetChild(0).GetComponent<DOTweenAnimation>();
        _popUpDictionary = new Dictionary<PopupType, UguiPopUpBase>();
        for (var i = 0; i < _popUpGameObjects.Length; i++)
        {
            var popUpGameObject = _popUpGameObjects[i];
            var popupTypeHandler = popUpGameObject.GetComponent<UguiPopUpBase>();
            if (popupTypeHandler != null)
            {
                var aPopUp = Instantiate(popUpGameObject, Vector3.zero, Quaternion.identity, this.transform);
                var aPopUpHandler = aPopUp.GetComponent<UguiPopUpBase>();
                var rectTransform = aPopUp.GetComponent<RectTransform>();
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                _popUpDictionary.Add(popupTypeHandler.Type, aPopUpHandler);
                aPopUpHandler.HideOnAwake();
            }
        }
    }

    private void OnEnable()
    {
        toastPanel.gameObject.SetActive(false);
        moduleUINoti.gameObject.SetActive(false);
        moduleUIChildNoti.gameObject.SetActive(false);
        //moduleUILevelUp.gameObject.SetActive(false);
    }

    #region NOTIFICATION POPUP
    public void OpenNotification_SetTitle(string title, string content, Action action = null, bool isYesNo = true)
    {
        OpenNotification(content, action, isYesNo);
        moduleUINoti.titleText.text = title;
    }

    public void OpenNotification(string content, Action action = null, bool isYesNo = false, Action actionClose = null)
    {
        if (moduleUINoti.IsOpened)
            return;
        //moduleUINoti.titleText.text = Localize.GetLocalizedString("notification").ToUpper();
        moduleUINoti.contentText.text = content;
        if (action == null)
            moduleUINoti.ClickYes = null;
        else
            moduleUINoti.ClickYes = action;
        if (actionClose == null)
            moduleUINoti.ClickNo = null;
        else
            moduleUINoti.ClickNo = actionClose;
        moduleUINoti.typeNotice = isYesNo ? TypeNotificationWindow.YesNo : TypeNotificationWindow.Normal;
        currentPopup = PopupType.Notification;
        moduleUINoti.transform.SetAsLastSibling();
        moduleUINoti.Init();
    }

    public void OpenChildNoti(string content, Action action = null, bool isYesNo = false)
    {
        StartCoroutine(CrWaitingToHideNoti(content, action, isYesNo));
    }

    IEnumerator CrWaitingToHideNoti(string content, Action action = null, bool isYesNo = false)
    {
        yield return new WaitForSecondsRealtime(moduleUINoti.boardAnim.duration);
        if (moduleUIChildNoti.IsOpened)
            yield return null;
        moduleUIChildNoti.titleText.text = Localize.GetLocalizedString("notification").ToUpper();
        moduleUIChildNoti.contentText.text = content;
        if (action == null)
            moduleUIChildNoti.ClickYes = null;
        else
            moduleUIChildNoti.ClickYes = action;
        moduleUIChildNoti.typeNotice = isYesNo ? TypeNotificationWindow.YesNo : TypeNotificationWindow.Normal;
        currentPopup = PopupType.ChildNotification;
        moduleUIChildNoti.Init();
    }
    #endregion

    #region TOAST PROCESS
    public void OpenToast(string content, float timeDelayToClose = 2f)
    {
        toastPanel.transform.GetComponentInChildren<UnityEngine.UI.Text>().text = content;
     
        if (toastPanel.activeSelf)
        {
            StopAllCoroutines();
            toastAnim.DORestart();
        }
        else
        {
            toastPanel.SetActive(true);
            toastAnim.DORestart();
        }
        StartCoroutine(CrCloseToast(timeDelayToClose));
    }

    private Coroutine CorouCloseToast;
    private IEnumerator CrCloseToast(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        toastAnim.DOPlayBackwards();
        yield return new WaitForSecondsRealtime(toastAnim.duration);
        toastPanel.SetActive(false);
    }

    public void CloseToastImmediate()
    {
        if (CorouCloseToast != null)
            StopCoroutine(CorouCloseToast);
        toastPanel.SetActive(false);
    }
    #endregion

    #region BONUS LEVEL UP
    public void OpenBonusLevelUp(int preLevel,int nextLevel)
    {
        //moduleUILevelUp.Show(preLevel, nextLevel);
        currentPopup = PopupType.BonusLevelUp;
    }
    #endregion

    public void CloseAllPopups()
    {
        switch (currentPopup)
        {
            case PopupType.Notification:
                moduleUINoti.ClickBtnNo();
                break;
            case PopupType.SelectLanguage:
                HidePopup(PopupType.SelectLanguage);
                break;
            case PopupType.ChildNotification:
                moduleUIChildNoti.ClickBtnNo();
                break;
            default:
                HidePopup(currentPopup);
                break;
        }
        currentPopup = PopupType.None;
    }

    public void ShowPopup(PopupType type, params object[] instantiateData)
    {
        UguiPopUpBase popUp;
        if (_popUpDictionary.TryGetValue(type, out popUp))
        {
            if (popUp.gameObject.activeSelf)
                popUp.gameObject.SetActive(true);
            popUp.transform.SetAsLastSibling();
            popUp.Show(instantiateData);
            currentPopup = type;
        }
    }

    public void HidePopup(PopupType type)
    {
        UguiPopUpBase popUp;
        if (_popUpDictionary.TryGetValue(type, out popUp))
        {
            if (popUp.gameObject.activeSelf)
                popUp.Hide();
            currentPopup = PopupType.None; ;
        }
    }
}
