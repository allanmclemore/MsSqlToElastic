using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSqlToElastic
{
    class Program
    {
        static void Main(string[] args)
        {
            var command = new Command(args);
        
            if (command.isValid())
            {
                process(command);
            }
            else
            {
                showHelp();
            }
        }
        static void process(Command command)
        {
            if (command.isPaging())
            {
                executeWithPaging(command);
            }
            else
            {
                execute(command);
            }
        }
        static void execute(Command command)
        {
            var data = InputReader.readAll(createReadRequest(command)).Result;
            var writer = new OutputWriter(command.elasticUrl, command.index);
            writer.RecreateIndex();
            writer.BulkUpsert(data);
        }
        static void executeWithPaging(Command command)
        {
            var writer = new OutputWriter(command.elasticUrl, command.index);
            writer.RecreateIndex();
            var request = createReadRequest(command);
            var data = InputReader.read(request).Result;
            while (data.Count > 0)
            {
                writer.BulkUpsert(data);
                request.page++;
                data = InputReader.read(request).Result;
            }
        }
        static ReadRequest createReadRequest(Command command)
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
        static bool isHelp(string[] args)
        {
            return (args.Length == 2) && (args[1].ToLower() == "-help");
        }
        static void showHelp()
        {
            Console.WriteLine("Syntax:");
            Console.WriteLine();
            Console.WriteLine("MsSqlToElastic -dbserver [server] -database [database] -sql [sql select statement] -elasticurl [url] -index [index] -pagesize [sql page size]");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine();
            Console.WriteLine("MsSqlToElastic -dbserver localhost -database MyDb -sql \"Select id, firstname, lastname, phone from customers order by id\" -elasticurl \"http://localhost:9200\" -index customers -pagesize 10000");
            Console.WriteLine();
        }
    }
}
