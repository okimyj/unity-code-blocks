/*
 * Window 시스템의 진입점이며 전체 생명 주기를 관리하는 클래스
 * 초기화 및 윈도우 생성, Open, Close 기능 제공
 * 내부적으로 WindowFactory, WindowDisplayer를 구성하고 호출
 */
using YJFramework.Core;
using UnityEngine;

namespace YJFramework.UI
{
    public partial class WindowManager : SingletonMonoDontDestroyBehaviour<WindowManager>
    {
        private bool _isInitialized = false;
        private WindowDisplayer _windowDisplayer;
        private WindowFactory _windowFactory;
        private void Initialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;
            _windowFactory = new WindowFactory(this, _holderInactiveWindows.transform);
            _windowDisplayer = new WindowDisplayer(_windowFactory, _holderActiveWindows.transform, _holderActiveTopWindows.transform);

            gameObject.layer = LayerMask.NameToLayer(CONSTANTS.LAYER_UI);
        }
        public WindowBase ShowWindow(WinKey key, Callback closeCallback, object openWinParam = null)
        {
            return _windowDisplayer.ShowWindow(key, closeCallback, openWinParam);
        }
        public void HideWindow(WinKey key)
        {
            _windowDisplayer.HideWindow(key);
        }
        public void PrecacheWindow(WinKey winKey)
        {
            _windowFactory.PrecacheWindow(winKey);
        }
        public void RemoveCachedWindow(WinKey winKey)
        {
            _windowDisplayer.HideWindow(winKey);
            _windowFactory.RemoveCachedWindow(winKey);
        }
    }

}
