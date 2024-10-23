using UnityEngine;
using System.Collections;
using System.IO;
using SimpleJSON;
using UnityEditor;

using Pieceton.Configuration;

namespace Pieceton.PatchSystem
{
    public class PatchListCheckFileMaker
    {
        public static void Make(BundleManifest _manifest, BuildTarget _build_target, string _version_name)
        {
            string[] _names = AssetDatabase.GetAllAssetBundleNames();
            if (null == _names)
                return;

            string seperChar = string.Format("{0}", Path.DirectorySeparatorChar);
            string _version = _version_name;//AppInfo.curGameVersionName;

            string _platform_folder = PBundlePathEditor.PlatformName(_build_target);
            string _path = PatchSystemEditor.ASSET_BUNDLE_ARCHIVE_PATH + _platform_folder;
            string _check_filename = _path + seperChar + "checkList.txt";// + _version + ".txt";

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            StreamWriter _writer = new StreamWriter(File.Open(_check_filename, FileMode.Create));
            if (null == _writer)
                return;

            _writer.WriteLine("[" + _version + "]");

            _writer.WriteLine("[Config]");

            JSONNode _node = ParseConfigJson(_writer);

            if (null != _node)
            {
                Append_Issue(_writer, _node);
                Append_GameServer(_writer, _node);
                Append_FixedGameServer(_writer, _node);
            }

            _writer.WriteLine("");
            _writer.WriteLine("");
            _writer.WriteLine("[AssetBundleList]");
            int count = _names.Length;
            for (int n = 0; n < count; ++n)
            {
                _writer.WriteLine(_manifest.GetHashFolderName(_names[n]) + "/" + _names[n]);
            }

            _writer.Close();
        }

        private static JSONNode ParseConfigJson(StreamWriter _writer)
        {
            if (null != _writer)
            {
                string _config_file_path = PatchSystemEditor.CONFIG_ORIGINAL_PATH + DefGameConfig.CONFIG_FILENAME;

                StreamReader _config_stream = new StreamReader(File.Open(_config_file_path, FileMode.Open));
                if (null != _config_stream)
                {
                    string _config_data = _config_stream.ReadToEnd();
                    _config_stream.Close();

                    return JSON.Parse(_config_data);
                }

                Append_Error(_writer, "Not found configFile = " + _config_file_path);
            }

            return null;
        }

        private static void Append_ConfigCheckList(StreamWriter _writer)
        {
            if (null == _writer)
                return;

            _writer.WriteLine("[Config]");

            string _config_file_path = PatchSystemEditor.CONFIG_ORIGINAL_PATH + DefGameConfig.CONFIG_FILENAME;
            StreamReader _config_stream = new StreamReader(File.Open(_config_file_path, FileMode.Open));
            if (null != _config_stream)
            {
                string _config_data = _config_stream.ReadToEnd();
                _config_stream.Close();

                JSONNode _node = JSON.Parse(_config_data);

                Append_Issue(_writer, _node);
                Append_GameServer(_writer, _node);
                Append_FixedGameServer(_writer, _node);
            }
            else
            {
                Append_Error(_writer, "Not found configFile = " + _config_file_path);
            }
        }

        private static void Append_Issue(StreamWriter _writer, JSONNode _node)
        {
            if (null == _writer || null == _node)
                return;

            _writer.WriteLine("- Issue -");

            string _issue_type = _node[GameConfigFile.KEY_ISSUE];

            JSONArray _array_issuetype = _node[GameConfigFile.KEY_ISSUE_TYPE].AsArray;
            if (null != _array_issuetype)
            {
                bool _is_valid_issue = false;
                int _issue_count = _array_issuetype.Count;
                for (int n = 0; n < _issue_count; ++n)
                {
                    string _cur_type = _array_issuetype[n][n.ToString()];
                    if (_issue_type == _cur_type)
                    {
                        _is_valid_issue = true;
                        break;
                    }
                }

                if (_is_valid_issue)
                {
                    _writer.WriteLine(_issue_type);
                }
                else
                {
                    Append_Error(_writer, "Unknown Issue = " + _issue_type);
                }
            }
            else
            {
                Append_Error(_writer, "not found issue_type");
            }
        }

        private static void Append_GameServer(StreamWriter _writer, JSONNode _node)
        {
            if (null == _writer || null == _node)
                return;

            _writer.WriteLine("");
            _writer.WriteLine("- Support gameServerName -");

            int _auth_server_count = 0;
            string[] _auth_server_names = ServerConfig.GetValidServerNames();

            if (null != _auth_server_names)
            {
                _auth_server_count = _auth_server_names.Length;
                for (int n = 0; n < _auth_server_count; ++n)
                {
                    _writer.WriteLine(_auth_server_names[n]);
                }
            }

            _writer.WriteLine("");
            _writer.WriteLine("- Register gameServer -");

            if (null != _auth_server_names)
            {
                for (int n = 0; n < _auth_server_count; ++n)
                {
                    string _auth_server_name = _auth_server_names[n];
                    string _auth_address = GetGameServerAddress(_node, _auth_server_name);
                    if (!string.IsNullOrEmpty(_auth_address))
                    {
                        _writer.WriteLine(_auth_server_name + " : " + _auth_address);
                    }
                }
            }
        }

        private static void Append_FixedGameServer(StreamWriter _writer, JSONNode _node)
        {
            if (null == _writer || null == _node)
                return;

            string _fixed_game_server = _node[GameConfigFile.KEY_FIXED_GAME_SERVER];
            if (!string.IsNullOrEmpty(_fixed_game_server))
            {
                string _auth_address = GetGameServerAddress(_node, _fixed_game_server);
                if (!string.IsNullOrEmpty(_auth_address))
                {
                    _writer.WriteLine("");
                    _writer.WriteLine("- Fixed GameServer -");
                    _writer.WriteLine(_fixed_game_server);
                }
                else
                {
                    Append_Error(_writer, "Unknown fixedGameServer = " + _fixed_game_server);
                }
            }
        }

        private static string GetGameServerAddress(JSONNode _node, string _auth_server_name)
        {
            if (null != _node && !string.IsNullOrEmpty(_auth_server_name))
            {
                return _node[GameConfigFile.KEY_AUTH_SERVER_LIST][_auth_server_name];
            }
            return null;
        }

        private static void Append_Error(StreamWriter _writer, string _error)
        {
            if (null != _writer)
            {
                _writer.WriteLine("");
                _writer.WriteLine("* Error *");
                _writer.WriteLine(_error);
                _writer.WriteLine("");

                Debug.LogError(_error);
            }
        }
    }
}