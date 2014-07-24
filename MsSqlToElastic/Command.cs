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
            
            if (isHelp() || isReadme())
            {
                return;
            }

            if(_args.isEmpty() || _args.Length.isOdd())
            {
                throw new ArgumentException(Global.INVALID_COMMAND);
            }

            loadParameters();
        }
        public bool isHelp()
        {
            return (_args.Length == 1) && (_args[0].ToLower() == "-help");
        }
        public bool isReadme()
        {
            return (_args.Length == 1) && (_args[0].ToLower() == "-readme");
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
        public string id
        {
            get
            {
                return val(keys.id);
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
                const int DEFAULT_SQL_PAGE_SIZE = 1000;
                int size;
                if (Int32.TryParse(val(keys.pagesize), out size))
                {
                    return size;
                }
                else
                {
                    return DEFAULT_SQL_PAGE_SIZE;
                }
            }
        }
        public bool isRecreateIndex
        {
            get
            {
                return string.Equals(val(keys.append), "false", StringComparison.OrdinalIgnoreCase);
            }
        }

        public string sql
        {
            get
            {
                return val(keys.sql);
            }
        }
        public string type
        {
            get
            {
                const string DEFAULT_TYPE = "object";
                if(val(keys.type).isNullEmptyOrSpaces())
                {
                    return DEFAULT_TYPE;
                }
                return val(keys.type);
            }
        }
        public static string syntax()
        {
            var result = new StringBuilder();
            foreach (var key in (keys[])Enum.GetValues(typeof(keys)))
            {

                result.Append(" -");
                result.Append(key.ToString());
                result.Append(" [");
                if (optional(key))
                {
                    result.Append("optional ");
                }
                result.Append("value");
                result.Append("]");
            }
            return result.ToString();
        }
        private string formatKey(string key)
        {
            if ((key.Length > 1) && (key[0] == '-'))
            {
                return key.Substring(1).ToLower();
            }
            else
            {
                throw new ArgumentException(Global.INVALID_COMMAND);
            }

        }
        private void loadParameters()
        {
            for (int i = 0; i < _args.Length; i = i + 2)
            {
                var switchHasValue = _args.Length >= i + 1;
                if (switchHasValue)
                {
                    parameters.Add(formatKey(_args[i]), _args[i + 1]);
                }
            }
        }
        public static bool optional(keys key)
        {
            return (key == keys.append) ||
                    (key == keys.pagesize) || 
                    (key == keys.type) ||
                    (key == keys.id);
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
        public enum keys
        {
            dbserver,
            database,
            sql,
            elasticurl,
            index,
            append,
            pagesize,
            type,
            id
        }
        readonly Dictionary<string, string> parameters = new Dictionary<string, string>();
        readonly string[] _args;

    }
}

