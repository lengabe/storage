using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Storage.API.Models
{
    public class ServiceResultError
    {
        public ServiceResultError(string message)
        {
            Message = message;
        }

        public static ServiceResultError Error => new ServiceResultError("ERROR");

        public string Message { get; }
    }

    public class ServiceResult<T> : ServiceResult where T : new()
    {
        public T Value { get; private set; }

        public ServiceResult(IdentityResult result) : base(result)
        {
            Value = new T();
        }

        public ServiceResult(params ServiceResultError[] errors) : base(errors)
        {
        }

        public ServiceResult(bool succeeded) : base(succeeded)
        {
            Succeeded = succeeded;
        }

        public ServiceResult(T value) : base(true)
        {
            Value = value;
        }
    }

    public class ServiceResult
    {
        public bool Succeeded { get; protected init; }
        public List<ServiceResultError> Errors { get; }

        public static ServiceResult Success => new ServiceResult(true);
        public static ServiceResult Unsuccess = new ServiceResult(false);
        public static ServiceResult Error = new ServiceResult(ServiceResultError.Error);
        public ServiceResult(IdentityResult result)
        {
            Succeeded = result.Succeeded;
            Errors = result.Errors.Select(x => new ServiceResultError(x.Description)).ToList();
        }

        public ServiceResult(params ServiceResultError[] errors)
        {
            Succeeded = false;
            Errors = errors.ToList();
        }

        protected ServiceResult(bool succeeded)
        {
            Succeeded = succeeded;
        }
    }
}
