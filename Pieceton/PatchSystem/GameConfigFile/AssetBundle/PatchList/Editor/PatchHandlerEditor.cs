using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

using Pieceton.Misc;
using Pieceton.Configuration;

namespace Pieceton.PatchSystem
{
    public static partial class PatchHandlerEditor
    {
        public static void Make(BundleManifest _manifest, BuildTarget _build_target, ReleaseType _release_type, string _version_name)
        {
            string _platform = PBundlePathEditor.PlatformName(_build_target);
            string _cache_path = PBundlePathEditor.BuildOutputPath(_build_target, _release_type);

            //string _patch_path = PatchSystemEditor.ASSET_BUNDLE_ARCHIVE_PATH + _platform + "/" + _version_name;
            string _patch_path = PBundlePathEditor.MakeBundleArchivePath(_build_target, _version_name);

            if (CopyAssetBundleToPatch(_manifest, _cache_path, _patch_path, _platform))
            {
                CopyConfigFile(_patch_path);

                PatchListMaker.Make(_manifest, _cache_path, _patch_path);
                PatchListCheckFileMaker.Make(_manifest, _build_target, _version_name);

                PatchListUploadFileMaker.Make(_manifest, _build_target, _version_name);
            }
        }

        private static bool CopyAssetBundleToPatch(BundleManifest _manifest, string _bundle_cache_path, string _patch_path, string _platform)
        {
            if (Directory.Exists(_bundle_cache_path))
            {
                string _src_path = Path.Combine(System.Environment.CurrentDirectory, _bundle_cache_path);
                string _dst_path = Path.Combine(System.Environment.CurrentDirectory, _patch_path);

                try
                {
                    if (Directory.Exists(_dst_path))
                        FileUtil.DeleteFileOrDirectory(_dst_path);

                    Directory.CreateDirectory(_dst_path);
                }
                catch (Exception e)
                {
                    string msg = string.Format("CopyAssetBundleToPatch_HashVersion() error='{0}'\npath={1}", e, _dst_path);
                    throw new Exception(msg);
                }

                try
                {
                    string _minifest_file_path = _bundle_cache_path + "/" + _platform;
                    if (File.Exists(_minifest_file_path))
                    {
                        string _bundle_src_root_path = _src_path + "/";
                        string _bundle_dst_path = _dst_path + "/";

                        string[] _bundles = _manifest.manifest.GetAllAssetBundles();
                        if (null != _bundles)
                        {
                            int count = _bundles.Length;
                            for (int n = 0; n < count; ++n)
                            {
                                string _bundle_name = _bundles[n];
                                string _src_bundle_path = _bundle_src_root_path + _bundle_name;

                                string _hash = _manifest.GetHashFolderName(_bundle_name);

                                string _dst_bundle_path = _bundle_dst_path + _hash + "/";
                                Directory.CreateDirectory(_dst_bundle_path);

                                _dst_bundle_path += PBundlePath.MakePackFileName(_bundle_name);

                                if (!PFileCopy.Copy(_src_bundle_path, _dst_bundle_path))
                                {
                                    string msg = string.Format("not fount config file. {0}", _src_bundle_path);
                                    throw new Exception(msg);
                                }
                            }
                        }

                        return true;
                    }
                }
                catch (Exception e)
                {
                    string msg = string.Format("CopyAssetBundleToPatch_HashVersion() error='{0}'", e);
                    throw new Exception(msg);
                }
            }

            return false;
        }

        private static void CopyConfigFile(string _patch_path)
        {
            try
            {
                if (!Directory.Exists(PatchSystemEditor.CONFIG_ORIGINAL_PATH))
                    Directory.CreateDirectory(PatchSystemEditor.CONFIG_ORIGINAL_PATH);
            }
            catch (Exception e)
            {
                string msg = string.Format("Failed Copy config file. path = {0}\n{1}", _patch_path, e);
                throw new Exception(msg);
            }

            string _src_config_file_path = PatchSystemEditor.CONFIG_ORIGINAL_PATH + DefGameConfig.CONFIG_FILENAME;
            string _dst_config_file_path = _patch_path + "/" + DefGameConfig.CONFIG_FILENAME;

            if (!PFileCopy.Copy(_src_config_file_path, _dst_config_file_path))
            {
                string msg = string.Format("not fount config file. {0}", _src_config_file_path);
                throw new Exception(msg);
            }
        }
    }
}