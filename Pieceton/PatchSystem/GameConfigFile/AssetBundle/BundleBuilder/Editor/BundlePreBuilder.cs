using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class BundlePreBuilder
{
    public static bool HasDuplicatedAssetInBundle(string[] _bundle_names)
    {
        Dictionary<string, List<string>> allDic = MakeBundleDictionary(_bundle_names);
        if (allDic.Count <= 0)
            return false;

        Dictionary<string, List<string>> dupDic = MakeDuplicatedAssetDictionary(allDic);
        if (dupDic.Count <= 0)
            return false;

        ProccessDuplicatedAssets(dupDic);

        return true;
    }

    public static bool HasIncludedDotInBundle(string[] _bundle_names)
    {
        if (null == _bundle_names)
            return false;

        int count = _bundle_names.Length;

        string errMsg = "";

        for (int i = 0; i < count; ++i)
        {
            string bundleName = _bundle_names[i];
            if (bundleName.Split('.').Length > 2)
            {
                errMsg += bundleName + "\n";
            }
        }

        if (!string.IsNullOrEmpty(errMsg))
        {
            string desc = string.Format("invliad bundle name.\n{0}", errMsg);
            throw new Exception(desc);
        }

        return true;
    }

    private static Dictionary<string, List<string>> MakeBundleDictionary(string[] _bundle_names)
    {
        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

        if (null == _bundle_names)
            return dic;

        int count = _bundle_names.Length;
        if (count <= 0)
            return dic;

        for (int i = 0; i < count; ++i)
        {
            string bundleName = _bundle_names[i];

            List<string> pathList;
            if (!dic.TryGetValue(bundleName, out pathList))
            {
                pathList = new List<string>();
                dic.Add(bundleName, pathList);
            }

            string[] allBundlePaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
            if (null != allBundlePaths)
            {
                int pathCount = allBundlePaths.Length;
                if (pathCount > 0)
                {
                    for (int n = 0; n < pathCount; ++n)
                    {
                        pathList.Add(allBundlePaths[n]);
                    }
                }
            }
        }

        return dic;
    }

    private static Dictionary<string, List<string>> MakeDuplicatedAssetDictionary(Dictionary<string, List<string>> _all_bundle_dic)
    {
        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

        if (null == _all_bundle_dic)
            return dic;

        if (_all_bundle_dic.Count <= 0)
            return dic;

        Dictionary<string, List<string>> tempDic = new Dictionary<string, List<string>>();

        Dictionary<string, List<string>>.Enumerator iter = _all_bundle_dic.GetEnumerator();
        while (iter.MoveNext())
        {
            string bundleName = iter.Current.Key;
            List<string> list = iter.Current.Value;

            int count = list.Count;
            if (count <= 0)
                continue;

            tempDic.Clear();

            for (int i = 0; i < count; ++i)
            {
                string assetName = Path.GetFileName(list[i]);

                List<string> pathList;
                if (!tempDic.TryGetValue(assetName, out pathList))
                {
                    pathList = new List<string>();
                    tempDic.Add(assetName, pathList);
                }

                pathList.Add(list[i]);
            }

            if (tempDic.Count <= 0)
                continue;

            Dictionary<string, List<string>>.Enumerator tmpIter = tempDic.GetEnumerator();
            while (tmpIter.MoveNext())
            {
                string assetName = tmpIter.Current.Key;
                List<string> pathList = tmpIter.Current.Value;

                int pathCount = pathList.Count;
                if (pathCount > 1)
                {
                    List<string> dupList;
                    if (!dic.TryGetValue(bundleName, out dupList))
                    {
                        dupList = new List<string>();
                        dic.Add(bundleName, dupList);
                    }

                    dupList.AddRange(pathList);
                }
            }
        }

        return dic;
    }

    private static void ProccessDuplicatedAssets(Dictionary<string, List<string>> _duplicated_dic)
    {
        if (null == _duplicated_dic)
            return;

        if (_duplicated_dic.Count <= 0)
            return;

        string errMsg = "";

        Dictionary<string, List<string>>.Enumerator iter = _duplicated_dic.GetEnumerator();
        while (iter.MoveNext())
        {
            string bundleName = iter.Current.Key;
            List<string> list = iter.Current.Value;

            errMsg += string.Format("Duplicated assets in bundle('{0}')\n", bundleName);

            int count = list.Count;
            for (int i = 0; i < count; ++i)
            {
                errMsg += string.Format("{0}\n", list[i]);
            }

            errMsg += "\n";
        }

        throw new Exception(errMsg);
    }
}