using System;

namespace UIFramework.Core
{
    public class Singleton<T> where T : class, new()
    {
        private static readonly Lazy<T> lazy = new Lazy<T>(() => new T());
        public static T Instance { get { return lazy.Value; } }

        protected static bool initializeLock;

        static Singleton()
        {

        }
    }
}
