using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Pieceton.PatchSystem
{
    public class UnusedBundleFileRemover
    {
        public static void Remove(AssetBundleManifest _manifest, BuildTarget _build_target, ReleaseType _release_type)
        {
            string _platform = PBundlePathEditor.PlatformName(_build_target);
            string _out_path = PBundlePathEditor.BuildOutputPath(_build_target, _release_type);

            Dictionary<string, string> _bundle_dic = new Dictionary<string, string>();

            string _manifest_extension = ".manifest";

            _bundle_dic.Add(_platform, "");
            _bundle_dic.Add(_platform + _manifest_extension, "");

            string[] _bundles = _manifest.GetAllAssetBundles();
            if (null != _bundles)
            {
                int count = _bundles.Length;
                for (int n = 0; n < count; ++n)
                {
                    string _file_name = _bundles[n];
                    _bundle_dic.Add(_file_name, null);
                    _bundle_dic.Add(_file_name + _manifest_extension, null);
                }
            }

            string[] _files = Directory.GetFiles(_out_path, "*.*", SearchOption.AllDirectories);
            if (null != _files)
            {
                string _separatorString = string.Format("{0}", Path.DirectorySeparatorChar);
                string _remove_string = _out_path + _separatorString;

                int count = _files.Length;
                for (int n = 0; n < count; ++n)
                {
                    string _file_name = _files[n];
                    _file_name = _file_name.Replace(_remove_string, "");
                    _file_name = _file_name.Replace("\\", "/");

                    if (!_bundle_dic.ContainsKey(_file_name))
                    {
                        //파일 제거
                        string _remove_file = _out_path + _separatorString + _file_name;
                        File.Delete(_remove_file);
                    }
                }
            }
        }
    }
}