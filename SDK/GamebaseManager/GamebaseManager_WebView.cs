using Framework.Core;
using Toast.Gamebase;
using System;
using UnityEngine;

public partial class GamebaseManager : SingletonMonoDontDestroyBehaviour<GamebaseManager> 
{ 
    public void ShowWebView(string url, Action onClosedCallback, int orientation = GamebaseScreenOrientation.UNSPECIFIED)
    {
        GamebaseRequest.Webview.GamebaseWebViewConfiguration configuration = new GamebaseRequest.Webview.GamebaseWebViewConfiguration();
        configuration.orientation = orientation;
        configuration.colorR = 128;
        configuration.colorG = 128;
        configuration.colorB = 128;
        configuration.colorA = 255;
        configuration.isBackButtonVisible = true;
        Gamebase.Webview.ShowWebView(url, configuration, (error) => {
            onClosedCallback?.Invoke();
        });
    }
}