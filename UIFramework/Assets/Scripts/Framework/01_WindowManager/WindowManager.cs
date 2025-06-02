using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Window
{
    public partial class WindowManager : SingletonMonoDontDestroyBehaviour<WindowManager>
    {
        private bool m_isInitialized = false;
        private WindowDisplayer m_windowDisplayer;
        private WindowFactory m_windowFactory;
        private void Initialize()
        {
            if (m_isInitialized)
                return;
            m_isInitialized = true;
            m_windowFactory = new WindowFactory(this, m_holderInactiveWindows.transform);
            m_windowDisplayer = new WindowDisplayer(m_windowFactory, m_holderActiveWindows.transform, m_holderActiveTopWindows.transform);

            gameObject.layer = LayerMask.NameToLayer(CONSTANTS.LAYER_UI);
        }
        public WindowBase ShowWindow(WinKey key, Callback closeCallback, object openWinParam = null)
        {
            return m_windowDisplayer.ShowWindow(key, closeCallback, openWinParam);
        }

        public void HideWindow(WinKey key)
        {
            m_windowDisplayer.HideWindow(key);
        }
        
        public void PrecacheWindow(WinKey winKey)
        {
            m_windowFactory.PrecacheWindow(winKey);
        }
        public void RemoveCachedWindow(WinKey winKey)
        {
            m_windowDisplayer.HideWindow(winKey);
            m_windowFactory.RemoveCachedWindow(winKey);
        }
    }

}
