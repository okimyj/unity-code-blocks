using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

using Pieceton.BuildPlug;
using Pieceton.Configuration;

namespace Pieceton.PatchSystem
{
    public class PiecetonBundleUploader
    {
        public static void BundleOverride(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                throw new Exception("PiecetonBundleUploader::BundleOverride(PBuildPlug) PBuildPlug is null.");

            CopyPython.CopyBundle(Helper_PiecetonBuildPlug.GetPatchRoot(_build_plug));

            DateTime curDate = DateTime.Now;

            ReqUploadBundle(_build_plug, UploadInfoType.Bundle);

            ReqUploadFile(_build_plug, UploadInfoType.BundleBackup, DefPatchHandler.PATCH_LIST_FILENAME, curDate);
            ReqUploadFile(_build_plug, UploadInfoType.BundleBackup, DefPatchHandler.PATCH_LIST_BUNDLES_FILENAME, curDate);
        }

        public static void BundleAppend(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                throw new Exception("PiecetonBundleUploader::BundleAppend(PBuildPlug) PBuildPlug is null.");

            CopyPython.CopyBundle(Helper_PiecetonBuildPlug.GetPatchRoot(_build_plug));

            DateTime curDate = DateTime.Now;

            ReqAppendBundle(_build_plug, UploadInfoType.BundleAppend);

            ReqUploadFile(_build_plug, UploadInfoType.BundleAppendBackup, DefPatchHandler.PATCH_LIST_FILENAME, curDate);
            ReqUploadFile(_build_plug, UploadInfoType.BundleAppendBackup, DefGameConfig.CONFIG_FILENAME, curDate);
            ReqUploadFile(_build_plug, UploadInfoType.BundleAppendBackup, DefPatchHandler.PATCH_LIST_BUNDLES_FILENAME, curDate);
        }


        private static void ReqUploadBundle(PBuildPlug _build_plug, UploadInfoType _upload_info_type)
        {
            PUploadInfo uploadInfo = Helper_PiecetonBuildPlug.MakeUploadInfo(_build_plug, _upload_info_type);
            // if (!uploadInfo.use)
            // {
            //     Debug.LogErrorFormat("UploadPackage::ReqUploadBundle() dont use setting");
            //     return;
            // }

            bool isSFTP = uploadInfo.protocol == PUploadProtocolType.SFTP;

            string pythonName = isSFTP ? DefPython.UPLOAD_BUNDLE : DefPython.BUNDLE_SWITCH;
            string pythonRoot = pythonName == DefPython.UPLOAD_BUNDLE ? PatchSystemEditor.UPLOAD_PYTHON_ORIGINAL_ROOT : PatchSystemEditor.UPLOAD_PYTHON_ORIGINAL_OLD_ROOT;

            string python_path = Helper_PiecetonBuildPlug.GetPatchRoot(_build_plug) + pythonName;
            string buildTarget = PBundlePathEditor.PlatformName(_build_plug);

            string sProtocol = uploadInfo.protocol.ToString();
            string url = uploadInfo.url;
            string id = uploadInfo.id;
            string pw = uploadInfo.pw;

            string uploadRoot = DefPython.REMOTE_ROOT_BUNDLE;
            string productName = PlugPreferenceInfo.instance.ProductName;
            string releaseFolderName = DefReleaseType.GetReleaseFolderName(_build_plug.releaseType);
            string gameVersionName = Helper_PiecetonBuildPlug.GetGameVersionName(_build_plug);


            // old style
            PythonCommand cmd = PythonCommand.Make(python_path, url, id, pw, uploadRoot, productName, releaseFolderName, buildTarget, gameVersionName);
            if (isSFTP)
            {
                // new style
                cmd = PythonCommand.Make(python_path, sProtocol, url, id, pw, uploadRoot, productName, releaseFolderName, buildTarget, gameVersionName);
            }

            if (!uploadInfo.IsValid())
            {
                Debug.LogErrorFormat("UploadPackage::ReqUploadBundle() invalid remote info. url='{0}' id='{1}' pw='{2}'\n cmd='{3}'", url, id, pw, cmd);
                return;
            }
            CopyPython.CopyBundleUploader(pythonRoot, pythonName, Helper_PiecetonBuildPlug.GetPatchRoot(_build_plug));
            string instPath = Helper_PiecetonBuildPlug.GetInstalledPythonPath(_build_plug);
            ExecutePython.Execute(instPath, cmd);
        }

        private static void ReqAppendBundle(PBuildPlug _build_plug, UploadInfoType _upload_info_type)
        {
            PUploadInfo uploadInfo = Helper_PiecetonBuildPlug.MakeUploadInfo(_build_plug, _upload_info_type);

            bool isSFTP = uploadInfo.protocol == PUploadProtocolType.SFTP;

            string pythonName = isSFTP ? DefPython.APPEND_BUNDLE : DefPython.BUNDLE_APPEND;

            string python_path = Helper_PiecetonBuildPlug.GetPatchRoot(_build_plug) + pythonName;
            string buildTarget = PBundlePathEditor.PlatformName(_build_plug);

            string sProtocol = uploadInfo.protocol.ToString();
            string url = uploadInfo.url;
            string id = uploadInfo.id;
            string pw = uploadInfo.pw;

            string uploadRoot = DefPython.REMOTE_ROOT_BUNDLE;
            string productName = PlugPreferenceInfo.instance.ProductName;
            string releaseFolderName = DefReleaseType.GetReleaseFolderName(_build_plug.releaseType);
            string gameVersionName = Helper_PiecetonBuildPlug.GetGameVersionName(_build_plug);


            // old style
            PythonCommand cmd = PythonCommand.Make(python_path, url, id, pw, uploadRoot, productName, releaseFolderName, buildTarget, gameVersionName);
            if (isSFTP)
            {
                // new style
                cmd = PythonCommand.Make(python_path, sProtocol, url, id, pw, uploadRoot, productName, releaseFolderName, buildTarget, gameVersionName);
            }

            if (!uploadInfo.IsValid())
            {
                Debug.LogErrorFormat("UploadPackage::ReqAppendBundle() invalid remote info. url='{0}' id='{1}' pw='{2}'\n cmd='{3}'", url, id, pw, cmd);
                return;
            }

            string instPath = Helper_PiecetonBuildPlug.GetInstalledPythonPath(_build_plug);
            ExecutePython.Execute(instPath, cmd);
        }

        private static void ReqUploadFile(PBuildPlug _build_plug, UploadInfoType _upload_info_type, string _file_name, DateTime _upload_date)
        {
            PUploadInfo uploadInfo = Helper_PiecetonBuildPlug.MakeUploadInfo(_build_plug, _upload_info_type);

            bool isBackup = DefUploadInfoType.IsBackupType(_upload_info_type);

            //bool useBackup = DefReleaseType.UseBuildBackup(BuildInfo.releaseType);
            bool useBackup = Helper_PiecetonBuildPlug.GetUseUploadBackup(_build_plug, _upload_info_type);

            if (isBackup && !useBackup)
                return;

            bool isSFTP = uploadInfo.protocol == PUploadProtocolType.SFTP;

            string pythonName = isSFTP ? DefPython.UPLOAD_FILE : DefPython.PATCH_LIST;

            string python_path = Helper_PiecetonBuildPlug.GetPatchRoot(_build_plug) + pythonName;

            string sProtocol = uploadInfo.protocol.ToString();
            string url = uploadInfo.url;
            string id = uploadInfo.id;
            string pw = uploadInfo.pw;

            string uploadRoot = (isBackup ? DefPython.REMOTE_ROOT_BACKUP : DefPython.REMOTE_ROOT_BUNDLE);
            BuildTarget buildTarget = Helper_PiecetonBuildPlug.GetBuildTarget(_build_plug);

            string buildTargetName = (isBackup ? "None" : PBundlePathEditor.PlatformName(buildTarget)); // None, none 이면 플랫폼 버전폴더 사용하지 않음

            string productName = PlugPreferenceInfo.instance.ProductName;
            string releaseFolderName = DefReleaseType.GetReleaseFolderName(_build_plug.releaseType);
            string gameVersionName = Helper_PiecetonBuildPlug.GetGameVersionName(_build_plug);

            string dstName = (isBackup ? Helper_PiecetonBuildPlug.GetBackupPatchFileName(_build_plug, _file_name, _upload_date) : _file_name);


            // old style
            PythonCommand cmd = PythonCommand.Make(python_path, url, id, pw, uploadRoot, productName, releaseFolderName, buildTargetName, gameVersionName, _file_name, dstName);
            if (isSFTP)
            {
                // new style
                cmd = PythonCommand.Make(python_path, sProtocol, url, id, pw, uploadRoot, productName, releaseFolderName, buildTargetName, gameVersionName, _file_name, dstName);
            }

            if (!uploadInfo.IsValid())
            {
                Debug.LogErrorFormat("UploadPackage::ReqUploadFile() invalid remote info. url='{0}' id='{1}' pw='{2}'\n cmd='{3}'", url, id, pw, cmd);
                return;
            }

            string instPath = Helper_PiecetonBuildPlug.GetInstalledPythonPath(_build_plug);
            ExecutePython.Execute(instPath, cmd);
        }
    }
}