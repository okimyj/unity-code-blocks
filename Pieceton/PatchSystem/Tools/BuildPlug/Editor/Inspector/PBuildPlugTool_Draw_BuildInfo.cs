using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Pieceton.Misc;
using Pieceton.PatchSystem;

namespace Pieceton.BuildPlug
{
    public sealed class PBuildPlugTool_Draw_BuildInfo : PBuildPlugTool_DrawBase
    {
        public static void Draw(PBuildPlug _build_plug)
        {
            if (!DrawBegin(_build_plug, "Build info"))
                return;

            GUILayout.BeginVertical("Box");
            {
                Draw_Platform();
                Draw_Release();

                Draw_PackageName();
                Draw_UseSvnRevision();
                Draw_DisplayAppName();
                Draw_AdditionalDefineSymbols();

                Draw_Override();

                Draw_Generate();
            }
            GUILayout.EndVertical();
        }

        private static void Draw_Platform()
        {
            PiecetonPlatform nVal = (PiecetonPlatform)EditorGUILayout.EnumPopup("platform", objectInst.platform);
            if (nVal != objectInst.platform)
            {
                if (nVal <= PiecetonPlatform.None || nVal >= PiecetonPlatform.End)
                    return;

                objectInst.platform = nVal;
                objectInst.SetHasDirty();
            }
        }

        private static void Draw_Release()
        {
            GUILayout.Space(10);

            ReleaseType nVal = (ReleaseType)EditorGUILayout.EnumPopup("release", objectInst.releaseType);
            if (nVal != objectInst.releaseType)
            {
                objectInst.releaseType = nVal;
                objectInst.SetHasDirty();
            }
        }

        private static void Draw_PackageName()
        {
            GUILayout.Space(10);

            if (objectInst.platform == PiecetonPlatform.iOS)
            {
                TextField(objectInst, "BI Enterprise", ref objectInst.iosEnterpriseCert);
            }
        }

        private static void Draw_UseSvnRevision()
        {
            int idx = (int)objectInst.platform;

            if (Toggle(objectInst, "Use svn revision", ref objectInst.UseSvnRevision[idx]))
            {

            }
        }

        private static void Draw_DisplayAppName()
        {
            if (objectInst.platform == PiecetonPlatform.iOS)
            {
                GUILayout.Space(10);

                if (TextField(objectInst, "Display App Name", ref objectInst.ios_AppDisplayName))
                {
                    PlayerSettings.iOS.applicationDisplayName = objectInst.ios_AppDisplayName;
                }
            }
        }

        private static void Draw_AdditionalDefineSymbols()
        {
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Additional define symbols");

            ++EditorGUI.indentLevel;
            {
                EditorGUILayout.LabelField("ReleaseSymbol", DefReleaseType.GetReleaseSymbol(objectInst.releaseType));

                int idx = (int)objectInst.platform;

                ListString symbols = objectInst.Symbols[idx];

                int count = symbols.list.Count;
                if (IntField(objectInst, "Additional count", ref count))
                {
                    Resize<string>(symbols.list, count);
                    objectInst.SetHasDirty();
                }

                ++EditorGUI.indentLevel;
                {
                    for (int i = 0; i < count; ++i)
                    {
                        string symbol = EditorGUILayout.TextField("added " + i, symbols.list[i]);
                        if (symbol != symbols.list[i])
                        {
                            symbols.list[i] = symbol;
                            objectInst.SetHasDirty();
                        }
                    }
                }
                --EditorGUI.indentLevel;

                objectInst.Symbols[idx] = symbols;
            }
            --EditorGUI.indentLevel;
        }

        private static void Draw_Override()
        {
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical("Box");
            {
                int idx = (int)objectInst.platform;

                bool overrideVersion = objectInst.overrideGameVersion;

                string gameVer = (overrideVersion ? objectInst.GameVersion[idx] : PlugPreferenceInfo.instance.gameVersion[idx]);
                string codeVer = (overrideVersion ? objectInst.VersionCode[idx] : PlugPreferenceInfo.instance.versionCode[idx]);

                overrideVersion = EditorGUILayout.ToggleLeft("Override", overrideVersion);
                if (overrideVersion != objectInst.overrideGameVersion)
                {
                    objectInst.overrideGameVersion = overrideVersion;
                    objectInst.SetHasDirty();
                }

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("GameVersion", gameVer);

                    EditorGUI.BeginDisabledGroup(!overrideVersion);
                    {
                        TextField(objectInst, ref objectInst.GameVersion[idx]);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("VersionCode", codeVer);

                    EditorGUI.BeginDisabledGroup(!overrideVersion);
                    {
                        TextField(objectInst, ref objectInst.VersionCode[idx]);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private static void Draw_Generate()
        {
            GUILayout.Space(10);

            if (GUILayout.Button("Generate ReleaseInfo", GUILayout.Height(30)))
            {
                BuildInfoGenerator.Generate(objectInst);
            }
        }
    }
}
