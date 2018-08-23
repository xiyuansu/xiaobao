
using System.Data;
using System.Text;
using YR.Busines;

namespace Asiasofti.SmartVehicle.Manager
{
    public class AreaManager
    {
        public DataTable GetEnableCityList()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,FullName from YR_Area where Layers=2 and EnabledMark=1");
            DataTable dt = DataFactory.SqlDataBase().GetDataTableBySQL(strSql);
            return dt;
        }
    }
}
