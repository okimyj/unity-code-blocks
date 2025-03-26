using Framework.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Profanity
{
    public class ProfanityManager : Singleton<ProfanityManager>
    {
        private const string PROFANITY_CHAT_FILE_NAME = "profanity-chat";
        public ProfanityChecker Chat = new ProfanityChecker(PROFANITY_CHAT_FILE_NAME);
    }

}
