using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

namespace Pieceton.Misc
{
    public static class PFileGenerator
    {
        public static string MakeLine(int tabs, string code, int returnCount = 1)
        {
            string indent = "";
            for (int i = 0; i < tabs; i++)
            {
                indent += "\t";
            }
            string CRs = "";
            for (int i = 0; i < returnCount; i++)
            {
                CRs += "\n";
            }
            return indent + code + CRs;
        }

        public static bool WriteFile(string code, string root, string path)
        {
            bool success = false;

            if (false == string.IsNullOrEmpty(code))
            {
                CheckOrCreateDirectory(root);

                Debug.LogFormat("PFileGenerator::WriteFile() path='{0}'", path);

                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    writer.NewLine = "\n"; // mac 빌드오류수정
                    try
                    {
                        writer.WriteLine("{0}", code);
                        success = true;
                    }
                    catch (System.Exception ex)
                    {
                        string msg = " \n" + ex.ToString();
                        Debug.LogError(msg);
                        EditorUtility.DisplayDialog("Error when trying to regenerate file " + path, msg, "OK");
                    }
                }
            }

            return success;
        }

        private static void CheckOrCreateDirectory(string dir)
        {
            try
            {
                if (File.Exists(dir))
                {
                    Debug.LogWarning(dir + " is a file instead of a directory !");
                    return;
                }
                else if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(ex.Message);
                throw ex;
            }
        }
    }
}