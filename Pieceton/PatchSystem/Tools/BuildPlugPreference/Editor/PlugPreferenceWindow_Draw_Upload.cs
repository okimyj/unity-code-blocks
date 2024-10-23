using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pieceton.PatchSystem
{
    public partial class PlugPreferenceWindow
    {
        private void Draw_UploadInfo()
        {
            EditorGUILayout.LabelField("[ Upload ]");
            EditorGUILayout.BeginVertical("Box");
            {
                PUploadProtocolType p = PUploadProtocolType.FTP;
                for (; p < PUploadProtocolType.End; ++p)
                {
                    int i = (int)p;
                    EditorGUILayout.LabelField(string.Format("{0} protocol", p));

                    ++EditorGUI.indentLevel;
                    {
                        string url = EditorGUILayout.TextField("upload url", PlugPreferenceInfo.instance.uploadUrl[i]);
                        if (url != PlugPreferenceInfo.instance.uploadUrl[i])
                        {
                            PlugPreferenceInfo.instance.uploadUrl[i] = url;
                            SetNeedDirty();
                        }

                        string id = EditorGUILayout.TextField("upload id", PlugPreferenceInfo.instance.uploadId[i]);
                        if (id != PlugPreferenceInfo.instance.uploadId[i])
                        {
                            PlugPreferenceInfo.instance.uploadId[i] = id;
                            SetNeedDirty();
                        }

                        string pw = EditorGUILayout.TextField("upload pw", PlugPreferenceInfo.instance.uploadPw[i]);
                        if (pw != PlugPreferenceInfo.instance.uploadPw[i])
                        {
                            PlugPreferenceInfo.instance.uploadPw[i] = pw;
                            SetNeedDirty();
                        }
                    }
                    --EditorGUI.indentLevel;
                    GUILayout.Space(10);
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}