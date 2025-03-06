using Framework.Core;
using Toast.Gamebase;
using UnityEngine;
using RKClient.Game;

public partial class GamebaseManager : SingletonMonoDontDestroyBehaviour<GamebaseManager> 
{
    private void GamebaseEventHandler(GamebaseResponse.Event.GamebaseEventMessage message)
    {
        Debug.Log("[GamebaseManager] GamebaseEventHandler - message.category : " + message.category);
        switch (message.category)
        {
            case GamebaseEventCategory.SERVER_PUSH_APP_KICKOUT_MESSAGE_RECEIVED:        // Kickout ServerPush메세지를 받았을 때 
            case GamebaseEventCategory.SERVER_PUSH_APP_KICKOUT:             // Kickout ServerPush메세지를 받고 해당 팝업을 닫았을 때
            case GamebaseEventCategory.SERVER_PUSH_TRANSFER_KICKOUT:        // Guest 계정을 다른 단말기로 이전을 성공하게 되면 이전 단말기에서 킥아웃 메시지를 받게 됩니다.
                {
                    GamebaseResponse.Event.GamebaseEventServerPushData serverPushData = GamebaseResponse.Event.GamebaseEventServerPushData.From(message.data);
                    if (serverPushData != null)
                    {
                        GameManager.Instance.RestartApp();
                    }
                    break;
                }
            
            case GamebaseEventCategory.OBSERVER_HEARTBEAT:
                {
                    GamebaseResponse.Event.GamebaseEventObserverData observerData = GamebaseResponse.Event.GamebaseEventObserverData.From(message.data);
                    if (observerData != null)
                    {
                        GameManager.Instance.RestartApp();
                    }
                    break;
                }
            case GamebaseEventCategory.OBSERVER_LAUNCHING:
                {
                    ShowLaunchingInformationPopup();
                    break;
                }
        }
    }
}