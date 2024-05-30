using Facebook.Unity;
using Hellmade.Sound;
using Newtonsoft.Json;
using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SignInWithApple;

public class PopupSetting : Popups {
    static PopupSetting _Instance;

    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtID;
    public TextMeshProUGUI txtFacebookStatus;
    public TextMeshProUGUI txtFacebookButton;
    public GameObject appleButton;
    public GameObject facebookButton;

    public Image SoundImg;
    public Image MusicImg;

    public Text soundTxt = null;
    public Text musicTxt = null;

    public Color activeColor = Color.white;
    public Color diactiveColor = Color.gray;

    public Dropdown dropdown = null;
    public SignInWithApple signInWithApple;

    Action _onClose;

    public static void Show() {
        if (_Instance == null) {
            _Instance = Instantiate(
            Resources.Load<PopupSetting>("Prefabs/Pop-ups/Setting/Popup Setting"),
            Popups.CanvasPopup.transform,
            false);
        }

        _Instance.Appear();
    }

    #region Overrive Methods
    public override void Appear() {
        base.Appear();

        SetImg();
        txtID.text = "ID: " + DataGameSave.dataServer.userid.ToString();
        txtName.text = DataGameSave.dataServer.Name.ToString();

        if (facebookButton != null)
        {
            facebookButton.SetActive(GameManager.Platform == RuntimePlatform.Android);
        }

        if (GameManager.Platform == RuntimePlatform.Android) {
            bool showFacebookButton = DataGameSave.dataServer.level >= 5;
            txtFacebookButton.transform.parent.gameObject.SetActive(showFacebookButton);
            txtFacebookStatus.gameObject.SetActive(showFacebookButton);
            appleButton.SetActive(false);

            if (DataGameSave.dataLogin.facebookid == null || DataGameSave.dataLogin.facebookid == "") {
                txtFacebookStatus.text = TextMan.Get("Not logged it");
                txtFacebookButton.text = TextMan.Get("Log in");
            } else {
                txtFacebookButton.text = TextMan.Get("Log out");
                txtFacebookStatus.text = string.Format(TextMan.Get("Logged in as {0}"), DataGameSave.dataServer.Name);
            }
        } else {
            bool showApplekButton = DataGameSave.dataServer.level >= 5;
            if (appleButton != null)
                appleButton.SetActive(showApplekButton);
            txtFacebookButton.transform.parent.gameObject.SetActive(false);

            if (DataGameSave.dataLogin.googleid == null || DataGameSave.dataLogin.googleid == "") {
                txtFacebookStatus.text = TextMan.Get("Not logged it");
                txtFacebookButton.text = TextMan.Get("Log in");
            } else {
                txtFacebookButton.text = TextMan.Get("Log out");
                txtFacebookStatus.text = string.Format(TextMan.Get("Logged in as {0}"), DataGameSave.dataServer.Name);
            }
        }

        AnimationHelper.AnimatePopupShowScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_APPEAR);

        EazySoundManager.PlaySound(Sounds.Instance.Show_Popup);
    }

    public override void Disappear() {
        AnimationHelper.AnimatePopupCloseScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_DISAPPEAR,
            () => {
                base.Disappear();
                if (_onClose != null) {
                    _onClose.Invoke();
                    _onClose = null;
                }
            });
    }

    public override void Disable() {
        base.Disable();
    }

    public override void NextStep(object value = null) {

    }

    #endregion

    void SetImg() {
        if (DataGameSave.dataLocal.musicVolume == 1) {
            MusicImg.color = activeColor;
            musicTxt.color = activeColor;
        } else {
            MusicImg.color = diactiveColor;
            musicTxt.color = diactiveColor;
        }

        if (DataGameSave.dataLocal.soundVolume == 1) {
            SoundImg.color = activeColor;
            soundTxt.color = activeColor;
        } else {
            SoundImg.color = diactiveColor;
            soundTxt.color = diactiveColor;
        }

        dropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("languageidx", 0));
    }

    public void OnClose() {
        if (GameStatics.IsAnimating)
            return;

        Disappear();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnMusic() {
        if (GameStatics.IsAnimating)
            return;

        Sounds.IgnorePopupShow = true;
        Sounds.IgnorePopupClose = true;

        if (DataGameSave.dataLocal.musicVolume == 1) {
            DataGameSave.dataLocal.musicVolume = 0;
            EazySoundManager.GlobalMusicVolume = 0;
        } else {
            DataGameSave.dataLocal.musicVolume = 1;
            EazySoundManager.GlobalMusicVolume = 1;
        }

        SetImg();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnSound() {
        if (GameStatics.IsAnimating)
            return;

        Sounds.IgnorePopupShow = true;
        Sounds.IgnorePopupClose = true;
        //
        if (DataGameSave.dataLocal.soundVolume == 1) {
            DataGameSave.dataLocal.soundVolume = 0;
            EazySoundManager.GlobalSoundsVolume = 0;
            EazySoundManager.GlobalUISoundsVolume = 0;
        } else {
            DataGameSave.dataLocal.soundVolume = 1;
            EazySoundManager.GlobalSoundsVolume = 1;
            EazySoundManager.GlobalUISoundsVolume = 1;
        }

        SetImg();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnSetLang(int idx) {
        idx = dropdown.value;

        if (SteveRogers.TextMan.LangIndex == idx)
            return;

        PlayerPrefs.SetInt("languageidx", idx);

        dropdown.SetValueWithoutNotify(idx);

        SteveRogers.TextMan.LangIndex = idx;
        var arr = FindObjectsOfType<SteveRogers.LocalizedText>();

        if (arr != null && arr.Length > 0)
            foreach (var l in arr)
                l.ReUpdate();
    }

    public void OnFacebookButtonClick() {
        if (DataGameSave.dataLogin.facebookid != "" && DataGameSave.dataLogin.facebookid != null) {
            PopupConfirm.ShowYesNo(TextMan.Get("Information"), TextMan.Get("Log out of this facebook account?"),()=> {
                DataGameSave.LogOutFacebook();
            });
        } else {
            PopupConfirm.ShowYesNo(TextMan.Get("Information"), TextMan.Get("Log in to facebook account?"), () => {
                DataGameSave.LoginFacebookButtonClick();
            });
        }

            //}

            //if (DataGameSave.dataLogin.facebookid != "" && DataGameSave.dataLogin.facebookid != null) {
            //    PopupConfirm.ShowOK("Oops!", TextMan.Get("Seem like you're already logged in!"));
            //    return;
            //}
            //Debug.Log("Check FB inited");

            //if (FB.IsInitialized)
            //{
            //    LogInFacebook();
            //} else
            //{
            //    FB.Init(() => {
            //        LogInFacebook();
            //    });
            //}


            
    }


    public void OnAppleButtonClick()
    {
        if (DataGameSave.dataLogin.googleid != "" && DataGameSave.dataLogin.googleid != null)
        {
            PopupConfirm.ShowYesNo(TextMan.Get("Information"), TextMan.Get("Log out of this apple account?"), () => {
                DataGameSave.LogOutApple();
            });
        }
        else
        {
            PopupConfirm.ShowYesNo(TextMan.Get("Information"), TextMan.Get("Log in to apple account?"), () => {
                DataGameSave.LoginAppleButtonClick(signInWithApple);
            });
        }
    }

    //void LogInFacebook() {
    //    Debug.Log("Login FB");
    //    Facebook.Unity.FB.LogInWithReadPermissions(new string[] { "public_profile", "email" }, (res) => {
    //        if (string.IsNullOrEmpty(res.Error))
    //        {
    //            string facebookid = "";
    //            res.ResultDictionary.TryGetValue("user_id", out facebookid);

    //            Debug.Log("Login FB success: fbid = " + facebookid);

    //            if (facebookid == "")
    //            {
    //                return;
    //            }

    //            WWWForm form = new WWWForm();
    //            form.AddField("facebookid", facebookid.ToString());

    //            Debug.Log("Sending FB id to server = " + facebookid);
    //            ServerSystem.Instance.SendRequest(ServerConstants.GET_UNIVERSE_DATA_BY_FACEBOOK_ID, form, () =>
    //            {
    //                Debug.Log("Send FB id to server success = " + facebookid);
    //                if (!ServerSystem.Instance.IsResponseOK())
    //                {
    //                    DataGameSave.dataServer.facebookid = facebookid.ToString();
    //                    DataGameSave.SaveToServer();

    //                    DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Facebook;
    //                    DataGameSave.dataLogin.facebookid = facebookid;
    //                    DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);

    //                    PopupConfirm.ShowOK(TextMan.Get("Success"), TextMan.Get("Data saved to your facebook account"));
    //                }
    //                else
    //                {
    //                    UniverseModel model = JsonConvert.DeserializeObject<UniverseModel>(ServerSystem.Instance.ReadData());
    //                    DataGameServer server = JsonConvert.DeserializeObject<DataGameServer>(model.jsonserver);
    //                    DataGameLocal local = JsonConvert.DeserializeObject<DataGameLocal>(model.jsonlocal);

    //                    if (model.userid == DataGameSave.dataServer.userid)
    //                    {
    //                        PopupConfirm.ShowOK("Oops!", TextMan.Get("Seem like you're already logged in!"));
    //                    }
    //                    else
    //                    {
    //                        string question = TextMan.Get("There is another data with this acount \nName: {0} \nLevel: {1} \nDo you want to overwrite it?");
    //                        PopupConfirm.ShowYesNo(
    //                            TextMan.Get("Data existed"),
    //                            String.Format(question, server.Name, server.Level),
    //                            () =>
    //                            {
    //                                KeepThisData(facebookid, model);
    //                            }, TextMan.Get("Overwrite"),
    //                            () =>
    //                            {
    //                                KeepOtherData(facebookid, model);
    //                            },
    //                            TextMan.Get("Keep"));
    //                    }
    //                }
    //            });
    //        }
    //        else
    //        {
    //            Debug.Log("Login FB Error: " + res.Error);
    //        }
    //    });
    //}

    //public void KeepThisData(string facebookid, UniverseModel other) {
    //    WWWForm form = new WWWForm();
    //    form.AddField("user_id", other.userid);
    //    form.AddField("status", "deleted");
    //    ServerSystem.Instance.SendRequest(ServerConstants.SET_STATUS, form, () => {
    //        DataGameSave.dataServer.facebookid = facebookid;
    //        DataGameSave.SaveToServer();

    //        DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Facebook;
    //        DataGameSave.dataLogin.facebookid = facebookid;
    //        DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);
    //        PopupConfirm.ShowOK(TextMan.Get("Overwrite"), TextMan.Get("Overwrited your data in facebook account by this data"));
    //    });
    //}

    //public void KeepOtherData(string facebookid, UniverseModel other) {
    //    DataGameSave.dataServer.facebookid = facebookid.ToString();
    //    DataGameSave.SetStatus("deleted");

    //    DataGameSave.ReadUniverseModel(other);

    //    DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Facebook;
    //    DataGameSave.dataLogin.facebookid = facebookid;
    //    DataGameSave.dataLogin.userid = DataGameSave.dataServer.userid;
    //    DataGameSave.dataLogin.token = DataGameSave.dataServer.token;
    //    DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);

    //    PopupConfirm.ShowOK(TextMan.Get("Loaded data"), TextMan.Get("Loaded data from your facebook account"), TextMan.Get("OK"), () => {
    //        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //    });
    //}

    //public void SaveFacebookId(string facebookid) {
    //    DataGameSave.dataServer.facebookid = facebookid.ToString();
    //    DataGameSave.SaveToServer();

    //    DataGameSave.dataLogin.loginType = DataGameLogin.LoginType.Facebook;
    //    DataGameSave.dataLogin.facebookid = facebookid;
    //    DataGameLogin.SaveLoginDataToLocal(DataGameSave.dataLogin);
    //}

    //public void OnFacebookButtonClickTest() {
    //    Facebook.Unity.FB.LogInWithReadPermissions(new string[] { "public_profile", "email" }, (res) => {

    //        foreach (var item in res.ResultDictionary)
    //        {
    //            MyDebug.Log(item.Key + " : " + item.Value);
    //            Debug.Log(item.Key + " : " + item.Value);

    //        }

    //        string id;
    //        string firstname;
    //        string lastname;
    //        string gender;
    //        string email;

    //        MyDebug.Log("Userid is :" + res.AccessToken.UserId);

    //        Facebook.Unity.FB.API("me?fields=name", Facebook.Unity.HttpMethod.GET, (fbResult) =>
    //        {
    //            fbResult.ResultDictionary.TryGetValue("first_name",out firstname);
    //            fbResult.ResultDictionary.TryGetValue("last_name", out lastname);
    //            fbResult.ResultDictionary.TryGetValue("gender", out gender);
    //            fbResult.ResultDictionary.TryGetValue("email", out email);
    //            fbResult.ResultDictionary.TryGetValue("id", out id);

    //            Debug.Log(id);
    //            Debug.Log(firstname);
    //            Debug.Log(lastname);
    //            Debug.Log(gender);
    //            Debug.Log(email);

    //            foreach (var item in fbResult.ResultDictionary) {
    //                MyDebug.Log(item.Key + " : " + item.Value);
    //                Debug.Log(item.Key + " : " + item.Value);
    //            }
    //        });

    //        string query = string.Format("/me/picture?type=square&height={0}&width={1}", 50, 50);

    //        FB.API(query, HttpMethod.GET, (fbResult)=> {
    //            Texture2D text = fbResult.Texture;

    //            if (text.width >= 50 && text.height >= 50)
    //                SoundImg.sprite = Sprite.Create(text, new Rect(0, 0, 50, 50), Vector2.zero);
    //            else
    //                SoundImg.sprite = Sprite.Create(text, new Rect(0, 0, text.height, text.width), Vector2.zero);

    //            MyDebug.Log("Avatar loaded");
    //        });
    //    });
    //}
}
