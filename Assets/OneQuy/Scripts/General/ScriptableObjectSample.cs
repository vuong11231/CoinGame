using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteveRogers
{
    [CreateAssetMenu(
      fileName = ScriptableObjectSample.FILE_NAME,
      menuName = "Create " + ScriptableObjectSample.FILE_NAME)]
    public class ScriptableObjectSample : ScriptableObject
    {
        private const string FILE_NAME = "DataFile"; // ONLY CHANGE THIS!

        private static DataScriptableObject _Instance = null;

        public static DataScriptableObject Instance
        {
            get
            {
                if (_Instance != null)
                    return _Instance;
                else
                {
                    _Instance = Resources.Load<DataScriptableObject>(FILE_NAME);
                    return _Instance;
                }
            }
        }
    }
}