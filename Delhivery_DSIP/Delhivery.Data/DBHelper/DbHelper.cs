using Delhivery.Data.Constants;
using System.Data.SqlClient;

namespace Delhivery.Data.DBHelper
{
    public class DbHelper
    {
        public SqlConnection GetConnection()
        {
            return new SqlConnection(DbConstants.ConnectionString);
        }
    }
}