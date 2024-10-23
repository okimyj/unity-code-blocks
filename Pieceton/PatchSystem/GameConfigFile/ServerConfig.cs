using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public static class ServerConfig
    {
        public static Dictionary<GameServerType, string> authServerURL { get { return _authServerURL; } }
        public static Dictionary<GameServerType, string> inspectionServerURL { get { return _inspectionServerURL; } }

        private static Dictionary<GameServerType, string> _authServerURL = new Dictionary<GameServerType, string>();
        private static Dictionary<GameServerType, string> _inspectionServerURL = new Dictionary<GameServerType, string>();

        public static GameServerType curGameServerType
        #region curGameServerType
        {
            set
            {
                if (_curGameServerType != value)
                {
                    _curGameServerType = value;
                    SaveLastServiceGameServer(_curGameServerType);
                }
            }

            get
            {
                if (_fixedGameServerType != GameServerType.Unknown)
                    return _fixedGameServerType;

#if DEVENV_DEV
                return GameServerType.Dev;
#elif DEVENV_QA
                return GameServerType.Dev_QA;
#elif DEVENV_STAGING
                return GameServerType.Staging;
#endif
                return _curGameServerType;
            }
        }
        private static GameServerType _curGameServerType = GameServerType.Unknown;
        #endregion

        public static GameServerType fixedServer
        #region fixedServer
        {
            get { return _fixedGameServerType; }
        }
        private static GameServerType _fixedGameServerType = GameServerType.Unknown;
        #endregion

        private const string KEY_LAST_SERVICE_GAME_SERVER_KEY = "KEY_LAST_SERVICE_GAME_SERVER_KEY";


        public static bool Init(JSONNode _node)
        {
            _authServerURL.Clear();
            _inspectionServerURL.Clear();
            _fixedGameServerType = GameServerType.Unknown;

            if (null == _node)
                return false;

            GetServerInfo(_node, GameConfigFile.KEY_AUTH_SERVER_LIST, _authServerURL);
            GetServerInfo(_node, GameConfigFile.KEY_INSPECTION_SERVER, _inspectionServerURL);

            string fixedServer = _node[GameConfigFile.KEY_FIXED_GAME_SERVER];
            _fixedGameServerType = PDataParser.ParseEnum<GameServerType>(fixedServer, GameServerType.Unknown);



            return true;
        }

        #region initialize
        private static void GetServerInfo(JSONNode _root, string _info_key, Dictionary<GameServerType, string> _result)
        {
            if (null == _root || string.IsNullOrEmpty(_info_key) || null == _result)
                return;

            _result.Clear();

            JSONNode node = _root[_info_key];
            if (null == node)
                return;

            string[] serNames = GetValidServerNames();
            if (null == serNames)
                return;

            int count = serNames.Length;
            for (int i = 0; i < count; ++i)
            {
                string key = serNames[i];
                string url = node[key];

                if (string.IsNullOrEmpty(url))
                    continue;

                GameServerType type = PDataParser.ParseEnum(key, GameServerType.Unknown);
                if (!_result.ContainsKey(type))
                    _result.Add(type, url);
            }
        }

        public static string[] GetValidServerNames()
        {
            List<string> nameList = new List<string>();

            GameServerType type = GameServerType.Unknown + 1;
            for (; type < GameServerType.End; ++type)
            {
                nameList.Add(type.ToString());
            }

            return nameList.ToArray();
        }
        #endregion initialize

        public static string CurAuthServerIP()
        {
            string ip;
            if (_authServerURL.TryGetValue(curGameServerType, out ip))
                return ip;

            return "";
        }

        public static bool HasLastServiceGameServer()
        {
            return IsServiceGameServerType(LoadLastServiceGameServer());
        }

        public static void SaveLastServiceGameServer(GameServerType _game_server_type)
        {
            GameServerType serviceGameServer = _game_server_type;

            if (!IsServiceGameServerType(_game_server_type))
            {
                serviceGameServer = GameServerType.Unknown;
            }

            PlayerPrefs.SetString(KEY_LAST_SERVICE_GAME_SERVER_KEY, serviceGameServer.ToString());
        }
        public static GameServerType LoadLastServiceGameServer()
        {
            string lastGameServer = PlayerPrefs.GetString(KEY_LAST_SERVICE_GAME_SERVER_KEY, "");

            return PDataParser.ParseEnum(lastGameServer, GameServerType.Unknown);
        }

        public static bool IsServiceGameServerType(GameServerType _type)
        {
            if (_type > GameServerType.IOS_TEST && _type < GameServerType.End)
                return true;

            return false;
        }

        public static bool IsCurrentServerInspection()
        {
            return IsInspectionServer(curGameServerType);
        }

        public static string GetCurrentServerInspectionURL()
        {
            return GetInspectionServerURL(curGameServerType);
        }

        public static bool IsInspectionServer(GameServerType _type)
        {
            return _inspectionServerURL.ContainsKey(_type);
        }

        public static string GetInspectionServerURL(GameServerType _type)
        {
            string url;
            if (_inspectionServerURL.TryGetValue(_type, out url))
                return url;

            return "";
        }
    }
}