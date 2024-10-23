using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pieceton.Misc
{
    public class PCSVReader
    {
        //static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string SPLIT_RE = @"\t(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        public static List<Dictionary<string, string>> ReadAsset(string file)
        {
            TextAsset data = Resources.Load<TextAsset>(file);
            return Read(file, data);
        }

        public static List<Dictionary<string, string>> Read(string file)
        {
            StreamReader sr = File.OpenText(file);
            return Read(file, sr.ReadToEnd());
        }

        public static List<Dictionary<string, string>> Read(string fileName, TextAsset data)
        {
            if (null == data)
                return null;

            return Read(fileName, data.text);
        }

        public static List<Dictionary<string, string>> Read(string fileName, string _data)
        {
            if (string.IsNullOrEmpty(_data))
                return null;

            var list = new List<Dictionary<string, string>>();

            try
            {
                string[] lines = Regex.Split(_data, LINE_SPLIT_RE);

                if (lines.Length <= 1)
                    return list;

                string[] header = Regex.Split(lines[0], SPLIT_RE);
                for (int i = 1; i < lines.Length; ++i)
                {

                    string[] values = Regex.Split(lines[i], SPLIT_RE);
                    if (IsEmptyValues(values))
                        continue;

                    Dictionary<string, string> entry = new Dictionary<string, string>();
                    for (var j = 0; j < header.Length && j < values.Length; j++)
                    {
                        string value = values[j];

                        if (value.Length > 0)
                        {
                            if (value[0] == '\"' && value[value.Length - 1] == '\"')
                            {
                                value = value.Remove(0, 1);
                                value = value.Remove(value.Length - 1);
                            }

                            value = value.Replace("\"\"", "\"");
                        }
                        // 파싱 방식 변경

                        entry[header[j]] = value;
                    }
                    list.Add(entry);
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Failed load CSV file. {0}\n{1}", fileName, e);
                return null;
            }

            return list;
        }

        private static bool IsEmptyValues(string[] values)
        {
            bool isEmpty = true;
            int count = values.Length;
            for (int i = 0; i < count; ++i)
            {
                if (!string.IsNullOrEmpty(values[i]))
                    isEmpty = false;
            }

            return isEmpty;
        }

        public static Dictionary<string, string> ReadDic(TextAsset data)
        {
            if (null == data)
                return null;

            var list = new Dictionary<string, string>();

            string[] lines = Regex.Split(data.text, LINE_SPLIT_RE);

            if (lines.Length <= 1)
                return list;

            for (int i = 0; i < lines.Length; ++i)
            {
                string[] values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "")
                    continue;

                list.Add(values[0], values[1]);
            }
            return list;
        }
    }
}