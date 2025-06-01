using TodoWeb.Domain.Interface;

namespace TodoWeb.Domain.Entities
{
    public class Course : ISoftDelete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public virtual ICollection<StudentCourse> StudentCourses { get; set; }

        //public int CreatedBy { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public int UpdatedBy { get; set; } 
        //public DateTime UpdatedAt { get; set; } 

        public int DeletedBy { get; set; } // hai cái này có thể null
        public DateTime DeletedAt { get; set; } // hai cái này có thể null
        public bool IsDeleted { get; set; }

        public ICollection<Grade> Grades { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
    }
}
