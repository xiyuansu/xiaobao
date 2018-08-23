using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Asiasofti.SmartVehicle.Manager;
using System.Collections;
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;

namespace YR.Web.AppPage
{
    public partial class BaseInfo : System.Web.UI.Page
    {
        private string id;

        protected void Page_Load(object sender, EventArgs e)
        {
            id=Request["id"];
            InitPage();
        }

        private void InitPage()
        {
            ArticleManager articleManager = new ArticleManager();
            Hashtable article = articleManager.GetArticleInfoByID(id);
            if (article != null && article.Count > 0)
            {
                divTitle.InnerHtml = article["ARTICLENAME"].ToString();
                DateTime dt = DateTime.Parse(SiteHelper.GetHashTableValueByKey(article, "ReleaseTime"));
                divDate.InnerHtml = dt.ToString("yyyy-MM-dd HH:mm");
                divContent.InnerHtml = article["ARTICLECONTENT"].ToString();
            }
        }
    }
}