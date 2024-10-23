using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public class BundleStorageHandler
    {
        public const string BUNDLE_STORAGE_LIST_FILE_NAME = "bundleStorageList.txt";

        public static string bundleStoragePath
        #region bundleStoragePath
        {
            get
            {
                if (string.IsNullOrEmpty(_bundleStoragePath))
                {
                    _bundleStoragePath = bundleStorageRoot + "/";
                    Debug.LogFormat("[AssetBundleStorage] bundleStoragePath = {0}", _bundleStoragePath);
                }
                return _bundleStoragePath;
            }
        }
        private static string _bundleStoragePath = "";
        #endregion bundleStoragePath

        private static string bundleStorageRoot
        #region bundleStorageRoot
        {
            // 안드로이드 데이터 경로 얻어올때
            // 유니티 오류로
            // 엉뚱한 경로가 반환되는 경우가 있어서
            // 네이티브에서 얻어다 썼음
            get
            {
                if (string.IsNullOrEmpty(_bundleStorageRoot))
                {
#if !UNITY_EDITOR && UNITY_ANDROID
                    /*
                    string _bundleStorageRoot = NConnectAndroid.Instance.CallJavaStaticFunc("getPersistenceDataPath");
                    if (string.IsNullOrEmpty(_bundleStorageRoot))
                    {
                        _bundleStorageRoot = Application.persistentDataPath;

                        string msgFailGetPath = string.Format("fail getPersistenceDataPath. use to: {0}", _bundleStorageRoot);

                        Debug.LogErrorFormat(msgFailGetPath);
                        //PExceptionSender.Instance.SendLog(msgFailGetPath);
                        _bundleStorageRoot = Application.persistentDataPath;
                    }
                    /**/

                    if (string.IsNullOrEmpty(_bundleStorageRoot))
                    {
                        _bundleStorageRoot = Application.persistentDataPath;
                    }
#else
                    _bundleStorageRoot = Application.persistentDataPath;
#endif
                }

                return _bundleStorageRoot;
            }
        }
        private static string _bundleStorageRoot = "";
#endregion bundleStorageRoot
        
        private static BundleStorageInfoList storageList = new BundleStorageInfoList();

        public static bool Init()
        {
            return storageList.Init();
        }

        // 전체 번들 다시 다운로드 받는 함수
        // 호출 뒤 반드시 게임 재시작 시켜줘야 함
        public static void RedownloadAllBundles()
        {
            storageList.DeleteStorageFileList();
        }

        // 다운로드 받은 파일을 스토리지에 저장
        public static bool WriteBundle(WWWDownloadData_Bundle data)
        {
            if (!PatchHandler.IsValid())
                return true;

            string hashFolderName = PatchHandler.GetHashFolderName(data.fileName);
            string hashFolderPath = bundleStoragePath + hashFolderName;
            string dstPath = PatchHandler.MakeHashBundlePath(bundleStoragePath, data.fileName);

            if (string.IsNullOrEmpty(hashFolderName))
            {
                Debug.LogErrorFormat("WriteBundle. invalid hash string. bundle_path='{0}'", dstPath);
                return false;
            }

            try
            {
                if (File.Exists(dstPath))
                    File.Delete(dstPath);
            }
            catch { }

            //if (false == File.Exists(dstPath))
            {
                string msgWriteBundle = string.Format("WriteBundle. bundle_path='{0}'", dstPath);

                bool successWrite = false;

                try
                {
                    Debug.Log(msgWriteBundle);

                    if (false == Directory.Exists(hashFolderPath))
                    {
                        Directory.CreateDirectory(hashFolderPath);
                    }

                    using (FileStream fileStream = new FileStream(dstPath, FileMode.Create))
                    {
                        fileStream.Write(data.www.downloadHandler.data, 0, data.www.downloadHandler.data.Length);
                        fileStream.Dispose();
                        successWrite = true;
                    }
                }
                catch (Exception e)
                {
                    string msgWriteFileFail = string.Format("fail {0} {1}", msgWriteBundle, e.Message);
                    Debug.LogErrorFormat(msgWriteFileFail);
                    PExceptionSender.Instance.SendLog(msgWriteFileFail);
                }

                if (!successWrite)
                {
                    try
                    {
                        if (File.Exists(dstPath))
                            File.Delete(dstPath);
                    }
                    catch
                    {
                        Debug.LogErrorFormat("BundleStorageHandler::WriteBundle() delete wrong file error. {0}", dstPath);
                    }
                    return false;
                }
            }

            uint crc = PCRCChecker.GetCRC(File.ReadAllBytes(dstPath));
            if (!data.fileCRC.Equals(crc))
            {
                Debug.LogErrorFormat("BundleStorageHandler::WriteBundle() Invalid crc. force delete file = '{0}'", dstPath);

                try
                {
                    if (File.Exists(dstPath))
                        File.Delete(dstPath);
                }
                catch { }
                return false;
            }

            SetNoBackup(data.fileName);
            storageList.Add(data.fileName, dstPath, hashFolderPath, hashFolderName, data.hashCode.ToString(), data.fileCRC);

            return true;
        }

        // 패치리스트에 없는 파일들 스토리지에서 삭제
        public static IEnumerator DeleteUnusedBundles()
        {
            if (!PatchHandler.IsValid())
                yield break;

            Dictionary<string, BundleStorageInfo> existInTempCache = new Dictionary<string, BundleStorageInfo>();
            GetExistInTempCache(existInTempCache);
            if (existInTempCache.Count <= 0)
                yield break;

            AssetBundlePatchData patchData = null;
            Dictionary<string, AssetBundlePatchData>.Enumerator iter = PatchHandler.totalDic.GetEnumerator();
            while (iter.MoveNext())
            {
                patchData = iter.Current.Value;

                string hashFolderName = PatchHandler.MakeHashFolderName(patchData.hash, patchData.crc.ToString());

                if (existInTempCache.ContainsKey(hashFolderName))
                    existInTempCache.Remove(hashFolderName);

                //yield return null;
            }

            yield return DeleteUnusedBundleDirectory(existInTempCache);

            existInTempCache.Clear();
        }

        public static void DeleteBundle(string _bundle_name)
        {
            string path = PatchHandler.MakeHashBundlePath(bundleStoragePath, _bundle_name);

            try
            {
                Debug.LogFormat("BundleStorageHandler::DeleteBundle() {0}", path);
                File.Delete(path);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("BundleStorageHandler::DeleteBundle() {0}", e);
            }
        }

        // 스토리지의 폴더리스트 반환
        private static void GetExistInTempCache(Dictionary<string, BundleStorageInfo> _resultDic)
        {
            _resultDic.Clear();

            BundleStorageInfo info = null;
            Dictionary<string, BundleStorageInfo>.Enumerator iter = storageList.bundleDic.GetEnumerator();
            while (iter.MoveNext())
            {
                info = iter.Current.Value;

                _resultDic.Add(info.hashFolderName, info);
            }

            /*
            try
            {
                string[] hash_directories = Directory.GetDirectories(bundleStoragePath);

                int count = hash_directories.Length;
                if (count <= 0)
                    return;

                string hash = "";
                string hash_path = "";
                string hashDirectory = "";
                string[] split = null;

                for (int i = 0; i < count; ++i)
                {
                    hashDirectory = hash_directories[i];

                    if (Directory.Exists(hashDirectory))
                    {
                        split = hashDirectory.Split('/');
                        hash = split[split.Length - 1];

                        if (hash.Length < 32)
                            continue;

                        if (false == _resultDic.ContainsKey(hash))
                        {
                            _resultDic.Add(hash, hashDirectory);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("GetExistInTempCache fail. error='{0}'", e);
            }
            /**/
        }

        public static bool Has(string _bundle_name, string _hash, uint _crc)
        {
            if (PatchHandler.IsValid())
            {
                return ExistInStorage(_bundle_name, _hash, _crc);
            }

            return false;
        }

        private static bool ExistInStorage(string _bundle_name, string _hash, uint _crc)
        {
            return storageList.HasInStorage(_bundle_name, _hash, _crc);
            /*;
            string dstPath = PatchHandler.MakeHashBundlePath(bundleStoragePath, _bundle_name);

            bool isExist = false;

            try
            {
                isExist = File.Exists(dstPath);
            }
            catch (Exception e)
            {
                string msgExistBundleFail = string.Format("fail {0} {1}", dstPath, e.Message);
                Debug.LogError(msgExistBundleFail);
                PExceptionSender.Instance.SendLog(msgExistBundleFail);
            }

            //Debug.LogFormat("exist bundle. result='{0}', path='{1}'", isExist, dstPath);

            return isExist;
            /**/
        }

        private static IEnumerator DeleteUnusedBundleDirectory(Dictionary<string, BundleStorageInfo> _unuse_bundle_dic)
        {
            string removeFolderPath = "";
            string removeFile = "";

            Dictionary<string, BundleStorageInfo>.Enumerator removeIter = _unuse_bundle_dic.GetEnumerator();

            BundleStorageInfo info = null;
            while (removeIter.MoveNext())
            {
                info = removeIter.Current.Value;
                removeFolderPath = info.bundleFolderPath;

                try
                {
                    if (Directory.Exists(removeFolderPath))
                    {
                        string[] files = Directory.GetFiles(removeFolderPath);
                        if (files.Length > 0)
                        {
                            for (int i = 0; i < files.Length; ++i)
                            {
                                removeFile = files[i].Replace("\\", "/");

                                if (File.Exists(removeFile))
                                {
                                    string msgRemoveFile = string.Format("remove no use bundle: {0}", removeFile);
                                    Debug.LogFormat(msgRemoveFile);

                                    File.Delete(removeFile);
                                }
                            }
                        }

                        string msgRemovePath = string.Format("DeleteBundle. path='{0}'", removeFolderPath);

                        Debug.Log(msgRemovePath);
                        Directory.Delete(removeFolderPath);
                    }

                    storageList.Remove(info.bundleName);
                }
                catch (Exception e)
                {
                    string msgRemoveFileFail = string.Format("DeleteUnusedBundles fail. error='{0}'", e);
                    Debug.LogErrorFormat(msgRemoveFileFail);
                    PExceptionSender.Instance.SendLog(msgRemoveFileFail);
                }

                yield return null;
            }
        }

        private static void SetNoBackup(string bundleName)
        {
#if UNITY_IPHONE
        if (PatchHandler.IsValid())
        {
            string hash = PatchHandler.GetHashFolderName(bundleName);
            string hashDir = bundleStoragePath + hash;
            SetNoBackupFlag(hashDir);

            // 꼬물딱지 Caching.SetNoBackupFlag() 먹통
            //Caching.SetNoBackupFlag(_www.url, _hash);
        }
#endif
        }

        public static void SetNoBackupFlag(string _storage_path)
        {
#if UNITY_IPHONE
        if (!string.IsNullOrEmpty(_storage_path))
        {
            UnityEngine.iOS.Device.SetNoBackupFlag(_storage_path);
            Debug.LogFormat("SetNoBackupFlag='{0}'", _storage_path);
        }
#endif
        }
    }
}