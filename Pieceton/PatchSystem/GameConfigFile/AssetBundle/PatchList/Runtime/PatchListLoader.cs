using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public partial class PatchHandler
    {
        //public static bool Load(string _json_data, Dictionary<string, AssetBundlePatchData> _dic, List<string> _variants)
        //{
        //    if (null == _dic)
        //        _dic = new Dictionary<string, AssetBundlePatchData>();

        //    if (null == _variants)
        //        _variants = new List<string>();

        //    _dic.Clear();
        //    _variants.Clear();

        //    if (!string.IsNullOrEmpty(_json_data))
        //    {
        //        JSONNode _root = JSONNode.LoadFromBase64(_json_data);
        //        if (null == _root)
        //            return false;

        //        if (false == ParseAssetBundlePatchData(_root, _dic))
        //            return false;

        //        if (false == ParseAssetBundlePatchData_Variant(_root, _variants))
        //            return false;
        //    }

        //    return true;
        //}

        //private static bool ParseAssetBundlePatchData(JSONNode _root, Dictionary<string, AssetBundlePatchData> _dic)
        //{
        //    if (null == _root)
        //    {
        //        return false;
        //    }

        //    JSONNode _total = _root[DefPatchHandler.KEY_DATA];
        //    if (null == _total)
        //    {
        //        return false;
        //    }

        //    int count = _total.Count;
        //    for (int n = 0; n < count; ++n)
        //    {
        //        JSONNode _node = _total[n];
        //        if (null == _node)
        //        {
        //            return false;
        //        }

        //        if (null == _node[DefPatchHandler.KEY_DATA_BUNDLE_NAME])
        //        {
        //            return false;
        //        }

        //        if (string.IsNullOrEmpty(_node[DefPatchHandler.KEY_DATA_HASH]))
        //        {
        //            return false;
        //        }

        //        string bundleName = _node[DefPatchHandler.KEY_DATA_BUNDLE_NAME];

        //        AssetBundlePatchData _data = new AssetBundlePatchData();
        //        _data.hash = _node[DefPatchHandler.KEY_DATA_HASH];
        //        _data.size = PDataParser.ParseInt(_node[DefPatchHandler.KEY_DATA_FILE_SIZE], 0);
        //        _data.crc = PDataParser.ParseUInt(_node[DefPatchHandler.KEY_DATA_FILE_CRC], 0);

        //        JSONNode _node_dependency = _node[DefPatchHandler.KEY_DATA_DEPENDENCY];
        //        if (null != _node_dependency)
        //        {
        //            int _dependency_count = _node_dependency.Count;
        //            if (_dependency_count > 0)
        //            {
        //                _data.dependencyList = new List<string>();

        //                for (int i = 0; i < _dependency_count; ++i)
        //                {
        //                    //JSONData data = _node_dependency[i] as JSONData;   
        //                    if (null != _node_dependency[i])
        //                        _data.dependencyList.Add(_node_dependency[i]);

        //                    //if(Debug.isDebugBuild)
        //                    // Debug.Log("####### assetbundle " + _node[KEY_DATA_BUNDLE_NAME] + "parse dependency : " + i + " :: " + _node_dependency[i]);//by blackerl
        //                }
        //            }
        //        }

        //        _dic.Add(bundleName, _data);
        //    }

        //    return true;
        //}

        //private static bool ParseAssetBundlePatchData_Variant(JSONNode _root, List<string> _list)
        //{
        //    if (null == _root)
        //        return false;

        //    if (null == _list)
        //        return false;

        //    _list.Clear();

        //    JSONNode _patch = _root[DefPatchHandler.KEY_VARIANT];
        //    if (null != _patch)
        //    {
        //        int count = _patch.Count;
        //        for (int n = 0; n < count; ++n)
        //        {
        //            JSONNode _node = _patch[n];
        //            if (null != _node && null != _node[DefPatchHandler.KEY_VARIANT_VALUE])
        //            {
        //                _list.Add(_node[DefPatchHandler.KEY_VARIANT_VALUE]);
        //            }
        //        }
        //    }

        //    return true;
        //}
    }
}