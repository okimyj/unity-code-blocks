using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

using Pieceton.BuildPlug;
using Pieceton.Configuration;

namespace Pieceton.PatchSystem
{
    public class PBundlePathEditor
    {
        public static string ProjectRoot()
        {
            return Directory.GetParent(Application.dataPath).FullName + Path.DirectorySeparatorChar;
        }

        public static string PlatformName(PBuildPlug _plug)
        {
            if (null == _plug)
                throw new Exception("PBundlePathEditor::PlatformName() PBuildPlug is null.");

            return PlatformName(Helper_PiecetonBuildPlug.GetBuildTarget(_plug));
        }

        public static string PlatformName(BuildTarget _target)
        {
            Debug.LogFormat("PlatformName:{0}", _target);
            switch (_target)
            {
                case BuildTarget.Android: return "Android";
                case BuildTarget.iOS: return "iOS";
                case BuildTarget.WebGL: return "WebPlayer";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64: return "Windows";
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                //case BuildTarget.StandaloneOSX: return "OSX";
                default: return "unknown";
            }
        }

        public static string BuildOutputPath(BuildTarget buildTarget, ReleaseType releaseType)
        {
            string seperChar = string.Format("{0}", System.IO.Path.DirectorySeparatorChar);
            string rootDir = Directory.GetCurrentDirectory() + seperChar;// + ".." + seperChar + ".." + seperChar;
            string releaseDir = DefReleaseType.GetReleaseFolderName(releaseType);
            string platform = PlatformName(buildTarget);

            string output_path = rootDir + PatchSystemEditor.ASSET_BUNDLE_CACHE_FOLDER_NAME + seperChar + releaseDir + seperChar + platform;

            return output_path;
        }

        public static string MakeCDNUrlFull(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                throw new Exception("AssetBundleEditorPath::MakeCDNUrlFull() PBuildPlug is null.");

            string cdnRoot = Helper_PiecetonBuildPlug.GetDownloadBaseUrl(_build_plug, _build_plug.releaseType);

            string final = cdnRoot + MakeCDNUrlSub(_build_plug);
            Debug.LogFormat("CDN URL:{0}", final);
            return final;
        }

        public static string MakeCDNUrlSub(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                throw new Exception("AssetBundleEditorPath::MakeCDNUrlSub() PBuildPlug is null.");

            string release = DefReleaseType.GetReleaseFolderName(_build_plug.releaseType);
            string version = Helper_PiecetonBuildPlug.GetGameVersionName(_build_plug);
            string platformName = PlatformName(_build_plug);

            string final = MakeCDNProductPath(_build_plug) + release + "/" + platformName + "/" + version + "/";
            return final;
        }

        public static string MakeCDNProductPath(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                throw new Exception("AssetBundleEditorPath::MakeCDNProductPath() PBuildPlug is null.");
            // cdn 주소 바뀌면 수정해야 함. 다운로드하는 url은 nas의 bundle 폴더가 root라..
            // string final = DefPython.REMOTE_ROOT_BUNDLE + "/" + PlugPreferenceInfo.instance.ProductName + "/";
            string final = PlugPreferenceInfo.instance.ProductName + "/";
            return final;
        }

        public static string MakeBundleArchivePath(BuildTarget _build_target, string _version_name)
        {
            string _platform = PBundlePathEditor.PlatformName(_build_target);

            return PatchSystemEditor.ASSET_BUNDLE_ARCHIVE_PATH + _platform + "/" + _version_name;
        }
    }
}