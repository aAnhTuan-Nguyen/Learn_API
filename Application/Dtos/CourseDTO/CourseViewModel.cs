namespace TodoWeb.Application.Dtos.CourseDTO
{
    public class CourseViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public List<GradeViewModel> Grades { get; set; }
}
}
