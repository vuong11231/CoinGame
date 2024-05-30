using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SteveRogers;
using Newtonsoft.Json;

[Serializable]
public class AttackedInfoData
{
    public int id;
    public string name;
    public int mat;
}

[Serializable]
public class DataGameServer
{
    public int userid;
    public string Name;
    public string token;
    public string facebookid;
    //public string appleid;
    public string googleid;
    public bool isRemoveAds;
    public bool isAttacked = false;
    public string isAttackedCode = null;
    public float sunHp;
    public List<DataPlanet> ListPlanet;
    public List<string> ListEnemy;
    public int beAttacked;
    public bool IsCollect;
    public DateTime CollectTime;
    public float MaterialCollect;
    public float MaxTimeCollect;
    public int damageDay;
    public int damageWeek;
    public int damageMonth;
    public int damageWorld;
    public int rankChartId; //save avatar
    public int rankPoint;   //save battle boss vfact event
    public DateTime lastOnline;
    public int level;

    public List<AttackedInfoData> GetAttackedInfo()
    {
        List<AttackedInfoData> currentarr = null;

        try
        {
            currentarr = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AttackedInfoData>>(isAttackedCode);
        }
        catch { }

        return currentarr;
    }

    public float GetAllMaterialCollect()
    {
        if (ListPlanet.IsNullOrEmpty())
            return 0;

        float sum = 0;

        foreach (var planet in ListPlanet)
        {
            sum += planet.GetStartMoney(GameManager.Now, planet.CollectTime);
        }

        return sum;
    }

    // This is not saves to server!
    public void ResetMaterialCollect()
    {
        if (ListPlanet.IsNullOrEmpty())
            return;

        foreach (var planet in ListPlanet)
        {
            planet.CollectTime = GameManager.Now;
        }
    }

    public DataGameServer()
    {
        Name = "";
        isRemoveAds = false;
        level = 1;
        sunHp = 10;
        IsCollect = true;
        MaterialCollect = 0;
        MaxTimeCollect = 43200;
        ListPlanet = new List<DataPlanet>();
        ListEnemy = new List<string>();
        beAttacked = 0;
    }

    public void CheckInitSkinPieceAndLevel()
    {
        ReadSkinLevelUnlockData();

        if (DataGameSave.skinPieces == null || DataGameSave.skinPieces.Count < (int)PLanetName._Count) // init
        {
            if (DataGameSave.skinPieces == null)
            {
                DataGameSave.skinPieces = new List<int>((int)PLanetName._Count);
            }

            for (int i = DataGameSave.skinPieces.Count; i < (int)PLanetName._Count; i++)
                if (i == 0)
                {
                    DataGameSave.skinPieces.Add(1);
                }
                else
                {
                    DataGameSave.skinPieces.Add(0);
                }
        }

        if (DataGameSave.skinLevels == null || DataGameSave.skinLevels.Count < (int)PLanetName._Count) // init
        {
            if (DataGameSave.skinLevels == null)
            {
                DataGameSave.skinLevels = new List<int>((int)PLanetName._Count);
            }

            for (int i = DataGameSave.skinLevels.Count; i < (int)PLanetName._Count; i++)
            {
                DataGameSave.skinLevels.Add(1);
            }
        }
    }

    public void UpSkinLevel(PLanetName name, bool saveToServer)
    {
        var iname = (int)name;
        DataGameSave.skinLevels[iname]++;

        if (saveToServer)
        {
            DataGameSave.SaveToServer();
        }
    }

    public struct OneSkinLevelUnlockData
    {
        public int level;
        public int piece_for_unlock;
        public int bonus;
    }

    public void ReadSkinLevelUnlockData()
    {
        var content = Utilities.ReadAllText_Resources("DataSkinLevelUnlock");
        var dic = Utilities.CreateDictionaryFromCSVContent(ref content);
        DataGameSave.listSkinLevelUnlockData.Clear();
        foreach (var row in dic)
        {
            if (row.Key.ToLower().Contains("level"))
            {
                //Minh.ho: Delete first row
                continue;
            }

            OneSkinLevelUnlockData oneData = new OneSkinLevelUnlockData();
            int.TryParse(row.Key, out oneData.level);
            int.TryParse(row.Value[0], out oneData.piece_for_unlock);
            int.TryParse(row.Value[1], out oneData.bonus);
            DataGameSave.listSkinLevelUnlockData.Add(oneData);
        }
    }

    public int GetPieceNeedForNextLevel(int level)
    {
        foreach (var data in DataGameSave.listSkinLevelUnlockData)
        {
            if(data.level == level + 1)
            {
                return data.piece_for_unlock;
            }    
        }
        return 5;
    }
    public int GetBonusPercent_At_LevelSkin(int level)
    {
        foreach (var data in DataGameSave.listSkinLevelUnlockData)
        {
            if (data.level == level)
            {
                return data.bonus;
            }
        }
        return 5;
    }

    public float GetStartMoney(DateTime now)
    {
        if (MaterialCollect < 0)
            MaterialCollect = 0;

        float afkTime = (float)(now - CollectTime).TotalSeconds;

        if (afkTime >= MaxTimeCollect)
            afkTime = MaxTimeCollect;

        var data = ReadDataSun.Instance.ListDataSun.GetLastIfOverRange(level - 1);

        return MaterialCollect + afkTime * ((float)data.MaterialPer5Sec / 5);
    }
}