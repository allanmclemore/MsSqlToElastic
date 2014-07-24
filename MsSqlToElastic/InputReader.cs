
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
                await connection.OpenAsync().ConfigureAwait(false);
                return (await connection.QueryAsync<object>(
                                 request.sql.WithPaging(request.page, request.pagesize),null,null,600,null)
                                 .ConfigureAwait(false)).ToList<object>();
            }
        }
        public static async Task<List<object>> readAll(ReadRequest request)
        {
            using (var connection = new SqlConnection(request.connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                return (await connection.QueryAsync<object>(
                                 request.sql, null, null, 600, null)
                                 .ConfigureAwait(false)).ToList<object>();
            }
        }
        public static string WithPaging(this string sql, int page = 0, int pagesize = 1000)
        {
            var skipRows = page * pagesize;
            var result = new StringBuilder(sql);
            result.Append("\n OFFSET ");
            result.Append(skipRows.ToString());
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
                builder.ConnectTimeout = 30;
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

        public static ReadRequest create(Command command)
        {
            return new ReadRequest()
            {
                db = command.database,
                page = 0,
                pagesize = command.pagesize,
                server = command.dbServer,
                sql = command.sql
            };
        }
    }
}

