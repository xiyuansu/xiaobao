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
    /// App启动页图片
    /// </summary>
    public class AppStratingImages : IApiAction2
    {
        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            string mark = "0";
            string logo_path = HttpContext.Current.Server.MapPath("/api/operations/logo.png");
            if (System.IO.File.Exists(logo_path))
            {
                System.IO.FileInfo file_info = new System.IO.FileInfo(logo_path);
                mark = file_info.CreationTime.ToString("yyyyMMddHHmmss");
            }

            Hashtable startingimage = new Hashtable();
            startingimage["MARK"] = mark;
            startingimage["URL"] = string.Format("{0}/api/operations/logo.png?mark={1}",SiteHelper.GetFileServer(), mark);
            return SiteHelper.GetJsonFromHashTable(startingimage, "success", "获取数据成功", "StartImage");
        }
    }
}