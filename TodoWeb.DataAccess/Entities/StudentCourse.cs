namespace TodoWeb.Domain.Entities
{
    public class StudentCourse
    {
        public int StudentId { get; set; }
        public virtual Student Student { get; set; } // hai cái này là khóa ngoại
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }// hai cái này là khóa ngoại

        //public virtual ICollection<Grade> Grades { get; set; }
    }
}
