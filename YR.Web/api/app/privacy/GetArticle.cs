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

namespace YR.Web.api.app.privacy
{
    // <summary>
    /// 根据标识获取文章
    /// </summary>
    public class GetArticle : IApiAction2
    {
        private string articleMark = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["Mark"] == null || res["Mark"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                ArticleManager am = new ArticleManager();
                articleMark = res["Mark"].ToString().Trim();
                Hashtable article = am.GetArticleUrlByArticleMark(articleMark);
                if (article == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "获取数据失败");
                }
                else
                {
                    Hashtable result = new Hashtable();
                    result["url"] = string.Format("{0}/AppPage/BaseInfo.aspx?id={1}", SiteHelper.GetWebRoot(), SiteHelper.GetHashTableValueByKey(article, "id"));
                    return SiteHelper.GetJsonFromHashTable(result, "success", "获取数据成功");
                }
            }
        }
    }
}