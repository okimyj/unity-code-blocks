using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

using Pieceton.Misc;
using Pieceton.Configuration;

namespace Pieceton.PatchSystem
{
    public class BundleManifest
    {
        public AssetBundleManifest manifest { get; private set; }

        private class ExtendInfo
        {
            public uint crc;
            public string crcString;
        }

        private static string _out_path = "";
        private static Dictionary<string, ExtendInfo> _extendInfoDic = new Dictionary<string, ExtendInfo>();

        public List<string> bundleList { get { return _bundleList; } }
        private List<string> _bundleList = new List<string>();

        public BundleManifest(BuildTarget _build_target, ReleaseType _release_type)
        {
            Debug.LogFormat("defined releaseFolder='{0}' used releaseFolder='{1}'", ReleaseInfo.releaseFolderName, DefReleaseType.GetReleaseFolderName(_release_type));

            _out_path = PBundlePathEditor.BuildOutputPath(_build_target, _release_type);
            _bundleList.Clear();

            manifest = BuildPipeline.BuildAssetBundles(_out_path, AssetBundleEditor.bundleBuildOption, _build_target);
            if (null == manifest)
            {
                throw new NullReferenceException("AssetBundleManifest is null");
            }

            UnusedBundleFileRemover.Remove(manifest, _build_target, _release_type);

            MakeBundleList();
            MakeBundleCRC();
        }

        public string GetHashFolderName(string _bundle_name)
        {
            string hash = GetHashString(_bundle_name);
            string crc = GetCRCString(_bundle_name);

            return PatchHandler.MakeHashFolderName(hash, crc);
        }

        public string GetHashString(string _bundle_name)
        {
            if (null != manifest)
            {
                Hash128 _hash = manifest.GetAssetBundleHash(_bundle_name);
                return _hash.ToString();
            }

            return "";
        }

        public static uint GetCRCCode(string _bundle_name)
        {
            ExtendInfo info = GetExtendInfo(_bundle_name);
            if (null != info)
            {
                return info.crc;
            }

            return 0;
        }

        public static string GetCRCString(string _bundle_name)
        {
            ExtendInfo info = GetExtendInfo(_bundle_name);
            if (null != info)
            {
                return info.crcString;
            }

            return "0";
        }

        private static ExtendInfo GetExtendInfo(string _bundle_name)
        {
            ExtendInfo info;
            if (_extendInfoDic.TryGetValue(_bundle_name, out info))
                return info;

            return null;
        }

        private void MakeBundleList()
        {
            _bundleList.Clear();

            string[] _bundles = manifest.GetAllAssetBundles();
            if (null == _bundles)
                return;

            int _bundle_count = _bundles.Length;
            if (_bundle_count <= 0)
                return;
            
            for (int n = 0; n < _bundle_count; ++n)
            {
                if (string.IsNullOrEmpty(_bundles[n]))
                {
                    throw new Exception("Make bundle crc fail. Invalid bundle name");
                }

                _bundleList.Add(_bundles[n]);
            }
        }

        private void MakeBundleCRC()
        {
            _extendInfoDic.Clear();

            string[] allBundles = manifest.GetAllAssetBundles();
            if (null == allBundles)
                return;

            int count = allBundles.Length;

            try
            {
                for (int i = 0; i < count; i++)
                {
                    string bundleName = allBundles[i];

                    ExtendInfo info = new ExtendInfo();
                    info.crc = PCRCChecker.GetCRC(File.ReadAllBytes(_out_path + "/" + bundleName));
                    info.crcString = info.crc.ToString();

                    _extendInfoDic.Add(bundleName, info);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Make bundle crc fail. err='{0}'", e);
            }
        }
    }
}