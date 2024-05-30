using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattlePassCellData
{
    public int level;
    public BattlePassRewardType rewardType;
    public Sprite boxContainer;
    public int rewardNormal;
    public int rewardBattlePass;
    public int difficulty;
    public int meteorSmall;
    public int meteorColor;
    public int meteorDiamond;
    public int meteorGiant;

    public BattlePassCellData() { 
    
    }

    public BattlePassCellData(int level, Sprite boxContainer, BattlePassRewardType rewardType, int rewardNormal, int rewardBattlePass, int difficulty) {
        this.level = level;
        this.rewardType = rewardType;
        this.boxContainer = boxContainer;
        this.rewardNormal = rewardNormal;
        this.rewardBattlePass = rewardBattlePass;
        this.difficulty = difficulty;
    }

    public BattlePassCellData(int level, int meteorSmall, int meteorColor, int meteorDiamond, int meteorGiant, BattlePassRewardType rewardType, int rewardNormal) {
        this.level = level;
        this.rewardType = rewardType;
        this.rewardNormal = rewardNormal;
        this.meteorSmall = meteorSmall;
        this.meteorColor = meteorColor;
        this.meteorDiamond = meteorDiamond;
        this.meteorGiant = meteorGiant;
    }
}
