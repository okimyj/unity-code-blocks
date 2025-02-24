using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class UIMenu : Editor
{
    [MenuItem("UI Menu/Link Properties")]
    static public void LinkProperties()
    {
        if (null != Selection.activeGameObject)
        {
            MonoBehaviour[] ui = Selection.activeGameObject.GetComponents<MonoBehaviour>();
            if (ui != null)
            {
                for (int i = 0; i < ui.Length; i++)
                {
                    if (!ui[i].GetType().ToString().Contains("Unity"))
                    {
                        Debug.Log("ui.GetType() - " + ui[i].GetType());
                        var bindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
                        var props = ui[i].GetType().GetMembers(bindingFlags).Where(_ => _.MemberType == System.Reflection.MemberTypes.Field);
                        foreach (var prop in props)
                        {
                            System.Reflection.FieldInfo fieldInfo = (System.Reflection.FieldInfo)prop;
                            if (fieldInfo.FieldType.IsArray)
                            {
                                int startIndex = 0;
                                Transform child0 = FindNodeRecursively(Selection.activeGameObject.transform, prop.Name + "0");
                                if (child0 == null)
                                    startIndex = 1;

                                List<Object> list = new List<Object>();
                                while (true)
                                {
                                    Transform child = FindNodeRecursively(Selection.activeGameObject.transform, prop.Name + (list.Count + startIndex).ToString());
                                    if (child != null)
                                    {
                                        if (fieldInfo.FieldType.GetElementType().ToString().Contains("GameObject"))
                                            list.Add(child.gameObject);
                                        else
                                        {
                                            Component comp = child.GetComponent(fieldInfo.FieldType.GetElementType());
                                            if (comp != null)
                                            {
                                                list.Add(comp);
                                            }
                                            else
                                                break;
                                        }
                                    }
                                    else
                                        break;
                                }
                                System.Array filledArray = System.Array.CreateInstance(fieldInfo.FieldType.GetElementType(), list.Count);
                                System.Array.Copy(list.ToArray(), filledArray, list.Count);
                                fieldInfo.SetValue(ui[i], filledArray);
                                Debug.Log(string.Format("{0} is linked.", prop.Name) + "," + fieldInfo.FieldType);
                            }
                            else
                            {
                                Transform child = FindNodeRecursively(Selection.activeGameObject.transform, prop.Name);
                                if (child != null)
                                {
                                    if (fieldInfo.FieldType.ToString().Contains("GameObject"))
                                    {
                                        fieldInfo.SetValue(ui[i], child.gameObject);
                                        Debug.Log(string.Format("{0} is linked.", prop.Name) + "," + fieldInfo.FieldType);
                                    }
                                    else
                                    {
                                        Component comp = child.GetComponent(fieldInfo.FieldType);
                                        if (comp != null)
                                        {
                                            fieldInfo.SetValue(ui[i], comp);
                                            Debug.Log(string.Format("{0} is linked.", prop.Name) + "," + fieldInfo.FieldType);
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                }
            }
        }
    }

    public static Transform FindNodeRecursively(Transform node, string name, bool equal = true)
    {
        foreach (Transform child in node)
        {
            if (equal && child.name.Equals(name))
                return child;
            else if (!equal && child.name.Contains(name))
                return child;
            Transform result = FindNodeRecursively(child, name, equal);
            if (result != null)
                return result;
        }
        return null;
    }
}
