using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PJCall
{
    public static void CheckAndroid()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
    }

    public static void CheckIOS()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
    }

    public static void CheckWPA()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WSAPlayer);
    }
}