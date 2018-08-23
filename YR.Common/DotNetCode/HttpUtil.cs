using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace YR.Common.DotNetCode
{
    public class HttpUtil
    {
        public static string Get(string url, string postDataStr,string codename)
        {
            Encoding encoding = Encoding.GetEncoding(codename);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public static string Post(string url, string postDataStr, string codename)
        {
            Encoding encoding = Encoding.GetEncoding(codename);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] bs= encoding.GetBytes(postDataStr);
            request.ContentLength = bs.Length;
            Stream myRequestStream = request.GetRequestStream();
            myRequestStream.Write(bs, 0, bs.Length);
            myRequestStream.Flush();
            myRequestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

    }
}