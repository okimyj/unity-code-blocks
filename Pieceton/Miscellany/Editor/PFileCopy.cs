using System;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.WSA;

namespace Pieceton.Misc
{
    public static class PFileCopy
    {
        public static bool Copy(string _src_root, string _dst_root, string _name, bool _fail_to_exception = false)
        {
            return Copy(Path.Combine(_src_root, _name), Path.Combine(_dst_root, _name));
        }

        // ex
        // string _src_path = Application.dataPath + "/Movies/Opening.mp4";
        // string _dst_path = Application.streamingAssetsPath + "/Opening.mp4";
        public static bool Copy(string _src_path, string _dst_path, bool _fail_to_exception = false)
        {
            try
            {
                Debug.LogFormat("[PFileCopy] Try copy file!. {0} => {1}", _src_path, _dst_path);

                if (File.Exists(_src_path))
                {
                    if (File.Exists(_dst_path))
                    {
                        File.Delete(_dst_path);
                    }
                    var dst_dir = Path.GetDirectoryName(_dst_path);
                    if (!Directory.Exists(dst_dir))
                        Directory.CreateDirectory(dst_dir);

                    File.Copy(_src_path, _dst_path);
                    return true;
                }
                else
                {
                    Debug.LogErrorFormat("[PFileCopy] Not found srcFile. srcPath='{0}'", _src_path);
                }
            }
            catch (Exception e)
            {
                string msg = string.Format("[PFileCopy] error copy file.\n src = {0}\n dst = {1}\n err = {2}", _src_path, _dst_path, e);
                Debug.LogError(msg);

                if (_fail_to_exception)
                    throw new Exception(msg);
            }

            return false;
        }
        public static bool CopyDirectory(string _src_dir, string _dst_dir, bool _fail_to_exception = false)
        {
            try
            {
                Debug.LogFormat("[PFileCopy] Try copy directory!. {0} => {1}", _src_dir, _dst_dir);

                if (Directory.Exists(_src_dir))
                {
                    if (Directory.Exists(_dst_dir))
                    {
                        Directory.Delete(_dst_dir, true);
                    }
                    Directory.CreateDirectory(_dst_dir);

                    foreach (var file in Directory.GetFiles(_src_dir))
                    {
                        var name = Path.GetFileName(file);
                        var dst = Path.Combine(_dst_dir, name);
                        File.Copy(file, dst);
                    }

                    foreach (var dir in Directory.GetDirectories(_src_dir))
                    {
                        var name = Path.GetFileName(dir);
                        var dst = Path.Combine(_dst_dir, name);
                        CopyDirectory(dir, dst);
                    }

                    return true;
                }
                else
                {
                    Debug.LogErrorFormat("[PFileCopy] Not found srcDir. srcDir='{0}'", _src_dir);
                }
            }
            catch (Exception e)
            {
                string msg = string.Format("[PFileCopy] error copy directory.\n src = {0}\n dst = {1}\n err = {2}", _src_dir, _dst_dir, e);
                Debug.LogError(msg);

                if (_fail_to_exception)
                    throw new Exception(msg);
            }

            return false;
        }
    }
}