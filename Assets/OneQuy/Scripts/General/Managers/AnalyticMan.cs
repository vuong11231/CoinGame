#if SteveAnalytics

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Firebase.Analytics;

namespace SteveRogers
{
	public class AnalyticMan
	{
        public static void Fuck(string eventName)
        {
            if (Player.MelodyInSeconds.HasDevFile)
                return;

            FirebaseAnalytics.LogEvent(eventName);
            Analytics.CustomEvent(eventName);
        }

        public static void Fuck(string eventName, string paramName, string value)
        {
            if (Player.MelodyInSeconds.HasDevFile)
                return;

            FirebaseAnalytics.LogEvent(eventName, paramName, value);
                        
            Analytics.CustomEvent(eventName, new Dictionary<string, object>
            {
                { paramName, value }
            });
        }

        public static void Fuck(string eventName, string paramName, int value)
        {
            if (Player.MelodyInSeconds.HasDevFile)
                return;

            FirebaseAnalytics.LogEvent(eventName, paramName, value);
                        
            Analytics.CustomEvent(eventName, new Dictionary<string, object>
            {
                { paramName, value }
            });
        }

        public static void Fuck(string eventName, Dictionary<string, object> parameters)
        {
            if (Player.MelodyInSeconds.HasDevFile)
                return;

            var dta = new Parameter[parameters.Count];
            int i = 0;

            foreach (var p in parameters)
            {
                if (p.Value is int)
                    dta[i] = new Parameter(p.Key, (long)p.Value);
                else if (p.Value is string)
                    dta[i] = new Parameter(p.Key, p.Value.ToString());
                else
                    throw GlobalSteves.ExceptionMan.GetException("not implement");

                i++;
            }

            FirebaseAnalytics.LogEvent(eventName, dta);
            Analytics.CustomEvent(eventName, parameters);
        }
    }
}
#endif // SteveAnalytics