using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

struct NotificationValue
{
    public string Text;
    public long DelaySeconds;
    public bool isRepeat;
}

public class Notifications : Singleton<Notifications>
{
    public bool IsDebug = false;

    const string KEY_IDs = "NOTIFICATION_IDs";

    const string TITLE_TEXT = "2048 PET ISLAND";

    //string DAILY_CHALLENGE_TEXT = "Time to play Daily Challenge!";

    string[] TextStrings =
    {
        "2048: Best Relaxing Game, Keep Calm & Enjoy it.",
        "Play 2048 per day will help you train your brain to solve puzzles " + EmojiList.SportsMedal,
        "Someone beat your score, Comeback to game to challenge.",
        "Are you feeling lonely? Play 2048 Pet to get fun " + EmojiList.GlowingStar,
        "You can send us a feature you want in this game. We will do it for you " + EmojiList.VictoryHand,
        "Time for The Moon Night: 2048 Pet " + EmojiList.RedHeart,
        "We have another game named: Merge Pet. Have you tried it?"
    };

    string TEXT_DAY_10 = "Hi Boss, you have a Big Reward that not received yet. Please comeback to achieve it " + EmojiList.WrappedGift;
    string TEXT_DAY_15 = "Play 2048 per day will help you train your brain to solve puzzles " + EmojiList.SportsMedal;
    string TEXT_DAY_30 = "Are you feeling lonely? Play 2048 Pet to get fun " + EmojiList.GlowingStar;

    Dictionary<int, NotificationValue> _ListNotification = new Dictionary<int, NotificationValue>();
    int _id = 0;

    private void Start()
    {
        LocalNotification.Init();
    }

    bool _isInit = false;

    void CreateNotification(string text, DateTime date, int dayAdded)
    {
        long delaySeconds = (long)(date.AddDays(dayAdded) - DateTime.Now).TotalSeconds;

        if (IsDebug) Debug.LogError("Notifications >>> CreateNotification: " + delaySeconds);

        _ListNotification.Add(++_id, new NotificationValue()
        {
            Text = text,
            DelaySeconds = delaySeconds,
            isRepeat = false
        });
    }

    void CheckNotification()
    {
        if (!_isInit)
        {
            _isInit = true;
            _id = 0;
            _ListNotification.Clear();

            long delaySeconds = 0;

            DateTime evening8pm = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                DateTime.Today.Day,
                20,
                0,
                0);

            var tempStr = TextStrings.ToList();
            //tempStr.ShuffleList();

            if (DateTime.Now < evening8pm && ((evening8pm - DateTime.Now).TotalSeconds >= 7200))
            {
                // Day 0 -> 6
                for(int i = 0; i < 7; i++)
                {
                    CreateNotification(tempStr[i] , evening8pm, i);
                }

                CreateNotification(TEXT_DAY_10, evening8pm, 9); // Day 10
                CreateNotification(TEXT_DAY_15, evening8pm, 14); // Day 15
                CreateNotification(TEXT_DAY_30, evening8pm, 29); // Day 30
            }
            else
            {
                // Day 1 -> 7
                for (int i = 1; i <= 7; i++)
                {
                    CreateNotification(tempStr[i - 1], evening8pm, i);
                }

                CreateNotification(TEXT_DAY_10, evening8pm, 10); // Day 10
                CreateNotification(TEXT_DAY_15, evening8pm, 15); // Day 15
                CreateNotification(TEXT_DAY_30, evening8pm, 30); // Day 30
            }

            //
            //
            // Push Notifications
            string keys = "";

            if (IsDebug) Debug.LogError("Notifications >>> count: " + _ListNotification.Count);

            foreach (var noti in _ListNotification)
            {
                keys += noti.Key + " ";

                if(IsDebug) Debug.LogError("Notifications >>> delay: " + noti.Value.DelaySeconds + ", value: " + noti.Value.Text);

                LocalNotification.PushLocal(
                    noti.Key,
                    TITLE_TEXT,
                    noti.Value.Text,
                    noti.Value.DelaySeconds,
                    noti.Value.isRepeat);
            }

            keys = keys.Trim();

#if UNITY_IOS
            DataHelper.SetString(KEY_IDs, keys);
#endif
        }
    }

    void CheckCallbackiOS()
    {
#if UNITY_IOS
        if(!string.IsNullOrEmpty(DataHelper.GetString(KEY_IDs)))
        {
            string[] ss = DataHelper.GetString(KEY_IDs).Split(' ');

            bool isDone = false;

            foreach (UnityEngine.iOS.LocalNotification note in
                                  UnityEngine.iOS.NotificationServices.localNotifications)
            {
                IDictionary dic = note.userInfo;
                foreach (string value in dic.Values)
                {
                    foreach(string data in ss)
                    {
                        if (value == data)
                        {
                            // Analytics
                            //AnalyticsManager.Instance.TrackNotificationOpened(true);

                            isDone = true;
                            break;
                        }
                    }

                    if (isDone)
                        break;
                }

                if (isDone)
                    break;
            }
        }
#endif
    }

    void OnApplicationFocus(bool focus)
    {
        if(!focus)
        {
            CheckNotification();
        }
        else
        {
            // Check Callback
#if UNITY_IOS
            CheckCallbackiOS();
#elif UNITY_ANDROID

            //// Analytics
            //var callback = Assets.SimpleAndroidNotifications.NotificationManager.GetNotificationCallback();

            //if (callback != null)
            //{
            //    //AnalyticsManager.Instance.TrackNotificationOpened(true);
            //}
#endif

            //
            _isInit = false;
            LocalNotification.Cancel();
        }
    }

    //void OnApplicationPause(bool pause)
    //{
    //    if(pause)
    //    {
    //        CheckNotification();
    //    }
    //    else
    //    {
    //        _isInit = false;
    //        LocalNotification.Cancel();
    //    }
    //}
}