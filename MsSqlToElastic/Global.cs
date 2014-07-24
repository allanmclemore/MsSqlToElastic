using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSqlToElastic
{
    public static class Global
    {
        public const string INVALID_COMMAND = "INVALID COMMAND";

        public static bool isInvalidCommand(this ArgumentException e)
        {
            return e.Message.Equals(INVALID_COMMAND, StringComparison.OrdinalIgnoreCase);
        }

        public static bool isOdd(this int i)
        {
            return i % 2 != 0;
        }

        public static bool isEmpty(this string[] array)
        {
            return array == null || array.Length == 0;
        }
        public static bool isNullEmptyOrSpaces(this string str)
        {
            return str == null || str.Trim().Length == 0;
        }
        public static bool hasData(this BulkOutputRequest request)
        {
            return request != null && request.documents != null && request.documents.Count() > 0;
        }


    }
}
