using System;
using UnityEngine;
using System.Collections;
using System.Text;

namespace Pieceton.Misc
{
    public class PExceptionSender : MonoBehaviour
    {
        private const bool USE_EXCEPTION_LOG = false;

        private string webServerURL = ""; //"http://127.0.0.1:24750/WdbServer";

        private string uniqueID { get { return SystemInfo.deviceUniqueIdentifier; } }
        private string gameVersion { get { return Application.version; } }
        private string platform { get { return Application.platform.ToString(); } }

        public string userNick = "";
        public string releaseType = "";
        public string buildDate = "";

        private string _lastSendMsg = "";

        enum ESendType
        {
            Get,
            Post
        }
        private ESendType _sendType = ESendType.Post;

        enum EParamType
        {
            deviceId = 0,
            param1,
            param2,
            param3,
            param4,
            param5,
            info,
            End
        }

        private string GetParamValue(EParamType _pram)
        {
            switch (_pram)
            {
                case EParamType.deviceId: return uniqueID;
                case EParamType.param1: return EncodedString(releaseType);
                case EParamType.param2: return EncodedString(gameVersion);
                case EParamType.param3: return EncodedString(buildDate);
                case EParamType.param4: return EncodedString(platform);
                case EParamType.param5: return EncodedString(userNick);
                case EParamType.info: return EncodedString(_lastSendMsg);
            }

            return "";
        }

        public static void Create() { _instance = Instance; }

        public void SendLog(string _msg) { SendImpl(_msg); }

        #region instance
        private static PExceptionSender _instance = null;
        static PExceptionSender() { }
        private PExceptionSender() { }

        public static PExceptionSender Instance
        {
            get
            {
                if (null == _instance)
                {
                    GameObject go = new GameObject();
                    go.name = "PExceptionSender";
                    _instance = go.AddComponent<PExceptionSender>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        void Awake() { Application.logMessageReceived += OnLogEvent; }
        void OnDestroy() { Application.logMessageReceived -= OnLogEvent; }
        #endregion instance

        #region implementation
        private void OnLogEvent(string _condition, string _stack_trace, LogType _type)
        {
            if (!USE_EXCEPTION_LOG)
                return;

            if (_type != LogType.Exception)
                return;

            if (_condition.Contains("PExceptionSender.cs"))
                return;

            SendImpl(string.Format("{0}\n{1}", _condition, _stack_trace));
        }

        private void SendImpl(string _msg)
        {
            if (!USE_EXCEPTION_LOG)
                return;

            if (string.IsNullOrEmpty(webServerURL))
                return;

            if (_lastSendMsg.Equals(_msg))
                return;

            _lastSendMsg = _msg;

            const char RSLASH = '\u005C';
            string reMsg = _msg.Replace(RSLASH, '/');

            switch (_sendType)
            {
                case ESendType.Get: StartCoroutine(IEnumSend(new WWW(MakeSendString()))); break;
                case ESendType.Post: StartCoroutine(IEnumSend(new WWW(webServerURL, MakeSendForm()))); break;
            }
        }

        private string MakeSendString()
        {
            string sendMsg = webServerURL + "?";

            for (EParamType e = EParamType.deviceId; e < EParamType.End; ++e)
            {
                AddGetParam(ref sendMsg, e.ToString(), GetParamValue(e));
            }

            sendMsg = sendMsg.Substring(0, sendMsg.Length - 1);
            return sendMsg;
        }

        private WWWForm MakeSendForm()
        {
            WWWForm form = new WWWForm();

            for (EParamType e = EParamType.deviceId; e < EParamType.End; ++e)
            {
                AddPostParam(ref form, e.ToString(), GetParamValue(e));
            }

            return form;
        }

        private IEnumerator IEnumSend(WWW www)
        {
            yield return www;

            if (null != www.error)
            {
                Debug.LogErrorFormat("[PExceptionSender] failed send {0}", www.error);
            }
            else
            {
                Debug.Log("[PExceptionSender] success");
            }
        }

        private void AddGetParam(ref string _sendMsg, string _key, string _val)
        {
            _sendMsg = string.Format("{0}{1}={2}&", _sendMsg, _key, _val);
        }

        private void AddPostParam(ref WWWForm _form, string _key, string _val)
        {
            _form.AddField(_key, _val);
        }

        public static string EncodedString(string msg)
        {
            try
            {
                byte[] bytesToEncode = Encoding.UTF8.GetBytes(msg);
                string encodedText = Convert.ToBase64String(bytesToEncode);
                return encodedText;
            }
            catch { }

            return "";
        }

        public static string DecodedString(string encodedString)
        {
            try
            {
                byte[] bytesDecode = Convert.FromBase64String(encodedString);
                string decodedText = Encoding.UTF8.GetString(bytesDecode);
                return decodedText;
            }
            catch { }

            return "";
        }
        #endregion implementation
    }
}