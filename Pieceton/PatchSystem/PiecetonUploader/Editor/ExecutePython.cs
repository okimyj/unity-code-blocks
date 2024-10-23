using System;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

using Pieceton.Misc;
using UnityEngine;

public static class ExecutePython
{
    public static void Execute(string _installed_python_path, PythonCommand _execute_info)
    {
        if (null == _execute_info)
        {
            throw new Exception("PythonCommand is null");
        }

        string errorMsg = null;

        try
        {
            PLog.AnyLog("ExecutePython command = {0}", _execute_info.arguments);

            using (Process p = new Process())
            {

                p.StartInfo.FileName = _installed_python_path;
                p.StartInfo.Arguments = _execute_info.arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;

                using (Process process = Process.Start(p.StartInfo))
                {
                    string result = process.StandardOutput.ReadToEnd();
                    Debug.LogFormat("Process result : {0}", result);
                    System.IO.File.WriteAllText($"{Application.dataPath}/../../python_execute_log.txt", result);
                    errorMsg = process.StandardError.ReadToEnd();
                }
            }
        }
        catch (Exception e)
        {
            errorMsg = string.Format("Failed {0} execute process. error='{1}'", _execute_info.arguments, e);
        }

        if (false == string.IsNullOrEmpty(errorMsg))
        {
            Debug.LogError(errorMsg);
            throw new Exception(errorMsg);
        }
    }

    // dont use. instead Execute(string _installed_python_path, PythonCommand _execute_info)
    public static void Execute_old(PythonCommand _execute_info)
    {
        PLog.AnyLogError("dont use. instead Execute(string _installed_python_path, PythonCommand _execute_info)");
        throw new Exception();
    }
}