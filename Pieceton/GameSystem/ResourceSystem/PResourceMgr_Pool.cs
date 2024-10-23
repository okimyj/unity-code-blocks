using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pieceton.Misc.Resource
{
    public partial class PResourceMgr
    {
        private class Pool
        {
            public string key { get; private set; }
            public string resourceType { get; private set; }
            public string assetName { get; private set; }
            public GameObject asset { get; private set; }
            public int referenceCount { get; private set; }
            public int instancingCount { get; private set; }

            private List<GameObject> instances = null;

            Transform container = null;

            private bool isBundle = false;
            private bool released = false;

            public Pool(Transform _parent_tr, bool _is_bundle, string _resource_type, string _asset_name, GameObject _asset)
            {
                key = PPoolComponent.MakeKey(_resource_type, _asset_name);

                container = (new GameObject()).transform;
                container.gameObject.name = string.Format("container.{0}", key);
                container.SetParent(_parent_tr);

                resourceType = _resource_type;
                assetName = _asset_name;
                asset = _asset;
                referenceCount = 0;
                instancingCount = 0;
                instances = new List<GameObject>();

                isBundle = _is_bundle;
                released = false;
            }

            ~Pool()
            {
                ReleaseAll();

                container.SetParent(null);
                container = null;
            }

            public T Get<T>() where T : UnityEngine.Object
            {
                if (released)
                {
                    Debug.LogError("PResourceMgr.Pool::Get() Already called ReleaseAll.");
                    return null;
                }

                if (null == asset)
                    return null;

                GameObject result = null;

                if (instances.Count > 0)
                {
                    result = instances[0];
                    instances.RemoveAt(0);

                    ++referenceCount;
                }
                else
                {
                    result = Instantiate(asset);

                    PPoolComponent poolComponent = result.AddComponent<PPoolComponent>();
                    poolComponent.SetKey(key);

                    ++referenceCount;
                    ++instancingCount;
                }

                result.SetActive(true);

                if (typeof(T).Equals(typeof(GameObject)))
                    return result as T;

                return result.GetComponent<T>();
            }

            public void Release(GameObject _instance)
            {
                if (released)
                {
                    Debug.LogError("PResourceMgr.Pool::Release() Already called ReleaseAll.");
                    return;
                }

                if (null == _instance)
                    return;

                _instance.transform.SetParent(container, false);
                _instance.SetActive(false);

                instances.Add(_instance);

                --referenceCount;
            }

            public void ReleaseAll()
            {
                if (released)
                    return;

                if (referenceCount != 0 || instancingCount != instances.Count)
                {
                    Debug.LogError("PResourceMgr.Pool::ReleaseAll() There is an Instance that was not returned.");
                }

                int count = instances.Count;
                for (int i=count-1; i>=0; --i)
                {
                    GameObject.Destroy(instances[i]);
                    instances[i] = null;
                }

                instances.Clear();
                instances = null;

                if (isBundle)   BundleMgr.UnloadAsset(asset);
                else            Resources.UnloadAsset(asset);
                asset = null;

                released = true;
            }
        }

        public class PPoolComponent : MonoBehaviour
        {
            public string key { get; private set; }

            public void SetKey(string _key) { key = _key; }

            public static string MakeKey(string _resource_type, string _asset_path)
            {
                return _resource_type + "-" + _asset_path;
            }
        }
    }
}