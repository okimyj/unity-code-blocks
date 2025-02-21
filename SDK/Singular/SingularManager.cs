using Framework.Core;
using Singular;
using Framework.Resource;
using System.Collections.Generic;

public class SingularManager : SingletonMonoDontDestroyBehaviour<SingularManager>
{
#if !UNITY_EDITOR && RELEASE_LIVE
    private readonly bool USE_SINGULAR = true;
#else
    private readonly bool USE_SINGULAR = false;
#endif
    public enum EventKey
    {
        GA_LOGIN_ALL = 0,
        GA_LOGIN_AOS,
        GA_LOGIN_IOS,
        GA_LOGIN_GUEST,
        GA_COMPLETE_FIRSTBUY,
        GA_COMPLETE_BUY,
        GA_COMPLETE_AD,
        GA_COMPLETE_TICKET,
        GA_COMPLETE_DIA,
        GA_TEST_LOG,
    }
    private const string objectName = "SingularSDKObject";
    private SingularSDK sdkInstance;

    public void Initialize()
    {
        if (sdkInstance != null)
            return;
        
        GameLog.Log("SingularManager::Initialize - ");

        // object의 이름이 SingularSDKObject 여야 SDK 가 제대로 작동한다.
        var sdkObject = ResourceManager.Instance.RentObject(ResourceKey.PROGRAMASSET, "SingularSDKObject");
        sdkInstance = sdkObject?.GetComponent<SingularSDK>()??null;
        if(sdkInstance == null)
        {
            GameLog.LogError("SingularManager::Initialize - SingularSDKObject is null.");
            return;
        }
        sdkObject.name = objectName;
        sdkObject.transform.SetParent(transform);
    }
    public void SendEvent(EventKey key)
    {
        SendEvent(ConvertStringKey(key));
    }
    public void SendEvent(EventKey key, string[] args)
    {
        SendEvent(ConvertStringKey(key), args);
    }
    public void SendEvent(string name)
    {
        if (!USE_SINGULAR)
            return;
        GameLog.Log("SingularManager::SendEvent - name " + name);
        Initialize();
        SingularSDK.Event(name);
    }
    public void SendEvent(string name, string[] args)
    {
        if (!USE_SINGULAR)
            return;
        GameLog.Log("SingularManager::SendEvent - name " + name);
        Initialize();
        SingularSDK.Event(name, args);
    }
    public void SendEvent(Dictionary<string, object> args, string name)
    {
        if (!USE_SINGULAR)
            return;
        GameLog.Log("SingularManager::SendEvent Dictionary - name " + name);
        Initialize();
        SingularSDK.Event(args, name);
    }
    public void SendRevenue(string currency, double amount)
    {
        if (!USE_SINGULAR)
            return;
        GameLog.Log($"SingularManager::SendRevenue - currency : {currency} amount : {amount}");
        Initialize();
        SingularSDK.CustomRevenue(ConvertStringKey(EventKey.GA_COMPLETE_BUY), currency, amount);
    }
    public void SendFirstRevenue(string currency, double amount)
    {
        if (!USE_SINGULAR)
            return;
        GameLog.Log($"SingularManager::SendFirstRevenue : currency : {currency} amount : {amount}");
        Initialize();
        SingularSDK.CustomRevenue(ConvertStringKey(EventKey.GA_COMPLETE_FIRSTBUY), currency, amount);
    }

    private string ConvertStringKey(EventKey key)
    {
        return key.ToString().ToLowerInvariant();
    }
    

    
}
