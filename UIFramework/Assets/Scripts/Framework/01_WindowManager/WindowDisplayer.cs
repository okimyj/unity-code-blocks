using System.Collections.Generic;
using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Window
{
    public class WindowDisplayer : SingletonMonoDontDestroyBehaviour<WindowDisplayer>
    {
        private Transform _holder;
        private Transform _topHolder;
        private Dictionary<int, WindowBase> _showingWindowMap = new Dictionary<int, WindowBase>();
        private WindowFactory _windowFactory;

        public WindowDisplayer(WindowFactory windowFactory, Transform holder, Transform topHolder)
        {
            _windowFactory = windowFactory;
            _holder = holder;
            _topHolder = topHolder;
        }
        public WindowBase ShowWindow(WinKey winKey, Callback closeCallback, object openWinParam)
        {
            var parent = winKey.IsTopWindow ? _topHolder : _holder;
            int displayOrder = GetDisplayOrder(parent, winKey.Priority);
            WindowBase showingWindow = FindShowingWindow(winKey);
            if (showingWindow != null)
            {
                if (displayOrder >= 0)
                {
                    // 이미 보여지고 있는 ui이기 때문에 본인을 한번 뺀다.
                    displayOrder -= 1;
                    showingWindow.transform.SetSiblingIndex(displayOrder);
                }
                else
                    showingWindow.transform.SetAsLastSibling();
                return showingWindow;
            }

            
            WindowBase window = _windowFactory.RentWindow(winKey, parent);
            if (window == null)
            {
                GameLogger.LogError($"WindowDisplayer.ShowWindow() : window is null. winKey : {winKey.ID}");
                return null;
            }

            window.SetCloseCallback(closeCallback);

            if (displayOrder >= 0)
            {
                window.transform.SetSiblingIndex(displayOrder);
            }
            else
            {
                window.transform.SetAsLastSibling();
            }

            _showingWindowMap[winKey.ID] = window;

            OpenWindow(window, openWinParam);

            return window;
        }
        public void HideWindow(WinKey winKey)
        {
            WindowBase targetWindow = FindShowingWindow(winKey);
            if(targetWindow != null)
            {
                CloseWindow(targetWindow);
                _showingWindowMap.Remove(winKey.ID);
                _windowFactory.ReturnWindow(targetWindow);
            }
        }

        private void OpenWindow(WindowBase window, object openWinParam)
        {
            window.gameObject.SetActive(true);
            window.OnOpenWindow(openWinParam);
        }
        private void CloseWindow(WindowBase window)
        {
            window.CallAndClearCloseCallback();
            window.OnCloseWindow();
            window.gameObject.SetActive(false);
        }
        public WindowBase FindShowingWindow(WinKey winKey)
        {
            if (_showingWindowMap.TryGetValue(winKey.ID, out var window))
            {
                return window;
            }
            return null;
        }
        private int GetDisplayOrder(Transform parent, int displayPriority)
        {
            var index = -1;
            for (int i = parent.childCount - 1; i >= 0; --i)
            {
                WindowBase window = parent.GetChild(i).GetComponent<WindowBase>();
                if (window != null && window.WinKey.Priority > displayPriority)
                {
                    index = i;
                }
                else
                {
                    break;
                }
            }
            return index;
        }
    }

}
