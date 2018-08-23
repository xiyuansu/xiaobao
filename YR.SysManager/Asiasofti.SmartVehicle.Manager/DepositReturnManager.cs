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
    public class DepositReturnManager
    {
        /// <summary>
        /// 添加或修改信息
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public bool AddOrEdit(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().Submit_AddOrEdit("YR_DepositReturn", "ID", _key, ht);
        }

        /// <summary>
        /// 通过主键ID删除信息(逻辑删除)
        /// </summary>
        /// <param name="ht"></param>
        /// <param name="_key"></param>
        /// <returns></returns>
        public int Delete(Hashtable ht, string _key)
        {
            return DataFactory.SqlDataBase().UpdateByHashtable("YR_DepositReturn", "ID", _key, ht);
        }

        /// <summary>
        /// 通过ID获取信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Hashtable GetDataByID(string id)
        {
            //return DataFactory.SqlDataBase().GetHashtableById("YR_DepositReturn", "ID", id);

            StringBuilder sql = new StringBuilder();
            sql.Append(@"select a.*,b.RealName,b.BindPhone from YR_DepositReturn a join YR_UserInfo b on a.userid=b.id where a.ID=@ID");
            SqlParam param = new SqlParam();
            param.FieldName = "@ID";
            param.FiledValue = id;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
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
            strSql.Append(@"select a.*,b.RealName,b.BindPhone from YR_DepositReturn a join YR_UserInfo b on a.userid=b.id where 1=1");
            strSql.Append(SqlWhere);
            return DataFactory.SqlDataBase().GetPageList(strSql.ToString(), IList_param.ToArray(), "CreateTime", "desc", pageIndex, pageSize, ref count);
        }

        /// <summary>
        /// 获取所有退款申请信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllData()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"select * from YR_DepositReturn order by CreateTime");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }

        /// <summary>
        /// 申请押金退款
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool ApplyReturnDeposit(string uid)
        {
            List<StringBuilder> sqls = new List<StringBuilder>();
            List<object> objs = new List<object>();

            Hashtable deposit_ht = GetUserDeposit(uid);
            if (deposit_ht == null || deposit_ht.Keys.Count == 0)
                return false;

            string buyer_id = SiteHelper.GetHashTableValueByKey(deposit_ht, "buyer_id");
            string buyer_logon_id = SiteHelper.GetHashTableValueByKey(deposit_ht, "buyer_logon_id");
            StringBuilder sbInsertApply = new StringBuilder();
            if (!string.IsNullOrEmpty(buyer_id) && !string.IsNullOrEmpty(buyer_logon_id))
            {
                sbInsertApply.Append("insert into YR_DepositReturn(ID,UserID,State,DepositMoney,DepositPayWay,DepositTradeNo,CreateTime,buyer_id,buyer_logon_id)values(@ID,@UserID,@State,@DepositMoney,@DepositPayWay,@DepositTradeNo,@CreateTime,@buyer_id,@buyer_logon_id)");
                SqlParam[] parmInsertApply = new SqlParam[] {
                        new SqlParam("@ID", CommonHelper.GetGuid),
                        new SqlParam("@UserID", uid),
                        new SqlParam("@State", 1),
                        new SqlParam("@DepositMoney", deposit_ht["CHANGESAMOUNT"]),
                        new SqlParam("@DepositPayWay", deposit_ht["OPERATORWAY"]),
                        new SqlParam("@DepositTradeNo", deposit_ht["TRADENO"]),
                        new SqlParam("@CreateTime", SiteHelper.GetWebServerCurrentTime()),
                        new SqlParam("@buyer_id", buyer_id),
                        new SqlParam("@buyer_logon_id", buyer_logon_id)
                    };
                sqls.Add(sbInsertApply);
                objs.Add(parmInsertApply);
            }
            else
            {
                sbInsertApply.Append("insert into YR_DepositReturn(ID,UserID,State,DepositMoney,DepositPayWay,DepositTradeNo,CreateTime)values(@ID,@UserID,@State,@DepositMoney,@DepositPayWay,@DepositTradeNo,@CreateTime)");
                SqlParam[] parmInsertApply = new SqlParam[] {
                        new SqlParam("@ID", CommonHelper.GetGuid),
                        new SqlParam("@UserID", uid),
                        new SqlParam("@State", 1),
                        new SqlParam("@DepositMoney", deposit_ht["CHANGESAMOUNT"]),
                        new SqlParam("@DepositPayWay", deposit_ht["OPERATORWAY"]),
                        new SqlParam("@DepositTradeNo", deposit_ht["TRADENO"]),
                        new SqlParam("@CreateTime", SiteHelper.GetWebServerCurrentTime())
                    };
                sqls.Add(sbInsertApply);
                objs.Add(parmInsertApply);
            }

            StringBuilder sbUpdateUser = new StringBuilder();
            sbUpdateUser.Append("update YR_UserInfo set Deposit=@Deposit where ID=@UserID");
            SqlParam[] parmUpdateUser = new SqlParam[] {
                        new SqlParam("@UserID", uid),
                         new SqlParam("@Deposit", 0)
                    };
            sqls.Add(sbUpdateUser);
            objs.Add(parmUpdateUser);

            // 发送短消息
            StringBuilder msgAdd = new StringBuilder();
            msgAdd.Append("Insert into YR_Messages(");
            msgAdd.Append("ID,OperatorID,UserID,MessageTitle,MessageContent,MessageType,ModuleType,ReleaseTime,state) ");
            msgAdd.Append(" values(@ID,@OperatorID,@UserID,@MessageTitle,@MessageContent,@MessageType,@ModuleType,@ReleaseTime,@State)");
            StringBuilder sbMsg = new StringBuilder();
            sbMsg.Append("您的退款申请已提交,我们将会尽快为您处理，请您耐心等待.");
            SqlParam[] parmMsgAdd = new SqlParam[] {
                                     new SqlParam("@ID", CommonHelper.GetGuid),
                                     new SqlParam("@OperatorID", ""),
                                     new SqlParam("@UserID", uid),
                                     new SqlParam("@MessageTitle", "押金退款申请"),
                                     new SqlParam("@MessageContent",sbMsg.ToString()),
                                     new SqlParam("@MessageType", MessageType.System.GetHashCode()),
                                     new SqlParam("@ModuleType", MessageModuleType.UseVehicle.GetHashCode()),
                                     new SqlParam("@ReleaseTime",SiteHelper.GetWebServerCurrentTime()),
                                     new SqlParam("@State", MessageState.Enable.GetHashCode())};
            sqls.Add(msgAdd);
            objs.Add(parmMsgAdd);

            return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
        }

        /// <summary>
        /// 处理押金退款
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="remark"></param>
        /// <param name="operatorname"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool HandleReturnDeposit(string id, string state, string remark, string operatorname, int returntype = 0)
        {
            List<StringBuilder> sqls = new List<StringBuilder>();
            List<object> objs = new List<object>();

            Hashtable deposit_ht = GetDataByID(id);
            if (deposit_ht == null || deposit_ht.Keys.Count == 0)
                return false;

            StringBuilder sbUpdateApply = new StringBuilder();
            sbUpdateApply.Append("update YR_DepositReturn set State=@State,Remark=@Remark,UpdateTime=@UpdateTime,Operator=@Operator,ReturnType=@ReturnType where ID=@ID");
            SqlParam[] parmUpdateApply = new SqlParam[] {
                        new SqlParam("@ID", id),
                        new SqlParam("@State", state),
                        new SqlParam("@Remark", remark),
                        new SqlParam("@UpdateTime", SiteHelper.GetWebServerCurrentTime()),
                        new SqlParam("@Operator", operatorname),
                        new SqlParam("@ReturnType", returntype)
                    };
            sqls.Add(sbUpdateApply);
            objs.Add(parmUpdateApply);

            if (state == "3")
            {
                StringBuilder sbUpdateUser = new StringBuilder();
                sbUpdateUser.Append("update YR_UserInfo set Deposit=@Deposit where ID=@UserID");
                SqlParam[] parmUpdateUser = new SqlParam[] {
                        new SqlParam("@UserID", deposit_ht["USERID"]),
                         new SqlParam("@Deposit", deposit_ht["DEPOSITMONEY"])
                    };
                sqls.Add(sbUpdateUser);
                objs.Add(parmUpdateUser);
            }

            // 发送短消息
            StringBuilder msgAdd = new StringBuilder();
            msgAdd.Append("Insert into YR_Messages(");
            msgAdd.Append("ID,OperatorID,UserID,MessageTitle,MessageContent,MessageType,ModuleType,ReleaseTime,state) ");
            msgAdd.Append(" values(@ID,@OperatorID,@UserID,@MessageTitle,@MessageContent,@MessageType,@ModuleType,@ReleaseTime,@State)");
            SqlParam[] parmMsgAdd = new SqlParam[] {
                                     new SqlParam("@ID", CommonHelper.GetGuid),
                                     new SqlParam("@OperatorID", ""),
                                     new SqlParam("@UserID", deposit_ht["USERID"]),
                                     new SqlParam("@MessageTitle", "押金退款结果"),
                                     new SqlParam("@MessageContent",state=="2"?"退款操作成功":"退款操作成功失败："+remark),
                                     new SqlParam("@MessageType", MessageType.System.GetHashCode()),
                                     new SqlParam("@ModuleType", MessageModuleType.UseVehicle.GetHashCode()),
                                     new SqlParam("@ReleaseTime",SiteHelper.GetWebServerCurrentTime()),
                                     new SqlParam("@State", MessageState.Enable.GetHashCode())};
            sqls.Add(msgAdd);
            objs.Add(parmMsgAdd);

            return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
        }

        public bool HandleReturnedDeposit(string id, string operatorname)
        {
            List<StringBuilder> sqls = new List<StringBuilder>();
            List<object> objs = new List<object>();
            Hashtable deposit_ht = GetDataByID(id);
            if (deposit_ht == null || deposit_ht.Keys.Count == 0)
                return false;
            StringBuilder sbUpdateApply = new StringBuilder();
            sbUpdateApply.Append("update YR_DepositReturn set State=@State,Remark=@Remark,UpdateTime=@UpdateTime,Operator=@Operator,ReturnType=@ReturnType where ID=@ID");
            SqlParam[] parmUpdateApply = new SqlParam[] {
                        new SqlParam("@ID", id),
                        new SqlParam("@State", 2),
                        new SqlParam("@Remark", "已通过其他方式退款"),
                        new SqlParam("@UpdateTime", SiteHelper.GetWebServerCurrentTime()),
                        new SqlParam("@Operator", operatorname),
                        new SqlParam("@ReturnType", 1)
                    };
            sqls.Add(sbUpdateApply);
            objs.Add(parmUpdateApply);

            StringBuilder sbUpdateUser = new StringBuilder();
            sbUpdateUser.Append("update YR_UserInfo set Deposit=@Deposit where ID=@UserID");
            SqlParam[] parmUpdateUser = new SqlParam[] {
                        new SqlParam("@UserID", deposit_ht["USERID"]),
                         new SqlParam("@Deposit",0)
                    };
            sqls.Add(sbUpdateUser);
            objs.Add(parmUpdateUser);
            return DataFactory.SqlDataBase().BatchExecuteBySqlWithTrans(sqls.ToArray(), objs.ToArray());
        }

        /// <summary>
        /// 根据用户id获取押金缴纳信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Hashtable GetUserDeposit(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT YR_UserFinancial.* FROM YR_UserInfo join YR_UserFinancial on YR_UserInfo.ID=YR_UserFinancial.UserID and YR_UserInfo.Deposit>0 and YR_UserFinancial.ChangesType=5 and YR_UserFinancial.State=2 and YR_UserInfo.ID=@UserID order by YR_UserFinancial.ChangesTime desc");
            SqlParam param = new SqlParam();
            param.FieldName = "@UserID";
            param.FiledValue = uid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
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
        /// 根据用户id获取退款申请信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Hashtable GetAppyInfoByUid(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select * from YR_DepositReturn where UserID=@UserID and State=1 order by CreateTime desc");
            SqlParam param = new SqlParam();
            param.FieldName = "@UserID";
            param.FiledValue = uid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
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
        /// 根据用户id获取是否为首次缴纳押金
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool IsFirstDeposit(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT 1 FROM YR_UserFinancial WHERE ChangesType=5 and State=2 and UserID=@UserID");
            SqlParam param = new SqlParam();
            param.FieldName = "@UserID";
            param.FiledValue = uid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据会员id获取当前押金退款状态
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>返回值：1处理中,2退款成功,3退款失败</returns>
        public string GetDepositState(string uid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT TOP 1 State FROM YR_DepositReturn join YR_UserInfo on YR_DepositReturn.userid=YR_UserInfo.id where YR_UserInfo.deposit=0 and UserID=@UserID order by createtime desc");
            SqlParam param = new SqlParam();
            param.FieldName = "@UserID";
            param.FiledValue = uid;
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql, new SqlParam[] { param });
            if (dt == null || dt.Rows.Count <= 0)
            {
                return null;
            }
            else
            {
                return dt.Rows[0]["State"].ToString();
            }
        }
    }
}
