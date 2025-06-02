using UnityEngine;

namespace UIFramework.Core
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_instance = null;
        private static readonly object m_syncobj = new object();
        private static bool m_appIsClosing = false;

        public static T Instance
        {
            get
            {
                if (m_appIsClosing)
                    return null;

                if (m_instance == null)
                {
                    lock (m_syncobj)
                    {
                        if (m_instance == null)
                        {
                            T[] objs = FindObjectsOfType<T>();

                            if (objs.Length > 0)
                                m_instance = objs[0];
                            if (objs.Length > 1)
                                GameLogger.LogError("There is more than one " + typeof(T).Name + " in the scene.");

                            if (m_instance == null)
                            {
                                string goName = "(Singleton)" + typeof(T).ToString();
                                GameObject go = GameObject.Find(goName);
                                if (go == null)
                                    go = new GameObject(goName);
                                m_instance = go.AddComponent<T>();
                            }
                        }
                    }
                }
                return m_instance;
            }
        }

        public static bool IsExistInstance()
        {
            return m_instance != null && !m_appIsClosing;
        }

        protected virtual void OnApplicationQuit()
        {
            // release reference on exit
            m_appIsClosing = true;
        }
    }

}