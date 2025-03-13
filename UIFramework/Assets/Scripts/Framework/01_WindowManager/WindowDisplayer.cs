using System.Collections.Generic;
using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Window
{
    public partial class WindowDisplayer : SingletonMonoDontDestroyBehaviour<WindowDisplayer>
    {
        private Transform holder;
        private Transform topHolder;
        private Dictionary<int, WindowBase> showingWindowMap = new Dictionary<int, WindowBase>();
        private WindowFactory windowFactory;

        public WindowDisplayer(WindowFactory _windowFactory, Transform _holder, Transform _topHolder)
        {
            windowFactory = _windowFactory;
            holder = _holder;
            topHolder = _topHolder;
        }
        public WindowBase ShowWindow(WinKey winKey, Callback closeCallback, object openWinParam)
        {
            WindowBase showingWindow = FindShowingWindow(winKey);
            if (showingWindow != null)
            {
                return showingWindow;
            }

            Transform parent = winKey.IsTopWindow ? topHolder : holder;

            var showIdx = FindShowingWindowIndex(parent, winKey.Priority);
            WindowBase window = windowFactory.RentWindow(winKey, parent);
            if (window == null)
            {
                GameLogger.LogError("WindowDisplayer.ShowWindow() : window is null");
                return null;
            }

            window.SetCloseCallback(closeCallback);

            if (showIdx >= 0)
                window.transform.SetSiblingIndex(showIdx);
            else
                window.transform.SetAsLastSibling();

            showingWindowMap[winKey.ID] = window;

            OpenWindow(window, openWinParam);

            return window;
        }
        public void HideWindow(WinKey winKey)
        {
            var targetWindow = FindShowingWindow(winKey);
            if(targetWindow != null)
            {
                CloseWindow(targetWindow);
                showingWindowMap.Remove(winKey.ID);
                windowFactory.ReturnWindow(targetWindow);
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
            if (showingWindowMap.TryGetValue(winKey.ID, out var window))
            {
                return window;
            }
            return null;
        }
        private int FindShowingWindowIndex(Transform parent, int displayPriority)
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
