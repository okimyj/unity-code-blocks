using UIFramework.Core;
using UIFramework.Window;
using UnityEngine;

public class WindowBase : MonoBehaviour 
{ 
    public WinKey WinKey { get; protected set; }
    private WindowManager m_windowManager;
    private Callback m_closeCallback;

    public virtual void InitWindow(WinKey winKey, WindowManager windowManager)
    {
        WinKey = winKey;
        m_windowManager = windowManager;
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
        m_windowManager.HideWindow(WinKey);
    }
}