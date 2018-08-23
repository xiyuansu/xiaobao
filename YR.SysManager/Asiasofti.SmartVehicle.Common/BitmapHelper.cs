using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

namespace Asiasofti.SmartVehicle.Common
{
    public class BitmapHelper
    {
        /// <summary>
        /// 根据二进制流得到图片
        /// </summary>
        /// <param name="streamByte"></param>
        /// <returns></returns>
        public static Image ReturnPhoto(byte[] streamByte)
        {
            MemoryStream ms = new MemoryStream(streamByte);
            Image img = Image.FromStream(ms);
            return img;
        }
        /// <summary>
        /// 将图片转换成二进制流
        /// </summary>
        /// <param name="imgPhoto"></param>
        /// <returns></returns>
        public static byte[] PhotoImageInsert(Image imgPhoto)
        {
            //将Image转换成流数据，并保存为byte[] 
            MemoryStream mstream = new MemoryStream();
            imgPhoto.Save(mstream, ImageFormat.Bmp);
            byte[] byData = new Byte[mstream.Length];
            mstream.Position = 0;
            mstream.Read(byData, 0, byData.Length);
            mstream.Close();
            return byData;
        }
        /// <summary>
        /// 将图片转换成二进制流
        /// </summary>
        /// <param name="imagepath"></param>
        /// <returns></returns>
        public static byte[] GetPictureData(string imagepath)
        {
            /**/
            ////根据图片文件的路径使用文件流打开，并保存为byte[] 
            FileStream fs = new FileStream(imagepath, FileMode.Open);//可以是其他重载方法 
            byte[] byData = new byte[fs.Length];
            fs.Read(byData, 0, byData.Length);
            fs.Close();
            return byData;
        }
        /// <summary>
        /// 二进制转图片
        /// </summary>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public static Bitmap BytesToBitmap(byte[] Bytes)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(Bytes);
                return new Bitmap((Image)new Bitmap(stream));
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
            }
        }
        /// <summary>
        /// 将图片转换为二进制
        /// </summary>
        /// <param name="Bitmap"></param>
        /// <returns></returns>
        public static byte[] BitmapToBytes(Bitmap Bitmap)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();
                Bitmap.Save(ms, Bitmap.RawFormat);
                byte[] byteImage = new Byte[ms.Length];
                byteImage = ms.ToArray(); return byteImage;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
            }
        }
        /// <summary>
        /// 上传base64编码图片到服务器
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string toUpload(string fileBase64)
        {
            string virtual_path = SiteHelper.GetAppsetString("UploadImagePath") + DateTime.Now.ToString("yyyyMMdd")+"/";
            string path = HttpContext.Current.Server.MapPath(virtual_path);
            string fileName = string.Format("{0:yyyyMMddHHmmssffff}", DateTime.Now) + ".jpg";
            byte[] image = Convert.FromBase64String(fileBase64.Trim());
            Stream stream = new MemoryStream(image);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fullpath = path + fileName;
            new Bitmap((Image)new Bitmap(stream)).Save(fullpath);
            return virtual_path + fileName;


        }
    }
}
