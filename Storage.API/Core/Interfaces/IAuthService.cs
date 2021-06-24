using System;
using System.Threading.Tasks;
using Storage.API.Models;

namespace Storage.API.Core.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<LoginResponse>> Login(LoginRequest login);
    }
}
