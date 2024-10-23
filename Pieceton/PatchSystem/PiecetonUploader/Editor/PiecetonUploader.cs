using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pieceton.Configuration;
using Pieceton.BuildPlug;

namespace Pieceton.PatchSystem
{
    public class PiecetonUploader
    {
        public static void UploadPackage(string _pieceton_object_name)
        {
            PBuildPlug objectInst = PBuildPlug.LoadObject(_pieceton_object_name);
            UploadPackage(objectInst);
        }
        public static void UploadPackage(PBuildPlug _build_plug)
        {
            PiecetonPackageUploader.Package(_build_plug);
        }


        public static void UploadBundleOverride(string _pieceton_object_name)
        {
            PBuildPlug objectInst = PBuildPlug.LoadObject(_pieceton_object_name);
            UploadBundleOverride(objectInst);
        }
        public static void UploadBundleOverride(PBuildPlug _build_plug)
        {
            PiecetonBundleUploader.BundleOverride(_build_plug);
        }


        public static void UploadBundleAppend(string _pieceton_object_name)
        {
            PBuildPlug objectInst = PBuildPlug.LoadObject(_pieceton_object_name);
            UploadBundleAppend(objectInst);
        }
        public static void UploadBundleAppend(PBuildPlug _build_plug)
        {
            PiecetonBundleUploader.BundleAppend(_build_plug);
        }


        private static void CopyPython(PBuildPlug _build_plug)
        {

        }
    }
}