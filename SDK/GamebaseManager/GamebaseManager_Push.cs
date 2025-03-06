using System;
using Framework.Core;
using Framework.Utils;
using Toast.Gamebase;
#if UNITY_ANDROID
using UnityEngine;
using UnityEngine.Android;
#endif

public partial class GamebaseManager : SingletonMonoDontDestroyBehaviour<GamebaseManager>
{
    
    public bool HasPushNotificationPermission()
    {
#if UNITY_EDITOR
        return true;
#elif UNITY_ANDROID              
        return Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS");
#elif UNITY_IOS
        var notiSettings = Unity.Notifications.iOS.iOSNotificationCenter.GetNotificationSettings();
        return notiSettings.AuthorizationStatus == Unity.Notifications.iOS.AuthorizationStatus.Authorized;
#else
    return true;
#endif
    }
    
    public void RegisterPush(bool pushEnabled, bool adAgreement, bool adAgreementNight, bool requestPermission, Action<bool> callback)
    {
        GameLog.Log($"[GamebaeManager]RegisterPush - pushEnabled : {pushEnabled} adAgreement :{adAgreement} adAgreementNight : {adAgreementNight}");
        var configuration = new GamebaseRequest.Push.PushConfiguration() {
            pushEnabled = pushEnabled,
            adAgreement = adAgreement,
            adAgreementNight = adAgreementNight,
            requestNotificationPermission = requestPermission
        };
        var notificationOptions = new GamebaseRequest.Push.NotificationOptions() { badgeEnabled = true, foregroundEnabled = true };
        RegisterPushBody(configuration, notificationOptions, callback);
        /*
#if UNITY_ANDROID
        GameLog.Log($"[GamebaeManager]RegisterPush - androidVersion : {AndroidVersionCheck()}");
        if (Application.platform == RuntimePlatform.Android && AndroidVersionCheck() >= 33)
        {
            
            if (pushEnabled && !Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionGranted += (permissionName) =>
                {
                    GameLog.Log($"[GamebaeManager]RegisterPush - PermissionGranted.");
                    RegisterPushBody(configuration, notificationOptions, callback);
                };
                callbacks.PermissionDenied += (permissionName) =>
                {
                    GameLog.Log($"[GamebaeManager]RegisterPush - PermissionDenied.");
                    callback?.Invoke(false);
                };
                callbacks.PermissionDeniedAndDontAskAgain += (permissionName) =>
                {
                    GameLog.Log($"[GamebaeManager]RegisterPush - PermissionDeniedAndDontAskAgain.");
                    callback?.Invoke(false);
                };
                GameLog.Log($"[GamebaeManager]RegisterPush - ask request permission.");
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS", callbacks);
            }
            else
            {
                RegisterPushBody(configuration, notificationOptions, callback);
            }
        }
        else
        {
            // API 33 이하에서는 알림 권한을 요청하지 않고 바로 진행
            RegisterPushBody(configuration, notificationOptions, callback);
        }

#else
RegisterPushBody(configuration, notificationOptions, callback);
#endif
        */
    }

    int AndroidVersionCheck()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return version.GetStatic<int>("SDK_INT");
        }
#else
        return 0;
#endif
    }

    private void RegisterPushBody(GamebaseRequest.Push.PushConfiguration configuration, GamebaseRequest.Push.NotificationOptions notificationOptions, Action<bool> callback)
    {
        GameLog.Log($"[GamebaeManager]RegisterPushBody - configuration : {configuration} notificationOptions :{notificationOptions}");
        Gamebase.Push.RegisterPush(configuration, notificationOptions, (error) =>
                {
                    /*
                    var isSuccess = Gamebase.IsSuccess(error);
                    if (isSuccess)
                    {
                        GameOptionManager.Instance.SetInt(PlayerPrefs_Keys.EOptionKey.Notify_Contents, HasPushNotificationPermission() ? 1 : 0);
                    }
                    */
                });
        callback?.Invoke(true);
    }

}
