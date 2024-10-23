using System.IO;
using System;
using SimpleJSON;
using UnityEngine;

// Wrapping static class of SimpleJson.
// http://wiki.unity3d.com/index.php/SimpleJSON

namespace Pieceton.Misc
{
    public static class Helper_SimpleJson
    {
        public static string SaveToFileBase64(JSONNode _node, string _file_path, bool _fail_to_crash = true)
        {
            string err = string.Empty;

            try
            {
                Directory.CreateDirectory((new FileInfo(_file_path)).Directory.FullName);
                using (StreamWriter _stream = new StreamWriter(File.Open(_file_path, FileMode.Create)))
                {
                    string _result = _node.SaveToBase64();
                    _stream.Write(_result);
                    _stream.Close();
                    return _result;
                }
            }
            catch (Exception e)
            {
                err = string.Format("[Helper_SimpleJson] SaveToFileBase64(). file = '{0}' \nerror = '{1}'", _file_path, e);
            }

            if (_fail_to_crash)
            {
                throw new Exception(err);
            }

            Debug.LogError(err);

            return string.Empty;
        }

        public static void SaveToJson(JSONNode _node, string _file_path, bool _fail_to_crash = true)
        {
            string err = string.Empty;

            try
            {
                Directory.CreateDirectory((new FileInfo(_file_path)).Directory.FullName);
                using (StreamWriter _stream = new StreamWriter(File.Open(_file_path, FileMode.Create)))
                {
                    string _result = _node.ToString(4);
                    _stream.Write(_result);
                    _stream.Close();
                }

                return;
            }
            catch (Exception e)
            {
                err = string.Format("[Helper_SimpleJson] SaveToJson(). file = '{0}' \nerror = '{1}'", _file_path, e);
            }

            if (_fail_to_crash)
            {
                throw new Exception(err);
            }

            Debug.LogError(err);
        }
    }
}