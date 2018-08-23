using Asiasofti.SmartVehicle.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using YR.Web.api.api_class;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// app自动更新接口
    /// </summary>
    public class AppAutoUpdate : IApiAction2
    {
        
        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            string xmlfile = HttpContext.Current.Server.MapPath("/app/app_ver.xml");
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlfile);
            Hashtable appversion = new Hashtable();
            if (res["Client"] != null && "iOS" == res["Client"].ToString())
            {
                XmlNode node_ios = xml.SelectSingleNode("/ver/ios");
                string iOSVersion = node_ios.Attributes["IOSVersion"].Value;
                string iOSUrl = node_ios.Attributes["IOSUrl"].Value;
                string iOSUpdataRemark = node_ios.Attributes["IOSUpdataRemark"].Value;
            }
            else
            {
                XmlNode node_android = xml.SelectSingleNode("/ver/android");
                string androidVersion = node_android.Attributes["AndroidVersion"].Value;
                string androidUrl = node_android.Attributes["AndroidUrl"].Value;
                string androidUpdataRemark = node_android.Attributes["AndroidUpdataRemark"].Value;

                appversion["AndroidVersion"] = androidVersion;
                appversion["AndroidUrl"] = androidUrl;
                appversion["AndroidUpdataRemark"] = androidUpdataRemark;
            }
            return SiteHelper.GetJsonFromHashTable(appversion, "success", "获取数据成功", "AppVersion");
        }
    }
}