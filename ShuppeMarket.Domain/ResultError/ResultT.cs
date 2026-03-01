using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Domain.ResultError
{
    public sealed class Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value)
            : base(true, Error.None)
        {
            Value = value;
        }

        private Result(Error error)
            : base(false, error)
        {
            Value = default;
        }

        public static Result<T> Success(T value)
            => new(value);

        public static Result<T> Fail(Error error)
            => new(error);
    }
}
