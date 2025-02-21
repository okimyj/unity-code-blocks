using UnityEngine;

namespace UIFramework.Core
{
    public abstract class SingletonMonoDontDestroyBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = null;
        private static readonly object _syncobj = new object();
        private static bool appIsClosing = false;

        public static T Instance
        {
            get
            {
                if (appIsClosing)
                    return null;

                if (_instance == null)
                {
                    lock (_syncobj)
                    {
                        if (_instance == null)
                        {
                            T[] objs = FindObjectsOfType<T>();

                            if (objs.Length > 0)
                                _instance = objs[0];
                            if (objs.Length > 1)
                                GameLogger.LogError("There is more than one " + typeof(T).Name + " in the scene.");

                            if (_instance == null)
                            {
                                string goName = "(Singleton)" + typeof(T).ToString();
                                GameObject go = GameObject.Find(goName);
                                if (go == null)
                                    go = new GameObject(goName);
                                DontDestroyOnLoad(go);
                                _instance = go.AddComponent<T>();
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        public static bool IsExistInstance()
        {
            return _instance != null && !appIsClosing;
        }

        protected virtual void OnApplicationQuit()
        {
            appIsClosing = true;
        }
    }

}
