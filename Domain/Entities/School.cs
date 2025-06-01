namespace TodoWeb.Domain.Entities
{
    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        // 1 school có nhiều student
        // cái này giúp tạo mối quan hệ giữa 2 bảng
        public virtual ICollection<Student> Students { get; set; }
    }
}
