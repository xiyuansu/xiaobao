using Asiasofti.SmartVehicle.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;
using YR.Web.api.api_class;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 获取app启动图片
    /// </summary>
    public class AppStratingImages : IApiAction2
    {
        
        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            string mark = "0";
            string logo_path =HttpContext.Current.Server.MapPath("/api/app/logo.png");
            if (System.IO.File.Exists(logo_path))
            {
                System.IO.FileInfo file_info = new System.IO.FileInfo(logo_path);
                mark = file_info.CreationTime.ToString("yyyyMMddHHmmss");
            }
            Hashtable startingimage = new Hashtable();
            startingimage["MARK"] = mark;
            startingimage["URL"] = string.Format("http://{0}/api/app/logo.png?mark={1}", SiteHelper.GetWebRoot(), mark);
            return SiteHelper.GetJsonFromHashTable(startingimage, "success", "获取数据成功", "StartImage");
        }
    }
}