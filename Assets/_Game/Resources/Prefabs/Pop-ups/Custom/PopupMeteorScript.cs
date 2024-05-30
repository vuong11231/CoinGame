using SteveRogers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupMeteorScript : MonoBehaviour {
    public static int LEVEL_REQUIRE_1 = 3;
    public static int LEVEL_REQUIRE_2 = 8;
    public static int LEVEL_REQUIRE_3 = 15;

    public GameObject btnImage1;
    public GameObject btnImage2;
    public GameObject btnImage3;

    public GameObject btnText1;
    public GameObject btnText2;
    public GameObject btnText3;

    List<int> requireLevels;
    int remain1;
    int remain2;
    int remain3;

    private void Start() {
        requireLevels = new List<int>() { 0, LEVEL_REQUIRE_1, LEVEL_REQUIRE_2, LEVEL_REQUIRE_3 };

        CheckRestoreVisitCount();

        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_BELT_VISIT_COUNT_1), out remain1);
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_BELT_VISIT_COUNT_2), out remain2);
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_BELT_VISIT_COUNT_3), out remain3);

        SetupLevelDisplay(requireLevels[1], btnImage1, btnText1, remain1);
        SetupLevelDisplay(requireLevels[2], btnImage2, btnText2, remain2);
        SetupLevelDisplay(requireLevels[3], btnImage3, btnText3, remain3);
    }

    public void SetupLevelDisplay(int requiredLevel, GameObject btnImage, GameObject btnText, int remain) {
        if (btnText == null) {
            return;
        }
        if (DataGameSave.dataServer.level < requiredLevel) {
            btnImage.GetComponent<Image>().color = new Color32(100, 100, 100, 255);
            btnImage.GetComponent<Button>().enabled = false;
            btnText.GetComponent<Button>().enabled = false;
            btnImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format(TextMan.Get("Unlock LV {0}"), requiredLevel.ToString());
        } else {
            btnImage.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            btnImage.GetComponent<Button>().enabled = true;
            btnText.GetComponent<Button>().enabled = true;
            btnImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }

        btnText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = remain + "/" + GameManager.MAX_METEOR_VISIT_COUNT;
    }

    public void CheckRestoreVisitCount() {
        string savedDayCode = DataGameSave.GetMetaData(MetaDataKey.METEOR_BELT_DAYCODE);
        string nowDaycode = DataGameSave.GetDayCode(GameManager.Now);

        if (savedDayCode != nowDaycode) {
            DataGameSave.SaveMetaData(MetaDataKey.METEOR_BELT_DAYCODE, nowDaycode);
            DataGameSave.SaveMetaData(MetaDataKey.METEOR_BELT_VISIT_COUNT_1, GameManager.MAX_METEOR_VISIT_COUNT.ToString());
            DataGameSave.SaveMetaData(MetaDataKey.METEOR_BELT_VISIT_COUNT_2, GameManager.MAX_METEOR_VISIT_COUNT.ToString());
            DataGameSave.SaveMetaData(MetaDataKey.METEOR_BELT_VISIT_COUNT_3, GameManager.MAX_METEOR_VISIT_COUNT.ToString());
        }
    }

    public void SwitchToMeteorSceneWithLevel(int meteorBeltLevel) {
        MeteorBelt.isMeteorFall = false;

        if (meteorBeltLevel == 1) {
            GoToMeteor(meteorBeltLevel);
            return;
        }

        List<int> remainVisits = new List<int>() { 0, remain1, remain2, remain3 };

        if (remainVisits[meteorBeltLevel] <= 0) {
            return;
        } else {
            remainVisits[meteorBeltLevel]--;
        }

        DataGameSave.SaveMetaData(MetaDataKey.METEOR_BELT_VISIT_COUNT_1, remainVisits[1].ToString());
        DataGameSave.SaveMetaData(MetaDataKey.METEOR_BELT_VISIT_COUNT_2, remainVisits[2].ToString());
        DataGameSave.SaveMetaData(MetaDataKey.METEOR_BELT_VISIT_COUNT_3, remainVisits[3].ToString());

        GoToMeteor(meteorBeltLevel);
    }

    void GoToMeteor(int meteorBeltLevel) {
        GameManager.meteorBeltLevel = meteorBeltLevel;
        DataGameSave.dataEnemy = new DataGameServer();
        DataGameSave.dataEnemy = new DataGameServer();
        GetComponent<PopupCustom>().Onclose();

        Popups.IsShowed = false;
        Scenes.LastScene = SceneName.Gameplay;
        Scenes.ChangeScene(SceneName.MeteorBelt);
    }
}
