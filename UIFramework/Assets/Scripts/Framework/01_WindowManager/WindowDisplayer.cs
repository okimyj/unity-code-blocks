using System.Collections.Generic;
using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Window
{
    public partial class WindowDisplayer : SingletonMonoDontDestroyBehaviour<WindowDisplayer>
    {
        private Transform m_holder;
        private Transform m_topHolder;
        private Dictionary<int, WindowBase> m_showingWindowMap = new Dictionary<int, WindowBase>();
        private WindowFactory m_windowFactory;

        public WindowDisplayer(WindowFactory windowFactory, Transform holder, Transform topHolder)
        {
            m_windowFactory = windowFactory;
            m_holder = holder;
            m_topHolder = topHolder;
        }
        public WindowBase ShowWindow(WinKey winKey, Callback closeCallback, object openWinParam)
        {
            var parent = winKey.IsTopWindow ? m_topHolder : m_holder;
            int showIdx = FindShowingWindowIndex(parent, winKey.Priority);
            WindowBase showingWindow = FindShowingWindow(winKey);
            if (showingWindow != null)
            {
                if (showIdx >= 0)
                {
                    // 이미 보여지고 있는 ui이기 때문에 본인 index 빼야됨.
                    showIdx -= 1;
                    showingWindow.transform.SetSiblingIndex(showIdx);
                }
                else
                    showingWindow.transform.SetAsLastSibling();
                return showingWindow;
            }

            
            WindowBase window = m_windowFactory.RentWindow(winKey, parent);
            if (window == null)
            {
                GameLogger.LogError($"WindowDisplayer.ShowWindow() : window is null. winKey : {winKey.ID}");
                return null;
            }

            window.SetCloseCallback(closeCallback);

            if (showIdx >= 0)
            {
                window.transform.SetSiblingIndex(showIdx);
            }
            else
            {
                window.transform.SetAsLastSibling();
            }

            m_showingWindowMap[winKey.ID] = window;

            OpenWindow(window, openWinParam);

            return window;
        }
        public void HideWindow(WinKey winKey)
        {
            WindowBase targetWindow = FindShowingWindow(winKey);
            if(targetWindow != null)
            {
                CloseWindow(targetWindow);
                m_showingWindowMap.Remove(winKey.ID);
                m_windowFactory.ReturnWindow(targetWindow);
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
            if (m_showingWindowMap.TryGetValue(winKey.ID, out var window))
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
