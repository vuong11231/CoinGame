using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattlePassCell : MonoBehaviour
{
    public BattlePassCellData data;
    public GameObject progressBar;
    public GameObject effectCollectNormal;
    public GameObject effectCollectBattle;

    public TextMeshProUGUI txtLevel;
    public Image boxContainer1;
    public Image boxContainer2;
    public Image rewardRenderer1;
    public Image rewardRenderer2;
    public TextMeshProUGUI txtRewardNormal;
    public TextMeshProUGUI txtRewardBattlePass;
    public TextMeshProUGUI txtReceivedNormal;
    public TextMeshProUGUI txtReceivedBattlePass;

    // phần thưởng 
    public Sprite sprDiamond; // kim cương 
    public Sprite sprStoneBlackHoleRandom; // đá hố đen ngẫu nhiên 
    public Sprite sprPlanetRandom; // hành tinh ngẫu nhiên 
    public Sprite sprStoneEffectRandom; // đá hiệu ứng ngẫu nhiên 
    public Sprite sprTimeRecoverPlanet; // thời gian hồi hành tinh 
    public Sprite sprPlanetOrgange; // hành tinh cam
    public Sprite sprPlanetYellow; // hành tinh vàng 
    public Sprite sprPlanetPurple; // hành tinh tím
    public Sprite sprChest;

    public int index;

    public void LoadData(BattlePassCellData data) {
        this.data = data;

        txtLevel.text = data.level.ToString();
        if (data.boxContainer != null) {
            boxContainer1.sprite = data.boxContainer;
            boxContainer2.sprite = data.boxContainer;
        }

        txtRewardNormal.text = data.rewardNormal.ToString();
        txtRewardBattlePass.text = data.rewardBattlePass.ToString();

        Sprite spr = GetRewardSprite();
        if (spr != null) {
            rewardRenderer1.sprite = spr;
            rewardRenderer2.sprite = spr;
        } 
    }

    public void SetCollectableProgress(bool collectable, int collectedNormal, int collectedBattle) {
        progressBar.SetActive(collectable);
        txtReceivedNormal.gameObject.SetActive(collectedNormal > 0);
        txtReceivedBattlePass.gameObject.SetActive(collectedBattle > 0);
        bool vip = DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_VIP_ENABLED) == "1";
        effectCollectNormal.SetActive(collectable && collectedNormal == 0);
        effectCollectBattle.SetActive(collectable && collectedBattle == 0 && vip);
    }

    public void ReceivePrizeNormal() {
        BattlePass.Instance.ReceivePrize(index, false);
    }

    public void ReceivePrizeBattlePass() {
        BattlePass.Instance.ReceivePrize(index, true);
    }

    public void ReceivePrizeMeteorFall() {
        MeteorFallRewardManager.Instance.ReceivePrize(index);
    }

    Sprite GetRewardSprite() {
        if (data.rewardType == BattlePassRewardType.DIAMOND) {
            return sprDiamond;
        }
        if (data.rewardType == BattlePassRewardType.BLACK_HOLE_STONES_RANDOM) {
            return sprStoneBlackHoleRandom;
        }
        if (data.rewardType == BattlePassRewardType.EFFECT_STONES_RANDOM) {
            return sprStoneEffectRandom;
        }
        if (data.rewardType == BattlePassRewardType.AUTO_RESTORE_PLANET) {
            return sprTimeRecoverPlanet;
        }
        if (data.rewardType == BattlePassRewardType.PLANET_RANDOM) {
            return sprPlanetRandom;
        }
        if (data.rewardType == BattlePassRewardType.PLANET_ORANGE) {
            return sprPlanetOrgange;
        }
        if (data.rewardType == BattlePassRewardType.PLANET_PUPPLE) {
            return sprPlanetPurple;
        }
        if (data.rewardType == BattlePassRewardType.PLANET_YELLOW) {
            return sprPlanetYellow;
        }
        if (data.rewardType == BattlePassRewardType.CHEST) {
            return sprChest;
        }
        return null;
    }
}
