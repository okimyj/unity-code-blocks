/*
 * 각 윈도우를 식별하고 리소스 정보를 담는 고유 키 클래스
 */
using System;
using System.Collections.Generic;
using YJFramework.Resource;
using YJFramework.Core;

namespace YJFramework.UI
{
    public class WinKey : IEquatable<WinKey>
    {
        public static Dictionary<int, WinKey> AllWindowKeys = new Dictionary<int, WinKey>();
        public int ID { get; private set; }
        public ResKey ResourceKey { get; private set; }
        public string PrefabName { get; private set; }
        public int Priority { get; private set; }
        public bool CanBackKeyClose { get; private set; }
        public bool IsTopWindow { get; private set; }
        public bool IsStartShow { get; private set; }
        public WinKey(int id, ResKey resourceKey, string prefabName, int priority, bool canBackKeyClose, bool isTopWindow, bool isStartShow)
        {
            ID = id;
            ResourceKey = resourceKey;
            PrefabName = prefabName;
            Priority = priority;
            CanBackKeyClose = canBackKeyClose;
            IsTopWindow = isTopWindow;
            IsStartShow = isStartShow;
            if (AllWindowKeys.ContainsKey(id))
            {
                GameLogger.LogError($"Duplicate WinKey.ID {id}");
                return;
            }
            AllWindowKeys[id] = this;
        }
        public override bool Equals(object obj)
        {
            if (obj is WinKey)
            {
                return Equals((WinKey)obj);
            }
            return false;
        }
        public bool Equals(WinKey other)
        {
            return ID == other.ID;
        }
        public static bool operator ==(WinKey lhs, WinKey rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(WinKey lhs, WinKey rhs)
        {
            return !(lhs.Equals(rhs));
        }
    }

    public class WinKeyBasic
    {
        public static WinKey MessageBox = new WinKey(1, ResourceKey.BASIC_WINDOW, "MessageBox", 10000,
            /*BackKeyClose*/ false, /*HideDestroy*/ true, /*MostTopWindow*/ true);
    }


}