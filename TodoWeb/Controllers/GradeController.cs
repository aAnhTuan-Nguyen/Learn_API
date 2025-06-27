using Microsoft.AspNetCore.Mvc;
using TodoWeb.Application.Dtos;
using TodoWeb.Application.Services;

namespace TodoWeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GradeController : ControllerBase
    {
        private readonly IGradeService _gradeService;
        public GradeController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }
        
        [HttpGet("students/{studentID}")]
        public int Get(int studentID)
        {
            throw new NotImplementedException();
        }

        [HttpPost("students/{studentId}/courses/{courseId}")]
        public bool PostGrade(int studentId, int courseId, GradeCreateModel grade)
        {
            return _gradeService.PostGrade(studentId, courseId, grade);
        }
        [HttpPut("students/{studentId}/courses/{courseId}")]
        public bool PutGrade(int studentId, int courseId, GradeUpdateModel grade)
        {
            return _gradeService.PutGrade(studentId, courseId, grade);
        }

    }
}
