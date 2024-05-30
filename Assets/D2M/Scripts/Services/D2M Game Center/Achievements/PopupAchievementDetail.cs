using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lean.Pool;
using Hellmade.Sound;

public class PopupAchievementDetail : Popups
{
    static PopupAchievementDetail _instance;

    [SerializeField]
    private Image AchievementParentIcon;
    [SerializeField]
    private Image AchievementIcon;
    [SerializeField]
    private TextMeshProUGUI counter;
    [SerializeField]
    private TextMeshProUGUI achievementName;
    [SerializeField]
    private TextMeshProUGUI achievementDisc;
    [SerializeField]
    private Slider progressSlider;
    [SerializeField]
    private TextMeshProUGUI progressText;
    [SerializeField]
    private TextMeshProUGUI rewardValue;
    [SerializeField]
    private Sprite baseIcon;

    float firstPosition = -608;
    float secondPosition = 608;

    AchievementViewRowData _data;

    public static void Show(AchievementViewRowData data)
    {
        if (_instance == null)
        {
            _instance = Instantiate(
                Resources.Load<PopupAchievementDetail>("Prefabs/Pop-ups/Achievement/PopupAchievementDetail"),
                Popups.CanvasPopup2.transform,
                false);
        }

        _instance._data = data;
        _instance.Appear();
    }

    public override void Appear()
    {
        if (!GameStatics.IsAnimating)
        {
            base.Appear();
            UpdateUI();

            //Animation
            AnimationHelper.AnimateGameCenterShow
            (
                this,
                Background.GetComponent<Image>(),
                Panel.gameObject,
                Panel,
                firstPosition,
                secondPosition,
                8f
            );
        }
    }

    public override void Disappear()
    {
        if (!GameStatics.IsAnimating)
        {
            AnimationHelper.AnimateGameCenterClose
            (
                this,
                Background.GetComponent<Image>(),
                Panel.gameObject,
                Panel,
                firstPosition,
                8f,
                () =>
                {
                    Popups.DisappearIgnoreNextAppear = true;
                    base.Disappear();
                }
            );

            //Sound
            //EazySoundManager.PlaySound(Sounds.Instance.Btn_Close);
        }
    }

    public override void Disable()
    {
        throw new System.NotImplementedException();
    }

    public override void NextStep(object value = null)
    {
        throw new System.NotImplementedException();
    }

    private void UpdateUI()
    {
        //counter.text = DataGameSave.data.countAchievementComplete + " / " + GameCenters.MaxProcess.Length;
        //achievementName.text = _data.achievementName;
        ////
        //var progress = AchievementViewRowController.GetProgess(_data.type) / GameCenters.MaxProcess[(int)_data.type];
        //if (progress > 1)
        //    progress = 1;
        ////
        //progressSlider.value = progress;
        //Color color1;
        //Color color2;
        //ColorUtility.TryParseHtmlString("#ed1c24", out color1);
        //ColorUtility.TryParseHtmlString("#159018", out color2);
        //Color tmpColor = progressSlider.fillRect.GetComponent<Image>().color = Color.Lerp(color1, color2, progress);
        //// text
        //progressText.text = System.Math.Round(progress * 100, 2) + "%";
        //progressText.color = tmpColor;
        //// icon
        //if (_data.isReceived)
        //{
        //    AchievementParentIcon.sprite = _data.icon;
        //    AchievementIcon.gameObject.SetActive(false);
        //}
        //else
        //{
        //    if (progressSlider.value < 0.0001f)
        //    {
        //        AchievementParentIcon.sprite = baseIcon;
        //        AchievementIcon.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        AchievementParentIcon.sprite = _data.icon;
        //        AchievementIcon.gameObject.SetActive(true);
        //        AchievementIcon.fillAmount = 1f - progressSlider.value;
        //    }
        //}

        ////
        //if (_data.isReceived)
        //{
        //    achievementDisc.text = _data.achievementUnlock.Replace("<br>", "\n");
        //    rewardValue.text = "";
        //}
        //else
        //{
        //    achievementDisc.text = _data.achievementLock.Replace("<br>", "\n");
        //    rewardValue.text = _data.expReward.ToString() + " XP";
        //}
    }
}
