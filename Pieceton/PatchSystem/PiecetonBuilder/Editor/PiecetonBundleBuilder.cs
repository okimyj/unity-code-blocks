using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

using Pieceton.Configuration;
using Pieceton.BuildPlug;

namespace Pieceton.PatchSystem
{
    public static class PiecetonBundleBuilder
    {
        public static void Build(PBuildPlug _build_plug)
        {
            if (null == _build_plug)
                throw new Exception("PiecetonBundleBuilder::Build(PBuildPlug) PBuildPlug is null.");

            BuildTarget buildTarget = Helper_PiecetonBuildPlug.GetBuildTarget(_build_plug);
            bool useSimulattePatch = _build_plug.HasBuildFlag(PBuildPlug.PBuildFlag.SimulatePatch);
            bool useInternalBundle = _build_plug.HasBuildFlag(PBuildPlug.PBuildFlag.StreamBundle);
            bool useDuplicateCheck = _build_plug.HasBundleCheckFlag(PBuildPlug.PBundleCheckFlag.Duplicate);

            string versionName = Helper_PiecetonBuildPlug.GetGameVersionName(_build_plug);

            Debug.LogFormat("PiecetonBundleBuilder::Build Target {0}", buildTarget);
            Begin(useDuplicateCheck);

            Selection.activeObject = _build_plug;
            string _out_path = PBundlePathEditor.BuildOutputPath(buildTarget, _build_plug.releaseType);

            if (!Directory.Exists(_out_path))
                Directory.CreateDirectory(_out_path);

            string plugPath = AssetDatabase.GetAssetPath(_build_plug);
            Debug.LogFormat("PiecetonBundleBuilder::Build() 111 is valid PBuildPlug = '{0}', Selection = '{1}'", null != _build_plug, Selection.activeObject);

            PiecetonEnvironmentVariable.SetSubBundlePath(_build_plug);

            BundleManifest manifest = new BundleManifest(buildTarget, _build_plug.releaseType);

            if (null == _build_plug)
            {
                Debug.LogFormat("PiecetonBundleBuilder::Build() Lost PBuildPlug instance so reload PBuildPlug. path = '{0}'", plugPath);
                _build_plug = AssetDatabase.LoadAssetAtPath<PBuildPlug>(plugPath);
                Selection.activeObject = _build_plug;
            }

            End(manifest);

            PatchHandlerEditor.Make(manifest, buildTarget, _build_plug.releaseType, versionName);

            if (useInternalBundle)
            {
                if (!StreamingBundleHandlerEditor.Make(manifest, buildTarget, _build_plug.releaseType))
                {
                    string errMsg = string.Format("PiecetonBundleBuilder::Build() Failed make streaming asset bundle.");
                    throw new Exception(errMsg);
                }
            }

            if (useSimulattePatch || useInternalBundle)
            {
                StreamingBundleHandlerEditor.CopyStreamPatchFiles(buildTarget, _build_plug.releaseType, versionName);
            }
        }

        private static void Begin(bool _check_duplicate)
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();

            if (!_check_duplicate)
                return;

            string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();

            if (null == bundleNames)
                return;

            if (BundlePreBuilder.HasDuplicatedAssetInBundle(bundleNames))
                return;

            if (BundlePreBuilder.HasIncludedDotInBundle(bundleNames))
                return;
        }

        private static void End(BundleManifest _manifest)
        {
            if (null == _manifest)
                throw new NullReferenceException("BundleManifest is null");

            if (BundlePostBuilder.ExistInvalidBundleVariantName(_manifest))
                return;
        }
    }
}