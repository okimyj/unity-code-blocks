using System;
using Framework.Core;
#if !UNITY_EDITOR
using Framework.UI;
#endif
using Toast.Gamebase;
using UnityEngine;

public partial class GamebaseManager : SingletonMonoDontDestroyBehaviour<GamebaseManager>
{

    public void LoginForLastLoggedInProvider(Action<GamebaseLoginResult> callback)
    {
        loginCallback = callback;
        var provider = Gamebase.GetLastLoggedInProvider();
        if (string.IsNullOrEmpty(provider))
        {
            LoginCallback(new GamebaseError(GamebaseErrorCode.AUTH_UNKNOWN_ERROR), null);
        }
        Gamebase.LoginForLastLoggedInProvider(LoginResult);
    }
    public void Login(string providerName, Action<GamebaseLoginResult> callback)
    {
        loginCallback = callback;
        Gamebase.Login(providerName, LoginResult);
    }
    public void Logout(Action callback = null)
    {
        if (IsMappingGuest())
        {
            GameLog.Log("Guest Logout -> Withdraw.");
            Withdraw((success) => { callback?.Invoke(); });
            return;
        }
        Gamebase.Logout((error) =>
        {
            if(Gamebase.IsSuccess(error))
                callback?.Invoke();
            else
            {
#if UNITY_EDITOR
                callback?.Invoke();
#else
                GameLog.Log($"GamebaseManager::Logout - code : {error.code} message : {error.message}");
                WindowMgr.Instance.ShowToastMessage("TOAST_FAIL_TO_SIGNOUT", true);
#endif

            }
        });
    }
    
    private void LoginResult(GamebaseResponse.Auth.AuthToken authToken, GamebaseError error)
    {
        if (Gamebase.IsSuccess(error) == true)
        {
            LoginCallback(error, authToken);
        }
        else
        {
            if (!string.IsNullOrEmpty(Gamebase.GetAuthProviderAccessToken(Gamebase.GetLastLoggedInProvider())))
            {
                Debug.Log("GameBaseManager_Login::is already logged in.");
                // gamebase는 이미 로그인 되어있는 상황인데 gameserver쪽이 로그인 안되었을 수 있다. 
                LoginCallback(error, authToken);
                return;
            }
            if (error.code == (int)GamebaseErrorCode.SOCKET_ERROR || error.code == (int)GamebaseErrorCode.SOCKET_RESPONSE_TIMEOUT)
            {
                Debug.Log(string.Format("Retry LoginForLastLoggedInProvider or notify an error message to the user.: {0}", error.message));
            }
            LoginCallback(error, authToken);
        }
    }
    private void LoginCallback(GamebaseError error, GamebaseResponse.Auth.AuthToken authToken)
    {
        loginCallback?.Invoke(new GamebaseLoginResult(error, authToken));
        loginCallback = null;
    }

}
