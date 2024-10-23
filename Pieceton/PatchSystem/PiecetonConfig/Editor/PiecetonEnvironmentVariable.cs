using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Pieceton.BuildPlug;
using Pieceton.PatchSystem;

namespace Pieceton.Configuration
{
    public static class PiecetonEnvironmentVariable
    {
        private const string BUNDLR_ARCHIVE_PATH = "BUNDLR_ARCHIVE_PATH";
        private const string BUNDLE_SUB_PATH = "BUNDLE_SUB_PATH";

        public static void SetSubBundlePath(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                throw new Exception("PiecetonEnvirenmentVariant::SetSubBundlePath() PBuildPlug is null.");


            BuildTarget buildTarget = Helper_PiecetonBuildPlug.GetBuildTarget(_build_plug);
            string versionName = Helper_PiecetonBuildPlug.GetVersionCode(_build_plug);
            string archivePath = PBundlePathEditor.MakeBundleArchivePath(buildTarget, versionName);
            Environment.SetEnvironmentVariable(BUNDLR_ARCHIVE_PATH, archivePath);


            string subPath = PBundlePathEditor.MakeCDNUrlSub(_build_plug);
            Environment.SetEnvironmentVariable(BUNDLE_SUB_PATH, subPath);
        }
    }
}