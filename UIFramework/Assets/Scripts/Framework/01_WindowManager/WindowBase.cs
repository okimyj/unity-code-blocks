using UIFramework.Core;
using UIFramework.Window;
using UnityEngine;

public class WindowBase : MonoBehaviour 
{ 
    public WinKey WinKey { get; protected set; }
    protected WindowManager windowManager;
    private Callback closeCallback;

    public virtual void InitWindow(WinKey _winKey, WindowManager _windowManager)
    {
        WinKey = _winKey;
        windowManager = _windowManager;
    }
    public virtual void OnOpenWindow(object openWinParam) { }
    public virtual void OnCloseWindow() { }


    public void SetCloseCallback(Callback callback)
    {
        closeCallback = callback;
    }
    public void CallAndClearCloseCallback()
    {
        closeCallback?.Invoke();
        closeCallback = null;
    }
    
    protected void Close()
    {
        windowManager.HideWindow(WinKey);
    }
}