using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEditor;

using Pieceton.Misc;
using Pieceton.Configuration;
using Pieceton.BuildPlug;

namespace Pieceton.PatchSystem
{
    public class BundleBuilder
    {
        public static void BuildAssetBundleImpl(string _build_plug_name)
        {
            PBuildPlug buildPlug = PBuildPlug.LoadObject(_build_plug_name);
            PiecetonBundleBuilder.Build(buildPlug);
        }

        [MenuItem(PiecetonMenuEditor.ASSET_BUNDLE_MENU_SIMULATE)]
        private static void ToggleSimulateAssetBundle()
        {
            PAssetBundleSimulate.active = !PAssetBundleSimulate.active;
        }

        [MenuItem(PiecetonMenuEditor.ASSET_BUNDLE_MENU_SIMULATE, true)]
        private static bool ToggleSimulateAssetBundleValidate()
        {
            Menu.SetChecked(PiecetonMenuEditor.ASSET_BUNDLE_MENU_SIMULATE, PAssetBundleSimulate.active);
            return true;
        }
    }
}