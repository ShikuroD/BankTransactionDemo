using System;
using System.Text.Json.Serialization;

namespace Identity.API.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }   
        [JsonIgnore]
        public string Password { get; set; }

        public UserDto()
        {
        }
        public UserDto(int id, int accountNumber, string name, string username, string password)
        {
            Id = id;
            AccountNumber = accountNumber;
            Name = name;
            Username = username;
            Password = password;
        }
    }
}
