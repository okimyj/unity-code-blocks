#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Pieceton.PatchSystem
{
    public class TBundleRef_LoadAsset : TestBundleReference
    {
        private string bundleName = "testbundle";
        private Sprite sp;

        protected override IEnumerator OnProccess()
        {
            Do(LoadAsset);

            yield return Delay();

            Do(UnloadAsset);
        }

        private void LoadAsset()
        {
            sp = BundleMgr.LoadAsset<Sprite>(bundleName, "testAsset", ResExtension.UI_SPRITE);

            CheckReference(bundleName, 1);
        }

        private void UnloadAsset()
        {
            BundleMgr.UnloadAsset(sp);
        }
    }
}
#endif //UNITY_EDITOR