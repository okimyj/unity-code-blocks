using UnityEngine;
using System.Collections;

using UnityEditor;

namespace Pieceton.Misc
{
    public static class PPath
    {
        public static string GetParent(string fullPath)
        {
            int idx = fullPath.LastIndexOf('/');
            if (idx <= 0)
                return fullPath;

            return fullPath.Substring(0, idx);
        }

        public static bool IsInstancePrefab(Object prefab)
        {
            PrefabType prefabType = PrefabUtility.GetPrefabType(prefab);
            switch (prefabType)
            {
                case PrefabType.ModelPrefabInstance:
                case PrefabType.PrefabInstance:
                    return true;
            }

            return false;
        }

        public static string GetPrefabPath(Object prefab)
        {
            if (prefab == null)
                return null;

            string path = AssetDatabase.GetAssetPath(prefab);
            while (string.IsNullOrEmpty(path))
            {
                UnityEngine.Object parent = PrefabUtility.GetCorrespondingObjectFromSource(prefab);
                if (parent == null)
                    break;
                path = AssetDatabase.GetAssetPath(parent);
                prefab = parent;
            }

            if (false == string.IsNullOrEmpty(path))
            {
                string[] result = path.Split('/');
                return result[result.Length - 2];
            }
            return path;
        }

        public static string EraseCloneString(string sName)
        {
            int idx = sName.IndexOf('(');
            if (idx <= 0)
                return sName;

            return sName.Substring(0, idx);
        }
    }
}