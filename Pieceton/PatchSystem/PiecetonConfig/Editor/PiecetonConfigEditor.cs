using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pieceton.Configuration
{
    public static class PiecetonMenuEditor
    {
        public const string MENU_BUILD_PLUG_ROOT = PiecetonConfig.Menu.MENU_PIECETON_ROOT + "BuildPlug/";

        public const string MENU_PATCH_LOCALIZE_ROOT = PiecetonConfig.Menu.MENU_PIECETON_ROOT + "Localize/";

        ///////////////////////////////////////////////////////////////////////////
        // Menu item names of Package builder
        public const string MENU_PATCH_SYSTEM_PACKAGE_BUILER_ROOT = MENU_BUILD_PLUG_ROOT + "Package/";
        ///////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////
        /// Menu item names of Asset bundle builder 
        private const string ASSET_BUNDLE_MENU = MENU_BUILD_PLUG_ROOT + "AssetBundle/";

        public const string ASSET_BUNDLE_MENU_SIMULATE = MENU_BUILD_PLUG_ROOT + "Simulate AssetBundles";
        ///////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////
        // Menu item names of Upload
        public const string MENU_PATCH_SYSTEM_UPLOAD_ROOT = MENU_PATCH_SYSTEM_PACKAGE_BUILER_ROOT + "Upload/";
        ///////////////////////////////////////////////////////////////////////////
    }

    public static class PatchSystemEditor
    {
        // 패치시스템 루트
        public static readonly string PATCH_SYSTEM_ROOT = PiecetonConfig.Path.PIECETON_SYSTEM_ROOT + "PatchSystem/";


        // 파이썬 파일 루트
        public static readonly string UPLOAD_PYTHON_ORIGINAL_ROOT = PATCH_SYSTEM_ROOT + "PiecetonUploader/Editor/Python/";
        public static readonly string UPLOAD_PYTHON_ORIGINAL_OLD_ROOT = UPLOAD_PYTHON_ORIGINAL_ROOT + "old_style";

        public static readonly string UPLOAD_PYTHON_LIBRARY_FOLDER_NAME = "FUploaderClass";

        public static readonly string releaseInfoRoot = PATCH_SYSTEM_ROOT + "GeneratedFiles";
        public static readonly string buildInfoRoot = PATCH_SYSTEM_ROOT + "GeneratedFiles/Editor";

        public static readonly string releaseInfoPath = releaseInfoRoot + "/" + "ReleaseInfo.cs";
        public static readonly string buildInfoPath = buildInfoRoot + "/" + "BuildInfo.cs";

        public const string BUILD_INFO_GENERATOR_RELEASEINFO_NAME = "ReleaseInfo.cs";
        public const string BUILD_INFO_GENERATOR_BUILDINFO_NAME = "BuildInfo.cs";

        ///////////////////////////////////////////////////////////////////////////
        /// 에셋번들 폴더
        public static readonly string CONFIG_ORIGINAL_PATH = PATCH_SYSTEM_ROOT + "GameConfigFile/Editor/";

        public const string ASSET_BUNDLE_ARCHIVE_PATH = "PatchArchive/";
        public const string ASSET_BUNDLE_UPLOADLIST_FILENAME = "upload_list";
        public const string ASSET_BUNDLE_UPLOADLIST_EXTENSION = ".txt";

        public static string PatchArchiveRoot
        {
            get
            {
                if (string.IsNullOrEmpty(_patchArchiveRoot))
                {
                    string temp = Application.dataPath.Replace("Assets", "");

                    _patchArchiveRoot = temp + ASSET_BUNDLE_ARCHIVE_PATH;
                }

                return _patchArchiveRoot;
            }
        }
        private static string _patchArchiveRoot = "";

        public static string ASSET_BUNDLE_VERSION_PATH { get { return ASSET_BUNDLE_ARCHIVE_PATH + "Version"; } }
        ///////////////////////////////////////////////////////////////////////////

        public const string ASSET_BUNDLE_CACHE_FOLDER_NAME = "AssetBundlesCache";

        ///////////////////////////////////////////////////////////////////////////
        // iOS build infomation
        public const string DEVELOPER = "developer";
        public const string APPSTORE = "appstore";
        public const string ADHOC = "adhoc";
        public const string ENTERPRISE = "enterprise";

        public const string IOS_BUILD_PATH_DEVELOPER = "/../" + DEVELOPER;
        public const string IOS_BUILD_PATH_APPSTORE = "/../" + APPSTORE;
        public const string IOS_BUILD_PATH_ADHOC = "/../" + ADHOC;
        public const string IOS_BUILD_PATH_ENTERPRISE = "/../" + ENTERPRISE;
        ///////////////////////////////////////////////////////////////////////////
    }

    public static class AssetBundleEditor
    {
        // 에셋번들 포맷 옵션
        public static BuildAssetBundleOptions bundleBuildOption = BuildAssetBundleOptions.ChunkBasedCompression;
    }

}