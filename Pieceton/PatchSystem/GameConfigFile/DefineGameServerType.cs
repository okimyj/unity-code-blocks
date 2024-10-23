public enum GameServerType
{
    Unknown = -1,

    #region dont fix
    Dev,
    Dev_QA,
    Staging,

    IOS_TEST, // 검수서버
    #endregion

    // 게임마다 다르게 사용할 수 있는 부분
    Live,
    //Global,
    //GlobalZ,
    //Korea,
    //Japan,
    //China,
    //Asia,

    End
}