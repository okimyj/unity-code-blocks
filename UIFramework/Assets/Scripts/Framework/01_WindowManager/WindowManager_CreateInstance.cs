using UIFramework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework.Window
{
    public partial class WindowManager : SingletonMonoDontDestroyBehaviour<WindowManager>
    {
        private readonly float SCREEN_WIDTH = 720f;
        private readonly float SCREEN_HEIGHT = 1280f;
        
        private readonly string WINDOW_NAME_ROOT = "[WINDOWS_ROOT]";
        private readonly string WINDOW_NAME_CACHE_ROOT = "[WINDOWS_CACHE]";
        private readonly string WINDOW_NAME_HOLDER = "[WINDOWS_HOLDER]";
        private readonly string WINDOW_NAME_TOP_HOLDER = "[WINDOWS_TOP_HOLDER]";
        private readonly string CANVAS_NAME = "[UICanvas]";

        private GameObject rootWindow;
        private GameObject holderInactiveWindows;
        private GameObject holderActiveWindows;
        private GameObject holderActiveTopWindows;
        public static void CreateInstance()
        {
            var _windowManager = WindowManager.Instance;
            _windowManager.CreateRootWindow();
            _windowManager.Initialize();
        }

        private void CreateRootWindow()
        {
            if (rootWindow != null)
                return;
            rootWindow = new GameObject(WINDOW_NAME_ROOT);
            rootWindow.layer = LayerMask.NameToLayer(CONSTANTS.LAYER_UI);

            var rootWindowTr = rootWindow.transform;
            rootWindowTr.localPosition = Vector3.zero;
            rootWindowTr.localScale = Vector3.one;
            rootWindowTr.localRotation = Quaternion.identity;
            rootWindowTr.SetParent(transform);

            var canvas = MakeRootCanvas(rootWindowTr);
            canvas.sortingOrder = 1;

            holderInactiveWindows = CreateHolder(WINDOW_NAME_CACHE_ROOT, canvas.transform);
            holderActiveWindows = CreateHolder(WINDOW_NAME_HOLDER, canvas.transform);
            holderActiveTopWindows = CreateHolder(WINDOW_NAME_TOP_HOLDER, canvas.transform);
        }

        private GameObject CreateHolder(string holderName, Transform parent)
        {
            var go = new GameObject(holderName);
            var rect = go.AddComponent<RectTransform>();
            rect.SetParent(parent);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
            go.layer = LayerMask.NameToLayer(CONSTANTS.LAYER_UI);
            return go;
        }
        private Canvas MakeRootCanvas(Transform parent)
        {
            var goCanvas = new GameObject(CANVAS_NAME);
            var canvas = goCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            goCanvas.AddComponent<GraphicRaycaster>();
            goCanvas.transform.SetParent(parent);
            goCanvas.transform.localPosition = Vector3.zero;
            goCanvas.layer = LayerMask.NameToLayer(CONSTANTS.LAYER_UI);

            CanvasScaler canvas_scaler = goCanvas.AddComponent<CanvasScaler>();
            canvas_scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvas_scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvas_scaler.referenceResolution = new Vector2(SCREEN_WIDTH, SCREEN_HEIGHT);
            canvas_scaler.matchWidthOrHeight = 1.0f;

            return canvas;
        }


    }

}
