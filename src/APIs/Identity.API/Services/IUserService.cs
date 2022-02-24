using System;
using System.Threading.Tasks;
using Identity.API.Models;

namespace Identity.API.Services
{
    public interface IUserService
    {
        Task<User> GetBy(int Id);
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
    }
}
