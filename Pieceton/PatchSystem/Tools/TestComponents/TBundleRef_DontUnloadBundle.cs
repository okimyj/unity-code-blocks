#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Pieceton.PatchSystem
{
    public class TBundleRef_DontUnloadBundle : TestBundleReference
    {
        private string bundleName = "testbundle";
        private Sprite sp;

        protected override IEnumerator OnProccess()
        {
            Do(AddDontUnloadBundle);

            Do(LoadAsset);

            yield return Delay();

            Do(UnloadAsset);

            yield return Delay();

            Do(RemoveDontUnloadBundle);

            Do(UnloadBundle);

            yield return Delay();

            Do(UnloadAsset_2);
        }

        private void AddDontUnloadBundle()
        {
            BundleMgr.AddDontUnloadAssetBundle(bundleName);

            CheckReference(bundleName, 0);
        }

        private void RemoveDontUnloadBundle()
        {
            BundleMgr.RemoveDontUnloadAssetBundle(bundleName);
        }

        private void LoadAsset()
        {
            sp = BundleMgr.LoadAsset<Sprite>(bundleName, "testAsset", ResExtension.UI_SPRITE);

            CheckReference(bundleName, 1);
        }

        private void UnloadAsset()
        {
            BundleMgr.UnloadAsset(sp);

            CheckReference(bundleName, 0);
        }

        private void UnloadBundle()
        {
            BundleMgr.UnloadUnusedAssetBundles();
            CheckReference(bundleName, 0);
        }

        private void UnloadAsset_2()
        {
            BundleMgr.UnloadAsset(sp);
            CheckReference(bundleName, 0);
        }
    }
}
#endif //UNITY_EDITOR