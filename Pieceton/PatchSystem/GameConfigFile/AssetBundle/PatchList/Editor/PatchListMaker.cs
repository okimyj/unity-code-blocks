using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleJSON;

using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public static partial class PatchListMaker
    {
        public static void Make(BundleManifest _manifest, string _bundle_cache_path, string _patch_path)
        {
            if (null == _manifest)
            {
                Debug.LogError("invalid manifest");
                return;
            }

            List<string> _patch_list = _manifest.bundleList;

            if (null == _patch_list)
            {
                Debug.LogError("invalid patchList");
                return;
            }

            JSONNode _root = new JSONObject();

            SavePatchList_PatchData(_root, _bundle_cache_path, _manifest);
            SavePatchList_Variant(_root, _manifest);

            // PATCH_LIST_FILENAME
            string _result = Helper_SimpleJson.SaveToFileBase64(_root, string.Format("{0}/{1}", _patch_path, DefPatchHandler.PATCH_LIST_FILENAME));
            PatchHandler.LoadPatchData(_result);

            // PATCH_LIST_DEBUG_FILENAME
            Helper_SimpleJson.SaveToJson(_root, string.Format("{0}/{1}", _patch_path, DefPatchHandler.PATCH_LIST_DEBUG_FILENAME));

            MakeBundleList(_root, _manifest, _patch_path);
        }

        private static void SavePatchList_PatchData(JSONNode _root, string _bundle_cache_path, BundleManifest _manifest)
        {
            JSONArray _total = new JSONArray();
            _root.Add(DefPatchHandler.KEY_DATA, _total);

            string[] _bundles = _manifest.manifest.GetAllAssetBundles();
            int _total_count = _bundles.Length;
            for (int n = 0; n < _total_count; ++n)
            {
                if (string.IsNullOrEmpty(_bundles[n]))
                    continue;

                JSONNode _node = new JSONObject();

                string _bundle_name = _bundles[n];
                string _bundle_file = _bundle_cache_path + "/" + _bundle_name;

                string _hash = _manifest.GetHashString(_bundle_name);

                _node.Add(DefPatchHandler.KEY_DATA_BUNDLE_NAME, _bundle_name);
                _node.Add(DefPatchHandler.KEY_DATA_HASH, _hash);

                FileStream _file_stream = File.Open(_bundle_file, FileMode.Open);
                if (null != _file_stream)
                {
                    _node.Add(DefPatchHandler.KEY_DATA_FILE_SIZE, _file_stream.Length.ToString());
                    _file_stream.Close();

                    string crcCode = BundleManifest.GetCRCString(_bundle_name);
                    _node.Add(DefPatchHandler.KEY_DATA_FILE_CRC, crcCode);
                }

                JSONArray _root_dependency = new JSONArray();

                string[] _dependencies = _manifest.manifest.GetAllDependencies(_bundle_name);
                if (null != _dependencies)
                {
                    int _dependency_count = _dependencies.Length;
                    if (_dependency_count > 0)
                    {
                        for (int i = 0; i < _dependency_count; ++i)
                        {
                            JSONNode _node_dependency = new JSONString(_dependencies[i]);
                            _root_dependency.Add(_dependencies[i], _node_dependency);
                        }
                    }
                }

                _node.Add(DefPatchHandler.KEY_DATA_DEPENDENCY, _root_dependency);

                _total.Add(_node);
            }
        }

        private static void SavePatchList_Variant(JSONNode _root, BundleManifest _manifest)
        {
            JSONArray _variant = new JSONArray();
            _root.Add(DefPatchHandler.KEY_VARIANT, _variant);

            string[] _bundles_with_variant = _manifest.manifest.GetAllAssetBundlesWithVariant();
            int _total_count = _bundles_with_variant.Length;
            for (int n = 0; n < _total_count; ++n)
            {
                string _value = _bundles_with_variant[n];
                if (string.IsNullOrEmpty(_value))
                    continue;

                JSONNode _node = new JSONObject();

                _node.Add(DefPatchHandler.KEY_VARIANT_VALUE, _value);

                _variant.Add(_node);
            }
        }

        private static void MakeBundleList(JSONNode _root, BundleManifest _manifest, string _patch_path)
        {
            int buildedBundleCount = _manifest.manifest.GetAllAssetBundles().Length;
            int loadedBundleCount = PatchHandler.allBundleList.Length;

            if (buildedBundleCount != loadedBundleCount)
            {
                string msg = string.Format("invalid bundle count. build='{0}' load='{1}'", buildedBundleCount, loadedBundleCount);
                throw new Exception(msg);
            }

            // PATCH_LIST_BUNDLES_FILENAME
            string _bundle_list_file = _patch_path + "/" + DefPatchHandler.PATCH_LIST_BUNDLES_FILENAME;
            StreamWriter _streamDebug = new StreamWriter(File.Open(_bundle_list_file, FileMode.Create));
            if (null != _streamDebug)
            {
                string[] _bundles = _manifest.manifest.GetAllAssetBundles();
                int _total_count = _bundles.Length;
                for (int n = 0; n < _total_count; ++n)
                {
                    if (string.IsNullOrEmpty(_bundles[n]))
                        continue;

                    string _bundle_name = _bundles[n];
                    //string _hash = _manifest.GetHashCode(_bundle_name);
                    string _hash = _manifest.GetHashFolderName(_bundle_name);

                    string write = string.Format("{0}:{1}", _hash, _bundle_name);
                    _streamDebug.WriteLine(write);
                }

                _streamDebug.Close();
            }
        }
    }
}