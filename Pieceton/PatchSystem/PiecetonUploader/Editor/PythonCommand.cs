using UnityEngine;
using System.Collections;

public class PythonCommand
{
    //public string installed_python_root { get; private set; }   // 파이선 설치되어 있는 경로
    public string arguments;                                    // 파이선 으로 실행할 .py 파일과 파라메터

    public PythonCommand()
    {
//#if UNITY_EDITOR_WIN
//        installed_python_root = DefPython.INSTALLED_ROOT_AOS;
//#elif UNITY_EDITOR_OSX
//        installed_python_root = DefPython.INSTALLED_ROOT_IOS;
//#else
//        installed_python_root = "";
//#endif

        arguments = "";
    }
    
    public static PythonCommand Make(params string[] _args)
    {
        PythonCommand cmd = new PythonCommand();

        cmd.arguments = MakeArg(_args);
        Log(cmd.arguments);

        return cmd;
    }


    private static string MakeArg(params string[] _args)
    {
        if (null == _args)
            return "";

        int count = _args.Length;
        if (count <= 0)
            return "";

        string arg = "";

        for (int i = 0; i < count; ++i)
        {
            arg += _args[i];
            arg += " ";
        }

        arg = arg.Substring(0, arg.Length-1);

        return arg;
    }

    private static void Log(string _arguments)
    {
        Debug.Log("====== [Make Python argument] ======");
        Debug.Log(_arguments);
        Debug.Log("================================");
    }
}