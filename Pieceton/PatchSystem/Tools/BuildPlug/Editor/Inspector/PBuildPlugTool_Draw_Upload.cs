using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Pieceton.PatchSystem;

namespace Pieceton.BuildPlug
{
    public class PBuildPlugTool_Draw_Upload : PBuildPlugTool_DrawBase
    {
        public static void Draw(PBuildPlug _build_plug)
        {
            if (!DrawBegin(_build_plug, "Upload"))
                return;

            GUILayout.BeginVertical("Box");
            {
                Draw_PackageUpload(_build_plug);
                Draw_BundleUpload(_build_plug);
            }
            GUILayout.EndVertical();
        }

        private static void Draw_PackageUpload(PBuildPlug _build_plug)
        {
            GUILayout.BeginVertical("Box");
            {
                Draw_UploadProtocolType(_build_plug, UploadInfoType.Package, "package");

                if (GUILayout.Button("Upload Package", GUILayout.Height(30)))
                {
                    objectInst.AddExecuteFlag(PBuildPlug.PExecuteFlag.UploadPackage);
                }
            }
            GUILayout.EndVertical();
        }

        private static void Draw_BundleUpload(PBuildPlug _build_plug)
        {
            GUILayout.BeginVertical("Box");
            {
                Draw_UploadProtocolType(_build_plug, UploadInfoType.Bundle, "bundle");

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Upload Append", GUILayout.Height(30)))
                    {
                        objectInst.AddExecuteFlag(PBuildPlug.PExecuteFlag.UploadBundleAppend);
                    }

                    if (GUILayout.Button("Upload Override", GUILayout.Height(30)))
                    {
                        objectInst.AddExecuteFlag(PBuildPlug.PExecuteFlag.UploadBundleOverride);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        
        private static void Draw_UploadProtocolType(PBuildPlug _build_plug, UploadInfoType _info_type, string _label)
        {
            int index = (int)_info_type;

            PUploadProtocolType prevProtocol = _build_plug.uploadProtocol[index];
            PUploadProtocolType currProtocol = (PUploadProtocolType)EditorGUILayout.EnumPopup("protocol", prevProtocol);
            if (prevProtocol != currProtocol)
            {
                _build_plug.uploadProtocol[index] = currProtocol;
                //EditorUtility.SetDirty(_build_plug);
                _build_plug.SetHasDirty();
            }

            Toggle(_build_plug, "use backup for " + _label, ref _build_plug.useBackup[index]);
        }
    }
}