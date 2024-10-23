using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

using Pieceton.Configuration;
using Pieceton.Misc;
using Pieceton.BuildPlug;

namespace Pieceton.PatchSystem
{
    public static class PiecetonUnitySetter
    {

        public static void SwitchPlatform(PBuildPlug _build_plug)
        {
            BuildTarget buildTarget = Helper_PiecetonBuildPlug.GetBuildTarget(_build_plug);

            BuildTargetGroup group = PBuildUtil.BuildTargetToBuildTargetGroup(buildTarget);
            if (group == BuildTargetGroup.Unknown)
            {
                throw new Exception("[Builder] Invalid BuildTargetGroup.");
            }
            EditorUserBuildSettings.SwitchActiveBuildTarget(group, buildTarget);
        }
        public static void BuildSettings(string _build_plug_name) { BuildSettings(PBuildPlug.LoadObject(_build_plug_name)); }
        public static void BuildSettings(PBuildPlug _build_plug)
        {
            BuildTarget buildTarget = Helper_PiecetonBuildPlug.GetBuildTarget(_build_plug);
            BuildTargetGroup group = PBuildUtil.BuildTargetToBuildTargetGroup(buildTarget);
            List<string> symbols = Helper_PiecetonBuildPlug.GetDefineSymbols(_build_plug);

            PlayerSettings.companyName = PlugPreferenceInfo.instance.CompanyName;
            PlayerSettings.productName = PlugPreferenceInfo.instance.ProductName;

            SetPreDefineSymbol(_build_plug);

            PlayerSettings.applicationIdentifier = Helper_PiecetonBuildPlug.GetPackageName(_build_plug);
            PlayerSettings.bundleVersion = Helper_PiecetonBuildPlug.GetGameVersion(_build_plug);

            if (_build_plug.platform == PiecetonPlatform.Android)
            {
                PlayerSettings.Android.bundleVersionCode = int.Parse(Helper_PiecetonBuildPlug.GetVersionCode(_build_plug));
                PlayerSettings.Android.keystoreName = PlugPreferenceInfo.instance.KeystoreFullPath();
                PlayerSettings.Android.keystorePass = PlugPreferenceInfo.instance.keystorePass;
                PlayerSettings.Android.keyaliasName = PlugPreferenceInfo.instance.keyaliasName;
                PlayerSettings.Android.keyaliasPass = PlugPreferenceInfo.instance.keyaliasPass;

                EditorUserBuildSettings.androidBuildSystem = _build_plug.aos_BuildSystem;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = _build_plug.exportAndroidProject;
                EditorUserBuildSettings.androidBuildSubtarget = _build_plug.aos_TextureSubtarget;

                PlayerSettings.SetScriptingBackend(group, _build_plug.androidScriptBackend);
            }
            else if (_build_plug.platform == PiecetonPlatform.iOS)
            {
                PlayerSettings.iOS.buildNumber = Helper_PiecetonBuildPlug.GetVersionCode(_build_plug);

                PlayerSettings.iOS.applicationDisplayName = _build_plug.ios_AppDisplayName;
            }

            SwitchPlatform(_build_plug);
        }

        private static void SetPreDefineSymbol(PBuildPlug _build_plug)
        {
            BuildTarget buildTarget = Helper_PiecetonBuildPlug.GetBuildTarget(_build_plug);
            List<string> symbols = Helper_PiecetonBuildPlug.GetDefineSymbols(_build_plug);
            string releaseTypeSymbol = Helper_PiecetonBuildPlug.GetReleaseSymbol(_build_plug);
            string defineSymbols = PBuildUtil.AddDefineSymbols(releaseTypeSymbol, symbols);

            BuildTargetGroup group = PBuildUtil.BuildTargetToBuildTargetGroup(buildTarget);
            if (group == BuildTargetGroup.Unknown)
            {
                throw new Exception("[Builder] Invalid BuildTargetGroup.");
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defineSymbols);
            string settedSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            Debug.LogFormat("[Builder] Setted predefine symbol = '{0}'", settedSymbol);
        }
    }
}