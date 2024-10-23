#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Pieceton.PatchSystem
{
    public class TBundleRef_AsyncLoadAsset : TestBundleReference
    {
        private string bundleName = "testbundle";
        private Sprite affectPartyAsync;

        protected override IEnumerator OnProccess()
        {
            yield return IEnumDo(LoadAsyncAsset);

            yield return Delay();

            Do(UnloadAsset);
        }

        private void TestOnAssetLoadAsync(Sprite _sprite)
        {
            Debug.LogFormat("TestOnAssetLoadAsync() sprite='{0}'", _sprite);
            affectPartyAsync = _sprite;
        }

        private IEnumerator LoadAsyncAsset()
        {
            yield return BundleMgr.LoadAssetAsync<Sprite>(TestOnAssetLoadAsync, bundleName, "testAsset", ResExtension.UI_SPRITE);

            CheckReference(bundleName, 1);
        }

        private void UnloadAsset()
        {
            BundleMgr.UnloadAsset(affectPartyAsync);
        }
    }
}
#endif //UNITY_EDITOR