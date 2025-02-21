using Framework.Core;
using Framework.UI;
using RKClient.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using Toast.Gamebase;
using UnityEditor;
using UnityEngine;

public partial class GamebaseManager : SingletonMonoDontDestroyBehaviour<GamebaseManager>
{
    private readonly WaitForSeconds STATUS_CHECK_WAIT = new WaitForSeconds(10f);
    private Coroutine launchingStatusCheckRoutine;
    public bool IsCanEnterGameStatus(int code)
    {
        Debug.Log("IsCanEnterGameStatus - code : " + code);
        return (code == GamebaseLaunchingStatus.IN_SERVICE
            || code == GamebaseLaunchingStatus.RECOMMEND_UPDATE
            || code == GamebaseLaunchingStatus.IN_SERVICE_BY_QA_WHITE_LIST
            || code == GamebaseLaunchingStatus.IN_TEST
            || code == GamebaseLaunchingStatus.IN_REVIEW
            || code == GamebaseLaunchingStatus.IN_BETA);
    }

    public void StartCheckLaunchingInformation()
    {
        StopCheckLaunchingInformation();
        launchingStatusCheckRoutine = StartCoroutine(CheckLaunchingInformationRoutine());
    }
    public void StopCheckLaunchingInformation()
    {
        if(launchingStatusCheckRoutine != null)
        {
            StopCoroutine(launchingStatusCheckRoutine);
            launchingStatusCheckRoutine = null;
        }
    }
    // 로그인 하기 전 까지 주기적으로 launching status를 체크한다.
    private IEnumerator CheckLaunchingInformationRoutine()
    {
        var statusCode = initializeResult.launchingInfo.launching.status.code;
        while (IsCanEnterGameStatus(statusCode))
        {
            GetLaunchingInformation((info) => {
                statusCode = info.launching.status.code;
            });
            if(IsCanEnterGameStatus(statusCode))
                yield return STATUS_CHECK_WAIT;
        }
        ShowLaunchingInformationPopup();
    }


    public void ShowLaunchingInformationPopup()
    {
        GetLaunchingInformation((info) => {
            var maintenanceResult = ConvertLaunchingInfoToMaintenanceResult(info.launching);
            if (maintenanceResult.isFinished)
                return;
            var code = info.launching.status.code;
            if (code == GamebaseLaunchingStatus.INSPECTING_SERVICE || code == GamebaseLaunchingStatus.INSPECTING_ALL_SERVICES)
            {
                if (info != null && info.launching != null && info.launching.maintenance != null)
                {
                    PopupMaintenanceNotice popup = null;
                    if (WindowMgr.Instance.IsShowingWindow(WinKeyBasic.PopupMaintenanceNotice))
                    {
                        popup = WindowMgr.Instance.GetWindow(WinKeyBasic.PopupMaintenanceNotice) as PopupMaintenanceNotice;
                    }
                    else
                    {
                        popup = WindowMgr.Instance.ShowWindow(WinKeyBasic.PopupMaintenanceNotice) as PopupMaintenanceNotice;
                    }
                    popup.UpdateUIData(new PopupMaintenanceNotice.UIParam() { endDate = maintenanceResult.endDate, checkStatusAction = CheckMaintenanceStatus });
                }
                else
                {
                    WindowMgr.Instance.ShowMessageBoxUseLocalKey("MSG_SERVER_ERROR_TITLE", "TOAST_LOST_CONNECT", "BUTTON_CONFIRM",
                        () =>
                        {
                            GameManager.Instance.RestartApp();
                        });
                }
            }
            else if (code == GamebaseLaunchingStatus.REQUIRE_UPDATE || code == GamebaseLaunchingStatus.TERMINATED_SERVICE)
            {
                IntroManager.Instance.ShowUpdateClientWindow(() => {
#if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
                });
            }
        });
    }
    public void GetLaunchingInformation(Action<GamebaseResponse.Launching.LaunchingInfo> callback)
    {
        if (!IsInitialized)
        {
            Initialize((initResult) => {
                callback?.Invoke(initResult.launchingInfo);
            });
        }
        else
        {
            callback?.Invoke(Gamebase.Launching.GetLaunchingInformations());
        }
    }
    private void CheckMaintenanceStatus(Action<CheckGamebaseMaintenanceResult> callback)
    {
        GetLaunchingInformation((info) => {
            callback?.Invoke(ConvertLaunchingInfoToMaintenanceResult(info.launching));
        });
    }
    private CheckGamebaseMaintenanceResult ConvertLaunchingInfoToMaintenanceResult(GamebaseResponse.Launching.LaunchingInfo.GamebaseLaunching info)
    {
        var isFinished = IsCanEnterGameStatus(info.status.code);
        DateTime endDate = DateTime.Now;
        if (info.maintenance != null)
        {
            endDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(info.maintenance.localEndDate);
        }
        GameLog.Log($"[Gamebasemanager] ConvertLaunchingInfoToMaintenanceResult - code : {info.status.code} endDate : {endDate}");
        return new CheckGamebaseMaintenanceResult() { isFinished = isFinished, endDate = endDate };
    }

}
