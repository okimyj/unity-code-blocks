using UnityEngine;
using System.Collections;

namespace Pieceton.Misc
{
    public static class PMissingComponentFinder
    {
#if UNITY_EDITOR
        private const bool useCheckMissingScript = true;
#else
        private const bool useCheckMissingScript = false;
#endif

        public static void FindAll_InHierarchy()
        {
            if (!useCheckMissingScript)
                return;

            Object[] objs = Object.FindObjectsOfType(typeof(GameObject));
            if (null == objs)
                return;

            int count = objs.Length;

            for (int i = 0; i < count; ++i)
            {
                Find((GameObject)objs[i]);
            }
        }

        public static void Find(GameObject _go)
        {
            if (!useCheckMissingScript)
                return;

            if (null == _go)
                return;

            CheckComponent(_go.transform);
        }

        private static void CheckComponent(Transform _tr)
        {
            if (null == _tr)
                return;

            int childCount = _tr.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                CheckComponent(_tr.GetChild(i));
            }

            if (childCount <= 0)
            {
                Component[] _conponents = _tr.GetComponentsInChildren(typeof(Component), true);

                int count = _conponents.Length;

                for (int i = 0; i < count; ++i)
                {
                    if (null == _conponents[i])
                    {
                        string name = MakePath(_tr.transform);
                        Debug.LogErrorFormat("[Have a missing script] => {0}", name);
                    }
                }
            }
        }

        private static string MakePath(Transform _tr)
        {
            string name = "";

            if (null != _tr)
            {
                string parentName = MakePath(_tr.parent);
                if (string.IsNullOrEmpty(parentName))
                {
                    name = _tr.gameObject.name;
                }
                else
                {
                    name = parentName + "::" + _tr.gameObject.name;
                }

            }

            return name;
        }
    }
}