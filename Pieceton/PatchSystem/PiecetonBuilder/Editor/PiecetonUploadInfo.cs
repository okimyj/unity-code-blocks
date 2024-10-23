using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pieceton.PatchSystem
{
    public class PUploadInfo
    {
        public bool use = false;

        public PUploadProtocolType protocol = PUploadProtocolType.FTP;

        public string url = "";
        public string id = "";
        public string pw = "";

        //public PUploadInfo(bool _use, UploadProtocol _protocol, string _url, string _id, string _pw)
        //{
        //    use = _use;
        //    protocol = _protocol;
        //    url = _url;
        //    id = _id;
        //    pw = _pw;
        //}

        public void Set(bool _use, PUploadProtocolType _protocol, string _url, string _id, string _pw)
        {
            use = _use;
            protocol = _protocol;
            url = _url;
            id = _id;
            pw = _pw;
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
                return false;

            return true;
        }
    }
}