using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pieceton.PatchSystem
{
    public class DefPatchHandler
    {
        public const string PATCH_LIST_FILENAME = "patch_list.txt";
        public const string PATCH_LIST_DEBUG_FILENAME = "patch_list_debug.txt";
        public const string PATCH_LIST_BUNDLES_FILENAME = "patch_list_bundles.txt";

        public const string KEY_DATA = "data";
        public const string KEY_DATA_BUNDLE_NAME = "bundleName";
        public const string KEY_DATA_HASH = "hash";
        public const string KEY_DATA_FILE_SIZE = "size";
        public const string KEY_DATA_FILE_CRC = "crc";
        public const string KEY_DATA_DEPENDENCY = "dependency";

        public const string KEY_VARIANT = "variant";
        public const string KEY_VARIANT_VALUE = "value";
    }

    public class AssetBundlePatchData
    {
        public string hash;
        public int size;
        public uint crc;
        public List<string> dependencyList;

        public bool Equal(string _hash, uint _crc)
        {
            return (hash.Equals(_hash) && crc.Equals(_crc));
        }
    }
}