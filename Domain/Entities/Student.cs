using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoWeb.Domain.Entities
{
    [Table("Students")]
    public class Student
    {
        [Key] // nếu để mặc định thì nó tự đánh identity
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [MaxLength(255)]
        public string? FirstName { get; set; }
        [StringLength(255)]
        [Column("Surname")]
        public string? LastName { get; set; }

        public string Address1 { get; set; }

        //ConcurrencyCheck dùng để kiểm tra tính đồng bộ trên các cột cụ thể.

        //Timestamp dùng để kiểm tra tính đồng bộ của toàn bộ hàng dữ liệu.
        // nên dùng một cái thôi ko bị phí bộ nhớ
        //[ConcurrencyCheck]
        public decimal Balance { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public DateTime DateOfBirth { get; set; }
        
        public int Age { get; set; }

        // Foreign Key (FK)  
        [ForeignKey("School")]
        public int SID { get; set; } // khóa ngoại
        //public int SchoolId { get; set; } // khóa ngoại (để SchoolID vậy thì nó tự hiểu)

        // 1 student chỉ thuộc về 1 school
        public virtual School School { get; set; }

        public virtual ICollection<StudentCourse> StudentCourses { get; set; }

        public ICollection<Grade> Grades { get; set; }

        public virtual ICollection<StudentExamAnswer> StudentExamAnswers { get; set; }

    }
}
