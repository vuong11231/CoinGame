using UnityEngine;

namespace SteveRogers
{
    public partial class DebugMan : SingletonPersistent<DebugMan>
    {
        [Header("log genreral")]

        [SerializeField]
        private bool log_General = false;

        [SerializeField, Space(5)]
        private bool log_FirebaseStorage= false;
        public static bool Log_FirebaseStorage { get { return Instance && (Instance.log_General || Instance.log_FirebaseStorage); } }

        [SerializeField]
        private bool log_NetMan = false;        
        public static bool Log_NetMan { get { return Instance && (Instance.log_NetMan); } }
    }
}