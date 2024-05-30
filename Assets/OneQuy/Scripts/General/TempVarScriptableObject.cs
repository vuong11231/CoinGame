using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteveRogers
{
    [CreateAssetMenu(
      fileName = "TempVar",
      menuName = "Steve Rogers/TempVar File")]
    public class TempVarScriptableObject : ScriptableObject
    {
        [Header("float")]

        public float F1 = 0;
        public float F2 = 0;
        public float F3 = 0;
        public float F4 = 0;
        public float F5 = 0;


        [Header("int")]

        public int Int1 = 0;
        public int Int2 = 0;
        public int Int3 = 0;
        public int Int4 = 0;
        public int Int5 = 0;


        [Header("string")]

        public string S2 = null;
        public string S1 = null;
        public string S3 = null;
        public string S4 = null;
        public string S5 = null;

        [Header("bool")]

        public bool B2 = false;
        public bool B1 = false;

        [Header("lean")]

        public LeanTweenType
         tweenType1 = LeanTweenType.notUsed;
        public LeanTweenType
         tweenType2 = LeanTweenType.notUsed,
         tweenType3 = LeanTweenType.notUsed;
    }
}