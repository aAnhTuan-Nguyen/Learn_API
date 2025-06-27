namespace TodoWeb.Application.Dtos.StudentDTO
{
    public class StudentCourseGradeViewModel
    {
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public ICollection<GradeViewModel> Grades { get; set; }
    }
}
