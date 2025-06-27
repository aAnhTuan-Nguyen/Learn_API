namespace TodoWeb.Application.Dtos.StudentDTO
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string SchoolName { get; set; }
        public List<string> Emails { get; set; } = new List<string>();
        public Address Address { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string ZipCode { get; set; }
    }
}
