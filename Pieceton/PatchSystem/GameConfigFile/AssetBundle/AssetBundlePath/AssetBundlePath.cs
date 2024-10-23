using UnityEngine;
using System.Collections;

namespace Pieceton.PatchSystem
{
    public class PBundlePath
    {
        // ActorPrefab/Act_Model1 => Act_Model1
        public static string SplitPackName(string _bundle_name)
        {
            string packName = string.Empty;

            if (!string.IsNullOrEmpty(_bundle_name))
            {
                string[] split = _bundle_name.Split('/');

                int SplitCount = split.Length;

                for (int i = 0; i < SplitCount; ++i)
                {
                    if (split[i].Contains(ResExtension.BUNDLE_DOT_PACK))
                    {
                        packName = split[i];
                    }
                }

                if (string.IsNullOrEmpty(packName))
                {
                    packName = split[SplitCount - 1];
                }
            }

            return packName;
        }

        // ActorPrefab/Act_Model1 => Act_Model1.pak
        public static string MakePackFileName(string _bundle_name)
        {
            string packName = SplitPackName(_bundle_name);

            if (!string.IsNullOrEmpty(packName))
            {
                packName += ResExtension.BUNDLE_DOT_PACK;
            }

            return packName;
        }
    }
}