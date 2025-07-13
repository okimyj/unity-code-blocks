using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public partial class LinkProperties : Editor
{
    [MenuItem("UI Menu/Link Properties")]
    [MenuItem("CONTEXT/MonoBehaviour/Link Properties")]
    public static void RunLinkProperties()
    {
        if (Selection.activeGameObject == null)
            return;
        
        MonoBehaviour[] ui = Selection.activeGameObject.GetComponents<MonoBehaviour>();
        if (ui == null)
            return;
        
        for (int i = 0; i < ui.Length; i++)
        {
            // UnityEngine 기본 component인 경우 continue.
            if (ui[i].GetType().Namespace?.StartsWith("UnityEngine") ?? false)
                continue;
            
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var fields = ui[i].GetType().GetFields(bindingFlags);
            Undo.RecordObject(ui[i], "Link Properties");
            foreach (var field in fields)
            {
                if (field.FieldType.IsArray)
                {
                    // 시작 인덱스 체크. 0으로 시작하는 녀석이 없다면 1부터 시작하도록.
                    int startIndex = 0;
                    Transform child0 = FindNodeRecursively(Selection.activeGameObject.transform, field.Name + "0");
                    if (child0 == null)
                        startIndex = 1;

                    List<Object> list = new List<Object>();
                    while (true)
                    {
                        Transform child = FindNodeRecursively(Selection.activeGameObject.transform, field.Name + (list.Count + startIndex).ToString());
                        if (child == null)
                            break;

                        if (field.FieldType.GetElementType() == typeof(GameObject))
                            list.Add(child.gameObject);
                        else
                        {
                            Component comp = child.GetComponent(field.FieldType.GetElementType());
                            if (comp == null)
                                break;

                            list.Add(comp);
                        }
                    }
                    System.Array filledArray = System.Array.CreateInstance(field.FieldType.GetElementType(), list.Count);
                    System.Array.Copy(list.ToArray(), filledArray, list.Count);
                    field.SetValue(ui[i], filledArray);
                }
                else
                {
                    Transform child = FindNodeRecursively(Selection.activeGameObject.transform, field.Name);
                    if (child != null)
                    {
                        if (field.FieldType == typeof(GameObject))
                            field.SetValue(ui[i], child.gameObject);
                        else
                        {
                            Component comp = child.GetComponent(field.FieldType);
                            if (comp != null)
                                field.SetValue(ui[i], comp);
                        }
                    }
                }
            }
        }
    }
    // node 부터 name 에 해당하는 child를 재귀호출로 찾는다.
    public static Transform FindNodeRecursively(Transform node, string name)
    {
        foreach (Transform child in node)
        {
            if (child.name.Equals(name))
                return child;
            
            Transform result = FindNodeRecursively(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
