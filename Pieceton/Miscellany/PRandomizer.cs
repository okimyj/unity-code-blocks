using UnityEngine;

namespace Pieceton.Misc
{
    public static class PRandomizer
    {
        private static int _seed = 0x0f0f0f0f;

        private static int _m = 2147483399;
        private static int _a = 40692;
        private static int _q = _m / _a;
        private static int _r = _m % _a;
        private static int _rMax = _m - 1;

        public static void Reset(int value = 0x0f0f0f0f)
        {
            _seed = value;
        }

        public static int Rand()
        {
            _seed = _a * (_seed % _q) - _r * (_seed / _q);
            if (_seed < 0)
                _seed += _m;

            return _seed;
        }

        public static int Rand(int minValue, int maxValue)
        {
            if (minValue == maxValue)
                return minValue;

            //최소와 최대값이 포함된 값이 나온다
            if (minValue > maxValue)
            {
                Debug.LogWarning("invalid minValue = " + minValue);
                return maxValue;
            }

            int temp = (maxValue + 1) - minValue;
            return ((Rand() % temp) + minValue);
        }

        public static uint Rand(uint minValue, uint maxValue)
        {
            if (minValue == maxValue)
                return minValue;

            //최소와 최대값이 포함된 값이 나온다
            if (minValue > maxValue)
            {
                Debug.LogWarning("invalid minValue = " + minValue);
                return maxValue;
            }

            uint temp = (maxValue + 1) - minValue;
            return (((uint)Rand() % temp) + minValue);
        }

        public static int Rand_Percentage() //1~100
        {
            return Rand(1, 100);
        }

        public static double FRand()
        {
            return (double)(Rand() * (1.0 / (double)_rMax));
        }

        public static double FRand(double minValue, double maxValue)
        {
            int tempMin = (int)(minValue * 1000.0);
            int tempMax = (int)(maxValue * 1000.0);

            return (double)(Rand(tempMin, tempMax) * 0.001);
        }

        public static double FRand_Rate() //0.0 ~ 1.0
        {
            return FRand(0.0, 1.0);
        }

        private static int RandMax()
        {
            return _rMax;
        }
    }
}