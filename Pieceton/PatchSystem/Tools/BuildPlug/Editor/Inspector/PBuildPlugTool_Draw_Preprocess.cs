using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace Pieceton.BuildPlug
{
    public class PBuildPlugTool_Draw_Preprocess : PBuildPlugTool_DrawBase
    {
        public static void DrawInit(PBuildPlug _build_plug, ref ReorderableList _reorderableList)
        {
            _reorderableList = new ReorderableList(_build_plug.preProcessors, typeof(PBuildPreProcessor), true, true, true, true);
            _reorderableList.elementHeight = (EditorGUIUtility.singleLineHeight + 4) * 2 + 8;
            _reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Build Pre Processors");
            };
            _reorderableList.onAddCallback = (ReorderableList list) =>
            {
                _build_plug.preProcessors.Add(new PBuildPreProcessor());
                _build_plug.SetHasDirty();
            };
            _reorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                _build_plug.preProcessors.RemoveAt(list.index);
                _build_plug.SetHasDirty();
            };

            _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = _build_plug.preProcessors[index];
                rect.height -= 4;
                rect.y += 2;

                rect.height = EditorGUIUtility.singleLineHeight + 4;
                rect.y += 4;

                element.script = EditorGUI.ObjectField(rect, element.script, typeof(MonoScript), false);
                rect.y += EditorGUIUtility.singleLineHeight + 8;
                if (EditorGUI.DropdownButton(rect, new GUIContent(string.IsNullOrEmpty(element.methodName) ? "Select Method" : element.methodName), FocusType.Passive))
                {
                    var methods = ((MonoScript)element.script).GetClass().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);
                    GenericMenu menu = new GenericMenu();
                    foreach (var method in methods)
                    {
                        menu.AddItem(new GUIContent(method.Name), false, () =>
                        {
                            element.methodName = method.Name;
                        });
                    }
                    menu.ShowAsContext();
                }
            };
        }
        public static void Draw(PBuildPlug _build_plug, ref ReorderableList _reorderableList)
        {
            if (!DrawBegin(_build_plug, "PreProcess"))
                return;
            _reorderableList.DoLayoutList();
            if (_build_plug.preProcessors != null && _build_plug.preProcessors.Count > 0)
            {
                if (GUILayout.Button("Run Pre Process", GUILayout.Height(30)))
                {

                    RunBuildPreProcessors(_build_plug.name);
                }
            }
        }
        public static void RunBuildPreProcessors(string _build_plug_name) { RunBuildPreProcessors(PBuildPlug.LoadObject(_build_plug_name)); }
        public static void RunBuildPreProcessors(PBuildPlug _plug)
        {
            Debug.Log("RunBuildPreProcessors - start.");
            var taskList = new List<Task>();
            for (int i = 0; i < _plug.preProcessors.Count; ++i)
            {
                var preProcessor = _plug.preProcessors[i];
                if (preProcessor.script == null)
                {
                    Debug.LogError("Pre Process Script is null");
                    continue;
                }
                if (string.IsNullOrEmpty(preProcessor.methodName))
                {
                    Debug.LogError("Pre Process Method is null");
                    continue;
                }
                var method = ((MonoScript)preProcessor.script).GetClass().GetMethod(preProcessor.methodName);
                if (method == null)
                {
                    Debug.LogError($"Cannot found Build Pre Process method. scriptName : {preProcessor.script.name} methodName : {preProcessor.methodName}");
                    continue;
                }
                Debug.Log($"RunBuildPreProcessors - Run scriptName : {preProcessor.script.name} methodName : {preProcessor.methodName}");
                method.Invoke(_plug, null);
            }
        }
    }

}
