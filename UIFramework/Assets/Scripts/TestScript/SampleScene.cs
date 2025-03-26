using System.Collections;
using System.Collections.Generic;
using UIFramework.Core;
using UIFramework.Window;
using UnityEngine;

public class SampleScene : MonoBehaviour
{

    private void Awake()
    {
        AllManagerInitialize();
    }
    private void Start()
    {
        WindowManager.Instance.ShowWindow(WinKeyBasic.MessageBox, null);
    }

    private void AllManagerInitialize()
    {
        WindowManager.CreateInstance();
    }
}
