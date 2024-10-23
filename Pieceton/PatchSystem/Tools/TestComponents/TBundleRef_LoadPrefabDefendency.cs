#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Pieceton.PatchSystem
{
    public class TBundleRef_LoadPrefabDefendency : TestBundleReference
    {
        string bundleName = "testprefab_ref";
        string dependecyBundleName = "testbundle";

        private GameObject go;

        protected override IEnumerator OnProccess()
        {
            Do(LoadAsset);

            yield return Delay();

            Do(UnloadAsset);
        }

        private void LoadAsset()
        {
            go = BundleMgr.LoadAsset<GameObject>(bundleName, "testPrefab_ref", ResExtension.PREFAB);

            CheckReference(bundleName, 1);
            CheckReference(dependecyBundleName, 1);
        }

        private void UnloadAsset()
        {
            BundleMgr.UnloadAsset(go);

            CheckReference(bundleName, 0);
            CheckReference(dependecyBundleName, 0);
        }
    }
}
#endif //UNITY_EDITOR