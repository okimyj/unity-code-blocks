using UnityEngine;
using System.Text.RegularExpressions;

namespace Pieceton.Misc
{
    public class PRegularExpression
    {
        // 기본라틴어에서 스페이스만 허용. spece = \u0020
        private const string ratinParttern_ableSpace = "[\u0000-\u0019\u0021-\u002F\u003A-\u0040\u005B-\u0060\u007B-\u00BF]";

        private const string ratinParttern = "[\u0000-\u002F\u003A-\u0040\u005B-\u0060\u007B-\u00BF]";
        private const string hanguljamoParttern = "[\u1100-\u11F9\u3131-\u318E]";
        private const string specialCharParttern = "[\u200C-\u303F\u3200-\u33FF\uA4F8-\uABFF\uD7B0-\uF8FF\uFB00-\uFB06\uFEFF-\uFF0F\uFF1A-\uFF20\uFF3B-\uFF40\uFF5B-\uFF65\uFFE2-\uFFFF]";

        public static bool IsValid_NickName(string input)
        {
            bool ratinResult = Regex.IsMatch(input, ratinParttern);
            bool hanguljamoResult = Regex.IsMatch(input, hanguljamoParttern);
            bool specialCharResult = Regex.IsMatch(input, specialCharParttern);

            if (ratinResult || hanguljamoResult || specialCharResult)
                return false;

            return true;
        }

        public static bool IsValid_GuildName(string input)
        {
            bool ratinResult = Regex.IsMatch(input, ratinParttern_ableSpace);
            bool hanguljamoResult = Regex.IsMatch(input, hanguljamoParttern);
            bool specialCharResult = Regex.IsMatch(input, specialCharParttern);

            if (ratinResult || hanguljamoResult || specialCharResult)
                return false;

            return true;
        }
    }
}