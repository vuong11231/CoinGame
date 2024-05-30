using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SteveRogers;

public class PopupMeteorFallEventScript : MonoBehaviour
{
    public GameObject btnReward1;
    public GameObject btnReward2;
    public GameObject btnReward3;
    public GameObject btnReward4;

    public GameObject txtClaim1;
    public GameObject txtClaim2;
    public GameObject txtClaim3;
    public GameObject txtClaim4;

    public GameObject progress1;
    public GameObject progress2;
    public GameObject progress3;
    public GameObject progress4;

    public Image onOffImg;

    public Sprite on;
    public Sprite off;

    public Image nextButton;
    public Image eventImg;
    public Sprite event1;
    public Sprite event2;
    public TextMeshProUGUI txtNext;

    public TextMeshProUGUI txtDescription1;
    public TextMeshProUGUI txtDescription2;

    int eventShow = 1;
    //public Sprite onImg;
    //public Sprite offImg;
    //public Image 

    private void Start()
    {
        if (PlayerPrefs.GetString(PlayerPrefsKey.METEOR_FALL_EVENT_ENABLED) == "true")
        {
            onOffImg.sprite = on;
        }
        else
        {
            onOffImg.sprite = off;
        }
    }

    private void Update()
    {
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_WIN_MAX_COUNT), out int maxWin);
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_WIN_1), out int win1);
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_WIN_2), out int win2);
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_WIN_3), out int win3);
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_WIN_4), out int win4);

        btnReward1.SetActive(maxWin >= 1);
        btnReward2.SetActive(maxWin >= 2);
        btnReward3.SetActive(maxWin >= 3);
        btnReward4.SetActive(maxWin >= 4);

        progress1.SetActive(maxWin >= 1);
        progress2.SetActive(maxWin >= 2);
        progress3.SetActive(maxWin >= 3);
        progress4.SetActive(maxWin >= 4);

        txtClaim1.SetActive(maxWin >= 1 && win1 == 1);
        txtClaim2.SetActive(maxWin >= 2 && win1 == 1);
        txtClaim3.SetActive(maxWin >= 3 && win1 == 1);
        txtClaim4.SetActive(maxWin >= 4 && win1 == 1);
    }

    public void OnReward(int id)
    {
        List<string> keys = new List<string> { "", MetaDataKey.METEOR_FALL_WIN_1, MetaDataKey.METEOR_FALL_WIN_2, MetaDataKey.METEOR_FALL_WIN_3, MetaDataKey.METEOR_FALL_WIN_4 };

        int.TryParse(DataGameSave.GetMetaData(keys[id]), out int prizeReceived);
        if (prizeReceived == 1) {
            return;
        }

        if (id == 1)
        {
            //500 diamond
            DataReward reward = new DataReward
            {
                diamond = 500
            };

            GameManager.reward = reward;
            PopupMeteorResult.Show(reward: reward);

            DataGameSave.dataLocal.Diamond += 500;
            //DataGameSave.SaveToServer();
        }
        else if (id == 2)
        {
            // 300 meteor stones
            DataReward reward = PopupShop.GetRewardRandomEffectStones(300);
            GameManager.reward = reward;

            PopupMeteorResult.Show("Congratulation", "Return", reward, okFunction: () =>
            {
                DataGameSave.dataLocal.M_Material += reward.material;
                DataGameSave.dataLocal.M_AirStone += reward.air;
                DataGameSave.dataLocal.M_AntimatterStone += reward.antimater;
                DataGameSave.dataLocal.M_FireStone += reward.fire;
                DataGameSave.dataLocal.M_GravityStone += reward.gravity;
                DataGameSave.dataLocal.M_IceStone += reward.ice;
                DataGameSave.dataLocal.Diamond += reward.diamond;
                DataGameSave.dataLocal.M_ToyStone1 += reward.toy1;
                DataGameSave.dataLocal.M_ToyStone2 += reward.toy2;
                DataGameSave.dataLocal.M_ToyStone3 += reward.toy3;
                DataGameSave.dataLocal.M_ToyStone4 += reward.toy4;
                DataGameSave.dataLocal.M_ToyStone5 += reward.toy5;

                DataGameSave.dataServer.MaterialCollect += reward.material;

                //DataGameSave.SaveToLocal();
                //DataGameSave.SaveToServer();
            });
        }
        else if (id == 3)
        {
            // 30 random skins
            SkinDataReader.TryBuyRandomSkinPlanet(30, 0);
        }
        else {
            // first rank reward
            DataGameSave.RewardFirstRank();
        }

        DataGameSave.SaveMetaData(keys[id], "1");
        DataGameSave.SaveToServer();
    }

    public void OnOffEvent() {
        if (PlayerPrefs.GetString(PlayerPrefsKey.METEOR_FALL_EVENT_ENABLED) == "true")
        {
            PlayerPrefs.SetString(PlayerPrefsKey.METEOR_FALL_EVENT_ENABLED, "false");
            onOffImg.sprite = off;
        }
        else {
            PlayerPrefs.SetString(PlayerPrefsKey.METEOR_FALL_EVENT_ENABLED, "true");
            onOffImg.sprite = on;
        }

        //PlayerPrefs.SetString("meteor_fall_event", "true");
    }

    public void NextEvent() {
        if (eventShow == 1)
        {
            eventShow = 2;
            nextButton.gameObject.transform.localScale = new Vector3(-1, 1, 0);
            eventImg.sprite = event2;
            txtNext.text = TextMan.Get("Previous");
            txtDescription1.gameObject.SetActive(false);
            txtDescription2.gameObject.SetActive(true);
        }
        else {
            eventShow = 1;
            nextButton.gameObject.transform.localScale = new Vector3(1, 1, 0);
            eventImg.sprite = event1;
            txtNext.text = TextMan.Get("Next");
            txtDescription1.gameObject.SetActive(true);
            txtDescription2.gameObject.SetActive(false);
        }
    }
}
