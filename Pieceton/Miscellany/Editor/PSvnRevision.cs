using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

// 윈도우에는 리비전 얻어오는 테스트 성공
// mac에서는 테스트 되지 않았음

namespace Pieceton.Misc
{
    public class PSvnRevision
    {
        public static string GetAssetsRevision()
        {
            string svnInstallPath = "svn";  // 윈도우의 경우 PATH에 포함되어 있어야 한다.

            string seper = string.Format("{0}", Path.DirectorySeparatorChar);
            string curSvnPath = Directory.GetCurrentDirectory() + seper + "Assets";

            string revision = ParseSvnRevision(ExecuteSvnInfo(svnInstallPath, curSvnPath));
            Debug.LogFormat("svn revision.\n{0}", revision);

            return revision;
        }

        private static string ExecuteSvnInfo(string _svn_install_path, string _svn_path)
        {
            string result = "";
            string errorMsg = null;

            Debug.LogFormat("PSvnRevision:ExecuteSvnInfo() target svn = '{0}'", _svn_path);

            try
            {
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = _svn_install_path;
                    p.StartInfo.Arguments = "info " + _svn_path;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;

                    using (Process process = Process.Start(p.StartInfo))
                    {
                        result = process.StandardOutput.ReadToEnd();
                        Debug.LogFormat("Process result : {0}", result);

                        errorMsg = process.StandardError.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                errorMsg = string.Format("Failed svn revision. {0}", e);
            }

            if (false == string.IsNullOrEmpty(errorMsg))
            {
                Debug.LogError(errorMsg);
                //throw new Exception(errorMsg);
            }

            return result;
        }

        private static string ParseSvnRevision(string _svn_infos)
        {
            if (string.IsNullOrEmpty(_svn_infos))
                return "empty";
            
            string[] infos = _svn_infos.Split('\n');

            int count = infos.Length;
            if (count <= 0)
                return "length";

            string revision = "";

            for (int i = 0; i < count; ++i)
            {
                string info = infos[i].ToLower();
                if (info.Contains("last changed rev:"))
                {
                    int startPos = info.LastIndexOf(":") + 1;
                    revision = info.Substring(startPos);
                    revision = revision.Replace(" ", "");
                }
            }

            revision = revision.Replace("\n", "");
            revision = revision.Replace("\r", "");

            return revision;
        }
    }
}