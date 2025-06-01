using Microsoft.EntityFrameworkCore;
using TodoWeb.Application.Dtos;
using TodoWeb.Application.Dtos.StudentDTO;
using TodoWeb.Domain.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Application.Services
{
    public interface IGradeService
    {
        public int Get(int studentID);
        public bool PostGrade(int studentId, int courseId, GradeCreateModel grade);
        public bool PutGrade(int studentId, int courseId, GradeUpdateModel grade);
        public StudentCourseGradeViewModel GetGradeDetail(int studentId);

    }

    public class GradeService : IGradeService
    {
        private readonly IApplicationDbContext _dbContext;
        public GradeService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public int Get(int studentID)
        {
            throw new NotImplementedException();
            
        }
        public StudentCourseGradeViewModel GetGradeDetail(int studentId)
        {
            throw new NotImplementedException();
        }

        public bool PostGrade(int studentId, int courseId, GradeCreateModel grade)
        {
            var data = _dbContext.Grade.Find(studentId, courseId);
            if (data == null)
            {
                var newGrade = new Grade
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    AssignmentGrade = grade.AssignmentGrade,
                    PracticalGrade = grade.PracticalGrade,
                    FinalGrade = grade.FinalGrade
                };
                _dbContext.Grade.Add(newGrade);
            }
            _dbContext.SaveChanges();
            return true;
        }

        public bool PutGrade(int studentId, int courseId, GradeUpdateModel grade)
        {
            var query = _dbContext.Grade.Find(studentId, courseId);
            if (query == null)
            {
                return false;
            }
            query.AssignmentGrade = grade.AssignmentGrade;
            query.PracticalGrade = grade.PracticalGrade;
            query.FinalGrade = grade.FinalGrade;
            //_dbContext.Grade.Update(result);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
