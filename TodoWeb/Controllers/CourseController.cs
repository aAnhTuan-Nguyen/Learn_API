using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoWeb.Application.ActionFillter;
using TodoWeb.Application.Dtos.CourseDTO;
using TodoWeb.Application.Services;

namespace TodoWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(LogFillter), Arguments = [LogLevel.Warning])]
    [AuditFillter]
    [Authorize]
    // type filter khác service filter chổ nào
    // service filter là mình sẽ add vô DIContainer(file program)
    // và nó sẽ là add singleton
    // còn cái kia là tạo mới mỗi instance như addScoped

    // bình thường ko có gì đặc biệt thì dùng ServiceFilter
    // còn TypeFilter dùng khi muốn truyền argument

    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseController> _logger;
        public CourseController(ICourseService courseService, 
            ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }
        [HttpGet]
        public IEnumerable<CourseViewModel> Get(int? id)
        {
            //_logger.LogInformation("Get course id: " + id);

            //if (id == 10)
            //{
            //    _logger.LogWarning("Warning id: " + id);
            //}
            //if (id < 0)
            //{
            //    _logger.LogError("Error id can not small than 0");
            //    throw new Exception("Error id can not small than 0");
            //}
            return _courseService.GetCourses(id);
        }
        
        [TypeFilter(typeof(CacheFillter), Arguments = [10])]
        [HttpGet("{id}")]
        public CourseDetailViewModel GetCourseDetail(int id)
        {
            _logger.LogInformation("Get course id: " + id);

            if (id == 10)
            {
                _logger.LogWarning("Warning id: " + id);
            }
            if (id < 0)
            {
                _logger.LogError("Error id can not small than 0");
                throw new Exception("Error id can not small than 0");
            }
            return _courseService.GetCourseDetail(id);
        }

        // I/O bound, CPU bound
        // 
        [HttpPost]
        public async Task<int> Post(CourseCreateModel course)
        {
            return await _courseService.PostCourse(course);
        }

        [HttpPut]
        public bool Put(int id, CourseUpdateModel course)
        {
            return _courseService.PutCourse(id, course);
        }

        [HttpDelete]
        public bool Delete(int id)
        {
            return _courseService.DeleteCourse(id);
        }

        [HttpPost("assign")]
        public bool AssignCourseToStudent(int courseId, int studentId)
        {
            return _courseService.AssignCourseToStudent(courseId, studentId);
        }
    }
}
