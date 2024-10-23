using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class BundleMgr
{
#if UNITY_EDITOR
    public static Dictionary<Object, UsingAssetData> usingAssetDic { get { return _usingAssetDic; } }
    public static Dictionary<string, UsingAssetScene> usingSceneDic { get { return _usingSceneDic; } }
#else
    public static Dictionary<Object, UsingAssetData> usingAssetDic { get { return null; } }
    public static Dictionary<string, UsingAssetScene> usingSceneDic { get { return null; } }
#endif

    private static Dictionary<Object, UsingAssetData> _usingAssetDic = new Dictionary<Object, UsingAssetData>();
    private static Dictionary<string, UsingAssetScene> _usingSceneDic = new Dictionary<string, UsingAssetScene>();

    private static void IncreaseUsingData(string _bundle_name, string _asset_name, LoadedAssetBundle _loaded_bundle, Object _asset)
    {
        UsingAssetData _using_data;
        if (_usingAssetDic.TryGetValue(_asset, out _using_data))
        {
            _using_data.IncreaseRef();
        }
        else
        {
            _using_data = new UsingAssetData(_bundle_name, _asset_name, _loaded_bundle);
            _usingAssetDic.Add(_asset, _using_data);

            _loaded_bundle.usingData = _using_data;
        }
    }

    private static void DecreaseUsingData(Object _asset)
    {
        if (null == _asset)
            return;

        UsingAssetData _using_data;
        if (_usingAssetDic.TryGetValue(_asset, out _using_data))
        {
            if (DecreaseUsing(_using_data))
            {
                _usingAssetDic.Remove(_asset);
                _asset = null;
            }
        }
    }

    private static void IncreaseUsingScene(string _bundle_name, string _scene_name, LoadedAssetBundle _loaded_bundle)
    {
        UsingAssetScene _using_data;
        if (_usingSceneDic.TryGetValue(_scene_name, out _using_data))
        {
            _using_data.IncreaseRef();
        }
        else
        {
            _using_data = new UsingAssetScene(_bundle_name, _scene_name, _loaded_bundle);
            _usingSceneDic.Add(_scene_name, _using_data);

            _loaded_bundle.usingData = _using_data;
        }
    }

    private static void DecreaseUsingScene(string _scene_name)
    {
        if (string.IsNullOrEmpty(_scene_name))
            return;

        UsingAssetScene _using_data;
        if (_usingSceneDic.TryGetValue(_scene_name, out _using_data))
        {
            if (DecreaseUsing(_using_data))
            {
                _usingSceneDic.Remove(_scene_name);
            }
        }
    }

    private static bool DecreaseUsing(UsingAssetData _using_data)
    {
        if (null != _using_data)
        {
            _using_data.DecreaseRef();

            DecreaseBundleReference(_using_data.loadedAssetBundle);

            if (_using_data.loadedAssetBundle.referencedCount <= 1)
            {
                _using_data.loadedAssetBundle.usingData = null;
                //_using_data.loadedAssetBundle.assetBundle.Unload(false);

                UnloadAssetBundle(_using_data.bundleName);
            }

            return (_using_data.refCount <= 0);
        }

        return false;
    }
}