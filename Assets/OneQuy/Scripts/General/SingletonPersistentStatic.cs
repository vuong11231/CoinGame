using UnityEngine;

namespace SteveRogers
{
    public class SingletonPersistentStatic<T> : SingletonPersistent<T> where T : Component
    {
        protected static void CheckInit()
        {
            if (!Instance)
                throw new System.Exception("not instance found at type: " + typeof(T));
        }
    }
}