using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Pieceton.PatchSystem
{
    public class PatchProccessorTest : PatchProcessor
    {
        public Text txtCurJob;
        public Text txtTotalProgress;
        public Text txtCurProgress;

        public Image imgSprite;

        void Awake()
        {
            WWWDownloadMgr.notifyDownloadFileName += UpdateCurrentJob;
            WWWDownloadMgr.notifyTotalProgress += UpdateTotalProgress;
            WWWDownloadMgr.notifyCurrentProgress += UpdateCurrentProgress;
        }

        void OnDistroy()
        {
            WWWDownloadMgr.notifyDownloadFileName -= UpdateCurrentJob;
            WWWDownloadMgr.notifyTotalProgress -= UpdateTotalProgress;
            WWWDownloadMgr.notifyCurrentProgress -= UpdateCurrentProgress;
        }

        // Use this for initialization
        void Start()
        {
            StartPatch();
        }

        protected override IEnumerator OnEnd()
        {
            yield return TestLoadBundles();
        }

        protected void UpdateCurrentJob(string _desc)
        {
            if (null != txtCurJob)
            {
                txtCurJob.text = _desc;
            }
        }

        private void UpdateTotalProgress(int _cur, int _total)
        {
            if (null != txtTotalProgress)
            {
                txtTotalProgress.text = string.Format("total: {0} / {1}", _cur, _total);
            }
        }

        private void UpdateCurrentProgress(float _progress, string _msg)
        {
            if (null != txtCurProgress)
            {
                txtCurProgress.text = string.Format("current: {0}", _progress.ToString("0.##"));
            }
        }

        #region test bundle load
        //private Sprite affectPartyAsync;

        private void SetTestSprite(Sprite _sp)
        {
            if (null != imgSprite)
            {
                imgSprite.gameObject.SetActive((_sp != null ? true : false));
                imgSprite.sprite = _sp;
            }
        }
        private IEnumerator TestLoadBundles()
        {
#if !UNITY_EDITOR
        yield break;
#else
            if (!PatchHandler.IsValid())
                yield break;

            string[] allBundleNames = PatchHandler.allBundleList;
            int count = allBundleNames.Length;
            if (count <= 0)
                yield break;

            //LoadAllBundles();   //모든 번들 로드
            yield return TestBundleReference.RunTasks();
#endif
        }
        #endregion test bundle load
    }
}