
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;

namespace MsSqlToElastic
{
    public static class InputReader
    {
        public static async Task<List<object>> read(ReadRequest request)
        {
            using (var connection = new SqlConnection(request.connectionString))
            {
                return (await connection.QueryAsync<object>(
                                 request.sql.WithPaging(request.page, request.pagesize),null,null,600,null)
                                 .ConfigureAwait(false)).ToList<object>();
            }
        }
        public static async Task<List<object>> readAll(ReadRequest request)
        {
            using (var connection = new SqlConnection(request.connectionString))
            {
                return (await connection.QueryAsync<object>(
                                 request.sql, null, null, 600, null)
                                 .ConfigureAwait(false)).ToList<object>();
            }
        }
        public static string WithPaging(this string sql, int page = 0, int pagesize = 1000)
        {
            var result = new StringBuilder(sql);
            result.Append("\n OFFSET ");
            result.Append(page.ToString());
            result.Append(" ROWS \n FETCH NEXT ");
            result.Append(pagesize.ToString());
            result.Append(" ROWS ONLY;");
            return result.ToString();
        }
    }
    public class ReadRequest
    {
        public string connectionString
        {
            get
            {
                var builder = new SqlConnectionStringBuilder();
                builder.DataSource = server;
                builder.InitialCatalog = db;
                builder.IntegratedSecurity = true;
                builder.AsynchronousProcessing = true;
                return builder.ConnectionString;
            }
        }
        public string db { get; set; }
        public int page { get; set; }
        public int pagesize { get; set; }
        public string server { get; set; }
        public string sql { get; set; }
    }
}

