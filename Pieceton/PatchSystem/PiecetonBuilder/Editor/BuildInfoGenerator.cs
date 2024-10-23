using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

using Pieceton.Misc;
using Pieceton.Configuration;
using Pieceton.PatchSystem;

using Pieceton.BuildPlug;

public class BuildInfoGenerator
{
    public static void Generate(string _build_plug_name) { Generate(PBuildPlug.LoadObject(_build_plug_name)); }
    public static void Generate(string _build_plug_name, string buildNumber) { Generate(PBuildPlug.LoadObject(_build_plug_name), buildNumber); }
    public static void Generate(PBuildPlug _plug, string buildNumber = "")
    {
        if (null == _plug)
            throw new Exception("BuildInfoGenerator::Generate() PBuildPlug is null.");

        BuildTarget target = Helper_PiecetonBuildPlug.GetBuildTarget(_plug);
        string productName = PlugPreferenceInfo.instance.ProductName;
        string versionName = Helper_PiecetonBuildPlug.GetGameVersionName(_plug);
        bool simulatePatch = _plug.HasBuildFlag(PBuildPlug.PBuildFlag.SimulatePatch);
        bool streamBundle = _plug.HasBuildFlag(PBuildPlug.PBuildFlag.StreamBundle);

        ProductSetting(_plug);

        string writeBuildInfo = GetBuildInfoCode(_plug);
        string writeReleaseInfo = GetReleaseInfoCode(_plug, _plug.releaseType, productName, versionName, buildNumber);

        if (!PFileGenerator.WriteFile(writeBuildInfo, PatchSystemEditor.buildInfoRoot, PatchSystemEditor.buildInfoPath))
        {
            string errMsg = string.Format("BuildInfoGenerator::Generate() Failed generate BuildInfo.cs path='{0}'", PatchSystemEditor.buildInfoPath);
            throw new Exception(errMsg);
        }

        if (!PFileGenerator.WriteFile(writeReleaseInfo, PatchSystemEditor.releaseInfoRoot, PatchSystemEditor.releaseInfoPath))
        {
            string errMsg = string.Format("BuildInfoGenerator::Generate() Failed generate ReleaseInfo.cs path='{0}'", PatchSystemEditor.releaseInfoPath);
            throw new Exception(errMsg);
        }

        MakeDirectories(target);

        AssetDatabase.Refresh();
        Debug.LogFormat("BuildInfoGenerator::Generate() Success generate plug name = '{0}'", _plug.name);
    }

    private static void ProductSetting(PBuildPlug _plug)
    {
        PlayerSettings.companyName = PlugPreferenceInfo.instance.CompanyName;
        PlayerSettings.productName = PlugPreferenceInfo.instance.ProductName;
        PlayerSettings.applicationIdentifier = Helper_PiecetonBuildPlug.GetPackageName(_plug);
        PlayerSettings.bundleVersion = Helper_PiecetonBuildPlug.GetGameVersion(_plug);

        string versionCode = Helper_PiecetonBuildPlug.GetVersionCode(_plug);
        PlayerSettings.Android.bundleVersionCode = PDataParser.ParseInt(versionCode, 0);
        PlayerSettings.iOS.buildNumber = versionCode;

        PiecetonUnitySetter.BuildSettings(_plug);
    }

    private static string GetBuildInfoCode(PBuildPlug _plug)
    {
        string buildPlugName = _plug.name;

        string code = PFileGenerator.MakeLine(0, "using UnityEditor;", 2);
        code += PFileGenerator.MakeLine(0, "public class BuildInfo");
        code += PFileGenerator.MakeLine(0, "{");
        code += PFileGenerator.MakeLine(1, "public const string buildPlugName = \"" + buildPlugName + "\";");

        //code += PFileGenerator.MakeLine(1, "public const BuildTarget buildTarget = BuildTarget." + _buildTarget + ";");
        //code += PFileGenerator.MakeLine(1, "public const ReleaseType releaseType = ReleaseType." + _releaseType + ";");
        //code += PFileGenerator.MakeLine(1, "public const string releaseFolderName = \"" + Builder.GetReleaseFolderName(_releaseType) + "\";");
        code += PFileGenerator.MakeLine(0, "}");
        return code;
    }

    private static string GetReleaseInfoCode(PBuildPlug _plug, ReleaseType _releaseType, string _product_name, string _version_name, string _build_number = "")
    {
        string streamPatch = (Helper_PiecetonBuildPlug.IsActivatedSimulatePatch(_plug) ? "true" : "false");
        string streamBundle = (Helper_PiecetonBuildPlug.IsActivatedStreamingBundle(_plug) ? "true" : "false");

        long versionCode = long.Parse(Helper_PiecetonBuildPlug.GetVersionCode(_plug));
        string buildDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string buildDateForBackup = buildDate.Replace(' ', '_').Replace(':', '-');

        string tmpBaseUrl = Helper_PiecetonBuildPlug.GetDownloadBaseUrl(_plug, _releaseType);
        string cdnBaseUrl = tmpBaseUrl + PBundlePathEditor.MakeCDNProductPath(_plug);
        string releaseFolderName = DefReleaseType.GetReleaseFolderName(_releaseType);
        string curVersionDownloadURL = PBundlePathEditor.MakeCDNUrlFull(_plug);

        string revision = Helper_PiecetonBuildPlug.GetRevision(_plug);

        string code = PFileGenerator.MakeLine(0, "using System.Collections;", 2);
        code += PFileGenerator.MakeLine(0, "public class ReleaseInfo");
        code += PFileGenerator.MakeLine(0, "{");
        code += PFileGenerator.MakeLine(1, "public const bool useStreamPatch = " + streamPatch + ";");
        code += PFileGenerator.MakeLine(1, "public const bool useStreamBundle = " + streamBundle + ";");
        code += "\n";
        code += PFileGenerator.MakeLine(1, "public const string buildNumber = \"" + _build_number + "\";");
        code += "\n";
        code += PFileGenerator.MakeLine(1, "public const long versionCode = " + versionCode + ";");
        code += PFileGenerator.MakeLine(1, "public const string BuildRevision = \"" + revision + "\";");
        code += PFileGenerator.MakeLine(1, "public const string BuildDate = \"" + buildDate + "\";");
        code += PFileGenerator.MakeLine(1, "public const string BuildDateForBackup = \"" + buildDateForBackup + "\";");
        code += "\n";
        code += PFileGenerator.MakeLine(1, "public const string cdnBaseUrl = \"" + cdnBaseUrl + "\";");
        code += PFileGenerator.MakeLine(1, "public const string releaseFolderName = \"" + releaseFolderName + "\";");
        code += PFileGenerator.MakeLine(1, "public const string curVersionDownloadURL = \"" + curVersionDownloadURL + "\";");
        code += PFileGenerator.MakeLine(0, "}");
        return code;
    }

    private static void MakeDirectories(BuildTarget _buildTarget)
    {
        if (_buildTarget == BuildTarget.iOS)
        {
            string dataPath = Application.dataPath + "/..";

            CreateDirectory(dataPath + PatchSystemEditor.IOS_BUILD_PATH_DEVELOPER);
            CreateDirectory(dataPath + PatchSystemEditor.IOS_BUILD_PATH_APPSTORE);
            CreateDirectory(dataPath + PatchSystemEditor.IOS_BUILD_PATH_ADHOC);
            CreateDirectory(dataPath + PatchSystemEditor.IOS_BUILD_PATH_ENTERPRISE);
        }
    }

    private static void CreateDirectory(string _path)
    {
        if (!Directory.Exists(_path))
        {
            Debug.Log("mkPath = " + _path);
            Directory.CreateDirectory(_path);
        }
    }
}