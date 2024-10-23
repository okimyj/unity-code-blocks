using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

using Pieceton.BuildPlug;
using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public static class PiecetonBuildAndRun
    {
        // ADB로 기기에 접근 가능한 상태인지 확인하는 함수 필요하고
        // 빌드전, BuildAndRun 타이밍에 체크하도록 예외처리 추가 하는게 좋겠고
        // 프로젝트로 익스포트 사용하는 경우에는 호출하지 않도록
        public static void BuildAndRunForAndroid(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                throw new Exception("PiecetonBuildAndRun::BuildAndRunForAndroid() PBuildPlug is null.");

            if (!_build_plug.androidBuildAndRun)
                return;

            if (_build_plug.platform != PiecetonPlatform.Android)
            {
                Debug.LogError("PiecetonPackageBuilder::BuildAndRunForAndroid() Dont supported platform.");
                //throw new Exception("PiecetonPackageBuilder::BuildAndRunForAndroid() Dont supported platform.");
            }

            if (_build_plug.exportAndroidProject)
            {
                Debug.LogError("PiecetonPackageBuilder::BuildAndRunForAndroid() Dont supported in native build.");
                //throw new Exception("PiecetonPackageBuilder::BuildAndRunForAndroid() Dont supported in native build.");
                return;
            }

            string apkPath = Helper_PiecetonBuildPlug.GetPackageExportPath(_build_plug);
            if (!ExistApk(apkPath))
                return;

            string packageName = Helper_PiecetonBuildPlug.GetPackageName(_build_plug);

            BuildAndRunImpl(apkPath, packageName);
        }

        private static void BuildAndRunImpl(string _apk_path, string _package_name)
        {
            if (!ExistApk(_apk_path))
                return;

            string adbPath = GetADBPath();
            if (string.IsNullOrEmpty(adbPath))
                return;

            string activity = GetActivity(_package_name);
            if (string.IsNullOrEmpty(activity))
                return;

            Install(adbPath, _apk_path);
            Run(adbPath, activity);
        }

        private static void Install(string _adb_path, string _apk_path)
        {
            //%SDK_PATH%\adb install -r %APK_NAME%
            string arguments = string.Format("install -r {1}", _adb_path, _apk_path);
            Execute(_adb_path, arguments);
        }

        private static void Run(string _adb_path, string _activity_name)
        {
            //%SDK_PATH%\adb shell am start -n %TARGET_ACTIVITY%
            string arguments = string.Format("shell am start -n {1}", _adb_path, _activity_name);
            Execute(_adb_path, arguments);
        }

        private static void Execute(string _adb_path, string _argument)
        {
            string errorMsg = null;

            try
            {
                PLog.AnyLog("BuildAndRun::Execute() Arguments = {0}", _argument);

                using (Process p = new Process())
                {
                    p.StartInfo.FileName = _adb_path;
                    p.StartInfo.Arguments = _argument;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;

                    using (Process process = Process.Start(p.StartInfo))
                    {
                        string result = process.StandardOutput.ReadToEnd();
                        Debug.LogFormat("Process result : {0}", result);

                        errorMsg = process.StandardError.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                errorMsg = string.Format("Failed {0} execute process. error='{1}'", _argument, e);
            }

            if (false == string.IsNullOrEmpty(errorMsg))
            {
                Debug.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
        }

        private static bool ExistApk(string _apk_path)
        {
            if (string.IsNullOrEmpty(_apk_path))
            {
                PLog.AnyLogError("PiecetonBuildAndRun::BuildAndRun() Invalid Apk path.");
                return false;
            }

            bool existApk = false;
            try
            {
                existApk = File.Exists(_apk_path);
            }
            catch { }

            if (!existApk)
            {
                PLog.AnyLogError("PiecetonBuildAndRun::BuildAndRun() Not found Apk. path = '{0}'", _apk_path);
                return false;
            }

            return true;
        }

        private static string GetADBPath()
        {
            string sdkHome = Environment.GetEnvironmentVariable("ANDROID_HOME", EnvironmentVariableTarget.Machine);
            if (string.IsNullOrEmpty(sdkHome))
            {
                PLog.AnyLogError("PiecetonBuildAndRun::BuildAndRun() Not found system variable ANDROID_HOME.");
                return "";
            }

            string path = string.Format(@"{0}\platform-tools\adb.exe", sdkHome);

            bool existADB = false;
            try
            {
                existADB = File.Exists(path);
            }
            catch { }

            if (!existADB)
            {
                PLog.AnyLogError("PiecetonBuildAndRun::BuildAndRun() Not found Adb.exe. path = '{0}'", path);
                return "";
            }

            return path;
        }

        private static string GetActivity(string _package_name)
        {
            if (string.IsNullOrEmpty(_package_name))
            {
                PLog.AnyLogError("PiecetonBuildAndRun::BuildAndRun() Invalid package name.");
                return "";
            }

            string fixedUnityActivityName = "com.unity3d.player.UnityPlayerActivity";

            string packageActivityName = string.Format("{0}/{1}", _package_name, fixedUnityActivityName);

            return packageActivityName;
        }
    }
}