using System;
using System.Collections.Generic;
using System.Text;
using YR.DataBase;
using YR.Busines;
using System.Data;
using System.Collections;

namespace Asiasofti.SmartVehicle.Manager
{
    public class MessageReadLogManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messagereadlog"></param>
        public int AddMessageReadLog(Hashtable messagereadlog)
        {
            return DataFactory.SqlDataBase().InsertByHashtable("YR_MessageReadLog", messagereadlog);
        }

        public bool IsUserReadMessages(string uid, string msgID)
        {
            bool isRead = false;
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from yr_messagereadlog where userid='" + uid + "' and messageid='" + msgID + "'");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(sql);
            if(dt.Rows.Count>0)
            {
                isRead = true;
            }
            return isRead;
        }

    }
}
