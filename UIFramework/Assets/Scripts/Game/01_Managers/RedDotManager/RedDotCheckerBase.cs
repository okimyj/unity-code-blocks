/*
 * RedDotChecker의 기본이 되는 클래스- HasRedDot 프로퍼티를 통해 상태 변경 감지 및 자동 알림
 * 알림 핸들러 등록/호출/삭제 기능 포함
 * 콘텐츠 타입 별로 독립적인 클래스로 생성
 * Refresh 함수는 RedDot 조건을 판별하는 핵심 함수로,   모든 파생 클래스에서 반드시 구현해야 함으로 abstract 선언.
 */
using System;

namespace YJClient
{
    // 반드시 포함해야 하는 기본 함수 interface 선언.
    public interface IRedDotChecker
    {
        void Refresh();
        void Notify();
        void AddNotifyHandler(Action<EContentsType, bool> handler);
        void RemoveNotifyHandler(Action<EContentsType, bool> handler);
        void ClearNotifyHandler();
    }
    public abstract class RedDotCheckerBase : IRedDotChecker
    {
        public bool HasRedDot
        {
            get { return _hasRedDot; }
            protected set
            {
                if (_hasRedDot != value)
                {
                    _hasRedDot = value;
                    Notify();
                }
            }
        }
        protected readonly EContentsType _contentsType;
        private Action<EContentsType, bool> _notifyHandlers;
        private bool _hasRedDot;
        
        protected RedDotCheckerBase(EContentsType contentsType)
        {
            _contentsType = contentsType;
        }
        public abstract void Refresh();
        
        public void AddNotifyHandler(Action<EContentsType, bool> handler)
        {
            _notifyHandlers += handler;
        }
        public void Notify()
        {
            _notifyHandlers?.Invoke(_contentsType, _hasRedDot);
        }
        public void RemoveNotifyHandler(Action<EContentsType, bool> handler)
        {
            _notifyHandlers -= handler;
        }
        public void ClearNotifyHandler()
        {
            _notifyHandlers = null;
        }
    }

}
