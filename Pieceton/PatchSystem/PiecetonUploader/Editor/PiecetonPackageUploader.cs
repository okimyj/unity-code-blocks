using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Pieceton.BuildPlug;
using Pieceton.Configuration;
using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public enum PUploadProtocolType
    {
        None = -1,

        FTP = 0,
        SFTP,
        S3,

        End
    }

    public class PiecetonPackageUploader
    {
        public static void Package(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                throw new Exception("PiecetonPackageUploader::Package(PBuildPlug) PBuildPlug is null.");

            if (_build_plug.platform == PiecetonPlatform.Android)
            {
                ReqUploadPackage(_build_plug, UploadInfoType.Package, Helper_PiecetonBuildPlug.ExportType.Android_Release);
                ReqUploadPackage(_build_plug, UploadInfoType.Package, Helper_PiecetonBuildPlug.ExportType.Android_Debug);

                ReqUploadPackage(_build_plug, UploadInfoType.PackageBackup, Helper_PiecetonBuildPlug.ExportType.Android_Release);
                ReqUploadPackage(_build_plug, UploadInfoType.PackageBackup, Helper_PiecetonBuildPlug.ExportType.Android_Debug);
            }
            else if (_build_plug.platform == PiecetonPlatform.iOS)
            {
                ReqUploadPackage(_build_plug, UploadInfoType.Package, Helper_PiecetonBuildPlug.ExportType.iOS_Developer);
                ReqUploadPackage(_build_plug, UploadInfoType.Package, Helper_PiecetonBuildPlug.ExportType.iOS_Appstore);
                ReqUploadPackage(_build_plug, UploadInfoType.Package, Helper_PiecetonBuildPlug.ExportType.iOS_Enterprise);

                ReqUploadPackage(_build_plug, UploadInfoType.PackageBackup, Helper_PiecetonBuildPlug.ExportType.iOS_Developer);
                ReqUploadPackage(_build_plug, UploadInfoType.PackageBackup, Helper_PiecetonBuildPlug.ExportType.iOS_Appstore);
                ReqUploadPackage(_build_plug, UploadInfoType.PackageBackup, Helper_PiecetonBuildPlug.ExportType.iOS_Enterprise);
            }
        }

        private static void ReqUploadPackage(PBuildPlug _build_plug, UploadInfoType _upload_info_type, Helper_PiecetonBuildPlug.ExportType _export_type)
        {
            PUploadInfo uploadInfo = Helper_PiecetonBuildPlug.MakeUploadInfo(_build_plug, _upload_info_type);
            if (null == uploadInfo)
                throw new Exception();

            // 2024-06-13 없는게 맞는거 같음. 사용하지 않는다.
            // if (!uploadInfo.use)
            // {
            //     PLog.AnyLog("PiecetonPackageUploader::ReqUploadPackage() UploadInfoType = '{0}'", _upload_info_type);
            //     return;
            // }

            string gameVersionName = Helper_PiecetonBuildPlug.GetGameVersionName(_build_plug);

            if (_build_plug.platform == PiecetonPlatform.iOS)
            {
                if (!Helper_PiecetonBuildPlug.HasIOSCertification(_build_plug, _export_type))
                    return;
            }

            bool isSFTP = uploadInfo.protocol == PUploadProtocolType.SFTP;

            string pythonName = isSFTP ? DefPython.UPLOAD_PACKAGE : DefPython.PACKAGE_SWITCH;
            string pythonRoot = pythonName == DefPython.UPLOAD_PACKAGE ? PatchSystemEditor.UPLOAD_PYTHON_ORIGINAL_ROOT : PatchSystemEditor.UPLOAD_PYTHON_ORIGINAL_OLD_ROOT;

            bool isBackup = DefUploadInfoType.IsBackupType(_upload_info_type);

            bool useBackup = Helper_PiecetonBuildPlug.GetUseUploadBackup(_build_plug, _upload_info_type);

            if (isBackup && !useBackup)
                return;

            string apkRoot = Helper_PiecetonBuildPlug.GetPackageRoot(_build_plug, _export_type);
            string python_path = apkRoot + pythonName;

            string sProtocol = uploadInfo.protocol.ToString();
            string url = uploadInfo.url;
            string id = uploadInfo.id;
            string pw = uploadInfo.pw;

            string uploadRoot = (isBackup ? DefPython.REMOTE_ROOT_BACKUP : DefPython.REMOTE_ROOT_PRODUCT);
            string platform = _build_plug.platform.ToString();
            string versoinFolder = (isBackup ? gameVersionName : "None");  // None, none 이면 게임버전폴더 사용하지 않음

            string productName = PlugPreferenceInfo.instance.ProductName;

            string releaseFolderName = DefReleaseType.GetReleaseFolderName(_build_plug.releaseType);

            //string achivePackageName = Helper_PiecetonBuildPlug.GetPackageExportPath(_build_plug);
            string achivePackageName = _build_plug.AchivePackageName;

            string dstName = Helper_PiecetonBuildPlug.GetDeployPackageName(_build_plug, _export_type, isBackup);

            PythonCommand cmd = PythonCommand.Make(python_path, url, id, pw, uploadRoot, productName, releaseFolderName, platform, versoinFolder, achivePackageName, dstName);
            if (isSFTP)
            {
                cmd = PythonCommand.Make(python_path, sProtocol, url, id, pw, uploadRoot, productName, releaseFolderName, platform, versoinFolder, achivePackageName, dstName);
            }

            if (!uploadInfo.IsValid())
            {
                Debug.LogErrorFormat("PiecetonPackageUploader::ReqUploadPackage() invalid remote info. url='{0}' id='{1}' pw='{2}'\n cmd='{3}'", url, id, pw, cmd);
                return;
            }

            CopyPython.CopyPackageUploader(pythonRoot, pythonName, apkRoot);

            string instPath = Helper_PiecetonBuildPlug.GetInstalledPythonPath(_build_plug);
            ExecutePython.Execute(instPath, cmd);
        }
    }
}