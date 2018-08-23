using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using YR.Busines.IDAO;
using YR.Busines.DAL;
using System.Xml;
using YR.Common.DotNetUI;
using YR.Common.DotNetConfig;
using YR.Web.App_Code;

namespace YR.Web.YRBase.SysConfig
{
    public partial class Config_Form : PageBase
    {
        public StringBuilder str_OutputHtml = new StringBuilder();//附加信息
        string Property_Function = "系统配置信息";
        YR_System_IDAO systemidao = new YR_System_Dal();
        protected void Page_Load(object sender, EventArgs e)
        {
            str_OutputHtml.Append(systemidao.AppendProperty_Html(Property_Function));
            GetValue();
        }
        public void GetValue()
        {
            string returnValue = "";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Server.MapPath("/App_Code/Config.xml"));
            XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//appSettings/add");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                returnValue += xmlNode.Attributes["key"].Value + "∫" + xmlNode.Attributes["value"].Value + "∮";
            }
            AppendProperty_value.Value = returnValue;
            //XmlDocument webconfigDoc = new XmlDocument();
            //string filePath = Server.MapPath("/App_Code/Config.xml");
            //string xPath = "/appSettings/add[@key='?']";
            //webconfigDoc.Load(filePath);
            //XmlNode passkey = webconfigDoc.SelectSingleNode(xPath.Replace("?", "Version"));
            //passkey.Attributes["value"].InnerText = "5555555555";
            //webconfigDoc.Save(filePath);
        }
    }
}