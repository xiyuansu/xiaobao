using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YR.Common.ZCloudUtil
{
    /// <summary>
    /// 
    /// </summary>
    public class ZCloudSignUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="md5_key"></param>
        /// <returns></returns>
        public static bool verify(Dictionary<string, string> args, String md5_key)
        {
            if (args["sign"] == null)
            {
                return false;
            }
            String sign = args["sign"];
            // 过滤空值、sign与sign_type参数
            Dictionary<String, String> sParaNew = ZCloudCore.paraFilter(args);
            // 获取待签名字符串
            String preSignStr = ZCloudCore.createLinkString(sParaNew);
            // 获得签名验证结果
            return ZCloudMD5.verify(preSignStr, sign, md5_key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="md5_key"></param>
        /// <returns></returns>
        public static String getSign(Dictionary<string, string> args, String md5_key)
        {
            // 过滤空值、sign与sign_type参数
            Dictionary<string, string> sParaNew = ZCloudCore.paraFilter(args);
            // 获取待签名字符串
            String preSignStr = ZCloudCore.createLinkString(sParaNew);
            return ZCloudMD5.sign(preSignStr, md5_key);
        }
    }
}