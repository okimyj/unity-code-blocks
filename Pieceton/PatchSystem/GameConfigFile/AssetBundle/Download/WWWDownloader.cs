using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public class WWWDownloader : PSingletonComponent<WWWDownloader>
    {
        protected override void AwakeInstance() { }
        protected override bool InitInstance() { return true; }
        protected override void ReleaseInstance() { }

        private AssetBundleFlag _assetBundleFlag = AssetBundleFlag.None;
        private void AddFlag(AssetBundleFlag _flag) { _assetBundleFlag |= _flag; }
        private void RemoveFlag(AssetBundleFlag _flag) { _assetBundleFlag &= ~_flag; }
        public bool HasFlag(AssetBundleFlag _flag) { return (0 != (_assetBundleFlag & _flag)); }
        public bool IsProcessing() { return (_assetBundleFlag != AssetBundleFlag.None); }

        private readonly Dictionary<string, WWWDownloadData> _downloadingDic = new Dictionary<string, WWWDownloadData>();
        private readonly Dictionary<string, WWWDownloadData> _downloadedDic = new Dictionary<string, WWWDownloadData>();

        private readonly Dictionary<string, string> _downloadingErrors = new Dictionary<string, string>();

        private readonly List<string> _keysToRemoveList = new List<string>();


        public float curDownloadProgress
        {
            get
            {
                if (HasFlag(AssetBundleFlag.Downloading))
                {
                    return _curDownloadProgress;
                }

                return 1.0f;
            }
        }
        private float _curDownloadProgress = 0.0f;

        public void DownloadFile(string _file_name, bool _use_time_stamp)
        {
            string basePath = ReleaseInfo.curVersionDownloadURL;
            if (ReleaseInfo.useStreamPatch)
            {
#if UNITY_EDITOR
                basePath = string.Format("file:///{0}", StreamingBundleHandler.streamingBundlePath);
#else
                basePath = StreamingBundleHandler.streamingBundlePath;
#endif
            }

            string url = basePath + _file_name;
            if (_use_time_stamp)
            {
                url = string.Format("{0}?{1}", url, DateTime.Now.ToString("yyyyMMddhhmmss"));
            }

            AddDownloadData(DownloadDataType.File, url, _file_name);
        }

        public void DownloadAssetBundle(string _bundle_name)
        {
            string basePath = ReleaseInfo.curVersionDownloadURL;
            if (ReleaseInfo.useStreamPatch)
            {
#if UNITY_EDITOR
                basePath = string.Format("file:///{0}", StreamingBundleHandler.streamingBundlePath);
#else
                basePath = StreamingBundleHandler.streamingBundlePath;
#endif
            }

            string _url = PatchHandler.MakeHashBundlePath(basePath, _bundle_name);
            AddDownloadData(DownloadDataType.Bundle, _url, _bundle_name);
        }

        void Update()
        {
            if (_assetBundleFlag == AssetBundleFlag.None)
                return;

            if (HasFlag(AssetBundleFlag.FailBundleWrite))
                return;

            if (HasFlag(AssetBundleFlag.AskRetry))
                return;

            if (HasFlag(AssetBundleFlag.AskRetry_CRC))
                return;

            if (HasFlag(AssetBundleFlag.Downloading))
                DownloadProccess();
        }

        public bool IsDownloading(string _bundle_name)
        {
            return _downloadingDic.ContainsKey(_bundle_name);
        }

        public WWWDownloadData_File GetDownloadedFileData(string _file_name)
        {
            WWWDownloadData data;
            if (_downloadedDic.TryGetValue(_file_name, out data))
            {
                if (data.dataType == DownloadDataType.File)
                    return (WWWDownloadData_File)data;
            }

            return null;
        }

        public void RemoveDownloadedFileData(string _file_name)
        {
            WWWDownloadData data;
            if (_downloadedDic.TryGetValue(_file_name, out data))
            {
                data.Release();
                _downloadedDic.Remove(_file_name);
            }

            if (_downloadingDic.TryGetValue(_file_name, out data))
            {
                data.Release();
                _downloadingDic.Remove(_file_name);
            }
        }

        private void DownloadProccess()
        {
            float deltaTime = Time.unscaledDeltaTime;

            int totalCount = _downloadingDic.Count;
            if (totalCount <= 0)
                return;

            WWWDownloadData firstData = null;
            float _total_progress = 0.0f;

            Dictionary<string, WWWDownloadData>.Enumerator iter = _downloadingDic.GetEnumerator();
            while (iter.MoveNext())
            {
                WWWDownloadData data = iter.Current.Value;
                data.Update(deltaTime);

                if (null == firstData)
                {
                    firstData = data;
                }

                _total_progress += data.lastProgress;

                if (data.www.isDone && null == data.www.error)
                {
                    if (!data.IsValid())
                    {
                        if (data.dataType == DownloadDataType.Bundle)
                        {
                            string msgLog = string.Format("CRC Error URL={0}", data.url);
                            PExceptionSender.Instance.SendLog(msgLog);

                            if (data.Retry_CRC_Error() != WWWDownloadError.CRC_Error_But_Write)
                                continue;
                        }
                        else
                        {
                            data.Retry();
                            continue;
                        }
                    }

                    DownloadFinishProccess(data);
                }
            }

            if (null != firstData)
            {
                if (!firstData.isCached)
                {
                    WWWDownloadMgr.Instance.OnDownloadProgress(firstData);
                }

                firstData = null;
            }

            _curDownloadProgress = _total_progress / (float)totalCount;

            RemoveDownloadFinishProccess();

            DownloadErrorProccess();

            if (_downloadingDic.Count <= 0)
            {
                RemoveFlag(AssetBundleFlag.Downloading);
            }
        }

#region download core functions
        private void AddDownloadData(DownloadDataType _data_type, string _url, string _bundle_name)
        {
            if (_data_type == DownloadDataType.Bundle)
            {
                if (!PatchHandler.IsValid())
                {
                    if (Debug.isDebugBuild)
                        Debug.LogError("not yet Assetbundle initialize");
                    return;
                }
            }

            if (_downloadingDic.ContainsKey(_bundle_name))
            {
                Debug.LogErrorFormat("already downloading. {0}", _bundle_name);
                return;
            }

            WWWDownloadData data = CreateDownloadData(_data_type, _url, _bundle_name);
            if (null == data)
            {
                Debug.LogErrorFormat("WWWDownloadData is null");
                return;
            }

            Debug.LogFormat("[Download] {0}", _url);

            _downloadingDic.Add(_bundle_name, data);
            AddFlag(AssetBundleFlag.Downloading);
        }

        private WWWDownloadData CreateDownloadData(DownloadDataType _data_type, string _url, string _bundle_name)
        {
            switch (_data_type)
            {
                case DownloadDataType.File: return new WWWDownloadData_File(_url, _bundle_name);
                case DownloadDataType.Bundle: return new WWWDownloadData_Bundle(_url, _bundle_name);
            }

            return null;
        }

        private void DownloadFinishProccess(WWWDownloadData _data)
        {
            if (null == _data)
            {
                Debug.LogError("[WWWDownloader] DownloadFinishProcess() WWWDownloadData is null.");
                return;
            }

            Debug.LogFormat("[WWWDownloader] DownloadFinishProcess() {0}", _data.fileName);

            if (_data.dataType == DownloadDataType.Bundle)
            {
                if (false == BundleStorageHandler.WriteBundle((WWWDownloadData_Bundle)_data))
                {
                    AddFlag(AssetBundleFlag.FailBundleWrite);
                    string msg = "An error occurred while installing the patch.\nPlease restart the game and try again (write)";
                    WWWDownloadMgr.Instance.OnDownloadErrToGameRestart(FailDownload.FailedWrite, msg);
                    return;
                }
            }
            else
            {
                if (!_downloadedDic.ContainsKey(_data.fileName))
                {
                    _downloadedDic.Add(_data.fileName, _data);
                }
            }

            if (!_keysToRemoveList.Contains(_data.fileName))
            {
                _keysToRemoveList.Add(_data.fileName);
            }

            if (_downloadingErrors.ContainsKey(_data.fileName))
            {
                _downloadingErrors.Remove(_data.fileName);
            }
        }

        private void RemoveDownloadFinishProccess()
        {
            int removeCount = _keysToRemoveList.Count;
            if (removeCount <= 0)
                return;

            for (int i = 0; i < removeCount; ++i)
            {
                string fileName = _keysToRemoveList[i];

                WWWDownloadData data;
                if (_downloadingDic.TryGetValue(fileName, out data))
                {
                    _downloadingDic.Remove(fileName);
                    //data.Release();//www dispose
                }
            }

            _keysToRemoveList.Clear();
        }

        private FailDownload CollectDownloadError(out string _first_err_msg)
        {
            FailDownload failDownload = FailDownload.None;

            _first_err_msg = "";

            Dictionary<string, WWWDownloadData>.Enumerator iter = _downloadingDic.GetEnumerator();
            while (iter.MoveNext())
            {
                string errorMsg = "";

                WWWDownloadData data = iter.Current.Value;

                CheckError(data, ref failDownload, ref errorMsg);

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    data.AddError(errorMsg);
                }

                if (string.IsNullOrEmpty(_first_err_msg))
                {
                    _first_err_msg = errorMsg;
                }
            }

            return failDownload;
        }

        private void CheckError(WWWDownloadData data, ref FailDownload failDownload, ref string errorMsg)
        {
            errorMsg = "";

            if (data.IsTimeout())
            {
                failDownload = FailDownload.TimeOut;
                errorMsg = "timeout";
            }
            else if (null != data.www.error)
            {
                failDownload = FailDownload.WWWError;
#if (RELEASE_REAL || RELEASE_REAL_QA)
            errorMsg = "network error";
#else
                errorMsg = "network error: " + data.www.error;
#endif
            }
            else if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                failDownload = FailDownload.NotReachable;
                errorMsg = "notReachable";
            }
            else if (data.error != WWWDownloadError.None && data.error != WWWDownloadError.AutoCRC_Retry)
            {
                failDownload = FailDownload.CRC;

                switch (data.error)
                {
                    case WWWDownloadError.CRC_Error:
                        errorMsg = "Download File Error(CRC)";
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty(errorMsg))
            {
                Debug.LogErrorFormat("[WWWDownloader] {0}\n{1}", errorMsg, data.url);
            }
        }

        private void DownloadErrorProccess()
        {
            // 다운로드 오류 항목들 수집
            string firstErrorMsg = "";
            FailDownload failDownload = CollectDownloadError(out firstErrorMsg);

            // 파일 하나라도 전송 성공하지 못하면 남은 다운로드 항목들 모두 다시 받는다.
            switch (failDownload)
            {
                case FailDownload.None: break;
                case FailDownload.CRC:
                    {
                        AddFlag(AssetBundleFlag.AskRetry_CRC);
                        WWWDownloadMgr.Instance.OnDownloadErr(OnMsgBox_Retry_CRC, failDownload, firstErrorMsg);
                        break;
                    }
                default:
                case FailDownload.TimeOut:
                case FailDownload.WWWError:
                case FailDownload.NotReachable:
                    {
                        AddFlag(AssetBundleFlag.AskRetry);
                        WWWDownloadMgr.Instance.OnDownloadErr(OnMsgBox_Retry, failDownload, firstErrorMsg);
                        break;
                    }
            }
        }


        private IEnumerator RetryDownload()
        {
            yield return new WaitForSeconds(3.0f);

            Dictionary<string, WWWDownloadData>.Enumerator iter = _downloadingDic.GetEnumerator();
            while (iter.MoveNext())
            {
                WWWDownloadData data = iter.Current.Value;

                if (_downloadingErrors.ContainsKey(iter.Current.Key))
                {
                    _downloadingErrors.Remove(iter.Current.Key);
                }

                data.Retry();
            }

            yield return new WaitForSeconds(2.0f);

            RemoveFlag(AssetBundleFlag.AskRetry);
        }

        private IEnumerator RetryDownload_CRC(bool isOk)
        {
            yield return new WaitForSeconds(3.0f);

            Dictionary<string, WWWDownloadData>.Enumerator iter = _downloadingDic.GetEnumerator();
            while (iter.MoveNext())
            {
                WWWDownloadData data = iter.Current.Value;

                if (isOk)
                    data.Retry();
                else
                    data.Retry_CRC_Error_But_Write();
            }

            yield return new WaitForSeconds(2.0f);

            RemoveFlag(AssetBundleFlag.AskRetry_CRC);
        }
#endregion download core functions


        private void OnMsgBox_Retry(bool isOk)
        {
            StartCoroutine(RetryDownload());
        }

        private void OnMsgBox_Retry_CRC(bool isOk)
        {
            StartCoroutine(RetryDownload_CRC(isOk));
        }
    }
}