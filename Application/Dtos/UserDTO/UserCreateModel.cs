namespace TodoWeb.Application.Dtos.UserDTO
{
    public class UserCreateModel
    {
        //public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public Role Role { get; set; }
    }
    public enum Role
    {
        Admin,
        Teacher,
        Stud
    }
}
