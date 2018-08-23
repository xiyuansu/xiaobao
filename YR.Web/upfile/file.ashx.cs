using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Asiasofti.SmartVehicle.Manager;

namespace Founder.PKURG.Portal.Web.upfile
{
    /// <summary>
    /// file 的摘要说明
    /// </summary>
    public class file : IHttpHandler
    {
        SystemImagesManager imageManager = new SystemImagesManager();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string msg = "";
            string action = context.Request["action"];
            string fileid = context.Request["fileid"];
            if (action == "delfile")
            {
                if (DelFile(fileid)>0)
                {
                    msg = "1,删除成功";
                }
                else
                {
                    msg = "0,删除失败";
                }


            }
            if (action == "getfile")
            {
                //msg = fdh.getImgJson(fileid);
            }
            context.Response.Write(msg);
        }

        public int DelFile(string id)
        {
            try
            {
                return imageManager.DelImageInfoByID(id);
            }
            catch { return 0; }
        }



        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}