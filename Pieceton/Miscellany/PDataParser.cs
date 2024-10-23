using System;

namespace Pieceton.Misc
{
    public static class PDataParser
    {
        public static bool IsDefinedEnum<T>(string _at_value) where T : IConvertible
        {
            bool result = false;

            try { result = System.Enum.IsDefined(typeof(T), _at_value); }
            catch { }

            return result;
        }

        public static T ParseEnum<T>(string _in_value, T _def_value) where T : IConvertible
        {
            T result = _def_value;

            if (IsDefinedEnum<T>(_in_value))
            {
                try { result = (T)System.Enum.Parse(typeof(T), _in_value); }
                catch { }
            }

            return result;
        }

        public static T ParseEnum<T>(int _in_value, T _def_value) where T : IConvertible
        {
            return ParseEnum<T>(_in_value.ToString(), _def_value);
        }

        public static string[] GetEnumNames<T>() where T : IConvertible
        {
            string[] result = null;

            try { result = System.Enum.GetNames(typeof(T)); }
            catch { }

            return result;
        }

        public static string GetEnumName<T>(T _at) where T : IConvertible
        {
            string result = string.Empty;

            try { result = System.Enum.GetName(typeof(T), _at); }
            catch { }

            return result;
        }

        public static bool ParseBool(string _in_value, bool _def_value)
        {
            bool result = _def_value;

            if (!string.IsNullOrEmpty(_in_value))
            {
                try { result = bool.Parse(_in_value); }
                catch { }
            }

            return result;
        }

        public static int ParseInt(string _in_value, int _def_value)
        {
            int result = _def_value;

            if (!string.IsNullOrEmpty(_in_value))
            {
                try { result = int.Parse(_in_value); }
                catch { }
            }

            return result;
        }

        public static uint ParseUInt(string _in_value, uint _def_value)
        {
            uint result = _def_value;

            if (!string.IsNullOrEmpty(_in_value))
            {
                try { result = uint.Parse(_in_value); }
                catch { }
            }

            return result;
        }

        public static long ParseLong(string _in_value, long _def_value)
        {
            long result = _def_value;

            if (!string.IsNullOrEmpty(_in_value))
            {
                try { result = long.Parse(_in_value); }
                catch { }
            }

            return result;
        }

        public static short ParseShort(string _in_value, short _def_value)
        {
            short result = _def_value;

            if (!string.IsNullOrEmpty(_in_value))
            {
                try { result = short.Parse(_in_value); }
                catch { }
            }

            return result;
        }

        public static ushort ParseUShort(string _in_value, ushort _def_value)
        {
            ushort result = _def_value;

            if (!string.IsNullOrEmpty(_in_value))
            {
                try { result = ushort.Parse(_in_value); }
                catch { }
            }

            return result;
        }

        public static ulong ParseULong(string _in_value, ulong _def_value)
        {
            ulong result = _def_value;

            if (!string.IsNullOrEmpty(_in_value))
            {
                try { result = ulong.Parse(_in_value); }
                catch { }
            }

            return result;
        }

        public static float ParseFloat(string _in_value, float _def_value)
        {
            float result = _def_value;

            if (!string.IsNullOrEmpty(_in_value))
            {
                try { result = float.Parse(_in_value); }
                catch { }
            }

            return result;
        }

        public static double ParseDouble(string _in_value, double _def_value)
        {
            double result = _def_value;

            if (!string.IsNullOrEmpty(_in_value))
            {
                try { result = double.Parse(_in_value); }
                catch { }
            }

            return result;
        }


        public static bool ParseString(out string _result, string _data, char _split_char, int _index, string _default)
        {
            _result = _default;

            if (string.IsNullOrEmpty(_data))
                return false;

            string[] splits = _data.Split(_split_char);
            if (!IsValidParseData(splits, _index))
                return false;

            _result = splits[_index];
            return true;
        }

        public static bool ParseBool(out bool _result, string _data, char _split_char, int _index, bool _default)
        {
            _result = _default;

            if (string.IsNullOrEmpty(_data))
                return false;

            string[] splits = _data.Split(_split_char);
            if (!IsValidParseData(splits, _index))
                return false;

            _result = PDataParser.ParseBool(splits[_index], _default);
            return true;
        }

        public static bool ParseShort(out short _result, string _data, char _split_char, int _index, short _default)
        {
            _result = _default;

            if (string.IsNullOrEmpty(_data))
                return false;

            string[] splits = _data.Split(_split_char);
            if (!IsValidParseData(splits, _index))
                return false;

            _result = PDataParser.ParseShort(splits[_index], _default);
            return true;
        }

        public static bool ParseUShort(out ushort _result, string _data, char _split_char, int _index, ushort _default)
        {
            _result = _default;

            if (string.IsNullOrEmpty(_data))
                return false;

            string[] splits = _data.Split(_split_char);
            if (!IsValidParseData(splits, _index))
                return false;

            _result = PDataParser.ParseUShort(splits[_index], _default);
            return true;
        }

        public static bool ParseInt(out int _result, string _data, char _split_char, int _index, int _default)
        {
            _result = _default;

            if (string.IsNullOrEmpty(_data))
                return false;

            string[] splits = _data.Split(_split_char);
            if (!IsValidParseData(splits, _index))
                return false;

            _result = PDataParser.ParseInt(splits[_index], _default);
            return true;
        }

        public static bool ParseUInt(out uint _result, string _data, char _split_char, int _index, uint _default)
        {
            _result = _default;

            if (string.IsNullOrEmpty(_data))
                return false;

            string[] splits = _data.Split(_split_char);
            if (!IsValidParseData(splits, _index))
                return false;

            _result = PDataParser.ParseUInt(splits[_index], _default);
            return true;
        }

        public static bool ParseLong(out long _result, string _data, char _split_char, int _index, long _default)
        {
            _result = _default;

            if (string.IsNullOrEmpty(_data))
                return false;

            string[] splits = _data.Split(_split_char);
            if (!IsValidParseData(splits, _index))
                return false;

            _result = PDataParser.ParseLong(splits[_index], _default);
            return true;
        }

        public static bool ParseULong(out ulong _result, string _data, char _split_char, int _index, ulong _default)
        {
            _result = _default;

            if (string.IsNullOrEmpty(_data))
                return false;

            string[] splits = _data.Split(_split_char);
            if (!IsValidParseData(splits, _index))
                return false;

            _result = PDataParser.ParseULong(splits[_index], _default);
            return true;
        }

        public static bool ParseFloat(out float _result, string _data, char _split_char, int _index, float _default)
        {
            _result = _default;

            if (string.IsNullOrEmpty(_data))
                return false;

            string[] splits = _data.Split(_split_char);
            if (!IsValidParseData(splits, _index))
                return false;

            _result = PDataParser.ParseFloat(splits[_index], _default);
            return true;
        }

        public static bool ParseDouble(out double _result, string _data, char _split_char, int _index, double _default)
        {
            _result = _default;

            if (string.IsNullOrEmpty(_data))
                return false;

            string[] splits = _data.Split(_split_char);
            if (!IsValidParseData(splits, _index))
                return false;

            _result = PDataParser.ParseDouble(splits[_index], _default);
            return true;
        }

        public static bool ParseEnum<T>(out T _result, string _data, char _split_char, int _index, T _default) where T : IConvertible
        {
            _result = _default;

            if (string.IsNullOrEmpty(_data))
                return false;

            string[] splits = _data.Split(_split_char);
            if (!IsValidParseData(splits, _index))
                return false;

            _result = PDataParser.ParseEnum<T>(splits[_index], _default);
            return true;
        }

        public static bool IsValidParseData(string[] _split_data, int _index)
        {
            if (null == _split_data)
            {
                PLog.AnyLogError("PDataParser::IsValidParseData() _split_data is null");
                return false;
            }

            if ((_index < 0 || _index >= _split_data.Length))
            {
                PLog.AnyLogError("PDataParser::IsValidParseData() Out of length. data length = '{0}', used index = '{1}'", _split_data.Length, _index);
                return false;
            }

            if (string.IsNullOrEmpty(_split_data[_index]))
            {
                PLog.AnyLogError("PDataParser::IsValidParseData() Empty data at. used index = '{0}'", _index);
                return false;
            }

            return true;
        }
    }
}