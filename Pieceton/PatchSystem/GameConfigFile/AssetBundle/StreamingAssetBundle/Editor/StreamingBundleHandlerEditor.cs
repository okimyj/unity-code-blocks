using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

using Pieceton.Misc;
using Pieceton.Configuration;

namespace Pieceton.PatchSystem
{
    public partial class StreamingBundleHandlerEditor
    {
        public static bool Make(BundleManifest _manifest, BuildTarget _build_target, ReleaseType _release_type)
        {
            string _platform = PBundlePathEditor.PlatformName(_build_target);
            string _out_path = PBundlePathEditor.BuildOutputPath(_build_target, _release_type);

            if (Directory.Exists(_out_path))
            {
                string _src_path = Path.Combine(System.Environment.CurrentDirectory, _out_path);
                string _dst_path = StreamingBundleHandler.streamingBundlePath;

                CreateDirectory(_dst_path);

                string _minifest_file_path = _out_path + "/" + _platform;
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

                            Copy(_manifest, _bundle_name, _bundle_src_root_path, _bundle_dst_path);
                        }
                    }

                    return true;
                }
                else
                {
                    Debug.LogErrorFormat("Failed CopyStreamingAssets. not exist manifest. {0}", _minifest_file_path);
                }
            }
            else
            {
                Debug.LogErrorFormat("Failed CopyStreamingAssets. not exist asset path. {0}", _out_path);
            }

            return false;
        }

        private static void CreateDirectory(string _dst_path)
        {
            try
            {
                if (Directory.Exists(_dst_path))
                {
                    Debug.Log("remove streaming asset directory");
                    FileUtil.DeleteFileOrDirectory(_dst_path);

                    if (Directory.Exists(_dst_path))
                    {
                        Debug.LogWarning("dont remove streaming asset directory");
                    }
                }

                Directory.CreateDirectory(_dst_path);
            }
            catch (Exception e)
            {
                string errMsg = string.Format("CreateDirectory() path='{0}'\n err={1}", _dst_path, e);
                throw new Exception(errMsg);
            }
        }

        private static void Copy(BundleManifest _manifest, string _bundle_name, string _bundle_src_root_path, string _bundle_dst_path)
        {
            string _src_bundle_path = _bundle_src_root_path + _bundle_name;

            //string _hash = _manifest.GetHashCode(_bundle_name);
            string _hash = _manifest.GetHashFolderName(_bundle_name);

            Debug.LogFormat("copy streaming asset {0}/{1}", _hash, _bundle_name);

            string _dst_bundle_path = _bundle_dst_path + _hash + "/";
            Directory.CreateDirectory(_dst_bundle_path);
            _dst_bundle_path += PBundlePath.MakePackFileName(_bundle_name);

            File.Copy(_src_bundle_path, _dst_bundle_path);
        }

        public static void DeleteInternalBundle()
        {
            FileUtil.DeleteFileOrDirectory(StreamingBundleHandler.streamingBundleMetaPath);
            FileUtil.DeleteFileOrDirectory(StreamingBundleHandler.streamingBundlePath);
        }

        public static void CopyStreamPatchFiles(BuildTarget _build_target, ReleaseType _release_type, string _version_name)
        {
            string _platform = PBundlePathEditor.PlatformName(_build_target);
            string _cache_path = PBundlePathEditor.BuildOutputPath(_build_target, _release_type);

            //string _patch_path = PatchSystemEditor.ASSET_BUNDLE_ARCHIVE_PATH + _platform + "/" + _version_name;
            string _patch_path = PBundlePathEditor.MakeBundleArchivePath(_build_target, _version_name);

            string _src_config_file_path = PatchSystemEditor.CONFIG_ORIGINAL_PATH + DefGameConfig.CONFIG_FILENAME;
            string _dst_config_file_path = StreamingBundleHandler.streamingBundlePath + DefGameConfig.CONFIG_FILENAME;

            if (!PFileCopy.Copy(_src_config_file_path, _dst_config_file_path))
            {
                string msg = string.Format("not fount config file. {0}", _src_config_file_path);
                throw new Exception(msg);
            }

            string _src_patch_file_path = _patch_path + "/" + DefPatchHandler.PATCH_LIST_FILENAME;
            string _dst_patch_file_path = StreamingBundleHandler.streamingBundlePath + DefPatchHandler.PATCH_LIST_FILENAME;

            if (!PFileCopy.Copy(_src_patch_file_path, _dst_patch_file_path))
            {
                string msg = string.Format("not fount config file. {0}", _src_patch_file_path);
                throw new Exception(msg);
            }

            string _src_debug_file_path = _patch_path + "/" + DefPatchHandler.PATCH_LIST_DEBUG_FILENAME;
            string _dst_debug_file_path = StreamingBundleHandler.streamingBundlePath + DefPatchHandler.PATCH_LIST_DEBUG_FILENAME;

            if (!PFileCopy.Copy(_src_debug_file_path, _dst_debug_file_path))
            {
                string msg = string.Format("not fount config file. {0}", _src_debug_file_path);
                throw new Exception(msg);
            }
        }
    }
}