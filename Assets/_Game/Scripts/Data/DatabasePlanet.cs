using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DatabasePlanet : Singleton<DatabasePlanet>
{
    //public List<DataBasePlanet> ListDataPlanetDefault;

    public List<IcePlanet> ListIcePlanet;
    public List<FirePlanet> ListFirePlanet;
    public List<AirPlanet> ListAirPlanet;
    public List<GravityPlanet> ListGravityPlanet;
    public List<AntimatterPlanet> ListAntimatterialPlanet;

    public string path;
    public string pathTypePlanet;
    private void Start()
    {
        ReadFile_Data();
        ReadFile_TypeData();
    }

    int ValueIndex = 0;

    public void ReadFile_Data()
    {

        //List<string> lines;

        //string tempString = Resources.Load<TextAsset>(path).text;
        ////tempString = EncryptionHelper.Decrypt(tempString, true);
        
        //// read file
        //if (!string.IsNullOrEmpty(tempString))
        //{
        //    lines = FileHelper.ReadCSVFromText(tempString);
        //}
        //else
        //{
        //    Debug.LogError("Path sai");
        //    return;
        //}

        //// handle file
        //if (lines != null && lines.Count > 0)
        //{
        //    foreach (string i in lines)
        //    {
        //        if (string.IsNullOrEmpty(i))
        //        {
        //            Debug.LogError("(Level Player) Line is empty");
        //        }
        //        else
        //        {
        //            string[] column = i.Split('\t');

        //            if (column.Length < 2 || column[0] == "Level")
        //            {
        //                Debug.LogError("(Level Player) Column is missing");
        //            }
        //            else
        //            {
        //                DataBasePlanet Temp = new DataBasePlanet();

        //                Temp.Level = int.Parse(column[0]);
        //                Temp.Size = float.Parse(column[1]);
        //                Temp.Hp = int.Parse(column[2]);
        //                Temp.Dame = int.Parse(column[3]);

        //                Temp.Speed = int.Parse(column[4]);
        //                Temp.Gravity = int.Parse(column[5]);
        //                Temp.MaterialPer5Sec = int.Parse(column[6]);
        //                Temp.MaterialNeed = int.Parse(column[7]);

        //                ListDataPlanetDefault.Add(Temp);
        //            }
        //        }
        //    }
        //}
    }

    public void ReadFile_TypeData()
    {
        List<string> lines;

        string tempString = Resources.Load<TextAsset>(pathTypePlanet).text;
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
            foreach (string i in lines)
            {
             

                if (string.IsNullOrEmpty(i))
                {

                    Debug.LogError(i);

                    Debug.LogError("(Level Player) Line is empty");
                }
                else
                {
                    string[] column = i.Split('\t');
                    
                    if (column.Length < 2 || column[0] == "Level" || column[0]=="#")
                    {
                        if (column[0] == "#") ValueIndex++;
                    }
                    else
                    {
                        switch(ValueIndex)
                        {
                            case 0:
                                {
                                    IcePlanet Temp = new IcePlanet();
                                    Temp.Slow = float.Parse(column[1]);
                                    Temp.TimeSlow = int.Parse(column[2]);
                                    Temp.IceNeed = int.Parse(column[3]);
                                    ListIcePlanet.Add(Temp);
                                    break;
                                }

                            case 1:
                                {
                                    FirePlanet Temp = new FirePlanet();
                                    Temp.UpDame = float.Parse(column[1]);
                                    Temp.DameToAir = float.Parse(column[2]);
                                    Temp.FireNeed = int.Parse(column[3]);
                                    ListFirePlanet.Add(Temp);
                                    break;
                                }

                            case 2:
                                {
                                    AirPlanet Temp = new AirPlanet();
                                    Temp.UpSize = int.Parse(column[1]);
                                    Temp.DownDame = float.Parse(column[2]);
                                    Temp.AirNeed = int.Parse(column[3]);
                                    ListAirPlanet.Add(Temp);
                                    break;
                                }

                            case 3:
                                {
                                    GravityPlanet Temp = new GravityPlanet();
                                    Temp.UpGravity = int.Parse(column[1]);
                                    Temp.SizeGravity = float.Parse(column[2]);
                                    Temp.UpMater = float.Parse(column[3]);
                                    Temp.GravityNeed = int.Parse(column[4]);
                                    ListGravityPlanet.Add(Temp);
                                    break;
                                }

                            case 4:
                                {
                                    AntimatterPlanet Temp = new AntimatterPlanet();
                                    Temp.UpSizeDame = int.Parse(column[1]);
                                    Temp.UpDame = int.Parse(column[2]);
                                    Temp.AntimatterialNeed = int.Parse(column[3]);
                                    ListAntimatterialPlanet.Add(Temp);
                                    break;
                                }
                        }
                    }
                }
            }
        }
    }

}
//[Serializable]
//public class DataBasePlanet
//{
//    public int Level;
//    public float Size;
//    public int Hp;
//    public int Dame;
//    public int Speed;
//    public int Gravity;
//    public int MaterialPer5Sec;
//    public int MaterialNeed;
//}
[Serializable]
public class IcePlanet 
{
    public float Slow;
    public int TimeSlow;
    public int IceNeed;
}
[Serializable] 
public class FirePlanet
{
    public float UpDame;
    public float DameToAir;
    public int FireNeed;
}
[Serializable]
public class AirPlanet 
{
    public int UpSize;
    public float DownDame;
    public int AirNeed;
}
[Serializable]
public class GravityPlanet 
{
    public int UpGravity;
    public float SizeGravity;
    public float UpMater;
    public int GravityNeed;
}
[Serializable]
public class AntimatterPlanet
{
    public int UpSizeDame;
    public int UpDame;
    public int AntimatterialNeed;
}