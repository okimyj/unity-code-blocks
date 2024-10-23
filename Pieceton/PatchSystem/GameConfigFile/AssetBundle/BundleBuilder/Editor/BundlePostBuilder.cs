using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pieceton.PatchSystem
{
    public class BundlePostBuilder
    {
        public static bool ExistInvalidBundleVariantName(BundleManifest _manifest)
        {
            string[] allNames = _manifest.manifest.GetAllAssetBundles();
            string[] allWithVariantNames = _manifest.manifest.GetAllAssetBundlesWithVariant();

            if (null == allNames)
                return false;

            if (null == allWithVariantNames)
                return false;

            List<string> invalidList = new List<string>();

            int bundleCount = allNames.Length;
            for (int i = 0; i < bundleCount; ++i)
            {
                if (!ExistInVariant(allNames[i], allWithVariantNames))
                {
                    if (allNames[i].Split('.').Length > 1)
                    {
                        invalidList.Add(allNames[i]);
                    }
                }
            }

            if (invalidList.Count > 0)
            {
                string errMsg = "";

                foreach (string bundleName in invalidList)
                {
                    errMsg += string.Format("[{0}]\n", bundleName);

                    string[] allBundlePaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
                    if (null != allBundlePaths)
                    {
                        int pathCount = allBundlePaths.Length;
                        if (pathCount > 0)
                        {
                            for (int n = 0; n < pathCount; ++n)
                            {
                                errMsg += string.Format("{0}\n", allBundlePaths[n]);
                            }
                        }
                    }
                }

                string desc = string.Format("dot in bundle name. count='{0}'\n{1}", invalidList.Count, errMsg);
                throw new Exception(desc);
            }

            return false;
        }

        private static bool ExistInVariant(string _bundle_name, string[] _all_variants)
        {
            int count = _all_variants.Length;

            for (int i = 0; i < count; i++)
            {
                if (_all_variants[i].Equals(_bundle_name))
                    return true;
            }

            return false;
        }
    }
}