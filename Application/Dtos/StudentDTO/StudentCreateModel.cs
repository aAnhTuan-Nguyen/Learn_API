using System.ComponentModel.DataAnnotations;

namespace TodoWeb.Application.Dtos.StudentDTO
{
    public class StudentCreateModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address1 { get; set; }
        public int SID { get; set; }
    }
}
