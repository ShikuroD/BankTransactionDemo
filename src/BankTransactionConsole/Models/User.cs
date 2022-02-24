using System;
using System.Text.Json.Serialization;

namespace BankTransactionConsole.Models
{
    public class User
    {
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }   

        public User()
        {
        }
        public User(int id, int accountNumber, string name, string username)
        {
            Id = id;
            AccountNumber = accountNumber;
            Name = name;
            Username = username;
        }
    }
}
