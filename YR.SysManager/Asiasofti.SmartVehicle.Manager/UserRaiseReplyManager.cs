/*
 * 代理商信息数据操作类
 * 作者：SJ
 * 时间：2015-05-10
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using YR.Busines;
using YR.Common.DotNetCode;
using System.Data;

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 代理商众筹申请
    /// </summary>
    public class UserRaiseReplyManager
    {
        /// <summary>
        /// 添加代理商众筹申请信息
        /// </summary>
        /// <param name="ht"></param>
        public int AddUserRaiseReply(Hashtable ht)
        {
            ht["ID"] = CommonHelper.GetGuid;
            return DataFactory.SqlDataBase().InsertByHashtable("YR_UserRaiseReply", ht);
        }

        /// <summary>
        /// 添加或修改众筹申请信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditUserRaiseReplyInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_UserRaiseReply", "ID", _key, ht);
        }

        /// <summary>
        /// 众筹申请审核
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public bool AuditUserRaiseReplyInfo(Hashtable ht)
        {
            try
            {
                if (ht!=null)
                {
                    StringBuilder[] sqls = new StringBuilder[1];
                    object[] objs = new object[1];

                    // 更新用户余额
                    StringBuilder sbUpdateUserInfo = new StringBuilder();
                    sbUpdateUserInfo.Append(" UPDATE YR_UserRaiseReply SET [State] = @State,ReviewNote =@ReviewNote WHERE ID=@ID");
                    // 更新用户状态
                    sbUpdateUserInfo.Append(" update YR_UserInfo set IsRaiseUser=1 Where ID in (select UserID from YR_UserRaiseReply where ID=@ID)");
                    SqlParam[] parm = new SqlParam[] { new SqlParam("@ID", ht["ID"]), new SqlParam("@State", ht["State"]), new SqlParam("@ReviewNote", ht["ReviewNote"]) };
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
        /// 根据用户ID获取代理商众筹申请信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public Hashtable GetUserRaiseReplyByID(string id)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_UserRaiseReply", "ID", id);
        }
        /// <summary>
        /// 通过用户id获取用户众筹申请状态
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public DataTable GetUserRaiseReplyByUserID(string userid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select top 1 * from YR_UserRaiseReply where UserID='").Append(userid).Append("'");
            return DataFactory.SqlDataBase().GetDataTableBySQL(sql);
        }
        /// <summary>
        /// 众筹申请列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetUserRaiseReplyInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select a.*,b.RealName,b.BindPhone,c.AccountName,c.Account,c.BankName,c.OpenBank from YR_UserRaiseReply a ");
            strSql.Append(" inner join YR_UserInfo b on a.UserID=b.ID LEFT JOIN YR_UserBankCard c ON b.ID=c.UserID where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "ReplyTime", "Desc", pageIndex, pageSize, ref count);
        }
    }
}
