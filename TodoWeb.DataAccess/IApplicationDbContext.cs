using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TodoWeb.Domain.Entities;

namespace TodoWeb.Infrastructures
{
    public interface IApplicationDbContext
    {
        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<School> School { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<StudentCourse> StudentCourse { get; set; }
        public DbSet<Grade> Grade { get; set; }
        public EntityEntry<T> Entry<T>(T entity) where T : class;
        public DbSet<AuditLog> AuditLog { get; set; }

        public DbSet<Question> Question { get; set; }
        public DbSet<Exam> Exam { get; set; }
        public DbSet<ExamQuestion> ExamQuestion { get; set; }
        public DbSet<StudentExamAnswer> StudentExamAnswer { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public int SaveChanges();

        public Task<int> SaveChangesAsync();
    }
}
