using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OSHelper {

    public static bool IsInternetAvailable = true;
	public static System.Text.RegularExpressions.Regex MailValidator = new System.Text.RegularExpressions.Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$");

	public static void OpenAppstore() {
#if UNITY_IOS
		Application.OpenURL(StoreConstants.LINK_APP_IOS);
#elif UNITY_ANDROID
        Application.OpenURL(StoreConstants.LINK_APP_ANDROID);
#endif
    }

    public static void OpenDeveloperApp()
    {
#if UNITY_IOS
        Application.OpenURL(StoreConstants.LINK_PUBLISHER_IOS);
#elif UNITY_ANDROID
        Application.OpenURL(StoreConstants.LINK_PUBLISHER_ANDROID);
#endif
    }

    public static void Mail (string sender, string content) 
	{
		//email Id to send the mail to
        string email = StoreConstants.MAIL_FEEDBACK;

		//subject of the mail
		string subject = "[" + StoreConstants.GAME_NAME + "]";
		#if UNITY_IOS
		subject = MyEscapeURL("[" + StoreConstants.GAME_NAME + "] " + sender);
		#elif UNITY_ANDROID
		subject = MyEscapeURL("[" + StoreConstants.GAME_NAME + "] " + sender);
		#endif

		//body of the mail which consists of Device Model and its Operating System
		string body = MyEscapeURL ( 
			"Ver: " + Application.version + "\n" +
			"Model: " + SystemInfo.deviceModel + "\n" +
			"OS: " + SystemInfo.operatingSystem + "\n\n" +
			content);

		//Open the Default Mail App
		Application.OpenURL ("mailto:" + email + "?subject=" + subject + "&body=" + body);
	}  

	static string MyEscapeURL (string url) 
	{
		return WWW.EscapeURL(url).Replace("+","%20");
	}

    public static IEnumerator ShareScreenshot(System.Action callbackFinish = null)
    {
#if !UNITY_EDITOR
       string imageName = "screenshot.png";

       ScreenCapture.CaptureScreenshot(imageName, 2);

       for (int i = 0; i < 5; i++)
       {
           yield return null;
       }

       yield return new WaitForSeconds(2f);

       // share
       string path = Application.persistentDataPath + "/" + imageName;

       //new NativeShare().AddFile(path).Share();
#else
        yield return null;
#endif

        // callback
        if (callbackFinish != null)
            callbackFinish.Invoke();
    }
}


//SELECT* from DATA WHERE type="Ảnh"
//SELECT COUNT(*) from DATA WHERE type = "Ảnh"