using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.SimpleAndroidNotifications;
using Assets.SimpleAndroidNotifications.Data;
using Assets.SimpleAndroidNotifications.Helpers;
using Assets.SimpleAndroidNotifications.Enums;

public static class LocalNotification
{
    //static int _id = 1;

    public static void Init()
    {
        //#if UNITY_IOS
        //        LocalNotificationsManager.InitNotifications();
        //#elif UNITY_ANDROID

        //#endif
    }

    public static void PushLocal(int id, string title, string message, long seconds, bool isRepeat)
    {
        //#if UNITY_ANDROID
        var notificationParams = new NotificationParams
        {
            Id = id,
            Delay = TimeSpan.FromSeconds(seconds),
            Title = title,
            Message = message,
            Ticker = message,
            //Multiline = false,
            SmallIcon = NotificationIcon.Bell,
            LargeIcon = "app_icon",
            //Repeat = false
            //Repeat = isRepeat
        };

        NotificationManager.SendCustom(notificationParams);

        //#else
        //        LocalNotificationsManager.SendNotification(
        //            id,
        //            seconds,
        //            title,
        //            //"",
        //            message,
        //            message,
        //            "mln_blank_notification",
        //            "mln_notification",
        //            true,
        //            true,
        //            true,
        //            default(Color32),
        //            "");
        //#endif
    }

    public static void Cancel()
    {
        //#if UNITY_IOS
        //        LocalNotificationsManager.ClearLocalNotificationsInPanel();
        //#elif UNITY_ANDROID

        //#endif

        NotificationManager.CancelAll();
    }
}