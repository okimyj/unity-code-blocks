using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pieceton.PatchSystem
{
    public partial class PlugPreferenceWindow
    {
        private void Draw_ProductInfo()
        {
            EditorGUILayout.LabelField("[ ProductInfo ]");
            EditorGUILayout.BeginVertical("Box");
            {
                ++EditorGUI.indentLevel;
                {
                    string company = EditorGUILayout.TextField("Company Name", PlugPreferenceInfo.instance.CompanyName);
                    if (company != PlugPreferenceInfo.instance.CompanyName)
                    {
                        PlugPreferenceInfo.instance.CompanyName = company;
                        SetNeedDirty();
                    }

                    string product = EditorGUILayout.TextField("Product Name", PlugPreferenceInfo.instance.ProductName);
                    if (product != PlugPreferenceInfo.instance.ProductName)
                    {
                        PlugPreferenceInfo.instance.ProductName = product;
                        SetNeedDirty();
                    }

                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("Package Name (application identifier)");

                    ++EditorGUI.indentLevel;
                    {
                        PiecetonPlatform p = PiecetonPlatform.None + 1;
                        for (; p < PiecetonPlatform.End; ++p)
                        {
                            int i = (int)p;
                            string packageName = EditorGUILayout.TextField(p.ToString(), PlugPreferenceInfo.instance.PackageName[i]);
                            if (packageName != PlugPreferenceInfo.instance.PackageName[i])
                            {
                                PlugPreferenceInfo.instance.PackageName[i] = packageName;
                                SetNeedDirty();
                            }
                        }
                    }
                    --EditorGUI.indentLevel;
                }
                --EditorGUI.indentLevel;
            }
            EditorGUILayout.EndVertical();
        }
    }
}