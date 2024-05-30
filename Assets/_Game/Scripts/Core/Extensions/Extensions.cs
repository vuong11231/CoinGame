using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public static class Extensions
{
    public static int Replace<T>(this IList<T> source, T oldValue, T newValue)
    {
        if (source == null)
            throw new ArgumentNullException("source");

        var index = source.IndexOf(oldValue);
        if (index != -1)
            source[index] = newValue;
        return index;
    }

    public static void ReplaceAll<T>(this IList<T> source, T oldValue, T newValue)
    {
        if (source == null)
            throw new ArgumentNullException("source");

        int index = -1;
        do
        {
            index = source.IndexOf(oldValue);
            if (index != -1)
                source[index] = newValue;
        } while (index != -1);
    }


    public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T oldValue, T newValue)
    {
        if (source == null)
            throw new ArgumentNullException("source");

        return source.Select(x => EqualityComparer<T>.Default.Equals(x, oldValue) ? newValue : x);
    }

    public static void StartDelayMethod(this MonoBehaviour mono, float time, Action callback)
    {
        mono.StartCoroutine(Delay(time, callback));
    }

    static IEnumerator Delay(float time, Action callback)
    {
        yield return new WaitForSecondsRealtime(time);
        if (callback != null)
        {
            callback.Invoke();
        }
        else
        {
            Log.Info("Call back is destroyed");
        }
    }

    //private static Dictionary<Action, IEnumerator> allInvoke = new Dictionary<Action, IEnumerator>();
    //public static void Invoke(this MonoBehaviour mono, Action callback, float time, float timeRate)
    //{
    //    Clear();
    //    Log.Info(allInvoke.Count);
    //    IEnumerator a = Invoke(callback, time, timeRate);
    //    if (!allInvoke.ContainsKey(callback))
    //    {
    //        allInvoke.Add(callback, a);
    //    }
    //    else
    //    {
    //        mono.StopInvoke(callback);
    //    }
    //    mono.StartCoroutine(a);
    //}

    //public static void Clear()
    //{
    //    var clone = new Dictionary<Action, IEnumerator>();
    //    clone = allInvoke;
    //    foreach (KeyValuePair<Action,IEnumerator> element in clone)
    //    {
    //        if (element.Value == null)
    //        {
    //            allInvoke.Remove(element.Key);
    //        }
    //    }

    //    foreach (KeyValuePair<Action, IEnumerator> element in clone)
    //    {
    //        if (element.Key.Target. == null)
    //        {
    //            allInvoke.Remove(element.Key);
    //        }
    //    }
    //}

    //public static void StopInvoke(this MonoBehaviour mono, Action callback)
    //{
    //    if (allInvoke.ContainsKey(callback))
    //    {
    //        mono.StopCoroutine(allInvoke[callback]);
    //        allInvoke.Remove(callback);
    //    }
    //}


    //private static void RemoveCorotine(Action callback)
    //{
    //    Log.Info(allInvoke.Count);
    //    if (allInvoke.ContainsKey(callback))
    //    {
    //        allInvoke.Remove(callback);
    //    }
    //    Log.Info(allInvoke.Count);
    //}

    //static IEnumerator Invoke(Action callback, float time, float timeRate)
    //{
    //    if (time >= 0)
    //    {
    //        yield return new WaitForSeconds(time);
    //    }
    //    if (callback != null)
    //    {
    //        callback.Invoke();
    //    }
    //    WaitForSeconds timeRateLoop = new WaitForSeconds(timeRate);
    //    while (true)
    //    {
    //        yield return timeRateLoop;
    //        if (callback != null)
    //        {
    //            callback.Invoke();
    //        }
    //        else
    //        {
    //            RemoveCorotine(callback);
    //            yield break;
    //            Log.Info("Call back is destroyed");
    //        }
    //    }
    //}
}