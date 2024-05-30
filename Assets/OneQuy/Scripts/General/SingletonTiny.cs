namespace SteveRogers
{
    public class SingletonTiny<T> where T : new()
    {
        private static T instance = default;

        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();

                return instance;
            }
        }
    }
}