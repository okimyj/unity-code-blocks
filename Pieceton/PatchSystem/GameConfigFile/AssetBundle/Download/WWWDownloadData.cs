using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Pieceton.Misc;
using UnityEngine.Networking;

namespace Pieceton.PatchSystem
{
    public class WWWDownloadData
    {
        public UnityWebRequest www { get; protected set; }

        public DownloadDataType dataType { get; private set; }

        public string url { get; private set; }
        public string fileName { get; private set; }

        public int fileSize { get; protected set; }
        public uint fileCRC { get; protected set; }
        public Hash128 hashCode { get; protected set; }
        public bool isCached { get; protected set; }

        public float lastProgress { get; private set; }
        public float delayTime { get; private set; }

        public List<string> ErrorList { get { return errorList; } }
        private List<string> errorList = null;

        public WWWDownloadError error { get; protected set; }

#if false//DEVENV_DEV || DEVENV_QA || RELEASE_DEV_INQA
        public const float DOWNLOAD_TIMEOUT = 3.0f;
#else
        public const float DOWNLOAD_TIMEOUT = 20.0f;
#endif

        public WWWDownloadData(DownloadDataType _data_type, string _url, string _file_name)
        {
            dataType = _data_type;
            url = _url;

            fileName = _file_name;
            fileSize = 0;
            fileCRC = 0;
            hashCode = new Hash128(0, 0, 0, 0);
            isCached = false;

            lastProgress = 0;
            delayTime = 0.0f;
            error = WWWDownloadError.None;
        }

        public void Release()
        {
            if (null != www)
            {
                if (www.isDone)
                    www.Dispose();
                www = null;
            }
        }

        ~WWWDownloadData()
        {
            if (null != www)
                www = null;
        }

        public virtual void Retry()
        {
            //try
            //{
            //    www.Dispose();
            //}
            //catch (Exception e)
            //{
            //    if (Debug.isDebugBuild)
            //        Debug.LogErrorFormat("WWWDownloadData_File retry dispose error [{0}] error='{1}'", url, e);
            //}

            lastProgress = 0;
            delayTime = 0.0f;
            error = WWWDownloadError.None;
        }

        public virtual WWWDownloadError Retry_CRC_Error() { return WWWDownloadError.None; }

        public virtual void Retry_CRC_Error_But_Write() { }

        public void Update(float deltaTime)
        {
            float curProgress = www.downloadProgress;
            if (lastProgress != curProgress)
            {
                lastProgress = curProgress;
                delayTime = 0.0f;
            }
            else
            {
                delayTime += deltaTime;
            }
        }

        public virtual bool IsValid()
        {
            return true;
        }

        public bool IsTimeout()
        {
            if (delayTime >= DOWNLOAD_TIMEOUT)
                return true;

            return false;
        }

        public void AddError(string errorMsg)
        {
            if (!string.IsNullOrEmpty(errorMsg))
            {
                if (null == errorList)
                {
                    errorList = new List<string>();
                }
                errorList.Add(errorMsg);
            }
        }
    }


    public class WWWDownloadData_File : WWWDownloadData
    {
        public WWWDownloadData_File(string _url, string _file_name)
            : base(DownloadDataType.File, _url, _file_name)
        {
            www = UnityWebRequest.Get(_url);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SendWebRequest();
        }

        public override void Retry()
        {
            base.Retry();

            if (null != www)
            {
                Debug.LogFormat("[begin dispose] {0}", url);

                if (www.isDone)
                    www.Dispose();

                Debug.LogFormat("[end dispose] {0}", url);
                www = null;
            }
            www = UnityWebRequest.Get(url);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SendWebRequest();
        }
    }



    public class WWWDownloadData_Bundle : WWWDownloadData
    {
        public WWWDownloadData_Bundle(string _url, string _bundle_name)
            : base(DownloadDataType.Bundle, _url, _bundle_name)
        {
            string hash = PatchHandler.GetBundleHashString(_bundle_name);
            uint crc    = PatchHandler.GetBundleCRC(_bundle_name);

            bool hasStorage = BundleStorageHandler.Has(_bundle_name, hash, crc);
            bool hasStream = StreamingBundleHandler.Has(_bundle_name, hash, crc);

            this.isCached = hasStorage || hasStream;
            this.hashCode = Hash128.Parse(PatchHandler.GetBundleHashString(_bundle_name));
            this.fileSize = PatchHandler.GetBundleSize(_bundle_name);
            this.fileCRC = crc;

            Request();
        }

        public override bool IsValid()
        {
#if DONT_CRC_CHECK
        return true;
#else
            return IsValid_BundleCRC();
#endif
        }

        public override void Retry()
        {
            base.Retry();

            if (null != www)
            {
                // 재시도 루틴에 들어온 후
                // 네트워크 상태 정상복구되어 이미 번들이 받아졌다면
                // 언로드 시키고 다시 받는다.
                //if (www.isDone)
                //{
                //    if (null == www.error && null != www.assetBundle)
                //    {
                //        Debug.LogWarningFormat("loaded {0}", url);
                //        www.assetBundle.Unload(false);
                //    }

                //    Debug.LogFormat("[begin dispose] {0}", url);
                //    www.Dispose();
                //    Debug.LogFormat("[end dispose] {0}", url);
                //}

                www = null;
            }
            Request();
        }

        public override WWWDownloadError Retry_CRC_Error()
        {
            switch (error)
            {
                case WWWDownloadError.None:
                    {
                        Retry();
                        error = WWWDownloadError.AutoCRC_Retry;
                    }
                    break;

                case WWWDownloadError.AutoCRC_Retry:
                    {
                        error = WWWDownloadError.CRC_Error;
                    }
                    break;
            }

            return error;
        }

        public override void Retry_CRC_Error_But_Write()
        {
            if (error == WWWDownloadError.CRC_Error)
                error = WWWDownloadError.CRC_Error_But_Write;
        }

        private void Request()
        {
            www = UnityWebRequest.Get(url);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SendWebRequest();

            //if (www !=null&& www.isDone ==true &&www.error==null && www.assetBundle !=null)  // 바로 로딩이 되지 않는다.file도  asyncload 하는듯.
            //{
            //    if (Debug.isDebugBuild)
            //         Debug.Log("caching right now :" + url);
            //}
        }

        private bool IsValid_BundleCRC()
        {
            // 다운로드가 완료된 상황에서만 crc 체크가능.
            if (null != www)
            {
                if (www.isDone)
                {
                    uint uiCRC = PCRCChecker.GetCRC(www.downloadHandler.data);

                    bool valid = this.fileCRC == uiCRC;

                    if (!valid)
                    {
                        Debug.LogErrorFormat("{0} : patch_list({1}), WWW({2})", this.fileName, this.fileCRC, uiCRC);
                    }

                    return valid;
                }
            }

            Debug.LogErrorFormat("DOWNLOAD Failed : {0}", this.fileName);
            return false;
        }
    }
}