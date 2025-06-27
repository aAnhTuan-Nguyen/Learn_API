namespace TodoWeb.Domain.Entities
{
    public class ExamQuestion
    {
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        // hai cái khóa ngoại đặt vô nè
        public Exam Exam { get; set; }
        public Question Question { get; set; }
    }
}
