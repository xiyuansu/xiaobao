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
using YR.Common.DotNetLog;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 平台注册会员管理 
    /// </summary>
    public class UserInfoManager
    {
        private static Log Logger = LogFactory.GetLogger(typeof(UserInfoManager));
        /// <summary>
        /// 通过绑定手机获取会员记录
        /// </summary>
        /// <param name="bindPhone"></param>
        /// <returns></returns>
        public DataTable GetUserInfoByBindPhone(string bindPhone)
        {
            //如果客户端需要更多的数据可以在此扩展
            StringBuilder str = new StringBuilder();
            str.Append(@"select ID, NickName,'" + SiteHelper.GetFileServer() + "'+ HeadPortrait as HeadPortrait,RealName,BindPhone,'" + SiteHelper.GetFileServer() + "'+IDCard as IDCard,IDCardNum,RealNameCertification,Balance,Balance2,Deposit,RegistrionTime,LastloginTime,UserState,UserType from yr_userinfo where bindphone='" + bindPhone + "'");
            return DataFactory.SqlDataBase().GetDataTableBySQL(str);
        }

        /// <summary>
        /// 查询手机号是否在黑名单
        /// </summary>
        /// <param name="bindPhone"></param>
        /// <returns></returns>
        public DataTable GetBlackMobileByMobile(string bindPhone)
        {
            StringBuilder str = new StringBuilder();
            str.Append(@"select Mobile from YR_BlackMobile where Mobile='" + bindPhone + "'");
            return DataFactory.SqlDataBase().GetDataTableBySQL(str);
        }

        /// <summary>
        /// 通过用户ID获取用户信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Hashtable GetUserInfoByUserID(string userID)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_UserInfo", "ID", userID);
        }
        /// <summary>
        /// 根据用户id返回用户登录时的设备id
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetDeviceIDByUserID(string userID)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select top 1 DeviceID from yr_userinfo where ID='" + userID + "'");
            return DataFactory.SqlDataBase().GetSingleValueBySQL(sql);
        }
        /// <summary>
        /// 跟用户id获取用户信息返回DataTable
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public DataTable GetUserInfoByUid(string uid)
        {
            StringBuilder str = new StringBuilder();
            str.Append(@"select ID, NickName,'" + SiteHelper.GetFileServer() + "'+ HeadPortrait as HeadPortrait,RealName,BindPhone,'" + SiteHelper.GetFileServer() + "'+IDCard as IDCard,IDCardNum,RealNameCertification,Balance,Balance2,Deposit,RegistrionTime,LastloginTime,UserState,UserType from yr_userinfo where ID='" + uid + "'");
            return DataFactory.SqlDataBase().GetDataTableBySQL(str);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="ht"></param>
        public int AddUserInfo(Hashtable ht)
        {
            ht["ID"] = CommonHelper.GetGuid;
            ht["RegistrionTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
            ht["RealNameCertification"] = (int)UserRealNameCertification.Unauthorized;
            ht["UserState"] = (int)UserState.Enable;
            ht["Balance"] = 0;
            ht["Balance2"] = 0;
            ht["Deposit"] = 0;
            ht["RaiseBalance"] = 0;
            return DataFactory.SqlDataBase().InsertByHashtable("YR_UserInfo", ht);
        }

        /// <summary>
        /// 添加或修改用户信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditUserInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_UserInfo", "ID", _key, ht);
        }

        /// <summary>
        /// 更新指定用户为实名认证状态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="realName"></param>
        /// <param name="idCard"></param>
        /// <param name="invitationCode"></param>
        /// <returns></returns>
        public bool UpdateRealNameCertification(string userID, string realName, string idCard)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE YR_UserInfo SET RealName='" + realName + "',RealNameCertification=4,IDCardNum='" + idCard + "' WHERE RealNameCertification!=4 and ID='" + userID + "'");
            int iNum = DataFactory.SqlDataBase().ExecuteBySql(sb);
            return iNum > 0 ? true : false;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public DataTable UserLogin(string mobile)
        {
            DataTable dtUser = GetUserInfoByBindPhone(mobile);
            if (dtUser == null)
            {
                Hashtable ht = new Hashtable();
                ht["BindPhone"] = mobile;
                AddUserInfo(ht);
                dtUser = GetUserInfoByBindPhone(mobile);
            }
            return dtUser;
        }

        /// <summary>
        /// 用户列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetUserInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from YR_UserInfo where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "RegistrionTime", "Desc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllUser()
        {
            return DataFactory.SqlDataBase().GetDataTable("YR_UserInfo");
        }

        /// <summary>
        /// 获取用户类型列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserTypeList()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select ID,Name from YR_UserType");
            return DataFactory.SqlDataBase().GetDataTableBySQL(sb);
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserListByCondition(string condition = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select *,CASE WHEN isnull(RealName,'')='' THEN BindPhone ELSE BindPhone+'('+RealName+')' END FullName from YR_UserInfo where UserState=1" + condition);
            return DataFactory.SqlDataBase().GetDataTableBySQL(sb);
        }

        #region 用户挂失解挂操作

        public DataTable GetReportLossInfoByID(string reportLossID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select a.BindPhone,b.NoReportLossRemark,b.NoReportLossTime from YR_UserInfo a inner join YR_UserReportLossLog b on a.ID=b.UserID where b.ID='" + reportLossID + "'");
            return DataFactory.SqlDataBase().GetDataTableBySQL(sb);
        }


        public bool UserNoReportLoss(Hashtable ht)
        {
            try
            {
                StringBuilder[] sqls = new StringBuilder[2];
                object[] objs = new object[2];

                // 更新用户状态
                StringBuilder sbUpdateUserInfo = new StringBuilder();
                sbUpdateUserInfo.Append("update YR_UserInfo set UserState=@UserState,BindPhone=@BindPhone Where ID in (select UserID from YR_UserReportLossLog where ID=@ReportLossID)");
                SqlParam[] parm = new SqlParam[] { new SqlParam("@UserState", UserState.Enable.GetHashCode()), new SqlParam("@BindPhone", ht["BindPhone"]), new SqlParam("@ReportLossID", ht["ID"]) };
                sqls[0] = sbUpdateUserInfo;
                objs[0] = parm;

                // 更新挂失记录状态
                StringBuilder sbadd = new StringBuilder();
                sbadd.Append("update YR_UserReportLossLog set State=@State where ID=@ReportLossID");
                SqlParam[] parmAdd = new SqlParam[] { new SqlParam("@State", ReportLossState.NoReportLoss.GetHashCode()), new SqlParam("@ReportLossID", ht["ID"]) };
                sqls[1] = sbadd;
                objs[1] = parmAdd;
                return DataFactory.SqlDataBase().BatchExecuteBySql(sqls, objs) >= 0 ? true : false;
            }
            catch
            {
                return false;
            }
        }

        public bool UserReportLoss(string userID, int state, string remark)
        {
            try
            {
                StringBuilder[] sqls = new StringBuilder[2];
                object[] objs = new object[2];
                // 更新用户状态
                StringBuilder sbUpdateUserInfo = new StringBuilder();
                sbUpdateUserInfo.Append("update YR_UserInfo set UserState=@UserState Where ID =@UserID");
                SqlParam[] parm = new SqlParam[] { new SqlParam("@UserState", state), new SqlParam("@UserID", userID) };
                sqls[0] = sbUpdateUserInfo;
                objs[0] = parm;
                // 添加用户挂失记录
                StringBuilder sbadd = new StringBuilder();
                sbadd.Append("Insert into YR_UserReportLossLog(");
                sbadd.Append("ID,UserID,Remark,State,ReportLossTime");
                sbadd.Append(")Values(");
                sbadd.Append("@ID,@ReportLossUserID,@Remark,@State,@ReportLossTime)");
                SqlParam[] parmAdd = new SqlParam[] { 
                                     new SqlParam("@ID", CommonHelper.GetGuid),
                                     new SqlParam("@ReportLossUserID", userID),
                                     new SqlParam("@Remark", remark),
                                     new SqlParam("@State", state),
                                     new SqlParam("@ReportLossTime", DateTime.Now)};
                sqls[1] = sbadd;
                objs[1] = parmAdd;
                return DataFactory.SqlDataBase().BatchExecuteBySql(sqls, objs) >= 0 ? true : false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 用户挂失记录列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetUserReportLossInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select a.*,b.NickName,b.RealName,b.BindPhone from YR_UserReportLossLog a inner join YR_UserInfo b on a.UserID=b.ID where a.State=" + ReportLossState.ReportLoss.GetHashCode());
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "ReportLossTime", "Desc", pageIndex, pageSize, ref count);
        }

        #endregion

        #region 用户余额更新

        /// <summary>
        /// 更新用户主帐户余额
        /// </summary>
        /// <returns></returns>
        public bool UpdateUserBalance(Hashtable ht)
        {
            try
            {
                if (ht != null)
                {
                    List<StringBuilder> sqls = new List<StringBuilder>();
                    List<object[]> objs = new List<object[]>();

                    Hashtable userinfo = GetUserInfoByUserID(SiteHelper.GetHashTableValueByKey(ht, "UserID"));
                    decimal balance = 0.00m, balance2 = 0.00m, raiseBalance = 0.00m;
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance"), out balance);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance2"), out balance2);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "RaiseBalance"), out raiseBalance);

                    // 更新用户余额
                    decimal changesAmount = 0.00m,oprMoney=0.00m;
                    decimal.TryParse(ht["ChangesAmount"].ToString(),out changesAmount);
                    decimal.TryParse(ht["OprMoney"].ToString(), out oprMoney);
                    StringBuilder sbUpdateUserInfo = new StringBuilder();
                    if (ht["ChangesType"].ToString() == UserFinancialChangesType.PlatMinus.GetHashCode().ToString())//平台减少
                    {
                        sbUpdateUserInfo.Append("update YR_UserInfo set Balance=Balance-@ChangesAmount Where ID =@UserID");
                        SqlParam[] parm = new SqlParam[] { new SqlParam("@ChangesAmount", oprMoney), new SqlParam("@UserID", ht["UserID"]) };
                        sqls.Add(sbUpdateUserInfo);
                        objs.Add(parm);
                    }
                    else//平台增加
                    {
                        sbUpdateUserInfo.Append("update YR_UserInfo set Balance=Balance+@ChangesAmount Where ID =@UserID");
                        SqlParam[] parm = new SqlParam[] { new SqlParam("@ChangesAmount", oprMoney), new SqlParam("@UserID", ht["UserID"]) };
                        sqls.Add(sbUpdateUserInfo);
                        objs.Add(parm);
                    }

                    // 增加财务信息
                    StringBuilder balanceAdd = new StringBuilder();
                    balanceAdd.Append("Insert into YR_UserFinancial(ID,UserID,ChangesAmount,ChangesTime,ChangesType,OrderNum,Remark,Operator,OperatorType,OperatorWay,CurrentBalance,state)");
                    balanceAdd.Append("Values(@ID,@UserID,@ChangesAmount,@ChangesTime,@ChangesType,@OrderNum,@Remark,@Operator,@OperatorType,@OperatorWay,@CurrentBalance,@State)");
                    SqlParam[] parmBalanceAdd = new SqlParam[] { 
                                     new SqlParam("@ID", ht["ID"]),
                                     new SqlParam("@UserID", ht["UserID"]),
                                     new SqlParam("@ChangesAmount", ht["ChangesAmount"]),
                                     new SqlParam("@ChangesTime", ht["ChangesTime"]),
                                     new SqlParam("@ChangesType", ht["ChangesType"]),
                                     new SqlParam("@OrderNum", ht["OrderNum"]),
                                     new SqlParam("@Remark", ht["Remark"]),
                                     new SqlParam("@Operator", ht["Operator"]),
                                     new SqlParam("@OperatorType", ht["OperatorType"]),
                                     new SqlParam("@OperatorWay",ht["OperatorWay"]),
                                     new SqlParam("@CurrentBalance",balance+balance2+changesAmount),
                                     new SqlParam("@State", ht["State"])};
                    sqls.Add(balanceAdd);
                    objs.Add(parmBalanceAdd);

                    // 发送短消息
                    if (ht["ChangesType"].ToString() == UserFinancialChangesType.PlatAdd.GetHashCode().ToString() ||
                        ht["ChangesType"].ToString() == UserFinancialChangesType.PlatMinus.GetHashCode().ToString())
                    {
                        StringBuilder msgAdd = new StringBuilder();
                        msgAdd.Append("Insert into YR_Messages(");
                        msgAdd.Append("ID,OperatorID,UserID,MessageTitle,MessageContent,MessageType,ModuleType,ReleaseTime,state) ");
                        msgAdd.Append(" values(@ID,@OperatorID,@UserID,@MessageTitle,@MessageContent,@MessageType,@ModuleType,@ReleaseTime,@State)");
                        string remark = ht["ChangesType"].ToString() == UserFinancialChangesType.PlatAdd.GetHashCode().ToString() ? "增加了" : "扣除了";
                        SqlParam[] parmMsgAdd = new SqlParam[] { 
                                     new SqlParam("@ID", ht["ID"]),
                                     new SqlParam("@OperatorID", ht["Operator"]),
                                     new SqlParam("@UserID", ht["UserID"]),
                                     new SqlParam("@MessageTitle", "平台操作消息"),
                                     new SqlParam("@MessageContent", "平台为你"+remark+ht["OprMoney"].ToString()+"元"),
                                     new SqlParam("@MessageType", MessageType.System.GetHashCode()),
                                     new SqlParam("@ModuleType", MessageModuleType.UseVehicle.GetHashCode()),
                                     new SqlParam("@ReleaseTime",SiteHelper.GetWebServerCurrentTime()),
                                     new SqlParam("@State", MessageState.Enable.GetHashCode())};
                        sqls.Add(msgAdd);
                        objs.Add(parmMsgAdd);
                    }
                    return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 更新用户副帐户余额
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public bool UpdateUserBalance2(Hashtable ht)
        {
            try
            {
                if (ht != null)
                {
                    List<StringBuilder> sqls = new List<StringBuilder>();
                    List<object[]> objs = new List<object[]>();

                    Hashtable userinfo = GetUserInfoByUserID(SiteHelper.GetHashTableValueByKey(ht, "UserID"));
                    decimal balance = 0.00m, balance2 = 0.00m, raiseBalance = 0.00m;
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance"), out balance);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance2"), out balance2);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "RaiseBalance"), out raiseBalance);

                    // 更新用户余额
                    decimal changesAmount = 0.00m, oprMoney = 0.00m;
                    decimal.TryParse(ht["ChangesAmount"].ToString(), out changesAmount);
                    decimal.TryParse(ht["OprMoney"].ToString(), out oprMoney);
                    StringBuilder sbUpdateUserInfo = new StringBuilder();
                    if (ht["ChangesType"].ToString() == UserFinancialChangesType.PlatMinus.GetHashCode().ToString())//平台减少
                    {
                        sbUpdateUserInfo.Append("update YR_UserInfo set Balance2=Balance2-@ChangesAmount Where ID =@UserID");
                        SqlParam[] parm = new SqlParam[] { new SqlParam("@ChangesAmount", oprMoney), new SqlParam("@UserID", ht["UserID"]) };
                        sqls.Add(sbUpdateUserInfo);
                        objs.Add(parm);
                    }
                    else//平台增加
                    {
                        sbUpdateUserInfo.Append("update YR_UserInfo set Balance2=Balance2+@ChangesAmount Where ID =@UserID");
                        SqlParam[] parm = new SqlParam[] { new SqlParam("@ChangesAmount", oprMoney), new SqlParam("@UserID", ht["UserID"]) };
                        sqls.Add(sbUpdateUserInfo);
                        objs.Add(parm);
                    }

                    // 增加财务信息
                    StringBuilder balanceAdd = new StringBuilder();
                    balanceAdd.Append("Insert into YR_UserFinancial(ID,UserID,ChangesAmount,ChangesTime,ChangesType,OrderNum,Remark,Operator,OperatorType,OperatorWay,CurrentBalance,state)");
                    balanceAdd.Append("Values(@ID,@UserID,@ChangesAmount,@ChangesTime,@ChangesType,@OrderNum,@Remark,@Operator,@OperatorType,@OperatorWay,@CurrentBalance,@State)");
                    SqlParam[] parmBalanceAdd = new SqlParam[] { 
                                     new SqlParam("@ID", ht["ID"]),
                                     new SqlParam("@UserID", ht["UserID"]),
                                     new SqlParam("@ChangesAmount", ht["ChangesAmount"]),
                                     new SqlParam("@ChangesTime", ht["ChangesTime"]),
                                     new SqlParam("@ChangesType", ht["ChangesType"]),
                                     new SqlParam("@OrderNum", ht["OrderNum"]),
                                     new SqlParam("@Remark", ht["Remark"]),
                                     new SqlParam("@Operator", ht["Operator"]),
                                     new SqlParam("@OperatorType", ht["OperatorType"]),
                                     new SqlParam("@OperatorWay",ht["OperatorWay"]),
                                     new SqlParam("@CurrentBalance",balance+balance2+changesAmount),
                                     new SqlParam("@State", ht["State"])};
                    sqls.Add(balanceAdd);
                    objs.Add(parmBalanceAdd);

                    // 发送短消息
                    if (ht["ChangesType"].ToString() == UserFinancialChangesType.PlatAdd.GetHashCode().ToString() ||
                        ht["ChangesType"].ToString() == UserFinancialChangesType.PlatMinus.GetHashCode().ToString())
                    {
                        StringBuilder msgAdd = new StringBuilder();
                        msgAdd.Append("Insert into YR_Messages(");
                        msgAdd.Append("ID,OperatorID,UserID,MessageTitle,MessageContent,MessageType,ModuleType,ReleaseTime,state) ");
                        msgAdd.Append(" values(@ID,@OperatorID,@UserID,@MessageTitle,@MessageContent,@MessageType,@ModuleType,@ReleaseTime,@State)");
                        string remark = ht["ChangesType"].ToString() == UserFinancialChangesType.PlatAdd.GetHashCode().ToString() ? "增加了" : "扣除了";
                        SqlParam[] parmMsgAdd = new SqlParam[] { 
                                     new SqlParam("@ID", ht["ID"]),
                                     new SqlParam("@OperatorID", ht["Operator"]),
                                     new SqlParam("@UserID", ht["UserID"]),
                                     new SqlParam("@MessageTitle", "平台操作消息"),
                                     new SqlParam("@MessageContent", "平台为你"+remark+ht["OprMoney"].ToString()+"元"),
                                     new SqlParam("@MessageType", MessageType.System.GetHashCode()),
                                     new SqlParam("@ModuleType", MessageModuleType.UseVehicle.GetHashCode()),
                                     new SqlParam("@ReleaseTime",SiteHelper.GetWebServerCurrentTime()),
                                     new SqlParam("@State", MessageState.Enable.GetHashCode())};
                        sqls.Add(msgAdd);
                        objs.Add(parmMsgAdd);
                    }
                    return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 更新众筹余额
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public bool UpdateUserRaiseBalance(string userID)
        {
            try
            {
                if (!string.IsNullOrEmpty(userID))
                {
                    StringBuilder[] sqls = new StringBuilder[1];
                    object[] objs = new object[1];

                    // 更新用户余额
                    StringBuilder sbUpdateUserInfo = new StringBuilder();
                    sbUpdateUserInfo.Append("update YR_UserInfo set RaiseBalance=(SELECT isnull(SUM(ChangesCoin),0) ");
                    sbUpdateUserInfo.Append(" FROM YR_UserRaiseFinancial WITH(NOLOCK) WHERE YR_UserRaiseFinancial.UserID=@UserID and State=" + UserFinancialState.Effect.GetHashCode() + ") Where ID =@UserID");
                    SqlParam[] parm = new SqlParam[] { new SqlParam("@UserID", userID) };
                    sqls[0] = sbUpdateUserInfo;
                    objs[0] = parm;

                    return DataFactory.SqlDataBase().BatchExecuteBySql(sqls, objs) >= 0 ? true : false;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 第三方支付充值成功回调
        /// </summary>
        /// <returns></returns>
        public bool RechargeCallBack(Hashtable ht)
        {
            try
            {
                if (ht != null)
                {
                    List<StringBuilder> sqls = new List<StringBuilder>();
                    List<object> objs = new List<object>();

                    Hashtable userinfo = GetUserInfoByUserID(SiteHelper.GetHashTableValueByKey(ht, "UserID"));
                    decimal balance = 0.00m, balance2 = 0.00m, raiseBalance = 0.00m;
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance"), out balance);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance2"), out balance2);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "RaiseBalance"), out raiseBalance);

                    string buyer_id = SiteHelper.GetHashTableValueByKey(ht, "buyer_id");
                    string buyer_logon_id = SiteHelper.GetHashTableValueByKey(ht, "buyer_logon_id");

                    UserFinancialOperatorWay payway = (UserFinancialOperatorWay)ht["PayWay"];

                    UserFinancialManager ufm = new UserFinancialManager();
                    Hashtable uf = ufm.GetUserFinancialPayInfoByID(ht["ID"].ToString());
                    decimal money = 0.00m;
                    decimal.TryParse(uf["TOTAL_FEE"].ToString(), out money);

                    //根据充值规则向用户赠送副帐户余额
                    decimal present_money = 0.00m;
                    RechargeRules rules = new RechargeRules();
                    DataTable dt = rules.GetAvailableRules(money);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        string ruleid = dt.Rows[0]["ID"].ToString();
                        if (!string.IsNullOrEmpty(dt.Rows[0]["ChargeMoney"].ToString()))
                        {
                            decimal.TryParse(dt.Rows[0]["PresentMoney"].ToString(), out present_money);
                        }
                        else
                        {
                            int flag = 1;
                            int.TryParse(dt.Rows[0]["PresentedFlag"].ToString(), out flag);
                            decimal presented = 0.00m;
                            decimal.TryParse(dt.Rows[0]["Presented"].ToString(), out presented);
                            if (flag == 2)
                            {
                                presented = money * presented * 0.01m;
                            }
                            present_money = presented;
                        }
                        
                        StringBuilder sbUpdateRaiseBalance = new StringBuilder();
                        sbUpdateRaiseBalance.Append(" update YR_UserInfo set Balance2=Balance2+" + present_money + " Where ID =@UserID");
                        SqlParam[] parmUpdateRaiseBalance = new SqlParam[] { new SqlParam("@UserID", ht["UserID"].ToString()) };
                        sqls.Add(sbUpdateRaiseBalance);
                        objs.Add(parmUpdateRaiseBalance);

                        StringBuilder sbUpdateUserFinancial2 = new StringBuilder();
                        sbUpdateUserFinancial2.Append(" update YR_UserFinancial set RechargeRuleId=@RechargeRuleId,Remark=@Remark Where ID =@ID");
                        SqlParam[] parm2 = new SqlParam[] { new SqlParam("@ID", ht["ID"]), new SqlParam("@RechargeRuleId", ruleid), new SqlParam("@Remark", string.Format("赠送{0}元", present_money.ToString("#0.00"))) };
                        sqls.Add(sbUpdateUserFinancial2);
                        objs.Add(parm2);
                    }

                    //更新财务信息状态
                    StringBuilder sbUpdateUserFinancial = new StringBuilder();
                    if (!string.IsNullOrEmpty(buyer_id) && !string.IsNullOrEmpty(buyer_logon_id))
                    {
                        sbUpdateUserFinancial.Append(" update YR_UserFinancial set TradeNo=@TradeNo, CurrentBalance=" + (balance + balance2 + money + present_money) + ",State=@State,buyer_id=@buyer_id,buyer_logon_id=@buyer_logon_id Where ID =@ID");
                        SqlParam[] parm = new SqlParam[] { new SqlParam("@ID", ht["ID"]), new SqlParam("@TradeNo", ht["TradeNo"]), new SqlParam("@State", ht["State"]), new SqlParam("@buyer_id", buyer_id), new SqlParam("@buyer_logon_id", buyer_logon_id) };
                        sqls.Add(sbUpdateUserFinancial);
                        objs.Add(parm);
                    }
                    else
                    {
                        sbUpdateUserFinancial.Append(" update YR_UserFinancial set TradeNo=@TradeNo, CurrentBalance=" + (balance + balance2 + money + present_money) + ",State=@State Where ID =@ID");
                        SqlParam[] parm = new SqlParam[] { new SqlParam("@ID", ht["ID"]), new SqlParam("@TradeNo", ht["TradeNo"]), new SqlParam("@State", ht["State"]) };
                        sqls.Add(sbUpdateUserFinancial);
                        objs.Add(parm);
                    }

                    //更新用户当前余额
                    StringBuilder sbUpdateUserInfo = new StringBuilder();
                    sbUpdateUserInfo.Append(" update YR_UserInfo set Balance=Balance+" + money + " Where ID =@UserID");
                    SqlParam[] parmUserInfo = new SqlParam[] { new SqlParam("@UserID", ht["UserID"].ToString()) };
                    sqls.Add(sbUpdateUserInfo);
                    objs.Add(parmUserInfo);

                    //发系统充值通知
                    StringBuilder msgAdd = new StringBuilder();
                    msgAdd.Append("Insert into YR_Messages(");
                    msgAdd.Append("ID,OperatorID,UserID,MessageTitle,MessageContent,MessageType,ReleaseTime,state) ");
                    msgAdd.Append(" values(@ID,@OperatorID,@UserID,@MessageTitle,@MessageContent,@MessageType,@ReleaseTime,@State)");
                    string sbMsg = string.Format("您于{0}通过{1}充值{2}元，您当前余额{3}元，祝您用车愉快！", SiteHelper.GetWebServerCurrentTime().ToString("yyyy年MM月dd日HH:mm:ss"), EnumHelper.GetEnumShowName(typeof(UserFinancialOperatorWay), payway.GetHashCode()), money.ToString("#0.00"), (balance+balance2 + money).ToString("#0.00"));
                    if(present_money>0)
                    {
                        sbMsg = string.Format("您于{0}通过{1}充值{2}元并充值赠送{3}元，您当前余额{4}元，祝您用车愉快！", SiteHelper.GetWebServerCurrentTime().ToString("yyyy年MM月dd日HH:mm:ss"), EnumHelper.GetEnumShowName(typeof(UserFinancialOperatorWay), payway.GetHashCode()), money.ToString("#0.00"),present_money.ToString("#0.00"), (balance +balance2+ money+present_money).ToString("#0.00"));
                    }
                    SqlParam[] parmMsgAdd = new SqlParam[] { 
                                     new SqlParam("@ID", CommonHelper.GetGuid),
                                     new SqlParam("@OperatorID", ""),
                                     new SqlParam("@UserID", ht["UserID"]),
                                     new SqlParam("@MessageTitle", "充值提醒"),
                                     new SqlParam("@MessageContent",sbMsg),
                                     new SqlParam("@MessageType", MessageType.System.GetHashCode()),
                                     new SqlParam("@ReleaseTime",SiteHelper.GetWebServerCurrentTime()),
                                     new SqlParam("@State", MessageState.Enable.GetHashCode())};
                    sqls.Add(msgAdd);
                    objs.Add(parmMsgAdd);

                    return DataFactory.SqlDataBase().BatchExecuteBySql(sqls.ToArray(), objs.ToArray()) >= 0 ? true : false;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 押金支付回调处理
        /// </summary>
        /// <returns></returns>
        public bool DepositCallBack(Hashtable ht)
        {
            try
            {
                if (ht != null)
                {
                    List<StringBuilder> sqls = new List<StringBuilder>();
                    List<object[]> objs = new List<object[]>();

                    Hashtable userinfo = GetUserInfoByUserID(SiteHelper.GetHashTableValueByKey(ht, "UserID"));
                    decimal balance = 0.00m, balance2 = 0.00m, deposit = 0.00m;
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance"), out balance);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance2"), out balance2);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Deposit"), out deposit);

                    string buyer_id = SiteHelper.GetHashTableValueByKey(ht, "buyer_id");
                    string buyer_logon_id = SiteHelper.GetHashTableValueByKey(ht, "buyer_logon_id");

                    UserFinancialManager ufm = new UserFinancialManager();
                    Hashtable uf = ufm.GetUserFinancialPayInfoByID(ht["ID"].ToString());
                    decimal money = 0.00m;
                    decimal.TryParse(ht["TotalFee"].ToString(), out money);

                    //更新财务信息状态
                    StringBuilder balanceUpdate = new StringBuilder();
                    if (!string.IsNullOrEmpty(buyer_id) && !string.IsNullOrEmpty(buyer_logon_id))
                    {
                        balanceUpdate.Append("update YR_UserFinancial set TradeNo=@TradeNo,State=@State,buyer_id=@buyer_id,buyer_logon_id=@buyer_logon_id Where ID =@ID");
                        SqlParam[] parmBalance = new SqlParam[] {
                                    new SqlParam("@ID", ht["ID"]),
                                    new SqlParam("@State", ht["State"]),
                                    new SqlParam("@TradeNo", ht["TradeNo"]),
                                    new SqlParam("@buyer_id", buyer_id),
                                    new SqlParam("@buyer_logon_id", buyer_logon_id) };
                        sqls.Add(balanceUpdate);
                        objs.Add(parmBalance);
                    }
                    else
                    {
                        balanceUpdate.Append("update YR_UserFinancial set TradeNo=@TradeNo,State=@State where ID=@ID");
                        SqlParam[] parmBalance = new SqlParam[] {
                                    new SqlParam("@ID", ht["ID"]),
                                    new SqlParam("@State", ht["State"]),
                                    new SqlParam("@TradeNo", ht["TradeNo"])};
                        sqls.Add(balanceUpdate);
                        objs.Add(parmBalance);
                    }

                    StringBuilder sbUpdateUserInfo = new StringBuilder();
                    sbUpdateUserInfo.Append("update YR_UserInfo set Deposit=" + money + " Where ID =@UserID");
                    SqlParam[] parmUserInfo = new SqlParam[] { new SqlParam("@UserID", ht["UserID"].ToString()) };
                    sqls.Add(sbUpdateUserInfo);
                    objs.Add(parmUserInfo);

                    //邀请注册用户赠券
                    string uid = ht["UserID"].ToString();
                    DepositReturnManager depositManager = new DepositReturnManager();
                    bool isFirstDeposit = depositManager.IsFirstDeposit(uid);
                    string invite_uid = SiteHelper.GetHashTableValueByKey(userinfo, "InviteUserID");
                    if (!string.IsNullOrEmpty(invite_uid) && isFirstDeposit)
                    {
                        int couponNum = 0;
                        decimal couponMoney = 0.00m;
                        double CouponPeriod = 0;
                        //邀请注册双方同时赠送优惠券
                        SysSettingManager settingManager = new SysSettingManager();
                        int.TryParse(settingManager.GetValueByKey("InviteCouponNum"), out couponNum);
                        couponMoney = 0.00m;
                        decimal.TryParse(settingManager.GetValueByKey("InviteCouponMoney"), out couponMoney);
                        CouponPeriod = 0;
                        double.TryParse(settingManager.GetValueByKey("CouponPeriod"), out CouponPeriod);
                        if (couponNum > 0 && couponMoney > 0)
                        {
                            for (int i = 1; i <= couponNum; i++)
                            {
                                StringBuilder sbInsertCoupon = new StringBuilder();
                                sbInsertCoupon.Append("insert into YR_UserCoupon(ID,Money,MinMoney,BeginTime,EndTime,UserID,CreateTime)values(@ID,@Money,@MinMoney,@BeginTime,@EndTime,@UserID,@CreateTime)");
                                SqlParam[] parmInsertCoupon = new SqlParam[] { 
                        new SqlParam("@ID", Guid.NewGuid().ToString()), 
                        new SqlParam("@Money", couponMoney), 
                        new SqlParam("@MinMoney", "2.00"), 
                        new SqlParam("@BeginTime", DateTime.Now), 
                        new SqlParam("@EndTime", DateTime.Now.AddDays(CouponPeriod)), 
                        new SqlParam("@UserID", uid), 
                        new SqlParam("@CreateTime", DateTime.Now) };
                                sqls.Add(sbInsertCoupon);
                                objs.Add(parmInsertCoupon);
                            }

                            StringBuilder sbInsertMessage = new StringBuilder();
                            sbInsertMessage.Append("Insert into YR_Messages(");
                            sbInsertMessage.Append("ID,OperatorID,UserID,MessageTitle,MessageContent,MessageType,ReleaseTime,state) ");
                            sbInsertMessage.Append(" values(@ID,@OperatorID,@UserID,@MessageTitle,@MessageContent,@MessageType,@ReleaseTime,@State)");
                            string msg = string.Format("您于{0}获取{1}张{2}元骑行券，祝您用车愉快！", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), couponNum, couponMoney);
                            SqlParam[] parmInsertMessage = new SqlParam[] { 
                                     new SqlParam("@ID", CommonHelper.GetGuid),
                                     new SqlParam("@OperatorID", ""),
                                     new SqlParam("@UserID", uid),
                                     new SqlParam("@MessageTitle", "骑行券提醒"),
                                     new SqlParam("@MessageContent",msg),
                                     new SqlParam("@MessageType", MessageType.System.GetHashCode()),
                                     new SqlParam("@ReleaseTime",SiteHelper.GetWebServerCurrentTime()),
                                     new SqlParam("@State", MessageState.Enable.GetHashCode())};
                            sqls.Add(sbInsertMessage);
                            objs.Add(parmInsertMessage);

                            for (int i = 1; i <= couponNum; i++)
                            {
                                StringBuilder sbInsertCoupon = new StringBuilder();
                                sbInsertCoupon.Append("insert into YR_UserCoupon(ID,Money,MinMoney,BeginTime,EndTime,UserID,CreateTime)values(@ID,@Money,@MinMoney,@BeginTime,@EndTime,@UserID,@CreateTime)");
                                SqlParam[] parmInsertCoupon = new SqlParam[] { 
                        new SqlParam("@ID", Guid.NewGuid().ToString()), 
                        new SqlParam("@Money", couponMoney), 
                        new SqlParam("@MinMoney", "2.00"), 
                        new SqlParam("@BeginTime", DateTime.Now), 
                        new SqlParam("@EndTime", DateTime.Now.AddDays(CouponPeriod)), 
                        new SqlParam("@UserID", invite_uid), 
                        new SqlParam("@CreateTime", DateTime.Now) };
                                sqls.Add(sbInsertCoupon);
                                objs.Add(parmInsertCoupon);
                            }

                            StringBuilder sbInsertMessage2 = new StringBuilder();
                            sbInsertMessage2.Append("Insert into YR_Messages(");
                            sbInsertMessage2.Append("ID,OperatorID,UserID,MessageTitle,MessageContent,MessageType,ReleaseTime,state) ");
                            sbInsertMessage2.Append(" values(@ID,@OperatorID,@UserID,@MessageTitle,@MessageContent,@MessageType,@ReleaseTime,@State)");
                            string msg2 = string.Format("邀请好友注册成功，您于{0}获取{1}张{2}元骑行券，祝您用车愉快！", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), couponNum, couponMoney);
                            SqlParam[] parmInsertMessage2 = new SqlParam[] { 
                                     new SqlParam("@ID", CommonHelper.GetGuid),
                                     new SqlParam("@OperatorID", ""),
                                     new SqlParam("@UserID", invite_uid),
                                     new SqlParam("@MessageTitle", "骑行券提醒"),
                                     new SqlParam("@MessageContent",msg2),
                                     new SqlParam("@MessageType", MessageType.System.GetHashCode()),
                                     new SqlParam("@ReleaseTime",SiteHelper.GetWebServerCurrentTime()),
                                     new SqlParam("@State", MessageState.Enable.GetHashCode())};
                            sqls.Add(sbInsertMessage2);
                            objs.Add(parmInsertMessage2);
                        }
                    }

                    return DataFactory.SqlDataBase().BatchExecuteBySql(sqls.ToArray(), objs.ToArray()) >= 0 ? true : false;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 第三方支付订单成功回调
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public bool OrderPayCallback(Hashtable ht)
        {
            try
            {
                OrdersManager om = new OrdersManager();
                VehicleManager vm = new VehicleManager();
                SysSettingManager settingManager = new SysSettingManager();

                string financialid = ht["ID"].ToString();
                string tradeno = ht["TradeNo"].ToString();
                string ordernum = ht["OrderNum"].ToString();
                decimal paymoney = 0.00m;
                decimal.TryParse(ht["TotalFee"].ToString(), out paymoney);
                Hashtable orderinfo = om.GetOrderInfoByNum(ordernum);
                Hashtable userinfo = GetUserInfoByUserID(SiteHelper.GetHashTableValueByKey(orderinfo, "UserID"));
                Hashtable vehicleinfo = vm.GetVehicleInfoByID(SiteHelper.GetHashTableValueByKey(orderinfo, "VehicleID"));

                decimal balance = 0.00m, balance2 = 0.00m, raiseBalance = 0.00m;
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance"), out balance);
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance2"), out balance2);
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "RaiseBalance"), out raiseBalance);

                string buyer_id = SiteHelper.GetHashTableValueByKey(ht, "buyer_id");
                string buyer_logon_id = SiteHelper.GetHashTableValueByKey(ht, "buyer_logon_id");

                List<StringBuilder> sqls = new List<StringBuilder>();
                List<object> objs = new List<object>();

                //更改订单状态
                StringBuilder sbUpdateOrder = new StringBuilder();
                sbUpdateOrder.Append(@"UPDATE yr_orders SET PayMoney=@PayMoney,PayTime=@PayTime,FinishedTime=@FinishedTime,PayState=@PayState,OrderState=@OrderState where OrderNum=@OrderNum");
                SqlParam[] updateOrderParam = new SqlParam[] {
                     new SqlParam("@PayMoney", paymoney),
                     new SqlParam("@PayState", OrderPayState.AlreadyPay.GetHashCode()),
                     new SqlParam("@OrderState", OrderState.Finished.GetHashCode()),
                     new SqlParam("@PayTime", SiteHelper.GetWebServerCurrentTime()),
                     new SqlParam("@FinishedTime", SiteHelper.GetWebServerCurrentTime()),
                     new SqlParam("@OrderNum", ordernum)};
                sqls.Add(sbUpdateOrder);
                objs.Add(updateOrderParam);

                //更新财务信息状态
                StringBuilder balanceAdd = new StringBuilder();
                if (!string.IsNullOrEmpty(buyer_id) && !string.IsNullOrEmpty(buyer_logon_id))
                {
                    balanceAdd.Append("update YR_UserFinancial set TradeNo=@TradeNo, CurrentBalance=" + (balance + balance2) + ",State=@State,buyer_id=@buyer_id,buyer_logon_id=@buyer_logon_id Where ID =@ID");
                    SqlParam[] parmBalanceAdd = new SqlParam[] { new SqlParam("@ID", ht["ID"]), new SqlParam("@TradeNo", ht["TradeNo"]), new SqlParam("@State", ht["State"]), new SqlParam("@buyer_id", buyer_id), new SqlParam("@buyer_logon_id", buyer_logon_id) };
                    sqls.Add(balanceAdd);
                    objs.Add(parmBalanceAdd);
                }
                else
                {
                    balanceAdd.Append("update YR_UserFinancial set TradeNo=@TradeNo, CurrentBalance=" + (balance + balance2) + ",State=@State Where ID =@ID");
                    SqlParam[] parmBalanceAdd = new SqlParam[] { new SqlParam("@ID", financialid), new SqlParam("@TradeNo", tradeno), new SqlParam("@State", UserFinancialState.Effect.GetHashCode()) };
                    sqls.Add(balanceAdd);
                    objs.Add(parmBalanceAdd);
                }

                // 发送短消息
                StringBuilder msgAdd = new StringBuilder();
                msgAdd.Append("Insert into YR_Messages(");
                msgAdd.Append("ID,OperatorID,UserID,MessageTitle,MessageContent,MessageType,ModuleType,ReleaseTime,state) ");
                msgAdd.Append(" values(@ID,@OperatorID,@UserID,@MessageTitle,@MessageContent,@MessageType,@ModuleType,@ReleaseTime,@State)");
                StringBuilder sbMsg = new StringBuilder();
                sbMsg.Append("订单：").Append(ordernum).Append(" 消费 ").Append(paymoney.ToString("#0.00")).Append("元");
                SqlParam[] parmMsgAdd = new SqlParam[] { 
                                     new SqlParam("@ID", CommonHelper.GetGuid),
                                     new SqlParam("@OperatorID", ""),
                                     new SqlParam("@UserID", ht["UserID"]),
                                     new SqlParam("@MessageTitle", "订单消费通知"),
                                     new SqlParam("@MessageContent",sbMsg.ToString()),
                                     new SqlParam("@MessageType", MessageType.System.GetHashCode()),
                                     new SqlParam("@ModuleType", MessageModuleType.UseVehicle.GetHashCode()),
                                     new SqlParam("@ReleaseTime",SiteHelper.GetWebServerCurrentTime()),
                                     new SqlParam("@State", MessageState.Enable.GetHashCode())};
                sqls.Add(msgAdd);
                objs.Add(parmMsgAdd);

                //更新优惠券状态
                string couponID = SiteHelper.GetHashTableValueByKey(orderinfo, "CouponID");
                if (!string.IsNullOrEmpty(couponID))
                {
                    StringBuilder sbUpdateCoupon = new StringBuilder();
                    SqlParam[] parmUpdateCoupon = null;
                    sbUpdateCoupon.Append("update YR_UserCoupon set OrderID=@OrderID where ID=@ID");
                    parmUpdateCoupon = new SqlParam[] { new SqlParam("@OrderID", SiteHelper.GetHashTableValueByKey(orderinfo, "ID")), new SqlParam("@ID", SiteHelper.GetHashTableValueByKey(orderinfo, "CouponID")) };
                    sqls.Add(sbUpdateCoupon);
                    objs.Add(parmUpdateCoupon);
                }

                return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 用户订单余额支付
        /// </summary>
        /// <param name="ordernum"></param>
        /// <param name="payway"></param>
        /// <returns></returns>
        public bool UserOrderPay(string ordernum, out string strPay)
        {
            strPay = "";
            try
            {
                OrdersManager om = new OrdersManager();
                VehicleManager vm = new VehicleManager();
                SysSettingManager settingManager = new SysSettingManager();

                Hashtable ht = om.GetOrderInfoByNum(ordernum);
                Hashtable userinfo = GetUserInfoByUserID(SiteHelper.GetHashTableValueByKey(ht, "UserID"));
                decimal balance = 0.00m, balance2 = 0.00m, raiseBalance = 0.00m;
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance"), out balance);
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "Balance2"), out balance2);
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(userinfo, "RaiseBalance"), out raiseBalance);

                Hashtable vehicleinfo = vm.GetVehicleInfoByID(SiteHelper.GetHashTableValueByKey(ht, "VehicleID"));

                decimal totalMoney = 0.00m;
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(ht, "TotalMoney"), out totalMoney);
                decimal payMoney = 0.00m;
                decimal.TryParse(SiteHelper.GetHashTableValueByKey(ht, "PayMoney"), out payMoney);
                

                List<StringBuilder> sqls = new List<StringBuilder>();
                List<object> objs = new List<object>();

                strPay = "本次骑行共消费" + totalMoney + "元，共扣除账户余额" + payMoney + "元";

                //检测支付后余额不可为负值
                if (payMoney > (balance+balance2))
                    return false;

                //更改订单状态
                StringBuilder sbUpdateOrder = new StringBuilder();
                sbUpdateOrder.Append(@"UPDATE yr_orders SET PayMoney=@PayMoney,PayTime=@PayTime,FinishedTime=@FinishedTime,PayState=@PayState,OrderState=@OrderState where ID=@ID");
                SqlParam[] updateOrderParam = new SqlParam[] {
                     new SqlParam("@PayMoney", payMoney),
                     new SqlParam("@PayState", OrderPayState.AlreadyPay.GetHashCode()),
                     new SqlParam("@OrderState", OrderState.Finished.GetHashCode()),
                     new SqlParam("@PayTime", SiteHelper.GetWebServerCurrentTime()),
                     new SqlParam("@FinishedTime", SiteHelper.GetWebServerCurrentTime()),
                     new SqlParam("@ID", ht["ID"])};
                sqls.Add(sbUpdateOrder);
                objs.Add(updateOrderParam);

                if (payMoney > 0)
                {
                    //更新账户余额
                    StringBuilder sbUpdateUserInfo2 = new StringBuilder();
                    SqlParam[] parm2 = null;
                    if (balance > payMoney)
                    {
                        sbUpdateUserInfo2.Append("update YR_UserInfo set Balance=Balance-@ChangesAmount Where ID =@UserID");
                        parm2 = new SqlParam[] { new SqlParam("@ChangesAmount", payMoney), new SqlParam("@UserID", ht["USERID"]) };
                    }
                    else
                    {
                        sbUpdateUserInfo2.Append("update YR_UserInfo set Balance=0.00,Balance2=Balance2-@ChangesAmount Where ID =@UserID");
                        parm2 = new SqlParam[] { new SqlParam("@ChangesAmount", (payMoney - balance)), new SqlParam("@UserID", ht["USERID"]) };
                    }
                    sqls.Add(sbUpdateUserInfo2);
                    objs.Add(parm2);

                    // 增加财务信息
                    StringBuilder balanceAdd2 = new StringBuilder();
                    balanceAdd2.Append("Insert into YR_UserFinancial(");
                    balanceAdd2.Append("ID,UserID,ChangesAmount,ChangesTime,ChangesType,OrderNum,OrderPayID,Remark,Operator,OperatorType,OperatorWay,CurrentBalance,state) ");
                    balanceAdd2.Append("values(@ID,@UserID,@ChangesAmount,@ChangesTime,@ChangesType,@OrderNum,@OrderPayID,@Remark,@Operator,@OperatorType,@OperatorWay,@CurrentBalance,@State)");
                    SqlParam[] parmBalanceAdd2 = new SqlParam[] { 
                                     new SqlParam("@ID", CommonHelper.GetGuid),
                                     new SqlParam("@UserID", ht["USERID"]),
                                     new SqlParam("@ChangesAmount", -payMoney),
                                     new SqlParam("@ChangesTime", SiteHelper.GetWebServerCurrentTime()),
                                     new SqlParam("@ChangesType", UserFinancialChangesType.Consumption.GetHashCode().ToString()),
                                     new SqlParam("@OrderNum", ht["ORDERNUM"]),
                                     new SqlParam("@OrderPayID", SiteHelper.GeneratePayID()),
                                     new SqlParam("@Remark", "订单消费"),
                                     new SqlParam("@Operator", ""),
                                     new SqlParam("@OperatorType", UserFinancialOperatorType.Admin.GetHashCode().ToString()),
                                     new SqlParam("@OperatorWay",UserFinancialOperatorWay.Plat.GetHashCode()),
                                     new SqlParam("@CurrentBalance",balance+balance2-payMoney),
                                     new SqlParam("@State", UserFinancialState.Effect.GetHashCode().ToString())};
                    sqls.Add(balanceAdd2);
                    objs.Add(parmBalanceAdd2);
                }

                VehicleModelManager modeManager = new VehicleModelManager();
                Hashtable modeHt = modeManager.GetModelByID(SiteHelper.GetHashTableValueByKey(vehicleinfo, "Model"));
                StringBuilder msg2Add = new StringBuilder();
                msg2Add.Append("Insert into YR_Messages(");
                msg2Add.Append("ID,OperatorID,UserID,MessageTitle,MessageContent,MessageType,ReleaseTime,state) ");
                msg2Add.Append(" values(@ID,@OperatorID,@UserID,@MessageTitle,@MessageContent,@MessageType,@ReleaseTime,@State)");
                string sbMsg2 = string.Format("您于{0}骑行车辆{1}({2})，消费金额{3}元，当前账户余额{4}元！", SiteHelper.GetWebServerCurrentTime().ToString("yyyy年MM月dd日HH:mm:ss"), SiteHelper.GetHashTableValueByKey(vehicleinfo, "LicenseNumber"), SiteHelper.GetHashTableValueByKey(modeHt, "Name"), payMoney.ToString("#0.00"), (balance + balance2 - payMoney).ToString("#0.00"));
                SqlParam[] parmMsg2Add = new SqlParam[] { 
                                     new SqlParam("@ID", CommonHelper.GetGuid),
                                     new SqlParam("@OperatorID", ""),
                                     new SqlParam("@UserID", ht["USERID"]),
                                     new SqlParam("@MessageTitle", "余额消费提醒"),
                                     new SqlParam("@MessageContent",sbMsg2),
                                     new SqlParam("@MessageType", MessageType.System.GetHashCode()),
                                     new SqlParam("@ReleaseTime",SiteHelper.GetWebServerCurrentTime()),
                                     new SqlParam("@State", MessageState.Enable.GetHashCode())};
                sqls.Add(msg2Add);
                objs.Add(parmMsg2Add);

                //更新优惠券状态
                string couponID = SiteHelper.GetHashTableValueByKey(ht, "CouponID");
                if (!string.IsNullOrEmpty(couponID))
                {
                    StringBuilder sbUpdateCoupon = new StringBuilder();
                    SqlParam[] parmUpdateCoupon = null;
                    sbUpdateCoupon.Append("update YR_UserCoupon set OrderID=@OrderID where ID=@ID");
                    parmUpdateCoupon = new SqlParam[] { new SqlParam("@OrderID", SiteHelper.GetHashTableValueByKey(ht, "ID")), new SqlParam("@ID", SiteHelper.GetHashTableValueByKey(ht, "CouponID")) };
                    sqls.Add(sbUpdateCoupon);
                    objs.Add(parmUpdateCoupon);
                }

                return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
            }
            catch
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 异常还车
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="isUseFee">是否收费用车费</param>
        /// <returns></returns>
        public bool AbnormalReturnVehicle(string orderNum, bool isUseFee)
        {
            UserInfoManager uim = new UserInfoManager();
            OrdersManager om = new OrdersManager();
            VehicleManager vm = new VehicleManager();
            ServiceAreaManager areaManager = new ServiceAreaManager();
            SysSettingManager settingManager = new SysSettingManager();
            Hashtable order = om.GetOrderByNum(orderNum);
            //判断是否有车
            if (order == null)
            {
                return false;
            }
            else
            {
                if (SiteHelper.GetHashTableValueByKey(order, "OrderState") != OrderState.Valid.GetHashCode().ToString())
                    return false;

                string vehicle_id = SiteHelper.GetHashTableValueByKey(order, "VehicleID");
                Hashtable vehicle_ht = vm.GetVehicleInfoByID(vehicle_id);
                string longitude = SiteHelper.GetHashTableValueByKey(vehicle_ht, "Longitude");
                string latitude = SiteHelper.GetHashTableValueByKey(vehicle_ht, "Latitude");

                //处理订单
                if (order == null)
                {
                    return false;
                }
                else
                {
                    int renttime = 0;
                    int.TryParse(SiteHelper.GetHashTableValueByKey(order, "RentDuration"), out renttime);
                    decimal hours = 0.00m;
                    decimal kmprice = 0.80m;
                    decimal minuteprice = 0.40m;
                    decimal mileage = 0;
                    int minutes = 0;
                    decimal userprivilegeratio = 1;
                    decimal raisecommisssionratio = 0.10m;
                    decimal TotalMoney = 0.00m;
                    decimal SettlementMoney = 0.00m;
                    decimal occupyMoney = 0.0m;
                    decimal payMoney = 0.00m;
                    decimal RaiseCommissionMoney = 0.00m;
                    decimal outServiceAreaFee = 0.00m;
                    DateTime createtime = DateTime.Now;
                    DateTime startTime = DateTime.Now;
                    String returnParkingID = null, returnLocType = null;
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "KMPrice"), out kmprice);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "MinutePrice"), out minuteprice);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "Mileage"), out mileage);
                    //decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "Minutes"), out minutes);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "UserPrivilegeRatio"), out userprivilegeratio);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "RaiseCommissionRatio"), out raisecommisssionratio);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "TotalMoney"), out TotalMoney);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "SettlementMoney"), out SettlementMoney);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "OccupyMoney"), out occupyMoney);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(order, "RaiseCommissionMoney"), out RaiseCommissionMoney);
                    DateTime.TryParse(SiteHelper.GetHashTableValueByKey(order, "CreateTime"), out createtime);
                    DateTime.TryParse(SiteHelper.GetHashTableValueByKey(order, "StartTime"), out startTime);
                    int startminutes = 0;
                    int.TryParse(SiteHelper.GetHashTableValueByKey(order, "StartMinutes"), out startminutes);
                    string strStartTime = SiteHelper.GetHashTableValueByKey(order, "StartTime");

                    string gpsnum = SiteHelper.GetHashTableValueByKey(order, "VehicleGPSNum");
                    //锁车
                    bool isSuccess = vm.CloseVehicle(SiteHelper.GetHashTableValueByKey(order, "VehicleID"));
                    string f = isUseFee ? "收费" : "免费";
                    string r = isSuccess ? "成功" : "失败";
                    string imei = SiteHelper.GetHashTableValueByKey(vehicle_ht, "VehicleGPSNum");
                    Logger.Debug("异常还车"+f+ ",carId:" + imei + ",锁车" + r);
                    /*if (!isSuccess)
                    {
                        return false;
                    }*/
                    VehiclePriceManager priceManager = new VehiclePriceManager();
                    Hashtable billing_ht = priceManager.GetOrderSettlement(SiteHelper.GetHashTableValueByKey(order, "ID"));
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "MinutePrice"), out minuteprice);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "KmPrice"), out kmprice);
                    int.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "Minutes"), out minutes);
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(billing_ht, "TotalMoney"), out TotalMoney);

                    if (isUseFee)
                    { 
                        SettlementMoney = TotalMoney;

                        #region 判断还车点是否在运营区域内，运营区域外加收费用
                        Hashtable area_ht = areaManager.GetServiceAreaByVehicleID(SiteHelper.GetHashTableValueByKey(order, "VID"));
                        if (TotalMoney > 0 && area_ht != null && area_ht.Keys.Count > 0)
                        {
                            returnLocType = "03";
                            List<LatLng> area_pts = new List<LatLng>();
                            string coordinates = SiteHelper.GetHashTableValueByKey(area_ht, "Coordinates");
                            foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                LatLng pt = new LatLng(double.Parse(pt_arr[1]),double.Parse(pt_arr[0]));
                                area_pts.Add(pt);
                            }
                            LatLng vehicle_pt = new LatLng(double.Parse(latitude), double.Parse(longitude));
                            //vehicle_pt = SiteHelper.TransformFromWGSToGCJ(vehicle_pt);
                            vehicle_pt = SiteHelper.GPSToGCJ02(longitude, latitude);
                            vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6), Math.Round(vehicle_pt.longitude, 6));
                            bool isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, area_pts);
                            if (!isInPoly)
                            {
                                returnLocType = "04";
                                string serviceFee = settingManager.GetValueByKey("OutServiceAreaFee");
                                if (serviceFee.StartsWith("+"))
                                {
                                    serviceFee = serviceFee.Substring(1, serviceFee.Length - 1);
                                    decimal.TryParse(serviceFee, out outServiceAreaFee);
                                    TotalMoney = TotalMoney + outServiceAreaFee;
                                }
                                else
                                {
                                    serviceFee = serviceFee.Substring(1, serviceFee.Length - 1);
                                    decimal.TryParse(serviceFee, out outServiceAreaFee);
                                    TotalMoney = TotalMoney * outServiceAreaFee;
                                }
                                outServiceAreaFee = TotalMoney - SettlementMoney;
                            }
                        }
                        #endregion

                        #region 判断还车点是否在停车点内，否则加收费用
                        string returnVehicleMode = settingManager.GetValueByKey("ReturnVehicleMode");
                        if (TotalMoney > 0 && outServiceAreaFee == 0 && returnVehicleMode == "1")
                        {
                            returnLocType = "01";
                            LatLng vehicle_pt = new LatLng(double.Parse(latitude), double.Parse(longitude));
                            //vehicle_pt = SiteHelper.TransformFromWGSToGCJ(vehicle_pt);
                            vehicle_pt = SiteHelper.GPSToGCJ02(longitude, latitude);
                            vehicle_pt = new LatLng(Math.Round(vehicle_pt.latitude, 6), Math.Round(vehicle_pt.longitude, 6));
                            /*Hashtable parking_ht = areaManager.GetNearestParking(vehicle_pt.longitude.ToString(), vehicle_pt.latitude.ToString(), SiteHelper.GetHashTableValueByKey(order, "VID"));
                            List<LatLng> parking_pts = new List<LatLng>();
                            if (parking_ht != null && parking_ht.Keys.Count > 0)
                            {
                                returnParkingID = SiteHelper.GetHashTableValueByKey(parking_ht, "ID");
                                string coordinates = SiteHelper.GetHashTableValueByKey(parking_ht, "Coordinates");
                                foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    LatLng pt = new LatLng(double.Parse(pt_arr[1]), double.Parse(pt_arr[0]));
                                    parking_pts.Add(pt);
                                }
                            }
                            bool isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, parking_pts);
                            if (!isInPoly)
                            {
                                returnLocType = "02";
                                returnParkingID = null;
                                string serviceFee = settingManager.GetValueByKey("OutParkingAreaFee");
                                if (serviceFee.StartsWith("+"))
                                {
                                    serviceFee = serviceFee.Substring(1, serviceFee.Length - 1);
                                    decimal.TryParse(serviceFee, out outServiceAreaFee);
                                    TotalMoney = TotalMoney + outServiceAreaFee;
                                }
                                else
                                {
                                    serviceFee = serviceFee.Substring(1, serviceFee.Length - 1);
                                    decimal.TryParse(serviceFee, out outServiceAreaFee);
                                    TotalMoney = TotalMoney * outServiceAreaFee;
                                }
                                outServiceAreaFee = TotalMoney - SettlementMoney;
                            }*/
                            DataTable parking_dt = areaManager.GetNearestParkingList(vehicle_pt.longitude.ToString(), vehicle_pt.latitude.ToString(), SiteHelper.GetHashTableValueByKey(order, "VID"));
                            if (parking_dt != null)
                            {
                                bool isInPoly = false;
                                foreach (DataRow dr in parking_dt.Rows)
                                {
                                    List<LatLng> parking_pts = new List<LatLng>();
                                    returnParkingID = dr["ID"].ToString();
                                    string coordinates = dr["Coordinates"].ToString();
                                    foreach (string str in coordinates.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        string[] pt_arr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        LatLng pt = new LatLng(double.Parse(pt_arr[1]), double.Parse(pt_arr[0]));
                                        parking_pts.Add(pt);
                                    }
                                    isInPoly = SiteHelper.IsPtInPoly(vehicle_pt, parking_pts);
                                    if (isInPoly)
                                    {
                                        break;
                                    }
                                }
                                if (!isInPoly)
                                {
                                    returnLocType = "02";
                                    returnParkingID = null;
                                    string serviceFee = settingManager.GetValueByKey("OutParkingAreaFee");
                                    if (serviceFee.StartsWith("+"))
                                    {
                                        serviceFee = serviceFee.Substring(1, serviceFee.Length - 1);
                                        decimal.TryParse(serviceFee, out outServiceAreaFee);
                                        TotalMoney = TotalMoney + outServiceAreaFee;
                                    }
                                    else
                                    {
                                        serviceFee = serviceFee.Substring(1, serviceFee.Length - 1);
                                        decimal.TryParse(serviceFee, out outServiceAreaFee);
                                        TotalMoney = TotalMoney * outServiceAreaFee;
                                    }
                                    outServiceAreaFee = TotalMoney - SettlementMoney;
                                }
                            }
                        }
                        #endregion

                        payMoney = TotalMoney;
                    }

                    Hashtable userParm = new Hashtable();
                    userParm["VID"] = SiteHelper.GetHashTableValueByKey(order, "VehicleID");
                    userParm["OrderNum"] = SiteHelper.GetHashTableValueByKey(order, "OrderNum");
                    userParm["TotalMoney"] = TotalMoney;
                    userParm["SettlementMoney"] = SettlementMoney;
                    userParm["PayMoney"] = payMoney;
                    userParm["PayTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                    userParm["EndTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                    userParm["FinishedTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                    userParm["OID"] = SiteHelper.GetHashTableValueByKey(order, "ID");
                    userParm["ChangesAmount"] = -Math.Abs(TotalMoney);
                    userParm["UserFinancialID"] = CommonHelper.GetGuid;
                    userParm["ChangesTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
                    userParm["ChangesType"] = UserFinancialChangesType.Consumption.GetHashCode().ToString();
                    userParm["OrderID"] = SiteHelper.GetHashTableValueByKey(order, "ID");
                    userParm["RentDuration"] = hours;
                    userParm["Remark"] = isUseFee ? "异常还车(收费)" : "异常还车(免费)";
                    userParm["Minutes"] = minutes;
                    userParm["DiffMoney"] = TotalMoney;
                    userParm["Operator"] = "";
                    userParm["OperatorType"] = UserFinancialOperatorType.Admin.GetHashCode().ToString();
                    userParm["State"] = UserFinancialState.Effect.GetHashCode().ToString();
                    userParm["MessageID"] = CommonHelper.GetGuid;
                    userParm["OutServiceAreaFee"] = (int)outServiceAreaFee;
                    userParm["ReturnParkingID"] = returnParkingID;
                    userParm["ReturnLocType"] = returnLocType;

                    bool isSettlementSuccess = uim.ReturnVehicle(userParm);
                    if (isSettlementSuccess)
                    {
                        Logger.Debug("异常还车" + f + ",carId:" + imei + ",锁车" + r+",操作成功");
                        om.UpdateOrderGPS(SiteHelper.GetHashTableValueByKey(order, "ID"), false);
                        return true;
                    }
                    else
                    {
                        Logger.Debug("异常还车" + f + ",carId:" + imei + ",锁车" + r + ",操作失败");
                        return false;
                    }
                }
            }
        }

        public bool ReturnVehicle(Hashtable ht)
        {
            try
            {
                if (ht != null)
                {
                    decimal payMoney = 0.00m;
                    decimal.TryParse(SiteHelper.GetHashTableValueByKey(ht, "PayMoney"), out payMoney);

                    List<StringBuilder> sqls = new List<StringBuilder>();
                    List<object> objs = new List<object>();

                    //更改车辆状态
                    StringBuilder sbUpdateVehicle = new StringBuilder();
                    sbUpdateVehicle.Append("UPDATE yr_vehicles SET UseState=@UseState,ParkingID=@ParkingID WHERE ID=@ID");
                    SqlParam[] updateVehicleParam = new SqlParam[] { 
                        new SqlParam("@ID", ht["VID"]),
                        new SqlParam("@ParkingID", ht["ReturnParkingID"]),
                        new SqlParam("@UseState",VehicleUseState.Free.GetHashCode())
                    };
                    sqls.Add(sbUpdateVehicle);
                    objs.Add(updateVehicleParam);


                    //更改订单状态
                    StringBuilder sbUpdateOrder = new StringBuilder();
                    sbUpdateOrder.Append(@"UPDATE yr_orders SET TotalMoney=@TotalMoney,Minutes=@Minutes,SettlementMoney=@SettlementMoney,PayMoney=@PayMoney,PayState=@PayState,OrderState=@OrderState,EndTime=@EndTime,FinishedTime=@FinishedTime,OrderRemark=@OrderRemark,OutServiceAreaFee=@OutServiceAreaFee,ReturnLocType=@ReturnLocType,ToParking=@ToParking where ID=@ID and OrderState=" + OrderState.Valid.GetHashCode());
                    SqlParam[] updateOrderParam = new SqlParam[] { new SqlParam("@TotalMoney", ht["TotalMoney"]),
                     new SqlParam("@Minutes", ht["Minutes"]),
                     new SqlParam("@SettlementMoney", ht["SettlementMoney"]),
                     new SqlParam("@PayMoney", ht["PayMoney"]),
                     new SqlParam("@EndTime", ht["EndTime"]),
                     new SqlParam("@PayState", payMoney>0?OrderPayState.NotPay.GetHashCode():OrderPayState.AlreadyPay.GetHashCode()),
                     new SqlParam("@OrderState", payMoney>0?OrderState.UnPay.GetHashCode():OrderState.Finished.GetHashCode()),
                     new SqlParam("@FinishedTime", payMoney>0?null:ht["EndTime"]),
                     new SqlParam("@OrderRemark", ht["Remark"]),
                     new SqlParam("@OutServiceAreaFee", ht["OutServiceAreaFee"]),
                     new SqlParam("@ReturnLocType", ht["ReturnLocType"]),
                     new SqlParam("@ToParking", ht["ReturnParkingID"]),
                     new SqlParam("@ID", ht["OID"])};
                    sqls.Add(sbUpdateOrder);
                    objs.Add(updateOrderParam);

                    return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
