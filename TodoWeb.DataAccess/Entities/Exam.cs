namespace TodoWeb.Domain.Entities
{
    public class Exam
    {
        public int ExamId { get; set; }
        public int CourseId { get; set; }
        public string ExamName { get; set; }

        // 1 kiểm tra thì chỉ cho 1 khóa học
        public Course Course { get; set; }
        // một đề thì có nhiều câu hỏi
        public ICollection<ExamQuestion> ExamQuestions { get; set; }
        public ICollection<StudentExamAnswer> StudentExamAnswers { get; set; }
        public ICollection<Grade> Grades { get; set; }
    }
}
