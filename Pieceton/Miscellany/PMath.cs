using UnityEngine;
using System.Collections;

namespace Pieceton.Misc
{
    public static class PMath
    {
        public const float EPSILON = 0.001f;

        //public static T Lerp<T>(T _from, T _to, float _factor)
        public static float Lerp(float _from, float _to, float _factor)
        {
            //Mathf.Lerp는 최소, 최대값을 고정시켜 리턴시키기 때문에 이 함수를 사용하도록 하자
            return (_from + ((_to - _from) * _factor));
        }

        public static int Lerp(int _from, int _to, float _factor)
        {
            return (int)(_from + ((float)(_to - _from) * _factor));
        }

        public static long Lerp(long _from, long _to, float _factor)
        {
            return (long)(_from + ((double)(_to - _from) * _factor));
        }

        public static ulong Lerp(ulong _from, ulong _to, float _factor)
        {
            return (ulong)(_from + ((double)(_to - _from) * _factor));
        }

        public static Vector3 Lerp(Vector3 _from, Vector3 _to, float _factor)
        {
            return (_from + ((_to - _from) * _factor));
        }

        public static int Clamp(int _value, int _min, int _max)
        {
            if (_value < _min) return _min;
            if (_value > _max) return _max;
            return _value;
        }

        public static float Clamp01(float _value)
        {
            //Mathf.Clamp01() 사용시 정상적인 value값을 넣었을때 float.NaN을 리턴하는 경우가 확인 되었다.
            //연속적으로 수차례 발생되어 PMath.Clamp() 혹은 이 함수를 사용시 이상없이 동작한다. 되도록이면 PMath의 함수를 이용하도록 하자

            if (_value < 0.0f) return 0.0f;
            if (_value > 1.0f) return 1.0f;
            return _value;
        }

        public static float Clamp(float _value, float _min, float _max)
        {
            if (_value < _min) return _min;
            if (_value > _max) return _max;
            return _value;
        }

        public static double Clamp(double _value, double _min, double _max)
        {
            if (_value < _min) return _min;
            if (_value > _max) return _max;
            return _value;
        }

        public static float GetRatio(int _cur, int _min, int _max)
        {
            if (_max > 0 && _cur > _min)
            {
                float _total = _max - _min;
                if (_total > 0.0f)
                {
                    float _value = _cur - _min;
                    return (_value / _total);
                }
                else
                {
                    if (Debug.isDebugBuild)
                        Debug.LogErrorFormat("[PMath] Invalid GetRatio() current = '{0}' max = '{1}'", _min, _max);
                }
            }
            return 0.0f;
        }

        public static float GetRatio(long _cur, long _min, long _max)
        {
            if (_max > 0 && _cur > _min)
            {
                double _total = _max - _min;
                if (_total > 0.0f)
                {
                    double _value = _cur - _min;
                    return (float)(_value / _total);
                }
                else
                {
                    if (Debug.isDebugBuild)
                        Debug.LogErrorFormat("[PMath] Invalid GetRatio() current = '{0}' max = '{1}'", _min, _max);
                }
            }
            return 0.0f;
        }

        public static float GetRatio(float _cur, float _min, float _max)
        {
            if (_max > 0 && _cur > _min)
            {
                float _total = _max - _min;
                if (_total > 0.0f)
                {
                    float _value = _cur - _min;
                    return (_value / _total);
                }
                else
                {
                    if (Debug.isDebugBuild)
                        Debug.LogErrorFormat("[PMath] Invalid GetRatio() current = '{0}' max = '{1}'", _min, _max);
                }
            }
            return 0.0f;
        }

        public static double GetRatio_Double(int _cur, int _min, int _max)
        {
            if (_max > 0 && _cur > _min)
            {
                double _total = _max - _min;
                if (_total > 0.0)
                {
                    double _value = _cur - _min;
                    return (_value / _total);
                }
                else
                {
                    if (Debug.isDebugBuild)
                        Debug.LogErrorFormat("[PMath] Invalid GetRatio_Double() current = '{0}' max = '{1}'", _min, _max);
                }
            }
            return 0.0;
        }

        public static bool IsBetween(float _cur, float _p1, float _p2)
        {
            float min = (_p1 < _p2 ? _p1 : _p2);
            float max = (_p1 > _p2 ? _p1 : _p2);

            if (_cur >= min && _cur <= max)
                return true;

            return false;
        }

        public static bool IsBetween(int _cur, int _p1, int _p2)
        {
            int min = (_p1 < _p2 ? _p1 : _p2);
            int max = (_p1 > _p2 ? _p1 : _p2);

            if (_cur >= min && _cur <= max)
                return true;

            return false;
        }

        public static bool IsBetween(long _cur, long _p1, long _p2)
        {
            long min = (_p1 < _p2 ? _p1 : _p2);
            long max = (_p1 > _p2 ? _p1 : _p2);

            if (_cur >= min && _cur <= max)
                return true;

            return false;
        }

        public static bool IsBetween(ulong _cur, ulong _p1, ulong _p2)
        {
            ulong min = (_p1 < _p2 ? _p1 : _p2);
            ulong max = (_p1 > _p2 ? _p1 : _p2);

            if (_cur >= min && _cur <= max)
                return true;

            return false;
        }

        public static int Max(int _p1, int _p2)
        {
            int max = (_p1 > _p2 ? _p1 : _p2);

            return max;
        }

        public static int Min(int _p1, int _p2)
        {
            int min = (_p1 < _p2 ? _p1 : _p2);

            return min;
        }


        public static long Max(long _p1, long _p2)
        {
            long max = (_p1 > _p2 ? _p1 : _p2);

            return max;
        }

        public static long Min(long _p1, long _p2)
        {
            long min = (_p1 < _p2 ? _p1 : _p2);

            return min;
        }

        public static float Max(float _p1, float _p2)
        {
            float max = (_p1 > _p2 ? _p1 : _p2);

            return max;
        }

        public static float Min(float _p1, float _p2)
        {
            float min = (_p1 < _p2 ? _p1 : _p2);

            return min;
        }

        public static float Percent(int _total, int _val)
        {
            if (_total <= 0)
                return 0.0f;

            float result = (_val / _total * 100.0f);

            return result;
        }

        public static float PercentClamp(int _total, int _val)
        {
            if (_total <= 0)
                return 0.0f;

            float result = (_val / _total * 100.0f);

            if (result < 0.0f)
            {
                result = 0.0f;
            }
            else if (result > 100.0f)
            {
                result = 100.0f;
            }

            return result;
        }

        public static float Percent(float _total, float _val)
        {
            if (_total <= 0)
                return 0.0f;

            float result = (_val / _total * 100.0f);

            return result;
        }

        public static float PercentClamp(float _total, float _val)
        {
            if (_total <= 0)
                return 0.0f;

            float result = (_val / _total * 100.0f);

            if (result < 0.0f)
            {
                result = 0.0f;
            }
            else if (result > 100.0f)
            {
                result = 100.0f;
            }

            return result;
        }

        public static int MinusClamp(int _p1, int _p2, int _min, int _max)
        {
            int result = _p1 - _p2;

            if (result < _min)
            {
                result = _min;
            }
            else if (result > _max)
            {
                result = _max;
            }

            return result;
        }

        #region Easing Curves

        public static float clerp(float start, float end, float value)
        {
            float min = 0.0f;
            float max = 360.0f;
            float half = Mathf.Abs((max - min) / 2.0f);
            float retval = 0.0f;
            float diff = 0.0f;
            if ((end - start) < -half)
            {
                diff = ((max - start) + end) * value;
                retval = start + diff;
            }
            else if ((end - start) > half)
            {
                diff = -((max - end) + start) * value;
                retval = start + diff;
            }
            else retval = start + (end - start) * value;
            return retval;
        }

        public static float spring(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }

        public static float EaseInQuad(float start, float end, float value)
        {
            end -= start;
            return end * value * value + start;
        }

        public static float EaseOutQuad(float start, float end, float value)
        {
            end -= start;
            return -end * value * (value - 2) + start;
        }

        public static float EaseInOutQuad(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * value * value + start;
            value--;
            return -end / 2 * (value * (value - 2) - 1) + start;
        }

        public static float EaseInCubic(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value + start;
        }

        public static float EaseOutCubic(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value + 1) + start;
        }

        public static float EaseInOutCubic(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * value * value * value + start;
            value -= 2;
            return end / 2 * (value * value * value + 2) + start;
        }

        public static float EaseInQuart(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value + start;
        }

        public static float EaseOutQuart(float start, float end, float value)
        {
            value--;
            end -= start;
            return -end * (value * value * value * value - 1) + start;
        }

        public static float EaseInOutQuart(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * value * value * value * value + start;
            value -= 2;
            return -end / 2 * (value * value * value * value - 2) + start;
        }

        public static float EaseInQuint(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value * value + start;
        }

        public static float EaseOutQuint(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value * value * value + 1) + start;
        }

        public static float EaseInOutQuint(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * value * value * value * value * value + start;
            value -= 2;
            return end / 2 * (value * value * value * value * value + 2) + start;
        }

        public static float EaseInSine(float start, float end, float value)
        {
            end -= start;
            return -end * Mathf.Cos(value / 1 * (Mathf.PI / 2)) + end + start;
        }

        public static float EaseOutSine(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Sin(value / 1 * (Mathf.PI / 2)) + start;
        }

        public static float EaseInOutSine(float start, float end, float value)
        {
            end -= start;
            return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
        }

        public static float EaseInExpo(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (value / 1 - 1)) + start;
        }

        public static float EaseOutExpo(float start, float end, float value)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
        }

        public static float EaseInOutExpo(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
            value--;
            return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
        }

        public static float EaseInCirc(float start, float end, float value)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
        }

        public static float EaseOutCirc(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * Mathf.Sqrt(1 - value * value) + start;
        }

        public static float EaseInOutCirc(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
            value -= 2;
            return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
        }

        /* GFX47 MOD START */
        public static float EaseInBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            return end - EaseOutBounce(0, end, d - value) + start;
        }
        /* GFX47 MOD END */

        /* GFX47 MOD START */
        //private float bounce(float start, float end, float value){
        public static float EaseOutBounce(float start, float end, float value)
        {
            value /= 1f;
            end -= start;
            if (value < (1 / 2.75f))
            {
                return end * (7.5625f * value * value) + start;
            }
            else if (value < (2 / 2.75f))
            {
                value -= (1.5f / 2.75f);
                return end * (7.5625f * (value) * value + .75f) + start;
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25f / 2.75f);
                return end * (7.5625f * (value) * value + .9375f) + start;
            }
            else
            {
                value -= (2.625f / 2.75f);
                return end * (7.5625f * (value) * value + .984375f) + start;
            }
        }
        /* GFX47 MOD END */

        /* GFX47 MOD START */
        public static float EaseInOutBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            if (value < d / 2) return EaseInBounce(0, end, value * 2) * 0.5f + start;
            else return EaseOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
        }
        /* GFX47 MOD END */

        public static float EaseInBack(float start, float end, float value)
        {
            end -= start;
            value /= 1;
            float s = 1.70158f;
            return end * (value) * value * ((s + 1) * value - s) + start;
        }

        public static float EaseOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value = (value / 1) - 1;
            return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
        }

        public static float EaseInOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value /= .5f;
            if ((value) < 1)
            {
                s *= (1.525f);
                return end / 2 * (value * value * (((s) + 1) * value - s)) + start;
            }
            value -= 2;
            s *= (1.525f);
            return end / 2 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
        }

        public static float punch(float amplitude, float value)
        {
            float s = 9;
            if (value == 0)
            {
                return 0;
            }
            if (value == 1)
            {
                return 0;
            }
            float period = 1 * 0.3f;
            s = period / (2 * Mathf.PI) * Mathf.Asin(0);
            return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
        }

        /* GFX47 MOD START */
        public static float EaseInElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d) == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        }
        /* GFX47 MOD END */

        /* GFX47 MOD START */
        //private float elastic(float start, float end, float value){
        public static float EaseOutElastic(float start, float end, float value)
        {
            /* GFX47 MOD END */
            //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d) == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
        }

        /* GFX47 MOD START */
        public static float EaseInOutElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d / 2) == 2) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
            return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
        }
        /* GFX47 MOD END */

        #endregion
    }
}