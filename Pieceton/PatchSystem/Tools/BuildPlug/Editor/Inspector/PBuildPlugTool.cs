using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

using Pieceton.PatchSystem;
using Pieceton.Misc;
using Pieceton.Configuration;
using UnityEditorInternal;
using Newtonsoft.Json.Serialization;

namespace Pieceton.BuildPlug
{
    [CustomEditor(typeof(PBuildPlug))]
    public partial class PBuildPlugTool : Editor
    {
        private PBuildPlug instance;

        [MenuItem(PiecetonConfig.Menu.MENU_PIECETON_INSPECTOR)]
        public static void OpenInspector()
        {
            PBuildPlug obj = PBuildPlug.LoadCreateObject(PBuildPlug.configAssetName);
            obj.ClearExecuteFlag();
            Selection.activeObject = obj;
            obj.SetHasDirty();
        }

        SerializedObject serialObj;
        SerializedProperty myArray;
        ReorderableList _reorderablePreProcessors;

        private void OnEnable()
        {
            instance = (PBuildPlug)target;
            instance.ClearExecuteFlag();
            PBuildPlug.Initialize(instance);

            serialObj = new SerializedObject(target);
            myArray = serialObj.FindProperty("Symbols");
            PBuildPlugTool_Draw_Preprocess.DrawInit(instance, ref _reorderablePreProcessors);
        }

        public override void OnInspectorGUI()
        {
            serialObj.Update();

            string openWindowDesc = string.Format("Open Preference Window (ver. {0})", PiecetonVersion.version);

            if (GUILayout.Button(openWindowDesc, GUILayout.Height(30)))
            {
                PlugPreferenceWindow.OpenWindow();
            }

            PBuildPlugTool_Draw_BuildInfo.Draw(instance);
            PBuildPlugTool_Draw_Preprocess.Draw(instance, ref _reorderablePreProcessors);
            PBuildPlugTool_Draw_Package.Draw(instance);
            PBuildPlugTool_Draw_Bundle.Draw(instance);
            PBuildPlugTool_Draw_Upload.Draw(instance);

            if (!instance.EmptyExecuteFlag())
            {
                EditorApplication.delayCall += OnDelayCall;
            }

            if (instance.hasDirty)
            {
                instance.ClearHasDirty();
                serialObj.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(instance);
            }

            //base.OnInspectorGUI();
        }

        protected virtual void OnDelayCall()
        {
            try
            {
                string plugName = instance.name;
                if (instance.HasExecuteFlag(PBuildPlug.PExecuteFlag.BuildPackage))
                {
                    BuildInfoGenerator.Generate(plugName);
                    PiecetonBuilder.Package(plugName);
                }
                else if (instance.HasExecuteFlag(PBuildPlug.PExecuteFlag.BuildBundle))
                {
                    BuildInfoGenerator.Generate(plugName);
                    PiecetonBuilder.Bundle(plugName);
                }
                else if (instance.HasExecuteFlag(PBuildPlug.PExecuteFlag.UploadPackage))
                {
                    PiecetonUploader.UploadPackage(instance);
                }
                else if (instance.HasExecuteFlag(PBuildPlug.PExecuteFlag.UploadBundleOverride))
                {
                    PiecetonUploader.UploadBundleOverride(instance);
                }
                else if (instance.HasExecuteFlag(PBuildPlug.PExecuteFlag.UploadBundleAppend))
                {
                    PiecetonUploader.UploadBundleAppend(instance);
                }
            }
            catch (Exception e)
            {
                PLog.AnyLogError("PBuildPlugEditor::OnDelayCall()\n {0}", e);
            }

            instance.ClearExecuteFlag();
        }
    }
}