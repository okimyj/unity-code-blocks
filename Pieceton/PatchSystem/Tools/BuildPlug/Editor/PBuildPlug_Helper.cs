using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

using Pieceton.Misc;
using Pieceton.PatchSystem;

using Pieceton.Configuration;

namespace Pieceton.BuildPlug
{
    public static class Helper_PiecetonBuildPlug
    {
        public static string seperatorChar
        #region seperatorChar
        {
            get
            {
                if (string.IsNullOrEmpty(_seperatorChar))
                {
                    _seperatorChar = string.Format("{0}", Path.DirectorySeparatorChar);
                }

                return _seperatorChar;
            }
        }
        private static string _seperatorChar = null;
        #endregion seperatorChar

        public static readonly string baseGoogleProduct = AndroidExportName() + seperatorChar + PlayerSettings.productName + seperatorChar;
        public static readonly string adtBinPath = baseGoogleProduct + "bin" + seperatorChar;
        public static readonly string gradleBinPath = baseGoogleProduct + "build" + seperatorChar + "outputs" + seperatorChar + "apk" + seperatorChar;

        public static string GetPackageName(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                return "";

            return GetPackageName(_build_plug.platform);
        }

        public static string GetPackageName(PiecetonPlatform _platform_type)
        {
            if (_platform_type <= PiecetonPlatform.None || _platform_type >= PiecetonPlatform.End)
            {
                PLog.AnyLogError("Helper_PiecetonBuildPlug::GetPackageName() Invalid platform type. used {0}", _platform_type);
                return "";
            }

            return PlugPreferenceInfo.instance.PackageName[(int)_platform_type];
        }

        public static BuildTarget GetBuildTarget(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                return BuildTarget.NoTarget;

            return GetBuildTarget(_build_plug.platform);
        }

        public static BuildTarget GetBuildTarget(PiecetonPlatform _platform_type)
        {
            switch (_platform_type)
            {
                case PiecetonPlatform.Android: return BuildTarget.Android;
                case PiecetonPlatform.iOS: return BuildTarget.iOS;
                default: PLog.AnyLogError("Helper_PiecetonBuildPlug::GetBuildTarget() Not implementation. {0}", _platform_type); break;
            }

            return BuildTarget.NoTarget;
        }

        public static string GetReleaseSymbol(PBuildPlug _build_plug)
        {
            return DefReleaseType.GetReleaseSymbol(_build_plug.releaseType);
        }

        public static List<string> GetDefineSymbols(PBuildPlug _build_plug)
        {
            int idx = (int)_build_plug.platform;
            return DefReleaseType.MakeDefineSymbolList(_build_plug.Symbols[idx].list);
        }

        public static string GetGameVersion(PBuildPlug _build_plug)
        {
            int platform = (int)_build_plug.platform;

            if (_build_plug.overrideGameVersion)
            {
                return _build_plug.GameVersion[platform];
            }

            return PlugPreferenceInfo.instance.gameVersion[platform];
        }

        public static string GetGameVersionName(PBuildPlug _build_plug)
        {
            string dotVersion = GetGameVersion(_build_plug);
            return dotVersion.Replace('.', '_');
        }

        public static string GetVersionCode(PBuildPlug _build_plug)
        {
            int platform = (int)_build_plug.platform;

            if (_build_plug.overrideGameVersion)
            {
                return _build_plug.VersionCode[platform];
            }

            return PlugPreferenceInfo.instance.versionCode[platform];
        }

        public static string GetRevision(PBuildPlug _build_plug)
        {
            int idx = (int)_build_plug.platform;
            if (_build_plug.UseSvnRevision[idx])
                return PSvnRevision.GetAssetsRevision();

            return "";
        }

        public static bool IsActivatedSimulatePatch(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                return false;

            bool debug = _build_plug.HasBuildFlag(PBuildPlug.PBuildFlag.Debug);
            if (!debug)
                return false;

            bool patch = _build_plug.HasBuildFlag(PBuildPlug.PBuildFlag.SimulatePatch);

            return patch;
        }

        public static bool IsActivatedStreamingBundle(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                return false;

            if (_build_plug.platform != PiecetonPlatform.iOS)
            {
                bool patch = IsActivatedSimulatePatch(_build_plug);

                if (!patch)
                    return false;
            }

            bool stream = _build_plug.HasBuildFlag(PBuildPlug.PBuildFlag.StreamBundle);

            return stream;
        }

        public static string GetPackageExportPath(PBuildPlug _build_plug)
        {
            string target_dir = "";

            if (_build_plug.platform == PiecetonPlatform.Android)
            {
                if (_build_plug.aos_BuildSystem == AndroidBuildSystem.Gradle ||
                    !_build_plug.exportAndroidProject)
                {
                    target_dir = _build_plug.AchivePackageName;
                }
                else
                {
                    target_dir = AndroidExportName();
                }
                var exportType = ExportType.Android_Debug;
                // TODO : ...
                return GetPackageRoot(_build_plug, exportType) + seperatorChar + target_dir;
            }

            return "";
        }

        public static string AndroidExportName()
        {
            switch (EditorUserBuildSettings.androidBuildSystem)
            {
                case AndroidBuildSystem.Gradle: return PlayerSettings.productName + "_Gradle";
                //case AndroidBuildSystem.ADT: return PlayerSettings.productName + "_ADT";
                case AndroidBuildSystem.VisualStudio: Debug.Log("AndroidBuildSystem.VisualStudio: ????????????"); break;
            }

            return "";
        }

        public enum ExportType
        {
            Android_Release,
            Android_Debug,
            iOS_Developer,
            iOS_Appstore,
            iOS_AdHoc,
            iOS_Enterprise,
        }
        public static string GetDeployPackageName(PBuildPlug _build_plug, ExportType _eType, bool _isBackup = false)
        {
            string exportInfo = "";

            string versionName = GetGameVersionName(_build_plug);
            string versionCode = GetVersionCode(_build_plug);
            string platformName = GetPlatformFolderName(_build_plug);

            if (_isBackup)
            {
                exportInfo = string.Format("{0}__{1}__{2}", versionName, versionCode, ReleaseInfo.BuildDateForBackup);
            }
            else
            {
                string postName = _eType.ToString().Replace(platformName, "");
                exportInfo = string.Format("{0}_{1}", ReleaseInfo.releaseFolderName, postName);
            }

            return string.Format("{0}_{1}_{2}.{3}", PlugPreferenceInfo.instance.ProductName, platformName, exportInfo, _build_plug.ExtensionName);
        }

        public static BuildMachineOSType CurBuildMachineOSType()
        {
#if UNITY_EDITOR_WIN
            return BuildMachineOSType.Window;
#elif UNITY_EDITOR_OSX
            return BuildMachineOSType.Mac;
#else
            PLog.AnyLogError("Not supported build machine type");
            return BuildMachineOSType.None;
#endif
        }

        public static string GetInstalledPythonPath(PBuildPlug _build_plug)
        {
            BuildMachineOSType osType = CurBuildMachineOSType();
            if (osType > BuildMachineOSType.None && osType < BuildMachineOSType.End)
            {
                return PlugPreferenceInfo.instance.installedPythonPath[(int)osType];
            }

            return "";
        }

        public static string GetPlatformFolderName(PBuildPlug _build_plug)
        {
            Debug.LogFormat("GetPlatformFolderName: {0} -> {1}", _build_plug.name, _build_plug.platform);
            switch (_build_plug.platform)
            {
                case PiecetonPlatform.Android: return "Android";
                case PiecetonPlatform.iOS: return "iOS";
            }

            string msg = string.Format("Helper_PiecetonBuildPlug::GetPlatformFolderName() Dont Implement PiecetonPlatform. {0}", _build_plug.platform);
            throw new Exception(msg);
        }

        public static BuildOptions GetBuildOption(PBuildPlug _build_plug)
        {
            BuildOptions buildOptions = BuildOptions.AcceptExternalModificationsToPlayer;

            switch (_build_plug.platform)
            {
                case PiecetonPlatform.Android:
                    {
                        bool useDevelopBuild = _build_plug.HasBuildFlag(PBuildPlug.PBuildFlag.Debug);
                        buildOptions = (useDevelopBuild ? PiecetonPackageBuilder.opt_aos_debug : PiecetonPackageBuilder.opt_aos_default);

                        if (_build_plug.aos_BuildSystem == AndroidBuildSystem.Gradle ||
                            !_build_plug.exportAndroidProject)
                        {
                            buildOptions = PBuildUtil.RemoveBuildOption(buildOptions, BuildOptions.AcceptExternalModificationsToPlayer);
                        }
                        else
                        {
                            buildOptions = PBuildUtil.AddBuildOption(buildOptions, BuildOptions.AcceptExternalModificationsToPlayer);
                        }
                    }
                    break;

                case PiecetonPlatform.iOS:
                    {
                        if (_build_plug.HasBuildFlag(PBuildPlug.PBuildFlag.First))
                        {
                            buildOptions = BuildOptions.None;
                        }
                        else
                        {
                            buildOptions = BuildOptions.AcceptExternalModificationsToPlayer;
                        }
                    }
                    break;

                default: PLog.AnyLogError("Helper_PiecetonBuildPlug::GetBuildOptin() Not implemantation. '{0}'", _build_plug.platform); break;
            }

            return buildOptions;
        }

        public static string GetPackageRoot(PBuildPlug _build_plug, ExportType _export_type)
        {
            string curRootDir = Directory.GetCurrentDirectory() + seperatorChar + ".." + seperatorChar;

#if UNITY_EDITOR_WIN
            if (_build_plug.platform == PiecetonPlatform.Android)
            {
                switch (_build_plug.aos_BuildSystem)
                {
                    //case AndroidBuildSystem.ADT: return curRootDir + adtBinPath;
                    case AndroidBuildSystem.Gradle: return curRootDir + gradleBinPath;
                    case AndroidBuildSystem.VisualStudio: Debug.Log("AndroidBuildSystem.VisualStudio: ????????????"); break;
                }
            }

            return curRootDir;

#elif UNITY_EDITOR_OSX
            string iosCertificationName = "";

            if (_build_plug.platform == PiecetonPlatform.iOS)
            {
                PBuildPlug.PIOSCertificationFlag flag = ExportTypeToIOSCertification(_export_type);
                if (_build_plug.HasCertificationFlag(flag))
                {
                    switch (flag)
                    {
                        case PBuildPlug.PIOSCertificationFlag.Development: iosCertificationName = PatchSystemEditor.DEVELOPER; break;
                        case PBuildPlug.PIOSCertificationFlag.AppStore: iosCertificationName = PatchSystemEditor.APPSTORE; break;
                        case PBuildPlug.PIOSCertificationFlag.Enterprise: iosCertificationName = PatchSystemEditor.ENTERPRISE; break;
                    }
                }
            }

            if (string.IsNullOrEmpty(iosCertificationName))
                PLog.AnyLogError("Helper_PiecetonBuildPlug::GetPythonRoot() Invalid root directory.");

            return curRootDir + iosCertificationName + seperatorChar;
#else
            return "";
#endif
        }

        public static PBuildPlug.PIOSCertificationFlag ExportTypeToIOSCertification(ExportType _export_type)
        {
            switch (_export_type)
            {
                case ExportType.iOS_Developer: return PBuildPlug.PIOSCertificationFlag.Development;
                case ExportType.iOS_Appstore: return PBuildPlug.PIOSCertificationFlag.AppStore;
                case ExportType.iOS_AdHoc: return PBuildPlug.PIOSCertificationFlag.AdHoc;
                case ExportType.iOS_Enterprise: return PBuildPlug.PIOSCertificationFlag.Enterprise;
            }

            return PBuildPlug.PIOSCertificationFlag.None;
        }

        public static bool HasIOSCertification(PBuildPlug _build_plug, ExportType _export_type)
        {
            if (_build_plug.platform != PiecetonPlatform.iOS)
                return false;

            PBuildPlug.PIOSCertificationFlag flag = ExportTypeToIOSCertification(_export_type);

            string cert = GetIOSCertification(_build_plug, _export_type);

            if (string.IsNullOrEmpty(cert))
                return false;

            return _build_plug.HasCertificationFlag(flag);
        }

        public static string GetIOSCertification(PBuildPlug _build_plug, ExportType _export_type)
        {
            switch (_export_type)
            {
                case ExportType.iOS_Developer:
                case ExportType.iOS_Appstore: return Helper_PiecetonBuildPlug.GetPackageName(PiecetonPlatform.iOS);
                case ExportType.iOS_Enterprise: return _build_plug.iosEnterpriseCert;
            }

            return "";
        }

        public static string GetPatchRoot(PBuildPlug _build_plug)
        {
            string curRoot = Directory.GetCurrentDirectory() + seperatorChar;
            string patch = PatchSystemEditor.ASSET_BUNDLE_ARCHIVE_PATH;

            BuildTarget buildTarget = GetBuildTarget(_build_plug);
            string platform = PBundlePathEditor.PlatformName(buildTarget) + seperatorChar;

            return curRoot + patch + platform;
        }

        public static string GetBackupPatchFileName(PBuildPlug _build_plug, string _file_name, DateTime _upload_date)
        {
            string buildDate = _upload_date.ToString("yyyy-MM-dd HH:mm:ss");
            string buildDateForBackup = buildDate.Replace(' ', '_').Replace(':', '-');

            BuildTarget buildTarget = GetBuildTarget(_build_plug);
            string gameVersionName = GetGameVersionName(_build_plug);
            string versionCode = GetVersionCode(_build_plug);

            return string.Format("{0}__{1}__{2}__{3}__{4}__{5}", buildTarget, gameVersionName, versionCode, ReleaseInfo.BuildDateForBackup, buildDateForBackup, _file_name);
        }

        public static PUploadInfo MakeUploadInfo(PBuildPlug _build_plug, UploadInfoType _info_type)
        {
            PUploadInfo info = new PUploadInfo();

            if (null != _build_plug)
            {
                int index = (int)_info_type;

                if (index >= 0 && index < (int)UploadInfoType.End)
                {
                    PUploadProtocolType protocol = GetProtocolType(_build_plug, _info_type);

                    bool use = GetUseUploadBackup(_build_plug, _info_type);
                    string url = GetUploadUrl(_build_plug, _info_type);
                    string id = GetUploadID(_build_plug, _info_type);
                    string pw = GetUploadPW(_build_plug, _info_type);

                    info.Set(use, protocol, url, id, pw);
                    return info;
                }
                else
                {
                    PLog.AnyLogError("PBuildPlug_Helper::MakeUploadInfo() invalid UploadInfoType. tried = '{0}'", _info_type);
                }
            }
            else
            {
                PLog.AnyLogError("PBuildPlug_Helper::MakeUploadInfo() PBuildPlug is null.");
            }

            return info;
        }

        public static bool GetUseUploadBackup(PBuildPlug _build_plug, UploadInfoType _info_type)
        {
            if (null != _build_plug)
            {
                int index = (int)_info_type;

                if (index >= 0 && index < (int)UploadInfoType.End)
                {
                    return _build_plug.useBackup[index];
                }
            }

            return false;
        }

        public static PUploadProtocolType GetProtocolType(PBuildPlug _build_plug, UploadInfoType _info_type)
        {
            if (null != _build_plug)
            {
                UploadInfoType targetInfo = _info_type;

                switch (_info_type)
                {
                    case UploadInfoType.Package:
                    case UploadInfoType.PackageBackup: targetInfo = UploadInfoType.Package; break;
                    default: targetInfo = UploadInfoType.Bundle; break;
                }

                int index = (int)targetInfo;

                PUploadProtocolType protocol = _build_plug.uploadProtocol[index];

                return protocol;
            }

            return PUploadProtocolType.None;
        }

        public static string GetUploadUrl(PBuildPlug _build_plug, UploadInfoType _info_type)
        {
            if (null != _build_plug)
            {
                PUploadProtocolType protocol = GetProtocolType(_build_plug, _info_type);

                return PlugPreferenceInfo.instance.uploadUrl[(int)protocol];
            }

            return "";
        }

        public static string GetUploadID(PBuildPlug _build_plug, UploadInfoType _info_type)
        {
            if (null != _build_plug)
            {
                PUploadProtocolType protocol = GetProtocolType(_build_plug, _info_type);

                return PlugPreferenceInfo.instance.uploadId[(int)protocol];
            }

            return "";
        }

        public static string GetUploadPW(PBuildPlug _build_plug, UploadInfoType _info_type)
        {
            if (null != _build_plug)
            {
                PUploadProtocolType protocol = GetProtocolType(_build_plug, _info_type);

                return PlugPreferenceInfo.instance.uploadPw[(int)protocol];
            }

            return "";
        }

        public static string GetDownloadBaseUrl(PBuildPlug _build_plug, ReleaseType _release_type)
        {
            if (null != _build_plug)
            {
                return PlugPreferenceInfo.instance.downloadBaseUrl[(int)_release_type];
            }

            return "";
        }
    }
}