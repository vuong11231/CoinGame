using System;
using System.Collections.Generic;
using SteveRogers;
using UnityEngine;

public class ReadDataSun : Singleton<ReadDataSun>
{

    public List<DataBaseSun> ListDataSun;

    public string path;

    private void Start()
    {
        ReadFile_Data();
    }

    public void ReadFile_Data()
    {
        List<string> lines;
        string tempString = Resources.Load<TextAsset>(path).text;

        if (!string.IsNullOrEmpty(tempString))
        {
            lines = FileHelper.ReadCSVFromText(tempString);
        }
        else
        {
            Debug.LogError("Path sai: " + path);
            return;
        }

        if (lines != null && lines.Count > 0)
        {
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
                        DataBaseSun Temp = new DataBaseSun();
                        Temp.Level = int.Parse(column[0]);
                        Temp.Size = int.Parse(column[1]);
                        Temp.Hp = float.Parse(column[2]);
                        Temp.Dame = int.Parse(column[3]);
                        Temp.Gravity = int.Parse(column[4]);
                        Temp.Weight = int.Parse(column[5]);
                        Temp.MaterialNeed = int.Parse(column[6]);
                        Temp.MaterialPer5Sec = int.Parse(column[7]);
                        Temp.AmountOrbit = int.Parse(column[8]);
                        ListDataSun.Add(Temp);
                    }
                }
            }
        }
    }

}

[Serializable]
public class DataBaseSun
{
    public int Level;
    public int Size;
    public float Hp;
    public int Dame;
    public int Gravity;
    public int Weight;
    public int MaterialNeed;
    public int MaterialPer5Sec;
    public int AmountOrbit;
}

