using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

using Pieceton.Configuration;

namespace Pieceton.PatchSystem
{
    public static class PatchListUploadFileMaker
    {
        public static void Make(BundleManifest _manifest, BuildTarget _build_target, string _version_name)
        {
            if (null == _manifest)
                return;

            Debug.Log("File UploadList Write");

            List<string> pFileList = _manifest.bundleList;

            string seperChar = string.Format("{0}", Path.DirectorySeparatorChar);

            string version = _version_name;//AppInfo.curGameVersionName;
            string fileName = PatchSystemEditor.ASSET_BUNDLE_UPLOADLIST_FILENAME + "__" + version + PatchSystemEditor.ASSET_BUNDLE_UPLOADLIST_EXTENSION;

            string PlatFormPath = PBundlePathEditor.PlatformName(_build_target);
            string FilePath = PatchSystemEditor.ASSET_BUNDLE_ARCHIVE_PATH + PlatFormPath + seperChar + fileName;

            List<string>.Enumerator fileEnumerator = pFileList.GetEnumerator();

            try
            {
                int writeCount = 0;

                StreamWriter writer = File.CreateText(FilePath);

                while (fileEnumerator.MoveNext())
                {
                    string line = fileEnumerator.Current;

                    string _hash = _manifest.GetHashFolderName(line);

                    string packFileName = PBundlePath.MakePackFileName(line);

                    string addFile = string.Format("{0}:{1}", _hash, packFileName);

                    ++writeCount;

                    writer.WriteLine(addFile);
                }

                if (writeCount.Equals(0))
                {
                    writer.WriteLine("");
                }

                writer.Flush();
                writer.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw new Exception();
            }
        }
    }
}