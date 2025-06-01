namespace TodoWeb.Domain.Entities
{
    public class StudentExamAnswer
    {
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public char StudentAnswer { get; set; }
        public Student Student { get; set; }
        public Exam Exam { get; set; }
        public Question Question { get; set; }
    }
}
