//using UnityEngine;
//using System.Collections;

////public enum UploadProtocol
////{
////    FTP = 0,
////    SFTP
////}

//public class DefUploadInfo
//{
//    //////////////////////////////////////////////////////////////////////////////
//    /// 패키지 업로드용 정보
//    private const string PACKAGE_URL = "192.168.10.91";       // "192.168.10.91";
//    private const string PACKAGE_ID = "ftpuser";        // "administrator";
//    private const string PACKAGE_PW = "vhfflrhs1!";        // "vhfflrhs1!";
//    private const string PACKAGE_ID_SND = "ftpuser";    // "administrator";
//    private const string PACKAGE_PW_SND = "vhfflrhs1!";    // "vhfflrhs1!";
//    //////////////////////////////////////////////////////////////////////////////


//    //////////////////////////////////////////////////////////////////////////////
//    // 패키지, 번들 백업용 정보
//    public const string BACKUP_URL = "192.168.10.91";        // "192.168.10.91";
//    private const string BACKUP_ID = "ftpuser";         // "administrator";
//    private const string BACKUP_PW = "vhfflrhs1!";         // "vhfflrhs1!";
//    private const string BACKUP_ID_SND = "ftpuser";         // "administrator";
//    private const string BACKUP_PW_SND = "vhfflrhs1!";         // "vhfflrhs1!";

//    private const string DEV_CDN_URL = "192.168.10.91";       // "192.168.10.91";
//    private const string DEV_CDN_ID = "ftpuser";        // "administrator";
//    private const string DEV_CDN_PW = "vhfflrhs1!";        // "vhfflrhs1!";
//    private const string DEV_CDN_ID_SND = "ftpuser";    // "administrator";
//    private const string DEV_CDN_PW_SND = "vhfflrhs1!";    // "vhfflrhs1!";
//    //////////////////////////////////////////////////////////////////////////////


//    //////////////////////////////////////////////////////////////////////////////
//    // 정식 배포용 CDN
//    private const string CDN_URL = "133.186.134.79";           // "192.168.10.91";  "sfile.com2us.com";
//    private const string CDN_ID = "root";                   // "administrator";
//    private const string CDN_PW = "marble-keypairs.pem";            // "vhfflrhs1!";
//    //////////////////////////////////////////////////////////////////////////////


//    public static UploadInfo packageInfo    = new UploadInfo(PUploadProtocolType.FTP, PACKAGE_URL, PACKAGE_ID, PACKAGE_PW);
//    public static UploadInfo packageInfoSnd = new UploadInfo(PUploadProtocolType.FTP, PACKAGE_URL, PACKAGE_ID_SND, PACKAGE_PW_SND);
//    public static UploadInfo backupInfo     = new UploadInfo(PUploadProtocolType.FTP, BACKUP_URL, BACKUP_ID, BACKUP_PW, true);
//    public static UploadInfo backupInfoSnd  = new UploadInfo(PUploadProtocolType.FTP, BACKUP_URL, BACKUP_ID_SND, BACKUP_PW_SND, true);
//    public static UploadInfo devCdnInfo     = new UploadInfo(PUploadProtocolType.FTP, DEV_CDN_URL, DEV_CDN_ID, DEV_CDN_PW);
//    public static UploadInfo devCdnInfoSnd  = new UploadInfo(PUploadProtocolType.FTP, DEV_CDN_URL, DEV_CDN_ID_SND, DEV_CDN_PW_SND);
//    public static UploadInfo cdnInfo        = new UploadInfo(PUploadProtocolType.SFTP, CDN_URL, CDN_ID, CDN_PW);
//}

//public class UploadInfo
//{
//    public Pieceton.PatchSystem.PUploadProtocolType protocol;

//    public string url;
//    public string id;
//    public string pw;

//    public bool isBackup { get; private set; }

//    public UploadInfo(PUploadProtocolType _protocol, string _url, string _id, string _pw, bool _is_bacup = false)
//    {
//        protocol = _protocol;
//        url = _url;
//        id = _id;
//        pw = _pw;

//        isBackup = _is_bacup;
//    }

//    public bool IsValid()
//    {
//        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
//            return false;

//        return true;
//    }
//}