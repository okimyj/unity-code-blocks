using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pieceton.BuildPlug;
using Pieceton.Misc;
using Pieceton.PatchSystem;

public class PlugPreferenceInitializer
{
    public static void SetDefault(PlugPreferenceInfo _info)
    {
        if (null == _info)
        {
            return;
        }

        Init_ProductInfo(_info);
        Init_Version(_info);
        Init_AndroidCertificate(_info);
        Init_UploadUrl(_info);
    }

    public static void CheckValid(PlugPreferenceInfo _info)
    {
        if (null == _info)
            return;

        Init_UploadUrl(_info);
    }

    private static void Init_ProductInfo(PlugPreferenceInfo _info)
    {
        if (string.IsNullOrEmpty(_info.CompanyName))
        {
            _info.CompanyName = PlayerSettings.companyName;
        }

        if (string.IsNullOrEmpty(_info.ProductName))
        {
            _info.ProductName = PlayerSettings.productName;
        }

        string editorApplicationIdentifier = PlayerSettings.applicationIdentifier;

        PiecetonPlatform p = PiecetonPlatform.None + 1;
        for (; p < PiecetonPlatform.End; ++p)
        {
            int i = (int)p;

            string packageName = "";

            if (!string.IsNullOrEmpty(editorApplicationIdentifier))
            {
                
                if (string.IsNullOrEmpty(_info.PackageName[i]))
                {
                    BuildTarget target = Helper_PiecetonBuildPlug.GetBuildTarget(p);
                    switch (target)
                    {
                        case BuildTarget.Android:   packageName = PlayerSettings.applicationIdentifier; break;
                        case BuildTarget.iOS:       packageName = PlayerSettings.applicationIdentifier; break;
                        default: PLog.AnyLogError("PlugPreferenceInitializer::Init_ProductInfo() Not implementation. {0}", p); break;
                    }
                }
            }

            _info.PackageName[i] = packageName;
        }
    }

    private static void Init_Version(PlugPreferenceInfo _info)
    {
        int count = _info.gameVersion.Length;
        for (int i = 0; i < count; ++i)
        {
            _info.gameVersion[i] = PlayerSettings.bundleVersion;
        }

        for (PiecetonPlatform i = PiecetonPlatform.None + 1; i < PiecetonPlatform.End; ++i)
        {
            if (!string.IsNullOrEmpty(_info.versionCode[(int)i]))
                continue;

            _info.versionCode[(int)i] = PlugPreferenceInfo.instance.versionCode[(int)i];

            switch (i)
            {
                case PiecetonPlatform.Android:
                    {
                        _info.versionCode[(int)i] = PlayerSettings.Android.bundleVersionCode.ToString();
                    }
                    break;

                case PiecetonPlatform.iOS:
                    {
                        _info.versionCode[(int)i] = PlayerSettings.iOS.buildNumber;
                    }
                    break;

                default:
                    {
                        PLog.AnyLogError("Undefined version code. PircetonPlatform = '{0}'", i);
                    }
                    break;
            }
        }
    }

    private static void Init_AndroidCertificate(PlugPreferenceInfo _info)
    {
        if (string.IsNullOrEmpty(_info.keystoreName))
        {
            string tmp = PlayerSettings.Android.keystoreName;
            if (!string.IsNullOrEmpty(tmp))
            {
                tmp = tmp.Replace(PBundlePathEditor.ProjectRoot(), "");
            }

            _info.keystoreName = tmp;
        }

        if (string.IsNullOrEmpty(_info.keystorePass))
        {
            _info.keystorePass = PlayerSettings.Android.keystorePass;
        }

        if (string.IsNullOrEmpty(_info.keyaliasName))
        {
            _info.keyaliasName = PlayerSettings.Android.keyaliasName;
        }

        if (string.IsNullOrEmpty(_info.keyaliasPass))
        {
            _info.keyaliasPass = PlayerSettings.Android.keyaliasPass;
        }
    }

    private static void Init_UploadUrl(PlugPreferenceInfo _info)
    {
        int curLencth = (int)PUploadProtocolType.End;

        if (null == _info.uploadUrl || _info.uploadUrl.Length != curLencth)
        {
            List<string> tmp = new List<string>(_info.uploadUrl);
            PUtility.Resize(tmp, curLencth);
            _info.uploadUrl = tmp.ToArray();
        }

        if (null == _info.uploadId || _info.uploadId.Length != curLencth)
        {
            List<string> tmp = new List<string>(_info.uploadId);
            PUtility.Resize(tmp, curLencth);
            _info.uploadId = tmp.ToArray();
        }

        if (null == _info.uploadPw || _info.uploadPw.Length != curLencth)
        {
            List<string> tmp = new List<string>(_info.uploadPw);
            PUtility.Resize(tmp, curLencth);
            _info.uploadPw = tmp.ToArray();
        }
    }
}