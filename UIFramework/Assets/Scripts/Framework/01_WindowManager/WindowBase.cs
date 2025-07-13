using YJFramework.Core;
using UnityEngine;
namespace YJFramework.UI
{
    public class WindowBase : MonoBehaviour
    {
        public WinKey WinKey { get; protected set; }
        private WindowManager _windowManager;
        private Callback _closeCallback;

        public virtual void InitWindow(WinKey winKey, WindowManager windowManager)
        {
            WinKey = winKey;
            _windowManager = windowManager;
        }
        public virtual void OnOpenWindow(object openWinParam) { }
        public virtual void OnCloseWindow() { }
        public void SetCloseCallback(Callback callback)
        {
            _closeCallback = callback;
        }
        public void CallAndClearCloseCallback()
        {
            _closeCallback?.Invoke();
            _closeCallback = null;
        }
        protected void Close()
        {
            _windowManager.HideWindow(WinKey);
            CallAndClearCloseCallback();
        }
    }
}
