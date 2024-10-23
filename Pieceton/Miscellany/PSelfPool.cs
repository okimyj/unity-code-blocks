using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace Pieceton.Misc
{
    public abstract class IPSelfPoolClass { public abstract void Release(); }

    public class PSelfPoolMgr
    {
        private static List<PSelfPoolObject> _poolObjectList = new List<PSelfPoolObject>();
        private static List<IPSelfPoolClass> _poolClassList = new List<IPSelfPoolClass>();

        public static void Add(PSelfPoolObject _obj) { _poolObjectList.Add(_obj); }
        public static void Add(IPSelfPoolClass _class) { _poolClassList.Add(_class); }

        public static Transform poolObjectContainer
        #region poolObjectContainer
        {
            get
            {
                if (null == _container)
                {
                    GameObject go = new GameObject();
                    go.name = "PSelfPoolObjectContainer";
                    _container = go.transform;
                }
                return _container;
            }
        }
        private static Transform _container;
        #endregion poolObjectContainer

        public static void Release()
        {
            ReleaseObjects();
            ReleaseClasses();
        }

        public static void OnBeginChangeScene()
        {
            ReleaseObjects();
        }

        private static void ReleaseObjects()
        {
            int count = _poolObjectList.Count;
            for (int i = 0; i < count; ++i)
            {
                if (null == _poolObjectList[i])
                    continue;

                _poolObjectList[i].Release();
            }

            _poolObjectList.Clear();
        }

        private static void ReleaseClasses()
        {
            int count = _poolClassList.Count;
            for (int i = 0; i < count; ++i)
            {
                if (null == _poolClassList[i])
                    continue;

                _poolClassList[i].Release();
            }

            _poolClassList.Clear();
        }
    }

    public class PSelfPoolClass<T> : IPSelfPoolClass where T : class, new()
    {
        public int useCount { get { return _useCount; } }
        private int _useCount = 0;

        protected static List<T> _pooledObjectList = new List<T>();

        public PSelfPoolClass() { PSelfPoolMgr.Add(this); }

        public T Load()
        {
            ++_useCount;
            return GetPooledObject();
        }

        public void Unload(T _instance)
        {
            --_useCount;
            if (null != _instance)
            {
                _pooledObjectList.Add(_instance);
            }
        }

        public override void Release()
        {
            _useCount = 0;

            if (0 < _pooledObjectList.Count)
            {
                int _count = _pooledObjectList.Count;
                for (int n = 0; n < _count; ++n)
                {
                    _pooledObjectList[n] = null;
                }
                _pooledObjectList.Clear();
            }
        }

        protected T GetPooledObject()
        {
            if (0 < _pooledObjectList.Count)
            {
                T _result = _pooledObjectList[0];
                _pooledObjectList.RemoveAt(0);
                return _result;
            }

            return (new T());
        }
    }


    public class PSelfPoolObject
    {
        private StringBuilder stringBuilder
        #region StringBuilder
        {
            get
            {
                if (null != _strBuilder)
                    _strBuilder.Remove(0, _strBuilder.Length);
                else
                    _strBuilder = new StringBuilder();

                return _strBuilder;
            }
        }
        private StringBuilder _strBuilder = new StringBuilder();
        #endregion

        public Transform selfPoolContainer
        #region SelfPoolContainer
        {
            get
            {
                if (null == _selfPoolContainer)
                {
                    GameObject _objPoolContainer = new GameObject();

                    StringBuilder strBuilder = stringBuilder;
                    strBuilder.Append("SelfPoolContainer");

                    if (null != _orgPrefab)
                    {
                        strBuilder.AppendFormat("_{0}", _orgPrefab.name);
                    }

                    _objPoolContainer.name = strBuilder.ToString();

                    _selfPoolContainer = _objPoolContainer.transform;
                    _selfPoolContainer.SetParent(PSelfPoolMgr.poolObjectContainer);
                }
                return _selfPoolContainer;
            }
        }
        private Transform _selfPoolContainer;
        #endregion

        private readonly List<GameObject> _pooledObjectList = new List<GameObject>();
        private readonly Dictionary<GameObject, Transform> _usingObjectDic = new Dictionary<GameObject, Transform>();

        private GameObject _orgPrefab;
        bool destroyme = false;

        private System.Action<PSelfPoolObject> _releaseCallback = null;

        public PSelfPoolObject(System.Action<PSelfPoolObject> _releaseCB) { PSelfPoolMgr.Add(this); _releaseCallback = _releaseCB; }

        public void Init(GameObject _org_prefab)
        {
            if (_orgPrefab != _org_prefab)
            {
                _orgPrefab = _org_prefab;
            }
        }

        public void Cache(GameObject _org_prefab, int _count)
        {
            Init(_org_prefab);

            for (int i=0; i< _count; ++i)
            {
                if (null != _orgPrefab)
                {
                    GameObject instance = GameObject.Instantiate(_orgPrefab);
                    instance.transform.SetParent(selfPoolContainer);
                    instance.SetActive(false);
                    _pooledObjectList.Add(instance);
                }
            }
        }

        public GameObject Load(GameObject _org_prefab, Transform _parent, string _name)
        {
            Init(_org_prefab);

            GameObject _instance = GetPooledObject(_name);
            if (null != _instance)
            {
                _usingObjectDic.Add(_instance, _parent);
                _instance.transform.SetParent(_parent);

                _instance.SetActive(true);
            }
            return _instance;
        }

        public GameObject Load(Transform _parent, bool worldpositionstay)
        {
            GameObject _instance = GetPooledObject();
            if (null != _instance)
            {
                _usingObjectDic.Add(_instance, _parent);
                _instance.transform.SetParent(_parent, worldpositionstay);

                _instance.SetActive(true);
            }
            return _instance;
        }

        public void Unload(GameObject _instance)
        {
            if (destroyme == true)
                return;

            if (null != _instance)
            {
                Transform _parent;
                if (_usingObjectDic.TryGetValue(_instance, out _parent))
                {
                    _usingObjectDic.Remove(_instance);

                    _instance.transform.SetParent(selfPoolContainer);
                    _pooledObjectList.Add(_instance);

                    _instance.SetActive(false);
                }
            }
        }

        public void UnloadUsingObjects()
        {
            if (null != _usingObjectDic && 0 < _usingObjectDic.Count)
            {
                Dictionary<GameObject, Transform>.Enumerator iter = _usingObjectDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    GameObject _instance = iter.Current.Key;
                    if (null != _instance)
                    {
                        if (destroyme == false) // 재시작 단계일때  ...setparent호출시 onenable 이 호출되 recursion ..
                            _instance.transform.SetParent(selfPoolContainer);

                        _pooledObjectList.Add(_instance);

                        _instance.SetActive(false);
                    }
                }
                _usingObjectDic.Clear();
            }
        }

        public void Release()
        {
            if (null != _releaseCallback)
            {
                _releaseCallback(this);
                _releaseCallback = null;
            }
            destroyme = true; //recursion  when  restart singleton release ,on disable.

            UnloadUsingObjects();

            if (0 < _pooledObjectList.Count)
            {
                int _count = _pooledObjectList.Count;
                for (int n = 0; n < _count; ++n)
                {
                    try
                    {
                        GameObject.Destroy(_pooledObjectList[n]);
                    }
                    catch (Exception) { }

                    _pooledObjectList[n] = null;
                }
                _pooledObjectList.Clear();
            }

            _orgPrefab = null;
        }

        private GameObject GetPooledObject(string _name)
        {
            if (0 < _pooledObjectList.Count)
            {
                StringBuilder strBuilder = stringBuilder;
                strBuilder.AppendFormat("{0}{1}", _name, "(Clone)");

                GameObject _result = _pooledObjectList.Find(delegate (GameObject obj) { return (null != obj && obj.name == strBuilder.ToString()); });
                if (null != _result)
                {
                    _pooledObjectList.Remove(_result);
                    return _result;
                }

                Debug.LogError("Invalid self pool object");
            }

            if (null != _orgPrefab)
            {
                return GameObject.Instantiate(_orgPrefab);
            }

            Debug.LogError("Invalid self pool orgPrefab");

            return null;
        }

        private GameObject GetPooledObject()
        {
            if (0 < _pooledObjectList.Count)
            {
                GameObject _result = _pooledObjectList[0];
                _pooledObjectList.RemoveAt(0);

                if (null == _result)
                    Debug.LogError("Invalid self pool object");

                return _result;
            }
            else if (null != _orgPrefab)
            {
                return GameObject.Instantiate(_orgPrefab);
            }

            Debug.LogError("Invalid self pool orgPrefab");

            return null;
        }
    }
}