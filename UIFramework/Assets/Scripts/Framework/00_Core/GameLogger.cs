using System;
using UnityEngine;

namespace YJFramework.Core
{
    public class GameLogger
    {
#if UNITY_EDITOR
        public static bool ENABLE_LOG = true;
#elif DEVENV_DEV
        public static bool ENABLE_LOG = true;
#else
        public static bool ENABLE_LOG = false;
#endif
        public static void Log(string msg, params object[] args)
        {
            if (ENABLE_LOG)
            {
                try
                {
                    Debug.LogFormat(msg, args);
                }
                catch (Exception e)
                {
                    Debug.LogError(e + "\n" + e.StackTrace);
                }

            }
        }

        public static void LogWarning(string msg, params object[] args)
        {
            if (ENABLE_LOG)
            {
                Debug.LogWarningFormat(msg, args);
            }
        }

        public static void LogError(string msg, params object[] args)
        {
            Debug.LogErrorFormat("[ERROR] {0}", string.Format(msg, args));
        }

        public static void LogException(Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}