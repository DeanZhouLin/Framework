using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace JFx
{
    /// <summary>
    /// 加密解密
    /// </summary>
    public static class EncryptionHelper
    {
        /// <summary>
        /// 默认AES加密解密密钥向量
        /// </summary>
        private static string encryptIV = "GD*(&$158h%^H)(_";

        #region AES
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptString">明文</param>
        /// <param name="encryptKey">密钥</param>
        /// <returns>密文</returns>
        public static string EncryptByAES(string encryptString, string encryptKey)
        {
            encryptKey = encryptKey.PadRight(32, ' ');
            encryptKey = encryptKey.Substring(0, 32);

            encryptIV = encryptIV.PadRight(16, ' ');
            encryptIV = encryptIV.Substring(0, 16);

            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(encryptKey);
            rijndaelProvider.IV = Encoding.UTF8.GetBytes(encryptIV);
            ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();
            byte[] inputData = Encoding.UTF8.GetBytes(encryptString);
            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);
            return Convert.ToBase64String(encryptedData);
        }


        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="decryptString">密文</param>
        /// <param name="decryptKey">密钥</param>
        /// <returns>明文</returns>
        public static string DecryptByAES(string decryptString, string decryptKey)
        {
            decryptKey = decryptKey.PadRight(32, ' ');
            decryptKey = decryptKey.Substring(0, 32);

            encryptIV = encryptIV.PadRight(16, ' ');
            encryptIV = encryptIV.Substring(0, 16);

            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(decryptKey);
            rijndaelProvider.IV = Encoding.UTF8.GetBytes(encryptIV);
            ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();
            byte[] inputData = Convert.FromBase64String(decryptString);
            byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);
            return Encoding.UTF8.GetString(decryptedData);
        }

        #endregion

        #region DES
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
        public static string EncryptByDES(string encryptString, string encryptKey)
        {
            encryptKey = encryptKey.PadRight(8, ' ');
            encryptKey = encryptKey.Substring(0, 8);

            encryptIV = encryptIV.PadRight(8, ' ');
            encryptIV = encryptIV.Substring(0, 8);

            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey);
            byte[] rgbIV = Encoding.UTF8.GetBytes(encryptIV);
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());

        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
        public static string DecryptByDES(string decryptString, string decryptKey)
        {
            decryptKey = decryptKey.PadRight(8, ' ');
            decryptKey = decryptKey.Substring(0, 8);

            encryptIV = encryptIV.PadRight(8, ' ');
            encryptIV = encryptIV.Substring(0, 8);

            byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
            byte[] rgbIV = Encoding.UTF8.GetBytes(encryptIV);
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        #endregion

        #region encode
        /// <summary>
        /// 将指定的字符串（它将二进制数据编码为 Base64 数字）转换为等效的 8 位无符号整数数组。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Base64Encode(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// 将指定的字符串（它将二进制数据编码为 Base64 数字）转换为等效的 8 位无符号整数数组。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Base64Decode(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            byte[] bytes = Convert.FromBase64String(str);

            return Encoding.UTF8.GetString(bytes);
        }
        #endregion

        #region MD5
        /// <summary>
        /// 获取字符串的MD5值
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static string Md5(string source)
        {
            if (source == null)
                return null;
            return FormsAuthentication.HashPasswordForStoringInConfigFile(source, "md5");
        }
        #endregion
    }
}
