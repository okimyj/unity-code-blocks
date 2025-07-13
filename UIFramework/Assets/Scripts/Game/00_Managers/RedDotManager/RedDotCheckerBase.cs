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
        public virtual void Refresh() { }
        
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
