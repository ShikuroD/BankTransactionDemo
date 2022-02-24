using System;

namespace Identity.API.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse(User user, string token)
        {
            Id = user.Id;
            Name = user.Name;
            AccountNumber = user.AccountNumber;
            Username = user.Username;
            Token = token;
        }
    }
}
