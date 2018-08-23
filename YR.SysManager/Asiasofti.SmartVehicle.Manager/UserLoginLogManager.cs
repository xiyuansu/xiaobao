using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using YR.Busines;
using YR.DataBase;
using Asiasofti.SmartVehicle.Common;
using YR.Common.DotNetCode;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 用户登录日志管理
    /// </summary>
    public class UserLoginLogManager
    {
        /// <summary>
        /// 添加会员登录记录
        /// </summary>
        /// <param name="userLoginLog"></param>
        /// <returns></returns>
        public int AddUserLoginLog(Hashtable userLoginLog)
        {
            userLoginLog["ID"] = CommonHelper.GetGuid;
            userLoginLog["LoginTime"] = SiteHelper.GetWebServerCurrentTime();
            return DataFactory.SqlDataBase().InsertByHashtable("YR_UserLoignLog", userLoginLog);
        }
    }
}
