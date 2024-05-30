using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimatorUGUIPanel : MainMenuUGUIBase
{
    [Header("Anim only use for popup, panel")]
    public bool isPopupOrPanel;
    public bool isMainGroupScreen = false;
    [Space(30)]

    protected bool _isOpen;

    public override bool IsOpened
    {
        get { return _isOpen; }
    }

    #region Override Method

    protected override void OnCustomAwake()
    {
        base.OnCustomAwake();
        this.RegisterListener(EventID.ChangeLanguage, (sender, param) =>
         {
             UpdateLanguage();
         });
        if (isPopupOrPanel)
            isMainGroupScreen = false;
    }

    void ResizePopup(RectTransform rectTrans)
    {
        rectTrans.anchorMin = Vector2.zero;
        rectTrans.anchorMax = Vector2.one;
        rectTrans.pivot = Vector2.one * 0.5f;
        rectTrans.offsetMin = Vector2.zero;
        rectTrans.offsetMax = Vector2.zero;
    }

    public override void Open()
    {
        if (_isOpen)
            return;
        if (isPopupOrPanel)
        {
            var rectTran = transform.GetComponent<RectTransform>();
            if (rectTran.anchorMax != Vector2.one)
            {

                ResizePopup(rectTran);
            }
        }
        else
        {
            if (!isMainGroupScreen)
                MainMenuCanvas.Instance.topBar.DOPlayBackwards();
            //else
            //{
            //    transform.parent = MainMenuCanvas.Instance.mainGroupAnimControl.transform;
            //}
        }
        OpenPanelOnly();
    }

    public override void OnCustomShow(params object[] instantiateData)
    {
        //if (!isPopupOrPanel)
        //    MainMenuCanvas.Instance.CallTopBarAfterFade();
    }

    public override void Close()
    {
        //_isOpen = false;
        if (isPopupOrPanel)
        {
        }
        else
        {
            OnCustomHide();
            if (!isMainGroupScreen)
                MainMenuCanvas.Instance.topBar.DOPlayBackwards();
        }
        MainMenuCanvas.Instance.panelControl.CloseCurrentPopupAndOpenOrigin(() =>
        {
            //if (!isMainGroupScreen)
            //{
            //    MainMenuCanvas.Instance.CallMainElementAndTopBarAfterFadeAnim();
            //}
        });
    }

    public override void OpenPanelOnly()
    {
        _isOpen = true;
        if (isPopupOrPanel)
        {
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public override void ClosePanelOnly()
    {
        _isOpen = false;
        if (isPopupOrPanel)
        {
            gameObject.SetActive(false);
        }
        else
        {
            //MainMenuCanvas.Instance.panelControl.BackGlobalWindow();
            gameObject.SetActive(false);
        }
    }

    public void ClearObject()
    {
        if (isCanDestroyOnClose)
        {
            Destroy(this.gameObject);
            Resources.UnloadUnusedAssets();
        }
    }

    public override void CloseNotPlayAnimation()
    {
        _isOpen = false;
        gameObject.SetActive(false);
    }
    #endregion
}
