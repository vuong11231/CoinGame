using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DataGameLogin
{
    public enum LoginType { Normal, Facebook, Google, Apple }
    public int userid;
    public string username;
    public string token;
    public string facebookid;
    public string googleid;
    public LoginType loginType = LoginType.Normal;

    public static DataGameLogin LoadLoginDataFromLocal()
    {
        string filePath = FileHelper.GetWritablePath(GameConstants.LOGIN_DATA_LOCAL_FILE_NAME);
        if (!System.IO.File.Exists(filePath))
            return null;

        string content = FileHelper.LoadFileWithPassword(filePath,"",true);

        try {
            DataGameLogin result = Newtonsoft.Json.JsonConvert.DeserializeObject<DataGameLogin>(content);
            return result;
        }
        catch {
            return null;
        }
    }

    public static bool SaveLoginDataToLocal(DataGameLogin loginData)
    {
        try {
            string filePath = FileHelper.GetWritablePath(GameConstants.LOGIN_DATA_LOCAL_FILE_NAME);

            string content = Newtonsoft.Json.JsonConvert.SerializeObject(loginData);

            FileHelper.SaveFileWithPassword(content, filePath, "", true);
            return true;
        }
        catch 
        {
            return false;        
        }
    }
}

