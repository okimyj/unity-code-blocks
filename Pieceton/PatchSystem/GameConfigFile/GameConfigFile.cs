using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

using Pieceton.Misc;

namespace Pieceton.PatchSystem
{
    public static class GameConfigFile
    {
        public const string KEY_ISSUE = "issue";
        public const string KEY_ISSUE_TYPE = "issue_type";
        public const string KEY_AUTH_SERVER_LIST = "auth_server";
        public const string KEY_FIXED_GAME_SERVER = "fixed_game_server";
        public const string KEY_SERVICE_TYPE = "service_type";
        public const string KEY_BANNER_IMAGE_URL = "banner_img_url";
        public const string KEY_INSPECTION_SERVER = "inspection_server";

        public static bool Parse(string _downloaded_config, out PatchIssue _issue_type, out bool _reDownloadAndParsing, out string _error)
        {
            _issue_type = PatchIssue.None;

            _reDownloadAndParsing = false;

            _error = "";

            JSONNode _node = ParseJSON(_downloaded_config, out _error);

            if (null == _node)
            {
                _reDownloadAndParsing = true;
                return false;
            }

            string _issue = _node[KEY_ISSUE];
            if (!PDataParser.IsDefinedEnum<PatchIssue>(_issue))
            {
                _error = "invalid issueType = " + _issue;
                if (Debug.isDebugBuild)
                    Debug.LogError(_error);

                return false;
            }

            _issue_type = PDataParser.ParseEnum<PatchIssue>(_issue, PatchIssue.InvaidData);
            if (_issue_type != PatchIssue.None)
            {
                _error = _issue_type.ToString();
                return false;
            }

            return ServerConfig.Init(_node);
        }

        private static JSONNode ParseJSON(string _downloaded_config, out string _error)
        {
            JSONNode jNode = null;
            _error = "";

            if (!string.IsNullOrEmpty(_downloaded_config))
            {
                try
                {
                    jNode = JSON.Parse(_downloaded_config);
                }
                catch (Exception e)
                {
                    jNode = null;
                    _error = e.Message;
                }
            }

            return jNode;
        }
    }
}