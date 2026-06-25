using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Common
{
    public sealed record Result(bool success,string? error= null , ResultKind kind = ResultKind.OK)
    {
        public static Result OK() => new(true);
        public static Result Fail(string error, ResultKind kind = ResultKind.Conflict)
            => new(false, error, kind);
        public static Result NotFound(string error = "Resource not found")
            => new(false, error, ResultKind.NotFound);
        public static Result ValidationFailed(string error) => new(false, error, ResultKind.ValidationFailed);

    }
    public sealed  record Result<T>(bool success, T? data, string? error = null, ResultKind kind = ResultKind.OK)
    {
        public static Result<T> OK(T data) => new(true, data);
        public static Result<T> Fail(string error, ResultKind kind = ResultKind.Conflict)
            => new(false, default, error, kind);
        public static Result<T> NotFound(string error = "Resource not found")
            => new(false, default, error, ResultKind.NotFound);
    }
}
