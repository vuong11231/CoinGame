using DG.Tweening;
using UnityEngine;
using UniRx;
using System.Linq;

public class MainMenuCanvas : MonoBehaviour
{
    public static MainMenuCanvas Instance;
    public PanelUGUIControl panelControl;

    [Header("Element Anim")]
    public DOTweenAnimation topBar;
    [Space(10)]

    public GameObject testingBtn;
    public Transform targetCoinPos;

    [Header("Notifi Red Dot")]
    public GameObject notiDailyGift;
    public GameObject notiAchievement;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DestroyImmediate(gameObject);
    }

    private void Start()
    {
        topBar.DORestart();
        LanguageManager.Instance.OpenAskSelectLanguage();
        Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape)).Subscribe(OnClickEscapeButton).AddTo(this);
        //testingBtn.SetActive(GameNewManager.Instance.isTestUGUI);
    }

    void OnClickEscapeButton(long deltaTime)
    {
        if (PopupManager.Instance.currentPopup != PopupType.None)
            PopupManager.Instance.CloseAllPopups();
        else
            panelControl.BackGlobalWindow();
    }


    #region PROCESS TOP BUTTON BAR

    public void OnClickTesting()
    {
        //SoundManager.Instance.PlayAudioSE(TypeSoundSE.ClickButton);
        panelControl.OpenPopupOnly(MainMenuPanelType.TestingPopup);
    }
    #endregion

}
