using System;
using Framework.Core;
using Toast.Gamebase;
using UnityEngine;

public partial class GamebaseManager : SingletonMonoDontDestroyBehaviour<GamebaseManager> 
{
    
    public void ShowTermsView(Action<GamebaseResponse.Terms.ShowTermsViewResult> callback)
    {
        var configuration = new GamebaseRequest.Terms.GamebaseTermsConfiguration
        {
            forceShow = false
        };

        Gamebase.Terms.ShowTermsView(configuration, (data,error)=>{
            if (Gamebase.IsSuccess(error) == true)
            {
                Debug.Log("ShowTermsView succeeded.");
                
                GamebaseResponse.Terms.ShowTermsViewResult result = GamebaseResponse.Terms.ShowTermsViewResult.From(data);
                callback?.Invoke(result);
                
            }
            else
            {
                callback?.Invoke(null);
                Debug.Log(string.Format("ShowTermsView failed. error:{0}", error));
            }
        });
    }
}