using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pieceton.Misc
{
    public static class PBuildUtil
    {
        //public static string AndroidSDKPath
        //{
        //    get { return EditorPrefs.GetString("AndroidSDKRoot"); }
        //    set { EditorPrefs.SetString("AndroidSDKRoot", value); }
        //}

        public static string[] FindEnabledEditorScenes()
        {
            List<string> EditorScenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled)
                    continue;
                EditorScenes.Add(scene.path);
            }
            return EditorScenes.ToArray();
        }

        public static string AddDefineSymbols(string _origin, List<string> _add_symbols)
        {
            if (null == _add_symbols)
                return _origin;

            string defineSymbol = _origin;

            int count = _add_symbols.Count;
            for (int i = 0; i < count; ++i)
            {
                if (!string.IsNullOrEmpty(_add_symbols[i]))
                {
                    if (!defineSymbol.Equals(""))
                    {
                        defineSymbol += ";";
                    }
                    defineSymbol += _add_symbols[i];
                }
            }

            return defineSymbol;
        }

        public static BuildTargetGroup BuildTargetToBuildTargetGroup(BuildTarget _build_target)
        {
            switch (_build_target)
            {
                case BuildTarget.Android: return BuildTargetGroup.Android;
                case BuildTarget.iOS: return BuildTargetGroup.iOS;
                case BuildTarget.WSAPlayer: return BuildTargetGroup.WSA;
            }

            return BuildTargetGroup.Unknown;
        }

        public static bool HasBuildOption(BuildOptions _cur_option, BuildOptions _search_option)
        {
            return (0 != (_cur_option & _search_option));
        }

        public static BuildOptions AddBuildOption(BuildOptions _cur_option, BuildOptions _search_option)
        {
            if (!HasBuildOption(_cur_option, _search_option))
                _cur_option |= _search_option;

            return _cur_option;
        }

        public static BuildOptions RemoveBuildOption(BuildOptions _cur_option, BuildOptions _search_option)
        {
            if (HasBuildOption(_cur_option, _search_option))
                _cur_option &= ~_search_option;

            return _cur_option;
        }
    }
}