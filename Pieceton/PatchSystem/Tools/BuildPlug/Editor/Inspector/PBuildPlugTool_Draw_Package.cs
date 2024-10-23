using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Pieceton.Misc;

namespace Pieceton.BuildPlug
{
    public sealed class PBuildPlugTool_Draw_Package : PBuildPlugTool_DrawBase
    {
        private static bool isAbleBuild = true;

        public static void Draw(PBuildPlug _build_plug)
        {
            if (!DrawBegin(_build_plug, "Package"))
                return;

            isAbleBuild = true;

            GUILayout.BeginVertical("Box");
            {
                Draw_Package_Common();

                switch (objectInst.platform)
                {
                    case PiecetonPlatform.Android: Draw_Package_Android(); break;
                    case PiecetonPlatform.iOS: Draw_Package_iOS(); break;
                    default: PLog.AnyLogError("PBuildPlugTool_Draw_Package::Draw() Not implemantation. {0}", objectInst.platform); break;
                }

                if (IsAble_PackageBuild())
                {
                    if (GUILayout.Button("Build Package", GUILayout.Height(30)))
                    {
                        objectInst.AddExecuteFlag(PBuildPlug.PExecuteFlag.BuildPackage);
                    }
                }
            }
            GUILayout.EndVertical();
        }

        private static void Draw_Package_Common()
        {
            PBuildPlug.PBuildFlag flag = objectInst.buildFlag;

            bool hasDebug = EditorGUILayout.Toggle("Development", objectInst.HasBuildFlag(PBuildPlug.PBuildFlag.Debug));
            if (hasDebug) objectInst.AddBuildFlag(PBuildPlug.PBuildFlag.Debug);
            else objectInst.DelBuildFlag(PBuildPlug.PBuildFlag.Debug);

            // for test --//
            // EditorGUI.BeginDisabledGroup(!hasDebug);
            // {
            bool useSimulatePatch = EditorGUILayout.Toggle("Simulate Patch", objectInst.HasBuildFlag(PBuildPlug.PBuildFlag.SimulatePatch));
            if (useSimulatePatch) objectInst.AddBuildFlag(PBuildPlug.PBuildFlag.SimulatePatch);
            else objectInst.DelBuildFlag(PBuildPlug.PBuildFlag.SimulatePatch);
            // }
            // EditorGUI.EndDisabledGroup();

            // bool useStreamBundle = Helper_PiecetonBuildPlug.IsActivatedStreamingBundle(objectInst);

            // EditorGUI.BeginDisabledGroup(!useStreamBundle);
            // {
            bool useStream = EditorGUILayout.Toggle("Streaming bundle", objectInst.HasBuildFlag(PBuildPlug.PBuildFlag.StreamBundle));
            if (useStream) objectInst.AddBuildFlag(PBuildPlug.PBuildFlag.StreamBundle);
            else objectInst.DelBuildFlag(PBuildPlug.PBuildFlag.StreamBundle);
            // }
            // EditorGUI.EndDisabledGroup();

            if (flag != objectInst.buildFlag)
            {
                objectInst.SetHasDirty();
            }
        }

        private static void Draw_Package_Android()
        {
            MobileTextureSubtarget nTex = (MobileTextureSubtarget)EditorGUILayout.EnumPopup("texture compress", objectInst.aos_TextureSubtarget);
            if (nTex != objectInst.aos_TextureSubtarget)
            {
                EditorUserBuildSettings.androidBuildSubtarget = nTex;
                objectInst.aos_TextureSubtarget = nTex;
                objectInst.SetHasDirty();
            }

            AndroidBuildSystem nVal = (AndroidBuildSystem)EditorGUILayout.EnumPopup("build system", objectInst.aos_BuildSystem);
            if (nVal != objectInst.aos_BuildSystem)
            {
                switch (nVal)
                {
                    case AndroidBuildSystem.Gradle:
                        //case AndroidBuildSystem.ADT:
                        {
                            objectInst.aos_BuildSystem = nVal;
                            EditorUserBuildSettings.androidBuildSystem = nVal;
                            objectInst.SetHasDirty();
                        }
                        break;
                    case AndroidBuildSystem.VisualStudio:
                        {
                            Debug.Log("AndroidBuildSystem.VisualStudio: ????????????");
                        }
                        break;
                }

                if (/*nVal != AndroidBuildSystem.ADT && */nVal != AndroidBuildSystem.Gradle)
                {
                    objectInst.exportAndroidProject = false;
                    EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                }
            }

            ScriptingImplementation scriptBackend = (ScriptingImplementation)EditorGUILayout.EnumPopup("script backend", objectInst.androidScriptBackend);
            if (scriptBackend != objectInst.androidScriptBackend)
            {
                switch (scriptBackend)
                {
                    case ScriptingImplementation.Mono2x:
                    case ScriptingImplementation.IL2CPP:
                        objectInst.androidScriptBackend = scriptBackend;
                        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, scriptBackend);
                        break;

                    default:
                        Debug.LogWarning("Unknown ScriptBackEnd");
                        break;
                }

                objectInst.SetHasDirty();
            }

            if (nVal == AndroidBuildSystem.Gradle)
            {
                objectInst.exportAndroidProject = EditorGUILayout.Toggle("Export Project", objectInst.exportAndroidProject);
                EditorUserBuildSettings.exportAsGoogleAndroidProject = objectInst.exportAndroidProject;
            }

            if (!objectInst.exportAndroidProject)
            {
                objectInst.androidBuildAndRun = EditorGUILayout.Toggle("Build and Run", objectInst.androidBuildAndRun);
            }

            if (nVal != objectInst.aos_BuildSystem)
            {
                objectInst.SetHasDirty();
            }
        }

        private static void Draw_Package_iOS()
        {
            bool hasFirst = EditorGUILayout.Toggle("First build", objectInst.HasBuildFlag(PBuildPlug.PBuildFlag.First));
            if (hasFirst) objectInst.AddBuildFlag(PBuildPlug.PBuildFlag.First);
            else objectInst.DelBuildFlag(PBuildPlug.PBuildFlag.First);
            if (hasFirst != objectInst.HasBuildFlag(PBuildPlug.PBuildFlag.First))
            {
                objectInst.SetHasDirty();
            }

            EditorGUILayout.LabelField("XCode Export type");
            ++EditorGUI.indentLevel;
            {
                PBuildPlug.PIOSCertificationFlag tFlag = objectInst.ios_certificationFlag;

                bool hasDevelop = EditorGUILayout.Toggle("development", objectInst.HasCertificationFlag(PBuildPlug.PIOSCertificationFlag.Development));
                if (hasDevelop) objectInst.AddCertificationFlag(PBuildPlug.PIOSCertificationFlag.Development);
                else objectInst.DelCertificationFlag(PBuildPlug.PIOSCertificationFlag.Development);

                bool hasAppStore = EditorGUILayout.Toggle("app store", objectInst.HasCertificationFlag(PBuildPlug.PIOSCertificationFlag.AppStore));
                if (hasAppStore) objectInst.AddCertificationFlag(PBuildPlug.PIOSCertificationFlag.AppStore);
                else objectInst.DelCertificationFlag(PBuildPlug.PIOSCertificationFlag.AppStore);

                bool hasAdHoc = EditorGUILayout.Toggle("ad hoc", objectInst.HasCertificationFlag(PBuildPlug.PIOSCertificationFlag.AdHoc));
                if (hasAdHoc) objectInst.AddCertificationFlag(PBuildPlug.PIOSCertificationFlag.AdHoc);
                else objectInst.DelCertificationFlag(PBuildPlug.PIOSCertificationFlag.AdHoc);

                bool hasEnterprise = EditorGUILayout.Toggle("enterprise", objectInst.HasCertificationFlag(PBuildPlug.PIOSCertificationFlag.Enterprise));
                if (hasEnterprise) objectInst.AddCertificationFlag(PBuildPlug.PIOSCertificationFlag.Enterprise);
                else objectInst.DelCertificationFlag(PBuildPlug.PIOSCertificationFlag.Enterprise);

                if (!hasDevelop && !hasAppStore && !hasEnterprise && !hasAdHoc)
                {
                    string errMsg = "You must select at least one Export type.";
                    EditorGUILayout.HelpBox(errMsg, MessageType.Error);
                    isAbleBuild = false;
                }

                if (tFlag != objectInst.ios_certificationFlag)
                {
                    objectInst.SetHasDirty();
                }
            }
            --EditorGUI.indentLevel;
        }

        private static bool IsAble_PackageBuild()
        {
            return isAbleBuild;
        }
    }
}
