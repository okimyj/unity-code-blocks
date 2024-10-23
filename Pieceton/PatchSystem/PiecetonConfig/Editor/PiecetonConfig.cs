using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pieceton.Configuration
{
    public static partial class PiecetonConfig
    {
        public const string PIECETON = "Pieceton";
        public const string BUILD_PLUG_PREFERENCE_NAME = "BuildPlugPreference.asset";

        public static class Path
        {
            public const string PATH_PIECETON_ROOT = PIECETON + "/";
            public const string PATH_LOCALIZATION_BINARY_ROOT = PATH_PIECETON_ROOT + "Generated/Localize/";

#if UNITY_EDITOR
            private const string PIECETON_ROOT = "/Plugins/Pieceton/";
            public static readonly string PIECETON_SYSTEM_ROOT = Application.dataPath + PIECETON_ROOT;
            public const string PIECETON_ASSET_ROOT = "Assets" + PIECETON_ROOT;
            public const string PIECETON_PATCHSYSTEM_ROOT = PIECETON_ASSET_ROOT + "PatchSystem/";
            public const string PIECETON_BUILD_PLUG_ROOT = PIECETON_PATCHSYSTEM_ROOT + "BuildPlugFiles/";
            public const string PIECETON_RESOURCES_ROOT = PIECETON_ASSET_ROOT + "Resources/";

            public const string PIECETON_TOOLS_ROOT = PIECETON_PATCHSYSTEM_ROOT + "Tools/";
            public const string PIECETON_TOOLS_PREFERENCE_ROOT = PIECETON_TOOLS_ROOT + "BuildPlugPreference/";
#endif
        }

        public static class Menu
        {
            public const string MENU_PIECETON_ROOT = "Window/" + PIECETON + "/";
            public const string MENU_PIECETON_INSPECTOR = MENU_PIECETON_ROOT + "BuildPlug Inspector (default)";
        }

        public static class ComponentMenu
        {
            public const string COMPONENT_MENU_ROOT = PIECETON;
        }
    }
}