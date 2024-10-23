using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public class PatchProcessor : MonoBehaviour
    {
        void Awake()
        {
            WWWDownloadMgr.notifyDownloadErrorGameRestart += OnErrAskRestart;
            WWWDownloadMgr.notifyDownloadErrorRetry += OnErrAskRetry;

            WWWDownloadMgr.notifyDownloadFileName += OnNotifyDownloadBundleName;
            WWWDownloadMgr.notifyTotalProgress += OnNotifyTotalProgress;
            WWWDownloadMgr.notifyCurrentProgress += OnNotifyDonwloadingBundleProgress;
        }

        private void OnDestroy()
        {
            WWWDownloadMgr.notifyDownloadErrorGameRestart -= OnErrAskRestart;
            WWWDownloadMgr.notifyDownloadErrorRetry -= OnErrAskRetry;

            WWWDownloadMgr.notifyDownloadFileName -= OnNotifyDownloadBundleName;
            WWWDownloadMgr.notifyTotalProgress -= OnNotifyTotalProgress;
            WWWDownloadMgr.notifyCurrentProgress -= OnNotifyDonwloadingBundleProgress;
        }

        // Use this for initialization
        public void StartPatch()
        {
            if (!PAssetBundleSimulate.active)
            {
                OnComplete();
                return;
            }

            StartCoroutine(ExecutePatch());
        }

        private IEnumerator ExecutePatch()
        {
            yield return OnBegin();
            yield return OnProcess();
            yield return OnEnd();

            OnComplete();
        }
        protected virtual IEnumerator OnBegin() { yield return null; }

        protected virtual IEnumerator OnEnd() { yield return null; }

        protected virtual IEnumerator OnProcess()
        {
            /////////////////////////////////////////////////////////////////////////////////////////////
            //// 캐시 데이터 용량 제한. 지정한 용량이 차면 오래된것부터 제거한다.
            //Caching.maximumAvailableDiskSpace = BundleMgr.maxCacheDiskSpace;

            /////////////////////////////////////////////////////////////////////////////////////////////
            //// 캐시 준비될때까지 대기
            ////SetDescription("ready cache");
            //while (!Caching.ready)
            //    yield return null;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // 패치 리스트 다운로드
            yield return WWWDownloadMgr.Instance.Init_PatchList();


            ///////////////////////////////////////////////////////////////////////////////////////////
            // 사용하지 않는 번들 저장소에서 삭제
            yield return BundleStorageHandler.DeleteUnusedBundles();

            ///////////////////////////////////////////////////////////////////////////////////////////
            // config
            yield return WWWDownloadMgr.Instance.Init_Config();


            ///////////////////////////////////////////////////////////////////////////////////////////
            // 사용가능한 스트리밍 번들 리스트업
            yield return StreamingBundleHandler.Init();


            ///////////////////////////////////////////////////////////////////////////////////////////
            // 다운로드 받을 번들이 있는지 확인후 메세지 박스 출력
            yield return WWWDownloadMgr.Instance.AskBundleDownloadMsgBox(null);


            ///////////////////////////////////////////////////////////////////////////////////////////
            // 다운로드 받아야 되는 번들 다운로드
            yield return WWWDownloadMgr.Instance.DownloadBundles();
        }


        #region Event Handler : -------------------------------------------------------------------------------------------
        protected virtual void OnComplete() { }
        protected virtual void OnErrAskRestart(FailDownload _fail_download, string _err_msg) { }
        protected virtual void OnErrAskRetry(System.Action<bool> _func, FailDownload _fail_download, string _err_msg) { }
        protected virtual void OnNotifyDownloadBundleName(string bundlename) { }
        protected virtual void OnNotifyTotalProgress(int cur, int total) { }
        protected virtual void OnNotifyDonwloadingBundleProgress(float progress, string bundlename) { }
        #endregion
    }

}