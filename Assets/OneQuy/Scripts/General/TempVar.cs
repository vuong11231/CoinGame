using UnityEngine;

namespace SteveRogers
{
    public class TempVar : SingletonPersistent<TempVar>
    {

        [SerializeField]
        private TempVarScriptableObject tempVarScriptableObject = null;

#if UNITY_EDITOR
        public static float f1 { get { return Instance.tempVarScriptableObject.F1; } }
        public static float f2 { get { return Instance.tempVarScriptableObject.F2; } }
        public static float f3 { get { return Instance.tempVarScriptableObject.F3; } }
        public static float f4 { get { return Instance.tempVarScriptableObject.F4; } }
        public static float f5 { get { return Instance.tempVarScriptableObject.F5; } }


        public static float Replace_F1(float defaultValue, float condition = 0)
        {
            if (f1 < condition)
                return defaultValue;
            else
                return f1;
        }

        public static float Replace_F2(float defaultValue, float condition = 0)
        {
            if (f2 < condition)
                return defaultValue;
            else
                return f2;
        }
    

        public static int int1 { get { return Instance.tempVarScriptableObject.Int1; } }
        public static int int2 { get { return Instance.tempVarScriptableObject.Int2; } }
        public static int int3 { get { return Instance.tempVarScriptableObject.Int3; } }
        public static int int4 { get { return Instance.tempVarScriptableObject.Int4; } }
        public static int int5 { get { return Instance.tempVarScriptableObject.Int5; } }


        public static string s1 { get { return Instance.tempVarScriptableObject.S1; } }
        public static string s2 { get { return Instance.tempVarScriptableObject.S2; } }
        public static string s3 { get { return Instance.tempVarScriptableObject.S3; } }
        public static string s4 { get { return Instance.tempVarScriptableObject.S4; } }
        public static string s5 { get { return Instance.tempVarScriptableObject.S5; } }

        public static bool b1 { get { return Instance.tempVarScriptableObject.B1; } }
        public static bool b2 { get { return Instance.tempVarScriptableObject.B2; } }


        public static LeanTweenType LeanTweenType1 { get { return Instance.tempVarScriptableObject.tweenType1; } }
        public static LeanTweenType LeanTweenType2 { get { return Instance.tempVarScriptableObject.tweenType2; } }
        public static LeanTweenType LeanTweenType3 { get { return Instance.tempVarScriptableObject.tweenType3; } }
#endif
    }
}