using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;


namespace Pieceton.BuildPlug
{
    public partial class PBuildPlug
    {
        public enum PExecuteFlag
        {
            None = 0,

            BuildPackage = 1 << 0,
            BuildBundle = 1 << 1,
            UploadPackage = 1 << 2,
            UploadBundleOverride = 1 << 3,
            UploadBundleAppend = 1 << 4,
        }
        public void AddExecuteFlag(PExecuteFlag _flag) { executeFlag |= _flag; }
        public void DelExecuteFlag(PExecuteFlag _flag) { executeFlag &= ~_flag; }
        public bool HasExecuteFlag(PExecuteFlag _flag) { return (0 != (executeFlag & _flag)); }
        public bool EmptyExecuteFlag() { return (executeFlag == PExecuteFlag.None); }
        public void ClearExecuteFlag() { executeFlag = PExecuteFlag.None; }


        public enum PBuildFlag
        {
            None = 0,

            Debug = 1 << 0,
            StreamBundle = 1 << 1,
            SimulatePatch = 1 << 2,
            First = 1 << 3,
        }
        public void AddBuildFlag(PBuildFlag _flag) { buildFlag |= _flag; }
        public void DelBuildFlag(PBuildFlag _flag) { buildFlag &= ~_flag; }
        public bool HasBuildFlag(PBuildFlag _flag) { return (0 != (buildFlag & _flag)); }
        public bool EmptyBuildFlag() { return (buildFlag == PBuildFlag.None); }
        public void ClearBuildFlag() { buildFlag = PBuildFlag.None; }


        public enum PBundleCheckFlag
        {
            None = 0,

            Duplicate = 1 << 0,
        }
        public void AddBundleCheckFlag(PBundleCheckFlag _flag) { bundleCheckFlag |= _flag; }
        public void DelBundleCheckFlag(PBundleCheckFlag _flag) { bundleCheckFlag &= ~_flag; }
        public bool HasBundleCheckFlag(PBundleCheckFlag _flag) { return (0 != (bundleCheckFlag & _flag)); }
        public bool EmptyBuildCheckFlag() { return (bundleCheckFlag == PBundleCheckFlag.None); }
        public void ClearBuildCheckFlag() { bundleCheckFlag = PBundleCheckFlag.None; }


        public enum PIOSCertificationFlag
        {
            None = 0,

            Development = 1 << 0,
            AppStore = 1 << 1,
            AdHoc = 1 << 2,
            Enterprise = 1 << 3,
        }
        public void AddCertificationFlag(PIOSCertificationFlag _flag) { ios_certificationFlag |= _flag; }
        public void DelCertificationFlag(PIOSCertificationFlag _flag) { ios_certificationFlag &= ~_flag; }
        public bool HasCertificationFlag(PIOSCertificationFlag _flag) { return (0 != (ios_certificationFlag & _flag)); }
        public bool EmptyCertificationFlag() { return (ios_certificationFlag == PIOSCertificationFlag.None); }
        public void ClearCertificationFlag() { ios_certificationFlag = PIOSCertificationFlag.None; }
        public string ExtensionName => (platform == PiecetonPlatform.Android) ? "apk" : "ipa";
        public string AchivePackageName => $"{PlayerSettings.productName}-{releaseType}-{platform}.{ExtensionName}";
    }
}