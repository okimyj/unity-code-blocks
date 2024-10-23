using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pieceton.BuildPlug
{
    public sealed class PBuildPlugTool_Draw_Bundle : PBuildPlugTool_DrawBase
    {
        private static bool fold_AssetBundle = false;
        public static void Draw(PBuildPlug _build_plug)
        {
            if (!DrawBegin(_build_plug, "Bundle"))
                return;
            
            GUILayout.BeginVertical("Box");
            {
                Draw_Check();

                if (GUILayout.Button("Build Bundle", GUILayout.Height(30)))
                {
                    objectInst.AddExecuteFlag(PBuildPlug.PExecuteFlag.BuildBundle);
                }

                Draw_Bundle();
            }
            GUILayout.EndVertical();
        }

        private static void Draw_Check()
        {
            PBuildPlug.PBundleCheckFlag flag = objectInst.bundleCheckFlag;

            bool hasDebug = EditorGUILayout.Toggle("DuplicateCheck", objectInst.HasBundleCheckFlag(PBuildPlug.PBundleCheckFlag.Duplicate));
            if (hasDebug) objectInst.AddBundleCheckFlag(PBuildPlug.PBundleCheckFlag.Duplicate);
            else objectInst.DelBundleCheckFlag(PBuildPlug.PBundleCheckFlag.Duplicate);

            if (flag != objectInst.bundleCheckFlag)
            {
                objectInst.SetHasDirty();
            }
        }

        private static void Draw_Bundle()
        {
            string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();

            ++EditorGUI.indentLevel;
            {
                fold_AssetBundle = EditorGUILayout.Foldout(fold_AssetBundle, "AssetBundles");
                if (fold_AssetBundle)
                {
                    ++EditorGUI.indentLevel;
                    {
                        foreach (string bundle in bundleNames)
                            EditorGUILayout.LabelField(bundle);
                    }
                    --EditorGUI.indentLevel;
                }
            }
            --EditorGUI.indentLevel;
        }
    }
}