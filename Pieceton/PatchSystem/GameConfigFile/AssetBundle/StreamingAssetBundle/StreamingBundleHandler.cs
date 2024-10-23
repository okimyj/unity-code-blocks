using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Pieceton.PatchSystem
{
    public static partial class StreamingBundleHandler
    {
        public static System.Action<int, int> notifyStreamBundleCheckProgress;  //<_cur_index, _total_count>

        public const string STREAMING_TEMP = "/StreamingTemp/";
        private const string STREAMING_BUNDLE_FOLDER = "/InternalBundle/";
        private const string STREAMING_VIDEO_FOLDER = "/Videos/";

        private static PatchListInfo streamPatchListInfo = new PatchListInfo();

        public static string streamingBundlePath
        #region streamingBundlePath
        {
            get
            {
                if (string.IsNullOrEmpty(_streamingBundlePath))
                {
                    _streamingBundlePath = streamingBaseURL + STREAMING_BUNDLE_FOLDER;
                    Debug.LogFormat("[StreamingHandler] streamingBundlePath = {0}", _streamingBundlePath);
                }

                return _streamingBundlePath;
            }
        }
        private static string _streamingBundlePath = "";
        #endregion streamingBundlePath

        public static string streamingBundleMetaPath
        #region streamingBundleMetaPath
        {
            get
            {
                if (string.IsNullOrEmpty(_streamingBundleMetaPath))
                {
                    string folderName = STREAMING_BUNDLE_FOLDER.Replace("/", "");
                    _streamingBundleMetaPath = streamingBaseURL + "/" + folderName + ".meta";
                    Debug.LogFormat("[StreamingHandler] streamingBundleMetaPath = {0}", _streamingBundlePath);
                }

                return _streamingBundleMetaPath;
            }
        }
        private static string _streamingBundleMetaPath = "";
        #endregion streamingBundleMetaPath

        public static string streamingBundleURL
        #region streamingBundleURL
        {
            get
            {
                if (string.IsNullOrEmpty(_streamingBundleURL))
                {
                    _streamingBundleURL = streamingBaseURL + STREAMING_BUNDLE_FOLDER;
                    Debug.LogFormat("[StreamingHandler] streamingBundleURL = {0}", _streamingBundleURL);
                }

                return _streamingBundleURL;
            }
        }
        private static string _streamingBundleURL = "";
        #endregion streamingBundleURL

        public static string streamingVideoURL
        #region streamingVideoURL
        {
            get
            {
                if (string.IsNullOrEmpty(_streamingVideoURL))
                {
                    _streamingVideoURL = streamingBaseURL + STREAMING_VIDEO_FOLDER;
                    Debug.LogFormat("[StreamingHandler] streamingVideoURL = {0}", _streamingVideoURL);
                }

                return _streamingVideoURL;
            }
        }
        private static string _streamingVideoURL = "";
        #endregion streamingVideoURL

        public static string streamingBaseURL
        #region streamingBaseURL
        {
            get
            {
                if (string.IsNullOrEmpty(_streamingBaseURL))
                {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                    _streamingBaseURL = Application.streamingAssetsPath;
#elif UNITY_ANDROID
	                _streamingBaseURL = Application.streamingAssetsPath;
#elif UNITY_IOS
	                _streamingBaseURL = "file://" + Application.streamingAssetsPath;
#else
	                _streamingBaseURL = Application.streamingAssetsPath;
#endif
                    Debug.LogFormat("[StreamingHandler] streamingBaseURL = {0}", _streamingBaseURL);
                }
                return _streamingBaseURL;
            }
        }
        private static string _streamingBaseURL = "";
        #endregion streamingBaseURL

        public static IEnumerator Init()
        {
            string data = "";

            // #if !UNITY_EDITOR && UNITY_ANDROID
#if UNITY_ANDROID
            string streamListPath = Application.persistentDataPath + STREAMING_TEMP + DefPatchHandler.PATCH_LIST_FILENAME;
            data = File.ReadAllText(streamListPath);
#else
            string streamListPath = streamingBundlePath + DefPatchHandler.PATCH_LIST_FILENAME;
            using (StreamReader reader = new StreamReader(File.OpenRead(streamListPath)))
            {
                data = reader.ReadToEnd();
            }
#endif

            if (streamPatchListInfo.Load(data))
            {
                string[] allBundles = streamPatchListInfo.allBundleList;
                int _total_count = allBundles.Length;

                for (int i = 0; i < _total_count; ++i)
                {
                    string bundleName = allBundles[i];
                    if (IsExist(bundleName))
                    {
                        string bundlePath = PatchHandler.MakeHashBundleURL(streamingBundleURL, bundleName);
                        BundleStorageHandler.SetNoBackupFlag(bundlePath);
                    }

                    if (null != notifyStreamBundleCheckProgress)
                    {
                        notifyStreamBundleCheckProgress(i, _total_count);
                    }

                    yield return null;
                }
            }
        }

        public static bool Has(string _bundle_name, string _hash, uint _crc)
        {
            // #if !UNITY_EDITOR && UNITY_ANDROID
#if UNITY_ANDROID
            return false;
#else
            return streamPatchListInfo.Equal(_bundle_name, _hash, _crc);
#endif
        }

        private static bool IsExist(string _bundle_name)
        {
            string dstPath = PatchHandler.MakeHashBundlePath(streamingBundlePath, _bundle_name);

            bool isExist = false;

            try
            {
                isExist = File.Exists(dstPath);
            }
            catch (Exception) { }

            //Debug.LogFormat("exist streaming bundle. result='{0}', path='{1}'", isExist, dstPath);

            return isExist;
        }
    }
}