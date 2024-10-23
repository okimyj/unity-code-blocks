using UnityEngine;
using System.Collections.Generic;

namespace Pieceton.Misc
{
    public static class AbleSingleton { public static bool isAble = true; }

    public abstract class PSingletonData
    {
        public static readonly List<PSingletonData> singletonList = new List<PSingletonData>();

        public string initInstanceError = "";

        protected static void PushSingleton(PSingletonData _obj)
        {
            if (null != _obj)
            {
                singletonList.Add(_obj);

                _obj.initInstanceError = "";
                _obj.Initialize();
            }
        }

        public static bool InitSingletons()
        {
            int count = singletonList.Count;
            for (int n = 0; n < count; ++n)
            {
                if (null != singletonList[n])
                {
                    if (false == singletonList[n].Initialize())
                        return false;
                }
            }

            return true;
        }

        public static void ReleaseSingletons()
        {
            int count = singletonList.Count;
            for (int n = 0; n < count; ++n)
            {
                if (null != singletonList[n])
                    singletonList[n].ReleaseSingleton();
            }
            singletonList.Clear();
        }

        public abstract bool Initialize();
        protected abstract void ReleaseSingleton();
    }

    public abstract class PSingletonData<T> : PSingletonData where T : class, new()
    {
        private static T m_instance = null;
        public static T Instance
        {
            get
            {
                if (null == m_instance)
                {
                    if (!AbleSingleton.isAble)
                    {
                        throw new System.Exception("Instance(" + typeof(T) + ") already destroyed on application quit.");
                    }

                    m_instance = new T();
                    PSingletonData.PushSingleton(m_instance as PSingletonData);
                }
                return m_instance;
            }
        }

        private bool _initialized = false;
        public override bool Initialize()
        {
            if (!string.IsNullOrEmpty(initInstanceError))
                return false;

            if (!_initialized)
            {
                _initialized = true;
                return InitInstance();
            }

            return true;
        }

        protected override void ReleaseSingleton()
        {
            if (_initialized)
            {
                _initialized = false;
                initInstanceError = "";
                ReleaseInstance();
            }

            m_instance = null;
        }

        protected abstract bool InitInstance();
        protected abstract void ReleaseInstance();
    }

    public abstract class PSingletonComponent : MonoBehaviour
    {
        protected static bool _isProcessing_Release = false;
        public static readonly List<PSingletonComponent> singletonList = new List<PSingletonComponent>();

        public string initInstanceError { get { return _initInstanceError; } }
        protected string _initInstanceError = "";

        protected static void PushSingleton(PSingletonComponent _obj)
        {
            if (null != _obj)
            {
                if (Application.isPlaying)
                    DontDestroyOnLoad(_obj);

                AttachSingletonComponent(_obj);

                singletonList.Add(_obj);
                _obj.AwakeSingleton();
            }
        }

        private static void AttachSingletonComponent(PSingletonComponent _obj)
        {
            if (null != _obj)
            {
                _obj.gameObject.transform.SetParent(singletonRoot.transform);
            }
        }

        private static GameObject singletonRoot
        {
            get
            {
                if (null == _singletonRoot)
                {
                    _singletonRoot = GameObject.Find("SingletonRoot");
                    if (null == _singletonRoot)
                    {
                        _singletonRoot = new GameObject("SingletonRoot");
                        DontDestroyOnLoad(_singletonRoot);
                    }
                }

                return _singletonRoot;
            }
        }
        private static GameObject _singletonRoot = null;

        public static bool InitSingletons()
        {
            int count = singletonList.Count;
            for (int n = 0; n < count; ++n)
            {
                if (null != singletonList[n])
                {
                    if (false == singletonList[n].Initialize())
                        return false;
                }
            }

            return true;
        }

        public static void ReleaseSingletons()
        {
            if (!_isProcessing_Release)
            {
                _isProcessing_Release = true;

                int count = singletonList.Count;

                for (int n = count - 1; n >= 0; --n)
                {
                    if (null != singletonList[n])
                        singletonList[n].StopAllCoroutines();
                }

                for (int n = count - 1; n >= 0; --n)
                {
                    PSingletonComponent _component = singletonList[n];
                    if (null != _component && _component.gameObject.activeInHierarchy)
                        _component.gameObject.SetActive(false);
                }

                for (int n = count - 1; n >= 0; --n)
                {
                    if (null != singletonList[n])
                        singletonList[n].ReleaseSingleton();
                }

                singletonList.Clear();

                _isProcessing_Release = false;

                GameObject.Destroy(_singletonRoot);
                _singletonRoot = null;
            }
        }

        protected abstract void AwakeSingleton();
        public abstract bool Initialize();
        protected abstract void ReleaseSingleton();
    }

    public abstract class PSingletonComponent<T> : PSingletonComponent where T : MonoBehaviour
    {
        private static T m_instance;
        private static bool m_appQuitting = false;

        public static bool IsQuitting()
        {
            return (!AbleSingleton.isAble || m_appQuitting || _isProcessing_Release);
        }

        public static void CreateInstance()
        {
            if (null == m_instance)
                m_instance = Instance;
        }

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    if (IsQuitting())
                    {
                        throw new System.Exception("Instance(" + typeof(T) + ") already destroyed on application quit.");
                    }

                    T _instance = FindObjectOfType(typeof(T)) as T;
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        singleton.name = "(Singleton)" + typeof(T).ToString();
                        m_instance = singleton.AddComponent<T>();
                    }
                    else
                    {
                        m_instance = _instance;
                        PushSingleton(m_instance as PSingletonComponent);
                    }
                }

                return m_instance;
            }
        }

        public bool isInitialized { get { return _initialized; } }
        private bool _initialized = false;
        public override bool Initialize()
        {
            if (!string.IsNullOrEmpty(initInstanceError))
                return false;

            if (!_initialized)
            {
                _initialized = true;
                return InitInstance();
            }

            return true;
        }

        protected override void AwakeSingleton()
        {
            AwakeInstance();
        }

        protected override void ReleaseSingleton()
        {
            if (_initialized)
            {
                _initialized = false;
                _initInstanceError = "";

                if (null != m_instance)
                {
                    ReleaseInstance();
                    Destroy(m_instance);
                }
            }

            m_instance = null;
        }

        protected abstract void AwakeInstance();
        protected abstract bool InitInstance();
        protected abstract void ReleaseInstance();


        void Awake()
        {
            useGUILayout = false;

            if (null == m_instance)
            {
                m_instance = this as T;
                PushSingleton(m_instance as PSingletonComponent);
            }
        }

        void OnApplicationQuit()
        {
            if (_initialized)
            {
                //종료되고 있을때는 ReleaseSingleton()을 호출하지 말것
                ReleaseInstance();
            }

            m_appQuitting = true;
            m_instance = null;
        }
    }
}