using Microsoft.AspNetCore.Identity;

namespace TodoWeb.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string Salting { get; set; }
        public string FullName { get; set; }
        public Role Role { get; set; }
    }
    public enum  Role
    {
        Admin,
        Teacher,
        Student
    }
}
