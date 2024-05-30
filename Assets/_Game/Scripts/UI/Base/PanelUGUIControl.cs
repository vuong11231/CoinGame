using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PanelUGUIControl : MonoBehaviour {
    public MainMenuPanelType currentWindow = MainMenuPanelType.None;
    public MainMenuPanelType originWindow = MainMenuPanelType.None;
    public List<MainMenuUGUIBase> allPanelInMainMenu;

    public Dictionary<MainMenuPanelType, MainMenuUGUIBase> allController
    {
        get
        {
            if (_allControllerDict == null)
            {
                _allControllerDict = new Dictionary<MainMenuPanelType, MainMenuUGUIBase>();
                for (int i = 0; i < allPanelInMainMenu.Count; i++)
                {
                    _allControllerDict.Add(allPanelInMainMenu[i].windowType, allPanelInMainMenu[i]);
                }
            }
            return _allControllerDict;
        }
    }
    private Dictionary<MainMenuPanelType, MainMenuUGUIBase> _allControllerDict;

    public List<MainMenuUGUIBase> allPanelMainMenuPrefabs;
    #region Base Method

    protected void OpenPopup(MainMenuUGUIBase panelOpen, params object[] instantiateData)
    {
        //originWindow = currentWindow;
        if (panelOpen != null)
        {
            if (!panelOpen.gameObject.activeInHierarchy)
            {
                panelOpen.gameObject.SetActive(true);
            }

            currentWindow = panelOpen.windowType;
            panelOpen.gameObject.SetActive(true);
            panelOpen.Open();
            originWindow = panelOpen.originPanelType;
            panelOpen.OnCustomShow(instantiateData);
        }
        else
        {
            currentWindow = originWindow = MainMenuPanelType.None;
        }
    }

    protected void ClosePanel(MainMenuUGUIBase panelOpen, bool imidiate = false)
    {
        if (panelOpen != null)
        {

            if (imidiate)
            {
                panelOpen.CloseNotPlayAnimation();
            }
            else
            {
                panelOpen.ClosePanelOnly();
            }
            panelOpen.OnCustomHide();
        }
    }

    private MainMenuUGUIBase GetPanelPopup(MainMenuPanelType type)
    {
        if (type == MainMenuPanelType.None)
        {
            return null;
        }

        if (allController.ContainsKey(type) && allController[type] != null)
        {
            if (!allController[type].gameObject.activeInHierarchy)
            {
                allController[type].gameObject.SetActive(true);
            }
            return allController[type];
        }

        var result = LoadFromResources(type);
        return result;
    }

    private MainMenuUGUIBase LoadFromResources(MainMenuPanelType type)
    {
        try
        {
            MainMenuUGUIBase obj = null;
            for (int i = 0; i < allPanelMainMenuPrefabs.Count; i++)
            {
                if (allPanelMainMenuPrefabs[i].windowType.Equals(type))
                {
                    obj = allPanelMainMenuPrefabs[i];
                }
            }

            if (obj == null)
            {
                obj = Resources.Load<MainMenuUGUIBase>(UtilityGame.Format("UI/{0}", type));
            }

            if (obj != null)
            {
                var result = Instantiate(obj);
                //test
                //if (SceneManager.Instance.CurrentScene == NameScene.MainMenu.ToString())
                //    result.transform.parent = SceneMain.instance.mainMenuCanvas.transform;


                //else
                //    result.transform.parent = SceneGamePlay.instance.mainCanvas.transform;
                result.transform.parent = MainMenuCanvas.Instance.transform;
                result.GetComponent<RectTransform>().localPosition = Vector3.zero;
                result.GetComponent<RectTransform>().sizeDelta = Vector3.zero;
                result.transform.localScale = Vector3.one;
                if (result is AnimatorUGUIPanel)
                {
                    (result as AnimatorUGUIPanel).gameObject.SetActive(false);
                }

                if (!allController.ContainsKey(type))
                {
                    allController.Add(type, result);
                }
                else
                {
                    allController[type] = result;
                }
                return result;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }


    public T GetPanelControl<T>(MainMenuPanelType type) where T : MainMenuUGUIBase
    {
        return GetPanelPopup(type) as T;
    }
    #endregion

    #region Call Method

    public void CloseToNonePopup()
    {
        while (currentWindow != MainMenuPanelType.None)
        {
            MainMenuUGUIBase panelCurrent = GetPanelPopup(currentWindow);
            ClosePanel(panelCurrent);
            currentWindow = panelCurrent.originPanelType;
        }
    }

    public void CloseCurrentPopupAndOpenOrigin(Action CallBack=null,params object[] instantiateData)
    {
        MainMenuUGUIBase panelCurrent = GetPanelPopup(currentWindow);
        if (panelCurrent != null)
        {
            if (panelCurrent.CanClose)
            {
                //panelCurrent.Close();
                var parent = GetPanelPopup(originWindow);

                if (parent != null && !parent.IsOpened)
                {
                    ClosePanel(panelCurrent, true);
                }
                else
                {
                    ClosePanel(panelCurrent, false);
                }

                //Phuc: Added 28/9. Wrong then update here
                panelCurrent = GetPanelPopup(originWindow);
                currentWindow = originWindow;
                //Phuc: Added 28/9. Wrong then update here
                if (panelCurrent != null)
                {
                    originWindow = panelCurrent.originPanelType;
                    panelCurrent.OpenPanelParent(instantiateData);
                }
                else
                {
                    originWindow = MainMenuPanelType.None;
                    if (CallBack != null)
                        CallBack.Invoke();
                }
                try
                {
                    panelCurrent = GetPanelPopup(currentWindow);
                    if (panelCurrent != null)
                    {
                        panelCurrent.CallActionOpenParentAgain();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }
        else
        {
            foreach (var popup in allController)
            {
                if (popup.Value.IsOpened)
                {
                    ClosePanel(popup.Value, true);
                }
            }
        }
    }

    public void CloseToPopup(MainMenuPanelType type)
    {
        CloseToNonePopup();
        OpenPopup(GetPanelPopup(type));
    }

    public void SetBackToOriginPanel()
    {
        currentWindow = originWindow;
        originWindow = MainMenuPanelType.None;
    }

    public void OpenPopupAndKeepParent(MainMenuPanelType type, Action actionCallBackOpenParentAgain = null, params object[] instantiateData)
    {
        MainMenuUGUIBase panelCurrent = GetPanelPopup(type);
        if (currentWindow != panelCurrent.windowType)
        {
            panelCurrent.AddCallBack(actionCallBackOpenParentAgain);
            
            panelCurrent.originPanelType = currentWindow;
        }
        OpenPopup(panelCurrent, instantiateData);
    }

    public void OpenPopupAndCloseAllParentBefore(MainMenuPanelType type, Action actionCallBackOpenParentAgain = null, params object[] instantiateData)
    {
        if (type == currentWindow)
            return;
        MainMenuUGUIBase panelCurrent = GetPanelPopup(type);
        if (panelCurrent != null)
            panelCurrent.AddCallBack(actionCallBackOpenParentAgain);
        panelCurrent.originPanelType = currentWindow;
        MainMenuUGUIBase panelParent = GetPanelPopup(currentWindow);

        foreach (var popup in allController)
        {
            if (popup.Value.IsOpened)
            {
                ClosePanel(popup.Value, true);
            }
        }
        OpenPopup(panelCurrent, instantiateData);
    }

    public void OpenPopupAndCloseParent(MainMenuPanelType type, Action actionCallBackOpenParentAgain = null, params object[] instantiateData)
    {
        if (type == currentWindow)
            return;
        MainMenuUGUIBase panelCurrent = GetPanelPopup(type);
        if (panelCurrent != null)
            panelCurrent.AddCallBack(actionCallBackOpenParentAgain);
        panelCurrent.originPanelType = currentWindow;
        MainMenuUGUIBase panelParent = GetPanelPopup(currentWindow);

        ClosePanel(panelParent, true);
        OpenPopup(panelCurrent, instantiateData);
    }

    public void OpenPopupOnly(MainMenuPanelType type, params object[] instantiateData)
    {
        MainMenuUGUIBase panelCurrent = GetPanelPopup(type);
        OpenPopup(panelCurrent, instantiateData);
    }

    public void HideCurrentPanel()
    {
        GetPanelPopup(currentWindow).ClosePanelOnly();
    }

    public void HideForcePanelChildren()
    {
        MainMenuUGUIBase panelCurrent = GetPanelPopup(currentWindow);
        MainMenuUGUIBase panelParent = GetPanelPopup(originWindow);
        panelCurrent.CloseNotPlayAnimation();
        currentWindow = MainMenuPanelType.None;
        panelParent.ClosePanelOnly();
        originWindow = MainMenuPanelType.None;
    }

    public void ShowCurrentPanel()
    {
        MainMenuUGUIBase panelCurrent = GetPanelPopup(currentWindow);
        if (panelCurrent != null && !panelCurrent.IsOpened)
        {
            panelCurrent.OpenPanelOnly();
        }
    }

    public void HidePanel(MainMenuPanelType type)
    {
        MainMenuUGUIBase panelCurrent = GetPanelPopup(type);
        if (panelCurrent != null && panelCurrent.IsOpened)
        {
            panelCurrent.ClosePanelOnly();
        }
    }

    public void ShowPanel(MainMenuPanelType type)
    {
        MainMenuUGUIBase panelCurrent = GetPanelPopup(type);
        if (panelCurrent != null && !panelCurrent.IsOpened)
        {
            panelCurrent.OpenPanelOnly();
        }
    }

    public MainMenuUGUIBase GetControllerUI(MainMenuPanelType type)
    {
        if (allController.ContainsKey(type))
        {
            return allController[type];
        }
        return null;
    }

    public void BackGlobalWindow()
    {        
        switch (currentWindow)
        {
            case MainMenuPanelType.None:
            case MainMenuPanelType.UpgradeHeroScreen:
            case MainMenuPanelType.ShopScreen:
            case MainMenuPanelType.UpgradeBowScreen:
            case MainMenuPanelType.UpgradeStatScreen:
                PopupManager.Instance.CloseToastImmediate();
                PopupManager.Instance.OpenNotification(Localize.GetLocalizedString("are_you_sure_to_exit_game"), (() => {
                    UtilityGame.QuitGame();
                }), true);
                return;
        }
        switch (originWindow)
        {
            default:
                CloseCurrentPopupAndOpenOrigin(() =>
                {});
                break;
        }
    }

    public bool isExistInScene(MainMenuPanelType type)
    {
        if (allController.ContainsKey(type))
        {
            return true;
        }
        return false;
    }
    #endregion
}
