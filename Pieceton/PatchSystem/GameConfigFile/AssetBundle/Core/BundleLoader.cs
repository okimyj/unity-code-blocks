using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Pieceton.Misc;

public partial class BundleMgr
{
    public static T LoadAsset<T>(string _bundle_name, string _file_name, params string[] _arg) where T : UnityEngine.Object
    {
        T result = null;

        if (!PAssetBundleSimulate.active)
            return null;

        LoadedAssetBundle loadedBundle = GetOrLoadAssetBundle(_bundle_name);

        if (null != loadedBundle)
        {
            if (null != loadedBundle.assetBundle)
            {
                _file_name = MakeFileNameWithExtention(_file_name, _arg);

                result = loadedBundle.assetBundle.LoadAsset<T>(_file_name);
                if (null != result)
                {
                    IncreaseUsingData(_bundle_name, _file_name, loadedBundle, result);
                }
                else
                {
                    DecreaseBundleReference(loadedBundle);
                }
            }
            else
            {
                string errMsg = string.Format("Invalid AssetBundle! = '{0}' asset = '{1}'", _bundle_name, _file_name);
                Debug.LogError(errMsg);
                PExceptionSender.Instance.SendLog(errMsg);
            }
        }
        else
        {
            string errMsg = string.Format("not loaded LoadedAssetBundle! = '{0}' asset = '{1}'", _bundle_name, _file_name);
            Debug.LogError(errMsg);
            PExceptionSender.Instance.SendLog(errMsg);
        }

        if (null == result)
        {
            string errMsg = string.Format("not found resource! bundle = '{0}' asset = '{1}'", _bundle_name, _file_name);

            Debug.LogError(errMsg);

            PExceptionSender.Instance.SendLog(errMsg);
        }

        return result;
    }
    public static T[] LoadAllAsset<T>(string _bundle_name) where T : UnityEngine.Object
    {
        T[] result = null;

        if (!PAssetBundleSimulate.active)
            return null;
        LoadedAssetBundle loadedBundle = GetOrLoadAssetBundle(_bundle_name);
        if (null != loadedBundle)
        {
            result = loadedBundle.assetBundle.LoadAllAssets<T>();
            if (result == null)
            {
                string errMsg = string.Format("not found resources! bundle = '{0}'", _bundle_name);

                Debug.LogError(errMsg);

                PExceptionSender.Instance.SendLog(errMsg);
            }
        }
        return result;
    }

    public static IEnumerator LoadAssetAsync<T>(System.Action<T> _result_func, string _bundle_name, string _file_name, params string[] _arg) where T : UnityEngine.Object
    {
        if (null == _result_func)
            yield break;

        T result = null;

        if (!PAssetBundleSimulate.active)
        {
            _result_func(null);
            yield break;
        }

        LoadedAssetBundle loadedBundle = GetOrLoadAssetBundle(_bundle_name);

        if (null != loadedBundle)
        {
            _file_name = MakeFileNameWithExtention(_file_name, _arg);

            AssetBundleRequest _request = loadedBundle.assetBundle.LoadAssetAsync<T>(_file_name);
            if (null != _request)
            {
                while (!_request.isDone)
                    yield return null;

                result = _request.asset as T;
                if (null != result)
                {
                    IncreaseUsingData(_bundle_name, _file_name, loadedBundle, result);
                }
                else
                {
                    DecreaseBundleReference(loadedBundle);

                    string errMsg = string.Format("not found resource! bundle = '{0}' file = '{1}'", _bundle_name, _file_name);

                    if (Debug.isDebugBuild)
                        Debug.LogWarning(errMsg);

                    PExceptionSender.Instance.SendLog(errMsg);
                }
            }
        }

        _result_func(result);
    }

    public static void UnloadAsset(Object _asset)
    {
        if (!PAssetBundleSimulate.active)
            return;

        DecreaseUsingData(_asset);
    }



    public static void LoadScene(string _bundle_name, string _scene_name, bool _is_additive, params string[] _arg)
    {
        if (!PAssetBundleSimulate.active)
            return;

        LoadedAssetBundle loadedBundle = GetOrLoadAssetBundle(_bundle_name);

        if (null == loadedBundle)
        {
            string errMsg = string.Format("couldn't bundle{0} scene({1}) " + _bundle_name, _scene_name);

            if (Debug.isDebugBuild)
                Debug.LogError(errMsg);

            PExceptionSender.Instance.SendLog(errMsg);
            return;
        }

        _scene_name = MakeFileNameWithExtention(_scene_name, _arg);

        //모든 에셋번들의 로드가 완료되었을 경우 사용 가능
        if (_is_additive)
        {
            IncreaseUsingScene(_bundle_name, _scene_name, loadedBundle);
            SceneManager.LoadScene(_scene_name, LoadSceneMode.Additive);

            return;
        }

        SceneManager.LoadScene(_scene_name);
    }

    public static AssetBundleLoadLevelOperation LoadSceneAsync(string _bundle_name, string _scene_name, bool _is_additive, params string[] _arg)
    {
        if (!PAssetBundleSimulate.active)
            return null;

        _scene_name = MakeFileNameWithExtention(_scene_name, _arg);

        AssetBundleLoadLevelOperation assetOp = new AssetBundleLoadLevelOperation(LoadSceneOperation, _bundle_name, _scene_name, _is_additive);

        _inProgressOperations.Add(assetOp);
        _isBundleOperating = true;

        return assetOp;
    }

    public static void UnloadScene(string _scene_name)
    {
        if (!PAssetBundleSimulate.active)
            return;

        DecreaseUsingScene(_scene_name);

        Scene scene = SceneManager.GetSceneByName(_scene_name);
        if (scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(_scene_name);
        }
    }


    #region load operation functinos
    private static AssetBundleRequest LoadAssetOperation(string _bundle_name, string _asset_name, System.Type _type)
    {
        LoadedAssetBundle bundle = GetOrLoadAssetBundle(_bundle_name);
        if (null != bundle)
        {
            return bundle.assetBundle.LoadAssetAsync(_asset_name, _type);
        }

        return null;
    }

    private static AsyncOperation LoadSceneOperation(string _bundle_name, string _scene_name, bool _is_additive)
    {
        if (!PAssetBundleSimulate.active)
            return null;

        LoadedAssetBundle loadedBundle = GetOrLoadAssetBundle(_bundle_name);

        if (null == loadedBundle)
        {
            string errMsg = string.Format("couldn't bundle{0} scene({1}) ", _bundle_name, _scene_name);

            if (Debug.isDebugBuild)
                Debug.LogError(errMsg);

            PExceptionSender.Instance.SendLog(errMsg);
            return null;
        }

        if (_is_additive)
        {
            IncreaseUsingScene(_bundle_name, _scene_name, loadedBundle);
            return SceneManager.LoadSceneAsync(_scene_name, LoadSceneMode.Additive);
        }

        return SceneManager.LoadSceneAsync(_scene_name);
    }
    #endregion
}