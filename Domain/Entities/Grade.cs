namespace TodoWeb.Domain.Entities
{
    public class Grade
    {
        public int GradeId { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int StudentId { get; set; }
        //public virtual StudentCourse StudentCourse { get; set; }
        public Student Student { get; set; }
        public int? ExamId { get; set; }
        public Exam Exam { get; set; }
        public float AssignmentGrade { get; set; }
        public float PracticalGrade { get; set; }
        public float FinalGrade { get; set; }
    }
}
