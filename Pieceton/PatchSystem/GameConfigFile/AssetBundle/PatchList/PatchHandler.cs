using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using System;
using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public static partial class PatchHandler
    {
        public static Dictionary<string, AssetBundlePatchData> totalDic { get { return patchLiatInfo.totalDic; } }
        public static List<string> variants { get { return patchLiatInfo.variants; } }
        public static string[] allBundleList { get { return patchLiatInfo.allBundleList; } }

        public const bool WITH_CRC_HASH_FOLDER_NAME = true;

        private static PatchListInfo patchLiatInfo = new PatchListInfo();

        public static void LoadPatchData(string _json_data)
        {
            patchLiatInfo.Load(_json_data);
        }

        public static bool IsValid()
        {
            return (totalDic.Count > 0 && null != allBundleList);
        }

        public static Hash128 GetBundleHash128(string _bundle_name)
        {
            AssetBundlePatchData _data = GetTotalData(_bundle_name);
            if (null != _data)
                return Hash128.Parse(_data.hash);

            return new Hash128(0, 0, 0, 0);
        }

        public static string GetHashFolderName(string _bundle_name)
        {
            AssetBundlePatchData _data = GetTotalData(_bundle_name);
            if (null != _data)
            {
                return MakeHashFolderName(_data.hash, _data.crc.ToString());
            }

            return "";
        }

        public static string MakeHashFolderName(string _hash, string _crc)
        {
            if (WITH_CRC_HASH_FOLDER_NAME)
            {
                return _hash + "-" + _crc;
            }

            return _hash;
        }

        public static string GetBundleHashString(string _bundle_name)
        {
            AssetBundlePatchData _data = GetTotalData(_bundle_name);
            if (null != _data)
                return _data.hash;

            return "";
        }

        public static int GetBundleSize(string _bundle_name)
        {
            AssetBundlePatchData patch;
            if (totalDic.TryGetValue(_bundle_name, out patch))
            {
                return patch.size;
            }

            return 0;
        }

        public static uint GetBundleCRC(string _bundle_name)
        {
            AssetBundlePatchData patch;
            if (totalDic.TryGetValue(_bundle_name, out patch))
            {
                return patch.crc;
            }

            return 0;
        }

        public static bool IsValidCRC(string _bundle_name)
        {
            AssetBundlePatchData data = GetTotalData(_bundle_name);
            if (null != data)
            {
                try
                {
                    string path = MakeHashBundlePath(BundleStorageHandler.bundleStoragePath, _bundle_name);
                    if (File.Exists(path))
                    {
                        uint crc = PCRCChecker.GetCRC(File.ReadAllBytes(path));
                        bool isValid = data.crc.Equals(crc);
                        if (isValid)
                        {
                            Debug.LogFormat("PatchHandler::IsValidCRC() Valid crc. path = '{0}'", path);
                        }
                        else
                        {
                            Debug.LogErrorFormat("PatchHandler::IsValidCRC() Invalid crc. path = '{0}'", path);
                        }
                        return isValid;
                    }

                    Debug.LogErrorFormat("PatchHandler::IsValidCRC() Not found bundle. path = '{0}'", path);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("PatchHandler::IsValidCRC() {0}", e);
                }
            }

            return false;
        }

        public static AssetBundlePatchData GetTotalData(string _bundle_name)
        {
            AssetBundlePatchData _result;
            totalDic.TryGetValue(_bundle_name, out _result);
            return _result;
        }

        public static string[] GetAllDependency(string _bundle_name)
        {
            AssetBundlePatchData _data = GetTotalData(_bundle_name);
            if (null != _data && null != _data.dependencyList)
            {
                //Debug.LogError("dependency : " + _data.dependencyList.Count);
                return _data.dependencyList.ToArray();
            }

            return null;
        }

        // input : (cdnurl, bundlename)
        // output : cdnurl/budnehash/bundlename
        public static string MakeHashBundleURL(string _base_path, string _bundle_name)
        {
            string packName = Path.GetFileName(_bundle_name);

            if (IsValid())
            {
                return _base_path + GetHashFolderName(_bundle_name) + "/" + packName;
            }

            return _base_path + packName;
        }

        // input : (cdnurl, bundlename)
        // output : cdnurl/budnehash/bundlename.pak
        public static string MakeHashBundlePath(string _base_path, string _bundle_name)
        {
            string packName = PBundlePath.MakePackFileName(_bundle_name);

            if (IsValid())
            {
                return _base_path + GetHashFolderName(_bundle_name) + "/" + packName;
            }

            return _base_path + packName;
        }

        // bundlename.variant1 => bundlename
        // bundlename.variant2 => bundlename
        public static string GetBundleName_ExceptVariant(string _bundle_name)
        {
            string reName = _bundle_name;

            List<string> variantList = variants;
            int count = variantList.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    if (variantList[i].Equals(_bundle_name))
                    {
                        reName = _bundle_name.Substring(0, _bundle_name.LastIndexOf('.'));
                        break;
                    }
                }
            }

            return reName;
        }

        // bundlename => bundlename.variant1
        // bundlename => bundlename.variant2
        public static string GetBundleName_InclusionVariant(string _bundle_name)
        {
            string excName = GetBundleName_ExceptVariant(_bundle_name);

            if (string.IsNullOrEmpty(BundleMgr.activatedVariant))
                return excName;

            List<string> variantList = variants;
            int count = variantList.Count;
            if (count > 0)
            {
                string reName = excName + "." + BundleMgr.activatedVariant;

                for (int i = 0; i < count; ++i)
                {
                    if (variantList[i].Equals(reName))
                        return variantList[i];
                }
            }

            return excName;
        }
    }
}