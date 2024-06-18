using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class ReferencesFinder : EditorWindow
{
    [MenuItem("RKTools/References Finder")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ReferencesFinder));
    }
    public class AssetComponentData
    {
        public Component comp;
    }
    public class ObjectData
    {
        public GameObject obj;
        public List<AssetComponentData> usingList;
        public bool IsFoldOut;
        public ObjectData()
        {
            usingList = new List<AssetComponentData>();
        }
        public void AddAssetComponent(Component component)
        {
            usingList.Add(new AssetComponentData() { comp = component });
        }

    }
    private Vector2 scroll = Vector2.zero;
    private Object target;
    private List<ObjectData> foundObjects = new List<ObjectData>();
    private void OnGUI()
    {
        GUILayout.Label("Find References", EditorStyles.boldLabel);

        target = (Object)EditorGUILayout.ObjectField("Target", target, typeof(Object), true);
        if (GUILayout.Button("Find"))
        {
            if (target != null)
            {
                FindUsages();
            }
        }
        DrawFoundObjects();
    }
    private void FindUsages()
    {
        foundObjects.Clear();
        if (target is Font)
        {
            FindUsingFontAsset();
        }
        else if (target is TMP_FontAsset)
        {
            FindUsingTMPFontAsset();
        }
    }

    private void FindUsingFontAsset()
    {
        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab");
        var targetFont = target as Font;

        foreach (string prefabGUID in allPrefabs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var texts = prefab.GetComponentsInChildren<Text>(true);
            var objectData = new ObjectData() { obj = prefab };
            bool isFound = false;
            foreach (var text in texts)
            {
                if (text.font == targetFont)
                {
                    isFound = true;
                    objectData.AddAssetComponent(text);
                }
            }
            if (isFound)
                foundObjects.Add(objectData);
        }
    }
    private void FindUsingTMPFontAsset()
    {
        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab");
        var targetFont = target as TMP_FontAsset;
        foreach (string prefabGUID in allPrefabs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            TextMeshProUGUI[] textsTMP = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
            var objectData = new ObjectData() { obj = prefab };
            bool isFound = false;
            foreach (TextMeshProUGUI text in textsTMP)
            {
                if (text.font == targetFont)
                {
                    isFound = true;
                    objectData.AddAssetComponent(text);
                }
            }
            if (isFound)
                foundObjects.Add(objectData);
        }
    }

    private void DrawFoundObjects()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.BeginVertical();
        if (foundObjects == null || foundObjects.Count == 0)
        {
            EditorGUILayout.HelpBox("No references found", MessageType.Info);
        }
        else
        {
            foreach (var data in foundObjects)
            {
                EditorGUILayout.BeginHorizontal();
                data.IsFoldOut = EditorGUILayout.Foldout(data.IsFoldOut, data.obj.name);
                EditorGUILayout.ObjectField(data.obj, data.GetType(), true);
                EditorGUILayout.EndHorizontal();
                if (data.IsFoldOut)
                {
                    EditorGUILayout.BeginVertical();
                    foreach (var assetComp in data.usingList)
                    {
                        EditorGUILayout.ObjectField(assetComp.comp, assetComp.comp.GetType(), true);
                    }
                    EditorGUILayout.EndVertical();
                }

            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }
}
