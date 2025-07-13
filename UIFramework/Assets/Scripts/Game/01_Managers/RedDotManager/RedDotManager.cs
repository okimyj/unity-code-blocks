/*
 * UI의 RedDot 알림 상태를 통합 관리하는 클래스
 * 콘텐츠 타입별로 RedDotChecker를 등록하고 상태 갱신
 * 실시간 동기화를 위해 옵저버 패턴 적용
 * 팩토리 패턴으로 Checker 객체 생성 (확장, 보수 용이성)
 */
using System.Collections.Generic;
using YJFramework.Core;
using System;
namespace YJClient
{
    public class RedDotManager : SingletonMonoDontDestroyBehaviour<RedDotManager>
    {
        private Dictionary<EContentsType, Func<RedDotCheckerBase>> _checkerFactory = new Dictionary<EContentsType, Func<RedDotCheckerBase>>()
        {
            { EContentsType.Arena, ()=> new RedDotCheckerArena() },
            { EContentsType.Shop, ()=> new RedDotCheckerShop() },
            { EContentsType.Hero, ()=> new RedDotCheckerHero() },
            { EContentsType.Item, ()=> new RedDotCheckerItem() },
            { EContentsType.Mission, ()=> new RedDotCheckerMission() },
        };
        private Dictionary<EContentsType, RedDotCheckerBase> _reddotCheckerDict = new Dictionary<EContentsType, RedDotCheckerBase>();
        public void Initialize()
        {
            foreach(var factory in _checkerFactory)
            {
                _reddotCheckerDict[factory.Key] = factory.Value();
            }
            Refresh();
        }
        public void Refresh()
        {
            foreach(var checker in _reddotCheckerDict.Values)
            {
                checker.Refresh();
            }
        }
        public bool HasRedDot(EContentsType contentsType)
        {
            var checker = GetRedDotChecker(contentsType);
            return checker == null ? false : checker.HasRedDot;
        }
        public void AddNotifyHandler(EContentsType contentsType, Action<EContentsType, bool> handler)
        {
            var checker = GetRedDotChecker(contentsType);
            if (checker != null)
                checker.AddNotifyHandler(handler);
            else
                GameLogger.LogError($"[RedDotManager] AddNotifyHandler - haven't checker. {contentsType}");
        }
        public void RemoveNotifyHandler(EContentsType contentsType, Action<EContentsType, bool> handler)
        {
            var checker = GetRedDotChecker(contentsType);
            if (checker != null)
                checker.RemoveNotifyHandler(handler);
            else
                GameLogger.LogError($"[RedDotManager] RemoveNotifyHandler - haven't checker. {contentsType}");
        }
        public void Clear()
        {
            foreach(var checker in _reddotCheckerDict.Values)
            {
                checker.ClearNotifyHandler();
            }
            _reddotCheckerDict.Clear();
        }
        #region Private Helper 
        private RedDotCheckerBase GetRedDotChecker(EContentsType contentsType)
        {
            if (_reddotCheckerDict.TryGetValue(contentsType, out var checker))
            {
                return checker;
            }
            return null;
        }
        #endregion
    }
}
