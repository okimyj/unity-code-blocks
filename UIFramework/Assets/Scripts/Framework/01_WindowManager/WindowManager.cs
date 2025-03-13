using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Window
{
    public partial class WindowManager : SingletonMonoDontDestroyBehaviour<WindowManager>
    {
        private bool isInitialized = false;
        private WindowDisplayer windowDisplayer;
        private WindowFactory windowFactory;
        public void Initialize()
        {
            if (isInitialized)
                return;
            isInitialized = true;
            gameObject.layer = LayerMask.NameToLayer(CONSTANTS.LAYER_UI);
            windowFactory = new WindowFactory(this, holderInactiveWindows.transform);
            windowDisplayer = new WindowDisplayer(windowFactory, holderActiveWindows.transform, holderActiveTopWindows.transform);
        }
        public WindowBase ShowWindow(WinKey key, Callback closeCallback, object openWinParam = null)
        {
            return windowDisplayer.ShowWindow(key, closeCallback, openWinParam);
        }

        public void HideWindow(WinKey key)
        {
            windowDisplayer.HideWindow(key);
        }
        
        public void PrecacheWindow(WinKey winKey)
        {
            windowFactory.PrecacheWindow(winKey);
        }
        public void RemoveCachedWindow(WinKey winKey)
        {
            windowDisplayer.HideWindow(winKey);
            windowFactory.RemoveCachedWindow(winKey);
        }
    }

}
