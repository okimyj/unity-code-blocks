using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Globalization;

using System.Runtime.InteropServices;


namespace Pieceton.Misc
{
    public enum RemainTime
    {
        OneMinute = 0,
        Minutes,
        Hours,
        Days,
    }

    public static class PDateTime
    {
        public static void GetRemainTime(DateTime _now, DateTime _expire, out RemainTime _type, out int _val)
        {
            _type = RemainTime.OneMinute;
            _val = 0;

            if (_now > _expire)
            {
                PLog.AnyLogError("PDateTime::GetRemainTime() Current time is greater than expiration time.");
                return;
            }

            TimeSpan ts = _expire - _now;
            if (ts.Days > 0)
            {
                _type = RemainTime.Days;
                _val = ts.Days;
            }
            else if (ts.Hours > 0)
            {
                _type = RemainTime.Hours;
                _val = ts.Hours;
            }
            else if (ts.Minutes > 0)
            {
                _type = RemainTime.Minutes;
                _val = ts.Minutes;
            }
            else
            {
                _type = RemainTime.OneMinute;
                _val = ts.Seconds;
            }
        }

        public static CultureInfo GetCultureInfo(SystemLanguage _language)
        {
            CultureInfo cul2 = CultureInfo.GetCultures(CultureTypes.AllCultures).
                    LastOrDefault(x => x.EnglishName.Contains(Enum.GetName(_language.GetType(), _language)));

            return cul2;

        }

        public static string CurCultureFormattedDateTime(DateTime _date)
        {
            return FormattedDateTime(_date, GetCultureInfo(Application.systemLanguage));
        }

        public static string FormattedDateTime(DateTime _date, string _culture)
        {
            if (string.IsNullOrEmpty(_culture))
                return "";

            return FormattedDateTime(_date, new CultureInfo(_culture));
        }

        public static string FormattedDateTime(DateTime _date, CultureInfo _culture)
        {
            try
            {
                return _date.ToString("g", _culture);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Not suppoted {0}. so used en-US.\n{1}", _culture, e);
            }

            return _date.ToString("g", new CultureInfo("en-US"));
        }

        public static DateTime UtcToLocal(long _utc, string _culture)
        {
            return UtcToLocal(new DateTime(_utc), _culture);
        }

        public static DateTime UtcToLocal(DateTime _utc, string _culture)
        {
            return DateTime.Parse(_utc.ToString(), new CultureInfo(_culture)).ToLocalTime();
        }

        // 이번주의 특정요일
        public static DateTime ThisDayOfWeek(DayOfWeek _day)
        {
            return DateTime.Today.AddDays(Convert.ToInt32(_day) - Convert.ToInt32(DateTime.Today.DayOfWeek));
        }

        // 다음주의 특정요일
        public static DateTime NextDayOfWeek(DayOfWeek _day)
        {
            return DateTime.Today.AddDays((Convert.ToInt32(_day) - Convert.ToInt32(DateTime.Today.DayOfWeek)) + 7);
        }


        public static long DiffDays(DateTime prevDate, DateTime nextDate)
        {
            return (nextDate.Date - prevDate.Date).Days;
        }

        public static long DiffHours(DateTime prevDate, DateTime nextDate)
        {
            return (int)(nextDate.Date - prevDate.Date).TotalHours;
        }

        public static long DiffMinutes(DateTime prevDate, DateTime nextDate)
        {
            return (int)(nextDate.Date - prevDate.Date).TotalMinutes;
        }


        public static bool IsBetween(DateTime _cur, DateTime _p1, DateTime _p2)
        {
            DateTime min = (_p1 < _p2 ? _p1 : _p2);
            DateTime max = (_p1 > _p2 ? _p1 : _p2);

            if (_cur >= min && _cur <= max)
                return true;

            return false;
        }
    }
}