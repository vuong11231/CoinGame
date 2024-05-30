using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataReward
{
    public int material = 0;
    public int air = 0;
    public int antimater = 0;
    public int fire = 0;
    public int gravity = 0;
    public int ice = 0;
    public int diamond = 0;
    public int unknown = 0;
    public int toy1 = 0;
    public int toy2 = 0;
    public int toy3 = 0;
    public int toy4 = 0;
    public int toy5 = 0;

    public bool isEffecStones = false;

    public DataReward() { isEffecStones = false; }

    public bool IsEmpy() {
        return material == 0 && air == 0 && antimater == 0 && fire == 0 && gravity == 0 && ice == 0 && diamond == 0 && toy1 == 0 && toy2 == 0 && toy3 == 0 && toy4 == 0 && toy5 == 0;
    }

    public DataReward(int material, int air, int antimater, int fire, int gravity, int ice, int diamond, int unknown, int toy1, int toy2, int toy3, int toy4, int toy5)
    {
        isEffecStones = false;

        this.material = material;
        this.air = air;
        this.antimater = antimater;
        this.fire = fire;
        this.gravity = gravity;
        this.ice = ice;

        this.diamond = diamond;
        this.unknown = unknown;
    }

    public void Add(DataReward reward)
    {
        material += reward.material;
        air += reward.air;
        antimater += reward.antimater;
        fire += reward.fire;
        gravity += reward.gravity;
        ice += reward.ice;
        diamond += reward.diamond;
        unknown += reward.unknown;
        toy1 += reward.toy1;
        toy2 += reward.toy2;
        toy3 += reward.toy3;
        toy4 += reward.toy4;
        toy5 += reward.toy5;
    }
}
