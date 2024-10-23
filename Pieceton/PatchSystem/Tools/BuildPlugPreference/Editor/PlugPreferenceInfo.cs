using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Pieceton.PatchSystem;
using Pieceton.Configuration;

namespace Pieceton.PatchSystem
{
    public enum BuildPlugPreferenceType
    {
        ProductInfo = 0,
        GameVersion,
        Certificate,
        Upload,
        BundleDownload,
        Etc,
    }
    public class PlugPreferenceInfo : ScriptableObject
    {
        private static SerializedObject _serialObj;
        private static SerializedProperty _myArray;
        private static PlugPreferenceInfo _instance = null;
        public static PlugPreferenceInfo instance
        #region instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = Load();
                }

                return _instance;
            }
        }
        #endregion

        public string CompanyName;
        public string ProductName;
        public string[] PackageName = new string[(int)PiecetonPlatform.End];

        public string[] gameVersion = new string[(int)PiecetonPlatform.End];
        public string[] versionCode = new string[(int)PiecetonPlatform.End];

        public string keystoreName = "";
        public string keystorePass = "";
        public string keyaliasName = "";
        public string keyaliasPass = "";

        public string[] uploadUrl = new string[(int)PUploadProtocolType.End];
        public string[] uploadId = new string[(int)PUploadProtocolType.End];
        public string[] uploadPw = new string[(int)PUploadProtocolType.End];
        public string[] downloadBaseUrl = new string[(int)ReleaseType.End];

        public string[] installedPythonPath = new string[(int)BuildMachineOSType.End];

        public static void Reload()
        {
            _instance = null;
            _instance = Load();
        }

        private static PlugPreferenceInfo Load()
        {
            string path = string.Format("{0}{1}", PiecetonConfig.Path.PIECETON_TOOLS_PREFERENCE_ROOT, PiecetonConfig.BUILD_PLUG_PREFERENCE_NAME);
            Debug.LogFormat("PlugPreferenceInfo::Load() path = {0}", path);

            PlugPreferenceInfo info = AssetDatabase.LoadAssetAtPath<PlugPreferenceInfo>(path);
            if (null == info)
            {
                PlugPreferenceInfo tmpInfo = CreateInstance<PlugPreferenceInfo>();

                if (!Directory.Exists(PiecetonConfig.Path.PIECETON_BUILD_PLUG_ROOT))
                {
                    Directory.CreateDirectory(PiecetonConfig.Path.PIECETON_BUILD_PLUG_ROOT);
                }

                AssetDatabase.CreateAsset(tmpInfo, path);
                AssetDatabase.Refresh();

                info = AssetDatabase.LoadAssetAtPath<PlugPreferenceInfo>(path);
                PlugPreferenceInitializer.SetDefault(info);
            }

            PlugPreferenceInitializer.CheckValid(info);

            _serialObj = new SerializedObject(info);
            _myArray = _serialObj.FindProperty("Symbols");

            return info;
        }

        public string KeystoreFullPath()
        {
            return Path.Combine(PBundlePathEditor.ProjectRoot(), keystoreName);
        }

        public void Update()
        {
            if (null != _serialObj)
            {
                _serialObj.Update();
            }
        }

        public void Save()
        {
            if (null != _serialObj)
            {
                _serialObj.ApplyModifiedProperties();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}