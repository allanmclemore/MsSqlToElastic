using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using help = MsSqlToElastic.HelpWriter;
using reader = MsSqlToElastic.InputReader;
using Nest;

namespace MsSqlToElastic
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                execute(new Command(args));
            }
            catch (ArgumentException e)
            {
                if (e.isInvalidCommand())
                {
                    help.write();
                }
            }
        }
        static void execute(Command command)
        {
            if (command.isValid())
            {
                doWork(command);
            }
            else if (command.isReadme())
            {
                help.writeVerbose();
            }
            else
            {
                help.write();
            }
        }
        static void doWork(Command command)
        {
            var query = ReadRequest.create(command);
            var output = BulkOutputRequest.create(command);
            output.documents = InputReader.read(query).Result;
            var writer = new OutputWriter();
            if (command.isRecreateIndex)
            {
                writer.ResetIndex(output);
            }

            while (output.hasData())
            {
                writer.write(output);
                query.page++;
                output.documents = InputReader.read(query).Result;
            }
        }
    }
}
