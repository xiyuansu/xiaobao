using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using Asiasofti.SmartVehicle.Common;

namespace YR.Web.api.app.pay.alipay
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：1.0
    /// 修改日期：2016-06-06
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// </summary>
    public class Config
    {

        //↓↓↓↓↓↓↓↓↓↓请在这里配置您的基本信息↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

        //支付宝分配给开发者的应用ID
        public static string app_id = SiteHelper.GetAppsetString("alipay_app_id");

        // 合作身份者ID，签约账号，以2088开头由16位纯数字组成的字符串，查看地址：https://openhome.alipay.com/platform/keyManage.htm?keyType=partner
        public static string partner = SiteHelper.GetAppsetString("alipay_partner");

        // 收款支付宝账号，以2088开头由16位纯数字组成的字符串，一般情况下收款账号就是签约账号
        public static string seller_id = partner;

        //商户的私钥,原始格式，RSA公私钥生成：https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.nBDxfy&treeId=58&articleId=103242&docType=1
        public static string private_key = SiteHelper.GetAppsetString("alipay_private_key");
        //public static string private_key = HttpContext.Current.Server.MapPath("/App_Code/aop-sandbox-RSA-private-c#.pem");


        //支付宝的公钥，查看地址：https://b.alipay.com/order/pidAndKey.htm 
        public static string alipay_public_key = SiteHelper.GetAppsetString("alipay_public_key");
        //public static string alipay_public_key = HttpContext.Current.Server.MapPath("/App_Code/public-key.pem");

        // 签名方式
        //public static string sign_type = "RSA";
        public static string sign_type = "RSA2";

        // 调试用，创建TXT日志文件夹路径，见AlipayCore.cs类中的LogResult(string sWord)打印方法。
        public static string log_path = HttpRuntime.AppDomainAppPath.ToString() + "log/";

        // 字符编码格式 目前支持 gbk 或 utf-8
        public static string input_charset = "utf-8";

        // 支付类型 ，无需修改
        public static string payment_type = "1";

        // 调用的接口名，无需修改
        public static string service = "mobile.securitypay.pay";

        //↑↑↑↑↑↑↑↑↑↑请在这里配置您的基本信息↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

    }
}