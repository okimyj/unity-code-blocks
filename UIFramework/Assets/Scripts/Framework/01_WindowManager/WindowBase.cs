using UIFramework.Core;
using UIFramework.Window;
using UnityEngine;

public class WindowBase : MonoBehaviour 
{ 
    public WinKey WinKey { get; protected set; }
    protected WindowManager windowManager;
    private Callback m_closeCallback;

    public virtual void InitWindow(WinKey _winKey, WindowManager _windowManager)
    {
        WinKey = _winKey;
        windowManager = _windowManager;
    }
    public virtual void OnOpenWindow(object openWinParam) { }
    public virtual void OnCloseWindow() { }

    public void SetCloseCallback(Callback callback)
    {
        m_closeCallback = callback;
    }
    public void CallAndClearCloseCallback()
    {
        m_closeCallback?.Invoke();
        m_closeCallback = null;
    }
    
    protected void Close()
    {
        windowManager.HideWindow(WinKey);
    }
}