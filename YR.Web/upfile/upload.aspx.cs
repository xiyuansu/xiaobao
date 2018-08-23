using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Collections;
using Asiasofti.SmartVehicle.Manager;
using YR.Common.DotNetCode;
using YR.Common.DotNetBean;
using Asiasofti.SmartVehicle.Common;

namespace YR.Web.upfile
{
    public partial class upload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
            {
                if (RequestSession.GetSessionUser().UserName!=null)
                {
                    if (this.fileUpload.PostedFile != null && this.fileUpload.PostedFile.ContentLength > 0)
                    {
                        //按日期归类保存
                        string virtual_path = SiteHelper.GetAppsetString("UploadImagePath") + DateTime.Now.ToString("yyyyMMdd")+"/";
                        string path = Server.MapPath(virtual_path);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string fileName = Path.GetFileName(this.fileUpload.PostedFile.FileName);
                        string oldFileName = fileName;
                        fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + fileName;

                        string fileid = CommonHelper.GetGuid;
                        
                        int bufferSize = 1;
                        byte[] buffer = new byte[bufferSize];

                        //string filedataid = "";
                        string imgurl = Path.Combine(path, fileName);

                        int uploadedLength = 0;
                        int contentLength = this.fileUpload.PostedFile.ContentLength;
                        using (FileStream fs = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                        {
                            while (uploadedLength < contentLength)
                            {
                                int bytes = this.fileUpload.PostedFile.InputStream.Read(buffer, 0, bufferSize);
                                fs.Write(buffer, 0, bytes);
                                uploadedLength += bytes;
                            }
                        }

                        #region 入库
                        try
                        {
                            Hashtable ht = new Hashtable();
                            ht["ID"] = fileid;
                            ht["ImageName"] = fileName;
                            ht["RelationID"] = Request["relationID"].ToString();
                            ht["Category"] = Request["Category"].ToString();
                            ht["ImageUrl"] = virtual_path + fileName;
                            ht["Type"] = Request["Type"].ToString();
                            ht["CreateTime"] = DateTime.Now;
                            imgurl = ht["ImageUrl"].ToString();
                            SystemImagesManager imagesManager = new SystemImagesManager();
                            imagesManager.AddOrEditImageInfo(ht, string.Empty);
                        }
                        catch (Exception ex)
                        {
                            fileid = ex.Message.ToString();
                        }
                        #endregion
                        
                        const string js = "window.parent.onComplete('success', '上传成功,请继续上传或关闭 {0} ','{1}','{2}','{3}');";
                        //string filePath = Path.Combine(path, fileName);
                        //string view = "<a href=\'" + filePath + "\' target=\'_blank\'>预览</a>";
                        ScriptManager.RegisterStartupScript(this, typeof(upload), "progress", string.Format(js, "", fileid, imgurl, oldFileName), true);// "" fileName
                    }
                    else
                    {
                        const string js = "window.parent.onComplete('error', '上传文件出错','');";
                        ScriptManager.RegisterStartupScript(this, typeof(upload), "progress", js, true);
                    }
                }
                else
                {
                    const string js = "window.parent.onComplete('error', '权限不足','');";
                    ScriptManager.RegisterStartupScript(this, typeof(upload), "progress", js, true);
                }
            }
        }
    }
}