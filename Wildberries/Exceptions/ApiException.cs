using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesDeductions.Wildberries.Exceptions
{
    public class ApiException : Exception
    {
        public string? ErrorCode { get; }
        public ApiException(string code, string message) : base(message)
        {
            ErrorCode = code;
        }
    }
}
