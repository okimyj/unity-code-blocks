using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ReleaseType
{
    None = -1,

    Dev = 0,
    DevQA,
    Staging,
    Live,

    End
}

public static class DefReleaseType
{
    public static string GetReleaseSymbol(ReleaseType _release_type)
    {
        switch (_release_type)
        {
            case ReleaseType.Dev: return "DEVENV_DEV";
            case ReleaseType.DevQA: return "DEVENV_QA";
            case ReleaseType.Staging: return "DEVENV_STAGING";
            case ReleaseType.Live: return "RELEASE_LIVE";
        }

        return "DEVENV_DEV";
    }

    public static string GetReleaseFolderName(ReleaseType _release_type)
    {
        string releaseType = _release_type.ToString();
        return releaseType.Replace("RELEASE_", "");
    }

    public static List<string> MakeDefineSymbolList(List<string> _symbols)
    {
        if (null != _symbols)
        {
            List<string> symbolList = new List<string>();

            int count = _symbols.Count;
            for (int i = 0; i < count; ++i)
            {
                if (false == string.IsNullOrEmpty(_symbols[i]))
                    symbolList.Add(_symbols[i]);
            }

            return symbolList;
        }

        return null;
    }
}