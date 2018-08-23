using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YR.Common.ZCloudUtil
{

    /// <summary>
    /// 签名MD5工具类
    /// </summary>
    public class ZCloudMD5
    {
        /// <summary>
        /// 签名字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String sign(String text, String key)
        {
            text = text + key;
            return md5(text);
        }

        /// <summary>
        /// 验签字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="sign"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool verify(String text, String sign, String key)
        {
            text = text + key;
            String mysign = md5(text);
            if (mysign==sign)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 计算字符串md5值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string md5(string text)
        {
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    ret += a.ToString("x");
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }
    }
}