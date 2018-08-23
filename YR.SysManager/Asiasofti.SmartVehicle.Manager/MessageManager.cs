/*
 * 会员消息数据操作类
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

namespace Asiasofti.SmartVehicle.Manager
{
    /// <summary>
    /// 会员消息管理 
    /// </summary>
    public class MessageManager
    {
        /// <summary>
        /// 添加或修改会员消息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEditMessageInfo(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_Messages", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除会员消息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int DelMessageInfoByID(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_Messages", "ID", _key, ht);
        }

        /// <summary>
        /// 通过消息主键ID获取消息信息
        /// </summary>
        /// <param name="msgID">消息ID</param>
        /// <returns></returns>
        public Hashtable GetMessageInfoByID(string msgID)
        {
            return DataFactory.SqlDataBase().GetHashtableById("YR_Messages", "ID", msgID);
        }

        /// <summary>
        /// 会员消息列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetMessageInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select a.*,b.[User_Name] from YR_Messages a left JOIN Base_UserInfo b on a.OperatorID=b.[User_ID] where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "ReleaseTime", "Desc", pageIndex, pageSize, ref count);
        }
        /// <summary>
        /// 获取会员某个模块下未读的消息条数
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="moduletype"></param>
        /// <returns></returns>
        public string GetNoReadMessagesCountByUser(string uid,string messagetype)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select count(id) from yr_messages where MessageType=" + messagetype + " and ID NOT IN (select messageid from yr_messagereadlog where userid='" + uid + "') and (UserID IS NULL OR UserID='" + uid + "') and ((BeginTime<=getdate() or BeginTime is null) and (EndTime>=getdate() or EndTime is null)) and State=1 and (DeleteMark is null or DeleteMark!=0)");
            return DataFactory.SqlDataBase().GetSingleValueBySQL(sql);
        }
        /// <summary>
        /// 获取会员某个模块下未读的消息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="moduletype"></param>
        /// <returns></returns>
        public DataTable GetNoReadMessagesByUser(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select ID,MessageTitle,MessageContent,MessageType,MessageUrl,ImageUrl,Summary,ReleaseTime from yr_messages where ((BeginTime<=getdate() or BeginTime is null) and (EndTime>=getdate() or EndTime is null)) and State=1 and (DeleteMark is null or DeleteMark!=0) and ID NOT IN (select messageid from yr_messagereadlog where userid='" + uid + "') and (UserID IS NULL OR UserID='" + uid + "')").Append(" order by ReleaseTime desc ");
            return DataFactory.SqlDataBase().GetDataTableBySQL(sql);
        }
        /// <summary>
        /// 删除会员下的消息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="messageid"></param>
        /// <returns></returns>
        public bool DeleteUserMessagesByUser(string uid, string msgID)
        {
            string id = null;
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from yr_messagereadlog where userid='"+uid+"' and messageid='"+msgID+"'");
            DataTable dt= DataFactory.SqlDataBase().GetDataTableBySQL(sql);
            Hashtable ht = YR.Common.DotNetData.DataTableHelper.DataTableToHashtable(dt);
            if (ht.Count==0)
            {
                ht["ID"] = CommonHelper.GetGuid;
                ht["USERID"] = uid;
                ht["MESSAGEID"] = msgID;
                ht["ISREAD"] = 2;
            }
            else
            {
                ht["ISREAD"] = 2;
                id = ht["ID"].ToString();
                ht.Remove("ID");
            }
            return DataFactory.SqlDataBase().Submit_AddOrEdit("yr_messagereadlog", "ID", id, ht);
        }
        /// <summary>
        /// 获取已读消息
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="moduletype"></param>
        /// <returns></returns>
        public DataTable GetReadedMessagesByUser(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select ID,MessageTitle,MessageContent,MessageType,MessageUrl,ImageUrl,Summary,ReleaseTime from yr_messages where ((BeginTime<=getdate() or BeginTime is null) and (EndTime>=getdate() or EndTime is null)) and State=1 and (DeleteMark is null or DeleteMark!=0) and ID IN (select messageid  from yr_messagereadlog where userid='" + uid + "' and IsRead=1) order by ReleaseTime desc");
            return DataFactory.SqlDataBase().GetDataTableBySQL(sql);
        }

        public DataTable GetMessageInfoPageByUser(string uid,string messagetype,int currentPage, int pageNum)
        {
            StringBuilder sql = new StringBuilder();
            int start = (currentPage - 1) * pageNum + 1;
            int end = currentPage * pageNum;
            sql.Append("select * from (");
            sql.Append("select ROW_NUMBER() OVER (ORDER BY ReleaseTime desc) as RowNum,ID,MessageTitle,MessageContent,MessageType,MessageUrl,ImageUrl,Summary,ReleaseTime from dbo.yr_messages where (BeginTime<=getdate() or BeginTime is null) and (EndTime>=getdate() or EndTime is null) and State=1 and (userid is null or userid=@UserID) and (DeleteMark is null or DeleteMark!=0)");
            if(!string.IsNullOrEmpty(messagetype))
            {
                sql.AppendFormat(" and MessageType={0}", messagetype);
            }
            sql.Append(") as UserMessages where  RowNum between ").Append(start.ToString()).Append(" and ").Append(end.ToString());
            SqlParam sqlp = new SqlParam();
            sqlp.FieldName = "@UserID";
            sqlp.FiledValue = uid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { sqlp });
            return dt;
        }

        /// <summary>
        /// 获取已认证但没有邀请码的用户列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetCertifiedUser()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from YR_UserInfo where RealNameCertification=4 and InvitationCode is null");
            return DataFactory.SqlDataBase().GetDataTableBySQL(sql);
        }

        /// <summary>
        /// 文章列表，分页
        /// </summary>
        /// <param name="SqlWhere">SQL条件</param>
        /// <param name="IList_param">参数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总条数</param>
        /// <returns></returns>
        public DataTable GetPitsInfoPage(StringBuilder SqlWhere, IList<SqlParam> IList_param, int pageIndex, int pageSize, ref int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select m.ID,u.User_Name,MessageTitle,MessageContent,
                            case when MessageType=1 then '系统消息' when MessageType=2 then '用户信息' end as MessageType,
                            ReleaseTime,case when State=0 then  '未启用' when State=1 then '启用'end as State,
                            ImageUrl,BeginTime,EndTime,Summary
                            from YR_Messages m 
                            left join [Base_UserInfo] u on m.OperatorID=u.User_ID
                            where 1=1 and (m.DeleteMark!=0 or m.DeleteMark is null ) and MessageType=2  
                            ");

            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "ReleaseTime", "Desc", pageIndex, pageSize, ref count);
        }
    }
}
