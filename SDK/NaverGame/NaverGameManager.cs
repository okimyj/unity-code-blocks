using Framework.Core;
using Framework.UI;
using Framework.Utils;
using Toast.Gamebase;
using UnityEngine;

public class NaverGameManager : SingletonMonoDontDestroyBehaviour<NaverGameManager>
{
    private readonly string DISCORD_URL = "https://discord.gg/7e34SbtCrd";

    private const string IOS_URL_SCHEME = "getam-navergame-login";
    private const string IOS_APP_NAME = "겟앰프드키우기";

    private const string LOUNGE_ID = "GetAmped_Idle_Adventure";
    private const string CLIENT_ID = "xk5mId0chf3hSup_YP34";
    private const string CLIENT_SECRET = "o4liDJd8cx";
    private const int BOARD_ID_COMMUNITY = 0;     // 전체 board. 
    private const int BOARD_ID_NOTICE = 11;
    private bool isInitialized = false;
    private bool useSDK = false;

    public bool IsInitialized => isInitialized;
    public void Initialize()
    {
        // NaverGameSDK 내부에서 Gson을 사용하는것 같은데 네트워크에 연결되지 않은 상태에서 Initialize 하는 경우 오류가 발생한다.
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return;
        
        if (isInitialized)
            return;

#if !UNITY_EDITOR
        useSDK = GameOptionManager.Instance.CurrentLanguage.langCode == UIFunction.UILanguage.Korean;
#endif
        try
        {
            GameLog.Log("NaverGameManager::Initialize - ");
#if !UNITY_EDITOR && !SIMULATE_SERVER
            if (useSDK)
            {
                GLink.sharedInstance().init(LOUNGE_ID, CLIENT_ID, CLIENT_SECRET);
                // setAppName, setAppScheme은 iOS에서만 유효 --//
                GLink.sharedInstance().setAppName(IOS_APP_NAME);
                GLink.sharedInstance().setAppScheme(IOS_URL_SCHEME);
                // android sdk 33이상 navergame 측 버그로 인해 우선 screenshot 기능 disable.
                GLink.sharedInstance().setCanWriteFeedByScreenshot(false);
            }
            else
            {
                GLink.sharedInstance().setCanWriteFeedByScreenshot(false);
            }
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"NaverGameManager Exception :: {e}");
            throw e;
        }

        isInitialized = true;
    }
    public void ShowHomeBanner()
    {
        GameLog.Log("NaverGameManager::ShowHomeBanner - ");
        Initialize();
        if (useSDK)
            GLink.sharedInstance().executeHomeBanner();
    }
    public void ShowCommunityBoard()
    {
        ShowBoard(BOARD_ID_COMMUNITY);
    }
    public void ShowNoticeBoard()
    {
        ShowBoard(BOARD_ID_NOTICE);
    }
    private void ShowBoard(int boardId)
    {
        GameLog.Log("NaverGameManager::ShowBoard - ");
        if (useSDK)
            GLink.sharedInstance().executeBoard(boardId);
        else
            GamebaseManager.Instance.ShowWebView(DISCORD_URL, null);
    }

}
