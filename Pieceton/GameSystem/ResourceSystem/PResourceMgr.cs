using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pieceton.Misc.Resource
{
    public partial class PResourceMgr : PSingletonComponent<PResourceMgr>
    {
        private Transform poolTr = null;
        private Dictionary<string, Pool> goPool = null;

        protected override void AwakeInstance()
        {
            PResourcesInfo.Initialize();
            PBundleInfo.Initialize();

            poolTr = (new GameObject()).transform;
            poolTr.gameObject.name = "poolContainer";
            poolTr.SetParent(transform);

            goPool = new Dictionary<string, Pool>();
        }
        protected override bool InitInstance() { return true; }

        protected override void ReleaseInstance()
        {
            var iter = goPool.GetEnumerator();
            while (iter.MoveNext())
            {
                iter.Current.Value.ReleaseAll();
            }
            goPool.Clear();

            GameObject.Destroy(poolTr.gameObject);
            poolTr = null;
        }


        public T GetResource<T>(EPResourcesType _type, string _asset_name) where T : UnityEngine.Object
        {
            Pool pool = GetPoolWithCreate(PResourcesInfo.Get(_type), _asset_name);
            if (null == pool)
                return null;

            return pool.Get<T>();
        }

        public T GetBundle<T>(EPBundleType _type, string _asset_name) where T : UnityEngine.Object
        {
            Pool pool = GetPoolWithCreate(PBundleInfo.Get(_type), _asset_name);
            if (null == pool)
                return null;

            return pool.Get<T>();
        }

        public void PutInstance(GameObject _instance)
        {
            if (null == _instance)
                return;

            PPoolComponent poolComponent = _instance.GetComponent<PPoolComponent>();
            if (null == poolComponent)
            {
                Debug.LogErrorFormat("PResourceMgr::ReleaseInatnce() Not instance generated from PResource. Instance name = '{0}'", _instance.name);
                return;
            }

            Pool pool = GetPool(poolComponent.key);
            if (null == pool)
                return;

            pool.Release(_instance);
        }

        private Pool GetPoolWithCreate(PResourcesInfo _info, string _asset_name)
        {
            string key = PPoolComponent.MakeKey(_info.resoucesType.ToString(), _asset_name);

            Pool pool = null;
            if (goPool.TryGetValue(key, out pool))
                return pool;

            GameObject go = LoadAsset<GameObject>(_info.MakePath(_asset_name));
            if (null == go)
                return null;

            pool = new Pool(poolTr, false, _info.resoucesType.ToString(), _asset_name, go);
            goPool.Add(key, pool);

            return pool;
        }

        private Pool GetPoolWithCreate(PBundleInfo _info, string _asset_name)
        {
            string key = PPoolComponent.MakeKey(_info.bundleType.ToString(), _asset_name);

            Pool pool = null;
            if (goPool.TryGetValue(key, out pool))
                return pool;

            GameObject go = LoadBundle<GameObject>(_info.bundleType, _asset_name);
            if (null == go)
                return null;

            pool = new Pool(poolTr, false, _info.bundleType.ToString(), _asset_name, go);

            goPool.Add(key, pool);

            return pool;
        }

        private Pool GetPool(string _key)
        {
            Pool pool = null;
            if (goPool.TryGetValue(_key, out pool))
                return pool;

            Debug.LogErrorFormat("PResourceMgr::GetPool() Not found pool. key = '{0}'", _key);

            return null;
        }

        public static void UnloadUnusedAssetBundle()
        {
            Resources.UnloadUnusedAssets();
            BundleMgr.UnloadUnusedAssetBundles();
        }

        private static T LoadAssetDatabase<T>(string _asset_path, string _asset_name, string _asset_extension) where T : UnityEngine.Object
        {
            T result = default(T);

#if UNITY_EDITOR
            if (!PAssetBundleSimulate.active)
            {
                if (string.IsNullOrEmpty(_asset_path))
                {
                    PLog.AnyLogError("[LoadAssetDatabase] Asset path is empty");
                    return result;
                }

                if (string.IsNullOrEmpty(_asset_name))
                {
                    PLog.AnyLogError("[LoadAssetDatabase] Asset name is empty");
                    return result;
                }

                string path = "Assets/" + _asset_path + "/" + _asset_name + "." + _asset_extension;
                result = AssetDatabase.LoadAssetAtPath<T>(path);
                if (null == result)
                {
                    PLog.AnyLogError("[LoadAssetDatabase] Failed load asset. path = '{0}'", path);
                }
            }
#endif

            return result;
        }
    }
}