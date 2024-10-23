using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pieceton.PatchSystem
{
    public partial class PlugPreferenceWindow
    {
        private void Draw_BundleDownload()
        {
            EditorGUILayout.LabelField("[ BundleDownload ]");
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField("bundle download base url");
                GUILayout.Space(10);
                ReleaseType t = ReleaseType.None + 1;
                for (; t < ReleaseType.End; ++t)
                {
                    int i = (int)t;

                    ++EditorGUI.indentLevel;
                    {
                        string prefix = string.Format("ReleaseType : {0}", t);
                        string baseUrl = EditorGUILayout.TextField(prefix, PlugPreferenceInfo.instance.downloadBaseUrl[i]);
                        if (baseUrl != PlugPreferenceInfo.instance.downloadBaseUrl[i])
                        {
                            PlugPreferenceInfo.instance.downloadBaseUrl[i] = baseUrl;
                            SetNeedDirty();
                        }
                    }
                    --EditorGUI.indentLevel;
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}