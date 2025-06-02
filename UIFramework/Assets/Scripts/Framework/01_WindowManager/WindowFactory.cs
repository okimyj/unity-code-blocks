using System;
using System.Collections.Generic;
using UIFramework.Core;
using UIFramework.Resource;
using UnityEngine;

namespace UIFramework.Window
{
    public class WindowFactory
    {
        private Dictionary<int, WindowBase> m_allCachedWindow = new Dictionary<int, WindowBase>();
        private WindowManager m_windowManager;
        private Transform m_holder;
        public WindowFactory(WindowManager windowManager, Transform holder)
        {
            m_windowManager = windowManager;
            m_holder = holder;
        }
        public WindowBase CreateWindow(WinKey winKey, Transform parent)
        {
            var prefab = ResourceManager.Instance.LoadAsset<GameObject>(winKey.ResourceKey, winKey.PrefabName);

            if (prefab != null)
            {
                var goWindow = GameObject.Instantiate(prefab, parent);
                goWindow.transform.localScale = Vector3.one;
                goWindow.layer = LayerMask.NameToLayer(CONSTANTS.LAYER_UI);
                var window = goWindow.GetComponent<WindowBase>();
                if (window != null)
                {
                    window.InitWindow(winKey, m_windowManager);
                    window.gameObject.SetActive(false);
                    return window;
                }

                GameLogger.LogError("WindowFactory.CreateWindow -- Not Found WindowBase Component : " + winKey.PrefabName);
                return null;
            }

            GameLogger.LogError("WindowFactory.CreateWindow -- Could not load a gameobject  : " + winKey.PrefabName);
            return null;
        }
        public void PrecacheWindow(WinKey winKey)
        {
            if (!m_allCachedWindow.ContainsKey(winKey.ID))
            {
                WindowBase window = CreateWindow(winKey, this.m_holder);
                m_allCachedWindow[winKey.ID] = window;
            }
        }

        public void RemoveCachedWindow(WinKey winKey)
        {
            if (m_allCachedWindow.ContainsKey(winKey.ID))
            {
                GameObject.Destroy(m_allCachedWindow[winKey.ID].gameObject);
                m_allCachedWindow.Remove(winKey.ID);
            }
        }
        public void RemoveNotShowingWindow(Func<WinKey, bool> isShowing)
        {
            var windows = m_allCachedWindow.Values;
            var removeWinKeyIds = new List<int>();
            foreach (var win in windows)
            {
                if (!isShowing(win.WinKey))
                {
                    removeWinKeyIds.Add(win.WinKey.ID);
                    GameObject.Destroy(win.gameObject);
                }
            }
            for (int i = 0; i < removeWinKeyIds.Count; ++i)
            {
                m_allCachedWindow.Remove(removeWinKeyIds[i]);
            }
        }

        public WindowBase RentWindow(WinKey winKey, Transform parent)
        {
            if (!m_allCachedWindow.TryGetValue(winKey.ID, out var window))
            {
                window = CreateWindow(winKey, m_holder);
            }
            if(window != null)
            {
                m_allCachedWindow[winKey.ID] = window;
                window.transform.SetParent(parent);
            }

            return window;
        }

        public void ReturnWindow(WindowBase window)
        {
            window.SetCloseCallback(null);
            window.transform.SetParent(m_holder);
        }

        public WindowBase GetWindow(WinKey winKey)
        {
            if (m_allCachedWindow.TryGetValue(winKey.ID, out var window))
            {
                return window;
            }
            return null;
        }
    }
}
