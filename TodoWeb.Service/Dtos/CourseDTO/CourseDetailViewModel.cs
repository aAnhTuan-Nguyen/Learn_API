using TodoWeb.Application.Dtos.StudentDTO;

namespace TodoWeb.Application.Dtos.CourseDTO
{
    public class CourseDetailViewModel
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public List<StudentViewModel> Students { get; set; }
    }
}
