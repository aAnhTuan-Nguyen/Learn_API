using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoWeb.Application.Dtos;
using TodoWeb.Application.Dtos.CourseDTO;
using TodoWeb.Application.Dtos.StudentDTO;
using TodoWeb.Domain.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Application.Services
{
    public interface ICourseService
    {
        public IEnumerable<CourseViewModel> GetCourses(int? id);
        public CourseDetailViewModel GetCourseDetail(int id);
        public Task<int> PostCourse(CourseCreateModel course);
        public bool PutCourse(int id, CourseUpdateModel course);
        public bool DeleteCourse(int id);
        public bool AssignCourseToStudent(int courseId, int studentId);
    }

    public class CourseService : ICourseService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public CourseService(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public bool DeleteCourse(int id)
        {
            var data = _dbContext.Course.Find(id);
            if (data == null)
            {
                return false;
            }
            _dbContext.Course.Remove(data);
            _dbContext.SaveChanges();
            return true;
        }

        public CourseDetailViewModel GetCourseDetail(int id)
        {
            var course = _dbContext.Course.Find(id);
            if (course == null)
            {
                return null;
            };
            var students = _dbContext.Student
                .Where(x => x.StudentCourses.Any(y => y.CourseId == id))
                .Select(x => new StudentViewModel
                {
                    Id = x.Id,
                    FullName = $"{x.FirstName} {x.LastName}"
                }).ToList();
            var result = new CourseDetailViewModel
            {
                Id = course.Id,
                CourseName = course.Name,
                StartDate = course.StartDate,
                Students = students
            };
            return result;
        }

        public IEnumerable<CourseViewModel> GetCourses(int? id)
        {
            var query = _dbContext.Course;

            //List<Course> course = query.ToList();

            //var result = course.
            //    Select(course => _mapper.Map<CourseViewModel>(course))
            //    .ToList();

            // này map hết nè
            //var result = _mapper.Map<List<CourseViewModel>>(course);
            // ProjectTo giups map đúng cái mình cần

            List<CourseViewModel> result = _mapper.ProjectTo<CourseViewModel>(query).ToList();

            return result;
        }

        public async Task<int> PostCourse(CourseCreateModel course)
        {
            //var query = _dbContext.Course;
            if ( course == null)
            {
                return 0;
            }
            //var data = new Course
            //{
            //    Name = course.CourseName,
            //    StartDate = course.StartDate
            //};
            //var result = _mapper.ProjectTo<CourseCreateModel>(query).ToList();
            //List<Course> course = query.ToList();

            var result = _mapper.Map<Course>(course);

            _dbContext.Course.Add(result);
            await _dbContext.SaveChangesAsync();
            return result.Id;
        }

        public bool PutCourse(int id, CourseUpdateModel course) // src
        {
            var data = _dbContext.Course.Find(id);
            if (data == null)// dest
            {
                return false;
            }

            //data.Name = course.CourseName;
            //data.StartDate = course.StartDate;

            _mapper.Map(course, data);

            _dbContext.SaveChanges();
            return true;
        }
        public bool AssignCourseToStudent(int courseId, int studentId)
        {
            var course = _dbContext.Course.Find(courseId);
            var student = _dbContext.Student.Find(studentId);
            if (course == null || student == null)
            {
                return false;
            }
            var studentCourse = new StudentCourse
            {
                CourseId = courseId,
                StudentId = studentId
            };
            _dbContext.StudentCourse.Add(studentCourse);
            _dbContext.SaveChanges();
            return true;
        }

    }
}
