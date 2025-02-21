using Framework.Core;
using System.Collections;
using System.Collections.Generic;
using Toast.Gamebase;
using Toast.Gamebase.Internal.Single.Communicator;
using UnityEngine;

public partial class GamebaseManager : SingletonMonoDontDestroyBehaviour<GamebaseManager>
{
    public void ShowImageNotice()
    {
        GameLog.Log("GamebaseManager::ShowImageNotice - ");
        Gamebase.ImageNotice.ShowImageNotices(null, (error) => {
            GameLog.Log("ImageNotice is closed.");
        });
    }
    public void ShowCSWebView()
    {
        Gamebase.Contact.OpenContact((error) =>
        {
            if (Gamebase.IsSuccess(error) == true)
            {
                // A user close the contact web view.
            }
            else if (error.code == GamebaseErrorCode.UI_CONTACT_FAIL_INVALID_URL)  // 6911
            {
                GameLog.LogError("GamebaseManager::ShowCSWebView - Please check the url field in the TOAST Gamebase Console.");
            }
            else if (error.code == GamebaseErrorCode.UI_CONTACT_FAIL_ANDROID_DUPLICATED_VIEW) // 6913
            {
                // The customer center web view is already opened.
            }
            else
            {
                // An error occur when opening the contact web view.
            }
            //Gamebase.ImageNotice.ShowImageNotices
        });
    }
}
