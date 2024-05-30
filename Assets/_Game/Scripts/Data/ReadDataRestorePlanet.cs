using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadDataRestorePlanet : Singleton<ReadDataRestorePlanet>
{
    public List<DataRestorePlanet> ListDataRestore;

    public string path;

    private void Start()
    {
        ReadFile_Data();
    }

    public void ReadFile_Data()
    {
        List<string> lines;

        string tempString = Resources.Load<TextAsset>(path).text;
        //tempString = EncryptionHelper.Decrypt(tempString, true);
        
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
            foreach (string i in lines)
            {
                if (string.IsNullOrEmpty(i))
                {
                    Debug.LogError("(Level Player) Line is empty");
                }
                else
                {
                    string[] column = i.Split('\t');

                    if (column.Length < 1 || column[0] == "Index")
                    {
                        Debug.LogError("(Level Player) Column is missing");
                    }
                    else
                    {
                        DataRestorePlanet Temp = new DataRestorePlanet();
                        Temp.Index = int.Parse(column[0]);
                        Temp.Time = int.Parse(column[1]);
                        ListDataRestore.Add(Temp);
                    }
                }
            }
        }
    }
}

[Serializable]
public class DataRestorePlanet
{
    public int Index;
    public int Time;
}
