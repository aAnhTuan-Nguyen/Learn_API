using TodoWeb.Application.Dtos.StudentDTO;

namespace TodoWeb.Application.Dtos.SchoolDTO
{
    public class SchoolStudentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public IEnumerable<StudentViewModel> Students { get; set; }
    }
}
