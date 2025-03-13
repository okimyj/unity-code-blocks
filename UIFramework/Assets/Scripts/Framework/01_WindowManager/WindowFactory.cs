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

                GameLogger.LogError("WindowInGameFactory.CreateWindow -- Not Found WindowBase Component : " + winKey.PrefabName);
                return null;
            }

            GameLogger.LogError("WindowInGameFactory.CreateWindow -- Could not load a gameobject  : " + winKey.PrefabName);
            return null;
        }
        public void PrecacheWindow(WinKey winKey)
        {
            if (!this.allCachedWindow.ContainsKey(winKey.ID))
            {
                WindowBase window = CreateWindow(winKey, this.holder);
                this.allCachedWindow.Add(winKey.ID, window);
            }
        }

        public void RemoveCachedWindow(WinKey winKey)
        {
            if (this.allCachedWindow.ContainsKey(winKey.ID))
            {
                GameObject.Destroy(allCachedWindow[winKey.ID].gameObject);
                this.allCachedWindow.Remove(winKey.ID);
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
            if (this.allCachedWindow.ContainsKey(winKey.ID))
            {
                WindowBase w = this.allCachedWindow[winKey.ID];
                if (w != null)
                {
                    w.transform.SetParent(parent);
                }
                return w;
            }
            else
            {
                WindowBase w = CreateWindow(winKey, this.holder);
                if (w != null)
                {
                    this.allCachedWindow.Add(winKey.ID, w);
                    w.transform.SetParent(parent);
                    return w;
                }
            }

            return null;
        }

        public void ReturnWindow(WindowBase window)
        {
            window.SetCloseCallback(null);
            window.transform.SetParent(this.holder);
        }

        public WindowBase GetWindow(WinKey winKey)
        {
            if (this.allCachedWindow.ContainsKey(winKey.ID))
            {
                return this.allCachedWindow[winKey.ID];
            }
            return null;
        }
    }
}
