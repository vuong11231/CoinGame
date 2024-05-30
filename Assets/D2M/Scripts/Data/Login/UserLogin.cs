using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoginSNS
{
    FACEBOOK,
    GOOGLE
}

public struct LoginProfile
{
    public bool IsValidToFakeLogin;
    public LoginSNS SNS
    {
        get
        {
            switch(UserLoginData.GetLoginSNS())
            {
                case "FACEBOOK": return LoginSNS.FACEBOOK;
                case "GOOGLE": return LoginSNS.GOOGLE;
            }

            return LoginSNS.FACEBOOK;
        }
        set
        {
            UserLoginData.SetLoginSNS(value.ToString());
        }
    }
    public string UserID;
    public string Username
    {
        get
        {
            return UserLoginData.GetUsername();
        }
        set
        {
            UserLoginData.SetUsername(value);
        }
    }
    public string FirstName;
    public string Email;
    public Sprite _avatar;
    public Sprite Avatar
    {
        get
        {
            if (_avatar)
                return _avatar;

            int avatarId = (DataGameSave.dataServer.userid % 3) + 1;
            return Resources.Load<Sprite>("avatar_" + avatarId) as Sprite;
        }
        set
        {
            _avatar = value;
        }
    }
}

public static class UserLogin
{
    public static LoginProfile Profile;
}
