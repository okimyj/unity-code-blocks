using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pieceton.Configuration;
using Pieceton.Misc;

public class CopyPython
{
    public static void CopyPackageUploader(string _src_root, string _file_name, string _dst_root)
    {
        CopyUploaderClass(_dst_root);
        PFileCopy.Copy(_src_root, _dst_root, _file_name, true);
    }
    public static void CopyBundleUploader(string _src_root, string _file_name, string _dst_root)
    {
        CopyUploaderClass(_dst_root);
        PFileCopy.Copy(_src_root, _dst_root, _file_name, true);
    }

    public static void CopyBundle(string _dst_root)
    {
        PFileCopy.Copy(PatchSystemEditor.UPLOAD_PYTHON_ORIGINAL_OLD_ROOT, _dst_root, DefPython.BUNDLE_APPEND, true);
        PFileCopy.Copy(PatchSystemEditor.UPLOAD_PYTHON_ORIGINAL_OLD_ROOT, _dst_root, DefPython.PATCH_LIST, true);
    }
    private static void CopyUploaderClass(string _dst_root)
    {
        // FUploaderClass Copy. 정리 필요. 2024-06-13. yajin kim.
        PFileCopy.CopyDirectory($"{PatchSystemEditor.UPLOAD_PYTHON_ORIGINAL_ROOT}/{PatchSystemEditor.UPLOAD_PYTHON_LIBRARY_FOLDER_NAME}", $"{_dst_root}/{PatchSystemEditor.UPLOAD_PYTHON_LIBRARY_FOLDER_NAME}", true);
    }
}