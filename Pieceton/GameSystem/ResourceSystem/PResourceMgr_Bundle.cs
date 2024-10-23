using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pieceton.Misc.Resource
{
    public partial class PResourceMgr
    {
        private static T LoadBundle<T>(EPBundleType _type, string _asset_name) where T : UnityEngine.Object
        {
            T result = default(T);

            PBundleInfo info = PBundleInfo.Get(_type);
            if (null == info)
            {
                PLog.AnyLogError("[Load] Not found EPBundleType. tried = '{0}'", _type);
                return result;
            }

            return LoadBundle<T>(info.bundleName, _asset_name, info.assetExtension, info.assetPath);
        }

        private static T LoadBundle<T>(string _bundle_name, string _asset_name, string _asset_extension, string _asset_path) where T : UnityEngine.Object
        {
            T result = default(T);

            if (string.IsNullOrEmpty(_bundle_name))
            {
                PLog.AnyLogError("[Load] Bundle name is empty.");
                return result;
            }

            if (string.IsNullOrEmpty(_asset_name))
            {
                PLog.AnyLogError("[Load] Asset name is empty.");
                return result;
            }

            if (string.IsNullOrEmpty(_asset_extension))
            {
                PLog.AnyLogError("[Load] Asset extension is empty.");
                return result;
            }

            if (!PAssetBundleSimulate.active)
            {
                return LoadAssetDatabase<T>(_asset_path, _asset_name, _asset_extension);
            }

            result = BundleMgr.LoadAsset<T>(_bundle_name, _asset_name, _asset_extension);
            if (null == result)
            {
                PLog.AnyLogError("Failed load asset. bundle = '{0}' asset = '{1}'", _bundle_name, _asset_name);
            }

            return result;
        }


        public static IEnumerator LoadAsync<T>(System.Action<T> _result_func, EPBundleType _bundle_type, string _asset_name) where T : UnityEngine.Object
        {
            T result = default(T);

            PBundleInfo info = PBundleInfo.Get(_bundle_type);
            if (null == info)
            {
                PLog.AnyLogError("Not found resource type. bundle type = '{0}'", _bundle_type);
                _result_func(result);
                yield break;
            }

            yield return LoadBundleAsync<T>(_result_func, info.bundleName, _asset_name, info.assetExtension, info.assetPath);
        }

        private static IEnumerator LoadBundleAsync<T>(System.Action<T> _result_func, string _bundle_name, string _asset_name, string _asset_extension, string _asset_path) where T : UnityEngine.Object
        {
            if (null == _result_func)
            {
                PLog.AnyLogError("[LoadBundleAsync] Callback function is null.");
                yield break;
            }

            T result = default(T);

            if (string.IsNullOrEmpty(_bundle_name))
            {
                PLog.AnyLogError("[LoadBundleAsync] bundle name is empty.");
                _result_func(result);
                yield break;
            }

            if (string.IsNullOrEmpty(_asset_name))
            {
                PLog.AnyLogError("[LoadBundleAsync] asset name is empty.");
                _result_func(result);
                yield break;
            }

            if (!PAssetBundleSimulate.active)
            {
                result = LoadAssetDatabase<T>(_asset_path, _asset_name, _asset_extension);
                _result_func(result);
                yield break;
            }

            yield return BundleMgr.LoadAssetAsync<T>(_result_func, _bundle_name, _asset_name);
        }



        public static void UnloadBundle(GameObject _go)
        {
            BundleMgr.UnloadAsset(_go);
        }
    }
}