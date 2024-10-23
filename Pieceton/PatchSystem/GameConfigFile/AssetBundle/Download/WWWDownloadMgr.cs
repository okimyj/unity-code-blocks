using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public class WWWDownloadMgr : PSingletonComponent<WWWDownloadMgr>
    {
        protected override void AwakeInstance() { }
        protected override bool InitInstance() { return true; }

        protected override void ReleaseInstance()
        {
            notifyDownloadErrorGameRestart = null;
            notifyDownloadErrorRetry = null;

            notifyConfigErrorContinue = null;

            notifyDownloadFileName = null;
            notifyTotalProgress = null;
            notifyCurrentProgress = null;
        }


        private const bool useLog = true;

        public static System.Action notifyGameConfig_InvalidVersion;
        public static System.Action notifyGameConfig_Error;

        public static System.Action<FailDownload, string> notifyDownloadErrorGameRestart;                    // 다운로드 오류. 게임재시작 해야됨
        public static System.Action<System.Action<bool>, FailDownload, string> notifyDownloadErrorRetry;     // 다운로드 오류. 재시도 가능

        public static System.Action<System.Action<bool>, string> notifyConfigErrorContinue;     // 서버정보 확인중 오류가 발생했습니다. (확인) => 재시도

        public static System.Action<string> notifyDownloadFileName;
        public static System.Action<int, int> notifyTotalProgress;
        public static System.Action<float, string> notifyCurrentProgress;

        public static int downloadFileTotalCount { get; private set; }
        public static int downloadFileTotalSize { get; private set; }
        public static string formattedDownloadSize
        {
            get
            {
                float _down_size = (float)downloadFileTotalSize / (1024.0f * 1024.0f);
                return string.Format("{0}MB", _down_size.ToString("F"));
            }
        }

        private bool _waitRetryConfigParsing = true;
        private bool _isPauseProcess_ForContinueDownload = false;

        private string _download_text_temp = "";

        // 파일, 번들 다운로드중 오류발생 했다 복구가능한 상태가 아니므로
        // 적당한 메세지 박스와 함께 게임을 재시작 해주어야 한다
        public void OnDownloadErrToGameRestart(FailDownload _fail_download, string _err_msg)
        {
            LogError("[WWWDownloadMgr] Error = {0}, msg = {1}", _fail_download, _err_msg);

            if (null != notifyDownloadErrorGameRestart)
            {
                notifyDownloadErrorGameRestart(_fail_download, _err_msg);
            }
        }

        // 파일, 번들 다운로드중 오류발생 했다 다시 시도하겠냐는 메세지와 확인 버튼 출력
        // 확인버튼 클릭시 _call_back() 호출 해줄것
        public void OnDownloadErr(System.Action<bool> _call_back, FailDownload _fail_download, string _err_msg)
        {
            LogError("[WWWDownloadMgr] Error = {0}, msg = {1}", _fail_download, _err_msg);

            // 메세지 박스 등록되어 있지 않으면 재시도
            if (null == notifyDownloadErrorRetry)
            {
                _call_back(true);
                return;
            }

            notifyDownloadErrorRetry(_call_back, _fail_download, _err_msg);
        }

        private string ReadStreamingAssetFile(string _file_name)
        {
            string _full_path = StreamingBundleHandler.streamingBundlePath + _file_name;
            string msgLoadBundle = string.Format("load streaming file. path='{0}'", _full_path);
            try
            {
                string desc = "";
                using (StreamReader reader = new StreamReader(File.OpenRead(_full_path)))
                {
                    desc = reader.ReadToEnd();
                    reader.Dispose();
                }
                return desc;
            }
            catch (Exception e)
            {
                string msgLoadBundleFail = string.Format("fail {0}", msgLoadBundle);
                Debug.LogError(msgLoadBundleFail);
                PExceptionSender.Instance.SendLog(msgLoadBundleFail);
            }

            return "";
        }

#region download patch list
        public IEnumerator Init_PatchList()
        {
            yield return DownloadFile(DefPatchHandler.PATCH_LIST_FILENAME);

            PatchHandler.LoadPatchData(_download_text_temp);

            if (!PatchHandler.IsValid())
            {
                Debug.LogErrorFormat("[PatchProcessor] invalid patch list. require restart game.");
                OnDownloadErrToGameRestart(FailDownload.InvalidPatchList, "invalid_patch_list");

                while (true)
                    yield return null;
            }

            BundleStorageHandler.Init();

            Debug.LogFormat("[PatchProcessor] initialize finish.");
        }
        private bool FindAssetBundleDownloadInfo()
        {
            downloadFileTotalCount = 0;
            downloadFileTotalSize = 0;

            if (null != PatchHandler.totalDic)
            {
                string bundleName = "";

                AssetBundlePatchData data = null;
                Dictionary<string, AssetBundlePatchData>.Enumerator iter = PatchHandler.totalDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    bundleName = iter.Current.Key;
                    data = iter.Current.Value;
                    
                    if (!BundleStorageHandler.Has(bundleName, data.hash, data.crc))
                    {
                        if (!StreamingBundleHandler.Has(bundleName, data.hash, data.crc))
                        {
                            downloadFileTotalSize += data.size;
                            ++downloadFileTotalCount;
                        }
                    }
                }
            }

            if (downloadFileTotalCount > 0)
                return true;

            return false;
        }
#endregion download patch list

#region download config file
        public IEnumerator Init_Config()
        {
            bool configParsingProcessing = true;
            while (configParsingProcessing)
            {
                yield return DownloadFile(DefGameConfig.CONFIG_FILENAME);

                _waitRetryConfigParsing = true;

                bool _reDownloadAndParsing;
                PatchIssue _issue_type;
                string _error_title, _error_desc, _error;
                if (!GameConfigFile.Parse(_download_text_temp, out _issue_type, out _reDownloadAndParsing, out _error))
                {
                    if (_issue_type == PatchIssue.GameVersion)
                    {
                        // Config.json에 설정한 게임버전과 다르다
                        // 패키지 업데이트 받는곳으로 리다이렉트 시켜줘야한다.

                        Debug.LogErrorFormat("[PatchProcessor] {0}", PatchIssue.GameVersion);

                        if (null != notifyGameConfig_InvalidVersion)
                        {
                            notifyGameConfig_InvalidVersion();
                        }

                        while (true)
                            yield return null;
                    }

                    if (!_reDownloadAndParsing)
                    {
                        // Config.json 파싱 오류외 다른 문제가 있다.
                        // 게임 재시작 시켜줘야 한다.

                        Debug.LogErrorFormat("[PatchProcessor] {0}", PatchIssue.InvaidData);

                        if (null != notifyGameConfig_Error)
                        {
                            notifyGameConfig_Error();
                        }

                        while (true)
                            yield return null;
                    }

                    if (null != notifyConfigErrorContinue)
                    {
                        notifyConfigErrorContinue(OnNotify_ConfigParsing, _error);
                    }
                    else
                    {
                        OnNotify_ConfigParsing();
                    }

                    while (_waitRetryConfigParsing)
                        yield return null;
                }
                else
                {
                    configParsingProcessing = false;
                }

                // 정식 서비스 인지 소프트 런칭인지 설정값
                //GameServer.serviceType = _service_type;

                yield return null;
            }
        }
        private void OnNotify_ConfigParsing(bool isOK = true)
        {
            _waitRetryConfigParsing = false;
        }
#endregion download config file

#region download bundles
        public IEnumerator AskBundleDownloadMsgBox(System.Action<System.Action<bool>, float> _funcAskDownload)
        {
            if (FindAssetBundleDownloadInfo())
            {
                _isPauseProcess_ForContinueDownload = true;

                float _down_size = (float)downloadFileTotalSize / (1024.0f * 1024.0f);

                Debug.LogFormat("[PatchProcessor] asset bundle download continue. size = {0}MB", _down_size.ToString("F"));

                if (null != _funcAskDownload)
                {
                    _funcAskDownload(MsgBoxEvent_ContinueDownload, _down_size);
                }
                else
                {
                    MsgBoxEvent_ContinueDownload();
                }

                while (_isPauseProcess_ForContinueDownload)
                {
                    yield return null;
                }
            }
        }
        private void MsgBoxEvent_ContinueDownload(bool _is_ok = true)
        {
            _isPauseProcess_ForContinueDownload = false;
        }

        public IEnumerator DownloadBundles()
        {
            if (!PatchHandler.IsValid())
                yield break;

            int _cur_download_count = 0;
            int _cur_Index = 0;

            int totalBundleCount = PatchHandler.allBundleList.Length;
            float _once_progress = 1.0f;
            _once_progress /= (float)totalBundleCount;

            UpdateTotalProgress(0, totalBundleCount);

            if (downloadFileTotalCount > 0)
            {
                Dictionary<string, AssetBundlePatchData>.Enumerator _iter_bundle = PatchHandler.totalDic.GetEnumerator();
                while (_iter_bundle.MoveNext())
                {
                    string bundleName = _iter_bundle.Current.Key;
                    
                    string hash = PatchHandler.GetBundleHashString(bundleName);
                    uint crc    = PatchHandler.GetBundleCRC(bundleName);

                    bool hasStorage = BundleStorageHandler.Has(bundleName, hash, crc);
                    bool hasStream = StreamingBundleHandler.Has(bundleName, hash, crc);

                    bool needDownload = !(hasStorage || hasStream);

                    if (needDownload)
                        ++_cur_download_count;

                    float _start_progress = (float)(_cur_Index) / (float)totalBundleCount;

                    UpdateCurrentDownload(bundleName);

                    if (needDownload)
                    {
                        WWWDownloader.Instance.DownloadAssetBundle(bundleName);

                        while (WWWDownloader.Instance.IsProcessing())
                        {
                            float _cur_progress = _start_progress + (_once_progress * WWWDownloader.Instance.curDownloadProgress);
                            yield return null;
                        }
                    }

                    ++_cur_Index;

                    UpdateTotalProgress(_cur_Index, totalBundleCount);
                }
            }

            UpdateTotalProgress(totalBundleCount, totalBundleCount);
        }
#endregion download bundles

        private IEnumerator DownloadFile(string _file_name)
        {
            float lastProgress = 0.0f;

#if !UNITY_EDITOR && UNITY_ANDROID
            bool useWWW = true;
#else
            bool useWWW = !ReleaseInfo.useStreamPatch;
#endif
            if (useWWW)
            {
                WWWDownloader.Instance.DownloadFile(_file_name, !ReleaseInfo.useStreamPatch);
                while (WWWDownloader.Instance.IsProcessing())
                {
                    if (lastProgress != WWWDownloader.Instance.curDownloadProgress)
                    {
                        lastProgress = WWWDownloader.Instance.curDownloadProgress;
                        UpdateCurrentProgress(lastProgress, _file_name);
                    }
                    yield return null;
                }

                WWWDownloadData_File data = WWWDownloader.Instance.GetDownloadedFileData(_file_name);
                if (null == data)
                {
                    string msg = string.Format("failed {0} download.", _file_name);
                    PExceptionSender.Instance.SendLog(msg);

                    OnDownloadErrToGameRestart(FailDownload.UnknownError, msg);
                    while (true)
                        yield return null;
                }

                string tmpRoot = Application.persistentDataPath + StreamingBundleHandler.STREAMING_TEMP;
                Directory.CreateDirectory(tmpRoot);
                string tmpPath = tmpRoot + _file_name;

                System.IO.File.WriteAllBytes(tmpPath, data.www.downloadHandler.data);
                _download_text_temp = File.ReadAllText(tmpPath);

                //_download_text_temp = data.www.downloadHandler.text;
                WWWDownloader.Instance.RemoveDownloadedFileData(data.fileName);
            }
            else
            {
                _download_text_temp = ReadStreamingAssetFile(_file_name);
            }

            UpdateCurrentProgress(1.0f, _file_name);
        }

        private void UpdateCurrentDownload(string _file_name)
        {
            Log("[WWWDownloadMgr] file name. {0}", _file_name);

            if (null != notifyDownloadFileName)
            {
                notifyDownloadFileName(_file_name);
            }
        }

        private void UpdateTotalProgress(int _cur, int _total)
        {
            Log("[WWWDownloadMgr] total progress {0} / {1}", _cur, _total);

            if (null != notifyTotalProgress)
            {
                notifyTotalProgress(_cur, _total);
            }
        }

        private void UpdateCurrentProgress(float _cur, string _desc)
        {
            Log("[WWWDownloadMgr] current progress. {0}, ", _desc);

            if (null != notifyCurrentProgress)
            {
                notifyCurrentProgress(_cur, _desc);
            }
        }

        public void OnDownloadProgress(WWWDownloadData _data)
        {
            if (null == _data)
                return;

            string packName = PBundlePath.SplitPackName(_data.fileName);
            string downloadMsg = GetDownloadMsg(packName, _data.fileSize, _data.lastProgress);
            string msg = string.Format("{0}", downloadMsg);

            UpdateCurrentProgress(_data.lastProgress, msg);
        }


        private static string GetDownloadPercentString(float downloadProgress)
        {
            float fPercent = downloadProgress * 100.0f;
            string sPercent = fPercent.ToString("0.0");
            return sPercent;
        }

        private static string GetDownloadMsg(string downloadFileName, int fileSize, float downloadProgress)
        {
            string downloadPercent = GetDownloadPercentString(downloadProgress);
            int downloadSize = (int)((double)fileSize * (double)downloadProgress);
            string downloadFileSize = downloadSize.ToString();
            string msg = "";

#if (RELEASE_REAL || RELEASE_REAL_QA)
        msg = string.Format("({0}%)", downloadPercent);
#else
            msg = string.Format("{0} ({1}%)", downloadFileName, downloadPercent);
#endif
            if (fileSize > 0)
            {
                msg += string.Format("{0} [{1}/{2}]", msg, downloadFileSize, fileSize);
            }

            return msg;
        }



#region log
        private void Log(string _format, params object[] _arg)
        {
            if (!useLog)
                return;

            Debug.LogFormat(_format, _arg);
        }

        private void LogWarning(string _format, params object[] _arg)
        {
            if (!useLog)
                return;

            Debug.LogWarningFormat(_format, _arg);
        }

        private void LogError(string _format, params object[] _arg)
        {
            if (!useLog)
                return;

            Debug.LogErrorFormat(_format, _arg);
        }
#endregion log
    }
}