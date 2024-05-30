using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserLoginData
{
    const string k_user_username = "k_user_username";
    const string k_user_email = "k_user_email";
    const string k_user_avatar_url = "k_user_avatar_url";
    const string k_user_unique_token = "k_user_unique_token";
    const string k_user_data_change = "k_user_data_change";
    const string k_user_login_sns = "k_user_login_sns";
    //
    // Username
    public static string GetUsername()
    {
        return DataHelper.GetString(k_user_username, "");
    }

    public static void SetUsername(string username)
    {
        DataHelper.SetString(k_user_username, username);
    }

    //
    // Email
    public static string GetMail()
    {
        return DataHelper.GetString(k_user_email, "");
    }

    public static void SetMail(string mail)
    {
        DataHelper.SetString(k_user_email, mail);
    }

    //
    // Avatar
    public static string GetAvatarURL()
    {
        return DataHelper.GetString(k_user_avatar_url, "");
    }

    public static void SetAvatarURL(string avatarURL)
    {
        DataHelper.SetString(k_user_avatar_url, avatarURL);
    }

    //
    // Token
    public static string GetToken()
    {
        return DataHelper.GetString(k_user_unique_token, "");
    }

    public static void SetToken(string token)
    {
        DataHelper.SetString(k_user_unique_token, token);
    }

    //
    // Is Data Change
    public static bool IsDataChange()
    {
        return DataHelper.GetBool(k_user_data_change, false);
    }

    public static void SetDataChange(bool value)
    {
        DataHelper.SetBool(k_user_data_change, value);
    }

    // Login SNS
    //
    public static string GetLoginSNS()
    {
        return DataHelper.GetString(k_user_login_sns, "");
    }

    public static void SetLoginSNS(string sns)
    {
        DataHelper.SetString(k_user_login_sns, sns);
    }
}
