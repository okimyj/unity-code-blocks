using UnityEngine;
using System.Collections;

public class DefPython
{
    // 플랫폼별 파이선이 설치되어 있는 경로
    //public const string INSTALLED_ROOT_AOS = "C:\\Python27\\python.exe";
    //public const string INSTALLED_ROOT_IOS = "/usr/bin/python2.7";

    public const string REMOTE_ROOT_PRODUCT = "Product";
    public const string REMOTE_ROOT_BACKUP = "Backup";

    public const string REMOTE_ROOT_BUNDLE = "Bundle";

    public const string UPLOAD_PACKAGE = "upload_package.py";  // override
    public const string UPLOAD_FILE = "upload_file.py";        // override
    public const string UPLOAD_BUNDLE = "upload_bundle.py";    // override
    public const string APPEND_BUNDLE = "append_bundle.py";    // append

    //-- 이하 파이선 정리 되면 삭제될 얘들
    // 업로드 용도별 파이선 파일명
    public const string PACKAGE_SWITCH = "upload_package_switch.py";        // 원래 있던 패키지 파일 지우고 다시 업로드 (or 패키지 백업)

    public const string BUNDLE_SWITCH = "upload_bundle_switch.py";          // 원래 있던 번들 지우고 다시 업로드
    public const string BUNDLE_APPEND = "upload_bundle_to_cdn.py";          // 원래 있던 번들 유지하고 추가로 딜드된 번들 업로드

    public const string PATCH_LIST = "upload_patch_info.py";                // 번들 패치리스트, 컨픽 파일 업로드 (or 패치 파일 백업)
}