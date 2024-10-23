using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pieceton.PatchSystem
{
    public partial class PlugPreferenceWindow
    {
        private void Draw_GameVersioin()
        {
            EditorGUILayout.LabelField("[ Game Version ]");
            EditorGUILayout.BeginVertical("Box");
            {
                PiecetonPlatform p = PiecetonPlatform.None + 1;
                for (; p < PiecetonPlatform.End; ++p)
                {
                    int i = (int)p;
                    EditorGUILayout.LabelField(string.Format("{0} version", p));

                    ++EditorGUI.indentLevel;
                    {
                        string ver = EditorGUILayout.TextField("gameVersion", PlugPreferenceInfo.instance.gameVersion[i]);
                        if (ver != PlugPreferenceInfo.instance.gameVersion[i])
                        {
                            PlugPreferenceInfo.instance.gameVersion[i] = ver;
                            SetNeedDirty();
                        }

                        string code = EditorGUILayout.TextField("versionCode", PlugPreferenceInfo.instance.versionCode[i]);
                        if (code != PlugPreferenceInfo.instance.versionCode[i])
                        {
                            PlugPreferenceInfo.instance.versionCode[i] = code;
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