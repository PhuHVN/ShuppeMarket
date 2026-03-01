using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Domain.ResultError
{
    public sealed class Error
    {
        public string Code { get; }
        public string Message { get; }

        private Error(string code, string message)
        {
            Code = code;
            Message = message;
        }


        public static readonly Error None =
            new("None", string.Empty);

        public static readonly Error NotFound =
            new("NotFound", "Resource not found");

        public static readonly Error Invalid =
            new("Invalid", "Invalid request");

        public static readonly Error Conflict =
            new("Conflict", "Resource already exists");
    }
}
