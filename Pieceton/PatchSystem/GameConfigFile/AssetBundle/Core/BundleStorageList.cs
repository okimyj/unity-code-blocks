using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using SimpleJSON;
using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public class BundleStorageInfo
    {
        public string bundleName;
        public string bundlePath;
        public string bundleFolderPath;
        public string hashFolderName;
        public string hash;
        public uint crc;

        public bool Equal(string _hash, uint _crc)
        {
            return (hash.Equals(_hash) && crc.Equals(_crc));
        }
    }
    public class BundleStorageInfoList
    {
        private const string KEY_DATA = "data";

        private const string KEY_BUNDLE_NAME = "bundle_name";
        private const string KEY_BUNDLE_PATH = "bundle_path";
        private const string KEY_BUNDLE_FOLDER_PATH = "bundle_folder_path";
        private const string KEY_HASH_FOLDER_NAME = "hash_folder_name";
        private const string KEY_BUNDLE_HASH = "bundle_hash";
        private const string KEY_BUNDLE_CRC = "bundle_crc";

        private static string storageListFilePath
        #region storageListFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_storageListFilePath))
                {
                    _storageListFilePath = BundleStorageHandler.bundleStoragePath + BundleStorageHandler.BUNDLE_STORAGE_LIST_FILE_NAME;
                }

                return _storageListFilePath;
            }
        }
        private static string _storageListFilePath = "";
        #endregion  //storageListFilePath

        public Dictionary<string, BundleStorageInfo> bundleDic { get { return _bundleDic; } }
        private readonly Dictionary<string, BundleStorageInfo> _bundleDic = new Dictionary<string, BundleStorageInfo>();

        public bool Init()
        {
            _bundleDic.Clear();

            string data = "";
            if (File.Exists(storageListFilePath))
            {
                using (StreamReader reader = new StreamReader(File.OpenRead(storageListFilePath)))
                {
                    data = reader.ReadToEnd();
                }

                return Load(data);
            }
            else
            {
                WriteList();
            }

            return true;
        }

        public void DeleteStorageFileList()
        {
            try
            {
                if (File.Exists(storageListFilePath))
                {
                    File.Delete(storageListFilePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("BundleStorageList::DeleteStorageFileList() {0}", e);
            }

            _bundleDic.Clear();
        }

        private bool Load(string _json_data)
        {
            _bundleDic.Clear();

            if (string.IsNullOrEmpty(_json_data))
                return true;

            JSONNode _root = JSONNode.LoadFromBase64(_json_data);

            JSONArray _total = _root[KEY_DATA].AsArray;
            if (null == _total)
                return false;

            int count = _total.Count;
            for (int i = 0; i < count; ++i)
            {
                JSONNode _infoNode = _total[i];
                if (null == _infoNode)
                    return false;

                BundleStorageInfo info = new BundleStorageInfo();

                info.bundleName         = _infoNode[KEY_BUNDLE_NAME];
                info.bundlePath         = _infoNode[KEY_BUNDLE_PATH];
                info.bundleFolderPath   = _infoNode[KEY_BUNDLE_FOLDER_PATH];
                info.hashFolderName     = _infoNode[KEY_HASH_FOLDER_NAME];
                info.hash               = _infoNode[KEY_BUNDLE_HASH];
                info.crc                = PDataParser.ParseUInt(_infoNode[KEY_BUNDLE_CRC], 0);

                _bundleDic.Add(info.bundleName, info);
            }

            return true;
        }

        private void WriteList()
        {
            JSONNode _root = new JSONObject();

            JSONArray _total = new JSONArray();
            _root.Add(KEY_DATA, _total);

            Dictionary<string, BundleStorageInfo>.Enumerator iter = _bundleDic.GetEnumerator();
            while (iter.MoveNext())
            {
                string bundleName = iter.Current.Key;
                BundleStorageInfo info = iter.Current.Value;

                JSONNode jsonInfo = new JSONObject();

                jsonInfo.Add(KEY_BUNDLE_NAME, new JSONString(info.bundleName));
                jsonInfo.Add(KEY_BUNDLE_PATH, new JSONString(info.bundlePath));
                jsonInfo.Add(KEY_BUNDLE_FOLDER_PATH, new JSONString(info.bundleFolderPath));
                jsonInfo.Add(KEY_HASH_FOLDER_NAME, new JSONString(info.hashFolderName));
                jsonInfo.Add(KEY_BUNDLE_HASH, new JSONString(info.hash));
                jsonInfo.Add(KEY_BUNDLE_CRC, new JSONNumber(info.crc));

                _total.Add(jsonInfo);
            }

            Helper_SimpleJson.SaveToFileBase64(_root, storageListFilePath);
        }

        public bool HasInStorage(string _bundle_name, string _hash, uint _crc)
        {
            if (string.IsNullOrEmpty(_bundle_name))
            {
                PLog.AnyLogError("BundleStorageList::HasInStorage() Invalid bundle name. used = '{0}'", _bundle_name);
                return false;
            }

            BundleStorageInfo info = GetBundleStorageInfo(_bundle_name);
            if (null == info)
                return false;

            return info.Equal(_hash, _crc);
        }

        public BundleStorageInfo GetBundleStorageInfo(string _bundle_name)
        {
            if (string.IsNullOrEmpty(_bundle_name))
            {
                PLog.AnyLogError("BundleStorageList::GetBundleStorageInfo() Invalid bundle name. used = '{0}'", _bundle_name);
                return null;
            }

            BundleStorageInfo info;
            if (!_bundleDic.TryGetValue(_bundle_name, out info))
                return null;

            return info;
        }

        public void Add(string _name, string _path, string _hash_path, string _hash_folder_name, string _hash, uint _crc)
        {
            BundleStorageInfo info = null;
            if (_bundleDic.TryGetValue(_name, out info))
            {
                if (info.hash == _hash && info.crc == _crc)
                {
                    PLog.AnyLogError("BundleStorageList::Add() Already has bundle. bundle name = {0}, hash = {1}, crc = {2}", _name, _hash, _crc);
                    return;
                }

                PLog.AnyLog("BundleStorageList::Add() Changed bundle. bundle name = {0}, hash = {1}, crc = {2}", _name, _hash, _crc);
                _bundleDic.Remove(_name);
            }

            BundleStorageInfo newInfo = new BundleStorageInfo();
            newInfo.bundleName = _name;
            newInfo.bundlePath = _path;
            newInfo.bundleFolderPath = _hash_path;
            newInfo.hashFolderName = _hash_folder_name;
            newInfo.hash = _hash;
            newInfo.crc = _crc;

            _bundleDic.Add(_name, newInfo);
            WriteList();
        }

        public void Remove(string _name)
        {
            if (!_bundleDic.ContainsKey(_name))
                return;

            _bundleDic.Remove(_name);
            WriteList();
        }
    }
}