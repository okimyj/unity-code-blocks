using System;
using UnityEngine;
using System.Collections;

namespace Pieceton.Misc
{
    public static class PCurrency
    {
        private const long MICROS = 1000000;

        public static string MakeFormattedPrice(string _a_currency, string _a_micro_price)
        {
            Debug.LogFormat("MakeFormattedPrice({0}, {1})", _a_currency, _a_micro_price);

            string formattedString = "";

#if UNITY_IPHONE
            double tempAmount = 0;

            try
            {
                tempAmount = Convert.ToDouble(_a_micro_price);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Invalid micro price. parsing error. {0}", e);
                return "";
            }

            double dAmount = tempAmount / MICROS;

            formattedString = GetFormattedCurrencyString(_a_currency, dAmount);
            return formattedString;
#else
            long tempAmount = 0;
            try
            {
                tempAmount = Convert.ToInt64(_a_micro_price);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Invalid micro price. parsing error. {0}", e);
                return "";
            }

            double dAmount = tempAmount / (double)MICROS;

            if (_a_currency.Equals("KRW"))
            {
                //string customAmountMicros = (tempAmount * 1.1).ToString();
                dAmount = dAmount * 1.1;

                string sFormat = GetCurrencyFormat(_a_currency);
                string sAmount = string.Format(sFormat, dAmount);

                formattedString = string.Format("₩ {0}", sAmount);
            }
            else if (_a_currency.Equals("RUB"))
            {
                //string customAmountMicros = (tempAmount * 1.18).ToString();
                dAmount = dAmount * 1.18;

                string sFormat = GetCurrencyFormat(_a_currency);
                string sAmount = string.Format(sFormat, dAmount);

                formattedString = string.Format("{0}p.", sAmount);
            }
            else
            {
                formattedString = GetFormattedCurrencyString(_a_currency, dAmount);
            }
#endif

            return formattedString;
        }

        public static string MakeFormattedPrice(string _a_currency, decimal _price)
        {
            Debug.LogFormat("MakeFormattedPrice({0}, {1})", _a_currency, _price);

            string formattedString = "";

#if UNITY_IPHONE
            double tempAmount = 0;

            try
            {
                tempAmount = Convert.ToDouble(_a_currency);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Invalid micro price. parsing error. {0}", e);
                return "";
            }

            double dAmount = tempAmount / MICROS;

            formattedString = GetFormattedCurrencyString(_a_currency, dAmount);
            return formattedString;
#else
            double dAmount = (double)_price;

            if (_a_currency.Equals("KRW"))
            {
                //string customAmountMicros = (tempAmount * 1.1).ToString();
                dAmount = dAmount * 1.1;

                string sFormat = GetCurrencyFormat(_a_currency);
                string sAmount = string.Format(sFormat, dAmount);

                formattedString = string.Format("₩ {0}", sAmount);
            }
            else if (_a_currency.Equals("RUB"))
            {
                //string customAmountMicros = (tempAmount * 1.18).ToString();
                dAmount = dAmount * 1.18;

                string sFormat = GetCurrencyFormat(_a_currency);
                string sAmount = string.Format(sFormat, dAmount);

                formattedString = string.Format("{0}p.", sAmount);
            }
            else
            {
                formattedString = GetFormattedCurrencyString(_a_currency, dAmount);
            }
#endif

            return formattedString;
        }

        public static string MakeFormattedPrice(string _a_currency, double _a_none_micro_price)
        {
            return MakeFormattedPrice(_a_currency, ConvertToPriceString(_a_none_micro_price));
        }

        public static string MakeFormattedPrice(string _a_currency, float _a_none_micro_price)
        {
            return MakeFormattedPrice(_a_currency, ConvertToPriceString(_a_none_micro_price));
        }

        public static string GetFormattedCurrencyString(string currencyCode, double amount)
        {
            string sFormat = GetCurrencyFormat(currencyCode);
            string sAmount = string.Format(sFormat, amount);

            if (IsDotToComma(currencyCode))
            {
                sAmount = sAmount.Replace(".", ",");
            }

            return GetFormattedCurrencyString(currencyCode, sAmount);
        }

        private static string ConvertToPriceString(double _none_micro_price)
        {
            string tmpPrice = _none_micro_price.ToString();
#if UNITY_ANDROID
            if (_none_micro_price < MICROS)
            {
                long tmpLongPrice = (long)(_none_micro_price * MICROS);
                tmpPrice = tmpLongPrice.ToString();
            }
#endif
            return tmpPrice;
        }

        private static bool IsDotToComma(string currencyCode)
        {
            switch (currencyCode)
            {
                case "DKK":
                case "NOK":
                case "SEK":
                case "TRY":
                case "EUR":
                    return true;
            }

            return false;
        }

        public static string GetCurrencyFormat(string currencyCode)
        {
            if (IsDotToComma(currencyCode))
                return "{0:#.00}";

            switch (currencyCode)
            {
                case "KRW":
                case "RUB":
                case "INR":
                case "IDR":
                case "TWD":
                case "JPY":
                    return "{0:##,###}";
            }

            // 0.99 => "{0:##,###.00}"; => .99
            // 0.99 => "{0:##,##0.00}"; => 0.99
            return "{0:##,##0.00}";
            //---------------------------------
        }

        public static string GetFormattedCurrencyString(string currencyCode, string amount)
        {
            string format = currencyCode + " {0}";

#if UNITY_IPHONE
            switch (currencyCode)
            {
                case "KRW": format = "₩ {0}"; break;
                case "USD": format = "$ {0}"; break;         // 미국 달러
                case "CAD": format = "${0}"; break;         // 캐나다 달러
                case "EUR": format = "{0} €"; break;         // 유로화
                case "DKK": format = "{0} kr"; break;        // 덴마크 크로네
                case "NOK": format = "{0} kr"; break;        // 노르웨이 크로네
                case "CHF": format = "CHF {0}"; break;       // 스위스 프랑
                case "SEK": format = "{0} kr"; break;        // 스웨덴 크로나
                case "TRY": format = "{0} TL"; break;        // 터키 리라
                case "GBP": format = "£{0}"; break;          // 영국 파운드
                case "RUB": format = "{0}p."; break;         // 러시아 루블
                case "INR": format = "Rs. {0}"; break;       // 인도 루피
                case "ILS": format = "₪ {0}"; break;         // 이스라엘 세켈
                case "SAR": format = "SR {0}"; break;        // 사우디 아라비아
                case "ZAR": format = "R{0}"; break;          // 남아프리카 공화국 랜드
                case "AED": format = "AED {0}"; break;       // 아랍에미리트 디르함
                case "AUD": format = "${0}"; break;          // 호주 달러
                case "HKD": format = "$ {0}"; break;        // 홍콩 달러
                case "IDR": format = "Rp {0}"; break;       // 인도네시아 루피아
                case "NZD": format = "${0}"; break;          // 뉴질랜드 달러
                case "SGD": format = "${0}"; break;         // 싱가포르 달러
                case "TWD": format = "NT$ {0}"; break;       // 대만 달러
                case "CNY": format = "¥{0}"; break;          // 중국 위안
                case "JPY": format = "¥{0}"; break;          // 일본 엔
                case "MXN": format = "${0}"; break;          // 멕시코 페소
                case "THB": format = "฿{0}"; break;       // 태국 바트
                case "EGP": format = "EGP{0}";break;       // 이집트 파운드
                case "KZT": format = "{0}₸"; break;         // 카자흐스탄 텡게
                case "MYR": format = "RM{0}"; break;        // 말레이시아 링깃
                case "NGN": format = "₦ {0}"; break;        // 나이지리아 나이라
                case "PKR": format = "Rs.{0}"; break;       // 파키스탄 루피
                case "PHP": format = "₱ {0}"; break;        // 필리핀 페소
                case "QAR": format = "{0} QAR"; break;      // 카타르 리얄 달러
                case "TZS": format = "{0} TZS"; break;      // 탄자니아 실링
                case "VND": format = "{0}₫"; break;         // 베트남 동
            }
#endif

            string formatted = string.Format(format, amount);
            return formatted;
        }
    }
}