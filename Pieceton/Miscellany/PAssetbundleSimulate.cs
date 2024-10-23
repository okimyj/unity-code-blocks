using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pieceton.Misc
{
    public static class PAssetBundleSimulate
    {
        private static int m_SimulateAssetBundleInEditor = -1;
        private const string kSimulateAssetBundles = "SimulateAssetBundles";

        public static bool active
        {
#if UNITY_EDITOR
            get
            {
                if (m_SimulateAssetBundleInEditor == -1)
                    m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, false) ? 1 : 0;

                return m_SimulateAssetBundleInEditor != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != m_SimulateAssetBundleInEditor)
                {
                    m_SimulateAssetBundleInEditor = newValue;
                    EditorPrefs.SetBool(kSimulateAssetBundles, value);
                }
            }
#else
        get { return true;}
#endif
        }
    }
}