using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

using Pieceton.Misc;
using Pieceton.Configuration;

namespace Pieceton.PatchSystem
{
    public class BundleRefCheckWindow : EditorWindow
    {
        private static BundleRefCheckWindow _instance = null;

        private const float ONE_FIELD_MENU_WIDTH = 120.0f;

        private const float ONE_FIELD_WIDTH = 450.0f;
        private const float ONE_FIELD_HEIGHT = 600.0f;
        private static float curWindowWidth = ONE_FIELD_WIDTH * 2;
        private static float curWindowHeight = 1000.0f;

        private const float ITEM_VALUE_WIDTH = 40.0f;
        private const float ITEM_NAME_WIDTH = ONE_FIELD_WIDTH - ITEM_VALUE_WIDTH;

        private const float CHAR_WIDTH = 10.0f;

        private static int MAX_WIDTH_COUNT
        {
            get
            {
                float oneFieldWidth = (ONE_FIELD_WIDTH + 20.0f);
                float ExcludeMenuWidth = curWindowWidth - ONE_FIELD_MENU_WIDTH;

                int count = (int)(ExcludeMenuWidth / oneFieldWidth);
                if (count < 1)
                    count = 1;
                return count;
            }
        }

        [MenuItem(PiecetonMenuEditor.MENU_BUILD_PLUG_ROOT + "Tools/Bundle Reference Checker")]
        static void OpenBundleRefCheckTool()
        {
            if (null != _instance)
            {
                _instance.Close();
                _instance = null;
            }

            _instance = EditorWindow.GetWindow(typeof(BundleRefCheckWindow), false, "BundleRefCheckWindow") as BundleRefCheckWindow;
            _instance.minSize = new Vector2(curWindowWidth, ONE_FIELD_HEIGHT);

            Rect pos = _instance.position;
            pos.width = curWindowWidth;
            pos.height = curWindowHeight;

            _instance.position = pos;
        }

        void Update()
        {
            if (null == _instance)
            {
                _instance = EditorWindow.GetWindow(typeof(BundleRefCheckWindow), false, "BundleRefCheckWindow") as BundleRefCheckWindow;
            }

            curWindowWidth = _instance.position.width;

            Repaint();
        }

        private static string lookatBundleName = "";
        private static int lookatBundleRefCount = 0;

        private static string lookatAssetName = "";
        private static int lookatAssetRefCount = 0;
        private static Dictionary<string, LoadedAssetBundle> _listupShowingBundleDic = new Dictionary<string, LoadedAssetBundle>();
        private static Dictionary<Object, UsingAssetData> _listupShowingUsingAssetDic = new Dictionary<Object, UsingAssetData>();
        private static Dictionary<string, UsingAssetScene> _listupShowingUsingSceneDic = new Dictionary<string, UsingAssetScene>();

        private void OnGUI()
        {
            if (null == _instance)
                return;

            if (!PAssetBundleSimulate.active)
                return;

            ListupShowBundle();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical("Box", GUILayout.Width(ONE_FIELD_MENU_WIDTH));
                {
                    DrawControll_Menu();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    DrawControll_ShowBundle();
                    EditorGUILayout.Space();

                    DrawControll_ShowAsset();
                    EditorGUILayout.Space();

                    DrawControll_ShowScene();
                    EditorGUILayout.Space();

                    DrawControll_UnitTest();
                    EditorGUILayout.Space();

                    DrawControll_DontUnloadBundles();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                {
                    DrawLoadedBundle();
                    DrawUsingAssetData();
                    DrawUsingSceneData();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawControll_Menu()
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.Width(ONE_FIELD_MENU_WIDTH));
            {
                {
                    GUILayout.Label("Bundle");

                    EditorGUI.indentLevel++;

                    EditorGUILayout.LabelField("lookat bundle name");
                    lookatBundleName = EditorGUILayout.TextField(lookatBundleName);

                    lookatBundleRefCount = DrawIntField("lookat refCount", lookatBundleRefCount, ONE_FIELD_MENU_WIDTH);
                    if (lookatBundleRefCount < 0)
                        lookatBundleRefCount = 0;

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();

                {
                    GUILayout.Label("Asset");

                    EditorGUI.indentLevel++;

                    EditorGUILayout.LabelField("lookat asset name");
                    lookatAssetName = EditorGUILayout.TextField(lookatAssetName);

                    lookatAssetRefCount = DrawIntField("lookat refCount", lookatAssetRefCount, ONE_FIELD_MENU_WIDTH);
                    if (lookatAssetRefCount < 0)
                        lookatAssetRefCount = 0;

                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawControll_ShowBundle()
        {
            GUILayout.Label("Bundle");

            EditorGUILayout.BeginVertical("Box", GUILayout.Width(ONE_FIELD_MENU_WIDTH));
            {
                DrawLabelField("loaded count", BundleMgr.usingAssetDic.Count.ToString(), ONE_FIELD_MENU_WIDTH);
                DrawLabelField("ref cur/tot", FormatBundleRefCount(), ONE_FIELD_MENU_WIDTH);
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawControll_ShowAsset()
        {
            GUILayout.Label("Asset");

            EditorGUILayout.BeginVertical("Box", GUILayout.Width(ONE_FIELD_MENU_WIDTH));
            {
                DrawLabelField("using count", BundleMgr.usingAssetDic.Count.ToString(), ONE_FIELD_MENU_WIDTH);
                DrawLabelField("ref cur/tot", FormatAssetRefCount(), ONE_FIELD_MENU_WIDTH);
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawControll_ShowScene()
        {
            GUILayout.Label("Added Scene");

            EditorGUILayout.BeginVertical("Box", GUILayout.Width(ONE_FIELD_MENU_WIDTH));
            {
                DrawLabelField("using count", BundleMgr.usingSceneDic.Count.ToString(), ONE_FIELD_MENU_WIDTH);
                DrawLabelField("ref cur/tot", FormatSceneRefCount(), ONE_FIELD_MENU_WIDTH);
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawControll_UnitTest()
        {
            if (string.IsNullOrEmpty(TestBundleReference.executeTestName))
                return;

            GUILayout.Label("Unit test");

            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField(TestBundleReference.executeTestName);

                List<string> doList = TestBundleReference.doingList;
                int count = doList.Count;
                for (int i = 0; i < count; ++i)
                {
                    EditorGUILayout.LabelField(doList[i]);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawControll_DontUnloadBundles()
        {
            if (AssetBundleStorage.dontUnloadAssets.Count <= 0)
                return;

            GUILayout.Label("Dont unload bundles");

            EditorGUILayout.BeginVertical("Box");
            {
                List<string>.Enumerator iter = AssetBundleStorage.dontUnloadAssets.GetEnumerator();
                while (iter.MoveNext())
                {
                    EditorGUILayout.LabelField(iter.Current);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawLabelField(string _menu_name, string _value, float _width)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (_width > 0.0f)
                {
                    EditorGUILayout.LabelField(_menu_name, GUILayout.Width(_width));
                }
                else
                {
                    EditorGUILayout.LabelField(_menu_name);
                }

                EditorGUILayout.LabelField(_value, GUILayout.Width(_value.Length * CHAR_WIDTH));
            }
            EditorGUILayout.EndHorizontal();
        }

        private static int DrawIntField(string _menu_name, int _value, float _width)
        {
            int rVal = _value;
            EditorGUILayout.BeginHorizontal();
            {
                if (_width > 0.0f)
                {
                    EditorGUILayout.LabelField(_menu_name, GUILayout.Width(_width));
                }
                else
                {
                    EditorGUILayout.LabelField(_menu_name);
                }

                rVal = EditorGUILayout.IntField(_value);
            }
            EditorGUILayout.EndHorizontal();

            return rVal;
        }

        private static string DrawTextField(string _menu_name, string _value)
        {
            string rVal = _value;
            EditorGUILayout.BeginHorizontal();
            {
                //EditorGUILayout.LabelField(_menu_name, GUILayout.Width(ITEM_NAME_WIDTH));
                EditorGUILayout.LabelField(_menu_name);
                rVal = EditorGUILayout.TextField(_value);
            }
            EditorGUILayout.EndHorizontal();

            return rVal;
        }





        private static void ListupShowBundle()
        {
            _listupShowingBundleDic.Clear();

            Dictionary<string, LoadedAssetBundle>.Enumerator iter = AssetBundleStorage.loadedAssetBundles.GetEnumerator();
            while (iter.MoveNext())
            {
                string bundleName = iter.Current.Key;
                LoadedAssetBundle loadedBundle = iter.Current.Value;

                if (!string.IsNullOrEmpty(lookatBundleName))
                {
                    if (!bundleName.ToLower().Contains(lookatBundleName.ToLower()))
                        continue;
                }

                if (loadedBundle.referencedCount < lookatBundleRefCount)
                    continue;

                _listupShowingBundleDic.Add(bundleName, loadedBundle);
            }


            _listupShowingUsingAssetDic.Clear();
            Dictionary<Object, UsingAssetData>.Enumerator aIter = BundleMgr.usingAssetDic.GetEnumerator();
            while (aIter.MoveNext())
            {
                UsingAssetData data = aIter.Current.Value;
                string bundleName = data.bundleName;
                string assetName = data.assetName;

                if (!_listupShowingBundleDic.ContainsKey(bundleName))
                    continue;

                if (!string.IsNullOrEmpty(lookatAssetName))
                {
                    if (!assetName.ToLower().Contains(lookatAssetName.ToLower()))
                        continue;
                }

                if (data.refCount < lookatAssetRefCount)
                    continue;

                _listupShowingUsingAssetDic.Add(aIter.Current.Key, data);
            }


            _listupShowingUsingSceneDic.Clear();
            Dictionary<string, UsingAssetScene>.Enumerator sIter = BundleMgr.usingSceneDic.GetEnumerator();
            while (sIter.MoveNext())
            {
                UsingAssetScene data = sIter.Current.Value;
                string bundleName = data.bundleName;
                string assetName = data.assetName;

                if (!_listupShowingBundleDic.ContainsKey(bundleName))
                    continue;

                if (!string.IsNullOrEmpty(lookatAssetName))
                {
                    if (!assetName.ToLower().Contains(lookatAssetName.ToLower()))
                        continue;
                }

                if (data.refCount < lookatAssetRefCount)
                    continue;

                _listupShowingUsingSceneDic.Add(sIter.Current.Key, data);
            }
        }

        private static string FormatBundleRefCount()
        {
            int cur, tot;
            GetBundleRefCount(out cur, out tot);
            return string.Format("{0}/{1}", cur, tot);
        }

        private static string FormatAssetRefCount()
        {
            int cur, tot;
            GetAssetRefCount(out cur, out tot);
            return string.Format("{0}/{1}", cur, tot);
        }

        private static string FormatSceneRefCount()
        {
            int cur, tot;
            GetSceneRefCount(out cur, out tot);
            return string.Format("{0}/{1}", cur, tot);
        }

        private static void GetBundleRefCount(out int _curRef, out int _totRef)
        {
            _curRef = 0;
            _totRef = 0;

            Dictionary<string, LoadedAssetBundle>.Enumerator iter = _listupShowingBundleDic.GetEnumerator();
            while (iter.MoveNext())
            {
                _curRef += iter.Current.Value.referencedCount;
            }

            iter = AssetBundleStorage.loadedAssetBundles.GetEnumerator();
            while (iter.MoveNext())
            {
                _totRef += iter.Current.Value.referencedCount;
            }
        }

        private static void GetAssetRefCount(out int _curRef, out int _totRef)
        {
            _curRef = 0;
            _totRef = 0;

            Dictionary<Object, UsingAssetData>.Enumerator iter = _listupShowingUsingAssetDic.GetEnumerator();
            while (iter.MoveNext())
            {
                _curRef += iter.Current.Value.refCount;
            }

            iter = BundleMgr.usingAssetDic.GetEnumerator();
            while (iter.MoveNext())
            {
                _totRef += iter.Current.Value.refCount;
            }
        }

        private static void GetSceneRefCount(out int _curRef, out int _totRef)
        {
            _curRef = 0;
            _totRef = 0;

            Dictionary<string, UsingAssetScene>.Enumerator iter = _listupShowingUsingSceneDic.GetEnumerator();
            while (iter.MoveNext())
            {
                _curRef += iter.Current.Value.refCount;
            }

            iter = BundleMgr.usingSceneDic.GetEnumerator();
            while (iter.MoveNext())
            {
                _totRef += iter.Current.Value.refCount;
            }
        }

        private static void DrawLoadedBundleAt(LoadedAssetBundle _loaded_bundle)
        {
            DrawOneItem(_loaded_bundle.Name, _loaded_bundle.referencedCount);
        }

        private static void DrawUsingAssetAt(UsingAssetData _using_data)
        {
            string bundleAssetName = string.Format("{0}({1})", _using_data.assetName, _using_data.bundleName);
            DrawOneItem(bundleAssetName, _using_data.refCount);
        }

        private static void DrawUsingAssetSceneAt(UsingAssetScene _using_data)
        {
            string bundleAssetName = string.Format("{0}({1})", _using_data.assetName, _using_data.bundleName);
            DrawOneItem(bundleAssetName, _using_data.refCount);
        }

        private static void DrawOneItem(string _name, int _count)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(ONE_FIELD_WIDTH));
            {
                EditorGUILayout.LabelField(_name, GUILayout.Width(ITEM_NAME_WIDTH));
                string count = _count.ToString();
                EditorGUILayout.LabelField(count, GUILayout.Width(count.Length * CHAR_WIDTH));
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawLoadedBundle()
        {
            if (_listupShowingBundleDic.Count <= 0)
                return;

            EditorGUILayout.BeginVertical("Box");
            {
                int curWidthIndex = 0;

                Dictionary<string, LoadedAssetBundle>.Enumerator iter = _listupShowingBundleDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        DrawLoadedBundleAt(iter.Current.Value);

                        while (true)
                        {
                            ++curWidthIndex;
                            if (curWidthIndex >= MAX_WIDTH_COUNT)
                            {
                                curWidthIndex = 0;
                                break;
                            }

                            if (!iter.MoveNext())
                                break;

                            DrawLoadedBundleAt(iter.Current.Value);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawUsingAssetData()
        {
            if (_listupShowingUsingAssetDic.Count <= 0)
                return;

            EditorGUILayout.BeginVertical("Box");
            {
                int curWidthIndex = 0;

                Dictionary<Object, UsingAssetData>.Enumerator iter = _listupShowingUsingAssetDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        DrawUsingAssetAt(iter.Current.Value);

                        while (true)
                        {
                            ++curWidthIndex;
                            if (curWidthIndex >= MAX_WIDTH_COUNT)
                            {
                                curWidthIndex = 0;
                                break;
                            }

                            if (!iter.MoveNext())
                                break;

                            DrawUsingAssetAt(iter.Current.Value);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawUsingSceneData()
        {
            if (_listupShowingUsingSceneDic.Count <= 0)
                return;

            EditorGUILayout.BeginVertical("Box");
            {
                int curWidthIndex = 0;

                Dictionary<string, UsingAssetScene>.Enumerator iter = _listupShowingUsingSceneDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        DrawUsingAssetAt(iter.Current.Value);

                        while (true)
                        {
                            ++curWidthIndex;
                            if (curWidthIndex >= MAX_WIDTH_COUNT)
                            {
                                curWidthIndex = 0;
                                break;
                            }

                            if (!iter.MoveNext())
                                break;

                            DrawUsingAssetAt(iter.Current.Value);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}