/*
 * 维修站数据操作类
 * 作者：SJ
 * 时间：2015-05-10
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YR.Common;
using YR.DataBase;
using YR.Busines;
using System.Collections;
using YR.Common.DotNetCode;
using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 用户意见反馈管理 
    /// </summary>
    public class FeedbackManager
    {
        /// <summary>
        /// 添加用户反馈记录
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public bool AddFeedback(Hashtable ht)
        {
            ht["CreateTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
            return DataFactory.SqlDataBase().InsertByHashtable("YR_AppFeedback", ht) > 0 ? true : false;
        }
    }
}
