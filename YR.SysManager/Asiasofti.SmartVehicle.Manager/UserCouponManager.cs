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
using YR.Common.DotNetData;
using System.Net;
using YR.Common.DotNetBean;
using System.Configuration;
using System.Xml;


namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 用户优惠券管理类
    /// </summary>
    public class UserCouponManager
    {
        /// <summary>
        /// 添加或修改信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEdit(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_UserCoupon", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DeleteByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_UserCoupon", "ID", _key, ht);
        }

        /// <summary>
        /// 通过ID获取信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Hashtable GetDataByID(string id)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_UserCoupon", "ID", id);
        }

        /// <summary>
        /// 列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select a.*,b.RealName,b.BindPhone from YR_UserCoupon a join YR_UserInfo b on a.userid=b.id where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 获取指定用户下的优惠券
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserCoupons(string uid,bool isEnable,int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select a.*,b.RealName,b.BindPhone from YR_UserCoupon a join YR_UserInfo b on a.userid=b.id where a.userid='{0}'", uid);
            if (isEnable)
                strSql.Append(" and a.BeginTime<=getdate() and a.EndTime>=getdate()");
            DataTable dt = DataFactory.SqlDataBase().GetPageList(strSql.ToString(), null, "CreateTime", "Desc", pageIndex, pageSize, ref count);
            return dt;
        }

        /// <summary>
        /// 新注册用户赠送优惠券
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool NewUserGiveCoupons(string uid)
        {
            try
            {
                SysSettingManager settingManager = new SysSettingManager();

                List<StringBuilder> sqls = new List<StringBuilder>();
                List<object> objs = new List<object>();

                int couponNum = 0;
                int.TryParse(settingManager.GetValueByKey("RegCouponNum"), out couponNum);
                decimal couponMoney = 0.00m;
                decimal.TryParse(settingManager.GetValueByKey("RegCouponMoney"), out couponMoney);
                double CouponPeriod = 0;
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
                }

                return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 邀请好友注册赠送优惠券,在用户首次缴纳押金成功后进行赠券操作
        /// </summary>
        /// <param name="reg_uid"></param>
        /// <returns></returns>
        public bool InviteUserGiveCoupons(string uid)
        {
            try
            {
                UserInfoManager userManager = new UserInfoManager();
                DepositReturnManager depositManager = new DepositReturnManager();
                SysSettingManager settingManager = new SysSettingManager();

                List<StringBuilder> sqls = new List<StringBuilder>();
                List<object> objs = new List<object>();

                bool isFirstDeposit=depositManager.IsFirstDeposit(uid);
                Hashtable user_ht= userManager.GetUserInfoByUserID(uid);
                if (user_ht != null && user_ht.Keys.Count > 0 && isFirstDeposit)
                {
                    string invite_uid = SiteHelper.GetHashTableValueByKey(user_ht, "InviteUserID");

                    if (!string.IsNullOrEmpty(invite_uid))
                    {
                        int couponNum = 0;
                        decimal couponMoney = 0.00m;
                        double CouponPeriod = 0;
                        //邀请注册双方同时赠送优惠券
                        couponNum = 0;
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
                }

                return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 订单分享赠送优惠券
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="oid"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public bool OrderShareGiveCoupons(string uid,string oid,string platform)
        {
            try
            {
                SysSettingManager settingManager = new SysSettingManager();

                List<StringBuilder> sqls = new List<StringBuilder>();
                List<object> objs = new List<object>();

                int couponNum = 0;
                int.TryParse(settingManager.GetValueByKey("OrderShareNum"), out couponNum);
                decimal couponMoney = 0.00m;
                decimal.TryParse(settingManager.GetValueByKey("OrderShareMoney"), out couponMoney);
                double CouponPeriod = 0;
                double.TryParse(settingManager.GetValueByKey("CouponPeriod"), out CouponPeriod);
                int orderShareTimes = 0;//当天最大赠送次数限制
                int.TryParse(settingManager.GetValueByKey("OrderShareTimes"), out orderShareTimes);
                int shareTimes=GetCouponCountByDate(uid, DateTime.Now);
                if (couponNum > 0 && couponMoney > 0 && shareTimes < orderShareTimes)
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
                }

                StringBuilder sbUpdateOrder = new StringBuilder();
                sbUpdateOrder.Append("update YR_Orders set ShareState=@ShareState,SharePlatform=@SharePlatform where ID=@ID");
                SqlParam[] parmUpdateOrder = new SqlParam[] { 
                        new SqlParam("@ShareState", 1), 
                        new SqlParam("@SharePlatform", platform), 
                        new SqlParam("@ID", oid) };
                sqls.Add(sbUpdateOrder);
                objs.Add(parmUpdateOrder);

                return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取用户指定日期得到的优惠券数量
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int GetCouponCountByDate(string uid,DateTime dt)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"select count(1) from YR_UserCoupon where userid='{0}' and CreateTime between '{1}' and '{2}'", uid, dt.Date.ToString("yyyy-MM-dd 00:00:00"), dt.Date.ToString("yyyy-MM-dd 23:59:59"));
            return (int)DataFactory.SqlDataBase().GetObjectValue(strSql);
        }
    }
}
