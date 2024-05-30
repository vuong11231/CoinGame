using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PLanetName
{
    Moon,
    Mercury,
    Venus,
    Earth,
    Mars,
    Jupiter,
    Nepturn,
    Saturn,
    Uranus,
    Pluto,
    Poseidon,
    Artemis,
    Hercules,
    Hera,
    Unicorn,
    Strange,
    Inferno,
    NightMare,
    BloodyP,
    IceBall,
    Reddusk,
    Thunder,
    Storm,

    _Count
} // Ko thay đổi thứ tự enum này

[Serializable]
public class DataPlanet
{

    public TypePlanet type = TypePlanet.Default;

    public TypePlanet Type
    {
        get { return type; }

        set
        {
            if (value == TypePlanet.Destroy)
            {
                IsCollect = true;
            }

            type = value;
        }
    }

    public float MaterialPerSec;
    public int LevelSpeed = 0;
    public int LevelSize = 0;
    public int LevelHeavier = 0;
    public int LevelElement = 0;
    public float HP;
    public DateTime ShootTime;
    public DateTime CollectTime;
    public bool IsCollect;
    public float MaterialCollect = 0;
    public float MaxTimeCollect;

    public PLanetName Name = PLanetName.Moon; // level of upgrade
    public PLanetName skin = PLanetName.Moon;
    public bool ClockWise = true;
    public long addedEffectTick = -1;

    public DataPlanet()
    {
        Name = PLanetName.Moon;
        type = TypePlanet.Default;
        MaterialPerSec = 1;
        IsCollect = true;
        HP = 100;
        MaxTimeCollect = 43200;
    }

    public void SetDataToDefault(DateTime _ShootTime)
    {

        HP = DataMaster.GetUpgradeValue(Name, DataMaster.DataMasterModel.HpStart, LevelSize, DataMaster.DataMasterModel.HpPerUpgrade);
        ShootTime = _ShootTime;
    }

    public float GetDame()
    {
        var avatarPower = SkinDataReader.Get(skin);
        var res = DataMaster.GetUpgradeValue(Name, DataMaster.DataMasterModel.DameStart, LevelSpeed, DataMaster.DataMasterModel.DamePerUpgrade);
        return (res + avatarPower.damePercent * res / 100);

    }

    public float GetSpeed() // sub 
    {
        return GetSpeed(Name, LevelSpeed);
    }

    public float GetSpeed(PLanetName Name, int LevelSpeed) // main 
    {
        float res = DataMaster.DataMasterModel.SpeedStart;

        for (var i = PLanetName.Moon; i <= Name; i++)
        {
            for (int u = 0; (i == Name && u <= LevelSpeed) || (i < Name && u < 10); u++)
            {
                if (i == PLanetName.Moon && u == 0)
                    continue;

                res += res / 100f * DataMaster.DataMasterModel.SpeedPerUpgrade;
            }
        }

        var avatarPower = SkinDataReader.Get(skin);        

        return (res + avatarPower.speedPercent * res / 100);
    }

    public float GetHP()
    {
        var avatarPower = SkinDataReader.Get(skin);
        var res = DataMaster.GetUpgradeValue(Name, DataMaster.DataMasterModel.HpStart, LevelSize, DataMaster.DataMasterModel.HpPerUpgrade);
        return (res + avatarPower.hpPercent * res / 100);        
    }

    public float GetGravity()
    {
        return DataMaster.GetUpgradeValue(Name, DataMaster.DataMasterModel.GravityStart, LevelHeavier, DataMaster.DataMasterModel.GravityPerUpgrade);
    }

    public float GetCollectMaterial()
    {
        var avatarPower = SkinDataReader.Get(skin);
        var res = DataMaster.GetUpgradeValue(Name, DataMaster.DataMasterModel.CollectMaterialStart, LevelHeavier, DataMaster.DataMasterModel.CollectMaterialPerUpgrade);

        return (res + avatarPower.matPerSecondAddition);
    }

    public float GetSize()
    {
        var avatarPower = SkinDataReader.Get(skin);
        var res = DataMaster.GetUpgradeValue(Name, DataMaster.DataMasterModel.SizeStart, LevelSize, DataMaster.DataMasterModel.SizePerUpgrade);
        return (res + avatarPower.sizePercent * res / 100);
    }

    public static int MaxPlanetNumber
    {
        get
        {
            return (int)PLanetName._Count;
        }
    }

    public float GetStartMoney(DateTime now, DateTime savedCollectTime)
    {
        if (MaterialCollect < 0)
            MaterialCollect = 0;

        if (MaxTimeCollect <= 0)
            MaxTimeCollect = 0;

        float afkTime = (float)(now - savedCollectTime).TotalSeconds;

        if (afkTime < 0)
            afkTime = 0;

        if (afkTime >= MaxTimeCollect)
        {
            afkTime = MaxTimeCollect;
        }

        return MaterialCollect + afkTime * (MaterialPerSec / 5);
    }
}