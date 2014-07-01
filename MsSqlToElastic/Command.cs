using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSqlToElastic
{
    public class Command
    {
        public Command(string[] args)
        {
            _args = args;
            loadParameters(args);
        }
        public bool isHelp()
        {
            return (_args.Length == 2) && (_args[1].ToLower() == "-help");
        }
        public bool isPaging()
        {
            return pagesize > 0;
        }
        public bool isValid()
        {
            foreach (var key in (keys[])Enum.GetValues(typeof(keys)))
            {
                if (!parameters.ContainsKey(key.ToString())
                    || parameters[key.ToString()] == null
                    || parameters[key.ToString()].Trim() == string.Empty)
                {
                    if (!optional(key))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        public string database
        {
            get
            {
                return val(keys.database);
            }
        }
        public string dbServer
        {
            get
            {
                return val(keys.dbserver);
            }
        }
        public string elasticUrl
        {
            get
            {
                return val(keys.elasticurl);
            }
        }
        public string index
        {
            get
            {
                return val(keys.index);
            }
        }
        public int pagesize
        {
            get
            {
                int size;
                if (Int32.TryParse(val(keys.pagesize), out size))
                {
                    return size;
                }
                else
                {
                    return 0;
                }
            }
        }
        public string sql
        {
            get
            {
                return val(keys.sql);
            }
        }
        private string formatKey(string key)
        {
            if ((key.Length > 1) && (key[0] == '-'))
            {
                return key.Substring(1).ToLower();
            }
            else
            {
                throw new Exception("Invalid command. For commadn usage type: indexSql -help");
            }

        }
        private void loadParameters(string[] args)
        {
            for (int i = 0; i < args.Length; i = i + 2)
            {
                var switchHasValue = args.Length >= i + 1;
                if (switchHasValue)
                {
                    parameters.Add(formatKey(args[i]), args[i + 1]);
                }
            }
        }
        private bool optional(keys key)
        {
            return key == keys.pagesize;
        }
        private string val(keys key)
        {
            if (parameters.ContainsKey(key.ToString()))
            {
                return parameters[key.ToString()];
            }
            else
            {
                return string.Empty;
            }
        }
        enum keys
        {
            dbserver,
            database,
            sql,
            elasticurl,
            index,
            pagesize
        }
        readonly Dictionary<string, string> parameters = new Dictionary<string, string>();
        readonly string[] _args;
    }
}

