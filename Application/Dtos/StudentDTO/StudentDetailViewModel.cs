using TodoWeb.Application.Dtos.CourseDTO;

namespace TodoWeb.Application.Dtos.StudentDTO
{
    public class StudentDetailViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public List<CourseViewModel> Courses { get; set; }

    }
}
