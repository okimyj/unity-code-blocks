using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Pieceton.Misc
{
    public static class PUtility
    {
        public static void ClampMove3D(this Transform tr, Vector3 move, Vector3 min, Vector3 max)
        {
            Vector3 pos = tr.position + move;

            pos.x = Mathf.Clamp(pos.x, min.x, max.x);
            pos.y = Mathf.Clamp(pos.y, min.y, max.y);
            pos.z = Mathf.Clamp(pos.z, min.z, max.z);

            tr.position = pos;
        }

        public static void SetDefaultScale(this Transform trans)
        {
            trans.localScale = Vector3.one;
        }

        //입력된 GameObject의 자식들만 검색하여 해당 컴포넌트를 돌려준다
        public static T FindComponentInChildren<T>(this GameObject obj, string objName) where T : Component
        {
            T[] arrComponent = obj.GetComponentsInChildren<T>();
            if (null != arrComponent)
            {
                int count = arrComponent.Length;
                for (int n = 0; n < count; ++n)
                {
                    if (arrComponent[n].name == objName)
                    {
                        return arrComponent[n];
                    }
                }
            }

            return null;
        }

        //현재 하이러키에 존재하는 모든 오브젝트를 검색하여 해당 컴포넌트를 돌려준다
        public static T FindComponentInScene<T>(string objName) where T : Component
        {
            GameObject obj = GameObject.Find(objName);
            if (null != obj)
            {
                T resultComponent = obj.GetComponent<T>();
                if (null != resultComponent)
                {
                    return resultComponent;
                }
                //else {
                //    Debug.LogWarning("Invalid component(" + typeof(T) + ")" + ", objName(" + objName + ")");
                //}
            }
            //else {
            //    Debug.LogWarning("Invalid objName(" + objName + ")");
            //}

            return null;
        }

        public static int GetDataIndexInList<T>(T _src, List<T> _targetlist) where T : class
        {
            if (null != _src && null != _targetlist)
            {
                int count = _targetlist.Count;
                for (int n = 0; n < count; ++n)
                {
                    if (_src == _targetlist[n])
                    {
                        return n;
                    }
                }
            }
            return -1;
        }

        public static bool IsExistsInList<T>(T _src, List<T> _targetlist) where T : class
        {
            if (null != _src && null != _targetlist)
            {
                int count = _targetlist.Count;
                for (int n = 0; n < count; ++n)
                {
                    if (_src == _targetlist[n])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null != obj)
            {
                obj.layer = newLayer;

                foreach (Transform child in obj.transform)
                {
                    if (null != child)
                    {
                        SetLayerRecursively(child.gameObject, newLayer);
                    }
                }
            }
        }

        public static void StartProcess(this MonoBehaviour _component, IEnumerator _func, ref IEnumerator _result)
        {
            if (null == _component)
                return;

            StopProcess(_component, ref _result);

            _result = _func;
            _component.StartCoroutine(_result);
        }

        public static void StopProcess(this MonoBehaviour _component, ref IEnumerator _result)
        {
            if (null == _component)
                return;

            if (null == _result)
                return;

            _component.StopCoroutine(_result);
            _result = null;
        }

        public static Color RandomColor()
        {
            float r = (float)PRandomizer.FRand(0.0, 1.0);
            float g = (float)PRandomizer.FRand(0.0, 1.0);
            float b = (float)PRandomizer.FRand(0.0, 1.0);

            return new Color(r, g, b);
        }

        public static double DiffDays(DateTime _left, DateTime _right)
        {
            if (_left != _right)
            {
                DateTime min = (_left < _right ? _left : _right);
                DateTime max = (_left > _right ? _left : _right);

                TimeSpan difference = max - min;
                return difference.TotalDays;
            }

            return 0;
        }

        public static double DiffHours(DateTime _left, DateTime _right)
        {
            if (_left != _right)
            {
                DateTime min = (_left < _right ? _left : _right);
                DateTime max = (_left > _right ? _left : _right);

                TimeSpan difference = max - min;
                return difference.TotalHours;
            }

            return 0;
        }

        public static double DiffSeconds(DateTime _left, DateTime _right)
        {
            if (_left != _right)
            {
                DateTime min = (_left < _right ? _left : _right);
                DateTime max = (_left > _right ? _left : _right);

                TimeSpan difference = max - min;
                return difference.TotalSeconds;
            }

            return 0;
        }

        public static bool IsResetDaily(DateTime _last, DateTime _now, int _reset_hour)
        {
            if (_last >= _now)
                return false;

            bool needReset = false;

            double diffDays = DiffHours(_last, _now);
            if (diffDays >= 24)
            {
                needReset = true;
            }
            else
            {
                DateTime today5Clock = new DateTime(_now.Year, _now.Month, _now.Day, _reset_hour, 0, 0);
                if (_now >= today5Clock)
                {
                    if (_last < today5Clock)
                    {
                        needReset = true;
                    }
                }
            }

            return needReset;
        }

        public static void Resize<T>(List<T> list, int size, T element = default(T))
        {
            int count = list.Count;

            if (size < count)
            {
                list.RemoveRange(size, count - size);
            }
            else if (size > count)
            {
                if (size > list.Capacity)   // Optimization
                    list.Capacity = size;

                list.AddRange(Enumerable.Repeat(element, size - count));
            }
        }

        public static void SetParent(Transform _obj, Transform _parent, bool _world_position_stay)
        {
            if (null != _obj && null != _parent)
            {
                _obj.SetParent(_parent, _world_position_stay);
                _obj.localScale = Vector3.one;
                _obj.localPosition = Vector3.zero;
                _obj.localRotation = Quaternion.identity;
            }
        }
    }
}