using UnityEngine;

namespace SteveRogers
{
    public class SetDontDestroyThenDestroyThis : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            Destroy(this);
        }
    }
}