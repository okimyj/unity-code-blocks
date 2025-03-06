using System;
using System.Collections;
using Framework.Core;
using Framework.UI;
using Framework.Utils;
using RKClient.ClientCore;
using RKClient.Game;
using Toast.Gamebase;
using UnityEditor;
using UnityEngine;

public partial class GamebaseManager : SingletonMonoDontDestroyBehaviour<GamebaseManager>
{
    private Action<GamebaseLoginResult> loginCallback;
    public bool IsInitialized => initializeResult != null;
    public bool IsLoggedIn => Gamebase.GetUserID() != null;
    public string StoreCode { get; private set; }
    private GamebaseInitializeResult initializeResult = null;
    public void Initialize(Action<GamebaseInitializeResult> callback, bool force = false)
    {
        if (!force && IsInitialized)
        {
            GameLog.Log("GameBaseManager::Initialize - is already initialized.");
            callback(initializeResult);
            return;
        }
        GameLog.Log("GameBaseManager::Initialize - ");
#if DEVENV_DEV
        Gamebase.SetDebugMode(true);
#else
    Gamebase.SetDebugMode(false);
#endif

        /**
        * Gamebase Configuration.
        */
        var configuration = new GamebaseRequest.GamebaseConfiguration();
        configuration.appID = "3g2ovD3T";
        
        configuration.appVersion = GetGameVersion();
        
        configuration.displayLanguageCode = GetSystemLanguageCodeToGamebaseCode();
#if UNITY_ANDROID
        StoreCode = GamebaseStoreCode.GOOGLE;
#elif UNITY_IOS
        StoreCode = GamebaseStoreCode.APPSTORE;
#else
        StoreCode = GamebaseStoreCode.WINDOWS;
#endif
        configuration.storeCode = StoreCode;
        configuration.enablePopup = true;
        configuration.enableLaunchingStatusPopup = false; // 점검 팝업 커스텀 하려면 false.
        configuration.enableBanPopup = true;
        configuration.enableKickoutPopup = true;

#if DEVENV_DEV || DEVENV_QA
        Gamebase.Push.SetSandboxMode(true);
#else
        Gamebase.Push.SetSandboxMode(false);
#endif

        /**
        * Gamebase Initialize.
        */
        Gamebase.RemoveAllEventHandler();
        Gamebase.AddEventHandler(GamebaseEventHandler);
        Gamebase.Initialize(configuration, (launchingInfo, error) =>
        {
            if (Gamebase.IsSuccess(error) == true)
            {
                initializeResult = new GamebaseInitializeResult(error, launchingInfo);
                //Status information of game app version set in the Gamebase Unity SDK initialization.
                var status = launchingInfo.launching.status;
                
                GameLog.Log("GameBaseManager::Initialize succeeded. status code : " + status.code);
                
                // Game status code (e.g. Under maintenance, Update is required, Service has been terminated)
                // refer to GamebaseLaunchingStatus
                if (IsCanEnterGameStatus(status.code))
                {
                    if(status.code == GamebaseLaunchingStatus.RECOMMEND_UPDATE)
                    {
                        WindowMgr.Instance.ShowToastMessage("MSG_CLIENT_UPDATE_DESC", true);
                    }
                    callback?.Invoke(initializeResult);
                    // Service is now normally provided.
                }
                else
                {
                    callback?.Invoke(initializeResult);
                }
            }
            else
            {
                // Check the error code and handle the error appropriately.
                GameLog.LogError($"Initialization failed. error is [{error.code}] :: {error.message}");
                if(error.code == GamebaseErrorCode.SOCKET_ERROR || error.code == GamebaseErrorCode.SOCKET_RESPONSE_TIMEOUT)
                {
                    WindowMgr.Instance.ShowMessageBoxUseLocalKey("MSG_FAIL_TO_GAMEBASE_TITLE", "MSG_FAIL_TO_GAMEBASE_DESC", "BUTTON_CONFIRM", "BUTTON_CANCEL", 
                        () => { RetryInitialize(callback); }, 
                        () => { SceneLoader.LoadScene(SceneLoader.INTRO_SCENE_NAME); });
                }
                else if (error.code == GamebaseErrorCode.LAUNCHING_UNREGISTERED_CLIENT || error.code == GamebaseErrorCode.LAUNCHING_NOT_EXIST_CLIENT_ID)
                {
                    IntroManager.Instance.ShowUpdateClientWindow(() => {
#if UNITY_EDITOR
                        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
                    });
                }
                else
                {
                    var title = UIFunction.GetLocalizeStr("MSG_FAIL_TO_GAMEBASE_RESET_TITLE");
                    var message = UIFunction.GetLocalizeStr("MSG_FAIL_TO_GAMEBASE_RESET_DESC", new string[] { error.code.ToString() });
                    var btnStr = UIFunction.GetLocalizeStr("BUTTON_CONFIRM");
                    WindowMgr.Instance.ShowMessageBox(title, message, btnStr
                        , () => {
#if UNITY_EDITOR
                            EditorApplication.isPlaying = false;
#else
                            Application.Quit();
#endif
                    });
                }
                
            }
        });
        
    }
    

    #region Private Helper
    private void RetryInitialize(Action<GamebaseInitializeResult> callback)
    {
        Initialize(callback);
    }
    
    private string GetSystemLanguageCodeToGamebaseCode()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Korean:
                return GamebaseDisplayLanguageCode.Korean;
            case SystemLanguage.English:
                return GamebaseDisplayLanguageCode.English;
            case SystemLanguage.German:
                return GamebaseDisplayLanguageCode.German;
            case SystemLanguage.Spanish:
                return GamebaseDisplayLanguageCode.Spanish;
            case SystemLanguage.Finnish:
                return GamebaseDisplayLanguageCode.Finnish;
            case SystemLanguage.French:
                return GamebaseDisplayLanguageCode.French;
            case SystemLanguage.Indonesian:
                return GamebaseDisplayLanguageCode.Indonesian;
            case SystemLanguage.Italian:
                return GamebaseDisplayLanguageCode.Italian;
            case SystemLanguage.Japanese:
                return GamebaseDisplayLanguageCode.Japanese;
            case SystemLanguage.Portuguese:
                return GamebaseDisplayLanguageCode.Portuguese;
            case SystemLanguage.Russian:
                return GamebaseDisplayLanguageCode.Russian;
            case SystemLanguage.Thai:
                return GamebaseDisplayLanguageCode.Thai;
            case SystemLanguage.Vietnamese:
                return GamebaseDisplayLanguageCode.Vietnamese;
            case SystemLanguage.ChineseSimplified:
                return GamebaseDisplayLanguageCode.Chinese_Simplified;
            case SystemLanguage.ChineseTraditional:
                return GamebaseDisplayLanguageCode.Chinese_Traditional;
        }
        return GamebaseDisplayLanguageCode.English;
    }
    private static string GetGameVersion()
    {
        var format = "{0}";
#if DEVENV_DEV
        format = "DEV_{0}";
#elif DEVENV_QA
        format = "QA_{0}";
#endif
        return string.Format(format, ReleaseInfo.gameVersion);
    }
#endregion
}
