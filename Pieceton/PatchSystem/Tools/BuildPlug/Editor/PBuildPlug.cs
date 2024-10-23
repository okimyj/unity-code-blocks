using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

using Pieceton.Misc;
using Pieceton.Configuration;
using Pieceton.PatchSystem;

namespace Pieceton.BuildPlug
{
    [System.Serializable]
    public class ListString { public List<string> list = new List<string>(); }
    [System.Serializable]
    public class PBuildPreProcessor
    {
        [SerializeField]
        public Object script;
        [SerializeField]
        public string methodName;
    }
    [InitializeOnLoad]
    public partial class PBuildPlug : ScriptableObject
    {
        public ReleaseType releaseType = ReleaseType.Dev;
        public PiecetonPlatform platform = PiecetonPlatform.None;

        public AndroidBuildSystem aos_BuildSystem = AndroidBuildSystem.Gradle;
        public MobileTextureSubtarget aos_TextureSubtarget = MobileTextureSubtarget.ETC;
        public bool exportAndroidProject = false;
        public ScriptingImplementation androidScriptBackend = ScriptingImplementation.Mono2x;

        public string iosEnterpriseCert = "";

        public bool overrideGameVersion = false;
        public string[] GameVersion = new string[(int)PiecetonPlatform.End];
        public string[] VersionCode = new string[(int)PiecetonPlatform.End];
        public bool[] UseSvnRevision = new bool[(int)PiecetonPlatform.End];

        public ListString[] Symbols = new ListString[(int)PiecetonPlatform.End];

        public string ios_AppDisplayName;


        public PUploadProtocolType[] uploadProtocol = new PUploadProtocolType[(int)UploadInfoType.End];
        public bool[] useBackup = new bool[(int)UploadInfoType.End];

        public PExecuteFlag executeFlag = PExecuteFlag.None;
        public PBuildFlag buildFlag = PBuildFlag.None;
        public PBundleCheckFlag bundleCheckFlag = PBundleCheckFlag.None;

        public PIOSCertificationFlag ios_certificationFlag = PIOSCertificationFlag.None;

        public const string configAssetName = "PBPlug_default";
        public const string configAssetPath = configAssetName + ".asset";

        public bool androidBuildAndRun { get { return _androidBuildAndRun; } set { _androidBuildAndRun = value; } }
        private bool _androidBuildAndRun = false;

        public bool hasDirty { get { return _hasDirty; } }
        private bool _hasDirty = false;
        public void SetHasDirty() { _hasDirty = true; }
        public void ClearHasDirty() { _hasDirty = false; }
        public List<PBuildPreProcessor> preProcessors;

        public static PBuildPlug LoadObject(string _object_name)
        {
            if (string.IsNullOrEmpty(_object_name))
                return null;

            string path = string.Format("{0}{1}.asset", PiecetonConfig.Path.PIECETON_BUILD_PLUG_ROOT, _object_name);

            Debug.LogFormat("PBuildPlug::LoadObject() obj name = {0}, path = {1}", _object_name, path);

            PBuildPlug plug = AssetDatabase.LoadAssetAtPath<PBuildPlug>(path);
            if (null == plug)
                return null;

            //SerializedObject serialObj = new SerializedObject(plug);
            return plug;
        }

        public static PBuildPlug LoadCreateObject(string _object_name)
        {
            if (string.IsNullOrEmpty(_object_name))
                return null;

            string path = string.Format("{0}{1}.asset", PiecetonConfig.Path.PIECETON_BUILD_PLUG_ROOT, _object_name);
            PBuildPlug objectInst = AssetDatabase.LoadAssetAtPath<PBuildPlug>(path);
            if (null == objectInst)
            {
                PBuildPlug exampleAsset = CreateInstance<PBuildPlug>();

                if (!Directory.Exists(PiecetonConfig.Path.PIECETON_BUILD_PLUG_ROOT))
                {
                    Directory.CreateDirectory(PiecetonConfig.Path.PIECETON_BUILD_PLUG_ROOT);
                }

                AssetDatabase.CreateAsset(exampleAsset, PiecetonConfig.Path.PIECETON_BUILD_PLUG_ROOT + configAssetPath);
                AssetDatabase.Refresh();

                objectInst = AssetDatabase.LoadAssetAtPath<PBuildPlug>(path);
                Initialize(objectInst);
            }

            return objectInst;
        }

        private static PBuildPlug LoadPiecetonObject(string _path)
        {
            return AssetDatabase.LoadAssetAtPath<PBuildPlug>(_path);
        }

        #region for editor
#if UNITY_EDITOR
        public static void Initialize(PBuildPlug _build_plug)
        {
            Init_Platform(_build_plug);

            Init_GameVersion(_build_plug);
            Init_VersionCode(_build_plug);

            Init_AdditionalDefineSymbols(_build_plug);

            Init_IOS_AppDisplayName(_build_plug);
            Init_IOS_ExportType(_build_plug);

            Init_UploadInfos(_build_plug);

            AssetDatabase.SaveAssets();
        }

        private static void Init_Platform(PBuildPlug _build_plug)
        {
            if (_build_plug.platform <= PiecetonPlatform.None || _build_plug.platform >= PiecetonPlatform.End)
            {
                //_build_plug.platform = PiecetonPlatform.Android;
                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.Android: _build_plug.platform = PiecetonPlatform.Android; break;
                    case BuildTarget.iOS: _build_plug.platform = PiecetonPlatform.iOS; break;
                    default: _build_plug.platform = PiecetonPlatform.None; break;
                }

                if (_build_plug.platform == PiecetonPlatform.None)
                {
                    PLog.AnyLogError("PBuildPlug::Init_Platform() invalid taret platform is {0}", _build_plug);
                }
                else
                {
                    PLog.AnyLog("PBuildPlug::Init_Platform() active taret platform is {0}", _build_plug);
                }

                EditorUtility.SetDirty(_build_plug);
            }
        }

        private static void Init_GameVersion(PBuildPlug _build_plug)
        {
            int platformCount = (int)PiecetonPlatform.End;
            int platformIdx = (int)_build_plug.platform;

            for (int i = 0; i < platformCount; ++i)
            {
                if (!string.IsNullOrEmpty(_build_plug.GameVersion[i]))
                    continue;

                _build_plug.GameVersion[i] = PlugPreferenceInfo.instance.gameVersion[i];
                EditorUtility.SetDirty(_build_plug);
            }
        }

        private static void Init_VersionCode(PBuildPlug _build_plug)
        {
            int platformCount = (int)PiecetonPlatform.End;
            int platformIdx = (int)_build_plug.platform;

            for (int i = 0; i < platformCount; ++i)
            {
                if (!string.IsNullOrEmpty(_build_plug.VersionCode[(int)i]))
                    continue;

                _build_plug.VersionCode[(int)i] = PlugPreferenceInfo.instance.versionCode[(int)i];
                EditorUtility.SetDirty(_build_plug);
            }
        }

        private static void Init_AdditionalDefineSymbols(PBuildPlug _build_plug)
        {
            for (PiecetonPlatform i = PiecetonPlatform.None + 1; i < PiecetonPlatform.End; ++i)
            {
                if (null == _build_plug.Symbols[(int)i])
                    _build_plug.Symbols[(int)i] = new ListString();
            }
            // _instance.AdditionalDefineSymbols은 사용하지 않는 경우도 있어 빈값 허용
        }

        private static void Init_IOS_AppDisplayName(PBuildPlug _build_plug)
        {
            if (string.IsNullOrEmpty(_build_plug.ios_AppDisplayName))
            {
                _build_plug.ios_AppDisplayName = PlayerSettings.productName;
            }
        }

        private static void Init_IOS_ExportType(PBuildPlug _build_plug)
        {
            //if (_instance.ios_certificationFlag == PIOSCertificationFlag.None)
            //{
            //    _instance.ios_certificationFlag = PIOSCertificationFlag.Development;
            //}
        }

        private static void Init_UploadInfos(PBuildPlug _build_plug)
        {
            //if (string.IsNullOrEmpty(_build_plug.whynot))
            //{
            //    EditorUtility.SetDirty(_build_plug);
            //    _build_plug.whynot = "whynot";
            //}
        }
#endif
        #endregion for editor
    }
}