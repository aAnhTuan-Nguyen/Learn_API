using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoWeb.Application.ActionFillter;
using TodoWeb.Application.Dtos.StudentDTO;
using TodoWeb.Application.Services;

namespace TodoWeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [TypeFilter(typeof(LogFillter), Arguments = [LogLevel.Error])]
    [Authorize(Roles ="Stud")]

    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public IEnumerable<StudentViewModel> Get(int? id)
        {
            return _studentService.GetStudents(id);
        }
        [HttpGet("{id}")]
        public StudentDetailViewModel GetStudentDetail(int id)
        {
            return _studentService.GetStudentDetail(id);
        }

        // 5/5/2025
        [HttpGet("GetStudent")]
        public IEnumerable<StudentViewModel> GetStudent()
        {
            return _studentService.GetStudent();
        }

        [HttpPost]
        public IActionResult Post(StudentCreateModel student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_studentService.PostStudent(student));
        }

        [HttpPut]
        public bool Put(int id, StudentUpdateModel student)
        {
            return _studentService.PutStudent(id, student);
        }

        [HttpDelete]
        public bool Delete(int id)
        {
            return _studentService.DeleteStudnet(id);
        }
        
    }
}
