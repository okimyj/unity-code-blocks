using RKCommon.DataTable;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Framework.Profanity
{
    public class ProfanityChecker
    {
        
        public class TrieNode
        {
            public Dictionary<char, TrieNode> Children = new Dictionary<char, TrieNode>();
            public bool IsEndOfWord;
        }
        private TrieNode trieRoot;
        private bool isInitialized = false;
        public ProfanityChecker(string fileName)
        {
            Initialize(fileName);
        }
        public ProfanityChecker() { }
        public void Load(string fileName)
        {
            Initialize(fileName);
        }
        public string FilterMessage(string text, char replace = '*')
        {
            if (!isInitialized)
                return text;

            var strBuilder = new StringBuilder(text);
            var textLength = text.Length;
            var lastReplaceIndex = -1;
            for (int i = 0; i < textLength; i++)
            {
                var node = trieRoot;

                for (int j = i; j < textLength; j++)
                {
                    var lowerChar = char.ToLowerInvariant(text[j]);

                    if (!node.Children.ContainsKey(lowerChar))
                        break;

                    node = node.Children[lowerChar];

                    if (node.IsEndOfWord)
                    {
                        // 금칙어를 ****로 치환
                        for (int k = i; k <= j; k++)
                            strBuilder[k] = replace;
                        lastReplaceIndex = j;
                    }
                }
                // 이미 치환한 문자도 검사하게된다. 건너뛰기.
                if (lastReplaceIndex > -1)
                {
                    i = lastReplaceIndex;
                    lastReplaceIndex = -1;
                }
                    
            }
            return strBuilder.ToString();
        }
        public bool IsExistProfanity(string _check)
        {
            if (!isInitialized)
                return false;

            string filtered = new string(_check.Where(c => !char.IsDigit(c)).ToArray());

            for (int i = 0; i < filtered.Length; i++)
            {
                if (SearchInTrie(filtered, i))
                    return true;
            }
            return false;
        }
        private bool SearchInTrie(string word, int start)
        {
            TrieNode currentNode = trieRoot;
            for (int i = start; i < word.Length; i++)
            {
                var lowerChar = char.ToLowerInvariant(word[i]);
                if (!currentNode.Children.ContainsKey(lowerChar))
                {
                    return false;
                }
                currentNode = currentNode.Children[lowerChar];
                if (currentNode.IsEndOfWord)
                {
                    return true;
                }
            }
            return false;
        }
        #region Initialize
        private bool Initialize(string fileName)
        {
            if (isInitialized)
                return true;
            isInitialized = true;
            var profanityList = new List<string>();
            if (!LoadProfanity(ref profanityList, fileName))
            {
                return false;
            }

            trieRoot = new TrieNode();

            foreach (var profanity in profanityList)
            {
                TrieNode currentNode = trieRoot;
                foreach (var ch in profanity)
                {
                    if (!currentNode.Children.ContainsKey(ch))
                    {
                        currentNode.Children[ch] = new TrieNode();
                    }
                    currentNode = currentNode.Children[ch];
                }
                currentNode.IsEndOfWord = true;
            }
            return true;
        }


        private bool LoadProfanity(ref List<string> _list, string fileName)
        {
            try
            {
                _list.Clear();
                var text = DataTableManager.Instance.LoadTextDataTable(fileName);

                if (!string.IsNullOrEmpty(text))
                {
                    // 파일의 각 줄을 읽어서 HashSet에 추가
                    using (var reader = new StringReader(text))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                _list.Add(line.ToLowerInvariant().Trim()); // 공백 제거 후 HashSet에 추가
                            }
                                
                        }
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                DataTableManager.Instance.LogError("[DATA TABLE ERROR] Failed to LoadProfanity : " + ex.Message);
            }
            return false;
        }
        #endregion Initialize
    }
    

}
