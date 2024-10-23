using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public class AssetBundleStorage
    {
#if UNITY_EDITOR
        public static Dictionary<string, LoadedAssetBundle> loadedAssetBundles { get { return _loadedAssetBundles; } }
        public static List<string> dontUnloadAssets { get { return _dontUnloadAssets; } }
#else
        public static Dictionary<string, LoadedAssetBundle> loadedAssetBundles { get { return null; } }
        public static List<string> dontUnloadAssets { get { return null; } }
#endif

        private static Dictionary<string, LoadedAssetBundle> _loadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
        private static Dictionary<string, string[]> _dependencies = new Dictionary<string, string[]>();
        private static string[] _variants = { };

        private static List<string> _dontUnloadAssets = new List<string>();

        public LoadedAssetBundle LoadAssetBundle(string _bundle_name, out string _error, bool _no_exit_then_download)
        {
            _error = "";

            if (!PAssetBundleSimulate.active)
            {
                _error = "simulate asset bundle mode";
                return null;
            }

            LoadBundle_Internal(_bundle_name, _no_exit_then_download);
            LoadBundle_Dependencies(_bundle_name, _no_exit_then_download);

            LoadedAssetBundle loadedAssetBundle;
            if (false == TryGetLoadedBundle(_bundle_name, out loadedAssetBundle))
            {
                _error = string.Format("not found cached asset bundle. bundleName='{0}'", _bundle_name);
                return null;
            }

            return GetLoadedAssetBundle(_bundle_name, out _error);
        }

        public void UnloadAssetBundle(string _bundle_name)
        {
            if (!PAssetBundleSimulate.active)
                return;

            string reName = PatchHandler.GetBundleName_ExceptVariant(_bundle_name);

            if (_dontUnloadAssets.Contains(reName))
            {
                //DecreaseReference(loadedAssetBundle);
                return;
            }

            LoadedAssetBundle loadedAssetBundle;
            if (!TryGetLoadedBundle(_bundle_name, out loadedAssetBundle))
                return;

            if (null != loadedAssetBundle.usingData)
            {
                if (loadedAssetBundle.usingData.refCount > 0)
                    return;
            }

            if (loadedAssetBundle.referencedCount > 0)
                return;

            UnloadAssetBundle_Internal(_bundle_name);
            UnloadAssetBundle_Dependencies(_bundle_name);
        }

        public LoadedAssetBundle GetLoadedAssetBundle(string _bundle_name, out string error)
        {
            LoadedAssetBundle bundle = null;
            error = "";

            string reName = PatchHandler.GetBundleName_ExceptVariant(_bundle_name);

            TryGetLoadedBundle(reName, out bundle);
            if (null == bundle)
            {
                error = string.Format("not found loaded asset bundle. {0}", _bundle_name);
                return null;
            }

            string[] dependencies = null;
            if (!TryGetDependencyBundle(reName, out dependencies))
                return bundle;

            foreach (var dependency in dependencies)
            {
                LoadedAssetBundle dependentBundle;
                TryGetLoadedBundle(dependency, out dependentBundle);
                if (dependentBundle == null)
                {
                    error = string.Format("not found loaded dependent bundle. {0}", dependency);
                    return null;
                }
            }

            return bundle;
        }


        #region load functions
        private void LoadBundle_Internal(string _bundle_name, bool _no_exit_then_download)
        {
            LoadedAssetBundle loadedBundle;
            if (TryGetLoadedBundle(_bundle_name, out loadedBundle))
            {
                loadedBundle.referencedCount++;
                return;
            }

            if (WWWDownloader.Instance.IsDownloading(_bundle_name))
                return;

            loadedBundle = LoadBundleInStorage(_bundle_name, _no_exit_then_download);

            AddLoadedBundle(_bundle_name, loadedBundle);
        }

        private bool TryGetLoadedBundle(string _bundle_name, out LoadedAssetBundle _loaded_bundle)
        {
            string reName = PatchHandler.GetBundleName_ExceptVariant(_bundle_name);
            return _loadedAssetBundles.TryGetValue(reName, out _loaded_bundle);
        }

        private void AddLoadedBundle(string _bundle_name, LoadedAssetBundle _loaded_bundle)
        {
            if (null == _loaded_bundle)
                return;

            string reName = PatchHandler.GetBundleName_ExceptVariant(_bundle_name);
            if (!_loadedAssetBundles.ContainsKey(reName))
            {
                _loadedAssetBundles.Add(reName, _loaded_bundle);
            }
        }

        private bool TryGetDependencyBundle(string _bundle_name, out string[] _dependencyList)
        {
            string reName = PatchHandler.GetBundleName_ExceptVariant(_bundle_name);
            return _dependencies.TryGetValue(reName, out _dependencyList);
        }

        private void AddDependencyBundle(string _bundle_name, string[] _dependencyList)
        {
            string reName = PatchHandler.GetBundleName_ExceptVariant(_bundle_name);
            if (!_dependencies.ContainsKey(reName))
            {
                _dependencies.Add(reName, _dependencyList);
            }
        }

        private void RemoveDependencyBundle(string _bundle_name)
        {
            string reName = PatchHandler.GetBundleName_ExceptVariant(_bundle_name);
            if (_dependencies.ContainsKey(reName))
            {
                _dependencies.Remove(reName);
            }
        }

        private LoadedAssetBundle LoadBundleInStorage(string _bundle_name, bool _no_exit_then_download)
        {
            LoadedAssetBundle loadedAssetBundle = null;
            if (TryGetLoadedBundle(_bundle_name, out loadedAssetBundle))
            {
                return loadedAssetBundle;
            }

            string variantBundleName = PatchHandler.GetBundleName_InclusionVariant(_bundle_name);

            string basePath = BundleStorageHandler.bundleStoragePath;

            string hash = PatchHandler.GetBundleHashString(variantBundleName);
            uint crc    = PatchHandler.GetBundleCRC(variantBundleName);
            if (StreamingBundleHandler.Has(variantBundleName, hash, crc))
            {
                basePath = StreamingBundleHandler.streamingBundlePath;
            }

            string _full_path = PatchHandler.MakeHashBundlePath(basePath, variantBundleName);

            string msgLoadBundle = string.Format("load bundle from storage. path='{0}'", _full_path);

            try
            {
                if (File.Exists(_full_path))
                {
                    AssetBundle bundle = null;

                    if (IsEncodedBundle(variantBundleName))
                    {
                        FileStream _file_stream = File.OpenRead(_full_path);
                        byte[] readBytes = FileStreamToBytes(_file_stream);
                        _file_stream.Dispose();

                        byte[] decoding = PEncryptor.Instance.FixedDecrypt(readBytes);
                        bundle = AssetBundle.LoadFromMemory(decoding);
                    }
                    else
                    {
                        bundle = AssetBundle.LoadFromFile(_full_path);
                    }

                    if (null != bundle)
                    {
                        loadedAssetBundle = new LoadedAssetBundle(_bundle_name, bundle);
                        return loadedAssetBundle;
                    }

                    #region LoadFromMemory
                    /*
                    byte[] readBytes = FileStreamToBytes(_file_stream);
                    if (null != readBytes)
                    {
                        AssetBundle bundle = AssetBundle.LoadFromMemory(readBytes);
                        if (null != bundle)
                        {
                            Debug.LogFormat("load cached bundle url ='{0}'", _url);

                            loadedAssetBundle = new LoadedAssetBundle(bundle);
                            _loadedBundlesInTempCache.Add(_asset_bundle_name, loadedAssetBundle);
                        }
                        else
                        {
                            error = string.Format("failed load from memory cached bundle. path='{0}'", _url);
                        }
                    }
                    else
                    {
                        error = string.Format("failed decrypt cached bundle. path='{0}'", _url);
                    }
                    _file_stream.Dispose();
                    /**/
                    #endregion
                }
                else
                {
                    if (_no_exit_then_download)
                    {
                        WWWDownloader.Instance.DownloadAssetBundle(variantBundleName);
                    }
                }
            }
            catch (Exception e)
            {
                string msgLoadBundleFail = string.Format("fail {0},{1}", msgLoadBundle, e.ToString());
                Debug.LogError(msgLoadBundleFail);
                PExceptionSender.Instance.SendLog(msgLoadBundleFail);
            }

            return null;
        }

        private void LoadBundle_Dependencies(string _bundle_name, bool _no_exit_then_download)
        {
            if (!PatchHandler.IsValid())
            {
                Debug.LogError("Please initialize PatchHandler");
                return;
            }

            string[] dependencies = PatchHandler.GetAllDependency(_bundle_name);
            if (null == dependencies)
                return;

            int count = dependencies.Length;
            if (count <= 0)
                return;

            for (int i = 0; i < dependencies.Length; i++)
                dependencies[i] = RemapVariantName(dependencies[i]);

            for (int i = 0; i < count; ++i)
            {
                AddDependencyBundle(_bundle_name, dependencies);

                // Debug.Log("!!!!!!!!!!!!! try load dependency :" +i+ "::" + dependencies[i] +" from origin:" + bundleName);//by blackerl

                LoadBundle_Internal(dependencies[i], _no_exit_then_download);
            }
        }
        #endregion load functions

        #region unload functions
        protected void UnloadAssetBundle_Internal(string _bundle_name)
        {
            string error;
            LoadedAssetBundle bundle = GetLoadedAssetBundle(_bundle_name, out error);
            if (bundle == null)
                return;

            if (--bundle.referencedCount <= 0)
            {
                string reName = PatchHandler.GetBundleName_ExceptVariant(_bundle_name);

                if (null != bundle.assetBundle)
                    bundle.assetBundle.Unload(true);

                bundle.assetBundle = null;
                //bundle.Unload();      // unload 하면 다시 load 함수를 쓸수 없다. . by blackerl

                _loadedAssetBundles.Remove(reName);
            }
        }

        protected void UnloadAssetBundle_Dependencies(string _bundle_name)
        {
            string[] dependencies = null;
            if (!TryGetDependencyBundle(_bundle_name, out dependencies))
                return;

            foreach (var dependency in dependencies)
            {
                //Debug.Log("$$$$$$$$$$$$$$ unload assetbundle dependency  bundle ::  " + dependency +" from " + _asset_bundle_name); //by blackerl
                UnloadAssetBundle_Internal(dependency);
            }

            RemoveDependencyBundle(_bundle_name);
        }
        #endregion unload functions

        #region dont unload bundle
        public void AddDontUnloadAssetBundle(string _bundle_name)
        {
            string reName = PatchHandler.GetBundleName_ExceptVariant(_bundle_name);

            if (_dontUnloadAssets.Contains(reName))
                return;

            _dontUnloadAssets.Add(reName);
        }

        public void RemoveDontUnloadAssetBundle(string _bundle_name)
        {
            string reName = PatchHandler.GetBundleName_ExceptVariant(_bundle_name);

            if (!_dontUnloadAssets.Contains(reName))
                return;

            _dontUnloadAssets.Remove(reName);
        }
        #endregion dont unload bundle

        public void IncreaseBundleReferenceWithDependency(LoadedAssetBundle _use_bundle)
        {
            if (null == _use_bundle)
                return;

            _use_bundle.referencedCount++;

            string[] dependencies = null;
            if (!TryGetDependencyBundle(_use_bundle.Name, out dependencies))
                return;

            string dependency = "";
            string error;

            int count = dependencies.Length;
            for (int i = 0; i < count; ++i)
            {
                dependency = dependencies[i];

                LoadedAssetBundle bundle = GetLoadedAssetBundle(dependency, out error);
                if (bundle == null)
                    continue;

                bundle.referencedCount++;
            }
        }

        public void DecreaseBundleReferenceWithDependency(LoadedAssetBundle _use_bundle)
        {
            if (null == _use_bundle)
                return;

            _use_bundle.referencedCount--;

            string[] dependencies = null;
            if (!TryGetDependencyBundle(_use_bundle.Name, out dependencies))
                return;

            string dependency = "";
            string error;

            int count = dependencies.Length;
            for (int i = 0; i < count; ++i)
            {
                dependency = dependencies[i];

                LoadedAssetBundle bundle = GetLoadedAssetBundle(dependency, out error);
                if (bundle == null)
                    continue;

                bundle.referencedCount--;
            }
        }

        protected string RemapVariantName(string _bundle_name)
        {
            if (PatchHandler.IsValid())
            {
                string[] bundlesWithVariant = PatchHandler.variants.ToArray();

                if (System.Array.IndexOf(bundlesWithVariant, _bundle_name) < 0)
                    return _bundle_name;

                string[] split = _bundle_name.Split('.');

                int bestFit = int.MaxValue;
                int bestFitIndex = -1;

                int varitantCount = bundlesWithVariant.Length;
                for (int i = 0; i < varitantCount; i++)
                {
                    string atVariant = bundlesWithVariant[i];

                    string[] curSplit = atVariant.Split('.');
                    if (curSplit[0] != split[0])
                        continue;

                    int found = System.Array.IndexOf(_variants, curSplit[1]);
                    if (found != -1 && found < bestFit)
                    {
                        bestFit = found;
                        bestFitIndex = i;
                    }
                }

                if (bestFitIndex != -1)
                    return bundlesWithVariant[bestFitIndex];
            }

            return _bundle_name;
        }

        public bool IsEncodedBundle(string _bundle_name)
        {
            if (BundleMgr.useEncryptBundle)
            {
                // 별도로 압축해서 사용할 번들이면 true 리턴
            }

            return false;
        }

        public static byte[] FileStreamToBytes(FileStream _file_stream)
        {
            if (null == _file_stream)
                return null;

            byte[] bytes = new byte[_file_stream.Length];
            int numBytesToRead = (int)_file_stream.Length;
            int numBytesRead = 0;
            while (numBytesToRead > 0)
            {
                int n = _file_stream.Read(bytes, numBytesRead, numBytesToRead);

                if (n == 0)
                    break;

                numBytesRead += n;
                numBytesToRead -= n;
            }

            return bytes;
        }
    }
}