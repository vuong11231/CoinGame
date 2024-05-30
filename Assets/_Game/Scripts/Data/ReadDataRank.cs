using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadDataRank : Singleton<ReadDataRank>
{
    [HideInInspector]
    public List<List<DataBaseRank>> ListDataRank;
    public List<string> path;

    private void Start()
    {
        ListDataRank = new List<List<DataBaseRank>>();
        var count = path.Count;
        for(int i = 0; i < count; i++)
            ReadFile_Rank(i);
    }

    public void ReadFile_Rank(int index)
    {
        List<string> lines;

        string tempString = Resources.Load<TextAsset>(path[index]).text;
        tempString = EncryptionHelper.Decrypt(tempString, true);

        // read file
        if (!string.IsNullOrEmpty(tempString))
        {
            lines = FileHelper.ReadCSVFromText(tempString);
        }
        else
        {
            Debug.LogError("Path sai");
            return;
        }

        // handle file
        if (lines != null && lines.Count > 0)
        {
            var tmpList = new List<DataBaseRank>();
            foreach (string i in lines)
            {
                if (string.IsNullOrEmpty(i))
                {
                    Debug.LogError("(Level Player) Line is empty");
                }
                else
                {
                    string[] column = i.Split('\t');

                    if (column.Length < 2 || column[0] == "Level")
                    {
                        Debug.LogError("(Level Player) Column is missing");
                    }
                    else
                    {
                        DataBaseRank Temp = new DataBaseRank();
                        Temp.PlanetRank = int.Parse(column[1]);
                        Temp.SolarSystemRank = int.Parse(column[2]);
                        Temp.GalaxyRank = int.Parse(column[3]);
                        Temp.IntergalacticRank = int.Parse(column[4]);
                        Temp.Universe = int.Parse(column[5]);
                        tmpList.Add(Temp);
                    }
                }
            }
            ListDataRank.Add(tmpList);
        }
    }

}

[Serializable]
public class DataBaseRank
{
    public int PlanetRank;
    public int SolarSystemRank;
    public int GalaxyRank;
    public int IntergalacticRank;
    public int Universe;
}
