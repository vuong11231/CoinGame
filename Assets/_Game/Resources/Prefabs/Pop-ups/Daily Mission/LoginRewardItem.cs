using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginRewardItem : MonoBehaviour
{
    public Text txtDay;
    public Text txtAmount;
    public Image imgRewardIcon;

    RewardLogin rewardInfo;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Setup(RewardLogin info)
    {
        this.rewardInfo = info;
        updateUI();
    }

    public void setIcon(Sprite s)
    {
        imgRewardIcon.sprite = s;
    }

    void updateUI()
    {
        txtDay.text = "Ngày " + rewardInfo.day;
        txtAmount.text = "" + rewardInfo.amount;

        if (rewardInfo.type == RewardLogin.Type.RecoverTime)
        {
            txtAmount.text += "s";
        }
    }

    void onButtonClicked()
    {

    }
}
