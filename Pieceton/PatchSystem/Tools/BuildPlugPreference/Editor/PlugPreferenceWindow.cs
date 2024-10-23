using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Pieceton.Configuration;
using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public partial class PlugPreferenceWindow : EditorWindow
    {
        private static PlugPreferenceWindow _instance = null;

        private const float MIN_WINDOW_WIDTH = 800f;
        private const float MIN_WINDOW_HEIGHT = 600f;

        private const float MENU_TAB_WDITH = 200f;
        private const float MENU_BUTTON_HEIGHT = 50f;
        private const float MENU_BUTTON_PADDING = 5f;
        private const float CONTENT_WINDOW_PADDING = 10f;

        private static int selectedMenuIndex = 0;
        private static string[] menuNames = null;

        private bool isNeedDirty = false;
        private void SetNeedDirty()
        {
            isNeedDirty = true;
        }

        [MenuItem(PiecetonMenuEditor.MENU_BUILD_PLUG_ROOT + "Preference")]
        public static void OpenWindow()
        {
            CreateWindow();
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        private static void CreateWindow()
        {
            if (null != _instance)
            {
                _instance.Close();
                _instance = null;
            }

            _instance = EditorWindow.GetWindow(typeof(PlugPreferenceWindow), false, "BuildPlug Preference") as PlugPreferenceWindow;
            _instance.minSize = new Vector2(MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);

            Rect pos = _instance.position;
            pos.x = Mathf.Max((Screen.currentResolution.width / 4) - (MIN_WINDOW_WIDTH / 2), 50f);
            pos.y = Mathf.Max((Screen.currentResolution.height / 2) - (MIN_WINDOW_HEIGHT / 2), 50f);
            pos.width = MIN_WINDOW_WIDTH;
            pos.height = MIN_WINDOW_HEIGHT;

            _instance.position = pos;

            selectedMenuIndex = (int)BuildPlugPreferenceType.ProductInfo;
            menuNames = PDataParser.GetEnumNames<BuildPlugPreferenceType>();

            PlugPreferenceInfo.Reload();
        }

        void Update()
        {
            if (null == _instance)
            {
                CreateWindow();
            }

            PlugPreferenceInfo.instance.Update();
            Repaint();
        }

        private void OnGUI()
        {
            if (null == _instance)
                return;

            float mx = 0f;
            float my = 0f;
            float mw = MENU_TAB_WDITH;
            float mh = position.height;
            GUILayout.BeginArea(new Rect(mx, my, mw, mh), GUI.skin.box);
            {
                float bx = MENU_BUTTON_PADDING;
                float by = MENU_BUTTON_PADDING;// (position.height / 2) - (menuButtonsHeight / 2);
                float bw = MENU_TAB_WDITH - (MENU_BUTTON_PADDING * 2);
                float bh = MENU_BUTTON_HEIGHT * menuNames.Length;

                int newIndex = GUI.SelectionGrid(new Rect(bx, by, bw, bh), selectedMenuIndex, menuNames, 1);
                if (selectedMenuIndex != newIndex)
                {
                    selectedMenuIndex = newIndex;
                    isNeedDirty = true;
                    GUI.FocusControl(null);
                }
            }
            GUILayout.EndArea();


            float cx = MENU_TAB_WDITH + CONTENT_WINDOW_PADDING;
            float cy = CONTENT_WINDOW_PADDING;
            float cw = position.width - (MENU_TAB_WDITH + (CONTENT_WINDOW_PADDING * 2));
            float ch = position.height - (CONTENT_WINDOW_PADDING * 2);
            GUILayout.BeginArea(new Rect(cx, cy, cw, ch));
            {
                BuildPlugPreferenceType type = (BuildPlugPreferenceType)selectedMenuIndex;
                switch (type)
                {
                    case BuildPlugPreferenceType.ProductInfo: Draw_ProductInfo(); break;
                    case BuildPlugPreferenceType.GameVersion: Draw_GameVersioin(); break;
                    case BuildPlugPreferenceType.Certificate: Draw_Certificate(); break;
                    case BuildPlugPreferenceType.Upload: Draw_UploadInfo(); break;
                    case BuildPlugPreferenceType.BundleDownload: Draw_BundleDownload(); break;
                    case BuildPlugPreferenceType.Etc: Draw_Etc(); break;
                }
            }
            GUILayout.EndArea();


            if (isNeedDirty)
            {
                isNeedDirty = false;
                PlugPreferenceInfo.instance.Save();
                EditorUtility.SetDirty(PlugPreferenceInfo.instance);
            }
        }
    }
}