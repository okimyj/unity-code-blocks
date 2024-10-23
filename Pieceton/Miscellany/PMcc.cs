using UnityEngine;
using System;
using System.Collections.Generic;

// 출처 : https://github.com/gwerz/mccmnc/blob/master/MccMnc.cs

namespace Pieceton.Misc
{
    public class PMcc
    {
        private static Dictionary<short, string> mcc2country = new Dictionary<short, string>();
        private static Dictionary<string, short> country2mcc = new Dictionary<string, short>();
        private static Dictionary<string, string> hni2carrier = new Dictionary<string, string>();

        static PMcc()
        {
            initMccCountry();
            initMccCarrier();
        }

        // input param : //iso alpha-2
        public static short GetMcc(string _country_code)
        {
            short val;
            if (country2mcc.TryGetValue(_country_code, out val))
                return val;

            return 0;
        }

        private short mcc;
        private short mnc;

        public PMcc(short mcc, short mnc)
        {
            this.mcc = mcc;
            this.mnc = mnc;
        }

        public short getMcc() { return mcc; }
        public short getMnc() { return mnc; }
        public string isoCountryCode() { return PMcc.isoCountryCode(mcc); }
        public string carrier() { return PMcc.carrier(mcc, mnc); }

        /**
            * translate mcc to iso 2-letter country code
            * @param mcc
            * @return iso 2-letter country code
            */
        public static string isoCountryCode(short mcc)
        {
            if (!mcc2country.ContainsKey(mcc))
                return "Country Unknown";
            return mcc2country[mcc];
        }

        /**
            * translate specificed mcc/mnc tuple to carrier display name
            * @param mcc
            * @param mnc
            * @return carrier display name
            */
        public static string carrier(short mcc, short mnc)
        {
            string hni = mcc + "-" + mnc;
            if (!hni2carrier.ContainsKey(hni))
                return "Destination Unknown";
            return hni2carrier[hni];
        }

        private static void AddCode(short _mcc, string _contry)
        {
            if (!mcc2country.ContainsKey(_mcc))
            {
                mcc2country.Add(_mcc, _contry);
            }
            else
            {
                Debug.LogWarningFormat("[MccCode] Duplicated MCC. mcc = '{0}', contry = '{1}'", _mcc, _contry);
            }

            if (!country2mcc.ContainsKey(_contry))
            {
                country2mcc.Add(_contry, _mcc);
            }
            else
            {
                Debug.LogWarningFormat("[MccCode] Duplicated Country. mcc = '{0}', country = '{1}'", _mcc, _contry);
            }
        }

        private static void AddCarrier(string _key, string _val)
        {
            if (hni2carrier.ContainsKey(_key))
            {
                Debug.LogWarningFormat("[MccCode] Duplicated Carrier. key = '{0}', value = '{1}'", _key, _val);
                return;
            }

            hni2carrier.Add(_key, _val);
        }

        private static void initMccCountry()
        {
            #region
            mcc2country.Clear();
            country2mcc.Clear();

            AddCode(289, "ge");//Abkhazia
            AddCode(412, "af");//Afghanistan
            AddCode(276, "al");//Albania
            AddCode(603, "dz");//Algeria
            AddCode(544, "as");//American Samoa
            AddCode(213, "ad");//Andorra
            AddCode(631, "ao");//Angola
            AddCode(365, "ai");//Anguilla
            AddCode(344, "ag");//Antigua and Barbuda
            AddCode(722, "ar");//Argentina Republic
            AddCode(283, "am");//Armenia
            AddCode(363, "aw");//Aruba
            AddCode(505, "au");//Australia
            AddCode(232, "at");//Austria
            AddCode(400, "az");//Azerbaijan
            AddCode(364, "bs");//Bahamas
            AddCode(426, "bh");//Bahrain
            AddCode(470, "bd");//Bangladesh
            AddCode(342, "bb");//Barbados
            AddCode(257, "by");//Belarus
            AddCode(206, "be");//Belgium
            AddCode(702, "bz");//Belize
            AddCode(616, "bj");//Benin
            AddCode(350, "bm");//Bermuda
            AddCode(402, "bt");//Bhutan
            AddCode(736, "bo");//Bolivia
            AddCode(218, "ba");//Bosnia & Herzegov.
            AddCode(652, "bw");//Botswana
            AddCode(724, "br");//Brazil
            AddCode(348, "vg");//British Virgin Islands
            AddCode(528, "bn");//Brunei Darussalam
            AddCode(284, "bg");//Bulgaria
            AddCode(613, "bf");//Burkina Faso
            AddCode(414, "mm");//Burma
            AddCode(642, "bi");//Burundi
            AddCode(456, "kh");//Cambodia
            AddCode(624, "cm");//Cameroon
            AddCode(302, "ca");//Canada
            AddCode(625, "cv");//Cape Verde
            AddCode(346, "ky");//Cayman Islands
            AddCode(623, "cf");//Central African Rep.
            AddCode(622, "td");//Chad
            AddCode(730, "cl");//Chile
            AddCode(460, "cn");//China
            AddCode(732, "co");//Colombia
            AddCode(654, "km");//Comoros
            AddCode(630, "cd");//Congo, Dem. Rep.
            AddCode(629, "cg");//Congo, Republic
            AddCode(548, "ck");//Cook Islands
            AddCode(712, "cr");//Costa Rica
            AddCode(219, "hr");//Croatia
            AddCode(368, "cu");//Cuba
            AddCode(280, "cy");//Cyprus
            AddCode(230, "cz");//Czech Rep.
            AddCode(238, "dk");//Denmark
            AddCode(638, "dj");//Djibouti
            AddCode(366, "dm");//Dominica
            AddCode(370, "do");//Dominican Republic
            AddCode(740, "ec");//Ecuador
            AddCode(602, "eg");//Egypt
            AddCode(706, "sv");//El Salvador
            AddCode(627, "gq");//Equatorial Guinea
            AddCode(657, "er");//Eritrea
            AddCode(248, "ee");//Estonia
            AddCode(636, "et");//Ethiopia
            AddCode(288, "fo");//Faroe Islands
            AddCode(542, "fj");//Fiji
            AddCode(244, "fi");//Finland
            AddCode(208, "fr");//France
            AddCode(340, "fg");//French Guiana
            AddCode(547, "pf");//French Polynesia
            AddCode(628, "ga");//Gabon
            AddCode(607, "gm");//Gambia
            AddCode(282, "ge");//Georgia
            AddCode(262, "de");//Germany
            AddCode(620, "gh");//Ghana
            AddCode(266, "gi");//Gibraltar
            AddCode(202, "gr");//Greece
            AddCode(290, "gl");//Greenland
            AddCode(352, "gd");//Grenada
            AddCode(310, "gu");//Guam
            AddCode(704, "gt");//Guatemala
            AddCode(611, "gn");//Guinea
            AddCode(632, "gw");//Guinea-Bissau
            AddCode(738, "gy");//Guyana
            AddCode(372, "ht");//Haiti
            AddCode(708, "hn");//Honduras
            AddCode(454, "hk");//Hongkong, China
            AddCode(216, "hu");//Hungary
            AddCode(274, "is");//Iceland
            AddCode(404, "in");//India
            AddCode(405, "in");//India
            AddCode(510, "id");//Indonesia

            //mcc2country.Add(901,"n/a");//International Networks

            AddCode(432, "ir");//Iran
            AddCode(418, "iq");//Iraq
            AddCode(272, "ie");//Ireland
                               //mcc2country.Add(425,"il");//Israel
            AddCode(222, "it");//Italy
            AddCode(612, "ci");//Ivory Coast
            AddCode(338, "jm");//Jamaica
            AddCode(440, "jp");//Japan
            AddCode(441, "jp");//Japan
            AddCode(416, "jo");//Jordan
            AddCode(401, "kz");//Kazakhstan
            AddCode(639, "ke");//Kenya
            AddCode(545, "ki");//Kiribati
            AddCode(467, "kp");//Korea N., Dem. People's Rep.
            AddCode(450, "kr");//Korea S, Republic of
            AddCode(419, "kw");//Kuwait
            AddCode(437, "kg");//Kyrgyzstan
            AddCode(457, "la");//Laos P.D.R.
            AddCode(247, "lv");//Latvia
            AddCode(415, "lb");//Lebanon
            AddCode(651, "ls");//Lesotho
            AddCode(618, "lr");//Liberia
            AddCode(606, "ly");//Libya
            AddCode(295, "li");//Liechtenstein
            AddCode(246, "lt");//Lithuania
            AddCode(270, "lu");//Luxembourg
            AddCode(455, "mo");//Macao, China
            AddCode(294, "mk");//Macedonia
            AddCode(646, "mg");//Madagascar
            AddCode(650, "mw");//Malawi
            AddCode(502, "my");//Malaysia
            AddCode(472, "mv");//Maldives
            AddCode(610, "ml");//Mali
            AddCode(278, "mt");//Malta
            AddCode(609, "mr");//Mauritania
            AddCode(617, "mu");//Mauritius
            AddCode(334, "mx");//Mexico
            AddCode(550, "fm");//Micronesia
            AddCode(259, "md");//Moldova
            AddCode(212, "mc");//Monaco
            AddCode(428, "mn");//Mongolia
            AddCode(354, "ms");//Montserrat
            AddCode(604, "ma");//Morocco
            AddCode(643, "mz");//Mozambique
            AddCode(649, "na");//Namibia
            AddCode(429, "np");//Nepal
            AddCode(204, "nl");//Netherlands
            AddCode(362, "an");//Netherlands Antilles
            AddCode(546, "nc");//New Caledonia
            AddCode(530, "nz");//New Zealand
            AddCode(614, "ne");//Niger
            AddCode(621, "ng");//Nigeria
            AddCode(242, "no");//Norway
            AddCode(422, "om");//Oman
            AddCode(410, "pk");//Pakistan
            AddCode(425, "ps");//Palestinian Territory
            AddCode(714, "pa");//Panama
            AddCode(537, "pg");//Papua New Guinea
            AddCode(744, "py");//Paraguay
            AddCode(716, "pe");//Peru
            AddCode(515, "ph");//Philippines
            AddCode(260, "pl");//Poland
            AddCode(268, "pt");//Portugal
            AddCode(427, "qa");//Qatar
            AddCode(647, "re");//Reunion
            AddCode(226, "ro");//Romania
            AddCode(250, "ru");//Russian Federation
            AddCode(635, "rw");//Rwanda
            AddCode(356, "kn");//Saint Kitts and Nevis
            AddCode(358, "lc");//Saint Lucia
            AddCode(549, "ws");//Samoa
            AddCode(292, "sm");//San Marino
            AddCode(626, "st");//Sao Tome & Principe
            AddCode(901, "n/a");//Satellite Networks
            AddCode(420, "sa");//Saudi Arabia
            AddCode(608, "sn");//Senegal
            AddCode(220, "rs");//Serbia 
            AddCode(293, "si");//Slovenia
            AddCode(540, "sb");//Solomon Islands
            AddCode(637, "so");//Somalia
            AddCode(655, "za");//South Africa
            AddCode(214, "es");//Spain
            AddCode(413, "lk");//Sri Lanka
            AddCode(308, "pm");//St. Pierre & Miquelon
            AddCode(360, "vc");//St. Vincent & Gren.
            AddCode(634, "sd");//Sudan
            AddCode(746, "sr");//Suriname
            AddCode(653, "sz");//Swaziland
            AddCode(240, "se");//Sweden
            AddCode(228, "ch");//Switzerland
            AddCode(417, "sy");//Syrian Arab Republic
            AddCode(466, "tw");//Taiwan
            AddCode(436, "tk");//Tajikistan
            AddCode(640, "tz");//Tanzania
            AddCode(520, "th");//Thailand
            AddCode(514, "tp");//Timor-Leste
            AddCode(615, "tg");//Togo
            AddCode(539, "to");//Tonga
            AddCode(374, "tt");//Trinidad and Tobago
            AddCode(605, "tn");//Tunisia
            AddCode(286, "tr");//Turkey
            AddCode(438, "tm");//Turkmenistan
            AddCode(641, "ug");//Uganda
            AddCode(255, "ua");//Ukraine
            AddCode(424, "ae");//United Arab Emirates
            AddCode(234, "uk");//United Kingdom
            AddCode(235, "uk");//United Kingdom
                               //mcc2country.Add(310,"us");//United States
            AddCode(311, "us");//United States
            AddCode(316, "us");//United States
            AddCode(748, "uy");//Uruguay
            AddCode(434, "uz");//Uzbekistan
            AddCode(541, "vu");//Vanuatu
            AddCode(734, "ve");//Venezuela
            AddCode(452, "vn");//Viet Nam
            AddCode(376, "vi");//Virgin Islands, U.S.
            AddCode(421, "ye");//Yemen
            AddCode(645, "zm");//Zambia
            AddCode(648, "zw");//Zimbabwe
            #endregion
        }

        private static void initMccCarrier()
        {
            hni2carrier.Clear();

            #region
            AddCarrier("289-88", "A-Mobile");
            AddCarrier("289-68", "A-Mobile");
            AddCarrier("289-67", "Aquafon");
            AddCarrier("412-88", "Afghan Telecom Corp. (AT)");
            AddCarrier("412-80", "Afghan Telecom Corp. (AT)");
            AddCarrier("412-1", "Afghan Wireless/AWCC");
            AddCarrier("412-40", "Areeba");
            AddCarrier("412-50", "Etisalat");
            AddCarrier("412-20", "Roshan");
            AddCarrier("276-1", "AMC Mobil");
            AddCarrier("276-3", "Eagle Mobile");
            AddCarrier("276-4", "PLUS Communication Sh.a");
            AddCarrier("276-2", "Vodafone");
            AddCarrier("603-1", "ATM Mobils");
            AddCarrier("603-2", "Orascom / DJEZZY");
            AddCarrier("603-3", "Wataniya / Nedjma");
            AddCarrier("544-11", "Blue Sky Communications");
            AddCarrier("213-3", "Mobiland");
            AddCarrier("631-4", "MoviCel");
            AddCarrier("631-2", "Unitel");
            AddCarrier("365-840", "Cable and Wireless");
            AddCarrier("365-10", "Digicell / Wireless Vent. Ltd");
            AddCarrier("344-30", "APUA PCS");
            AddCarrier("344-920", "C & W");
            AddCarrier("344-930", "Cing. Wirel./DigiCel");
            AddCarrier("722-310", "Claro/ CTI/AMX");
            AddCarrier("722-330", "Claro/ CTI/AMX");
            AddCarrier("722-320", "Claro/ CTI/AMX");
            AddCarrier("722-10", "Compania De Radiocomunicaciones Moviles SA");
            AddCarrier("722-70", "Movistar/Telefonica");
            AddCarrier("722-20", "Nextel");
            AddCarrier("722-341", "Telecom Personal S.A.");
            AddCarrier("283-1", "ArmenTel/Beeline");
            AddCarrier("283-4", "Karabakh Telecom");
            AddCarrier("283-10", "Orange");
            AddCarrier("283-5", "Vivacell");
            AddCarrier("363-20", "Digicel");
            AddCarrier("363-1", "Setar GSM");
            AddCarrier("505-14", "AAPT Ltd.");
            AddCarrier("505-24", "Advanced Comm Tech Pty.");
            AddCarrier("505-9", "Airnet Commercial Australia Ltd..");
            AddCarrier("505-4", "Department of Defense");
            AddCarrier("505-26", "Dialogue Communications Pty Ltd");
            AddCarrier("505-12", "H3G Ltd.");
            AddCarrier("505-6", "H3G Ltd.");
            AddCarrier("505-88", "Localstar Holding Pty. Ltd");
            AddCarrier("505-19", "Lycamobile Pty Ltd");
            AddCarrier("505-8", "Railcorp/Vodafone");
            AddCarrier("505-99", "Railcorp/Vodafone");
            AddCarrier("505-13", "Railcorp/Vodafone");
            AddCarrier("505-90", "Singtel Optus");
            AddCarrier("505-2", "Singtel Optus");
            AddCarrier("505-1", "Telstra Corp. Ltd.");
            AddCarrier("505-11", "Telstra Corp. Ltd.");
            AddCarrier("505-71", "Telstra Corp. Ltd.");
            AddCarrier("505-72", "Telstra Corp. Ltd.");
            AddCarrier("505-5", "The Ozitel Network Pty.");
            AddCarrier("505-16", "Victorian Rail Track Corp. (VicTrack)");
            AddCarrier("505-7", "Vodafone");
            AddCarrier("505-3", "Vodafone");
            AddCarrier("232-11", "A1 MobilKom");
            AddCarrier("232-2", "A1 MobilKom");
            AddCarrier("232-9", "A1 MobilKom");
            AddCarrier("232-1", "A1 MobilKom");
            AddCarrier("232-15", "T-Mobile/Telering");
            AddCarrier("232-0", "Fix Line");
            AddCarrier("232-10", "H3G");
            AddCarrier("232-14", "H3G");
            AddCarrier("232-12", "Orange/One Connect");
            AddCarrier("232-6", "Orange/One Connect");
            AddCarrier("232-5", "Orange/One Connect");
            AddCarrier("232-4", "T-Mobile/Telering");
            AddCarrier("232-3", "T-Mobile/Telering");
            AddCarrier("232-7", "T-Mobile/Telering");
            AddCarrier("232-8", "Telefonica");
            AddCarrier("400-1", "Azercell Telekom B.M.");
            AddCarrier("400-4", "Azerfon.");
            AddCarrier("400-3", "Caspian American Telecommunications LLC (CATEL)");
            AddCarrier("400-2", "J.V. Bakcell GSM 2000");
            AddCarrier("364-30", "Bahamas Telco. Comp.");
            AddCarrier("364-390", "Bahamas Telco. Comp.");
            AddCarrier("364-39", "Bahamas Telco. Comp.");
            AddCarrier("364-3", "Smart Communications");
            AddCarrier("426-1", "Batelco");
            AddCarrier("426-2", "MTC Vodafone");
            AddCarrier("426-4", "VIVA");
            AddCarrier("470-2", "Robi/Aktel");
            AddCarrier("470-5", "Citycell");
            AddCarrier("470-6", "Citycell");
            AddCarrier("470-1", "GrameenPhone");
            AddCarrier("470-3", "Orascom");
            AddCarrier("470-4", "TeleTalk");
            AddCarrier("470-7", "Airtel/Warid");
            AddCarrier("342-600", "C & W BET Ltd.");
            AddCarrier("342-810", "Cingular Wireless");
            AddCarrier("342-750", "Digicel");
            AddCarrier("342-50", "Digicel");
            AddCarrier("342-820", "Sunbeach");
            AddCarrier("257-3", "BelCel JV");
            AddCarrier("257-4", "BeST");
            AddCarrier("257-1", "Mobile Digital Communications");
            AddCarrier("257-2", "MTS");
            AddCarrier("206-20", "Base/KPN");
            AddCarrier("206-1", "Belgacom/Proximus");
            AddCarrier("206-10", "Mobistar/Orange");
            AddCarrier("206-2", "SNCT/NMBS");
            AddCarrier("206-5", "Telenet BidCo NV");
            AddCarrier("702-67", "DigiCell");
            AddCarrier("702-68", "International Telco (INTELCO)");
            AddCarrier("616-4", "Bell Benin/BBCOM");
            AddCarrier("616-2", "Etisalat/MOOV");
            AddCarrier("616-5", "GloMobile");
            AddCarrier("616-1", "Libercom");
            AddCarrier("616-3", "MTN/Spacetel");
            AddCarrier("350-0", "Bermuda Digital Communications Ltd (BDC)");
            AddCarrier("350-99", "CellOne Ltd");
            AddCarrier("350-10", "DigiCel / Cingular");
            AddCarrier("350-2", "M3 Wireless Ltd");
            AddCarrier("350-1", "Telecommunications (Bermuda & West Indies) Ltd (Digicel Bermuda)");
            AddCarrier("402-11", "B-Mobile");
            AddCarrier("402-17", "Bhutan Telecom Ltd (BTL)");
            AddCarrier("402-77", "TashiCell");
            AddCarrier("736-2", "Entel Pcs");
            AddCarrier("736-1", "Nuevatel");
            AddCarrier("736-3", "TELECEL BOLIVIA");
            AddCarrier("362-91", "United Telecommunications Services NV (UTS)");
            AddCarrier("218-90", "BH Mobile");
            AddCarrier("218-3", "Eronet Mobile");
            AddCarrier("218-5", "M-Tel");
            AddCarrier("652-4", "beMOBILE");
            AddCarrier("652-1", "Mascom Wireless (Pty) Ltd.");
            AddCarrier("652-2", "Orange");
            AddCarrier("724-12", "Claro/Albra/America Movil");
            AddCarrier("724-38", "Claro/Albra/America Movil");
            AddCarrier("724-5", "Claro/Albra/America Movil");
            AddCarrier("724-1", "Vivo S.A./Telemig");
            AddCarrier("724-34", "CTBC Celular SA (CTBC)");
            AddCarrier("724-33", "CTBC Celular SA (CTBC)");
            AddCarrier("724-32", "CTBC Celular SA (CTBC)");
            AddCarrier("724-8", "TIM");
            AddCarrier("724-39", "Nextel (Telet)");
            AddCarrier("724-0", "Nextel (Telet)");
            AddCarrier("724-30", "Oi (TNL PCS / Oi)");
            AddCarrier("724-31", "Oi (TNL PCS / Oi)");
            AddCarrier("724-16", "Brazil Telcom");
            AddCarrier("724-24", "Amazonia Celular S/A");
            AddCarrier("724-15", "Sercontel Cel");
            AddCarrier("724-7", "CTBC/Triangulo");
            AddCarrier("724-19", "Vivo S.A./Telemig");
            AddCarrier("724-3", "TIM");
            AddCarrier("724-2", "TIM");
            AddCarrier("724-4", "TIM");
            AddCarrier("724-37", "Unicel do Brasil Telecomunicacoes Ltda");
            AddCarrier("724-23", "Vivo S.A./Telemig");
            AddCarrier("724-11", "Vivo S.A./Telemig");
            AddCarrier("724-10", "Vivo S.A./Telemig");
            AddCarrier("724-6", "Vivo S.A./Telemig");
            AddCarrier("348-570", "Caribbean Cellular");
            AddCarrier("348-770", "Digicel");
            AddCarrier("348-170", "LIME");
            AddCarrier("528-2", "b-mobile");
            AddCarrier("528-11", "Datastream (DTSCom)");
            AddCarrier("528-1", "Telekom Brunei Bhd (TelBru)");
            AddCarrier("284-6", "BTC Mobile EOOD (vivatel)");
            AddCarrier("284-3", "BTC Mobile EOOD (vivatel)");
            AddCarrier("284-5", "Cosmo Mobile EAD/Globul");
            AddCarrier("284-1", "MobilTel AD");
            AddCarrier("613-3", "TeleCel");
            AddCarrier("613-1", "TeleMob-OnaTel");
            AddCarrier("613-2", "AirTel/ZAIN/CelTel");
            AddCarrier("414-1", "Myanmar Post & Teleco.");
            AddCarrier("642-2", "Africel / Safaris");
            AddCarrier("642-8", "HiTs Telecom");
            AddCarrier("642-3", "Onatel / Telecel");
            AddCarrier("642-7", "Smart Mobile / LACELL");
            AddCarrier("642-1", "Spacetel / Econet");
            AddCarrier("642-82", "U-COM");
            AddCarrier("456-4", "Cambodia Advance Communications Co. Ltd (CADCOMMS)");
            AddCarrier("456-2", "Hello/Malaysia Telcom");
            AddCarrier("456-8", "Metfone");
            AddCarrier("456-18", "MFone/Camshin");
            AddCarrier("456-1", "Mobitel/Cam GSM");
            AddCarrier("456-3", "QB/Cambodia Adv. Comms.");
            AddCarrier("456-6", "Smart Mobile");
            AddCarrier("456-5", "Smart Mobile");
            AddCarrier("456-9", "Sotelco Ltd (Beeline Cambodia)");
            AddCarrier("624-1", "MTN");
            AddCarrier("624-2", "Orange");
            AddCarrier("302-652", "BC Tel Mobility");
            AddCarrier("302-630", "Bell Aliant");
            AddCarrier("302-651", "Bell Mobility");
            AddCarrier("302-610", "Bell Mobility");
            AddCarrier("302-670", "CityWest Mobility");
            AddCarrier("302-361", "Clearnet");
            AddCarrier("302-360", "Clearnet");
            AddCarrier("302-380", "DMTS Mobility");
            AddCarrier("302-710", "Globalstar Canada");
            AddCarrier("302-640", "Latitude Wireless");
            AddCarrier("302-370", "FIDO (Rogers AT&T/ Microcell)");
            AddCarrier("302-320", "mobilicity");
            AddCarrier("302-702", "MT&T Mobility");
            AddCarrier("302-655", "MTS Mobility");
            AddCarrier("302-660", "MTS Mobility");
            AddCarrier("302-701", "NB Tel Mobility");
            AddCarrier("302-703", "New Tel Mobility");
            AddCarrier("302-760", "Public Mobile");
            AddCarrier("302-657", "Quebectel Mobility");
            AddCarrier("302-720", "Rogers AT&T Wireless");
            AddCarrier("302-654", "Sask Tel Mobility");
            AddCarrier("302-680", "Sask Tel Mobility");
            AddCarrier("302-656", "Tbay Mobility");
            AddCarrier("302-653", "Telus Mobility");
            AddCarrier("302-220", "Telus Mobility");
            AddCarrier("302-500", "Videotron");
            AddCarrier("302-490", "WIND");
            AddCarrier("625-1", "CV Movel");
            AddCarrier("625-2", "T+ Telecom");
            AddCarrier("346-50", "Digicel Cayman Ltd");
            AddCarrier("346-6", "Digicel Ltd.");
            AddCarrier("346-140", "LIME / Cable & Wirel.");
            AddCarrier("623-1", "Centrafr. Telecom+");
            AddCarrier("623-4", "Nationlink");
            AddCarrier("623-3", "Orange/Celca");
            AddCarrier("623-2", "Telecel Centraf.");
            AddCarrier("622-4", "Salam/Sotel");
            AddCarrier("622-2", "Tchad Mobile");
            AddCarrier("622-3", "Tigo/Milicom/Tchad Mobile");
            AddCarrier("622-1", "Zain/Airtel/Celtel");
            AddCarrier("730-6", "Blue Two Chile SA");
            AddCarrier("730-11", "Celupago SA");
            AddCarrier("730-15", "Cibeles Telecom SA");
            AddCarrier("730-3", "Claro");
            AddCarrier("730-10", "Entel PCS");
            AddCarrier("730-1", "Entel Telefonia Mov");
            AddCarrier("730-14", "Netline Telefonica Movil Ltda");
            AddCarrier("730-4", "Nextel SA");
            AddCarrier("730-9", "Nextel SA");
            AddCarrier("730-5", "Nextel SA");
            AddCarrier("730-2", "TELEFONICA");
            AddCarrier("730-7", "TELEFONICA");
            AddCarrier("730-12", "Telestar Movil SA");
            AddCarrier("730-0", "TESAM SA");
            AddCarrier("730-13", "Tribe Mobile SPA");
            AddCarrier("730-8", "VTR Banda Ancha SA");
            AddCarrier("460-2", "China Mobile GSM");
            AddCarrier("460-0", "China Mobile GSM");
            AddCarrier("460-7", "China Mobile GSM");
            AddCarrier("460-4", "China Space Mobile Satellite Telecommunications Co. Ltd (China Spacecom)");
            AddCarrier("460-5", "China Telecom");
            AddCarrier("460-3", "China Telecom");
            AddCarrier("460-6", "China Unicom");
            AddCarrier("460-1", "China Unicom");
            AddCarrier("732-130", "Avantel SAS");
            AddCarrier("732-102", "Movistar");
            AddCarrier("732-103", "TIGO/Colombia Movil");
            AddCarrier("732-1", "TIGO/Colombia Movil");
            AddCarrier("732-101", "Comcel S.A. Occel S.A./Celcaribe");
            AddCarrier("732-2", "Edatel S.A.");
            AddCarrier("732-123", "Movistar");
            AddCarrier("732-111", "TIGO/Colombia Movil");
            AddCarrier("732-142", "UNE EPM Telecomunicaciones SA ESP");
            AddCarrier("732-20", "UNE EPM Telecomunicaciones SA ESP");
            AddCarrier("732-154", "Virgin Mobile Colombia SAS");
            AddCarrier("654-1", "HURI - SNPT");
            AddCarrier("630-86", "Orange RDC sarl");
            AddCarrier("630-5", "SuperCell");
            AddCarrier("630-89", "TIGO/Oasis");
            AddCarrier("630-1", "Vodacom");
            AddCarrier("630-88", "Yozma Timeturns sprl (YTT)");
            AddCarrier("630-2", "ZAIN CelTel");
            AddCarrier("629-1", "Airtel Congo SA");
            AddCarrier("629-2", "Zain/Celtel");
            AddCarrier("629-10", "MTN/Libertis");
            AddCarrier("629-7", "Warid");
            AddCarrier("548-1", "Telecom Cook Islands");
            AddCarrier("712-3", "Claro");
            AddCarrier("712-1", "ICE");
            AddCarrier("712-2", "ICE");
            AddCarrier("712-4", "Movistar");
            AddCarrier("219-1", "T-Mobile/Cronet");
            AddCarrier("219-2", "Tele2");
            AddCarrier("219-10", "VIPnet d.o.o.");
            AddCarrier("368-1", "C-COM");
            AddCarrier("362-95", "EOCG Wireless NV");
            AddCarrier("362-69", "Polycom N.V./ Curacao Telecom d.b.a. Digicel");
            AddCarrier("280-10", "Areeba");
            AddCarrier("280-20", "PrimeTel PLC");
            AddCarrier("280-1", "Vodafone/CyTa");
            AddCarrier("230-8", "Compatel s.r.o.");
            AddCarrier("230-2", "O2");
            AddCarrier("230-1", "T-Mobile / RadioMobil");
            AddCarrier("230-5", "Travel Telekommunikation s.r.o.");
            AddCarrier("230-4", "Ufone");
            AddCarrier("230-3", "Vodafone");
            AddCarrier("230-99", "Vodafone");
            AddCarrier("238-5", "ApS KBUS");
            AddCarrier("238-23", "Banedanmark");
            AddCarrier("238-28", "CoolTEL ApS");
            AddCarrier("238-6", "Hi3G");
            AddCarrier("238-12", "Lycamobile Ltd");
            AddCarrier("238-3", "Mach Connectivity ApS");
            AddCarrier("238-7", "");
            AddCarrier("238-4", "NextGen Mobile Ltd (CardBoardFish)");
            AddCarrier("238-10", "TDC Denmark");
            AddCarrier("238-1", "TDC Denmark");
            AddCarrier("238-77", "Telenor/Sonofon");
            AddCarrier("238-2", "Telenor/Sonofon");
            AddCarrier("238-20", "Telia");
            AddCarrier("238-30", "Telia");
            AddCarrier("638-1", "Djibouti Telecom SA (Evatis)");
            AddCarrier("366-110", "C & W");
            AddCarrier("366-20", "Cingular Wireless/Digicel");
            AddCarrier("366-50", "Wireless Ventures (Dominica) Ltd (Digicel Dominica)");
            AddCarrier("370-2", "Claro");
            AddCarrier("370-1", "Orange");
            AddCarrier("370-3", "TRIcom");
            AddCarrier("370-4", "Trilogy Dominicana S. A.");
            AddCarrier("740-2", "Alegro/Telcsa");
            AddCarrier("740-0", "MOVISTAR/OteCel");
            AddCarrier("740-1", "Porta/Conecel");
            AddCarrier("602-1", "EMS - Mobinil");
            AddCarrier("602-3", "ETISALAT");
            AddCarrier("602-2", "Vodafone (Misrfone Telecom)");
            AddCarrier("706-1", "CLARO/CTE");
            AddCarrier("706-2", "Digicel");
            AddCarrier("706-5", "INTELFON SA de CV");
            AddCarrier("706-4", "Telefonica");
            AddCarrier("706-3", "Telemovil");
            AddCarrier("627-3", "HiTs-GE");
            AddCarrier("627-1", "ORANGE/GETESA");
            AddCarrier("657-0", "EriTel");
            AddCarrier("657-1", "Eritel");
            AddCarrier("248-1", "EMT GSM");
            AddCarrier("248-2", "Radiolinja Eesti");
            AddCarrier("248-3", "Tele2 Eesti AS");
            AddCarrier("248-4", "Top Connect OU");
            AddCarrier("636-1", "ETH/MTN");
            AddCarrier("750-1", "Cable and Wireless South Atlantic Ltd (Falkland Islands");
            AddCarrier("288-3", "Edge Mobile Sp/F");
            AddCarrier("288-1", "Faroese Telecom");
            AddCarrier("288-2", "Kall GSM");
            AddCarrier("542-2", "DigiCell");
            AddCarrier("542-1", "Vodafone");
            AddCarrier("244-14", "Alands");
            AddCarrier("244-26", "Compatel Ltd");
            AddCarrier("244-3", "DNA/Finnet");
            AddCarrier("244-13", "DNA/Finnet");
            AddCarrier("244-12", "DNA/Finnet");
            AddCarrier("244-4", "DNA/Finnet");
            AddCarrier("244-21", "Elisa/Saunalahti");
            AddCarrier("244-5", "Elisa/Saunalahti");
            AddCarrier("244-82", "ID-Mobile");
            AddCarrier("244-11", "Mundio Mobile (Finland) Ltd");
            AddCarrier("244-9", "Nokia Oyj");
            AddCarrier("244-10", "TDC Oy Finland");
            AddCarrier("244-91", "TeliaSonera");
            AddCarrier("208-27", "AFONE SA");
            AddCarrier("208-92", "Association Plate-forme Telecom");
            AddCarrier("208-28", "Astrium");
            AddCarrier("208-21", "Bouygues Telecom");
            AddCarrier("208-20", "Bouygues Telecom");
            AddCarrier("208-88", "Bouygues Telecom");
            AddCarrier("208-14", "Lliad/FREE Mobile");
            AddCarrier("208-7", "GlobalStar");
            AddCarrier("208-6", "GlobalStar");
            AddCarrier("208-5", "GlobalStar");
            AddCarrier("208-29", "Orange");
            AddCarrier("208-16", "Lliad/FREE Mobile");
            AddCarrier("208-15", "Lliad/FREE Mobile");
            AddCarrier("208-25", "Lycamobile SARL");
            AddCarrier("208-3", "MobiquiThings");
            AddCarrier("208-24", "MobiquiThings");
            AddCarrier("208-31", "Mundio Mobile (France) Ltd");
            AddCarrier("208-26", "NRJ");
            AddCarrier("208-23", "Omer/Virgin Mobile");
            AddCarrier("208-89", "Omer/Virgin Mobile");
            AddCarrier("208-2", "Orange");
            AddCarrier("208-1", "Orange");
            AddCarrier("208-91", "Orange");
            AddCarrier("208-13", "S.F.R.");
            AddCarrier("208-11", "S.F.R.");
            AddCarrier("208-10", "S.F.R.");
            AddCarrier("208-9", "S.F.R.");
            AddCarrier("208-4", "SISTEER");
            AddCarrier("208-0", "Tel/Tel");
            AddCarrier("208-22", "Transatel SA");
            AddCarrier("340-20", "Bouygues/DigiCel");
            AddCarrier("340-8", "AMIGO/Dauphin");
            AddCarrier("340-1", "Orange Caribe");
            AddCarrier("340-2", "Outremer Telecom");
            AddCarrier("340-11", "TelCell GSM");
            AddCarrier("340-3", "TelCell GSM");
            AddCarrier("547-15", "Pacific Mobile Telecom (PMT)");
            AddCarrier("547-20", "Tikiphone");
            AddCarrier("628-4", "Azur/Usan S.A.");
            AddCarrier("628-1", "Libertis S.A.");
            AddCarrier("628-2", "MOOV/Telecel");
            AddCarrier("628-3", "ZAIN/Celtel Gabon S.A.");
            AddCarrier("607-2", "Africel");
            AddCarrier("607-3", "Comium");
            AddCarrier("607-1", "Gamcel");
            AddCarrier("607-4", "Q-Cell");
            AddCarrier("282-1", "Geocell Ltd.");
            AddCarrier("282-3", "Iberiatel Ltd.");
            AddCarrier("282-2", "Magti GSM Ltd.");
            AddCarrier("282-4", "MobiTel/Beeline");
            AddCarrier("282-0", "Silknet");
            AddCarrier("262-17", "E-Plus");
            AddCarrier("262-NaN", "Debitel");
            AddCarrier("262-77", "E-Plus");
            AddCarrier("262-3", "E-Plus");
            AddCarrier("262-5", "E-Plus");
            AddCarrier("262-14", "Group 3G UMTS");
            AddCarrier("262-43", "Lycamobile");
            AddCarrier("262-13", "Mobilcom");
            AddCarrier("262-8", "O2");
            AddCarrier("262-7", "O2");
            AddCarrier("262-11", "O2");
            AddCarrier("262-10", "O2");
            AddCarrier("262-12", "O2");
            AddCarrier("262-NaN", "Talkline");
            AddCarrier("262-6", "Telekom/T-mobile");
            AddCarrier("262-1", "Telekom/T-mobile");
            AddCarrier("262-16", "Telogic/ViStream");
            AddCarrier("262-4", "Vodafone D2");
            AddCarrier("262-2", "Vodafone D2");
            AddCarrier("262-9", "Vodafone D2");
            AddCarrier("620-4", "Expresso Ghana Ltd");
            AddCarrier("620-7", "GloMobile");
            AddCarrier("620-3", "Milicom/Tigo");
            AddCarrier("620-1", "MTN");
            AddCarrier("620-2", "Vodafone");
            AddCarrier("620-6", "ZAIN");
            AddCarrier("266-6", "CTS Mobile");
            AddCarrier("266-9", "eazi telecom");
            AddCarrier("266-1", "Gibtel GSM");
            AddCarrier("202-7", "AMD Telecom SA");
            AddCarrier("202-1", "Cosmote");
            AddCarrier("202-2", "Cosmote");
            AddCarrier("202-4", "Organismos Sidirodromon Ellados (OSE)");
            AddCarrier("202-3", "OTE Hellenic Telecommunications Organization SA");
            AddCarrier("202-10", "Tim/Wind");
            AddCarrier("202-9", "Tim/Wind");
            AddCarrier("202-5", "Vodafone");
            AddCarrier("290-1", "Tele Greenland");
            AddCarrier("352-110", "Cable & Wireless");
            AddCarrier("352-30", "Digicel");
            AddCarrier("352-50", "Digicel");
            AddCarrier("340-8", "Dauphin Telecom SU (Guadeloupe Telecom) (Guadeloupe");
            AddCarrier("340-20", "Digicel Antilles Francaises Guyane SA (Guadeloupe");
            AddCarrier("340-1", "Orange Caribe");
            AddCarrier("340-2", "Outremer Telecom Guadeloupe (only) (Guadeloupe");
            AddCarrier("340-10", "United Telecommunications Services Caraibe SARL (UTS Caraibe, Guadeloupe Telephone Mobile) (Guadeloupe");
            AddCarrier("340-3", "United Telecommunications Services Caraibe SARL (UTS Caraibe, Guadeloupe Telephone Mobile) (Guadeloupe");
            AddCarrier("310-480", "Choice Phone LLC");
            AddCarrier("310-370", "Docomo");
            AddCarrier("310-470", "Docomo");
            AddCarrier("310-140", "GTA Wireless");
            AddCarrier("310-33", "Guam Teleph. Auth.");
            AddCarrier("310-32", "IT&E OverSeas");
            AddCarrier("311-250", "Wave Runner LLC");
            AddCarrier("704-1", "SERCOM");
            AddCarrier("704-3", "Telefonica");
            AddCarrier("704-2", "TIGO/COMCEL");
            AddCarrier("611-4", "Areeba - MTN");
            AddCarrier("611-5", "Celcom");
            AddCarrier("611-3", "Intercel");
            AddCarrier("611-1", "Orange/Spacetel");
            AddCarrier("611-2", "SotelGui");
            AddCarrier("632-0", "GuineTel");
            AddCarrier("632-1", "GuineTel");
            AddCarrier("632-3", "Orange");
            AddCarrier("632-2", "SpaceTel");
            AddCarrier("738-2", "Cellink Plus");
            AddCarrier("738-1", "DigiCel");
            AddCarrier("372-1", "Comcel");
            AddCarrier("372-2", "Digicel");
            AddCarrier("372-3", "National Telecom SA (NatCom)");
            AddCarrier("708-40", "Digicel");
            AddCarrier("708-30", "HonduTel");
            AddCarrier("708-30", "HonduTel");
            AddCarrier("708-1", "SERCOM/CLARO");
            AddCarrier("708-1", "SERCOM/CLARO");
            AddCarrier("708-2", "Telefonica/CELTEL");
            AddCarrier("708-2", "Telefonica/CELTEL");
            AddCarrier("454-13", "China Mobile/Peoples");
            AddCarrier("454-12", "China Mobile/Peoples");
            AddCarrier("454-9", "China Motion");
            AddCarrier("454-7", "China Unicom Ltd");
            AddCarrier("454-11", "China-HongKong Telecom Ltd (CHKTL)");
            AddCarrier("454-1", "Citic Telecom Ltd.");
            AddCarrier("454-18", "CSL Ltd.");
            AddCarrier("454-2", "CSL Ltd.");
            AddCarrier("454-0", "CSL Ltd.");
            AddCarrier("454-10", "CSL/New World PCS Ltd.");
            AddCarrier("454-14", "H3G/Hutchinson");
            AddCarrier("454-5", "H3G/Hutchinson");
            AddCarrier("454-4", "H3G/Hutchinson");
            AddCarrier("454-3", "H3G/Hutchinson");
            AddCarrier("454-16", "HKT/PCCW");
            AddCarrier("454-19", "HKT/PCCW");
            AddCarrier("454-20", "HKT/PCCW");
            AddCarrier("454-29", "HKT/PCCW");
            AddCarrier("454-47", "shared by private TETRA systems");
            AddCarrier("454-40", "shared by private TETRA systems");
            AddCarrier("454-8", "Trident Telecom Ventures Ltd.");
            AddCarrier("454-17", "Vodafone/SmarTone");
            AddCarrier("454-15", "Vodafone/SmarTone");
            AddCarrier("454-6", "Vodafone/SmarTone");
            AddCarrier("216-1", "Pannon/Telenor");
            AddCarrier("216-30", "T-mobile/Magyar");
            AddCarrier("216-71", "UPC Magyarorszag Kft.");
            AddCarrier("216-70", "Vodafone");
            AddCarrier("274-9", "Amitelo");
            AddCarrier("274-7", "IceCell");
            AddCarrier("274-8", "Landssiminn");
            AddCarrier("274-1", "Landssiminn");
            AddCarrier("274-11", "NOVA");
            AddCarrier("274-4", "VIKING/IMC");
            AddCarrier("274-3", "Vodafone/Tal hf");
            AddCarrier("274-2", "Vodafone/Tal hf");
            AddCarrier("274-5", "Vodafone/Tal hf");
            AddCarrier("404-29", "Aircel");
            AddCarrier("404-28", "Aircel");
            AddCarrier("404-25", "Aircel");
            AddCarrier("404-17", "Aircel");
            AddCarrier("404-42", "Aircel");
            AddCarrier("404-33", "Aircel");
            AddCarrier("404-1", "Aircel Digilink India");
            AddCarrier("404-15", "Aircel Digilink India");
            AddCarrier("404-60", "Aircel Digilink India");
            AddCarrier("405-55", "AirTel");
            AddCarrier("405-53", "AirTel");
            AddCarrier("405-51", "AirTel");
            AddCarrier("405-56", "Airtel (Bharati Mobile) - Assam");
            AddCarrier("404-86", "Barakhamba Sales & Serv.");
            AddCarrier("404-13", "Barakhamba Sales & Serv.");
            AddCarrier("404-58", "BSNL");
            AddCarrier("404-81", "BSNL");
            AddCarrier("404-74", "BSNL");
            AddCarrier("404-38", "BSNL");
            AddCarrier("404-57", "BSNL");
            AddCarrier("404-80", "BSNL");
            AddCarrier("404-73", "BSNL");
            AddCarrier("404-34", "BSNL");
            AddCarrier("404-66", "BSNL");
            AddCarrier("404-55", "BSNL");
            AddCarrier("404-72", "BSNL");
            AddCarrier("404-77", "BSNL");
            AddCarrier("404-64", "BSNL");
            AddCarrier("404-54", "BSNL");
            AddCarrier("404-71", "BSNL");
            AddCarrier("404-76", "BSNL");
            AddCarrier("404-62", "BSNL");
            AddCarrier("404-53", "BSNL");
            AddCarrier("404-59", "BSNL");
            AddCarrier("404-75", "BSNL");
            AddCarrier("404-51", "BSNL");
            AddCarrier("405-10", "Bharti Airtel Limited (Delhi)");
            AddCarrier("404-79", "CellOne A&N");
            AddCarrier("404-89", "Escorts Telecom Ltd.");
            AddCarrier("404-88", "Escorts Telecom Ltd.");
            AddCarrier("404-87", "Escorts Telecom Ltd.");
            AddCarrier("404-82", "Escorts Telecom Ltd.");
            AddCarrier("404-12", "Escotel Mobile Communications");
            AddCarrier("404-19", "Escotel Mobile Communications");
            AddCarrier("404-56", "Escotel Mobile Communications");
            AddCarrier("405-5", "Fascel Limited");
            AddCarrier("404-5", "Fascel");
            AddCarrier("404-70", "Hexacom India");
            AddCarrier("404-16", "Hexcom India");
            AddCarrier("404-4", "Idea Cellular Ltd.");
            AddCarrier("404-24", "Idea Cellular Ltd.");
            AddCarrier("404-22", "Idea Cellular Ltd.");
            AddCarrier("404-78", "Idea Cellular Ltd.");
            AddCarrier("404-7", "Idea Cellular Ltd.");
            AddCarrier("404-69", "Mahanagar Telephone Nigam");
            AddCarrier("404-68", "Mahanagar Telephone Nigam");
            AddCarrier("404-83", "Reliable Internet Services");
            AddCarrier("405-9", "RELIANCE TELECOM");
            AddCarrier("404-36", "Reliance Telecom Private");
            AddCarrier("404-52", "Reliance Telecom Private");
            AddCarrier("404-50", "Reliance Telecom Private");
            AddCarrier("404-67", "Reliance Telecom Private");
            AddCarrier("404-18", "Reliance Telecom Private");
            AddCarrier("404-85", "Reliance Telecom Private");
            AddCarrier("404-9", "Reliance Telecom Private");
            AddCarrier("404-41", "RPG Cellular");
            AddCarrier("404-14", "Spice");
            AddCarrier("404-44", "Spice");
            AddCarrier("404-11", "Sterling Cellular Ltd.");
            AddCarrier("404-30", "Usha Martin Telecom");
            AddCarrier("510-8", "Axis/Natrindo");
            AddCarrier("510-89", "H3G CP");
            AddCarrier("510-21", "Indosat/Satelindo/M3");
            AddCarrier("510-1", "Indosat/Satelindo/M3");
            AddCarrier("510-0", "PT Pasifik Satelit Nusantara (PSN)");
            AddCarrier("510-0", "PT Pasifik Satelit Nusantara (PSN)");
            AddCarrier("510-27", "PT Sampoerna Telekomunikasi Indonesia (STI)");
            AddCarrier("510-28", "PT Smartfren Telecom Tbk");
            AddCarrier("510-9", "PT Smartfren Telecom Tbk");
            AddCarrier("510-11", "PT. Excelcom");
            AddCarrier("510-10", "Telkomsel");
            AddCarrier("901-13", "Antarctica");
            AddCarrier("432-19", "Mobile Telecommunications Company of Esfahan JV-PJS (MTCE)");
            AddCarrier("432-70", "MTCE");
            AddCarrier("432-35", "MTN/IranCell");
            AddCarrier("432-32", "Taliya");
            AddCarrier("432-11", "TCI / MCI");
            AddCarrier("432-14", "TKC/KFZO");
            AddCarrier("418-5", "Asia Cell");
            AddCarrier("418-92", "Itisaluna and Kalemat");
            AddCarrier("418-82", "Korek");
            AddCarrier("418-40", "Korek");
            AddCarrier("418-45", "Mobitel (Iraq-Kurdistan) and Moutiny");
            AddCarrier("418-20", "ZAIN/Atheer");
            AddCarrier("418-30", "Orascom Telecom");
            AddCarrier("418-8", "Sanatel");
            AddCarrier("272-4", "Access Telecom Ltd.");
            AddCarrier("272-9", "Clever Communications Ltd");
            AddCarrier("272-7", "eircom Ltd");
            AddCarrier("272-5", "H3G");
            AddCarrier("272-11", "Liffey Telecom");
            AddCarrier("272-13", "Lycamobile");
            AddCarrier("272-3", "Meteor Mobile Ltd.");
            AddCarrier("272-2", "O2/Digifone");
            AddCarrier("272-1", "Vodafone Eircell");
            AddCarrier("425-14", "Alon Cellular Ltd");
            AddCarrier("425-2", "Cellcom ltd.");
            AddCarrier("425-8", "Golan Telekom");
            AddCarrier("425-15", "Home Cellular Ltd");
            AddCarrier("425-77", "Hot Mobile/Mirs");
            AddCarrier("425-7", "Hot Mobile/Mirs");
            AddCarrier("425-1", "Orange/Partner Co. Ltd.");
            AddCarrier("425-3", "Pelephone");
            AddCarrier("425-16", "Rami Levy Hashikma Marketing Communications Ltd");
            AddCarrier("222-34", "BT Italia SpA");
            AddCarrier("222-2", "Elsacom");
            AddCarrier("222-99", "Hi3G");
            AddCarrier("222-33", "Hi3G");
            AddCarrier("222-77", "IPSE 2000");
            AddCarrier("222-35", "Lycamobile Srl");
            AddCarrier("222-7", "Noverca Italia Srl");
            AddCarrier("222-30", "RFI Rete Ferroviaria Italiana SpA");
            AddCarrier("222-48", "Telecom Italia Mobile SpA");
            AddCarrier("222-43", "Telecom Italia Mobile SpA");
            AddCarrier("222-1", "TIM");
            AddCarrier("222-10", "Vodafone");
            AddCarrier("222-6", "Vodafone");
            AddCarrier("222-44", "WIND (Blu) -");
            AddCarrier("222-88", "WIND (Blu) -");
            AddCarrier("612-7", "Aircomm SA");
            AddCarrier("612-2", "Atlantik Tel./Moov");
            AddCarrier("612-4", "Comium");
            AddCarrier("612-1", "Comstar");
            AddCarrier("612-5", "MTN");
            AddCarrier("612-3", "Orange");
            AddCarrier("612-6", "OriCell");
            AddCarrier("612-0", "Warid");
            AddCarrier("338-110", "Cable & Wireless");
            AddCarrier("338-20", "Cable & Wireless");
            AddCarrier("338-180", "Cable & Wireless");
            AddCarrier("338-50", "DIGICEL/Mossel");
            AddCarrier("440-0", "eMobile");
            AddCarrier("440-76", "KDDI Corporation");
            AddCarrier("440-71", "KDDI Corporation");
            AddCarrier("440-53", "KDDI Corporation");
            AddCarrier("440-77", "KDDI Corporation");
            AddCarrier("440-8", "KDDI Corporation");
            AddCarrier("440-72", "KDDI Corporation");
            AddCarrier("440-54", "KDDI Corporation");
            AddCarrier("440-79", "KDDI Corporation");
            AddCarrier("440-7", "KDDI Corporation");
            AddCarrier("440-73", "KDDI Corporation");
            AddCarrier("440-55", "KDDI Corporation");
            AddCarrier("440-88", "KDDI Corporation");
            AddCarrier("440-50", "KDDI Corporation");
            AddCarrier("440-74", "KDDI Corporation");
            AddCarrier("440-70", "KDDI Corporation");
            AddCarrier("440-89", "KDDI Corporation");
            AddCarrier("440-51", "KDDI Corporation");
            AddCarrier("440-75", "KDDI Corporation");
            AddCarrier("440-56", "KDDI Corporation");
            AddCarrier("441-70", "KDDI Corporation");
            AddCarrier("440-52", "KDDI Corporation");
            AddCarrier("440-23", "NTT Docomo");
            AddCarrier("440-21", "NTT Docomo");
            AddCarrier("441-44", "NTT Docomo");
            AddCarrier("440-13", "NTT Docomo");
            AddCarrier("440-69", "NTT Docomo");
            AddCarrier("440-16", "NTT Docomo");
            AddCarrier("441-99", "NTT Docomo");
            AddCarrier("440-34", "NTT Docomo");
            AddCarrier("440-64", "NTT Docomo");
            AddCarrier("440-37", "NTT Docomo");
            AddCarrier("440-25", "NTT Docomo");
            AddCarrier("440-2", "NTT Docomo");
            AddCarrier("440-22", "NTT Docomo");
            AddCarrier("441-43", "NTT Docomo");
            AddCarrier("440-27", "NTT Docomo");
            AddCarrier("440-87", "NTT Docomo");
            AddCarrier("440-17", "NTT Docomo");
            AddCarrier("440-31", "NTT Docomo");
            AddCarrier("440-65", "NTT Docomo");
            AddCarrier("440-36", "NTT Docomo");
            AddCarrier("441-92", "NTT Docomo");
            AddCarrier("440-3", "NTT Docomo");
            AddCarrier("440-12", "NTT Docomo");
            AddCarrier("440-58", "NTT Docomo");
            AddCarrier("440-28", "NTT Docomo");
            AddCarrier("440-61", "NTT Docomo");
            AddCarrier("440-18", "NTT Docomo");
            AddCarrier("441-91", "NTT Docomo");
            AddCarrier("440-32", "NTT Docomo");
            AddCarrier("441-40", "NTT Docomo");
            AddCarrier("440-66", "NTT Docomo");
            AddCarrier("440-35", "NTT Docomo");
            AddCarrier("441-93", "NTT Docomo");
            AddCarrier("440-9", "NTT Docomo");
            AddCarrier("440-49", "NTT Docomo");
            AddCarrier("440-29", "NTT Docomo");
            AddCarrier("440-60", "NTT Docomo");
            AddCarrier("440-19", "NTT Docomo");
            AddCarrier("441-90", "NTT Docomo");
            AddCarrier("440-33", "NTT Docomo");
            AddCarrier("440-67", "NTT Docomo");
            AddCarrier("440-14", "NTT Docomo");
            AddCarrier("441-94", "NTT Docomo");
            AddCarrier("441-41", "NTT Docomo");
            AddCarrier("440-10", "NTT Docomo");
            AddCarrier("440-62", "NTT Docomo");
            AddCarrier("440-1", "NTT Docomo");
            AddCarrier("440-39", "NTT Docomo");
            AddCarrier("440-30", "NTT Docomo");
            AddCarrier("440-20", "NTT Docomo");
            AddCarrier("441-45", "NTT Docomo");
            AddCarrier("440-24", "NTT Docomo");
            AddCarrier("440-68", "NTT Docomo");
            AddCarrier("440-15", "NTT Docomo");
            AddCarrier("441-98", "NTT Docomo");
            AddCarrier("441-42", "NTT Docomo");
            AddCarrier("440-11", "NTT Docomo");
            AddCarrier("440-63", "NTT Docomo");
            AddCarrier("440-38", "NTT Docomo");
            AddCarrier("440-26", "NTT Docomo");
            AddCarrier("440-99", "NTT Docomo");
            AddCarrier("440-78", "Okinawa Cellular Telephone");
            AddCarrier("440-47", "SoftBank Mobile Corp");
            AddCarrier("440-41", "SoftBank Mobile Corp");
            AddCarrier("440-95", "SoftBank Mobile Corp");
            AddCarrier("441-64", "SoftBank Mobile Corp");
            AddCarrier("440-46", "SoftBank Mobile Corp");
            AddCarrier("440-97", "SoftBank Mobile Corp");
            AddCarrier("440-42", "SoftBank Mobile Corp");
            AddCarrier("440-90", "SoftBank Mobile Corp");
            AddCarrier("441-65", "SoftBank Mobile Corp");
            AddCarrier("440-92", "SoftBank Mobile Corp");
            AddCarrier("440-98", "SoftBank Mobile Corp");
            AddCarrier("440-43", "SoftBank Mobile Corp");
            AddCarrier("440-48", "SoftBank Mobile Corp");
            AddCarrier("440-93", "SoftBank Mobile Corp");
            AddCarrier("440-6", "SoftBank Mobile Corp");
            AddCarrier("441-61", "SoftBank Mobile Corp");
            AddCarrier("440-44", "SoftBank Mobile Corp");
            AddCarrier("440-4", "SoftBank Mobile Corp");
            AddCarrier("440-94", "SoftBank Mobile Corp");
            AddCarrier("441-62", "SoftBank Mobile Corp");
            AddCarrier("440-45", "SoftBank Mobile Corp");
            AddCarrier("440-40", "SoftBank Mobile Corp");
            AddCarrier("440-96", "SoftBank Mobile Corp");
            AddCarrier("441-63", "SoftBank Mobile Corp");
            AddCarrier("440-85", "KDDI Corporation");
            AddCarrier("440-83", "KDDI Corporation");
            AddCarrier("440-81", "KDDI Corporation");
            AddCarrier("440-80", "KDDI Corporation");
            AddCarrier("440-86", "KDDI Corporation");
            AddCarrier("440-84", "KDDI Corporation");
            AddCarrier("440-82", "KDDI Corporation");
            AddCarrier("416-77", "Orange/Petra");
            AddCarrier("416-3", "Umniah Mobile Co.");
            AddCarrier("416-2", "Xpress");
            AddCarrier("416-1", "ZAIN /J.M.T.S");
            AddCarrier("401-1", "Beeline/KaR-Tel LLP");
            AddCarrier("401-7", "Dalacom/Altel");
            AddCarrier("401-2", "K-Cell");
            AddCarrier("401-77", "NEO/MTS");
            AddCarrier("639-5", "Econet Wireless");
            AddCarrier("639-7", "Orange");
            AddCarrier("639-2", "Safaricom Ltd.");
            AddCarrier("639-3", "Zain/Celtel Ltd.");
            AddCarrier("545-9", "Kiribati Frigate");
            AddCarrier("467-193", "Sun Net");
            AddCarrier("450-2", "KT Freetel Co. Ltd.");
            AddCarrier("450-4", "KT Freetel Co. Ltd.");
            AddCarrier("450-8", "KT Freetel Co. Ltd.");
            AddCarrier("450-6", "LG Telecom");
            AddCarrier("450-3", "SK Telecom");
            AddCarrier("450-5", "SK Telecom Co. Ltd");
            AddCarrier("419-4", "Viva");
            AddCarrier("419-3", "Wantaniya");
            AddCarrier("419-2", "Zain");
            AddCarrier("437-3", "AkTel LLC");
            AddCarrier("437-1", "Beeline/Bitel");
            AddCarrier("437-5", "MEGACOM");
            AddCarrier("437-9", "O!/NUR Telecom");
            AddCarrier("457-2", "ETL Mobile");
            AddCarrier("457-1", "Lao Tel");
            AddCarrier("457-8", "Tigo/Millicom");
            AddCarrier("457-3", "UNITEL/LAT");
            AddCarrier("247-5", "Bite Latvija");
            AddCarrier("247-1", "Latvian Mobile Phone");
            AddCarrier("247-9", "SIA Camel Mobile");
            AddCarrier("247-8", "SIA IZZI");
            AddCarrier("247-7", "SIA Master Telecom");
            AddCarrier("247-6", "SIA Rigatta");
            AddCarrier("247-2", "Tele2");
            AddCarrier("247-3", "TRIATEL/Telekom Baltija");
            AddCarrier("415-33", "Cellis");
            AddCarrier("415-32", "Cellis");
            AddCarrier("415-35", "Cellis");
            AddCarrier("415-34", "FTML Cellis");
            AddCarrier("415-39", "MIC2/LibanCell");
            AddCarrier("415-38", "MIC2/LibanCell");
            AddCarrier("415-37", "MIC2/LibanCell");
            AddCarrier("415-1", "MIC1 (Alfa)");
            AddCarrier("415-3", "MIC2/LibanCell");
            AddCarrier("415-36", "MIC2/LibanCell");
            AddCarrier("651-2", "Econet/Ezi-cel");
            AddCarrier("651-1", "Vodacom Lesotho");
            AddCarrier("618-7", "Celcom");
            AddCarrier("618-3", "Celcom");
            AddCarrier("618-4", "Comium BVI");
            AddCarrier("618-2", "Libercell");
            AddCarrier("618-20", "LibTelco");
            AddCarrier("618-1", "Lonestar");
            AddCarrier("606-2", "Al-Madar");
            AddCarrier("606-1", "Al-Madar");
            AddCarrier("606-6", "Hatef");
            AddCarrier("606-0", "Libyana");
            AddCarrier("606-3", "Libyana");
            AddCarrier("295-6", "CUBIC (Liechtenstein");
            AddCarrier("295-7", "First Mobile AG");
            AddCarrier("295-5", "Mobilkom AG");
            AddCarrier("295-2", "Orange");
            AddCarrier("295-1", "Swisscom FL AG");
            AddCarrier("295-77", "Alpmobile/Tele2");
            AddCarrier("246-2", "Bite");
            AddCarrier("246-1", "Omnitel");
            AddCarrier("246-3", "Tele2");
            AddCarrier("270-77", "Millicom Tango GSM");
            AddCarrier("270-1", "P+T LUXGSM");
            AddCarrier("270-99", "VOXmobile S.A.");
            AddCarrier("455-4", "C.T.M. TELEMOVEL+");
            AddCarrier("455-1", "C.T.M. TELEMOVEL+");
            AddCarrier("455-2", "China Telecom");
            AddCarrier("455-5", "Hutchison Telephone (Macau) Company Ltd");
            AddCarrier("455-3", "Hutchison Telephone (Macau) Company Ltd");
            AddCarrier("455-6", "Smartone Mobile");
            AddCarrier("455-0", "Smartone Mobile");
            AddCarrier("294-75", "MTS/Cosmofone");
            AddCarrier("294-2", "MTS/Cosmofone");
            AddCarrier("294-1", "T-Mobile/Mobimak");
            AddCarrier("294-3", "VIP Mobile");
            AddCarrier("646-1", "MADACOM");
            AddCarrier("646-2", "Orange/Soci");
            AddCarrier("646-3", "Sacel");
            AddCarrier("646-4", "Telma");
            AddCarrier("650-1", "TNM/Telekom Network Ltd.");
            AddCarrier("650-10", "Zain/Celtel ltd.");
            AddCarrier("502-1", "Art900");
            AddCarrier("502-151", "Baraka Telecom Sdn Bhd");
            AddCarrier("502-13", "CelCom");
            AddCarrier("502-19", "CelCom");
            AddCarrier("502-16", "Digi Telecommunications");
            AddCarrier("502-10", "Digi Telecommunications");
            AddCarrier("502-20", "Electcoms Wireless Sdn Bhd");
            AddCarrier("502-17", "Maxis");
            AddCarrier("502-12", "Maxis");
            AddCarrier("502-11", "MTX Utara");
            AddCarrier("502-153", "Packet One Networks (Malaysia) Sdn Bhd");
            AddCarrier("502-155", "Samata Communications Sdn Bhd");
            AddCarrier("502-154", "Talk Focus Sdn Bhd");
            AddCarrier("502-18", "U Mobile");
            AddCarrier("502-152", "YES");
            AddCarrier("472-1", "Dhiraagu/C&W");
            AddCarrier("472-2", "Wataniya/WMOBILE");
            AddCarrier("610-1", "Malitel");
            AddCarrier("610-2", "Orange/IKATEL");
            AddCarrier("278-21", "GO/Mobisle");
            AddCarrier("278-77", "Melita");
            AddCarrier("278-1", "Vodafone");
            AddCarrier("340-2", "Outremer Telecom Martinique (only) (Martinique");
            AddCarrier("340-12", "United Telecommunications Services Caraibe SARL (UTS Caraibe, Martinique Telephone Mobile) (Martinique");
            AddCarrier("340-3", "United Telecommunications Services Caraibe SARL (UTS Caraibe, Martinique Telephone Mobile) (Martinique");
            AddCarrier("609-2", "Chinguitel SA");
            AddCarrier("609-1", "Mattel");
            AddCarrier("609-10", "Mauritel");
            AddCarrier("617-10", "Emtel Ltd");
            AddCarrier("617-2", "Mahanagar Telephone");
            AddCarrier("617-3", "Mahanagar Telephone");
            AddCarrier("617-1", "Orange/Cellplus");
            AddCarrier("334-0", "Axtel");
            AddCarrier("334-50", "IUSACell/UneFon");
            AddCarrier("334-40", "IUSACell/UneFon");
            AddCarrier("334-4", "IUSACell/UneFon");
            AddCarrier("334-50", "IUSACell/UneFon");
            AddCarrier("334-3", "Movistar/Pegaso");
            AddCarrier("334-30", "Movistar/Pegaso");
            AddCarrier("334-90", "NEXTEL");
            AddCarrier("334-10", "NEXTEL");
            AddCarrier("334-1", "NEXTEL");
            AddCarrier("334-70", "Operadora Unefon SA de CV");
            AddCarrier("334-80", "Operadora Unefon SA de CV");
            AddCarrier("334-60", "SAI PCS");
            AddCarrier("334-0", "SAI PCS");
            AddCarrier("334-20", "TelCel/America Movil");
            AddCarrier("334-2", "TelCel/America Movil");
            AddCarrier("550-1", "FSM Telecom");
            AddCarrier("259-4", "Eventis Mobile");
            AddCarrier("259-5", "IDC/Unite");
            AddCarrier("259-3", "IDC/Unite");
            AddCarrier("259-99", "IDC/Unite");
            AddCarrier("259-2", "Moldcell");
            AddCarrier("259-1", "Orange/Voxtel");
            AddCarrier("212-1", "Dardafone LLC");
            AddCarrier("212-1", "Monaco Telecom");
            AddCarrier("212-10", "Monaco Telecom");
            AddCarrier("212-1", "Post and Telecommunications of Kosovo JSC (PTK)");
            AddCarrier("428-98", "G-Mobile Corporation Ltd");
            AddCarrier("428-99", "Mobicom");
            AddCarrier("428-0", "Skytel Co. Ltd");
            AddCarrier("428-88", "Unitel");
            AddCarrier("297-2", "Monet/T-mobile");
            AddCarrier("297-3", "Mtel");
            AddCarrier("297-1", "Promonte GSM");
            AddCarrier("354-860", "Cable & Wireless");
            AddCarrier("604-1", "IAM/Itissallat");
            AddCarrier("604-2", "INWI/WANA");
            AddCarrier("604-0", "Medi Telecom");
            AddCarrier("643-1", "mCel");
            AddCarrier("643-3", "Movitel");
            AddCarrier("643-4", "Vodacom Sarl");
            AddCarrier("649-3", "Leo / Orascom");
            AddCarrier("649-1", "MTC");
            AddCarrier("649-2", "Switch/Nam. Telec.");
            AddCarrier("429-2", "Ncell");
            AddCarrier("429-1", "NT Mobile / Namaste");
            AddCarrier("429-4", "Smart Cell");
            AddCarrier("204-14", "6GMOBILE BV");
            AddCarrier("204-23", "Aspider Solutions");
            AddCarrier("204-5", "Elephant Talk Communications Premium Rate Services Netherlands BV");
            AddCarrier("204-17", "Intercity Mobile Communications BV");
            AddCarrier("204-10", "KPN Telecom B.V.");
            AddCarrier("204-8", "KPN Telecom B.V.");
            AddCarrier("204-69", "KPN Telecom B.V.");
            AddCarrier("204-12", "KPN/Telfort");
            AddCarrier("204-28", "Lancelot BV");
            AddCarrier("204-9", "Lycamobile Ltd");
            AddCarrier("204-6", "Mundio/Vectone Mobile");
            AddCarrier("204-21", "NS Railinfrabeheer B.V.");
            AddCarrier("204-24", "Private Mobility Nederland BV");
            AddCarrier("204-98", "T-Mobile B.V.");
            AddCarrier("204-16", "T-Mobile B.V.");
            AddCarrier("204-20", "Orange/T-mobile");
            AddCarrier("204-2", "Tele2");
            AddCarrier("204-7", "Teleena Holding BV");
            AddCarrier("204-68", "Unify Mobile");
            AddCarrier("204-18", "UPC Nederland BV");
            AddCarrier("204-4", "Vodafone Libertel");
            AddCarrier("204-3", "Voiceworks Mobile BV");
            AddCarrier("204-15", "Ziggo BV");
            AddCarrier("362-630", "Cingular Wireless");
            AddCarrier("362-69", "DigiCell");
            AddCarrier("362-51", "TELCELL GSM");
            AddCarrier("362-91", "SETEL GSM");
            AddCarrier("362-951", "UTS Wireless");
            AddCarrier("546-1", "OPT Mobilis");
            AddCarrier("530-28", "2degrees");
            AddCarrier("530-5", "Telecom Mobile Ltd");
            AddCarrier("530-2", "NZ Telecom CDMA");
            AddCarrier("530-4", "Telstra");
            AddCarrier("530-24", "Two Degrees Mobile Ltd");
            AddCarrier("530-1", "Vodafone");
            AddCarrier("530-3", "Walker Wireless Ltd.");
            AddCarrier("710-21", "Empresa Nicaraguense de Telecomunicaciones SA (ENITEL)");
            AddCarrier("710-30", "Movistar");
            AddCarrier("710-73", "Claro");
            AddCarrier("614-3", "Etisalat/TeleCel");
            AddCarrier("614-4", "Orange/Sahelc.");
            AddCarrier("614-1", "Orange/Sahelc.");
            AddCarrier("614-2", "Zain/CelTel");
            AddCarrier("621-20", "Airtel/ZAIN/Econet");
            AddCarrier("621-60", "ETISALAT");
            AddCarrier("621-50", "Glo Mobile");
            AddCarrier("621-40", "M-Tel/Nigeria Telecom. Ltd.");
            AddCarrier("621-30", "MTN");
            AddCarrier("621-99", "Starcomms");
            AddCarrier("621-25", "Visafone");
            AddCarrier("621-1", "Visafone");
            AddCarrier("555-1", "Niue Telecom");
            AddCarrier("242-9", "Com4 AS");
            AddCarrier("242-21", "Jernbaneverket (GSM-R)");
            AddCarrier("242-20", "Jernbaneverket (GSM-R)");
            AddCarrier("242-23", "Lycamobile Ltd");
            AddCarrier("242-2", "Netcom");
            AddCarrier("242-5", "Network Norway AS");
            AddCarrier("242-22", "Network Norway AS");
            AddCarrier("242-6", "ICE Nordisk Mobiltelefon AS");
            AddCarrier("242-8", "TDC Mobil A/S");
            AddCarrier("242-4", "Tele2");
            AddCarrier("242-12", "Telenor");
            AddCarrier("242-1", "Telenor");
            AddCarrier("242-3", "Teletopia");
            AddCarrier("242-7", "Ventelo AS");
            AddCarrier("422-3", "Nawras");
            AddCarrier("422-2", "Oman Mobile/GTO");
            AddCarrier("410-8", "Instaphone");
            AddCarrier("410-1", "Mobilink");
            AddCarrier("410-6", "Telenor");
            AddCarrier("410-3", "UFONE/PAKTel");
            AddCarrier("410-7", "Warid Telecom");
            AddCarrier("410-4", "ZONG/CMPak");
            AddCarrier("552-80", "Palau Mobile Corp. (PMC) (Palau");
            AddCarrier("552-1", "Palau National Communications Corp. (PNCC) (Palau");
            AddCarrier("425-5", "Jawwal");
            AddCarrier("425-6", "Wataniya Mobile");
            AddCarrier("714-1", "Cable & Wireless S.A.");
            AddCarrier("714-3", "Claro");
            AddCarrier("714-4", "Digicel");
            AddCarrier("714-20", "Movistar");
            AddCarrier("714-2", "Movistar");
            AddCarrier("537-3", "Digicel");
            AddCarrier("537-2", "GreenCom PNG Ltd");
            AddCarrier("537-1", "Pacific Mobile");
            AddCarrier("744-2", "Claro/Hutchison");
            AddCarrier("744-3", "Compa");
            AddCarrier("744-1", "Hola/VOX");
            AddCarrier("744-5", "TIM/Nucleo/Personal");
            AddCarrier("744-4", "Tigo/Telecel");
            AddCarrier("716-20", "Claro /Amer.Mov./TIM");
            AddCarrier("716-10", "Claro /Amer.Mov./TIM");
            AddCarrier("716-2", "GlobalStar");
            AddCarrier("716-1", "GlobalStar");
            AddCarrier("716-6", "Movistar");
            AddCarrier("716-7", "Nextel");
            AddCarrier("515-0", "Fix Line");
            AddCarrier("515-1", "Globe Telecom");
            AddCarrier("515-2", "Globe Telecom");
            AddCarrier("515-88", "Next Mobile");
            AddCarrier("515-18", "RED Mobile/Cure");
            AddCarrier("515-3", "Smart");
            AddCarrier("515-5", "SUN/Digitel");
            AddCarrier("260-17", "Aero2 SP.");
            AddCarrier("260-18", "AMD Telecom.");
            AddCarrier("260-38", "CallFreedom Sp. z o.o.");
            AddCarrier("260-12", "Cyfrowy POLSAT S.A.");
            AddCarrier("260-8", "e-Telko");
            AddCarrier("260-9", "Lycamobile");
            AddCarrier("260-16", "Mobyland");
            AddCarrier("260-36", "Mundio Mobile Sp. z o.o.");
            AddCarrier("260-7", "Play/P4");
            AddCarrier("260-11", "NORDISK Polska");
            AddCarrier("260-5", "Orange/IDEA/Centertel");
            AddCarrier("260-3", "Orange/IDEA/Centertel");
            AddCarrier("260-35", "PKP Polskie Linie Kolejowe S.A.");
            AddCarrier("260-98", "Play/P4");
            AddCarrier("260-6", "Play/P4");
            AddCarrier("260-1", "Polkomtel/Plus");
            AddCarrier("260-10", "Sferia");
            AddCarrier("260-14", "Sferia");
            AddCarrier("260-13", "Sferia");
            AddCarrier("260-34", "T-Mobile/ERA");
            AddCarrier("260-2", "T-Mobile/ERA");
            AddCarrier("260-4", "Tele2");
            AddCarrier("260-15", "Tele2");
            AddCarrier("268-4", "CTT - Correios de Portugal SA");
            AddCarrier("268-7", "Optimus");
            AddCarrier("268-3", "Optimus");
            AddCarrier("268-6", "TMN");
            AddCarrier("268-1", "Vodafone");
            AddCarrier("330-11", "Puerto Rico Telephone Company Inc. (PRTC)");
            AddCarrier("427-1", "Qtel");
            AddCarrier("427-2", "Vodafone");
            AddCarrier("647-0", "Orange");
            AddCarrier("647-2", "Outremer Telecom");
            AddCarrier("647-10", "SFR");
            AddCarrier("226-3", "Cosmote");
            AddCarrier("226-11", "Enigma Systems");
            AddCarrier("226-10", "Orange");
            AddCarrier("226-5", "RCS&RDS Digi Mobile");
            AddCarrier("226-2", "Romtelecom SA");
            AddCarrier("226-6", "Telemobil/Zapp");
            AddCarrier("226-1", "Vodafone");
            AddCarrier("226-4", "Telemobil/Zapp");
            AddCarrier("250-12", "Baykal Westcom");
            AddCarrier("250-28", "Bee Line GSM");
            AddCarrier("250-10", "DTC/Don Telecom");
            AddCarrier("250-20", "JSC Rostov Cellular Communications");
            AddCarrier("250-13", "Kuban GSM");
            AddCarrier("250-35", "LLC Ekaterinburg-2000");
            AddCarrier("250-20", "LLC Personal Communication Systems in the Region");
            AddCarrier("250-2", "Megafon");
            AddCarrier("250-1", "MTS");
            AddCarrier("250-3", "NCC");
            AddCarrier("250-16", "NTC");
            AddCarrier("250-19", "OJSC Altaysvyaz");
            AddCarrier("250-99", "OJSC Vimpel-Communications (VimpelCom)");
            AddCarrier("250-11", "Orensot");
            AddCarrier("250-92", "Printelefone");
            AddCarrier("250-4", "Sibchallenge");
            AddCarrier("250-44", "StavTelesot");
            AddCarrier("250-20", "Tele2/ECC/Volgogr.");
            AddCarrier("250-93", "Telecom XXL");
            AddCarrier("250-39", "U-Tel/Ermak RMS");
            AddCarrier("250-17", "U-Tel/Ermak RMS");
            AddCarrier("250-39", "UralTel");
            AddCarrier("250-17", "UralTel");
            AddCarrier("250-5", "Yenisey Telecom");
            AddCarrier("250-15", "ZAO SMARTS");
            AddCarrier("250-7", "ZAO SMARTS");
            AddCarrier("635-14", "Airtel Rwanda Ltd");
            AddCarrier("635-10", "MTN/Rwandacell");
            AddCarrier("635-13", "TIGO");
            AddCarrier("356-110", "Cable & Wireless");
            AddCarrier("356-50", "Digicel");
            AddCarrier("356-70", "UTS Cariglobe");
            AddCarrier("358-110", "Cable & Wireless");
            AddCarrier("358-30", "Cingular Wireless");
            AddCarrier("358-50", "Digicel (St Lucia) Limited");
            AddCarrier("549-27", "Samoatel Mobile");
            AddCarrier("549-1", "Telecom Samoa Cellular Ltd.");
            AddCarrier("292-1", "Prima Telecom");
            AddCarrier("626-1", "CSTmovel");
            AddCarrier("901-14", "AeroMobile");
            AddCarrier("901-11", "InMarSAT");
            AddCarrier("901-12", "Maritime Communications Partner AS");
            AddCarrier("901-5", "Thuraya Satellite");
            AddCarrier("420-7", "Zain");
            AddCarrier("420-3", "Etihad/Etisalat/Mobily");
            AddCarrier("420-1", "STC/Al Jawal");
            AddCarrier("420-4", "Zain");
            AddCarrier("608-3", "Expresso/Sudatel");
            AddCarrier("608-1", "Orange/Sonatel");
            AddCarrier("608-2", "Sentel GSM");
            AddCarrier("220-3", "MTS/Telekom Srbija");
            AddCarrier("220-2", "Telenor/Mobtel");
            AddCarrier("220-1", "Telenor/Mobtel");
            AddCarrier("220-5", "VIP Mobile");
            AddCarrier("633-10", "Airtel");
            AddCarrier("633-1", "C&W");
            AddCarrier("633-2", "Smartcom");
            AddCarrier("619-3", "Africel");
            AddCarrier("619-5", "Africel");
            AddCarrier("619-1", "Zain/Celtel");
            AddCarrier("619-4", "Comium");
            AddCarrier("619-2", "Tigo/Millicom");
            AddCarrier("619-25", "Mobitel");
            AddCarrier("525-12", "GRID Communications Pte Ltd");
            AddCarrier("525-3", "MobileOne Ltd");
            AddCarrier("525-1", "Singtel");
            AddCarrier("525-7", "Singtel");
            AddCarrier("525-2", "Singtel");
            AddCarrier("525-5", "Starhub");
            AddCarrier("525-6", "Starhub");
            AddCarrier("362-51", "TelCell NV (Sint Maarten");
            AddCarrier("362-91", "UTS St. Maarten (Sint Maarten");
            AddCarrier("231-6", "O2");
            AddCarrier("231-1", "Orange");
            AddCarrier("231-5", "Orange");
            AddCarrier("231-15", "Orange");
            AddCarrier("231-2", "T-Mobile");
            AddCarrier("231-4", "T-Mobile");
            AddCarrier("231-99", "Zeleznice Slovenskej republiky (ZSR)");
            AddCarrier("293-41", "Dukagjini Telecommunications Sh.P.K.");
            AddCarrier("293-41", "Ipko Telecommunications d. o. o.");
            AddCarrier("293-41", "Mobitel");
            AddCarrier("293-40", "SI.Mobil");
            AddCarrier("293-10", "Slovenske zeleznice d.o.o.");
            AddCarrier("293-64", "T-2 d.o.o.");
            AddCarrier("293-70", "TusMobil/VEGA");
            AddCarrier("540-2", "bemobile");
            AddCarrier("540-10", "BREEZE");
            AddCarrier("540-1", "BREEZE");
            AddCarrier("637-30", "Golis");
            AddCarrier("637-19", "HorTel");
            AddCarrier("637-60", "Nationlink");
            AddCarrier("637-10", "Nationlink");
            AddCarrier("637-4", "Somafone");
            AddCarrier("637-82", "Telcom Mobile Somalia");
            AddCarrier("637-1", "Telesom");
            AddCarrier("655-2", "8.ta");
            AddCarrier("655-21", "Cape Town Metropolitan");
            AddCarrier("655-7", "Cell C");
            AddCarrier("655-12", "MTN");
            AddCarrier("655-10", "MTN");
            AddCarrier("655-6", "Sentech");
            AddCarrier("655-1", "Vodacom");
            AddCarrier("655-19", "Wireless Business Solutions (Pty) Ltd");
            AddCarrier("659-3", "Gemtel Ltd (South Sudan");
            AddCarrier("659-2", "MTN South Sudan (South Sudan");
            AddCarrier("659-4", "Network of The World Ltd (NOW) (South Sudan");
            AddCarrier("659-6", "Zain South Sudan (South Sudan");
            AddCarrier("214-23", "Lycamobile SL");
            AddCarrier("214-22", "Movistar");
            AddCarrier("214-15", "BT Espana Compania de Servicios Globales de Telecomunicaciones SAU");
            AddCarrier("214-18", "Cableuropa SAU (ONO)");
            AddCarrier("214-8", "Euskaltel SA");
            AddCarrier("214-20", "fonYou Wireless SL");
            AddCarrier("214-21", "Jazz Telecom SAU");
            AddCarrier("214-26", "Lleida");
            AddCarrier("214-25", "Lycamobile SL");
            AddCarrier("214-7", "Movistar2");
            AddCarrier("214-5", "Movistar");
            AddCarrier("214-9", "Orange");
            AddCarrier("214-3", "Orange2");
            AddCarrier("214-11", "Orange");
            AddCarrier("214-17", "R Cable y Telecomunicaciones Galicia SA");
            AddCarrier("214-19", "Simyo/KPN");
            AddCarrier("214-16", "Telecable de Asturias SA");
            AddCarrier("214-27", "Truphone");
            AddCarrier("214-1", "Vodafone");
            AddCarrier("214-6", "Vodafone Enabler Espana SL");
            AddCarrier("214-4", "Yoigo");
            AddCarrier("413-5", "Bharti Airtel");
            AddCarrier("413-3", "Etisalat/Tigo");
            AddCarrier("413-8", "H3G Hutchison");
            AddCarrier("413-1", "Mobitel Ltd.");
            AddCarrier("413-2", "MTN/Dialog");
            AddCarrier("308-1", "Ameris");
            AddCarrier("360-110", "C & W");
            AddCarrier("360-100", "Cingular");
            AddCarrier("360-10", "Cingular");
            AddCarrier("360-50", "Digicel");
            AddCarrier("360-70", "Digicel");
            AddCarrier("634-0", "Canar Telecom");
            AddCarrier("634-22", "MTN");
            AddCarrier("634-2", "MTN");
            AddCarrier("634-15", "Sudani One");
            AddCarrier("634-7", "Sudani One");
            AddCarrier("634-5", "Vivacell");
            AddCarrier("634-8", "Vivacell");
            AddCarrier("634-1", "ZAIN/Mobitel");
            AddCarrier("634-6", "ZAIN/Mobitel");
            AddCarrier("746-3", "Digicel");
            AddCarrier("746-2", "Telecommunicatiebedrijf Suriname (TELESUR)");
            AddCarrier("746-1", "Telesur");
            AddCarrier("746-4", "UNIQA");
            AddCarrier("653-10", "Swazi MTN");
            AddCarrier("653-1", "SwaziTelecom");
            AddCarrier("240-35", "42 Telecom AB");
            AddCarrier("240-16", "42 Telecom AB");
            AddCarrier("240-26", "Beepsend");
            AddCarrier("240-0", "Compatel");
            AddCarrier("240-28", "CoolTEL Aps");
            AddCarrier("240-25", "Digitel Mobile Srl");
            AddCarrier("240-22", "Eu Tel AB");
            AddCarrier("240-27", "Fogg Mobile AB");
            AddCarrier("240-18", "Generic Mobile Systems Sweden AB");
            AddCarrier("240-17", "Gotalandsnatet AB");
            AddCarrier("240-4", "H3G Access AB");
            AddCarrier("240-2", "H3G Access AB");
            AddCarrier("240-36", "ID Mobile");
            AddCarrier("240-23", "Infobip Ltd.");
            AddCarrier("240-11", "Lindholmen Science Park AB");
            AddCarrier("240-12", "Lycamobile Ltd");
            AddCarrier("240-29", "Mercury International Carrier Services");
            AddCarrier("240-3", "Orange");
            AddCarrier("240-10", "Spring Mobil AB");
            AddCarrier("240-14", "TDC Sverige AB");
            AddCarrier("240-7", "Tele2 Sverige AB");
            AddCarrier("240-5", "Tele2 Sverige AB");
            AddCarrier("240-24", "Tele2 Sverige AB");
            AddCarrier("240-24", "Telenor (Vodafone)");
            AddCarrier("240-8", "Telenor (Vodafone)");
            AddCarrier("240-4", "Telenor (Vodafone)");
            AddCarrier("240-6", "Telenor (Vodafone)");
            AddCarrier("240-9", "Telenor Mobile Sverige AS");
            AddCarrier("240-5", "Telia Mobile");
            AddCarrier("240-1", "Telia Mobile");
            AddCarrier("240-0", "EUTel");
            AddCarrier("240-8", "Timepiece Servicos De Consultoria LDA (Universal Telecom)");
            AddCarrier("240-13", "Ventelo Sverige AB");
            AddCarrier("240-20", "Wireless Maingate AB");
            AddCarrier("240-15", "Wireless Maingate Nordic AB");
            AddCarrier("228-51", "BebbiCell AG");
            AddCarrier("228-9", "Comfone AG");
            AddCarrier("228-5", "Comfone AG");
            AddCarrier("228-7", "TDC Sunrise");
            AddCarrier("228-54", "Lycamobile AG");
            AddCarrier("228-52", "Mundio Mobile AG");
            AddCarrier("228-3", "Orange");
            AddCarrier("228-1", "Swisscom");
            AddCarrier("228-12", "TDC Sunrise");
            AddCarrier("228-2", "TDC Sunrise");
            AddCarrier("228-8", "TDC Sunrise");
            AddCarrier("228-53", "upc cablecom GmbH");
            AddCarrier("417-2", "MTN/Spacetel");
            AddCarrier("417-9", "Syriatel Holdings");
            AddCarrier("417-1", "Syriatel Holdings");
            AddCarrier("466-68", "ACeS Taiwan - ACeS Taiwan Telecommunications Co Ltd");
            AddCarrier("466-5", "Asia Pacific Telecom Co. Ltd (APT)");
            AddCarrier("466-11", "Chunghwa Telecom LDM");
            AddCarrier("466-92", "Chunghwa Telecom LDM");
            AddCarrier("466-7", "Far EasTone");
            AddCarrier("466-6", "Far EasTone");
            AddCarrier("466-2", "Far EasTone");
            AddCarrier("466-1", "Far EasTone");
            AddCarrier("466-3", "Far EasTone");
            AddCarrier("466-10", "Global Mobile Corp.");
            AddCarrier("466-56", "International Telecom Co. Ltd (FITEL)");
            AddCarrier("466-88", "KG Telecom");
            AddCarrier("466-99", "TransAsia");
            AddCarrier("466-97", "Taiwan Cellular");
            AddCarrier("466-93", "Mobitai");
            AddCarrier("466-89", "VIBO");
            AddCarrier("466-9", "VMAX Telecom Co. Ltd");
            AddCarrier("436-4", "Babilon-M");
            AddCarrier("436-5", "Bee Line");
            AddCarrier("436-2", "CJSC Indigo Tajikistan");
            AddCarrier("436-12", "Tcell/JC Somoncom");
            AddCarrier("436-3", "MLT/TT mobile");
            AddCarrier("436-1", "Tcell/JC Somoncom");
            AddCarrier("640-8", "Benson Informatics Ltd");
            AddCarrier("640-6", "Dovetel (T) Ltd");
            AddCarrier("640-9", "ExcellentCom (T) Ltd");
            AddCarrier("640-11", "Smile Communications Tanzania Ltd");
            AddCarrier("640-7", "Tanzania Telecommunications Company Ltd (TTCL)");
            AddCarrier("640-2", "TIGO/MIC");
            AddCarrier("640-1", "Tri Telecomm. Ltd.");
            AddCarrier("640-4", "Vodacom Ltd");
            AddCarrier("640-5", "ZAIN/Celtel");
            AddCarrier("640-3", "Zantel/Zanzibar Telecom");
            AddCarrier("520-20", "ACeS Thailand - ACeS Regional Services Co Ltd");
            AddCarrier("520-15", "ACT Mobile");
            AddCarrier("520-3", "Advanced Wireless Networks/AWN");
            AddCarrier("520-1", "AIS/Advanced Info Service");
            AddCarrier("520-23", "Digital Phone Co.");
            AddCarrier("520-0", "Hutch/CAT CDMA");
            AddCarrier("520-18", "Total Access (DTAC)");
            AddCarrier("520-5", "Total Access (DTAC)");
            AddCarrier("520-99", "True Move/Orange");
            AddCarrier("520-4", "True Move/Orange");
            AddCarrier("514-1", "Telin/ Telkomcel");
            AddCarrier("514-2", "Timor Telecom");
            AddCarrier("615-2", "Telecel/MOOV");
            AddCarrier("615-3", "Telecel/MOOV");
            AddCarrier("615-1", "Togo Telecom/TogoCELL");
            AddCarrier("539-43", "Shoreline Communication");
            AddCarrier("539-1", "Tonga Communications");
            AddCarrier("374-129", "Bmobile/TSTT");
            AddCarrier("374-130", "Digicel");
            AddCarrier("374-140", "LaqTel Ltd.");
            AddCarrier("605-1", "Orange");
            AddCarrier("605-3", "Orascom Telecom");
            AddCarrier("605-2", "Tunisie Telecom");
            AddCarrier("286-4", "AVEA/Aria");
            AddCarrier("286-3", "AVEA/Aria");
            AddCarrier("286-1", "Turkcell");
            AddCarrier("286-2", "Vodafone-Telsim");
            AddCarrier("438-1", "Barash Communication");
            AddCarrier("438-2", "TM-Cell");
            AddCarrier("376-350", "Cable & Wireless (TCI) Ltd");
            AddCarrier("376-50", "Digicel TCI Ltd");
            AddCarrier("376-352", "IslandCom Communications Ltd.");
            AddCarrier("553-1", "Tuvalu Telecommunication Corporation (TTC)");
            AddCarrier("641-1", "Celtel");
            AddCarrier("641-66", "i-Tel Ltd");
            AddCarrier("641-30", "K2 Telecom Ltd");
            AddCarrier("641-10", "MTN Ltd.");
            AddCarrier("641-14", "Orange");
            AddCarrier("641-33", "Smile Communications Uganda Ltd");
            AddCarrier("641-18", "Suretelecom Uganda Ltd");
            AddCarrier("641-11", "Uganda Telecom Ltd.");
            AddCarrier("641-22", "Warid Telecom");
            AddCarrier("255-6", "Astelit/LIFE");
            AddCarrier("255-5", "Golden Telecom");
            AddCarrier("255-39", "Golden Telecom");
            AddCarrier("255-4", "Intertelecom Ltd (IT)");
            AddCarrier("255-67", "KyivStar");
            AddCarrier("255-3", "KyivStar");
            AddCarrier("255-21", "Telesystems Of Ukraine CJSC (TSU)");
            AddCarrier("255-7", "TriMob LLC");
            AddCarrier("255-50", "UMC/MTS");
            AddCarrier("255-2", "Beeline");
            AddCarrier("255-1", "UMC/MTS");
            AddCarrier("255-68", "Beeline");
            AddCarrier("424-3", "DU");
            AddCarrier("430-2", "Etisalat");
            AddCarrier("424-2", "Etisalat");
            AddCarrier("431-2", "Etisalat");
            AddCarrier("234-3", "Airtel/Vodafone");
            AddCarrier("234-77", "BT Group");
            AddCarrier("234-76", "BT Group");
            AddCarrier("234-7", "Cable and Wireless");
            AddCarrier("234-92", "Cable and Wireless");
            AddCarrier("234-36", "Calbe and Wireless Isle of Man");
            AddCarrier("234-18", "Cloud9/wire9 Tel.");
            AddCarrier("235-2", "Everyth. Ev.wh.");
            AddCarrier("234-17", "FlexTel");
            AddCarrier("234-55", "Guernsey Telecoms");
            AddCarrier("234-14", "HaySystems");
            AddCarrier("234-20", "Hutchinson 3G");
            AddCarrier("234-94", "Hutchinson 3G");
            AddCarrier("234-75", "Inquam Telecom Ltd");
            AddCarrier("234-50", "Jersey Telecom");
            AddCarrier("234-35", "JSC Ingenicum");
            AddCarrier("234-26", "Lycamobile");
            AddCarrier("234-58", "Manx Telecom");
            AddCarrier("234-1", "Mapesbury C. Ltd");
            AddCarrier("234-28", "Marthon Telecom");
            AddCarrier("234-10", "O2 Ltd.");
            AddCarrier("234-2", "O2 Ltd.");
            AddCarrier("234-11", "O2 Ltd.");
            AddCarrier("234-8", "OnePhone");
            AddCarrier("234-16", "Opal Telecom");
            AddCarrier("234-34", "Everyth. Ev.wh./Orange");
            AddCarrier("234-33", "Everyth. Ev.wh./Orange");
            AddCarrier("234-19", "PMN/Teleware");
            AddCarrier("234-12", "Railtrack Plc");
            AddCarrier("234-22", "Routotelecom");
            AddCarrier("234-24", "Stour Marine");
            AddCarrier("234-37", "Synectiv Ltd.");
            AddCarrier("234-31", "Everyth. Ev.wh./T-Mobile");
            AddCarrier("234-30", "Everyth. Ev.wh./T-Mobile");
            AddCarrier("234-32", "Everyth. Ev.wh./T-Mobile");
            AddCarrier("234-27", "Vodafone");
            AddCarrier("234-9", "Tismi");
            AddCarrier("234-25", "Truphone");
            AddCarrier("234-51", "Jersey Telecom");
            AddCarrier("234-23", "Vectofone Mobile Wifi");
            AddCarrier("234-15", "Vodafone");
            AddCarrier("234-91", "Vodafone");
            AddCarrier("234-78", "Wave Telecom Ltd");
            AddCarrier("310-50", "");
            AddCarrier("310-880", "");
            AddCarrier("310-850", "Aeris Comm. Inc.");
            AddCarrier("310-640", "");
            AddCarrier("310-510", "Airtel Wireless LLC");
            AddCarrier("310-190", "Unknown");
            AddCarrier("312-90", "Allied Wireless Communications Corporation");
            AddCarrier("311-130", "");
            AddCarrier("311-30", "Americell PA3 LP");
            AddCarrier("310-710", "Arctic Slope Telephone Association Cooperative Inc.");
            AddCarrier("310-150", "AT&T Wireless Inc.");
            AddCarrier("310-680", "AT&T Wireless Inc.");
            AddCarrier("310-70", "AT&T Wireless Inc.");
            AddCarrier("310-560", "AT&T Wireless Inc.");
            AddCarrier("310-410", "AT&T Wireless Inc.");
            AddCarrier("310-380", "AT&T Wireless Inc.");
            AddCarrier("310-170", "AT&T Wireless Inc.");
            AddCarrier("310-980", "AT&T Wireless Inc.");
            AddCarrier("311-810", "Bluegrass Wireless LLC");
            AddCarrier("311-800", "Bluegrass Wireless LLC");
            AddCarrier("311-440", "Bluegrass Wireless LLC");
            AddCarrier("310-900", "Cable & Communications Corp.");
            AddCarrier("311-590", "California RSA No. 3 Limited Partnership");
            AddCarrier("311-500", "Cambridge Telephone Company Inc.");
            AddCarrier("310-830", "Caprock Cellular Ltd.");
            AddCarrier("311-270", "Verizon Wireless");
            AddCarrier("311-286", "Verizon Wireless");
            AddCarrier("311-480", "Verizon Wireless");
            AddCarrier("311-275", "Verizon Wireless");
            AddCarrier("311-485", "Verizon Wireless");
            AddCarrier("310-12", "Verizon Wireless");
            AddCarrier("311-280", "Verizon Wireless");
            AddCarrier("311-110", "Verizon Wireless");
            AddCarrier("311-285", "Verizon Wireless");
            AddCarrier("311-390", "Verizon Wireless");
            AddCarrier("311-274", "Verizon Wireless");
            AddCarrier("311-484", "Verizon Wireless");
            AddCarrier("310-10", "Verizon Wireless");
            AddCarrier("311-279", "Verizon Wireless");
            AddCarrier("311-489", "Verizon Wireless");
            AddCarrier("310-910", "Verizon Wireless");
            AddCarrier("311-284", "Verizon Wireless");
            AddCarrier("311-273", "Verizon Wireless");
            AddCarrier("311-289", "Verizon Wireless");
            AddCarrier("311-483", "Verizon Wireless");
            AddCarrier("310-4", "Verizon Wireless");
            AddCarrier("311-278", "Verizon Wireless");
            AddCarrier("311-488", "Verizon Wireless");
            AddCarrier("310-890", "Verizon Wireless");
            AddCarrier("311-283", "Verizon Wireless");
            AddCarrier("311-272", "Verizon Wireless");
            AddCarrier("311-288", "Verizon Wireless");
            AddCarrier("311-482", "Verizon Wireless");
            AddCarrier("311-277", "Verizon Wireless");
            AddCarrier("311-487", "Verizon Wireless");
            AddCarrier("310-590", "Verizon Wireless");
            AddCarrier("311-282", "Verizon Wireless");
            AddCarrier("311-271", "Verizon Wireless");
            AddCarrier("311-287", "Verizon Wireless");
            AddCarrier("311-481", "Verizon Wireless");
            AddCarrier("311-276", "Verizon Wireless");
            AddCarrier("311-486", "Verizon Wireless");
            AddCarrier("310-13", "Verizon Wireless");
            AddCarrier("311-281", "Verizon Wireless");
            AddCarrier("312-270", "Cellular Network Partnership LLC");
            AddCarrier("310-360", "Cellular Network Partnership LLC");
            AddCarrier("312-280", "Cellular Network Partnership LLC");
            AddCarrier("311-190", "");
            AddCarrier("310-230", "Cellular South Licenses Inc.");
            AddCarrier("310-30", "");
            AddCarrier("311-120", "Choice Phone LLC");
            AddCarrier("310-480", "Choice Phone LLC");
            AddCarrier("310-630", "");
            AddCarrier("310-420", "Cincinnati Bell Wireless LLC");
            AddCarrier("310-180", "Cingular Wireless");
            AddCarrier("310-620", "Coleman County Telco /Trans TX");
            AddCarrier("311-40", "");
            AddCarrier("310-6", "Consolidated Telcom");
            AddCarrier("310-60", "Consolidated Telcom");
            AddCarrier("312-380", "");
            AddCarrier("310-930", "");
            AddCarrier("311-240", "");
            AddCarrier("310-8", "Unknown");
            AddCarrier("310-80", "");
            AddCarrier("310-700", "Cross Valliant Cellular Partnership");
            AddCarrier("312-30", "Cross Wireless Telephone Co.");
            AddCarrier("311-140", "Cross Wireless Telephone Co.");
            AddCarrier("311-520", "");
            AddCarrier("311-440", "Cumberland Cellular Partnership");
            AddCarrier("311-810", "Cumberland Cellular Partnership");
            AddCarrier("311-800", "Cumberland Cellular Partnership");
            AddCarrier("312-40", "Custer Telephone Cooperative Inc.");
            AddCarrier("310-16", "Denali Spectrum License LLC");
            AddCarrier("310-440", "Dobson Cellular Systems");
            AddCarrier("310-990", "E.N.M.R. Telephone Coop.");
            AddCarrier("310-750", "East Kentucky Network LLC");
            AddCarrier("312-130", "East Kentucky Network LLC");
            AddCarrier("312-120", "East Kentucky Network LLC");
            AddCarrier("310-9", "Edge Wireless LLC");
            AddCarrier("310-90", "Edge Wireless LLC");
            AddCarrier("310-610", "Elkhart TelCo. / Epic Touch Co.");
            AddCarrier("311-210", "");
            AddCarrier("311-311", "Farmers");
            AddCarrier("311-460", "Fisher Wireless Services Inc.");
            AddCarrier("311-370", "GCI Communication Corp.");
            AddCarrier("310-430", "GCI Communication Corp.");
            AddCarrier("310-920", "Get Mobile Inc.");
            AddCarrier("310-970", "");
            AddCarrier("310-7", "Unknown");
            AddCarrier("311-250", "i CAN_GSM");
            AddCarrier("311-340", "Illinois Valley Cellular RSA 2 Partnership");
            AddCarrier("311-30", "");
            AddCarrier("311-410", "Iowa RSA No. 2 Limited Partnership");
            AddCarrier("312-170", "Iowa RSA No. 2 Limited Partnership");
            AddCarrier("310-770", "Iowa Wireless Services LLC");
            AddCarrier("310-650", "Jasper");
            AddCarrier("310-870", "Kaplan Telephone Company Inc.");
            AddCarrier("311-800", "Kentucky RSA #3 Cellular General Partnership");
            AddCarrier("311-440", "Kentucky RSA #3 Cellular General Partnership");
            AddCarrier("311-810", "Kentucky RSA #3 Cellular General Partnership");
            AddCarrier("311-810", "Kentucky RSA #4 Cellular General Partnership");
            AddCarrier("311-800", "Kentucky RSA #4 Cellular General Partnership");
            AddCarrier("311-440", "Kentucky RSA #4 Cellular General Partnership");
            AddCarrier("312-180", "Keystone Wireless LLC");
            AddCarrier("310-690", "Keystone Wireless LLC");
            AddCarrier("311-310", "Lamar County Cellular");
            AddCarrier("310-16", "LCW Wireless Operations LLC");
            AddCarrier("310-16", "Leap Wireless International Inc.");
            AddCarrier("311-90", "");
            AddCarrier("310-40", "Matanuska Tel. Assn. Inc.");
            AddCarrier("310-780", "Message Express Co. / Airlink PCS");
            AddCarrier("311-660", "");
            AddCarrier("311-330", "Michigan Wireless LLC");
            AddCarrier("311-0", "");
            AddCarrier("311-390", "");
            AddCarrier("310-400", "Minnesota South. Wirel. Co. / Hickory");
            AddCarrier("311-20", "Missouri RSA No 5 Partnership");
            AddCarrier("311-10", "Missouri RSA No 5 Partnership");
            AddCarrier("312-220", "Missouri RSA No 5 Partnership");
            AddCarrier("312-10", "Missouri RSA No 5 Partnership");
            AddCarrier("311-920", "Missouri RSA No 5 Partnership");
            AddCarrier("310-350", "Mohave Cellular LP");
            AddCarrier("310-570", "MTPCS LLC");
            AddCarrier("310-290", "NEP Cellcorp Inc.");
            AddCarrier("310-34", "Nevada Wireless LLC");
            AddCarrier("311-380", "");
            AddCarrier("310-600", "New-Cell Inc.");
            AddCarrier("311-100", "");
            AddCarrier("311-300", "Nexus Communications Inc.");
            AddCarrier("310-130", "North Carolina RSA 3 Cellular Tel. Co.");
            AddCarrier("312-230", "North Dakota Network Company");
            AddCarrier("311-610", "North Dakota Network Company");
            AddCarrier("310-450", "Northeast Colorado Cellular Inc.");
            AddCarrier("311-710", "Northeast Wireless Networks LLC");
            AddCarrier("310-670", "Northstar");
            AddCarrier("310-11", "Northstar");
            AddCarrier("311-420", "Northwest Missouri Cellular Limited Partnership");
            AddCarrier("310-540", "");
            AddCarrier("310-740", "");
            AddCarrier("310-760", "Panhandle Telephone Cooperative Inc.");
            AddCarrier("310-580", "PCS ONE");
            AddCarrier("311-170", "PetroCom");
            AddCarrier("311-670", "Pine Belt Cellular, Inc.");
            AddCarrier("311-80", "");
            AddCarrier("310-790", "");
            AddCarrier("310-100", "Plateau Telecommunications Inc.");
            AddCarrier("310-940", "Poka Lambro Telco Ltd.");
            AddCarrier("311-730", "");
            AddCarrier("311-540", "");
            AddCarrier("310-500", "Public Service Cellular Inc.");
            AddCarrier("312-160", "RSA 1 Limited Partnership");
            AddCarrier("311-430", "RSA 1 Limited Partnership");
            AddCarrier("311-350", "Sagebrush Cellular Inc.");
            AddCarrier("311-30", "Sagir Inc.");
            AddCarrier("311-910", "");
            AddCarrier("310-46", "SIMMETRY");
            AddCarrier("311-260", "SLO Cellular Inc / Cellular One of San Luis");
            AddCarrier("310-320", "Smith Bagley Inc.");
            AddCarrier("310-15", "Unknown");
            AddCarrier("316-11", "Southern Communications Services Inc.");
            AddCarrier("310-2", "Sprint Spectrum");
            AddCarrier("311-870", "Sprint Spectrum");
            AddCarrier("311-490", "Sprint Spectrum");
            AddCarrier("310-120", "Sprint Spectrum");
            AddCarrier("316-10", "Sprint Spectrum");
            AddCarrier("312-190", "Sprint Spectrum");
            AddCarrier("311-880", "Sprint Spectrum");
            AddCarrier("310-260", "T-Mobile");
            AddCarrier("310-200", "T-Mobile");
            AddCarrier("310-250", "T-Mobile");
            AddCarrier("310-160", "T-Mobile");
            AddCarrier("310-240", "T-Mobile");
            AddCarrier("310-660", "T-Mobile");
            AddCarrier("310-230", "T-Mobile");
            AddCarrier("310-31", "T-Mobile");
            AddCarrier("310-220", "T-Mobile");
            AddCarrier("310-270", "T-Mobile");
            AddCarrier("310-210", "T-Mobile");
            AddCarrier("310-300", "T-Mobile");
            AddCarrier("310-280", "T-Mobile");
            AddCarrier("310-330", "T-Mobile");
            AddCarrier("310-800", "T-Mobile");
            AddCarrier("310-310", "T-Mobile");
            AddCarrier("311-740", "");
            AddCarrier("310-740", "Telemetrix Inc.");
            AddCarrier("310-14", "Testing");
            AddCarrier("310-950", "Unknown");
            AddCarrier("310-860", "Texas RSA 15B2 Limited Partnership");
            AddCarrier("311-830", "Thumb Cellular Limited Partnership");
            AddCarrier("311-50", "Thumb Cellular Limited Partnership");
            AddCarrier("310-460", "TMP Corporation");
            AddCarrier("310-490", "Triton PCS");
            AddCarrier("310-960", "Uintah Basin Electronics Telecommunications Inc.");
            AddCarrier("312-290", "Uintah Basin Electronics Telecommunications Inc.");
            AddCarrier("311-860", "Uintah Basin Electronics Telecommunications Inc.");
            AddCarrier("310-20", "Union Telephone Co.");
            AddCarrier("311-220", "United States Cellular Corp.");
            AddCarrier("310-730", "United States Cellular Corp.");
            AddCarrier("311-650", "United Wireless Communications Inc.");
            AddCarrier("310-38", "USA 3650 AT&T");
            AddCarrier("310-520", "VeriSign");
            AddCarrier("310-3", "Unknown");
            AddCarrier("310-23", "Unknown");
            AddCarrier("310-24", "Unknown");
            AddCarrier("310-25", "Unknown");
            AddCarrier("310-530", "West Virginia Wireless");
            AddCarrier("310-26", "Unknown");
            AddCarrier("310-340", "Westlink Communications, LLC");
            AddCarrier("311-150", "");
            AddCarrier("311-70", "Wisconsin RSA #7 Limited Partnership");
            AddCarrier("310-390", "Yorkville Telephone Cooperative");
            AddCarrier("0-0", "");
            AddCarrier("748-1", "Ancel/Antel");
            AddCarrier("748-3", "Ancel/Antel");
            AddCarrier("748-10", "Claro/AM Wireless");
            AddCarrier("748-7", "MOVISTAR");
            AddCarrier("434-4", "Bee Line/Unitel");
            AddCarrier("434-1", "Buztel");
            AddCarrier("434-7", "MTS/Uzdunrobita");
            AddCarrier("434-5", "Ucell/Coscom");
            AddCarrier("434-2", "Uzmacom");
            AddCarrier("541-5", "DigiCel");
            AddCarrier("541-0", "DigiCel");
            AddCarrier("541-1", "SMILE");
            AddCarrier("734-3", "DigiTel C.A.");
            AddCarrier("734-2", "DigiTel C.A.");
            AddCarrier("734-1", "DigiTel C.A.");
            AddCarrier("734-6", "Movilnet C.A.");
            AddCarrier("734-4", "Movistar/TelCel");
            AddCarrier("452-7", "Beeline");
            AddCarrier("452-1", "Mobifone");
            AddCarrier("452-3", "S-Fone/Telecom");
            AddCarrier("452-5", "VietnaMobile");
            AddCarrier("452-8", "Viettel Mobile");
            AddCarrier("452-6", "Viettel Mobile");
            AddCarrier("452-4", "Viettel Mobile");
            AddCarrier("452-2", "Vinaphone");
            AddCarrier("376-350", "Cable & Wireless (Turks & Caicos)");
            AddCarrier("376-50", "Digicel");
            AddCarrier("376-352", "IslandCom");
            AddCarrier("421-4", "HITS/Y Unitel");
            AddCarrier("421-2", "MTN/Spacetel");
            AddCarrier("421-1", "Sabaphone");
            AddCarrier("421-3", "Yemen Mob. CDMA");
            AddCarrier("645-3", "Cell Z/MTS");
            AddCarrier("645-2", "MTN/Telecel");
            AddCarrier("645-1", "Zain/Celtel");
            AddCarrier("648-4", "Econet");
            AddCarrier("648-1", "Net One");
            AddCarrier("648-3", "Telecel");
            #endregion
        }
    }
}