using Framework.Core;
using System;
using Toast.Gamebase;

public partial class GamebaseManager : SingletonMonoDontDestroyBehaviour<GamebaseManager>
{
    public class GamebaseResult
    {
        public bool isSuccess;
        public GamebaseError gamebaseErr;
        public GamebaseResult() { }
        public GamebaseResult(GamebaseError error)
        {
            isSuccess = Gamebase.IsSuccess(error);
            gamebaseErr = error;
        }
    }
    public class GamebaseInitializeResult: GamebaseResult
    {
        public GamebaseResponse.Launching.LaunchingInfo launchingInfo;
        public GamebaseInitializeResult(GamebaseError error, GamebaseResponse.Launching.LaunchingInfo launchingInfo) : base(error)
        {
            this.launchingInfo = launchingInfo;
            GameLog.Log($"GamebaseInitializeResult - this.launchingInfo : {this.launchingInfo} code : {this.launchingInfo.launching.status.code}");
        }
    }
    public class GamebaseLoginResult : GamebaseResult
    {
        public GamebaseResponse.Auth.AuthToken authToken;
        public GamebaseLoginResult(GamebaseError error, GamebaseResponse.Auth.AuthToken authToken) : base(error)
        {
            this.authToken = authToken;
        }
    }
    public class GamebaseMappingResult : GamebaseResult
    {
        public string provider;
        public GamebaseMappingResult(GamebaseError error, string provider) : base(error)
        {
            this.provider = provider;
        }
    }
    public class CheckGamebaseMaintenanceResult
    {
        public bool isFinished;
        public DateTime endDate;
    }
    
}
