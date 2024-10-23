using UnityEngine;
using System.Collections.Generic;

using Pieceton.Misc;
using Pieceton.PatchSystem;

public partial class BundleMgr : PSingletonComponent<BundleMgr>
{
    protected override void AwakeInstance() { }
    protected override bool InitInstance() { return true; }
    protected override void ReleaseInstance() { }

    public const bool useEncryptBundle = true;

    private AssetBundleStorage bundleStorage = new AssetBundleStorage();


    private static string _activatedVariant = "";
    public static string activatedVariant { get { return _activatedVariant; } }
    public static void SetVariant(string _cur) { _activatedVariant = _cur; }


    private static readonly List<AssetBundleLoadOperation> _inProgressOperations = new List<AssetBundleLoadOperation>();
    public static bool isBundleOperating { get { return _isBundleOperating; } }
    private static bool _isBundleOperating = false;


    private static LoadedAssetBundle GetOrLoadAssetBundle(string _bundle_name)
    {
        LoadedAssetBundle _loaded_bundle = GetLoadedAssetBundle(_bundle_name);
        if (null == _loaded_bundle)
        {
            _loaded_bundle = LoadAssetBundle(_bundle_name);
        }
        else
        {
            IncreaseBundleReference(_loaded_bundle);
        }

        return _loaded_bundle;
    }

    private static LoadedAssetBundle LoadAssetBundle(string _bundle_name, bool _no_exit_then_download = false)
    {
        string _out_error = "";

        if (IsQuitting())
            return null;

        LoadedAssetBundle loadedBundle = Instance.bundleStorage.LoadAssetBundle(_bundle_name, out _out_error, _no_exit_then_download);

        if (!string.IsNullOrEmpty(_out_error))
        {
            Debug.LogErrorFormat("[BundleMgr] LoadAssetBundle. bundleName='{0}' error ='{1}'", _bundle_name, _out_error);
        }

        return loadedBundle;
    }

    private static LoadedAssetBundle GetLoadedAssetBundle(string _bundle_name)
    {
        string _out_error = "";

        if (IsQuitting())
            return null;

        LoadedAssetBundle loadedBundle = Instance.bundleStorage.GetLoadedAssetBundle(_bundle_name, out _out_error);
        if (!string.IsNullOrEmpty(_out_error))
        {
            if (Debug.isDebugBuild)
                Debug.LogWarning(_out_error);
        }

        return loadedBundle;
    }

    private static void UnloadAssetBundle(string _bundle_name)
    {
        Instance.bundleStorage.UnloadAssetBundle(_bundle_name);
    }

    public static void UnloadUnusedAssetBundles()
    {
        if (!PAssetBundleSimulate.active)
            return;

        if (!PatchHandler.IsValid())
            return;

        int count = PatchHandler.allBundleList.Length;
        for (int i = 0; i < count; ++i)
        {
            UnloadAssetBundle(PatchHandler.allBundleList[i]);
        }
    }

    private static void IncreaseBundleReference(LoadedAssetBundle _use_bundle)
    {
        if (IsQuitting())
            return;

        Instance.bundleStorage.IncreaseBundleReferenceWithDependency(_use_bundle);
    }

    private static void DecreaseBundleReference(LoadedAssetBundle _use_bundle)
    {
        if (IsQuitting())
            return;

        Instance.bundleStorage.DecreaseBundleReferenceWithDependency(_use_bundle);
    }


    public static void AddDontUnloadAssetBundle(string _bundle_name)
    {
        if (IsQuitting())
            return;

        Instance.bundleStorage.AddDontUnloadAssetBundle(_bundle_name);
    }

    public static void RemoveDontUnloadAssetBundle(string _bundle_name)
    {
        if (IsQuitting())
            return;

        Instance.bundleStorage.RemoveDontUnloadAssetBundle(_bundle_name);
    }

    public static void SetAssetBundleManifestObject(AssetBundleManifest _obj) { }

    private void Update()
    {
        if (isBundleOperating)
        {
            // Update all in progress operations
            for (int n = 0; n < _inProgressOperations.Count;)
            {
                if (!_inProgressOperations[n].Update())
                {
                    _inProgressOperations.RemoveAt(n);
                }
                else
                    ++n;
            }

            if (_inProgressOperations.Count <= 0)
            {
                _isBundleOperating = false;
            }
        }
    }

    // 일부 파일이 확장자를 같이 해주지않으면 로딩되지 않는 유니티 버그가 있음
    private static string MakeFileNameWithExtention(string _file_name, params string[] _arg)
    {
        if (null != _arg && _arg.Length > 0)
        {
            _file_name += "." + _arg[0];
        }

        return _file_name;
    }

    private static bool HasAssetInBundle(AssetBundle _bundle, string _asset_name)
    {
        if (null != _bundle && !string.IsNullOrEmpty(_asset_name))
        {
            string[] names = _bundle.GetAllAssetNames();
            if (null != names)
            {
                int count = names.Length;
                for (int i = 0; i < count; ++i)
                {
                    if (names[i].Equals(_asset_name))
                        return true;
                }
            }
        }

        return false;
    }
}