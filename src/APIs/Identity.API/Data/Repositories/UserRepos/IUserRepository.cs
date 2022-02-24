using System;
using System.Threading.Tasks;
using Identity.API.Models;

namespace Identity.API.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUser(string username, string password);
    }
}
