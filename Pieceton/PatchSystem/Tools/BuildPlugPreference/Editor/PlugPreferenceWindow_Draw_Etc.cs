using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pieceton.PatchSystem
{
    public partial class PlugPreferenceWindow
    {
        private void Draw_Etc()
        {
            EditorGUILayout.LabelField("[ Etc ]");
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField("python path");
                GUILayout.Space(10);
                BuildMachineOSType t = BuildMachineOSType.None + 1;
                for (; t < BuildMachineOSType.End; ++t)
                {
                    int i = (int)t;

                    ++EditorGUI.indentLevel;
                    {
                        string prefix = string.Format("Machin OS Type : {0}", t);
                        string path = EditorGUILayout.TextField(prefix, PlugPreferenceInfo.instance.installedPythonPath[i]);
                        if (path != PlugPreferenceInfo.instance.installedPythonPath[i])
                        {
                            PlugPreferenceInfo.instance.installedPythonPath[i] = path;
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
