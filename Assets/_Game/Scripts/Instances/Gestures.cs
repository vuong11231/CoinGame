using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gestures : Singleton<Gestures>
{
    static UnityEngine.EventSystems.EventSystem EventSystemGO;
    public static void Enable()
    {
        //if (EventSystemGO == null)
        //{
        //    var go = GameObject.FindWithTag("Event System");
        //    if(go != null)
        //    {
        //        EventSystemGO = go.GetComponent<UnityEngine.EventSystems.EventSystem>();
        //    }
        //}

        //if(EventSystemGO != null)
        //    EventSystemGO.enabled = true;
    }

    public static void Disable()
    {
        //if (EventSystemGO == null)
        //{
        //    var go = GameObject.FindWithTag("Event System");
        //    if (go != null)
        //    {
        //        EventSystemGO = go.GetComponent<UnityEngine.EventSystems.EventSystem>();
        //    }
        //}

        //if (EventSystemGO != null)
        //    EventSystemGO.enabled = false;
    }
}
