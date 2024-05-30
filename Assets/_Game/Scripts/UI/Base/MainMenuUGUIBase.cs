using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MainMenuPanelType
{
    None,
    TestingPopup,
    SpinScreen,
    MainScreen,
    EquipmentScreen,
    DailyGiftScreen,
    StarterPackPopup,
    WatchVideoPopup,
    GameResultPopup,
    ContinuePopup,
    AchievementScreen,
    UpgradeStatScreen,
    ShopScreen,
    UpgradeHeroScreen,
    UpgradeBowScreen,
    BuyArrowPopup,
}
public abstract class MainMenuUGUIBase : MonoBehaviour {

    protected object[] instantiateData;
    public MainMenuPanelType windowType;
    public bool isCanDestroyOnClose = false;
    [HideInInspector] public List<MainMenuPanelType> originPanelTypes;
    
    #region Custom New Method

    protected virtual void OnCustomAwake() { }

    protected virtual void OnCustomStart() { }

    public virtual void OnCustomShow(params object[] instantiateData) { }

    public virtual void OnCustomHide() { }

    void Awake()
    {
        OnCustomAwake();
    }

    void Start()
    {
        OnCustomStart();
    }

    #endregion

    public MainMenuPanelType originPanelType
    {
        get
        {
            if (originPanelTypes.Count > 0)
            {
                return originPanelTypes[originPanelTypes.Count - 1];
            }
            else
            {
                return MainMenuPanelType.None;
            }
        }

        set
        {
            if (value != MainMenuPanelType.None)
            {
                originPanelTypes.Add(value);
            }
            else
            {
                RemoveParent();
            }
        }
    }

    public void RemoveParent()
    {
        if (originPanelTypes.Count > 0)
        {
            originPanelTypes.RemoveAt(originPanelTypes.Count - 1);
        }
    }

    abstract public bool IsOpened { get; }
    public virtual bool CanClose { get { return true; } }

    public List<Action> listActionCallBackOpenParentAgain = new List<Action>();

    private Action actionCallBackOpenParentAgain
    {
        get
        {
            if (listActionCallBackOpenParentAgain.Count > 0)
            {
                return listActionCallBackOpenParentAgain[listActionCallBackOpenParentAgain.Count - 1];
            }
            return null;
        }
    }

    public void AddCallBack(Action callback)
    {
        listActionCallBackOpenParentAgain.Add(callback);
    }

    public void CallActionOpenParentAgain()
    {
        if (actionCallBackOpenParentAgain != null)
        {
            actionCallBackOpenParentAgain.Invoke();
        }
        if (listActionCallBackOpenParentAgain.Count > 0)
            listActionCallBackOpenParentAgain.RemoveAt(listActionCallBackOpenParentAgain.Count - 1);
    }

    public virtual void Open()
    {

    }

    //public abstract void Open(TypeWindowInMainMenu originType);

    public abstract void Close();
    #region Call Method
    public abstract void OpenPanelOnly();

    public abstract void ClosePanelOnly();

    public abstract void CloseNotPlayAnimation();

    public void OpenPanelParent(params object[] instantiateData)
    {
        RemoveParent();
    }

    public virtual void UpdateLanguage() { }

    public void OpenPanelWithoutCloseParent()
    {
        OpenPanelOnly();
    }

    public void ClosePanelWithoutOpenParent()
    {
        ClosePanelOnly();
    }
    #endregion
}
