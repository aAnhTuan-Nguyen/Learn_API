using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TodoWeb.Application.Dtos.StudentDTO;
using TodoWeb.Domain.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Application.Services
{
    public interface IStudentService
    {
        public IEnumerable<StudentViewModel> GetStudents(int? schoolID);
        public IEnumerable<StudentViewModel> GetStudent();
        public int PostStudent(StudentCreateModel student);
        public bool PutStudent(int id, StudentUpdateModel student);
        public bool DeleteStudnet(int id);
        public StudentDetailViewModel GetStudentDetail(int studentId);
    }
    public class StudentService : IStudentService
    {
        // Giúp giao tiếp với database
        private const string STUDENT_KEY = "StudentKey";
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        public StudentService(IApplicationDbContext dbContext, IMapper mapper, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _cache = cache;
        }

        public bool DeleteStudnet(int id)
        {
            var student = _dbContext.Student.Find(id);
            if (student == null) return false;
            _dbContext.Student.Remove(student);
            _dbContext.SaveChanges();
            return true;
        }

        public StudentDetailViewModel GetStudentDetail(int studentId)
        {

            //var studentCourses = _dbContext.StudentCourse
            //    .Where(studentCourse => studentCourse.StudentId == studentId)
            //    .ToList();
            //var courseViewModels = new List<CourseViewModel>();
            //foreach (var studentCourse in studentCourses)
            //{
            //    var course = _dbContext.Course.Find(studentCourse.CourseId);
            //    //var courseViewModel = new CourseViewModel
            //    //{
            //    //    CourseId = course.Id,
            //    //    CourseName = course.Name,
            //    //    StartDate = course.StartDate
            //    //};
            //    var courseViewModel = _mapper.Map<CourseViewModel>(course);
            //    courseViewModels.Add(courseViewModel);
            //}
            var student = _dbContext.Student.Where(x => x.Id == studentId)
                .Include(x => x.StudentCourses)
                .ThenInclude(x => x.Course)
                .FirstOrDefault();
            //var result = new StudentDetailViewModel
            //{
            //    Id = student.Id,
            //    FullName = $"{student.FirstName} {student.LastName}",
            //    Courses = courseViewModels
            //};

            var result = _mapper.Map<StudentDetailViewModel>(student);
            return result;
        }

        public IEnumerable<StudentViewModel> GetStudents(int? schoolID)
        {
            var query = _dbContext.Student
                .Include(student => student.School)
                .AsQueryable();

            if (schoolID.HasValue)
            {
                query = query.Where(student => student.SID == schoolID);
            }
            var result = _mapper.ProjectTo<StudentViewModel>(query).ToList();
            return result;

            //return query
            //    .Select(student => new StudentViewModel
            //    {
            //        Id = student.Id,
            //        FullName = $"{student.FirstName} {student.LastName}",
            //        Age = student.Age,
            //        SchoolName = student.School.Name
            //    })
            //    .ToList();
        }
        public IEnumerable<StudentViewModel> GetStudent()
        {
            //var data = _cache.Get<IEnumerable<StudentViewModel>>(STUDENT_KEY);
            //if (data == null)
            //{
            //    data = GetAllStudents();
            //    var cacheOption = new MemoryCacheEntryOptions()
            //        .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
            //    _cache.Set(STUDENT_KEY, data, cacheOption);
            //}
            //return data;

            //explain code bellow
            // 1. Check if the data is already in the cache using the key STUDENT_KEY.
            // 2. If the data is not in the cache, create a new cache entry using _cache.GetOrCreate.
            // 3. Set the expiration time for the cache entry to 30 seconds.
            // 4. Call the GetAllStudents method to retrieve the data from the database.
            // 5. Return the data.

            var data = _cache.GetOrCreate(STUDENT_KEY, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
                return GetAllStudents();
            });

            return data;
        }

        private IEnumerable<StudentViewModel> GetAllStudents()
        {
            var students = _dbContext.Student
                     .Include(student => student.School)
                     .ToList();
            var result = _mapper.Map<IEnumerable<StudentViewModel>>(students);
            return result;
        }

        public int PostStudent(StudentCreateModel student)
        {
            if (student == null)
            {
                return -1;
            }
            if(student.Id == 0)
            {
                student.Id = _dbContext.Student.Max(student => student.Id) + 1;
            }
            //if (_dbContext.Student.Any(student => student.Id == student.Id))
            //{
            //    return -1;
            //}
            var newStudent = new Student
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Address1 = student.Address1,
                DateOfBirth = student.DateOfBirth,
                SID = student.SID,
            };
            _dbContext.Student.Add(newStudent);
            _dbContext.SaveChanges();
            return newStudent.Id;
        }
        public bool PutStudent(int id, StudentUpdateModel student)
        {
            var query = _dbContext.Student.Find(id);
            if (query == null) return false;
            query.FirstName = student.FirstName;
            query.LastName = student.LastName;
            query.Address1 = student.Address1;
            query.SID = student.SID;
            query.Balance = student.Balance;
            _dbContext.SaveChanges();
            return true;
        }
    }
}
