using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pieceton.PatchSystem
{
    public partial class PlugPreferenceWindow
    {
        private void Draw_Certificate()
        {
            EditorGUILayout.LabelField("[ Certificate ]");
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField(string.Format("{0} certification", PiecetonPlatform.Android));

                ++EditorGUI.indentLevel;
                {
                    string root = PBundlePathEditor.ProjectRoot();

                    EditorGUILayout.LabelField("Project Root", root);

                    string subPath = EditorGUILayout.TextField("KeyStore Sub Path", PlugPreferenceInfo.instance.keystoreName);
                    if (subPath != PlugPreferenceInfo.instance.keystoreName)
                    {
                        PlugPreferenceInfo.instance.keystoreName = subPath;
                        SetNeedDirty();
                    }

                    EditorGUILayout.Space(10);

                    EditorGUILayout.LabelField("KeyStoreName", PlugPreferenceInfo.instance.KeystoreFullPath());

                    string keyPass = EditorGUILayout.TextField("KeyStorePass", PlugPreferenceInfo.instance.keystorePass);
                    if (keyPass != PlugPreferenceInfo.instance.keystorePass)
                    {
                        PlugPreferenceInfo.instance.keystorePass = keyPass;
                        SetNeedDirty();
                    }

                    string aliasName = EditorGUILayout.TextField("AliasName", PlugPreferenceInfo.instance.keyaliasName);
                    if (aliasName != PlugPreferenceInfo.instance.keyaliasName)
                    {
                        PlugPreferenceInfo.instance.keyaliasName = aliasName;
                        SetNeedDirty();
                    }

                    string aliasPass = EditorGUILayout.TextField("AliasPass", PlugPreferenceInfo.instance.keyaliasPass);
                    if (aliasPass != PlugPreferenceInfo.instance.keyaliasPass)
                    {
                        PlugPreferenceInfo.instance.keyaliasPass = aliasPass;
                        SetNeedDirty();
                    }
                }
                --EditorGUI.indentLevel;
            }
            EditorGUILayout.EndVertical();
        }
    }
}