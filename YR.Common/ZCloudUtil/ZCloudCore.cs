using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YR.Common.ZCloudUtil
{
    public class ZCloudCore
    {
        /// <summary>
        /// 除去数组中的空值和签名参数
        /// </summary>
        /// <param name="sArray"></param>
        /// <returns></returns>
        public static Dictionary<string, string> paraFilter(Dictionary<string, string> sArray)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (sArray == null || sArray.Count <= 0)
            {
                return result;
            }

            foreach (var item in sArray)
            {
                string key = item.Key;
                String value = item.Value;
                if (string.IsNullOrEmpty(value) || string.Compare(key, "sign", true) != 0 || string.Compare(key, "msg", true) != 0)
                {
                    continue;
                }
                result.Add(key, value);
            }

            return result;
        }

        /// <summary>
        /// 把数组所有元素排序，并按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static String createLinkString(Dictionary<string, string> args)
        {
            SortedDictionary<string, string> sortedDict = new SortedDictionary<string, string>();
            foreach (var item in args)
                sortedDict.Add(item.Key, item.Value);

            String prestr = "";
            foreach (var item in sortedDict)
            {
                prestr = prestr + item.Key + "=" + item.Value + "&";
            }
            if (prestr.EndsWith("&"))
            {
                prestr.Remove(prestr.Length - 1, 1);
            }

            return prestr;
        }

        /// <summary>
        /// hashtable转换为dictionary
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public static Dictionary<string, string> hashtableToDictionary(Hashtable ht)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach(string key in ht.Keys)
            {
                dict.Add(key,ht[key].ToString());
            }
            return dict;
        }
             
    }
}