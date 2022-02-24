using System;
using System.Linq;
using System.Threading.Tasks;
using Identity.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly IdentityContext _context;

        public UserRepository(IdentityContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetUser(string username, string password)
        {
            User user = null;
            try
            {
                user = (await _context.Users.ToListAsync())
                .Where(m => m.Username.Equals(username) && m.Password.Equals(password))
                .Single();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            return user;

        }
    }
}
