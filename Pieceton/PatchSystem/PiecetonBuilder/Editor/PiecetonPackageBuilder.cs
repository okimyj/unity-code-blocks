using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;
#if !UNITY_5
using UnityEditor.Build.Reporting;
#endif

using Pieceton.Configuration;
using Pieceton.Misc;
using Pieceton.BuildPlug;

namespace Pieceton.PatchSystem
{
    public static class PiecetonPackageBuilder
    {
        public const BuildOptions opt_aos_default = BuildOptions.SymlinkSources | BuildOptions.AcceptExternalModificationsToPlayer;
        public const BuildOptions opt_aos_debug = BuildOptions.SymlinkSources |
                                                    BuildOptions.Development |
                                                    BuildOptions.ConnectWithProfiler |
                                                    BuildOptions.AllowDebugging |
                                                    BuildOptions.AcceptExternalModificationsToPlayer;

        public static void Build(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                throw new Exception("PiecetonPackageBuilder::Build(PBuildPlug) PBuildPlug is null.");

            string plugName = _build_plug.name;

            Build_Begin(ref _build_plug);

            _build_plug = PBuildPlug.LoadObject(plugName);

            switch (_build_plug.platform)
            {
                case PiecetonPlatform.Android: GenericBuild_Android(ref _build_plug); break;
                case PiecetonPlatform.iOS: GenericBuild_iOS(ref _build_plug); break;
                default: PLog.AnyLogError("PiecetonPackageBuilder::Build() Not implementation. PiecetonPlatform = '{0}'", _build_plug.platform); break;
            }

            Build_End();

            PiecetonBuildAndRun.BuildAndRunForAndroid(_build_plug);
        }

        public static void Build_Begin(ref PBuildPlug _build_plug)
        {
            BuildTarget buildTarget = Helper_PiecetonBuildPlug.GetBuildTarget(_build_plug);
            bool useInternalBundle = _build_plug.HasBuildFlag(PBuildPlug.PBuildFlag.StreamBundle);
            List<string> addSymbols = Helper_PiecetonBuildPlug.GetDefineSymbols(_build_plug);

            StreamingBundleHandlerEditor.DeleteInternalBundle();

            PiecetonUnitySetter.BuildSettings(_build_plug);

            if (useInternalBundle)
            {
                PiecetonBundleBuilder.Build(_build_plug);
            }
        }

        public static void Build_End()
        {
            StreamingBundleHandlerEditor.DeleteInternalBundle();
        }

        private static void GenericBuild_Android(ref PBuildPlug _build_plug)
        {
            string outputPath = Helper_PiecetonBuildPlug.GetPackageExportPath(_build_plug);
            if (!string.IsNullOrEmpty(outputPath))
            {
                try
                {
                    if (File.Exists(outputPath))
                    {
                        File.Delete(outputPath);
                    }
                }
                catch (Exception e)
                {
                    PLog.AnyLogError("{0}", e);
                }
            }

            GenericBuild(ref _build_plug, outputPath);
        }

        private static void GenericBuild_iOS(ref PBuildPlug _build_plug)
        {
            string rootPath = PPath.GetParent(Application.dataPath);

            string packageName = Helper_PiecetonBuildPlug.GetPackageName(PiecetonPlatform.iOS);

            if (_build_plug.HasCertificationFlag(PBuildPlug.PIOSCertificationFlag.Development))
            {
                PlayerSettings.applicationIdentifier = packageName;
                GenericBuild(ref _build_plug, rootPath + PatchSystemEditor.IOS_BUILD_PATH_DEVELOPER);
            }

            if (_build_plug.HasCertificationFlag(PBuildPlug.PIOSCertificationFlag.AppStore))
            {
                PlayerSettings.applicationIdentifier = packageName;
                GenericBuild(ref _build_plug, rootPath + PatchSystemEditor.IOS_BUILD_PATH_APPSTORE);
            }

            if (_build_plug.HasCertificationFlag(PBuildPlug.PIOSCertificationFlag.AdHoc))
            {
                PlayerSettings.applicationIdentifier = packageName;
                GenericBuild(ref _build_plug, rootPath + PatchSystemEditor.IOS_BUILD_PATH_ADHOC);
            }

            if (_build_plug.HasCertificationFlag(PBuildPlug.PIOSCertificationFlag.Enterprise))
            {
                PlayerSettings.applicationIdentifier = _build_plug.iosEnterpriseCert;
                GenericBuild(ref _build_plug, rootPath + PatchSystemEditor.IOS_BUILD_PATH_ENTERPRISE);
            }
        }

        private static void GenericBuild(ref PBuildPlug _build_plug, string _export_path)
        {
            string svn_revision = Environment.GetEnvironmentVariable("SVN_REVISION");
            Debug.LogFormat("svn revision : {0}", svn_revision);

            string[] scenes = PBuildUtil.FindEnabledEditorScenes();
            BuildTarget buildTarget = Helper_PiecetonBuildPlug.GetBuildTarget(_build_plug);
            BuildOptions options = Helper_PiecetonBuildPlug.GetBuildOption(_build_plug);

            string plugPath = AssetDatabase.GetAssetPath(_build_plug);
            Debug.LogFormat("PiecetonPackageBuilder::GenericBuild() 111 is valid PBuildPlug = '{0}', Selection = '{1}'", null != _build_plug, Selection.activeObject);

            PiecetonEnvironmentVariable.SetSubBundlePath(_build_plug);

            Debug.Log(string.Format("[Build - {0}] start! path : {1}", buildTarget.ToString(), _export_path));
#if UNITY_5
            string res = BuildPipeline.BuildPlayer(scenes, _export_path, buildTarget, options);
            if (res.Length > 0)
            {
                throw new Exception(string.Format("[Build - {0}] failure! {1}", buildTarget.ToString(), res));
            }
#else
            BuildReport buildReport = BuildPipeline.BuildPlayer(scenes, _export_path, buildTarget, options);
            GenericBuildResult(buildReport);
#endif

            if (null == _build_plug)
            {
                Debug.LogFormat("PiecetonPackageBuilder::GenericBuild() Lost PBuildPlug instance so reload PBuildPlug. path = '{0}'", plugPath);
                _build_plug = AssetDatabase.LoadAssetAtPath<PBuildPlug>(plugPath);
                Selection.activeObject = _build_plug;
            }

            Debug.LogFormat("PiecetonPackageBuilder::GenericBuild() 222 is valid PBuildPlug = '{0}', Selection = '{1}'", null != _build_plug, Selection.activeObject);

            Debug.Log(string.Format("[Build - {0}] success! path : {1}", buildTarget.ToString(), _export_path));
        }

#if !UNITY_5
        private static void GenericBuildResult(BuildReport _report)
        {
            if (null == _report)
            {
                throw new Exception(string.Format("PiecetonPackegeBuilder::GenericBuildResult() Invalid BuildReport."));
            }

            BuildSummary summary = _report.summary;
            Debug.LogFormat("PiecetonPackegeBuilder::GenericBuildResult() result type = {0}", summary.result);

            if (summary.result != BuildResult.Succeeded)
            {
                string error = "";
                foreach (BuildStep step in _report.steps)
                {
                    error += step.messages + "\n";
                }

                throw new Exception(string.Format("PiecetonPackegeBuilder::GenericBuildResult() \n{0}", error));
            }
        }
#endif
    }
}