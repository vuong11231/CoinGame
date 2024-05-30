using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteveRogers
{
    [CreateAssetMenu(
      fileName = TutManData.FILE_NAME,
      menuName = "Create " + TutManData.FILE_NAME)]
    public class TutManData : ScriptableObject
    {
        private const string FILE_NAME = "TutManData"; // ONLY CHANGE THIS!

        private static TutManData _Instance = null;

        public static TutManData Instance
        {
            get
            {
                if (_Instance != null)
                    return _Instance;
                else
                {
                    _Instance = Resources.Load<TutManData>(FILE_NAME);
                    return _Instance;
                }
            }
        }

        public List<TutMan.Step> steps = null;
    }
}