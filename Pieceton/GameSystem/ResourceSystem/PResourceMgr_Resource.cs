using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pieceton.Misc;

namespace Pieceton.Misc.Resource
{
    public partial class PResourceMgr
    {
        private static T LoadAsset<T>(EPResourcesType _type, string _asset_name) where T : UnityEngine.Object
        {
            T result = default(T);

            PResourcesInfo info = PResourcesInfo.Get(_type);
            if (null == info)
            {
                PLog.AnyLogError("[Load] Not found PResourcesInfo. EPResourcesType = '{0}'", _type);
                return result;
            }

            return LoadAsset<T>(info.MakePath(_asset_name));
        }

        public static T LoadAsset<T>(string _path) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(_path))
            {
                PLog.AnyLogError("[LoadResources] Asset name is empty. tried = '{0}'", _path);
                return default(T);
            }
            
            return Resources.Load<T>(_path);
        }


        public static IEnumerator LoadAssetAsync<T>(System.Action<T> _result_func, EPResourcesType _type, string _asset_name) where T : UnityEngine.Object
        {
            T result = default(T);

            PResourcesInfo info = PResourcesInfo.Get(_type);
            if (null == info)
            {
                PLog.AnyLogError("[LoadAsync] Not found PResourcesInfo. EPResourcesType = '{0}'", _type);
                _result_func(result);
                yield break;
            }

            yield return LoadResourcesAsync<T>(_result_func, info.MakePath(_asset_name));
        }

        private static IEnumerator LoadResourcesAsync<T>(System.Action<T> _result_func, string _resources_path) where T : UnityEngine.Object
        {
            if (null == _result_func)
            {
                PLog.AnyLogError("[LoadResourcesAsync] Callback function is null.");
                yield break;
            }

            T result = default(T);

            if (string.IsNullOrEmpty(_resources_path))
            {
                _result_func(result);
                yield break;
            }

            ResourceRequest request = Resources.LoadAsync<T>(_resources_path);
            if (null == request)
            {
                PLog.AnyLogError("[LoadResourcesAsync] ResourceRequest is null. tried asset = '{0}'", _resources_path);
                _result_func(result);
                yield break;
            }

            while (!request.isDone)
                yield return null;

            _result_func((T)request.asset);
        }

        public static void UnloadAsset(Object _prefab)
        {
            if (null != _prefab)
            {
                Resources.UnloadAsset(_prefab);
                _prefab = null;
            }
        }
    }
}