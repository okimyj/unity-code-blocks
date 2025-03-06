using System;
using System.Collections.Generic;
using Framework.Core;
using Framework.Utils;
using Toast.Gamebase;
using UnityEngine;

public partial class GamebaseManager : SingletonMonoDontDestroyBehaviour<GamebaseManager>
{
    
    public bool IsMappingGoogle()
    {
        return IsMapping(GamebaseAuthProvider.GOOGLE);
    }
    public bool IsMappingAppleID()
    {
        return IsMapping(GamebaseAuthProvider.APPLEID);
    }
    public bool IsMappingGuest()
    {
        return !IsMappingGoogle() && !IsMappingAppleID();
    }

    public void AddMapping(string provider, Action<GamebaseMappingResult> callback)
    {
        Gamebase.AddMapping(provider, (authToken, error) =>
        {
            callback?.Invoke(new GamebaseMappingResult(error, provider));
        });
    }
    public void RemoveMapping(string provider, Action<GamebaseResult> callback)
    {
        Gamebase.RemoveMapping(provider, (error) => {
            callback?.Invoke(new GamebaseResult(error));
        });
    }
    public void ChangeLoginIdP(GamebaseError addMappingErr, Action<GamebaseResult> callback)
    {
        // guest로 로그인 되어있는 경우 해당 계정 탈퇴 처리.
        if (IsMappingGuest())
        {
            Withdraw((success) => { ChangeLoginIdpBody(addMappingErr, callback); });
        }
        else
        {
            ChangeLoginIdpBody(addMappingErr, callback);
        }
    }
    private void ChangeLoginIdpBody(GamebaseError addMappingErr, Action<GamebaseResult> callback)
    {
        // ForcingMappingTicket 클래스의 From() 메소드를 이용하여 ForcingMappingTicket 인스턴스를 얻습니다.
        GamebaseResponse.Auth.ForcingMappingTicket forcingMappingTicket = GamebaseResponse.Auth.ForcingMappingTicket.From(addMappingErr);

        // ForcingMappingTicket의 UserID로 로그인합니다.
        Gamebase.ChangeLogin(forcingMappingTicket, (authTokenForcibly, error) =>
        {
            callback?.Invoke(new GamebaseResult(error));
        });
    }
    public void AddMappingForcibly(GamebaseError addMappingErr, Action<GamebaseResult> callback)
    {
        // ForcingMappingTicket 클래스의 From() 메소드를 이용하여 ForcingMappingTicket 인스턴스를 얻습니다.
        GamebaseResponse.Auth.ForcingMappingTicket forcingMappingTicket = GamebaseResponse.Auth.ForcingMappingTicket.From(addMappingErr);

        // 강제 매핑을 시도합니다.
        Gamebase.AddMappingForcibly(forcingMappingTicket, (authTokenForcibly, error) =>
        {
            callback?.Invoke(new GamebaseResult(error));
        });
    }

    public void Withdraw(Action<GamebaseResult> callback)
    {
        Debug.Log("GameBaseManager::Withdraw - userID : " + Gamebase.GetUserID() + " token : " + Gamebase.GetAccessToken());
        Gamebase.Withdraw((error) =>
        {
            var result = new GamebaseResult(error);
            if (result.isSuccess)
                PlayerPrefsManager.SetString(PlayerPrefs_Keys.LAST_LOGIN_ID, "");
            callback?.Invoke(result);
        });
    }
    public void RequestWithDrawal(Action<GamebaseResult> callback)
    {
        Debug.Log("GameBaseManager::RequestWithDrawal - userID : " + Gamebase.GetUserID() + " token : " + Gamebase.GetAccessToken());
        Gamebase.TemporaryWithdrawal.RequestWithdrawal((data, error) =>
        {
            var result = new GamebaseResult(error);
            if (result.isSuccess)
                PlayerPrefsManager.SetString(PlayerPrefs_Keys.LAST_LOGIN_ID, "");
            // 탈퇴 후 메인에서 last login provider로 다시 로그인 시도를 한다. 여기서 로그아웃 해줌.
            Logout(() => {
                callback?.Invoke(result);
            });
        });
    }
    public void CancelWithdrawal(Action<GamebaseResult> callback)
    {
        Gamebase.TemporaryWithdrawal.CancelWithdrawal((error) =>
        {
            callback?.Invoke(new GamebaseResult(error));
        });
    }

    private bool IsMapping(string providerName)
    {
        List<string> mappingList = null;
#if !UNITY_EDITOR
        mappingList = Gamebase.GetAuthMappingList();
#endif
        if (mappingList != null && mappingList.Count > 0)
        {
            for (int i = 0; i < mappingList.Count; ++i)
            {
                if (mappingList[i] == providerName)
                    return true;
            }
        }
        return false;
    }
}
