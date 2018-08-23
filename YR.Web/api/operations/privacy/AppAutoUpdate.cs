using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using YR.Common.DotNetCode;
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.operations.privacy
{
    /// <summary>
    /// app自动更新接口
    /// </summary>
    public class AppAutoUpdate : IApiAction2
    {
        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            string xmlfile = HttpContext.Current.Server.MapPath("/app/operations/app_ver.xml");
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlfile);

            XmlNode node_android = xml.SelectSingleNode("/ver/android");
            string androidVersion = node_android.Attributes["AndroidVersion"].Value;
            string androidUrl = node_android.Attributes["AndroidUrl"].Value;
            string androidUpdataRemark = node_android.Attributes["AndroidUpdataRemark"].Value;

            Hashtable appversion = new Hashtable();
            appversion["AndroidVersion"] = androidVersion;
            appversion["AndroidUrl"] = androidUrl;
            appversion["AndroidUpdataRemark"] = androidUpdataRemark;

            return SiteHelper.GetJsonFromHashTable(appversion, "success", "获取数据成功", "AppVersion");

        }
    }
}