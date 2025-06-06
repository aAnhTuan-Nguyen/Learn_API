using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using TodoWeb.Domain.Entities;
using TodoWeb.Domain.Interface;
using TodoWeb.Infrastructures.DatabaseMapping;
using TodoWeb.Infrastructures.Interceptors;

namespace TodoWeb.Infrastructures
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<School> School { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<StudentCourse> StudentCourse { get; set; }
        public DbSet<Grade> Grade { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Exam> Exam { get; set; }
        public DbSet<ExamQuestion> ExamQuestion { get; set; }
        public DbSet<StudentExamAnswer> StudentExamAnswer { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.
                UseSqlServer("Server=LAPTOP-8DPSPJCR\\SQLEXPRESS;Database=ToDoApp;Trusted_Connection=True;TrustServerCertificate=True");
            
            // tùy vào đặt mà trạng thái nó hoạt động theo thứ tự
            optionsBuilder.
                AddInterceptors(new SqlQueryLoggingInterceptor(), new AuditLogIntercepter());

        }

        // Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .Property(p => p.Age)
                .HasComputedColumnSql("DATEDIFF(YEAR, DateOfBirth, GETDATE())");

            //// define primary key of Course if we don't use [Key] attribute
            //modelBuilder.Entity<Course>()
            //    .HasKey(course => course.Id);

            //modelBuilder.Entity<StudentCourse>()
            //    .HasKey(sc => new { sc.StudentId, sc.CourseId });

            //modelBuilder.Entity<Grade>()
            //    .HasKey(g => new { g.StudentId, g.CourseId , g.ExamId});

            // Course 1--N StudentCourse
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);

            // Course 1--N Exam
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Exams)
                .HasForeignKey(e => e.CourseId);

            // Exam N--N Question (ExamQuestion)
            modelBuilder.Entity<ExamQuestion>()
                .HasKey(eq => new { eq.ExamId, eq.QuestionId });

            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamId);

            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Question)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(eq => eq.QuestionId);

            // Student N--N Exam (StudentExamAnswer)
            modelBuilder.Entity<StudentExamAnswer>()
                .HasKey(sea => new { sea.StudentId, sea.ExamId, sea.QuestionId });

            modelBuilder.Entity<StudentExamAnswer>()
                .HasOne(sea => sea.Student)
                .WithMany(s => s.StudentExamAnswers)
                .HasForeignKey(sea => sea.StudentId);

            modelBuilder.Entity<StudentExamAnswer>()
                .HasOne(sea => sea.Exam)
                .WithMany(e => e.StudentExamAnswers)
                .HasForeignKey(sea => sea.ExamId);

            modelBuilder.Entity<StudentExamAnswer>()
                .HasOne(sea => sea.Question)
                .WithMany(q => q.StudentExamAnswers)
                .HasForeignKey(sea => sea.QuestionId);

            // Student 1--N Grade
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Course)
                .WithMany(c => c.Grades)
                .HasForeignKey(g => g.CourseId);

            // Exam 1--N Grade (optional)
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Exam)
                .WithMany(e => e.Grades)
                .HasForeignKey(g => g.ExamId)
                .IsRequired(false); // ExamId có thể null

            // Cấu hình khóa chính cho các entity
            modelBuilder.Entity<Student>().HasKey(s => s.Id);
            modelBuilder.Entity<Course>().HasKey(c => c.Id);
            modelBuilder.Entity<Question>().HasKey(q => q.QuestionId);
            modelBuilder.Entity<Exam>().HasKey(e => e.ExamId);
            modelBuilder.Entity<Grade>().HasKey(g => g.GradeId);


            // cấu hình riêng cho các entity
            modelBuilder.ApplyConfiguration(new UserMapping());
            modelBuilder.Entity<RefreshToken>().ToTable("RefreshTokens");


            base.OnModelCreating(modelBuilder);
        }
        public int SaveChanges()
        {
            //var auditLogs = new List<AuditLog>();
            //foreach (var entity in ChangeTracker.Entries())
            //{
            //    var log = new AuditLog
            //    {
            //        EntityName = entity.Entity.GetType().Name,
            //        CreatedAt = DateTime.Now,
            //        Action = entity.State.ToString(),
            //    };
            //    if (entity.State == EntityState.Added)
            //    {
            //        log.NewValue = JsonSerializer.Serialize(entity.CurrentValues.ToObject());
            //    }
            //    if (entity.State == EntityState.Modified)
            //    {
            //        log.OldValue = JsonSerializer.Serialize(entity.OriginalValues.ToObject());
            //        log.NewValue = JsonSerializer.Serialize(entity.CurrentValues.ToObject());

            //    }
            //    if (entity.State == EntityState.Deleted)
            //    {
            //        // gox sai thong cam
            //        log.OldValue = JsonSerializer.Serialize(entity.OriginalValues.ToObject());
            //    }
            //    auditLogs.Add(log);

            //}
            //AuditLog.AddRange(auditLogs);
            return base.SaveChanges();
        }
        public EntityEntry<T> Entry<T>(T entity) where T : class
        {
            return base.Entry(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
        //btvn
        //public override EntityEntry Remove(object entity)
        //{
        //    if (entity is ISoftDelete softDeleteEntity)
        //    {
        //        // Fix: Ensure ISoftDelete interface has DeletedBy property
        //        softDeleteEntity.DeletedBy = 1;
        //        softDeleteEntity.DeletedAt = DateTime.Now;
        //        var entry = Entry(entity);
        //        entry.State = EntityState.Modified;
        //        return entry;
        //    }
        //    else
        //    {
        //        return base.Remove(entity);
        //    }
        //}
    }
}
