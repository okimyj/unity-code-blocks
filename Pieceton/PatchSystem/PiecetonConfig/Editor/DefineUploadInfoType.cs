using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UploadInfoType
{
    Package = 0,
    PackageBackup,
    Bundle,
    BundleBackup,
    BundleAppend,
    BundleAppendBackup,

    End
}

public static class DefUploadInfoType
{
    public static bool IsBackupType(UploadInfoType _type)
    {
        switch (_type)
        {
            case UploadInfoType.PackageBackup:
            case UploadInfoType.BundleBackup:
            case UploadInfoType.BundleAppendBackup:
                return true;
        }

        return false;
    }
}