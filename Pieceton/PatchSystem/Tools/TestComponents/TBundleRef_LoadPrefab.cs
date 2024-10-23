#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Pieceton.PatchSystem
{
    public class TBundleRef_LoadPrefab : TestBundleReference
    {
        private string bundleName = "testprefab";
        private GameObject go;

        protected override IEnumerator OnProccess()
        {
            Do(LoadPrefab);

            yield return Delay();

            Do(UnloadPrefab);
        }

        private void LoadPrefab()
        {
            go = BundleMgr.LoadAsset<GameObject>(bundleName, "testPrefab", ResExtension.PREFAB);

            CheckReference(bundleName, 1);
        }

        private void UnloadPrefab()
        {
            BundleMgr.UnloadAsset(go);
        }
    }
}
#endif //UNITY_EDITOR