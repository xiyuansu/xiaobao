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
using YR.Common.DotNetEncrypt;
using YR.Common.DotNetData;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 运维用户管理 
    /// </summary>
    public class OPUserManager
    {
        /// <summary>
        /// 通过登录账户名获取会员记录
        /// </summary>
        /// <param name="bindPhone"></param>
        /// <returns></returns>
        public DataTable GetUserInfoByAccount(string account)
        {
            StringBuilder str = new StringBuilder();
            str.Append(@"select UserID, UserAccount,'" + SiteHelper.GetFileServer() + "'+ HeadPortrait as HeadPortrait,UserName,IDCardNum,UserSex,Tel,Email,LastloginTime,UserState from YR_OPUser where DeleteMark=1 and useraccount='" + account + "'");
            return DataFactory.SqlDataBase().GetDataTableBySQL(str);
        }

        public DataTable GetUserInfoByMobile(string telphone)
        {
            StringBuilder str = new StringBuilder();
            str.Append(@"select UserID, UserAccount,'" + SiteHelper.GetFileServer() + "'+ HeadPortrait as HeadPortrait,UserName,IDCardNum,UserSex,Tel,Email,LastloginTime,UserState from YR_OPUser where DeleteMark=1 and tel='" + telphone + "'");
            return DataFactory.SqlDataBase().GetDataTableBySQL(str);
        }

        /// <summary>
        /// 通过用户ID获取用户信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Hashtable GetUserInfoByUserID(string userID)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_OPUser", "UserID", userID);
        }
        
        /// <summary>
        /// 跟用户id获取用户信息返回DataTable
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public DataTable GetUserInfoByUid(string uid)
        {
            StringBuilder str = new StringBuilder();
            str.Append(@"select UserID, UserAccount,'" + SiteHelper.GetFileServer() + "'+ HeadPortrait as HeadPortrait,UserName,IDCardNum,UserSex,Tel,Email,LastloginTime,UserState from YR_OPUser where DeleteMark=1 and UserID='" + uid + "'");
            return DataFactory.SqlDataBase().GetDataTableBySQL(str);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="ht"></param>
        public int AddUserInfo(Hashtable ht)
        {
            ht["UserID"] = CommonHelper.GetGuid;
            ht["CreateTime"] = SiteHelper.GetWebServerCurrentTime().ToString();
            ht["UserState"] = (int)UserState.Enable;
            return DataFactory.SqlDataBase().InsertByHashtable("YR_OPUser", ht);
        }

        /// <summary>
        /// 添加或修改用户信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditUserInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_OPUser", "UserID", _key, ht);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public DataTable UserLogin(string account,string pwd)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT  UserID,UserAccount,UserPwd,UserName,HeadPortrait,UserSex,Tel,Email,UserState");
            strSql.Append(" FROM YR_OPUser WHERE ");
            strSql.Append(" UserAccount=@UserAccount ");
            strSql.Append(" and UserPwd=@UserPwd ");
            strSql.Append(" and DeleteMark=1");
            SqlParam[] para = {
                                         new SqlParam("@UserAccount",account),
                                         new SqlParam("@UserPwd",Md5Helper.MD5(pwd, 32))};
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql, para);
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
            strSql.Append(@"select UserID,NickName,UserName,IDCardNum,HeadPortrait,UserSex,Tel,Email,UserState,CreateTime,ReceiveSMS from YR_OPUser where DeleteMark=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "Desc", pageIndex, pageSize, ref count);
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
        public DataTable GetOperateLogPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select l.ID,u.UserName,v.VehicleName,OperateTypeText,OperateTime from YR_OPLog l left join YR_OPUser u on u.UserID=l.UserID left join YR_Vehicles v on v.ID=l.VehicleID where 1=1 ");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "OperateTime", "Desc", pageIndex, pageSize, ref count);
        }

        public DataTable GetReceiveSMSUsers()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select distinct Tel from YR_OPUser where DeleteMark=1 and ReceiveSMS=1");
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllUser()
        {
            return DataFactory.SqlDataBase().GetDataTable("YR_OPUser");
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserListByCondition(string condition = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from YR_OPUser where UserState=1" + condition);
            return DataFactory.SqlDataBase().GetDataTableBySQL(sb);
        }

        /// <summary>
        /// 获取用户所管理的网点列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public DataTable GetUserParkingList(string uid)
        {
            StringBuilder str = new StringBuilder();
            str.Append(@"SELECT t1.* FROM YR_ServiceArea t1 join YR_OPUserParking t2 on t1.ID=t2.ParkingID  where t1.Status=1 and t1.AreaType=2 and t1.DeleteMark=1 and t2.UserID='" + uid + "'");
            DataTable dt= DataFactory.SqlDataBase().GetDataTableBySQL(str);
            if(dt==null || dt.Rows.Count==0)
            {
                str.Clear();
                str.Append(@"SELECT * FROM YR_ServiceArea where Status=1 and AreaType=2 and DeleteMark=1");
                dt = DataFactory.SqlDataBase().GetDataTableBySQL(str);
            }
            return dt;
        }

        /// <summary>
        /// 保存网点用户所管理的网点列表
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="parkinglist"></param>
        /// <returns></returns>
        public bool SaveUserParkingList(string uid,List<string> parkinglist)
        {
            List<StringBuilder> sqls = new List<StringBuilder>();
            List<object> objs = new List<object>();

            StringBuilder sqlDeleteUserParking = new StringBuilder();
            sqlDeleteUserParking.Append("delete YR_OPUserParking where UserID=@UserID");
            SqlParam[] parmDeleteUserParking = new SqlParam[] { new SqlParam("@UserID", uid) };
            sqls.Add(sqlDeleteUserParking);
            objs.Add(parmDeleteUserParking);

            parkinglist.Add(null);
            if (parkinglist != null && parkinglist.Count > 0)
            {
                foreach (string parkingid in parkinglist)
                {
                    StringBuilder sqlInsertUserParking = new StringBuilder();
                    sqlInsertUserParking.Append("insert into YR_OPUserParking(UserID,ParkingID)values(@UserID,@ParkingID)");
                    SqlParam[] parmInsertUserParking = new SqlParam[] { new SqlParam("@UserID", uid), new SqlParam("@ParkingID", parkingid) };
                    sqls.Add(sqlInsertUserParking);
                    objs.Add(parmInsertUserParking);
                }
            }
            return DataFactory.SqlDataBase().BatchExecuteBySql(sqls.ToArray(), objs.ToArray()) >= 0 ? true : false;
        }

        /// <summary>
        /// 添加网点用户操作记录
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public int AddUserOperate(Hashtable ht)
        {
            return DataFactory.SqlDataBase().InsertByHashtable("YR_OPLog", ht);
        }

        /// <summary>
        /// 根据网点操作员id获取当前所占车辆信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Hashtable GetVehicleInfoByOperator(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT top 1 YR_Vehicles.*,dbo.GetParkingName(YR_Vehicles.ID) ParkingName FROM YR_Vehicles join (SELECT top 1 VehicleID,OperateType,ROW_NUMBER() OVER (ORDER BY OperateTime desc ) as RowNumber FROM YR_OPLog where OperateType in(1,2) and UserID='" + uid + "' order by OperateTime desc) t2 on YR_Vehicles.id=t2.VehicleID where t2.OperateType=1 and YR_Vehicles.UseState=4");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return DataTableHelper.DataRowToHashTable(dt.Rows[0]);
            }
        }

        /// <summary>
        /// 根据运维用户id及关键字查找相关车辆
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public DataTable SearchVehicles(string userid, string keywords)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("SELECT ID,LicenseNumber FROM YR_Vehicles where (DeleteMark=1 or DeleteMark is null) and exists(SELECT 1 FROM YR_OPUserParking where UserID='{0}' and (ParkingID=yr_vehicles.ParkingID or ParkingID is null)) and (LicenseNumber like '%{1}%' or name like '%{1}%') order by LicenseNumber", userid, keywords);
            return DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
        }

    }
}
