using UnityEngine;
using System.Collections;

public class LoadedAssetBundle
{
    public AssetBundle assetBundle;
    public int referencedCount;
    public UsingAssetData usingData;
    public string Name;
    public LoadedAssetBundle(string n, AssetBundle _asset_bundle)
    {
        Name = n;
        assetBundle = _asset_bundle;
        referencedCount = 1;
    }

    private void Unload()
    {
        if (null != assetBundle)
        {
            //Debug.Log("!!!!!!!!!!!AssetBundle " + Name + " has been destroyed successfully !!!!!!!!!!"); //by blackerl
            assetBundle.Unload(true);
            assetBundle = null;
        }
    }

    ~LoadedAssetBundle()
    {
        Unload();
    }
}

public class UsingAssetData
{
    public int refCount { get; protected set; }
    public string bundleName { get; protected set; }
    public string assetName { get; private set; }
    public LoadedAssetBundle loadedAssetBundle { get; protected set; }

    public UsingAssetData(string _bundle_name, string _asset_name, LoadedAssetBundle _loaded_bundle)
    {
        bundleName = _bundle_name;
        assetName = _asset_name;
        loadedAssetBundle = _loaded_bundle;
        refCount = 1;
    }

    public void IncreaseRef() { ++refCount; }

    public void DecreaseRef() { --refCount; }
}

public class UsingAssetScene : UsingAssetData
{
    public string sceneName { get; private set; }
    public UsingAssetScene(string _bundle_name, string _scene_name, LoadedAssetBundle _loaded_bundle)
        : base(_bundle_name, _scene_name, _loaded_bundle)
    {
        sceneName = _scene_name;
    }

    //public void IncreaseRef() { ++refCount; }

    //public void DecreaseRef() { --refCount; }
}