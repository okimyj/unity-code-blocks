using System;
using System.Collections.Generic;
using UIFramework.Core;
using UIFramework.Resource;
using UnityEngine;

namespace UIFramework.Window
{
    public class WindowFactory
    {
        private Dictionary<int, WindowBase> _allCachedWindow = new Dictionary<int, WindowBase>();
        private WindowManager _windowManager;
        private Transform _holder;
        public WindowFactory(WindowManager windowManager, Transform holder)
        {
            _windowManager = windowManager;
            _holder = holder;
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
                    window.InitWindow(winKey, _windowManager);
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
            if (!_allCachedWindow.ContainsKey(winKey.ID))
            {
                WindowBase window = CreateWindow(winKey, this._holder);
                _allCachedWindow[winKey.ID] = window;
            }
        }

        public void RemoveCachedWindow(WinKey winKey)
        {
            if (_allCachedWindow.ContainsKey(winKey.ID))
            {
                GameObject.Destroy(_allCachedWindow[winKey.ID].gameObject);
                _allCachedWindow.Remove(winKey.ID);
            }
        }
        public void RemoveNotShowingWindow(Func<WinKey, bool> isShowing)
        {
            var windows = _allCachedWindow.Values;
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
                _allCachedWindow.Remove(removeWinKeyIds[i]);
            }
        }

        public WindowBase RentWindow(WinKey winKey, Transform parent)
        {
            if (!_allCachedWindow.TryGetValue(winKey.ID, out var window))
            {
                window = CreateWindow(winKey, _holder);
            }
            if(window != null)
            {
                _allCachedWindow[winKey.ID] = window;
                window.transform.SetParent(parent);
            }

            return window;
        }

        public void ReturnWindow(WindowBase window)
        {
            window.SetCloseCallback(null);
            window.transform.SetParent(_holder);
        }

        public WindowBase GetWindow(WinKey winKey)
        {
            if (_allCachedWindow.TryGetValue(winKey.ID, out var window))
            {
                return window;
            }
            return null;
        }
    }
}
