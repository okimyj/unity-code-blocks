using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;

namespace Pieceton.Misc
{
    public class PEncryptor
    {
        static PEncryptor _Instance;
        public static PEncryptor Instance { get { if (_Instance == null) { _Instance = new PEncryptor(); } return _Instance; } }

        private const int keyLangth = 16;

        byte[] Iv = new byte[keyLangth] { 1, 2, 3, 2, 2, 3, 2, 1, 1, 2, 8, 8, 7, 6, 1, 1 };
        byte[] simple = new byte[keyLangth] { 1, 2, 3, 2, 2, 3, 2, 1, 1, 2, 8, 8, 7, 6, 1, 1 };

        public void Init() { }

        public byte[] CreateKey()
        {
            byte[] key = new byte[keyLangth];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(key);
            return key;
        }


        public byte[] FixedEncrypt(byte[] data)
        {
            return Encrypt(simple, data, 0, data.Length);
        }

        public byte[] FixedDecrypt(byte[] data)
        {
            return Decrypt(simple, data, 0, data.Length);
        }


        public string Encrypt(byte[] key, string toEncrypt)
        {
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
            byte[] resultArray = Encrypt(key, toEncryptArray);

            if (null != resultArray)
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);

            return "";
        }

        public string Decrypt(byte[] key, string toDecrypt)
        {
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            byte[] resultArray = Decrypt(key, toEncryptArray);

            if (null != resultArray)
                return Encoding.UTF8.GetString(resultArray);

            return "";
        }


        public byte[] Encrypt(byte[] key, byte[] data, int offset = 0)
        {
            return Encrypt(key, data, offset, data.Length - offset);
        }
        public byte[] Encrypt(byte[] key, byte[] data, int offset, int count)
        {
            try
            {
                using (RijndaelManaged rDel = new RijndaelManaged())
                {
                    rDel.KeySize = 128;
                    rDel.BlockSize = 128;
                    rDel.Key = key;
                    rDel.IV = Iv;
                    rDel.Mode = CipherMode.CBC;
                    rDel.Padding = PaddingMode.PKCS7;
                    ICryptoTransform cTransform = rDel.CreateEncryptor();
                    return cTransform.TransformFinalBlock(data, offset, count);
                }
            }
            catch { }

            return null;
        }


        public Stream Encrypt(byte[] key, Stream dest, byte[] data, int offset = 0)
        {
            return Encrypt(key, dest, data, offset, (int)data.Length - offset);
        }
        public Stream Encrypt(byte[] key, Stream dest, byte[] data, int offset, int count)
        {
            byte[] buffer = Encrypt(key, data, offset, count);
            if (buffer != null)
            {
                dest.Write(buffer, 0, buffer.Length);
                return dest;
            }

            return null;
        }


        public byte[] Decrypt(byte[] key, byte[] data, int offset = 0)
        {
            return Decrypt(key, data, offset, data.Length - offset);
        }
        public byte[] Decrypt(byte[] key, byte[] data, int offset, int count)
        {
            try
            {
                using (RijndaelManaged rDel = new RijndaelManaged())
                {
                    rDel.KeySize = 128;
                    rDel.BlockSize = 128;
                    rDel.Key = key;
                    rDel.IV = Iv;
                    rDel.Mode = CipherMode.CBC;
                    rDel.Padding = PaddingMode.PKCS7;
                    ICryptoTransform cTransform = rDel.CreateDecryptor();
                    return cTransform.TransformFinalBlock(data, offset, count);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }

            return null;
        }


        public Stream Decrypt(byte[] key, Stream dest, byte[] data, int offset = 0)
        {
            return Decrypt(key, dest, data, offset, data.Length - offset);
        }
        public Stream Decrypt(byte[] key, Stream dest, byte[] data, int offset, int count)
        {
            byte[] buffer = Decrypt(key, data, offset, count);
            if (buffer != null)
            {
                dest.Write(buffer, 0, buffer.Length);
                return dest;
            }

            return null;
        }
    }
}