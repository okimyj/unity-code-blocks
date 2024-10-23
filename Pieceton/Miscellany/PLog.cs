using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pieceton.Misc
{
    public static class PLog
    {
        public enum EFlag
        {
            None = 0,

            System = 1 << 0,
            Resource = 1 << 1,
            InGame = 1 << 2,
            UI = 1 << 3,
        }
        private static EFlag _logTypeFlag = EFlag.None;

        public static void AddFlag(EFlag _flag) { _logTypeFlag |= _flag; }
        public static void DelFlag(EFlag _flag) { _logTypeFlag &= ~_flag; }
        public static bool HasFlag(EFlag _flag) { return (0 != (_logTypeFlag & _flag)); }
        public static void ClearFlag() { _logTypeFlag = EFlag.None; }

        public static void SetShpping() { ClearFlag(); AddFlag(EFlag.System); }


        public static void FlagLog(EFlag _flag, string _msg) { if (!HasFlag(_flag)) return; Debug.Log(_msg); }
        public static void FlagLogWarning(EFlag _flag, string _msg) { if (!HasFlag(_flag)) return; Debug.LogWarning(_msg); }
        public static void FlagLogError(EFlag _flag, string _msg) { if (!HasFlag(_flag)) return; Debug.LogError(_msg); }

        public static void FlagLog(EFlag _flag, string _format, params object[] _args) { if (!HasFlag(_flag)) return; Debug.LogFormat(_format, _args); }
        public static void FlagLogWarning(EFlag _flag, string _format, params object[] _args) { if (!HasFlag(_flag)) return; Debug.LogWarningFormat(_format, _args); }
        public static void FlagLogError(EFlag _flag, string _format, params object[] _args) { if (!HasFlag(_flag)) return; Debug.LogErrorFormat(_format, _args); }


        public static void Log(string _msg) { if (!Debug.isDebugBuild) return; Debug.Log(_msg); }
        public static void LogWarning(string _msg) { if (!Debug.isDebugBuild) return; Debug.LogWarning(_msg); }
        public static void LogError(string _msg) { if (!Debug.isDebugBuild) return; Debug.LogError(_msg); }

        public static void Log(string _format, params object[] _args) { if (!Debug.isDebugBuild) return; Debug.LogFormat(_format, _args); }
        public static void LogWarning(string _format, params object[] _args) { if (!Debug.isDebugBuild) return; Debug.LogWarningFormat(_format, _args); }
        public static void LogError(string _format, params object[] _args) { if (!Debug.isDebugBuild) return; Debug.LogErrorFormat(_format, _args); }


        public static void AnyLog(string _msg) { Debug.Log(_msg); }
        public static void AnyLogWarning(string _msg) { Debug.LogWarning(_msg); }
        public static void AnyLogError(string _msg) { Debug.LogError(_msg); }

        public static void AnyLog(string _format, params object[] _args) { Debug.LogFormat(_format, _args); }
        public static void AnyLogWarning(string _format, params object[] _args) { Debug.LogWarningFormat(_format, _args); }
        public static void AnyLogError(string _format, params object[] _args) { Debug.LogErrorFormat(_format, _args); }

        public static void EditorLog(string _format, params object[] _args)
        {
#if UNITY_EDITOR
            Debug.LogFormat(_format, _args);
#endif
        }

        public static void EditorLogWarning(string _format, params object[] _args)
        {
#if UNITY_EDITOR
            Debug.LogWarningFormat(_format, _args);
#endif
        }

        public static void EditorLogError(string _format, params object[] _args)
        {
#if UNITY_EDITOR
            Debug.LogErrorFormat(_format, _args);
#endif
        }
    }
}