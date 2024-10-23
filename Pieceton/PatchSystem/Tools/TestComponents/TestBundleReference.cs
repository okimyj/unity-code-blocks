#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pieceton.PatchSystem
{
    public abstract partial class TestBundleReference
    {
        protected string testName { get { return this.GetType().ToString(); } }

        public static string executeTestName { get; private set; }

        public static string lastSubFuncName { get; private set; }
        public static List<string> doingList { get { return _doingList; } }
        private static List<string> _doingList = new List<string>();

        protected const float TASK_DELAY_SECONDS = 3.0f;
        public IEnumerator Proccess()
        {
            yield return BeginProccess();
            yield return OnProccess();
            yield return EndProccess();
        }
        protected abstract IEnumerator OnProccess();

        private IEnumerator BeginProccess()
        {
            executeTestName = testName;
            lastSubFuncName = "";

            Debug.LogFormat("[TestBundleReference] {0} begin", testName);
            CheckZeroReference();
            yield return new WaitForSeconds(TASK_DELAY_SECONDS);
        }

        private IEnumerator EndProccess()
        {
            Debug.LogFormat("[TestBundleReference] {0} end", testName);
            CheckZeroReference();
            yield return new WaitForSeconds(TASK_DELAY_SECONDS);

            executeTestName = "";
            lastSubFuncName = "";
            _doingList.Clear();
        }

        protected IEnumerator Delay(float _delay_seconds = TASK_DELAY_SECONDS)
        {
            yield return new WaitForSeconds(_delay_seconds);
        }

        private void AddDoingList(System.Reflection.MemberInfo _method)
        {
            lastSubFuncName = _method.Name;
            _doingList.Add(lastSubFuncName);
        }

        private void CheckZeroReference()
        {
            string[] allBundleNames = PatchHandler.allBundleList;
            int count = allBundleNames.Length;

            for (int i = 0; i < count; ++i)
            {
                CheckReference(allBundleNames[i], 0);
            }
        }

        protected void CheckReference(string _bundle_name, int _target_ref_count)
        {
            int curCount = 0;
            LoadedAssetBundle loadedBundle;
            if (AssetBundleStorage.loadedAssetBundles.TryGetValue(_bundle_name, out loadedBundle))
            {
                curCount = loadedBundle.referencedCount;
            }

            if (curCount != _target_ref_count)
            {
                string msg = string.Format("{0}::{1}({2}, {3}) real='{4}'",
                                            testName, lastSubFuncName, _bundle_name, _target_ref_count, curCount);
                throw new Exception(msg);
            }
        }


        private static List<TestBundleReference> _tasks = new List<TestBundleReference>();
        private static void AddTask<T>() where T : TestBundleReference, new() { _tasks.Add(new T()); }

        public static IEnumerator RunTasks()
        {
            ListupTestComponents();

            Debug.Log("[TestBundleReference] ------------- Start -------------");

            int count = _tasks.Count;
            for (int i = 0; i < count; ++i)
            {
                yield return _tasks[i].Proccess();
            }

            Debug.Log("[TestBundleReference] ------------- Finish -------------");

            _tasks.Clear();
        }

        public static void ListupTestComponents()
        {
            AddTask<TBundleRef_LoadAsset>();
            AddTask<TBundleRef_AsyncLoadAsset>();
            AddTask<TBundleRef_TryLoadWrongBundle>();
            AddTask<TBundleRef_LoadPrefab>();
            AddTask<TBundleRef_LoadPrefabDefendency>();
            AddTask<TBundleRef_DontUnloadBundle>();
        }
    }
}
#endif //UNITY_EDITOR