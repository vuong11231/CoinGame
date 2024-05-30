using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StoreConstants
{
    public const string GAME_NAME = "Jewel Hunter";
    public const string MAIL_FEEDBACK = "info@d2mstudio.com";
    public const string LINK_APP_IOS = "itms-apps://itunes.apple.com/app/id1473689494?action=write-review&mt=8";
    public const string LINK_APP_ANDROID = "market://details?id=com.d2mstudio.jewelhunter";
    public const string LINK_PUBLISHER_IOS = "itms://itunes.apple.com/us/artist/d2m/id1473689494?uo=4";
    public const string LINK_PUBLISHER_ANDROID = "market://search?q=pub:\"D2M LTD.\"";

    public const string D2M_FACEBOOK_APP_IOS = "fb://page?id=2033059640317521";
    public const string D2M_FACEBOOK_APP_ANDROID = "fb://facewebmodal/f?href=https://www.facebook.com/2033059640317521/";
    public const string D2M_FACEBOOK_LINK = "https://www.facebook.com/2033059640317521/";

    //const string ID_Merge_Pet = "com.d2m.mergepet"; const string Appstore_Merge_Pet = "1464558032"; const string Scheme_Merge_Pet = "d2mmergepet";


//    public static string GetAppID(MoreGameDataElement DataIndex)
//    {
//#if UNITY_ANDROID
//        return DataIndex.ID;
//#elif UNITY_IOS
        
//        return DataIndex.scheme;
//#endif
//        return null;
//    }

//    static string GetAppstoreID(MoreGameDataElement DataIndex)
//    {
//        return DataIndex.ID_AppStore;
//    }

//    public static string GetLinkApp(MoreGameDataElement DataIndex)
//    {
//#if UNITY_ANDROID
//        return "market://details?id=" + GetAppID(DataIndex);
//#elif UNITY_IOS
//        return "itms-apps://itunes.apple.com/app/id" + GetAppstoreID(DataIndex);
//#endif
//        return null;
//    }

//    public static void OpenFacebookStudio()
//    {
//        #if UNITY_EDITOR
//                Application.OpenURL(StoreConstants.D2M_FACEBOOK_LINK);
//        #else
//        try
//        {
//#if UNITY_IOS
//            Application.OpenURL(MyConstant.D2M_FACEBOOK_APP_IOS);
//#elif UNITY_ANDROID
//            Application.OpenURL(StoreConstants.D2M_FACEBOOK_APP_ANDROID);
//#endif
    //    }
    //    catch
    //    {
    //        Application.OpenURL(StoreConstants.D2M_FACEBOOK_LINK);
    //    }
    //    #endif
    //}
}
