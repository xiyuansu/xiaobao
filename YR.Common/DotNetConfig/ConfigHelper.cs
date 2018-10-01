using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;
using System.IO;
using System.Configuration;
using YR.Common.DotNetLog;

namespace YR.Common.DotNetConfig
{
    /// <summary>
    ///  Config 公共帮助类
    /// </summary>
    public class ConfigHelper
    {
        private static Log Logger = LogFactory.GetLogger(typeof(ConfigHelper));
        /// <summary>
        /// 获得配置文件appSettings子节点值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        /*public static string GetAppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key].ToString().Trim();
        }*/

        /// <summary>
        /// 根据访问域名获取配置文件appSettings子节点值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSettings(string key)
        {
            string path ="";
            string host = "localhost";
            if (HttpContext.Current != null)
                host = HttpContext.Current.Request.Url.Host;
            path = GetConfigFile(host);
            string value = "";
            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            xml.Load(path);
            XmlNode node = xml.SelectSingleNode(string.Format("/appSettings/add[@key='{0}']", key));
            if (node != null)
            {
                value = node.Attributes["value"].Value;
            }
            //Logger.Debug("host:" + host + ",path:" + path+",key:"+key+",value:"+value);
            return value;
        }

        public static string GetAppSettings(string key,string host)
        {
            string path = "";
            path = GetConfigFile(host);

            string value = "";
            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            xml.Load(path);
            XmlNode node = xml.SelectSingleNode(string.Format("/appSettings/add[@key='{0}']", key));
            if (node != null)
            {
                value = node.Attributes["value"].Value;
            }
            return value;
        }

        /// <summary>
        /// 修改配置文件
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <param name="selectPath"></param>
        /// <param name="key"></param>
        /// <param name="keyValue"></param>
        public static void SetValue(XmlDocument xmlDocument, string selectPath, string key, string keyValue)
        {
            XmlNodeList xmlNodeList = xmlDocument.SelectNodes(selectPath);
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                if (xmlNode.Attributes["key"].Value.ToUpper().Equals(key.ToUpper()))
                {
                    xmlNode.Attributes["value"].Value = keyValue;
                    break;
                }
            }
        }

        /// <summary>
        /// 根据域名获取配置文件路径
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private static string GetConfigFile(string entry)
        {
            string configFile = "";

            string xmlfile = ConfigurationManager.AppSettings["SaaSConfig"].ToString().Trim();
            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            xml.Load(xmlfile);

            XmlNode node = xml.SelectSingleNode(string.Format("/list/item[@entry='{0}']",entry));
            if(node!=null)
            {
                configFile = node.Attributes["config"].Value;
            }

            return configFile;
        }

        /// <summary>
        /// 获取所有Saas列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSaasList()
        {
            List<string> list = new List<string>();

            string xmlfile = ConfigurationManager.AppSettings["SaaSConfig"].ToString().Trim();
            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            xml.Load(xmlfile);

            XmlNodeList nodeList = xml.SelectNodes("/list/item");
            foreach(XmlNode item in nodeList)
            {
                string entry = item.Attributes["entry"].Value;
                list.Add(entry);
            }

            return list;
        }
    }
}
