
[System.Flags]
public enum AssetBundleFlag
{
    None = 0,

    Downloading = 1 << 0,
    Operation = 1 << 1,
    AskRetry = 1 << 2,
    FailBundleWrite = 1 << 3,
    AskRetry_CRC = 1 << 4,
}

public enum FailDownload
{
    None = 0,
    UnknownError,
    WWWError,
    TimeOut,
    NotReachable,
    FailedWrite,
    InvalidPatchList,
    CRC,
}

public enum DownloadDataType
{
    File = 0,
    Bundle
}

public enum WWWDownloadError
{
    None = 0,
    AutoCRC_Retry,
    CRC_Error,
    CRC_Error_But_Write,
}