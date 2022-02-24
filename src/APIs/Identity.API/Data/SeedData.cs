using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.API.Models;

namespace Identity.API.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IdentityContext context)
        {
            context.Database.EnsureCreated();

            //seed AccountSummaries
            if (!context.Users.Any())
            {
                var users = new List<User> {
                    new User {AccountNumber = 1,
                            Name = "test 1",
                            Username =  "test1",
                            Password = "12345"},
                    new User {AccountNumber = 2,
                            Name = "test 2",
                            Username =  "test2",
                            Password = "12345"},
                    new User {AccountNumber = 3,
                            Name = "test 3",
                            Username =  "test3",
                            Password = "12345"}
                };
                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }
        }
    }
}
