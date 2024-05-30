using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Facebook.Unity;
using System.IO;
using System.Linq;

public class FacebookManager : MonoBehaviour
{
    static string[] Permissions = { "public_profile", "email" };
    
    const int PictureWidth = 300;
    const int PictureHeight = 300;

    static Action<string> _callbackLoginSuccess = null;
    static Action _callbackLoginFail = null;
    static Action _callbackAvatarSuccess = null;
    static Action _callbackAvatarFail = null;
    public static Action _callbackProfileSuccess = null;
    public static Action _callbackProfileFail = null;
    static Action _callbackEmailSuccess = null;
    static Action _callbackEmailFail = null;
    static Action<string> _callbackUserNameSuccess = null;
    static Action _callbackUserNameFail = null;
    static Action<List<string>> _callbackFriendSuccess = null;
    static Action _callbackFriendFail = null;

    private void Start()
    {
        Init();
    }

    public static void Init()
    {
        if (FB.IsInitialized)
            return;

        FB.Init(InitCallBack);

        //
        SetupLocal();
    }

    public static void ResumeFromPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            //app resume
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(FB.ActivateApp);
            }
        }
    }

    public static void InitCallBack()
    {
        FB.ActivateApp();
    }

    public static string LocalPathAvatar()
    {
        return Application.persistentDataPath + "/avatar.png";
    }
    static string accessToken = "";
    public static string GetAccessToken()
    {
        return accessToken;
    }

    public static void SetupLocal()
    {
        UserLogin.Profile.IsValidToFakeLogin = false;

        bool isValidUsername = false;
        bool isValidMail = false;

        // Avatar
        if (File.Exists(LocalPathAvatar()))
        {
            Texture2D text = MonoHelper.LoadPNG(LocalPathAvatar());
            if (text != null)
            {
                UserLogin.Profile.Avatar = Sprite.Create(text, new Rect(0, 0, text.height, text.width), Vector2.zero);
            }
        }

        // Username
        if (string.IsNullOrEmpty(UserLoginData.GetUsername()) == false)
        {
            UserLogin.Profile.Username = UserLoginData.GetUsername();
            isValidUsername = true;
        }

        // Email
        if (string.IsNullOrEmpty(UserLoginData.GetMail()) == false)
        {
            UserLogin.Profile.Email = UserLoginData.GetMail();
            isValidMail = true;
        }

        if (isValidUsername && isValidMail)
        {
            UserLogin.Profile.IsValidToFakeLogin = true;
        }
    }

    public static void SetCallbackAvatar(Action onSuccess, Action onFail)
    {
        _callbackAvatarSuccess = onSuccess;
        _callbackAvatarFail = onFail;
    }

    /// <summary>
    /// Login Facebook
    /// </summary>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    /// <param name="errorCallback">Action to be executed when the process fails.</param>
    public static void Login(Action<string> successCallback, Action errorCallback = null)
    {
        if (FB.IsLoggedIn)
        {
            if (successCallback != null)
                successCallback.Invoke("");

            return;
        }

        _callbackLoginSuccess = successCallback;
        _callbackLoginFail = errorCallback;
        
        FB.LogInWithReadPermissions(Permissions, LoginResult);
    }

    public static void LoginResult(ILoginResult res)
    {
        if (!ValidateResult(res))
        {
            if (_callbackLoginFail != null)
                _callbackLoginFail.Invoke();

            return;
        }

        accessToken = AccessToken.CurrentAccessToken.TokenString;

        // Automatically Get User's Email
        GetEmail(
            delegate ()
            {
                UserLogin.Profile.IsValidToFakeLogin = true;

                // success
                UserLogin.Profile.SNS = LoginSNS.FACEBOOK;
                UserLoginData.SetMail(UserLogin.Profile.Email);

                if (_callbackLoginSuccess != null)
                    _callbackLoginSuccess.Invoke(UserLogin.Profile.Email);
            },
            delegate ()
            {
            }
        );
        // Get User's Profile
        GetProfile();
        // Get User's Avatar URL
        GetAvatar();
        //
        //if (_callbackLoginSuccess != null)
        //    _callbackLoginSuccess.Invoke();

        // Linked with Playfab
        //GameCenterManager.Instance.LinkWithFacebook();
    }

    /// <summary>
    /// Is Logged In
    /// </summary>
    public static bool IsLoggedIn()
    {
        return FB.IsLoggedIn;
    }

    /// <summary>
    /// Logout Facebook
    /// </summary>
    public static void Logout()
    {
        FB.LogOut();

        UserLogin.Profile.Username = string.Empty;
    }

    // PROFILE

    /// <summary>
    /// Gets the profile.
    /// </summary>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    /// <param name="errorCallback">Action to be executed when the process fails.</param>
    public static void GetProfile(Action successCallback = null, Action errorCallback = null)
    {
        _callbackProfileSuccess = successCallback;
        _callbackProfileFail = errorCallback;

        FB.API("/me/?fields=first_name,name", HttpMethod.GET, GetProfileResult);
    }

    static void GetProfileResult(IGraphResult res)
    {
        UserLogin.Profile.UserID = res.ResultDictionary["id"].ToString();
        UserLogin.Profile.Username = res.ResultDictionary["name"].ToString();
        UserLogin.Profile.FirstName = res.ResultDictionary["first_name"].ToString();

        if (!ValidateResult(res))
        {
            if (_callbackProfileFail != null)
                _callbackProfileFail.Invoke();

            return;
        }
        if (_callbackProfileSuccess != null)
            _callbackProfileSuccess.Invoke();
    }

    // USERNAME

    /// <summary>
    /// Gets user name.
    /// </summary>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    /// <param name="errorCallback">Action to be executed when the process fails.</param>

    // EMAIL

    /// <summary>
    /// Gets the email.
    /// </summary>
    public static void GetEmail(Action successCallback = null, Action failCallback = null)
    {
        if (FB.IsLoggedIn)
        {

            _callbackEmailSuccess = successCallback;
            _callbackEmailFail = failCallback;

            FB.API("/me?fields=email", HttpMethod.GET, GetEmailResult);
        }
    }

    static void GetEmailResult(IGraphResult res)
    {
        IDictionary<string, object> dict = res.ResultDictionary;

        //foreach (var e in dict)
        //{
        //    Debug.Log("key: " + e.Key + ", value: " + e.Value);
        //}

        string s;
        string mail = "";
        if (dict.TryGetValue("email", out s))
        {
            mail = s;
        }

        if (string.IsNullOrEmpty(mail) == false)
        {
            UserLogin.Profile.Email = mail;

            if (_callbackEmailSuccess != null)
                _callbackEmailSuccess.Invoke();
        }
        else
        {
            if (_callbackEmailFail != null)
                _callbackEmailFail.Invoke();
        }
    }

    // AVATAR

    /// <summary>
    /// Gets the player's Facebook Avatar
    /// </summary>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    /// <param name="errorCallback">Action to be executed when the process fails.</param>
    public static void GetAvatar(Action successCallback = null, Action errorCallback = null)
    {
        //if (UserLogin.Profile.Avatar != null)
        //    return;

        string query = string.Format("/me/picture?type=square&height={0}&width={1}", PictureHeight, PictureWidth);
        if (successCallback != null) _callbackAvatarSuccess = successCallback;
        if (errorCallback != null) _callbackAvatarFail = errorCallback;

        FB.API(query, HttpMethod.GET, GetAvatarResult);
    }

    static void GetAvatarResult(IGraphResult res)
    {
        if (!ValidateResult(res) || res.Texture == null)
        {
            if (_callbackAvatarFail != null)
                _callbackAvatarFail.Invoke();

            return;
        }

        Texture2D text = res.Texture;

        if (text.width >= PictureWidth && text.height >= PictureHeight)
            UserLogin.Profile.Avatar = Sprite.Create(text, new Rect(0, 0, PictureHeight, PictureWidth), Vector2.zero);
        else
            UserLogin.Profile.Avatar = Sprite.Create(text, new Rect(0, 0, text.height, text.width), Vector2.zero);

        if (SpaceManager.Instance)
            SpaceManager.Instance.UpdateAvatar();

        if (_callbackAvatarSuccess != null)
        {
            _callbackAvatarSuccess.Invoke();
        }

        // save to local
        File.WriteAllBytes(LocalPathAvatar(), res.Texture.EncodeToPNG());
    }

    /// <summary>
    /// Gets the player's Facebook profile picture.
    /// </summary>
    public static void GetAvatarFromID(string id, Action<IGraphResult> successCallback = null, Action<IGraphResult> errorCallback = null)
    {
        string query = string.Format("/{0}/picture?type=square&height={1}&width={2}", id, PictureHeight, PictureWidth);

        FB.API(query, HttpMethod.GET,
            (res =>
            {
                if (!ValidateResult(res) || res.Texture == null)
                {
                    if (errorCallback != null)
                        errorCallback(res);

                    return;
                }

                if (successCallback != null)
                    successCallback(res);
            }));
    }

    /// <summary>
    /// Gets the player's Facebook Avatar from a URL.
    /// </summary>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    /// <param name="errorCallback">Action to be executed when the process fails.</param>
    public static void GetAvatarFromUrl(Action successCallback = null, Action errorCallback = null)
    {
        if (UserLogin.Profile.Avatar != null)
            return;

        string query = string.Format("/me/picture?type=square&height={0}&width={1}&redirect=false", PictureHeight, PictureWidth);

        FB.API(query, HttpMethod.GET,
            (res =>
            {
                if (!ValidateResult(res))
                {
                    if (errorCallback != null)
                        errorCallback.Invoke();

                    return;
                }

                if (successCallback != null)
                    successCallback.Invoke();

                UserLogin.Profile.Avatar = Sprite.Create(res.Texture, new Rect(0, 0, PictureHeight, PictureWidth), Vector2.zero);
            }));
    }

    /// <summary>
    /// Get the Avatar URL from an ID
    /// </summary>
    /// <returns>The URL.</returns>
    /// <param name="id">Identifier.</param>
    public static string GetAvatarUrlFromID(string id)
    {
        return "https://graph.facebook.com/" + id + "/" + string.Format("picture?type=square&height={0}&width={1}", PictureHeight, PictureWidth);
    }

    public static string GetAvatarUrl()
    {
        if (string.IsNullOrEmpty(UserLogin.Profile.UserID))
            return "";
        else
            return GetAvatarUrlFromID(UserLogin.Profile.UserID);
    }

    // FRIEND

    /// <summary>
    /// Gets the player's friend list
    /// </summary>
    /// <param name="successCallback">Action to be executed when the process is done correctly.</param>
    /// <param name="errorCallback">Action to be executed when the process fails.</param>
    public static void GetFriend(Action<List<string>> successCallback, Action failCallback)
    {
        if (FB.IsLoggedIn)
        {
            _callbackFriendSuccess = successCallback;
            _callbackFriendFail = failCallback;

            FB.API("/me?fields=friends", HttpMethod.GET, GetFriendResult);
        }
        else
        {
            if (failCallback != null)
                failCallback.Invoke();
        }
    }

    private static void GetFriendResult(IGraphResult res)
    {
        if (!res.ResultDictionary.ContainsKey("friends") && _callbackFriendFail != null)
        {
            _callbackFriendFail.Invoke();
            return;
        }

        var friendsList = res.ResultDictionary["friends"] as Dictionary<string, object>;

        if (friendsList == null && _callbackFriendFail != null)
        {
            _callbackFriendFail.Invoke();
            return;
        }

        if (!friendsList.ContainsKey("data") && _callbackFriendFail != null)
        {
            _callbackFriendFail.Invoke();
            return;
        }

        var data = friendsList["data"] as List<object>;

        if (data == null && _callbackFriendFail != null)
        {
            _callbackFriendFail.Invoke();
            return;
        }

        List<string> ids = new List<string>();

        foreach (var i in data)
        {
            Dictionary<string, object> item = (Dictionary<string, object>)i;

            if (item == null || !item.ContainsKey("id"))
            {
                continue;
            }

            ids.Add(item["id"] as string);
        }

        _callbackFriendSuccess.Invoke(ids);
    }

    //

    /// <summary>
    /// Validate the JSON result
    /// </summary>
    private static bool ValidateResult(IResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
            return true;

        Debug.Log(string.Format("{0} is invalid (Cancelled={1}, Error={2}, JSON={3})",
            result.GetType(), result.Cancelled, result.Error, Facebook.MiniJSON.Json.Serialize(result.ResultDictionary)));

        return false;
    }

    /// <summary>
    /// Open Messenger app
    /// </summary>
    public static void OpenMessenger()
    {

        if (!string.IsNullOrEmpty(UserLogin.Profile.UserID))
        {
            Application.OpenURL("https://m.me/" + UserLogin.Profile.UserID);
        }
        else
        {
            Application.OpenURL("https://m.me");
        }
    }

    public static void InviteFriend(Action<int> callback)
    {
        //#if !UNITY_EDITOR
        FB.AppRequest(
            "Hi Buddy, come play this great game!",
            null,
            new List<object>() { "app_non_users" },
            null,
            null,
            null,
            null,
            delegate (IAppRequestResult result)
            {
                if (result.Cancelled)
                {
                    Debug.Log("AppRequest Cancel");
                }
                else if (!String.IsNullOrEmpty(result.Error))
                {
                    Debug.Log("AppRequest Error");
                }
                else if (!String.IsNullOrEmpty(result.RawResult))
                {
                    Debug.Log("AppRequest Success");

                    callback.Invoke(result.To.Count());
                }
            }
        );
        //#else
        //        callback.Invoke(1);
        //#endif
    }
}
