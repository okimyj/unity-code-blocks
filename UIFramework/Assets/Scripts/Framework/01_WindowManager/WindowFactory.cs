using System;
using System.Collections.Generic;
using UIFramework.Core;
using UIFramework.Resource;
using UnityEngine;

namespace UIFramework.Window
{
    public class WindowFactory
    {
        private Dictionary<int, WindowBase> allCachedWindow = new Dictionary<int, WindowBase>();
        private WindowManager windowManager;
        private Transform holder;
        public WindowFactory(WindowManager _windowManager, Transform _holder)
        {
            windowManager = _windowManager;
            holder = _holder;
        }
        public WindowBase CreateWindow(WinKey winKey, Transform parent)
        {
            GameObject _prefab = ResourceManager.Instance.LoadAsset<GameObject>(winKey.ResourceKey, winKey.PrefabName);

            if (_prefab != null)
            {
                GameObject goWindow = GameObject.Instantiate(_prefab, parent);
                goWindow.transform.localScale = Vector3.one;
                goWindow.layer = LayerMask.NameToLayer(CONSTANTS.LAYER_UI);
                WindowBase window = goWindow.GetComponent<WindowBase>();
                if (window != null)
                {
                    window.InitWindow(winKey, windowManager);
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
            if (!allCachedWindow.ContainsKey(winKey.ID))
            {
                WindowBase window = CreateWindow(winKey, this.holder);
                allCachedWindow[winKey.ID] = window;
            }
        }

        public void RemoveCachedWindow(WinKey winKey)
        {
            if (allCachedWindow.ContainsKey(winKey.ID))
            {
                GameObject.Destroy(allCachedWindow[winKey.ID].gameObject);
                allCachedWindow.Remove(winKey.ID);
            }
        }
        public void RemoveNotShowingWindow(Func<WinKey, bool> isShowing)
        {
            var windows = allCachedWindow.Values;
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
                allCachedWindow.Remove(removeWinKeyIds[i]);
            }
        }

        public WindowBase RentWindow(WinKey winKey, Transform parent)
        {
            if (!allCachedWindow.TryGetValue(winKey.ID, out var window))
            {
                window = CreateWindow(winKey, holder);
            }
            if(window != null)
            {
                allCachedWindow[winKey.ID] = window;
                window.transform.SetParent(parent);
            }

            return window;
        }

        public void ReturnWindow(WindowBase window)
        {
            window.SetCloseCallback(null);
            window.transform.SetParent(holder);
        }

        public WindowBase GetWindow(WinKey winKey)
        {
            if (allCachedWindow.TryGetValue(winKey.ID, out var window))
            {
                return window;
            }
            return null;
        }
    }
}
